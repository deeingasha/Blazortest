using BlazorApp1.Components.Core.Http;
using BlazorApp1.Components.Core.Mapping;
using BlazorApp1.Components.Models;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorApp1.Components.Services;

public interface IDrugService
{
    Task<List<DrugTypeModel>> GetDrugTypesAsync();
    // Task<List<DrugModel>> GetDrugsAsync(string? searchTerm = null);
    Task<List<DrugModel>> GetDrugsAsync(string? searchTerm = null, string? drugTypeFilter = null);
    Task<PaginatedList<DrugModel>> GetDrugsAsync(int pageIndex, int pageSize, string? searchTerm = null, string? drugTypeFilter = null);
    Task<DrugModel?> GetDrugAsync(string id);
    Task SaveDrugAsync(DrugModel drug);
    Task<List<string>> GetManufacturersAsync();
}

public class DrugService : IDrugService
{
    private readonly IApiHttpClient _apiClient;
    private readonly ILogger<DrugService> _logger;
    private readonly IDrugMapper _mapper;
    private readonly IMemoryCache _cache;
    private const string DrugTypesCacheKey = "DrugTypes";
    private const string DrugListCacheKey = "DrugList";
    private static readonly List<string> _manufacturers = new()
    {
        "GSK", "Pfizer", "Roche", "Novartis", "Merck"
    };

