using System;

namespace BlazorApp1.Components.Models;

public class HospitalDto
{
    public int pn_Clinic_Code { get; set; }
    public string v_Clinic_Name { get; set; } = string.Empty;
    public int fn_Country_No { get; set; }
    public int fn_Province_No { get; set; }
    public int fn_Area_No { get; set; }
    public string v_Postal_Address { get; set; } = string.Empty;
    public string v_Physical_Address { get; set; } = string.Empty;
    public string v_Tel1 { get; set; } = string.Empty;
    public string v_Tel2 { get; set; } = string.Empty;
    public string v_Mobile1 { get; set; } = string.Empty;
    public string v_Mobile2 { get; set; } = string.Empty;
    public string v_Fax { get; set; } = string.Empty;
    public string v_Email { get; set; } = string.Empty;
    public string v_Website { get; set; } = string.Empty;
}
