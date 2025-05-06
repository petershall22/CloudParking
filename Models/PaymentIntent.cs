using MongoDB.Bson;

namespace CloudParking.Models
{
    public class PaymentIntent
    {
        public ObjectId Id { get; set; }
        public string UserId { get; set; }
        public int Amount { get; set; } // in pennies
        public DateTime? PaymentDate { get; set; }
        public bool IsPaid { get; set; } = false;
        public DateTime? PayBy { get; set; }
    }
}