    public DrugService(IApiHttpClient apiClient, ILogger<DrugService> logger, IDrugMapper mapper, IMemoryCache cache)
    {
        _apiClient = apiClient;
        _logger = logger;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<List<DrugTypeModel>> GetDrugTypesAsync()
    {
        try
        {
            return await _cache.GetOrCreateAsync(DrugTypesCacheKey, async entry =>
            {
                entry.SetSlidingExpiration(TimeSpan.FromMinutes(30));
                var dtos = await _apiClient.GetAsync<List<DrugTypeDto>>("api/Hospital/DrugType");
                var types = _mapper.ToTypeModelList(dtos ?? new List<DrugTypeDto>());
                _logger.LogInformation("Cached {Count} drug types", types.Count);
                return types;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching drug types");
            return new List<DrugTypeModel>();
        }
    }

    public async Task<List<DrugModel>> GetDrugsAsync(string? searchTerm = null, string? drugTypeFilter = null)
    {
        // try
        // {
        //     _logger.LogInformation("GetDrugsAsync called with searchTerm: '{SearchTerm}', drugTypeFilter: '{DrugTypeFilter}'",
        //         searchTerm, drugTypeFilter);

        //     // Get drug types from cache and set in mapper
        //     var drugTypes = await GetDrugTypesAsync();
        //     _mapper.SetDrugTypes(drugTypes);

        //     // Get drugs
        //     var dtos = await _apiClient.GetAsync<List<DrugDto>>("API/DrugDetails/DrugList");
        //     if (dtos == null)
        //     {
        //         _logger.LogWarning("No drugs returned from API");
        //         return new List<DrugModel>();
        //     }

        //     _logger.LogInformation("Received {Count} drugs from API", dtos.Count);
        //     var drugs = _mapper.ToModelList(dtos);

        //     // Apply filters
        //     var filtered = FilterDrugs(drugs, searchTerm, drugTypeFilter);
        //     _logger.LogInformation("Returning {Count} filtered drugs", filtered.Count);

        //     return filtered;
        // }
        // catch (Exception ex)
        // {
        //     _logger.LogError(ex, "Error fetching drugs");
        //     return new List<DrugModel>();
        // }

        // Cache the drug list for a short period
        var drugs = await _cache.GetOrCreateAsync(DrugListCacheKey, async entry =>
        {
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(1));
            var dtos = await _apiClient.GetAsync<List<DrugDto>>("API/DrugDetails/DrugList");
            var drugTypes = await GetDrugTypesAsync();
            _mapper.SetDrugTypes(drugTypes);
            return _mapper.ToModelList(dtos ?? new List<DrugDto>());
        });

        return FilterDrugs(drugs, searchTerm, drugTypeFilter);
    }

    public async Task<PaginatedList<DrugModel>> GetDrugsAsync(int pageIndex, int pageSize, string? searchTerm = null, string? drugTypeFilter = null)
    {
        var drugs = await _cache.GetOrCreateAsync(DrugListCacheKey, async entry =>
        {
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(1));
            var dtos = await _apiClient.GetAsync<List<DrugDto>>("API/DrugDetails/DrugList");
            var drugTypes = await GetDrugTypesAsync();
            _mapper.SetDrugTypes(drugTypes);
            return _mapper.ToModelList(dtos ?? new List<DrugDto>());
        });

        var filtered = FilterDrugs(drugs, searchTerm, drugTypeFilter);

        // Apply pagination
        var totalCount = filtered.Count;
        var items = filtered
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PaginatedList<DrugModel>(items, totalCount, pageIndex, pageSize);
    }

    private List<DrugModel> FilterDrugs(List<DrugModel> drugs, string? searchTerm, string? drugTypeFilter)
    {
        _logger.LogInformation("FilterDrugs called with {Count} drugs", drugs.Count);
        var filtered = drugs;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            _logger.LogInformation("Applying search filter: '{SearchTerm}'", searchTerm);
            filtered = filtered.Where(d =>
                d.DrugName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
            _logger.LogInformation("After search filter: {Count} drugs", filtered.Count);
        }

        if (!string.IsNullOrWhiteSpace(drugTypeFilter))
        {
            filtered = filtered.Where(d => d.DrugTypeNo == drugTypeFilter).ToList();
            _logger.LogInformation("After type filter: {Count} drugs, Type: {Type}",
                filtered.Count, drugTypeFilter);
        }

        return filtered;
    }


    public async Task<DrugModel?> GetDrugAsync(string id)
    {
        try
        {
            _logger.LogDebug($"Fetching drug with id {id} from API...");
            // Get drug types first to ensure mapper has them
            var drugTypes = await GetDrugTypesAsync();
            _mapper.SetDrugTypes(drugTypes);


            var dto = await _apiClient.GetAsync<DrugDto>($"API/DrugDetails/DrugList/{id}");
            if (dto == null)
            {
                _logger.LogWarning("No drug found with id {Id}", id);
                return null;
            }

            var drug = _mapper.ToModel(dto);
            _logger.LogInformation("Retrieved drug: DrugNo={DrugNo}, Name={Name}, Type={Type}",
                drug.DrugNo, drug.DrugName, drug.DrugTypeNo);

            return drug;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching drug with id {id}");
            return null;
        }
    }

    public async Task SaveDrugAsync(DrugModel drug)
    {
        _logger.LogInformation("SaveDrugAsync called with drug: {@Drug}", drug);
        try
        {
            if (string.IsNullOrEmpty(drug.DrugNo))
            {
                // Add new drug
                var nextId = await GenerateNextId();
                _logger.LogInformation("Generated next drug ID: {DrugNo}", nextId);

                var saveDto = _mapper.ToSaveDto(drug, nextId);
                _logger.LogInformation("Saving new drug with data: {@DrugData}", saveDto);

                await _apiClient.PostAsync<SaveDrugDto, SaveDrugDto>("API/DrugDetails/SaveDrug", saveDto);
                _logger.LogInformation("Successfully saved new drug");
            }
            else
            {
                // Edit existing drug
                _logger.LogInformation("Editing existing drug with ID: {DrugNo}", drug.DrugNo);
                // Edit existing drug
                _logger.LogWarning("Editing drug - ID: {DrugNo}, Name: {Name}, Type: {Type}",
                    drug.DrugNo, drug.DrugName, drug.DrugTypeNo);


                var saveDto = _mapper.ToSaveDto(drug, int.Parse(drug.DrugNo));
                _logger.LogWarning("Mapped to DTO: {@SaveDto}", saveDto);


                await _apiClient.PutAsync<SaveDrugDto, SaveDrugDto>($"API/DrugDetails/EditDrug/{saveDto.pn_Drug_No}", saveDto);
                _logger.LogInformation("Successfully updated drug");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving drug");
            throw;
        }
    }
    private async Task<int> GenerateNextId()
    {
        try
        {
            var drugs = await _apiClient.GetAsync<List<DrugDto>>("API/DrugDetails/DrugList");
            if (drugs == null || !drugs.Any())
                return 1;

            return drugs.Max(d => d.pn_Drug_No) + 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating next drug ID");
            throw;
        }
    }

    public async Task<List<string>> GetManufacturersAsync()
    {
        // TODO: Replace with API call
        return _manufacturers;
    }
}
