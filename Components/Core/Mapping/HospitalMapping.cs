using System;
using BlazorApp1.Components.Models;

namespace BlazorApp1.Components.Core.Mapping;

public interface IHospitalMapper
{
    // Convert one hospital from API format to our format
    HospitalFormModel ToModel(HospitalDto dto);

    // Convert one hospital from our format to API format
    HospitalDto ToDto(HospitalFormModel model);

    // Convert a list of hospitals from API format to our format
    List<HospitalFormModel> ToModelList(List<HospitalDto> dtos);

    IReadOnlyList<string> GetCountries();
    IReadOnlyList<string> GetProvinces();
    IReadOnlyList<string> GetAreas();
}

public class HospitalMapper : IHospitalMapper
{
    private readonly Dictionary<int, string> _countryMap;
    private readonly Dictionary<int, string> _provinceMap;
    private readonly Dictionary<int, string> _areaMap;

    public HospitalMapper()
    {
        _countryMap = new Dictionary<int, string>
        {
            { 1, "Kenya" },
            { 2, "Uganda" },
            { 3, "Tanzania" },
        };
        _provinceMap = new Dictionary<int, string>
        {
            { 1, "Nairobi" },
            { 2, "Mombasa" },
            { 3, "Kisumu" },
            { 4, "Nakuru" },
        };
        _areaMap = new Dictionary<int, string>
        {
            { 1, "Upper Hill" },
            { 2, "Parklands" },
            { 3, "CBD" },
            { 4, "Westlands" },
        };
    }
    public IReadOnlyList<string> GetCountries() => _countryMap.Values.ToList();
    public IReadOnlyList<string> GetProvinces() => _provinceMap.Values.ToList();
    public IReadOnlyList<string> GetAreas() => _areaMap.Values.ToList();

    public HospitalFormModel ToModel(HospitalDto dto)
    {
        return new HospitalFormModel
        {
            Id = dto.pn_Clinic_Code.ToString(),
            HospitalName = dto.v_Clinic_Name,
            PostalAddress = dto.v_Postal_Address,
            PhysicalAddress = dto.v_Physical_Address,
            Tel1 = dto.v_Tel1,
            Tel2 = dto.v_Tel2,
            Mobile1 = dto.v_Mobile1,
            Mobile2 = dto.v_Mobile2,
            Fax = dto.v_Fax,
            Email = dto.v_Email,
            Website = dto.v_Website,

            // TODO: fix in api
            Country = _countryMap.GetValueOrDefault(dto.fn_Country_No, "Unknown"),
            Province = _provinceMap.GetValueOrDefault(dto.fn_Province_No, "Unknown"),
            Area = _areaMap.GetValueOrDefault(dto.fn_Area_No, "Unknown")
        };
    }
    public HospitalDto ToDto(HospitalFormModel model)
    {
        return new HospitalDto
        {
            pn_Clinic_Code = int.TryParse(model.Id, out var id) ? id : 0,
            v_Clinic_Name = model.HospitalName,
            v_Postal_Address = model.PostalAddress,
            v_Physical_Address = model.PhysicalAddress,
            v_Tel1 = model.Tel1,
            v_Tel2 = model.Tel2,
            v_Mobile1 = model.Mobile1,
            v_Mobile2 = model.Mobile2,
            v_Fax = model.Fax,
            v_Email = model.Email,
            v_Website = model.Website,
            fn_Country_No = _countryMap.FirstOrDefault(x => x.Value == model.Country).Key,
            fn_Province_No = _provinceMap.FirstOrDefault(x => x.Value == model.Province).Key,
            fn_Area_No = _areaMap.FirstOrDefault(x => x.Value == model.Area).Key
        };
    }
    public List<HospitalFormModel> ToModelList(List<HospitalDto> dtos)
    {
        return dtos?.Select(ToModel).ToList() ?? new List<HospitalFormModel>();
    }
}


