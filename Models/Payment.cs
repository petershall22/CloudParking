namespace CloudParking.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int Amount { get; set; } 
        public DateTime? PaymentDate { get; set; }
        public bool IsPaid { get; set; } = false;
    }
}
