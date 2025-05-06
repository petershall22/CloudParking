namespace CloudParking.Models
{
    public class User
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int OverdueBalance { get; set; } = 0;
        public string LicensePlate { get; set; } = string.Empty;
    }
}
