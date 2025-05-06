namespace CloudParking.Models
{
    public class User
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public List<string> OwnedParkingSlots { get; set; } = new List<string>();
        public List<string> PaymentIntents { get; set; } = new List<string>();
        public List<string> ParkingLots { get; set; } = new List<string>();
    }
}
