using BlazorApp1.Components.Core.Http;
using BlazorApp1.Components.Core.Mapping;
using BlazorApp1.Components.Models;

namespace BlazorApp1.Components.Services;


public interface IHospitalService
{
    Task<List<HospitalFormModel>> GetAllHospitalsAsync();
    Task<HospitalFormModel> GetHospitalAsync(string id);
    Task SaveHospitalAsync(HospitalFormModel hospital);
}

public class HospitalService : IHospitalService
{
    private readonly IApiHttpClient _apiClient;
    private readonly ILogger<HospitalService> _logger;
    private readonly IHospitalMapper _mapper;

    public HospitalService(IApiHttpClient apiClient, ILogger<HospitalService> logger, IHospitalMapper mapper)
    {
        _apiClient = apiClient;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<List<HospitalFormModel>> GetAllHospitalsAsync()
    {
        _logger.LogInformation("Fetching hospitals from API...");
        try
        {
            var apiResponse = await _apiClient.GetAsync<List<HospitalDto>>("api/Hospital/HospitalInfo");
            if (apiResponse == null)
            {
                _logger.LogWarning("API returned null response");
                return new List<HospitalFormModel>();
            }
            _logger.LogInformation($"Received {apiResponse?.Count ?? 0} hospitals from API");
            return _mapper.ToModelList(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching hospitals");
            throw;
        }
    }
    public async Task<HospitalFormModel> GetHospitalAsync(string id)
    {
        try
        {  //TODO: get api
            var dto = await _apiClient.GetAsync<HospitalDto>($"api/Hospital/HospitalInfo/{id}");
            return dto != null
            ? _mapper.ToModel(dto) :
             new HospitalFormModel
             {
                 Id = string.Empty,
                 HospitalName = string.Empty,
             };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching hospital {Id}", id);
            throw;
        }
    }

    public async Task SaveHospitalAsync(HospitalFormModel hospital)
    {
        try
        {
            var dto = _mapper.ToDto(hospital);
            if (string.IsNullOrEmpty(hospital.Id))
            {
                // New hospital -- TODO: get api
                dto.pn_Clinic_Code = await GenerateNextId();
                await _apiClient.PostAsync<HospitalDto, HospitalDto>("api/Hospital/HospitalInfo", dto);
                _logger.LogInformation($"New hospital created with ID {dto.pn_Clinic_Code}");
            }
            else
            {
                // Update existing
                _logger.LogInformation($"Updating hospital with ID {dto.pn_Clinic_Code}");
                await _apiClient.PutAsync<HospitalDto, HospitalDto>($"api/Hospital/EditHospitalInfo/{dto.pn_Clinic_Code}", dto);
                _logger.LogInformation($"Hospital with ID {dto.pn_Clinic_Code} updated successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving hospital {Id}", hospital.Id);
            throw;
        }
    }

    private async Task<int> GenerateNextId()
    {
        try
        {
            var hospitals = await _apiClient.GetAsync<List<HospitalDto>>("api/Hospital/HospitalInfo");
            if (hospitals == null || !hospitals.Any())
            {
                _logger.LogInformation("No existing hospitals found");
                return 1;
            }
            var maxId = hospitals.Select(h => h.pn_Clinic_Code).Max();
            // var numericPart = int.Parse(lastId.Substring(3));
            // var numericPart = int.Parse(maxId.Replace("HOS", ""));
            // return $"HOS{numericPart + 1}";
            _logger.LogInformation($"Generated next ID: {maxId + 1}");
            return maxId + 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating hospital ID");
            throw;
        }
    }

    // TODO: Implement validation for hospital ID format
    // private bool IsValidHospitalId(string id)
    // {
    //     return !string.IsNullOrEmpty(id) &&
    //            id.StartsWith("HOS") &&
    //            int.TryParse(id.Replace("HOS", ""), out _);
    // }

}