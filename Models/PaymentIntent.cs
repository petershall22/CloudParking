using MongoDB.Bson;

namespace CloudParking.Models
{
    public class PaymentIntent
    {
        public ObjectId Id { get; set; }
        public string? StripePaymentIntentId { get; set; }
        required public string UserId { get; set; }
        public int Amount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public bool IsPaid { get; set; } = false;
    }
}
