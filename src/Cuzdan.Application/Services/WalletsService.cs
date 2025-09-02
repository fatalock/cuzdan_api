using Cuzdan.Application.Interfaces;
using Cuzdan.Application.DTOs;
using Cuzdan.Domain.Entities;

namespace Cuzdan.Application.Services;


public class WalletService(IUnitOfWork unitOfWork) : IWalletService
{


    public async Task<ApiResponse> CreateWalletAsync(CreateWalletDto createWalletDto, Guid Id)
    {


        var newWallet = new Wallet
        {
            WalletName = createWalletDto.WalletName,
            UserId = Id,
            Currency = createWalletDto.Currency
        };

        await unitOfWork.Wallets.AddAsync(newWallet);
        await unitOfWork.SaveChangesAsync();

        var response = new ApiResponse
        {
            IsSuccessful = true,
            SuccessMessage = "Wallet Created",
        };

        return response;
    }

    public async Task<List<WalletDto>> GetWalletsAsyc(Guid Id)
    {
        var wallets = await unitOfWork.Wallets.GetWalletsAsyc(Id);
        var walletDtos = wallets.Select(wallet => new WalletDto
        {
            Id = wallet.Id,
            WalletName = wallet.WalletName,
            Balance = wallet.Balance,
            AvailableBalance = wallet.AvailableBalance,
            Currency = wallet.Currency,

        }).ToList();

        return walletDtos;
    }

    public async Task<List<UserBalanceByCurrencyResponseDto>> GetTotalBalancePerCurrencyAsync(Guid Id)
    {
        var balancesFromRepo = await unitOfWork.Wallets.GetTotalBalancePerCurrencyAsync(Id);
        
        var response = balancesFromRepo.Select(b => new UserBalanceByCurrencyResponseDto
        {
            Currency = b.CurrencyType.ToString(),
            TotalBalance = b.TotalBalance
        }).ToList();

        return response;
    }

}