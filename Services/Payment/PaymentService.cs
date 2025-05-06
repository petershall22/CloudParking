using CloudParking.Models;
using MongoDB.Driver;

namespace CloudParking.Services.Payment
{
    public class PaymentService
    {
        private readonly IMongoClient _client;
        private IMongoDatabase _database;
        public PaymentService(IMongoClient client)
        {
            _client = client;
            _database = _client.GetDatabase("CloudParking");
        }

        public async Task<PaymentIntent> CreatePaymentIntent(string userId, int amount) 
        {
            PaymentIntent newPaymentIntent = new PaymentIntent();
            newPaymentIntent.Amount = amount
            newPaymentIntent.PayBy = DateTime.UtcNow.AddDays(5);
            newPaymentIntent.UserId = userId

            var collection = _database.GetCollection<PaymentIntent>("payment_intent");
            await collection.InsertOneAsync(newPaymentIntent);
            return newPaymentIntent;

        }

        public async Task<bool> CancelPaymentIntent(string paymentIntentId)
        {
            var collection = _database.GetCollection<PaymentIntent>("payment_intent");
            var document = collection.FindOneAndDelete(Builders<PaymentIntent>.Filter.Eq(pi => pi.Id.ToString(), paymentIntentId))
            return document != null;
         }

    }
}
