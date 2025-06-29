using System;
using BlazorApp1.Components.Models;

namespace BlazorApp1.Components.Core.Mapping;

public interface IDepartmentMapper
{
    DepartmentModel ToModel(DepartmentDto dto);
    DepartmentDto ToDto(DepartmentModel model);
    List<DepartmentModel> ToModelList(List<DepartmentDto> dtos);
}

public class DepartmentMapper : IDepartmentMapper
{
    public DepartmentModel ToModel(DepartmentDto dto)
    {
        return new DepartmentModel
        {
            Department_No = dto.pn_Department_No.ToString(),
            Company_No = dto.fn_Company_No.ToString(),
            Clinic_Branch_No = dto.fn_Clinic_Branch_No.ToString(),
            Department_name = dto.v_Department_name
        };
    }

    public DepartmentDto ToDto(DepartmentModel model)
    {
        return new DepartmentDto
        {
            pn_Department_No = int.TryParse(model.Department_No, out var deptNo) ? deptNo : 0,
            fn_Company_No = int.TryParse(model.Company_No, out var compNo) ? compNo : 0,
            fn_Clinic_Branch_No = int.TryParse(model.Clinic_Branch_No, out var branchNo) ? branchNo : 0,
            v_Department_name = model.Department_name
        };
    }

    public List<DepartmentModel> ToModelList(List<DepartmentDto> dtos)
    {
        return dtos?.Select(ToModel).ToList() ?? new List<DepartmentModel>();
    }

}
