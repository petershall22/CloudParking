using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CloudParking.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // POST: api/<AuthController>/register
        [HttpPost("register")]
        public IActionResult Register([FromBody] string value)
        {
            
            return Ok("User registered successfully");
        }

        // POST: api/<AuthController>/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] string value)
        {
            // Implement login logic here
            return Ok("User logged in successfully");
        }
        // POST: api/<AuthController>/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Implement logout logic here
            return Ok("User logged out successfully");
        }
        // POST: api/<AuthController>/refresh-token
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] string value)
        {
            // Implement refresh token logic here
            return Ok("Token refreshed successfully");
        }
    }
}
