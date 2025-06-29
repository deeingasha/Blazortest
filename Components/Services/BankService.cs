using System;

using BlazorApp1.Components.Core.Http;
using BlazorApp1.Components.Core.Mapping;
using BlazorApp1.Components.Models;
using Microsoft.Extensions.Logging;

namespace BlazorApp1.Components.Services;

public interface IBankService
{
    Task<List<BankModel>> GetAllBanksAsync();
    Task<BankModel> GetBankAsync(string id);
    Task SaveBankAsync(BankModel bank);
    Task<List<BankBranchModel>> GetBankBranchesAsync(string bankId);
    Task<BankBranchModel> GetBankBranchAsync(string id);
    Task SaveBankBranchAsync(BankBranchModel branch);
}

public class BankService : IBankService
{
    private readonly IApiHttpClient _apiClient;
    private readonly IBankMapper _mapper;
    private readonly ILogger<BankService> _logger;

    public BankService(
        IApiHttpClient apiClient,
        IBankMapper mapper,
        ILogger<BankService> logger)
    {
        _apiClient = apiClient;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<BankModel>> GetAllBanksAsync()
    {
        try
        {
            var dtos = await _apiClient.GetAsync<List<BankDto>>("api/Hospital/bankInfo");
            return _mapper.ToModelList(dtos ?? new List<BankDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting banks");
            throw;
        }
    }

    public async Task<BankModel> GetBankAsync(string id)
    {
        try
        {
            _logger.LogDebug("Getting bank with ID {Id}", id);
            var banks = await GetAllBanksAsync();
            return banks.FirstOrDefault(b => b.BankNo == id) ?? new BankModel();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bank {Id}", id);
            throw;
        }
    }

    public async Task SaveBankAsync(BankModel bank)
    {
        try
        {
            var dto = _mapper.ToDto(bank);
            if (string.IsNullOrEmpty(bank.BankNo))
            {
                // New bank
                dto.pn_Bank_No = await GenerateNextId("bank");
                bank.BankNo = dto.pn_Bank_No.ToString();
                await _apiClient.PostAsync<BankDto, BankDto>("api/Hospital/PostBankInfo", dto);
                _logger.LogInformation("New bank created with ID {Id}", dto.pn_Bank_No);
            }
            else
            {
                // Update existing
                var response = await _apiClient.PutAsync<BankDto, BankDto>($"api/Hospital/EditBankInfo/{dto.pn_Bank_No}", dto);
                _logger.LogInformation("Bank updated with ID {Id} : Name {Name}", dto.pn_Bank_No, dto.v_Bank_Name);

                if (response == null)
                {
                    throw new Exception($"Failed to save bank {bank.BankNo}");
                }
                // Update the model with response data
                bank.BankName = response.v_Bank_Name;
                bank.BankCode = response.v_Bank_Code;

                _logger.LogInformation("Updated bank {Id}, name: {Name}",
                    response.pn_Bank_No,
                    response.v_Bank_Name);
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving bank {Id}", bank.BankNo);
            throw;
        }
    }

    public async Task<List<BankBranchModel>> GetBankBranchesAsync(string bankId)
    {
        try
        {
            var dtos = await _apiClient.GetAsync<List<BankBranchDto>>($"api/Hospital/BranchList/{bankId}");
            // var allBranches = _mapper.ToBranchModelList(dtos ?? new List<BankBranchDto>());
            // return allBranches.Where(b => b.BankNo == bankId).ToList();
            return _mapper.ToBranchModelList(dtos ?? new List<BankBranchDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting branches for bank {Id}", bankId);
            throw;
        }
    }

    public async Task<BankBranchModel> GetBankBranchAsync(string id)
    {
        try
        {
            var dto = await _apiClient.GetAsync<BankBranchDto>($"api/Hospital/BankBranchInfo/{id}");
            return _mapper.ToBranchModel(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bank branch {Id}", id);
            throw;
        }
    }

    public async Task SaveBankBranchAsync(BankBranchModel branch)
    {
        try
        {
            var dto = _mapper.ToBranchDto(branch);
            if (string.IsNullOrEmpty(branch.BranchNo))
            {
                // New branch
                dto.pn_Branch_No = await GenerateNextId("branch");
                branch.BranchNo = dto.pn_Branch_No.ToString();
                await _apiClient.PostAsync<BankBranchDto, BankBranchDto>("api/Hospital/PostBankBranchInfo", dto);
                _logger.LogInformation("New bank branch created with ID {Id}", dto.pn_Branch_No);
            }
            else
            {
                // Update existing
                var response = await _apiClient.PutAsync<BankBranchDto, BankBranchDto>($"api/Hospital/EditBankBranchInfo/{branch.BranchNo}", dto);
                if (response == null)
                {
                    throw new Exception($"Failed to save bank branch {branch.BranchNo}");
                }
                // Update the model with response data
                branch.BranchName = response.v_Branch_Name;
                branch.BranchCode = response.v_Branch_Code;

                _logger.LogInformation("Updated bank branch {Id}, name: {Name}",
                    response.pn_Branch_No,
                    response.v_Branch_Name);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving bank branch");
            throw;
        }
    }

    // This method generates the next available BankNo or BranchNo depending on the type parameter.
    // type: "bank" or "branch"
    // Returns the next available ID as a string.
    public async Task<int> GenerateNextId(string type)
    {
        try
        {
            if (type == "bank")
            {
                _logger.LogDebug("Generating new bank ID...");
                var banks = await _apiClient.GetAsync<List<BankDto>>("api/Hospital/bankInfo");
                if (banks == null || !banks.Any())
                    return 1;
                return banks.Max(b => b.pn_Bank_No) + 1;
            }
            else if (type == "branch")
            {
                _logger.LogDebug("Generating new bank branch ID...");
                var branches = await _apiClient.GetAsync<List<BankBranchDto>>("api/Hospital/bankBranchInfo");
                if (branches == null || !branches.Any())
                    return 1;
                return branches.Max(b => b.pn_Branch_No) + 1;
            }
            else
            {
                throw new ArgumentException("Invalid type for ID generation");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating next {Type} ID", type);
            throw;
        }
    }

}
