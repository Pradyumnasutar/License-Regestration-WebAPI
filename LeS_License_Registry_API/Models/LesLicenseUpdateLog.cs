using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LeS_License_Registry_API.Models;

public partial class LesLicenseUpdateLog
{
    [Key]
    public int logid { get; set; }

    public int? licenseid { get; set; }

    public string? license_status { get; set; }

    public DateTime? updated_date { get; set; }

    public string? updated_by { get; set; }
    public string? ipaddress {  get; set; }

    public string? remarks { get; set; }
}
