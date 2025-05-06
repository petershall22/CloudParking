using CloudParking.Services.Parking;
using CloudParking.Services.Payment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ZstdSharp.Unsafe;

namespace CloudParking.Controllers.Payment
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {

        private readonly IMongoClient _client;
        private readonly PaymentService _paymentService; // Add a PaymentService instance  

        public PaymentController(IMongoClient client, PaymentService paymentService)
        {
            _client = client;
            _paymentService = paymentService;
        }

        [HttpPost("initiate")]
        public IActionResult InitiatePayment([FromBody] string value)
        {
            // Implement payment initiation logic here
            return Ok("Payment initiated successfully");
        }
           
    }
}
