using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CloudParking.Controllers.Parking
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ParkingSlotController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetSlots()
        {
            string? username = User.Identity?.Name;
            Console.WriteLine($"User {username} is accessing parking slots.");
            // Implement logic to get parking slots
            return Ok(new { slots = new List<string> { "Slot1", "Slot2" } });
        }
        [HttpGet("available")]
        public IActionResult GetAvailableSlots()
        {
            // Implement logic to get available parking slots
            return Ok(new { availableSlots = new List<string> { "Slot1", "Slot2" } });
        }

        [HttpPost("reserve")]
        public IActionResult ReserveSlot([FromBody] string slotId)
        {
            // Implement logic to reserve a parking slot
            return Ok($"Slot {slotId} reserved successfully");
        }

        [HttpPost("cancel")]
        public IActionResult CancelReservation([FromBody] string slotId)
        {
            // Implement logic to cancel a parking slot reservation
            return Ok($"Reservation for slot {slotId} cancelled successfully");
        }

        [HttpPost("check-in")]
        public IActionResult CheckIn([FromBody] string slotId)
        {
            // Implement logic for check-in
            return Ok($"Checked in to slot {slotId} successfully");
        }
        [HttpPost("check-out")]
        public IActionResult CheckOut([FromBody] string slotId)
        {
            // Implement logic for check-out
            return Ok($"Checked out of slot {slotId} successfully");
        }
    }
}
