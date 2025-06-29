using System;

namespace BlazorApp1.Components.Models;

public class DepartmentDto
{
    public int pn_Department_No { get; set; }
    public int fn_Company_No { get; set; }
    public int fn_Clinic_Branch_No { get; set; }
    public string v_Department_name { get; set; } = string.Empty;

}
