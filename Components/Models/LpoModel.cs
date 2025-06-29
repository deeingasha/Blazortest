namespace BlazorApp1.Components.Models;

public class LpoDto
{
    public int pn_LPO_No { get; set; }
    public int fn_Supplier_No { get; set; }
    public string v_Supplier_Name { get; set; } = string.Empty;
    public DateTime dt_LPO_Date { get; set; }
    public string v_Remarks { get; set; } = string.Empty;
    public string v_Supplier_Email { get; set; } = string.Empty;
    public decimal n_Total_Amount { get; set; }
    public bool b_Approved { get; set; }
    public List<LpoItemDto> Items { get; set; } = new();
}
public class LpoDetailsDto
{
    public string pfv_LPO_No { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public int? fn_Drug_No { get; set; }
    public string v_Invoice_No { get; set; } = string.Empty;
    public DateTime d_Entry_Date { get; set; }
    public int? fn_Received_By { get; set; }
    public decimal? n_Unit_Price { get; set; }
    public int? n_Ordered_Qty { get; set; }
    public int? n_Qty_Received { get; set; }
    public DateTime d_Exp_Date { get; set; }
    public decimal? n_Discount { get; set; }
    public int? n_Bonus { get; set; }
    public decimal? n_Total_Price { get; set; }
    public string itemName { get; set; } = string.Empty;
    public int? fn_Department_No { get; set; }
    public int fn_Supplier_No { get; set; }
    public int? fn_LPO_Status_No { get; set; }
}
public class LpoItemDto
{
    public int pn_LPOItem_No { get; set; }
    public int fn_LPO_No { get; set; }
    public int fn_Drug_No { get; set; }
    public decimal n_Unit_Price { get; set; }
    public int n_Quantity { get; set; }
    public decimal n_Total { get; set; }
}

public class SupplierDto
{
    public int pn_Entity_No { get; set; }
    public string v_FName { get; set; } = string.Empty;
    public string fv_Entity_Type_Code { get; set; } = string.Empty;

}

public class LpoModel
{
    public string LpoNo { get; set; } = string.Empty;
    public string SupplierId { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public DateTime LpoDate { get; set; } = DateTime.Today;
    public string Remarks { get; set; } = string.Empty;
    public string SupplierEmail { get; set; } = string.Empty;
    public string SupplierPassword { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public bool IsApproved { get; set; }
    public List<LpoItemModel> Items { get; set; } = new();
}

public class LpoItemModel
{
    public string ItemNo { get; set; } = string.Empty;
    public string DrugNo { get; set; } = string.Empty;
    public string DrugName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Total => UnitPrice * Quantity;
}

public class SupplierModel
{
    public string SupplierId { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class LpoListDto
{
    public string pfv_LPO_No { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public int? fn_Drug_No { get; set; }
    public string v_Invoice_No { get; set; } = string.Empty;
    public DateTime d_Entry_Date { get; set; }
    public int? fn_Received_By { get; set; }
    public decimal? n_Unit_Price { get; set; }
    public int? n_Qty_Received { get; set; }
    public DateTime d_Exp_Date { get; set; }
    public decimal? n_Discount { get; set; }
    public int? n_Bonus { get; set; }
    public decimal? n_Total_Price { get; set; }
    public string itemName { get; set; } = string.Empty;
    public int? fn_Department_No { get; set; }
    public int fn_Supplier_No { get; set; }
}

public class SaveLpoDto
{
    public string pv_LPO_No { get; set; } = string.Empty;
    public DateTime d_LPO_Date { get; set; }
    public string v_LPO_Remarks { get; set; } = string.Empty;
    public int fn_Supplier_No { get; set; }
    public int fn_Prepared_By { get; set; }
    public int fn_Company_No { get; set; }
    public int fn_Approved_By { get; set; }
    public string v_LPO_Status { get; set; } = string.Empty;
    public int? fn_LPO_Status_No { get; set; }
    public int fn_Department_No { get; set; }
}

public class SaveLpoItemDto
{
    public string pfv_LPO_No { get; set; } = string.Empty;
    public int pn_Item_No { get; set; }
    public int fn_Drug_No { get; set; }
    public decimal n_Unit_Price { get; set; }
    public int n_Ordered_Qty { get; set; }
    public decimal n_Total_Price { get; set; }
}

