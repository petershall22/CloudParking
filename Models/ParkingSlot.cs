using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace CloudParking.Models
{
    public class ParkingSlot
    {
        public ObjectId Id { get; set; }
        public string SlotNumber { get; set; } = string.Empty;
        public bool IsOccupied { get; set; }
        public DateTime? OccupiedAt { get; set; }
        public DateTime? FreeAt { get; set; }
        public string? UserId { get; set; }
        public int ParkingLotId { get; set; }
        public string LatLong { get; set; } = string.Empty;
        public int HourlyRate { get; set; }
    }
}
