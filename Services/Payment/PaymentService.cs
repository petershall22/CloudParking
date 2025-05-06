using CloudParking.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Stripe;

namespace CloudParking.Services.Payment
{
    public class PaymentService
    {
        private readonly IMongoClient _client;
        private IMongoDatabase _database;
        private string _currency = "GBP";
        private readonly PaymentIntentService _paymentIntentService = new PaymentIntentService();
        public PaymentService(IMongoClient client)
        {
            _client = client;
            _database = _client.GetDatabase("CloudParking");
            StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("StripeCloudParkingSK") ?? throw new Exception("Stripe API key not found in environment variables.");
        }

        public async Task<string> CreatePaymentIntent(string userId, int amount) 
        {
            Models.PaymentIntent newPaymentIntent = new Models.PaymentIntent
            {
                Amount = amount,
                UserId = userId
            };

            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = _currency,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            };

            Stripe.PaymentIntent stripePaymentIntent = _paymentIntentService.Create(options);

            var collection = _database.GetCollection<Models.PaymentIntent>("payment_intent");
            await collection.InsertOneAsync(newPaymentIntent);
            return stripePaymentIntent.ClientSecret;

        }

        public async Task<bool> CompletePaymentIntent(ObjectId objectId, string stripePaymentIntentId, string userId)
        {
            var collection = _database.GetCollection<Models.PaymentIntent>("payment_intent");
            var filter = Builders<Models.PaymentIntent>.Filter.Eq(pi => pi.Id, objectId);
            var update = Builders<Models.PaymentIntent>.Update
                .Set(pi => pi.StripePaymentIntentId, stripePaymentIntentId)
                .Set(pi => pi.IsPaid, true)
                .Set(pi => pi.PaymentDate, DateTime.UtcNow);

            var result = await collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public bool CancelPaymentIntent(string paymentIntentId)
        {
            var collection = _database.GetCollection<Models.PaymentIntent>("payment_intent");
            var document = collection.FindOneAndDelete(Builders<Models.PaymentIntent>.Filter.Eq(pi => pi.Id.ToString(), paymentIntentId));
            return document != null;
         }

    }
}
