using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CloudParking.Controllers.Payment
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        [HttpPost("initiate")]
        public IActionResult InitiatePayment([FromBody] string value)
        {
            // Implement payment initiation logic here
            return Ok("Payment initiated successfully");
        }
           
    }
}
