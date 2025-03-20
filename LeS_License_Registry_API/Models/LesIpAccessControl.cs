using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LeS_License_Registry_API.Models;

public partial class LesIpAccessControl
{
    [Key]
    public int ipaccessid { get; set; }

    public string? ip_address { get; set; }

    public string? access_type { get; set; }

    public string? remarks { get; set; }

    public DateTime? created_date { get; set; }

    public DateTime? updated_date { get; set; }
}
