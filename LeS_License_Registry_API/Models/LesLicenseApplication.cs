using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LeS_License_Registry_API.Models;

public partial class LesLicenseApplication
{
    [Key]
    public int licenseapplicationid { get; set; }

    public int? licenseid { get; set; }

    public string? application_name { get; set; }

    public string? application_version { get; set; }

    public DateTime? last_accessed_date { get; set; }

    public string? last_accessed_ip { get; set; }
    public bool isactive {  get; set; }
}
