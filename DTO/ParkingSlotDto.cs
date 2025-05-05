using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CloudParking.DTO
{
    public class ParkingSlotDto
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string SlotNumber { get; set; } = string.Empty;
        public bool IsOccupied { get; set; }
        public DateTime? OccupiedAt { get; set; }
        public DateTime? FreeAt { get; set; }
        public string? UserId { get; set; }
        public int ParkingLotId { get; set; }
        public string ParkingLotName { get; set; } = string.Empty;
        public string LatLong { get; set; } = string.Empty;         
    }
}
