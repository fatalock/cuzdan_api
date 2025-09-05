using Cuzdan.Application.Interfaces;
using Cuzdan.Application.DTOs;
using Cuzdan.Domain.Entities;

namespace Cuzdan.Application.Services;


public class WalletService(IUnitOfWork unitOfWork) : IWalletService
{


    public async Task<Result<WalletDto>> CreateWalletAsync(CreateWalletDto createWalletDto, Guid userId)
    {


        var newWallet = new Wallet
        {
            WalletName = createWalletDto.WalletName,
            UserId = userId,
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

        return Result<WalletDto>.Success(walletDtoToReturn);
    }

    public async Task<Result<List<WalletDto>>> GetWalletsAsync(Guid userId)
    {
        var wallets = await unitOfWork.Wallets.GetWalletsAsync(userId);
        var walletDtos = wallets.Select(wallet => new WalletDto
        {
            Id = wallet.Id,
            WalletName = wallet.WalletName,
            Balance = wallet.Balance,
            AvailableBalance = wallet.AvailableBalance,
            Currency = wallet.Currency,

        }).ToList();

        return Result<List<WalletDto>>.Success(walletDtos);
    }

    public async Task<Result<List<UserBalanceByCurrencyResponseDto>>> GetTotalBalancePerCurrencyAsync(Guid userId)
    {
        var balancesFromRepo = await unitOfWork.Wallets.GetTotalBalancePerCurrencyAsync(userId);
        
        var result = balancesFromRepo.Select(b => new UserBalanceByCurrencyResponseDto
        {
            Currency = b.CurrencyType,
            TotalBalance = b.TotalBalance
        }).ToList();

        return Result<List<UserBalanceByCurrencyResponseDto>>.Success(result);
    }

}