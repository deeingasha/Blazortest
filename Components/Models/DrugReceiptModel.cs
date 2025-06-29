namespace BlazorApp1.Components.Models;

public class DrugReceiptDto
{
    public int pn_Receipt_No { get; set; }
    public int fn_LPO_No { get; set; }
    public int fn_Supplier_No { get; set; }
    public DateTime dt_Order_Date { get; set; }
    public string v_Remarks { get; set; } = string.Empty;
    public string v_Approved_By { get; set; } = string.Empty;
    public string v_LPO_Status { get; set; } = string.Empty;
    public List<DrugReceiptItemDto> Items { get; set; } = new();
}

public class DrugReceiptItemDto
{
    public int pn_ReceiptItem_No { get; set; }
    public int fn_Receipt_No { get; set; }
    public int fn_Drug_No { get; set; }
    public string v_Invoice_No { get; set; } = string.Empty;
    public DateTime dt_Invoice_Date { get; set; }
    public DateTime dt_Expiry_Date { get; set; }
    public int n_Ordered_Qty { get; set; }
    public int n_Received_Qty { get; set; }
    public decimal n_Discount { get; set; }
    public int n_Bonus { get; set; }
    public decimal n_Total_Price { get; set; }
    public DateTime dt_Received_Date { get; set; }
    public string v_Manufacturer { get; set; } = string.Empty;
    public DateTime dt_Manufacture_Date { get; set; }
    public string v_Batch_No { get; set; } = string.Empty;
    public string v_Goods_Status { get; set; } = string.Empty;
}

public class DrugReceiptModel
{
    public string ReceiptNo { get; set; } = string.Empty;
    public string LpoNo { get; set; } = string.Empty;
    public string SupplierId { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; } = DateTime.Today;
    public string Remarks { get; set; } = string.Empty;
    public string ApprovedBy { get; set; } = string.Empty;
    public string LpoStatus { get; set; } = string.Empty;
    public List<DrugReceiptItemModel> Items { get; set; } = new();
}

public class DrugReceiptItemModel
{
    public string ItemNo { get; set; } = string.Empty;
    public string DrugNo { get; set; } = string.Empty;
    public string DrugName { get; set; } = string.Empty;
    public string InvoiceNo { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; } = DateTime.Today;
    public DateTime ExpiryDate { get; set; } = DateTime.Today.AddYears(1);
    public int OrderedQty { get; set; }
    public int ReceivedQty { get; set; }
    public decimal DiscountPercentage { get; set; }
    public int Bonus { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime ReceivedDate { get; set; } = DateTime.Today;
    public string Manufacturer { get; set; } = string.Empty;
    public DateTime ManufactureDate { get; set; } = DateTime.Today;
    public string BatchNo { get; set; } = string.Empty;
    public string GoodsStatus { get; set; } = string.Empty;
}
