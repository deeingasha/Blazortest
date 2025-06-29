using System;

namespace BlazorApp1.Components.Models;

public class BankDto
{
    public int pn_Bank_No { get; set; }
    public string v_Bank_Name { get; set; } = string.Empty;
    public string v_Bank_Code { get; set; } = string.Empty;

}
public class BankBranchDto
{
    public int pn_Branch_No { get; set; }
    public int fn_Bank_No { get; set; }
    public string v_Branch_Name { get; set; } = string.Empty;
    public string v_Branch_Code { get; set; } = string.Empty;
}
