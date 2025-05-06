using System.Security.Claims;
using CloudParking.Models;
using CloudParking.Services.Parking;
using CloudParking.Services.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using MongoDB.Driver;

namespace CloudParking.Controllers.Parking
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ParkingSlotController : ControllerBase
    {
        private readonly IMongoClient _client;
        private readonly ParkingService _parkingService; // Add a ParkingService instance  
        private readonly PaymentService _paymentService; // Add a PaymentService instance

        public ParkingSlotController(IMongoClient client, ParkingService parkingService, PaymentService paymentService)
        {
            _client = client;
            _parkingService = parkingService;  
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSlots()
        {
            var slots = await _parkingService.GetAllSlots();   
            return Ok(slots);
        }

        [HttpGet("available")]
        public IActionResult GetAvailableSlots()
        {
            var slots = _parkingService.GetAvailableSlots();
            return Ok(slots);
        }

        [HttpPost("initiate-reservation")]
        public async Task<IActionResult> ReserveSlot([FromBody] string slotId, TimeSpan duration)
        {
            string? oid = User.GetObjectId();

            if (oid == null)
            {
                return NotFound("Malformed token, no object ID found");
            }

            if (!await _parkingService.IsSlotAvailable(slotId))
            {
                return NotFound($"Slot {slotId} not available.");
            }

            ParkingSlot parkingSlot = await _parkingService.GetSlotById(slotId);
            parkingSlot.UserId = oid;
            parkingSlot.IsOccupied = false;
            parkingSlot.OccupiedAt = DateTime.UtcNow;
            parkingSlot.FreeAt = DateTime.UtcNow + duration;
             
            await _parkingService.UpdateSlot(id: parkingSlot.Id, parkingSlot);

            ParkingSlot slot = await _parkingService.GetSlotById(slotId);
            int amount = (int)duration.TotalHours * slot.HourlyRate;
            var paymentIntentSecret = _paymentService.CreatePaymentIntent(oid, amount);

            return Ok(paymentIntentSecret);
        }

        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn([FromBody] string slotId)
        {
            string? oid = User.GetObjectId();

            if (oid == null)
            {
                return NotFound("Malformed token, no object ID found");
            }

            if (!await _parkingService.CheckSlotOwned(slotId, oid))
            {
                return NotFound($"Slot {slotId} not found or not reserved by you.");
            }
            Models.ParkingSlot parkingSlot = await _parkingService.GetSlotById(slotId);
            parkingSlot.IsOccupied = true;
            parkingSlot.OccupiedAt = DateTime.UtcNow;
            parkingSlot.FreeAt = DateTime.UtcNow + duration;
     
            Models.ParkingSlot result = await _parkingService.UpdateSlot(id: parkingSlot.Id.ToString(), parkingSlot);
            return Ok($"Checked into slot {slotId} successfully, free at {result.FreeAt}");
        }

        [HttpPost("check-out")]
        public async Task<IActionResult> CheckOut([FromBody] string slotId)
        {
            string? oid = User.GetObjectId();

            if (oid == null)
            {
                return NotFound("Malformed token, no object ID found");
            }

            if (!await _parkingService.CheckSlotOwned(slotId, oid))
            {
                return NotFound($"Slot {slotId} not found or not reserved by you.");
            }
            if (!await _parkingService.IsSlotAvailable(slotId))
            {
                return NotFound($"Slot {slotId} not occupied.");
            }
            ParkingSlot slot = await _parkingService.GetSlotById(slotId);
            TimeSpan duration = DateTime.UtcNow - (slot.OccupiedAt ?? DateTime.UtcNow);
            int amount = (int)duration.TotalHours * slot.HourlyRate;
            var paymentIntent = _paymentService.CreatePaymentIntent(oid, amount);
            Models.ParkingSlot result = await _parkingService.CancelSlot(slotId);
            return Ok("Checked out");
        }
    }
}
