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

        public async Task<List<Models.ParkingSlot>> GetAllSlots()
        {
            var collection = _database.GetCollection<Models.ParkingSlot>("slots");
            return await collection.Find(_ => true).ToListAsync();
        }
        public async Task<List<Models.ParkingSlot>> GetAvailableSlots()
        {
            var collection = _database.GetCollection<Models.ParkingSlot>("slots");
            return await collection.Find(x => !x.IsOccupied).ToListAsync();
        }
        public async Task<Models.ParkingSlot> GetSlotById(string id)
        {
            var collection = _database.GetCollection<Models.ParkingSlot>("slots");
            return await collection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();
        }
        public async Task<bool> IsSlotAvailable(string id)
        {
            var collection = _database.GetCollection<Models.ParkingSlot>("slots");
            var filter = Builders<Models.ParkingSlot>.Filter.Eq(slot => slot.Id.ToString(), id) &
                         Builders<Models.ParkingSlot>.Filter.Eq(slot => slot.IsOccupied, false);
            var result = await collection.Find(filter).FirstOrDefaultAsync();
            return result != null;
        }
        public async Task<DTO.ParkingSlotDto> AddSlot(DTO.ParkingSlotDto parking)
        {
            var collection = _database.GetCollection<DTO.ParkingSlotDto>("slots");
            await collection.InsertOneAsync(parking);
            return parking;
        }
        public async Task<bool> CheckSlotOwned(string slotId, string userId)
        {
            var collection = _database.GetCollection<DTO.ParkingSlotDto>("slots");
            var filter = Builders<DTO.ParkingSlotDto>.Filter.Eq(slot => slot.Id.ToString(), slotId) &
                         Builders<DTO.ParkingSlotDto>.Filter.Eq(slot => slot.UserId, userId);
            var result = await collection.Find(filter).FirstOrDefaultAsync();
            return result != null;
        }
        public async Task<Models.ParkingSlot> ReserveSlot(string slotId, string userId, int reserveTime)
        {
            var collection = _database.GetCollection<DTO.ParkingSlotDto>("slots");
            var filter = Builders<DTO.ParkingSlotDto>.Filter.Eq(slot => slot.Id.ToString(), slotId);
            var update = Builders<DTO.ParkingSlotDto>.Update
                .Set(slot => slot.OccupiedAt, DateTime.UtcNow)
                .Set(slot => slot.FreeAt, DateTime.UtcNow.AddMinutes(reserveTime))
                .Set(slot => slot.UserId, userId);
            await collection.UpdateOneAsync(filter, update);
            return await GetSlotById(slotId);
        }
        public async Task<Models.ParkingSlot> CancelSlot(string slotId)
        {
            var collection = _database.GetCollection<DTO.ParkingSlotDto>("slots");
            var filter = Builders<DTO.ParkingSlotDto>.Filter.Eq(slot => slot.Id.ToString(), slotId);
            var update = Builders<DTO.ParkingSlotDto>.Update
                .Set(slot => slot.IsOccupied, false)
                .Set(slot => slot.OccupiedAt, null)
                .Set(slot => slot.FreeAt, null)
                .Set(slot => slot.UserId, null);
            await collection.UpdateOneAsync(filter, update);
            return await GetSlotById(slotId);
        }
        public async Task<Models.ParkingSlot> UpdateSlot(string id, Models.ParkingSlot parking)
        {
            var collection = _database.GetCollection<Models.ParkingSlot>("slots");
            var filter = Builders<Models.ParkingSlot>.Filter.Eq(slot => slot.Id.ToString(), id);
            await collection.ReplaceOneAsync(filter, parking);
            return parking;
        }
    }
}
    