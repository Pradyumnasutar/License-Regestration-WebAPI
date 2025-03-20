namespace LeS_License_Registry_API.Models
{
    public class LicenseUpdateModal
    {
        public int LicenseId { get; set; }  // Maps to customerId in frontend
        public string? Status { get; set; }
        public string? Remarks { get; set; }
        public int warningcount {  get; set; }  
        public int warningdays {  get; set; }

        public DateTime? ExpiryDate { get; set; } // Ensure proper date handling
    }
}
