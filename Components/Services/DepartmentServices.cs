using System;
using BlazorApp1.Components.Models;
using BlazorApp1.Components.Core.Http;
using BlazorApp1.Components.Core.Mapping;
using System.Text.Json;

namespace BlazorApp1.Components.Services;

public interface IDepartmentService
{

    Task<List<DepartmentModel>> GetAllDepartmentsAsync();
    Task<DepartmentModel> GetDepartmentAsync(string id);
    Task SaveDepartmentAsync(DepartmentModel department);
}

public class DepartmentService : IDepartmentService
{
    private readonly IApiHttpClient _apiClient;
    private readonly ILogger<DepartmentService> _logger;
    private readonly IDepartmentMapper _mapper;

    public DepartmentService(IApiHttpClient apiClient,
                               ILogger<DepartmentService> logger,
                               IDepartmentMapper mapper)
    {
        _apiClient = apiClient;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<List<DepartmentModel>> GetAllDepartmentsAsync()
    {
        try
        {
            _logger.LogDebug("Fetching departments from API...");
            var apiResponse = await _apiClient.GetAsync<List<DepartmentDto>>("api/Hospital/DepartmentInfo");

            if (apiResponse == null)
            {
                _logger.LogWarning("API returned null response");
                return new List<DepartmentModel>();
            }

            var departments = _mapper.ToModelList(apiResponse);
            _logger.LogInformation($"Successfully retrieved {departments.Count} departments");
            return departments;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching departments");
            throw;
        }
    }

    public async Task<DepartmentModel> GetDepartmentAsync(string id)
    {
        try
        {
            var dto = await _apiClient.GetAsync<DepartmentDto>($"api/Hospital/DepartmentInfo/{id}");
            if (dto == null)
            {
                _logger.LogWarning($"Department with ID {id} not found");
                return new DepartmentModel
                {
                    Department_No = string.Empty,
                    Company_No = string.Empty,
                    Clinic_Branch_No = string.Empty,
                    Department_name = string.Empty
                };
            }

            return _mapper.ToModel(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching department {Id}", id);
            throw;
        }
    }

    public async Task SaveDepartmentAsync(DepartmentModel department)
    {
        try
        {
            var dto = _mapper.ToDto(department);
            if (string.IsNullOrEmpty(department.Department_No))
            {
                // New department
                dto.pn_Department_No = await GenerateNextId();
                department.Department_No = dto.pn_Department_No.ToString();
                await _apiClient.PostAsync<DepartmentDto, DepartmentDto>("api/Hospital/PostDepartmentInfo", dto);
                _logger.LogInformation("Created new department with ID {Id}", dto.pn_Department_No);
            }
            else
            {
                // Update existing
                var response = await _apiClient.PutAsync<DepartmentDto, DepartmentDto>(
                    $"api/Hospital/PostDepartmentInfo/{dto.pn_Department_No}",
                    dto);
                if (response == null)
                {
                    throw new Exception($"Failed to update department {dto.pn_Department_No}");
                }

                // Update the model with response data
                department.Department_name = response.v_Department_name;
                department.Company_No = response.fn_Company_No.ToString();
                department.Clinic_Branch_No = response.fn_Clinic_Branch_No.ToString();

                _logger.LogInformation("Updated department {Id}, name: {Name}",
                    response.pn_Department_No,
                    response.v_Department_name);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving department {Id}", department.Department_No);
            throw;
        }
    }
    private async Task<int> GenerateNextId()
    {
        try
        {
            _logger.LogDebug("Generating new department ID...");
            var departments = await _apiClient.GetAsync<List<DepartmentDto>>("api/Hospital/DepartmentInfo");
            if (departments == null || !departments.Any())
            {
                return 1;
            }
            return departments.Max(d => d.pn_Department_No) + 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating next department ID");
            throw;
        }
    }

}