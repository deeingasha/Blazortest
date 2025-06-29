using BlazorApp1.Components.Core.Http;
using BlazorApp1.Components.Models;

namespace BlazorApp1.Components.Services;

public interface ILpoService
{
    Task<List<LpoModel>> GetAllLposAsync();
    Task<LpoModel?> GetLpoAsync(string id);
    Task<string> SaveLpoAsync(LpoModel lpo);
    Task<List<string>> GetLpoNumbersAsync(string supplierId);
    Task<List<SupplierModel>> GetSuppliersAsync();
    Task SendLpoToSupplierAsync(LpoModel lpo);
    Task GenerateLpoPdfAsync(LpoModel lpo);

}

public class LpoService : ILpoService
{
    private readonly IApiHttpClient _apiClient;
    private readonly ILogger<LpoService> _logger;
    private const string SupplierCacheKey = "Suppliers";

    // Keep mock data for ReceiveDrugs.razor
    private static readonly List<LpoModel> _mockLpos = new()
    {
        new LpoModel
        {
            LpoNo = "LPO001",
            SupplierId = "1",
            SupplierName = "ABC Pharmaceuticals",
            LpoDate = DateTime.Today.AddDays(-5),
            Remarks = "Regular monthly order",
            IsApproved = true,
            Items = new List<LpoItemModel>
            {
                new()
                {
                    ItemNo = "1",
                    DrugNo = "D001",
                    DrugName = "Paracetamol 500mg",
                    UnitPrice = 10,
                    Quantity = 1000,

                },
                new()
                {
                    ItemNo = "2",
                    DrugNo = "D002",
                    DrugName = "Amoxicillin 250mg",
                    UnitPrice = 15,
                    Quantity = 500,

                }
            }
        },
        new LpoModel
        {
            LpoNo = "LPO002",
            SupplierId = "2",
            SupplierName = "XYZ Medical Supplies",
            LpoDate = DateTime.Today.AddDays(-2),
            Remarks = "Emergency order",
            IsApproved = true,
            Items = new List<LpoItemModel>
            {
                new()
                {
                    ItemNo = "1",
                    DrugNo = "D003",
                    DrugName = "Ibuprofen 400mg",
                    UnitPrice = 8,
                    Quantity = 2000,

                }
            }
        }
    };

