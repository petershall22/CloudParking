using CloudParking.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CloudParking.Services.Parking
{
    public class ParkingService
    {
        private readonly IMongoClient _client;
        private IMongoDatabase _database;
        public ParkingService(IMongoClient client)
        {
            _client = client;
            _database = _client.GetDatabase("CloudParking");
        }
        public async Task UpdateExpiredSlots()
        {
            var collection = _database.GetCollection<ParkingSlot>("slots");
            var filter = Builders<ParkingSlot>.Filter.Eq(slot => slot.IsOccupied, true) &
                         Builders<ParkingSlot>.Filter.Lt(slot => slot.FreeAt, DateTime.UtcNow);
            var update = Builders<ParkingSlot>.Update
                .Set(slot => slot.IsOccupied, false)
                .Set(slot => slot.OccupiedAt, null)
                .Set(slot => slot.FreeAt, null)
                .Set(slot => slot.UserId, null);
            await collection.UpdateManyAsync(filter, update);
        }
        public async Task<ObjectId> CreateParkingLot(ParkingLot parkingLot)
        {
            var collection = _database.GetCollection<ParkingLot>("lots");
            await collection.InsertOneAsync(parkingLot);
            return parkingLot.Id;
        }
        public async Task<List<ParkingSlot>> GetAllSlots()
        {
            var collection = _database.GetCollection<ParkingSlot>("slots");
            return await collection.Find(_ => true).ToListAsync();
        }
        public async Task<List<ParkingSlot>> GetAvailableSlots()
        {

            var collection = _database.GetCollection<ParkingSlot>("slots");
            return await collection.Find(x => !x.IsOccupied).ToListAsync();
        }
        public async Task<ParkingSlot> GetSlotById(string id)
        {
            var collection = _database.GetCollection<ParkingSlot>("slots");
            return await collection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();
        }
        public async Task<bool> IsSlotAvailable(string id)
        {
            var collection = _database.GetCollection<ParkingSlot>("slots");
            var filter = Builders<ParkingSlot>.Filter.Eq(slot => slot.Id.ToString(), id) &
                         Builders<ParkingSlot>.Filter.Eq(slot => slot.IsOccupied, false);
            var result = await collection.Find(filter).FirstOrDefaultAsync();
            return result != null;
        }
        public async Task<ObjectId> AddSlot(ParkingSlot parking)
        {
            var slotCollection = _database.GetCollection<ParkingSlot>("slots");
            var lotCollection = _database.GetCollection <ParkingLot>("lots");
            await slotCollection.InsertOneAsync(parking);

            var filter = Builders<ParkingLot>.Filter.Eq(lot => lot.Id, parking.ParkingLotId);
            var update = Builders<ParkingLot>.Update.Push<ObjectId>(e => e.OwnedParkingSlots, parking.Id);
            await lotCollection.UpdateOneAsync(filter, update); // add parking slot id to the parking lot
            
            return parking.Id;
        }
        public async Task<bool> CheckSlotOwned(string slotId, string userId)
        {
            var collection = _database.GetCollection<ParkingSlot>("slots");
            var filter = Builders<ParkingSlot>.Filter.Eq(slot => slot.Id.ToString(), slotId) &
                         Builders<ParkingSlot>.Filter.Eq(slot => slot.UserId, userId);
            var result = await collection.Find(filter).FirstOrDefaultAsync();
            return result != null;
        }
        public async Task<ParkingSlot> ReserveSlot(string slotId, string userId, int reserveTime)
        {
            var collection = _database.GetCollection<ParkingSlot>("slots");
            var filter = Builders<ParkingSlot>.Filter.Eq(slot => slot.Id.ToString(), slotId);
            var update = Builders<ParkingSlot>.Update
                .Set(slot => slot.OccupiedAt, DateTime.UtcNow)
                .Set(slot => slot.FreeAt, DateTime.UtcNow.AddMinutes(reserveTime))
                .Set(slot => slot.UserId, userId);
            await collection.UpdateOneAsync(filter, update);
            return await GetSlotById(slotId);
        }
        public async Task<ParkingSlot> CancelSlot(string slotId)
        {
            var collection = _database.GetCollection<ParkingSlot>("slots");
            var filter = Builders<ParkingSlot>.Filter.Eq(slot => slot.Id.ToString(), slotId);
            var update = Builders<ParkingSlot>.Update
                .Set(slot => slot.IsOccupied, false)
                .Set(slot => slot.OccupiedAt, null)
                .Set(slot => slot.FreeAt, null)
                .Set(slot => slot.UserId, null);
            await collection.UpdateOneAsync(filter, update);
            return await GetSlotById(slotId);
        }
        public async Task<ParkingSlot> UpdateSlot(string id, ParkingSlot parking)
        {
            var collection = _database.GetCollection<ParkingSlot>("slots");
            var filter = Builders<ParkingSlot>.Filter.Eq(slot => slot.Id.ToString(), id);
            await collection.ReplaceOneAsync(filter, parking);
            return parking;
        }
    }
}
    