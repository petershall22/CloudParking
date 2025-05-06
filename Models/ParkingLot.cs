using MongoDB.Bson;

namespace CloudParking.Models
{
    public class ParkingLot
    {
        public ObjectId Id { get; set; }
        required public string Name { get; set; }
        required public List<ObjectId> OwnedParkingSlots { get; set; }
        required public decimal Latitude { get; set; }
        required public decimal Longitude { get; set; }

    }
}
