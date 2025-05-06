using System.Security.Claims;
using CloudParking.Services.Parking;
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
    public class ParkingLotController : ControllerBase
    {
        private readonly IMongoClient _client;
        private readonly ParkingService _parkingService; // Add a ParkingService instance  

        public ParkingLotController(IMongoClient client, ParkingService parkingService)
        {
            _client = client;
            _parkingService = parkingService;  
        }

        
    }
}
