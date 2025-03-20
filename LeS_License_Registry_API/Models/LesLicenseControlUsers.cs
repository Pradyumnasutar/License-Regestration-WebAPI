using System.ComponentModel.DataAnnotations;

namespace LeS_License_Registry_API.Models
{
    public class LesLicenseControlUsers
    {
        [Key]
        public int userid {  get; set; }
        public string username { get; set; } = "";
        public string email { get; set; } = "";
        public string? hashed_password { get; set; }
        public string? hash_salt { get; set; }

    }
}
