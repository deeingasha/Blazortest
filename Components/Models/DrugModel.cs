namespace BlazorApp1.Components.Models;

public class DrugTypeDto
{
    public int pn_Drug_Type_No { get; set; }
    public string v_Drug_Type { get; set; } = string.Empty;
    public string v_Description { get; set; } = string.Empty;
}
public class DrugDto
{
    public int pn_Drug_No { get; set; }
    public string v_Drug_Name { get; set; } = string.Empty;
    public int? fn_Drug_Type_No { get; set; }
    public int? fn_Service_Type_No { get; set; }
    public int? fn_Manufacturer_No { get; set; }
    public int? n_Reorder_Level { get; set; }
    public int n_Stock_Qty { get; set; }
    public int? n_Default_Price { get; set; }
    public string v_Description { get; set; } = string.Empty;
    // public string v_DrugType_Name { get; set; } = string.Empty;
    public string v_Manufacturer { get; set; } = string.Empty;
}

public class DrugTypeModel
{
    public string DrugTypeNo { get; set; } = string.Empty;
    public string DrugTypeName { get; set; } = string.Empty;
}

public class DrugModel
{
    public string DrugNo { get; set; } = string.Empty;
    public string DrugName { get; set; } = string.Empty;
    public string DrugTypeNo { get; set; } = string.Empty;
    public string DrugTypeName { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public int ReorderLevel { get; set; }
    public string Description { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public int UnitPrice { get; set; }
}
public class SaveDrugDto
{
    public int pn_Drug_No { get; set; }
    public string v_Drug_Name { get; set; } = string.Empty;
    public int fn_Drug_Type_No { get; set; }
    public int fn_Service_Type_No { get; set; }
    public int? fn_Manufacturer_No { get; set; }
    public int? n_Reorder_Level { get; set; }
}