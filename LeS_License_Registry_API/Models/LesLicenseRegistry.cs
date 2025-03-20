using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LeS_License_Registry_API.Models;

public partial class LesLicenseRegistry
{
    [Key]
    public int Licenseid { get; set; }

    public string? customer_name { get; set; }

    public string? activation_key { get; set; }

    public bool? bypass_license { get; set; }

    public int? license_period { get; set; }

    public DateTime? activated_date { get; set; }

    public string? activated_by { get; set; }

    public DateTime? expiry_date { get; set; }

    public string? status { get; set; }

    public DateTime? revoked_date { get; set; }

    public string? revoked_by { get; set; }

    public string? remarks { get; set; }

    public string? machine_name { get; set; }
    public int? warningdaysbeforeexpiry{get;set;}
    public DateTime? lastwarningdate { get;set;}
    public int? warningcount { get;set;}

}
public class CustomerDetails
{
    public LesLicenseRegistry? customer { get; set;}
    public List<LesLicenseApplication>? Applications { get; set;}
}
