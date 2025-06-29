using BlazorApp1.Components.Models;

namespace BlazorApp1.Components.Core.Mapping;

public interface IDrugMapper
{
    void SetDrugTypes(List<DrugTypeModel> drugTypes);
    DrugTypeModel ToTypeModel(DrugTypeDto dto);
    DrugModel ToModel(DrugDto dto);
    DrugDto ToDto(DrugModel model);
    SaveDrugDto ToSaveDto(DrugModel model, int nextId);
    List<DrugTypeModel> ToTypeModelList(List<DrugTypeDto> dtos);
    List<DrugModel> ToModelList(List<DrugDto> dtos);
}

public class DrugMapper : IDrugMapper
{
    private Dictionary<string, string> _drugTypeNames = new();

    public void SetDrugTypes(List<DrugTypeModel> drugTypes)
    {
        _drugTypeNames = drugTypes.ToDictionary(
            dt => dt.DrugTypeNo,
            dt => dt.DrugTypeName
        );
    }
    public DrugTypeModel ToTypeModel(DrugTypeDto dto)
    {
        return new DrugTypeModel
        {
            DrugTypeNo = dto.pn_Drug_Type_No.ToString(),
            DrugTypeName = dto.v_Drug_Type
        };
    }

    public DrugModel ToModel(DrugDto dto)
    {
        var drugTypeNo = dto.fn_Drug_Type_No?.ToString() ?? "0";

        return new DrugModel
        {
            DrugNo = dto.pn_Drug_No.ToString(),
            DrugName = dto.v_Drug_Name,
            DrugTypeNo = drugTypeNo,
            DrugTypeName = _drugTypeNames.GetValueOrDefault(drugTypeNo, "Unknown"),
            Manufacturer = dto.v_Manufacturer,
            ReorderLevel = dto.n_Reorder_Level ?? 0,
            Description = dto.v_Description,
            StockQuantity = dto.n_Stock_Qty,
            UnitPrice = dto.n_Default_Price ?? 0
        };
    }

    public DrugDto ToDto(DrugModel model)
    {
        return new DrugDto
        {
            pn_Drug_No = int.TryParse(model.DrugNo, out var no) ? no : 0,
            v_Drug_Name = model.DrugName,
            fn_Drug_Type_No = int.TryParse(model.DrugTypeNo, out var typeNo) ? typeNo : 0,
            // v_DrugType_Name = model.DrugTypeName,
            v_Manufacturer = model.Manufacturer,
            n_Reorder_Level = model.ReorderLevel,
            v_Description = model.Description,
            n_Stock_Qty = model.StockQuantity,
            n_Default_Price = model.UnitPrice
        };
    }
    public SaveDrugDto ToSaveDto(DrugModel model, int nextId)
    {
        return new SaveDrugDto
        {
            pn_Drug_No = nextId,
            v_Drug_Name = model.DrugName,
            fn_Drug_Type_No = int.Parse(model.DrugTypeNo),
            fn_Service_Type_No = 3, // Fixed value as specified
            fn_Manufacturer_No = null,
            n_Reorder_Level = model.ReorderLevel > 0 ? model.ReorderLevel : 10
        };
    }

    public List<DrugTypeModel> ToTypeModelList(List<DrugTypeDto> dtos)
    {
        return dtos?.Select(ToTypeModel).ToList() ?? new List<DrugTypeModel>();
    }

    public List<DrugModel> ToModelList(List<DrugDto> dtos)
    {
        return dtos?.Select(ToModel).ToList() ?? new List<DrugModel>();
    }
}