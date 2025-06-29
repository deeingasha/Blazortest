using System;

using BlazorApp1.Components.Models;

namespace BlazorApp1.Components.Core.Mapping;

public interface IBankMapper
{
    BankModel ToModel(BankDto dto);
    BankDto ToDto(BankModel model);
    List<BankModel> ToModelList(List<BankDto> dtos);
    BankBranchModel ToBranchModel(BankBranchDto dto);
    BankBranchDto ToBranchDto(BankBranchModel model);
    List<BankBranchModel> ToBranchModelList(List<BankBranchDto> dtos);
}

public class BankMapper : IBankMapper
{
    public BankModel ToModel(BankDto dto)
    {
        return new BankModel
        {
            BankNo = dto.pn_Bank_No.ToString(),
            BankName = dto.v_Bank_Name,
            BankCode = dto.v_Bank_Code
        };
    }

    public BankDto ToDto(BankModel model)
    {
        return new BankDto
        {
            pn_Bank_No = int.TryParse(model.BankNo, out var no) ? no : 0,
            v_Bank_Name = model.BankName,
            v_Bank_Code = model.BankCode
        };
    }

    public List<BankModel> ToModelList(List<BankDto> dtos)
    {
        return dtos?.Select(ToModel).ToList() ?? new List<BankModel>();
    }

    public BankBranchModel ToBranchModel(BankBranchDto dto)
    {
        return new BankBranchModel
        {
            BranchNo = dto.pn_Branch_No.ToString(),
            BankNo = dto.fn_Bank_No.ToString(),
            BranchName = dto.v_Branch_Name,
            BranchCode = dto.v_Branch_Code
        };
    }

    public BankBranchDto ToBranchDto(BankBranchModel model)
    {
        return new BankBranchDto
        {
            pn_Branch_No = int.TryParse(model.BranchNo, out var no) ? no : 0,
            fn_Bank_No = int.TryParse(model.BankNo, out var bankNo) ? bankNo : 0,
            v_Branch_Name = model.BranchName,
            v_Branch_Code = model.BranchCode
        };
    }

    public List<BankBranchModel> ToBranchModelList(List<BankBranchDto> dtos)
    {
        return dtos?.Select(ToBranchModel).ToList() ?? new List<BankBranchModel>();
    }
}
