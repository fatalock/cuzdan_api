using Cuzdan.Application.Interfaces;
using Cuzdan.Application.DTOs;
using Cuzdan.Domain.Entities;

namespace Cuzdan.Application.Services;


public class WalletService(IUnitOfWork unitOfWork) : IWalletService
{


    public async Task<WalletDto> CreateWalletAsync(CreateWalletDto createWalletDto, Guid Id)
    {


        var newWallet = new Wallet
        {
            WalletName = createWalletDto.WalletName,
            UserId = Id,
            Currency = createWalletDto.Currency
        };

        await unitOfWork.Wallets.AddAsync(newWallet);
        await unitOfWork.SaveChangesAsync();

        var walletDtoToReturn = new WalletDto
        {
            Id = newWallet.Id,
            WalletName = newWallet.WalletName,
            Balance = newWallet.Balance,
            Currency = newWallet.Currency,
            CreatedAt = newWallet.CreatedAt
        };

        return walletDtoToReturn;
    }

    public async Task<List<WalletDto>> GetWalletsAsync(Guid Id)
    {
        var wallets = await unitOfWork.Wallets.GetWalletsAsync(Id);
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