    public LpoService(IApiHttpClient apiClient, ILogger<LpoService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<List<LpoModel>> GetAllLposAsync()
    {
        await Task.Delay(300);
        return _mockLpos;
    }

    public async Task<List<string>> GetLpoNumbersAsync(string supplierId)
    {
        try
        {
            _logger.LogInformation("Fetching LPO numbers for supplier {SupplierId}", supplierId);
            var lpos = await _apiClient.GetAsync<List<LpoListDto>>($"api/DrugDetails/LoadLPOList/{supplierId}");
            // return lpos?.Select(l => l.pfv_LPO_No).Distinct().ToList() ?? new List<string>();

            if (lpos == null || !lpos.Any())
            {
                _logger.LogInformation("No LPOs found for supplier {SupplierId}", supplierId);
                return new List<string>();
            }

            return lpos?
                        .Select(l => l.pfv_LPO_No)
                        .Where(l => !string.IsNullOrEmpty(l))
                        .Distinct()
                        .OrderByDescending(l => l) // Sort by LPO number descending
                        .ToList() ?? new List<string>();
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No LPOs found for supplier {SupplierId}", supplierId);
            return new List<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching LPO numbers for supplier {SupplierId}", supplierId);
            return new List<string>();
        }
    }

    public async Task<LpoModel?> GetLpoAsync(string id)
    {
        try
        {
            _logger.LogInformation("Fetching LPO details for {Id}", id);
            var items = await _apiClient.GetAsync<List<LpoDetailsDto>>($"api/DrugDetails/LPODetails/{id}");

            if (items == null || !items.Any())
            {
                _logger.LogWarning("No LPO details found for {Id}", id);
                return null;
            }

            // Take first item for header info
            var firstItem = items.First();
            var lpoModel = new LpoModel
            {
                LpoNo = firstItem.pfv_LPO_No,
                SupplierId = firstItem.fn_Supplier_No.ToString(),
                LpoDate = firstItem.d_Entry_Date,
                IsApproved = firstItem.fn_LPO_Status_No == 7, // Assuming 7 is approved
                Items = new List<LpoItemModel>()
            };
            // Map all items
            foreach (var item in items)
            {
                if (item.fn_Drug_No.HasValue)
                {
                    lpoModel.Items.Add(new LpoItemModel
                    {
                        ItemNo = item.fn_Drug_No?.ToString() ?? string.Empty,
                        DrugNo = item.fn_Drug_No?.ToString() ?? string.Empty,
                        DrugName = item.itemName,
                        UnitPrice = item.n_Unit_Price ?? 0,
                        Quantity = item.n_Ordered_Qty ?? 0,
                    });
                }
            }

            _logger.LogInformation("Retrieved LPO with {Count} items", lpoModel.Items.Count);
            return lpoModel;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching LPO details for {Id}", id);
            return null;
        }
    }

    public async Task<string> SaveLpoAsync(LpoModel lpo)
    {
        try
        {
            var saveDto = new SaveLpoDto
            {
                d_LPO_Date = lpo.LpoDate,
                v_LPO_Remarks = lpo.Remarks,
                fn_Supplier_No = int.Parse(lpo.SupplierId),
                fn_Prepared_By = 7, // TODO: Get from current user
                fn_Company_No = 1,  // TODO: Get from configuration
                fn_Approved_By = 0,
                // v_LPO_Status = lpo.IsApproved ? "Approved" : "",
                fn_LPO_Status_No = lpo.IsApproved ? 7 : 8, // Assuming 7 is approved, 8 is pending
                fn_Department_No = 4 // TODO: Get from configuration
            };

            var response = await _apiClient.PostAsync<SaveLpoDto, SaveLpoDto>("api/DrugDetails/SaveLPO", saveDto);
            if (response == null) throw new Exception("Failed to save LPO");

            // Save LPO items
            foreach (var item in lpo.Items)
            {
                var itemDto = new SaveLpoItemDto
                {
                    pfv_LPO_No = response.pv_LPO_No,
                    pn_Item_No = int.Parse(item.ItemNo),
                    fn_Drug_No = int.Parse(item.DrugNo),
                    n_Unit_Price = item.UnitPrice,
                    n_Ordered_Qty = item.Quantity,
                    n_Total_Price = item.Total
                };

                await _apiClient.PostAsync<SaveLpoItemDto, SaveLpoItemDto>(
                    "api/DrugDetails/SaveLPODetails", itemDto);
            }

            return response.pv_LPO_No;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving LPO");
            throw;
        }
    }

    public async Task<List<SupplierModel>> GetSuppliersAsync()
    {
        try
        {
            _logger.LogInformation("Fetching suppliers from API");
            var dtos = await _apiClient.GetAsync<List<SupplierDto>>("API/DrugDetails/Supplier");

            return dtos?.Select(dto => new SupplierModel
            {
                SupplierId = dto.pn_Entity_No.ToString(),
                SupplierName = dto.v_FName,
                Email = string.Empty // TODO: API doesn't provide email yet
            }).ToList() ?? new List<SupplierModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching suppliers");
            return new List<SupplierModel>();
        }
    }

    public async Task SendLpoToSupplierAsync(LpoModel lpo)
    {
        // TODO: Implement email sending logic
        _logger.LogInformation("Sending LPO to supplier: {Email}", lpo.SupplierEmail);
    }

    public async Task GenerateLpoPdfAsync(LpoModel lpo)
    {
        // TODO: Implement PDF generation
        _logger.LogInformation("Generating PDF for LPO: {Id}", lpo.LpoNo);
    }
}
