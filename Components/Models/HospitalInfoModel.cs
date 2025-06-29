using System;

namespace BlazorApp1.Components.Models;

public class HospitalFormModel
{
    public required string Id { get; set; }
    public required string HospitalName { get; set; }

    /// Default parameterless constructor
    public HospitalFormModel()
    {
        Id = string.Empty;
        HospitalName = string.Empty;
    }
    public string Country { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string PostalAddress { get; set; } = string.Empty;
    public string PhysicalAddress { get; set; } = string.Empty;
    public string Tel1 { get; set; } = string.Empty;
    public string Tel2 { get; set; } = string.Empty;
    public string Mobile1 { get; set; } = string.Empty;
    public string Mobile2 { get; set; } = string.Empty;
    public string Fax { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

}
