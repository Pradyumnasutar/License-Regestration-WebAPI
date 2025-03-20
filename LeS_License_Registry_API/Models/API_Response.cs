namespace LeS_License_Registry_API.Models
{
    public class API_Response
    {
        public object? data { get; set; }
        public bool isSuccess { get; set; }
        public int totalRecords { get; set; } = 0;
        public string? message { get; set; }
    }
}
