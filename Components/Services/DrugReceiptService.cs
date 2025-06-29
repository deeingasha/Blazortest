using System;
using BlazorApp1.Components.Core.Http;
using BlazorApp1.Components.Models;

namespace BlazorApp1.Components.Services;

public interface IDrugReceiptService
{
    Task<List<DrugReceiptModel>> GetAllReceiptsAsync();
    Task<DrugReceiptModel?> GetReceiptAsync(string id);
    Task SaveReceiptAsync(DrugReceiptModel receipt);
    Task SaveReceiptItemAsync(string receiptNo, DrugReceiptItemModel item);
}

public class DrugReceiptService : IDrugReceiptService
{
    private readonly IApiHttpClient _apiClient;
    private readonly ILogger<DrugReceiptService> _logger;

    public DrugReceiptService(IApiHttpClient apiClient, ILogger<DrugReceiptService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    // Add mock data
    private static readonly List<DrugReceiptModel> _mockReceipts = new()
    {
        new DrugReceiptModel
        {
            ReceiptNo = "REC001",
            LpoNo = "LPO001",
            SupplierId = "1",
            SupplierName = "ABC Pharmaceuticals",
            OrderDate = DateTime.Today.AddDays(-5),
            Remarks = "Regular monthly order",
            ApprovedBy = "John Doe",
            LpoStatus = "Approved",
            Items = new List<DrugReceiptItemModel>
            {
                new DrugReceiptItemModel
                {
                    ItemNo = "1",
                    DrugNo = "D001",
                    DrugName = "Paracetamol 500mg",
                    InvoiceNo = "INV-001",
                    InvoiceDate = DateTime.Today,
                    ExpiryDate = DateTime.Today.AddYears(2),
                    OrderedQty = 1000,
                    ReceivedQty = 950,
                    DiscountPercentage = 5,
                    Bonus = 50,
                    TotalPrice = 9500,
                    ReceivedDate = DateTime.Today,
                    Manufacturer = "PharmaCo Ltd",
                    ManufactureDate = DateTime.Today.AddMonths(-6),
                    BatchNo = "BAT001",
                    GoodsStatus = "Good"
                },
                new DrugReceiptItemModel
                {
                    ItemNo = "2",
                    DrugNo = "D002",
                    DrugName = "Amoxicillin 250mg",
                    InvoiceNo = "INV-001",
                    InvoiceDate = DateTime.Today,
                    ExpiryDate = DateTime.Today.AddYears(1),
                    OrderedQty = 500,
                    ReceivedQty = 500,
                    DiscountPercentage = 2,
                    Bonus = 25,
                    TotalPrice = 5000,
                    ReceivedDate = DateTime.Today,
                    Manufacturer = "MediCorp",
                    ManufactureDate = DateTime.Today.AddMonths(-2),
                    BatchNo = "BAT002",
                    GoodsStatus = "Good"
                }
            }
        },
        new DrugReceiptModel
        {
            ReceiptNo = "REC002",
            LpoNo = "LPO002",
            SupplierId = "2",
            SupplierName = "XYZ Medical Supplies",
            OrderDate = DateTime.Today.AddDays(-2),
            Remarks = "Emergency order",
            ApprovedBy = "Jane Smith",
            LpoStatus = "Pending",
            Items = new List<DrugReceiptItemModel>()
        }
    };

    public async Task<List<DrugReceiptModel>> GetAllReceiptsAsync()
    {
        await Task.Delay(500); // Simulate network delay
        return _mockReceipts;
    }

    public async Task<DrugReceiptModel?> GetReceiptAsync(string id)
    {
        await Task.Delay(300); // Simulate network delay
        return _mockReceipts.FirstOrDefault(r => r.ReceiptNo == id);
    }

    public async Task SaveReceiptAsync(DrugReceiptModel receipt)
    {
        await Task.Delay(300); // Simulate network delay
        var existing = _mockReceipts.FirstOrDefault(r => r.ReceiptNo == receipt.ReceiptNo);
        if (existing != null)
        {
            _mockReceipts.Remove(existing);
        }
        _mockReceipts.Add(receipt);
        _logger.LogInformation("Saved receipt: {@Receipt}", receipt);
    }

    public async Task SaveReceiptItemAsync(string receiptNo, DrugReceiptItemModel item)
    {
        await Task.Delay(200); // Simulate network delay
        var receipt = _mockReceipts.FirstOrDefault(r => r.ReceiptNo == receiptNo);
        if (receipt != null)
        {
            var existingItem = receipt.Items.FirstOrDefault(i => i.ItemNo == item.ItemNo);
            if (existingItem != null)
            {
                receipt.Items.Remove(existingItem);
            }
            receipt.Items.Add(item);
            _logger.LogInformation("Saved receipt item: {@Item}", item);
        }
    }

}
