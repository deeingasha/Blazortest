using BlazorApp1.Components.Models;
using Microsoft.Extensions.Logging;

namespace BlazorApp1.Components.Services;

public interface IReagentService
{
    Task<List<ReagentModel>> GetAllReagentsAsync();
    Task<ReagentModel> GetReagentAsync(string id);
    Task SaveReagentAsync(ReagentModel reagent);
}

public class ReagentService : IReagentService
{
    private readonly ILogger<ReagentService> _logger;
    private static readonly List<ReagentModel> _reagents = new()
    {
        new ReagentModel
        {
            Reagent_No = "1",
            Reagent_Name = "Blood Glucose Test Strips",
            Description = "Used for measuring blood glucose levels",
            Reorder_Level = "100"
        },
        new ReagentModel
        {
            Reagent_No = "2",
            Reagent_Name = "COVID-19 Test Kit",
            Description = "Rapid antigen test kit",
            Reorder_Level = "50"
        },
        new ReagentModel
        {
            Reagent_No = "3",
            Reagent_Name = "HIV Test Kit",
            Description = "4th generation combo test",
            Reorder_Level = "75"
        }
    };

    public ReagentService(ILogger<ReagentService> logger)
    {
        _logger = logger;
    }

    public async Task<List<ReagentModel>> GetAllReagentsAsync()
    {
        _logger.LogInformation("Fetching all reagents");
        return await Task.FromResult(_reagents);
    }

    public async Task<ReagentModel> GetReagentAsync(string id)
    {
        _logger.LogInformation("Fetching reagent {Id}", id);
        var reagent = _reagents.FirstOrDefault(r => r.Reagent_No == id);
        return await Task.FromResult(reagent ?? new ReagentModel());
    }

    public async Task SaveReagentAsync(ReagentModel reagent)
    {
        if (string.IsNullOrEmpty(reagent.Reagent_No))
        {
            // New reagent
            reagent.Reagent_No = (_reagents.Count + 1).ToString();
            _reagents.Add(reagent);
            _logger.LogInformation("Created new reagent with ID {Id}", reagent.Reagent_No);
        }
        else
        {
            // Update existing
            var index = _reagents.FindIndex(r => r.Reagent_No == reagent.Reagent_No);
            if (index >= 0)
            {
                _reagents[index] = reagent;
                _logger.LogInformation("Updated reagent {Id}", reagent.Reagent_No);
            }
        }
        await Task.CompletedTask;
    }
}
