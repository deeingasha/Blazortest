using System;

namespace BlazorApp1.Components.Models;

public class BankModel
{
    public string BankNo { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string BankCode { get; set; } = string.Empty;
    public List<BankBranchModel> Branches { get; set; } = new();
}

public class BankBranchModel
{
    public string BranchNo { get; set; } = string.Empty;
    public string BankNo { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string BranchCode { get; set; } = string.Empty;
}
