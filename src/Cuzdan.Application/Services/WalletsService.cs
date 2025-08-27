using Cuzdan.Application.Exceptions;
using Cuzdan.Application.Interfaces;
using Cuzdan.Application.DTOs;
using Cuzdan.Domain.Entities;
using Cuzdan.Application.Extensions;

namespace Cuzdan.Application.Services;


public class WalletService(IWalletRepository walletRepository, IUnitOfWork unitOfWork) : IWalletService
{
    private readonly IWalletRepository _walletRepository = walletRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ApiResponse> CreateWalletAsync(CreateWalletDto createWalletDto, Guid Id)
    {

        var newWallet = new Wallet
        {
            WalletName = createWalletDto.WalletName,
            UserId = Id,
            Balance = 0,
            AvailableBalance = 0
        };

        await _walletRepository.AddAsync(newWallet);
        await _unitOfWork.SaveChangesAsync();

        var response = new ApiResponse
        {
            IsSuccessful = true,
            SuccessMessage = "Wallet Created",
        };

        return response;
    }

    public async Task<List<WalletDto>> GetWalletsAsyc(Guid Id)
    {
        var wallets = await _walletRepository.GetWalletsAsyc(Id);
        return wallets;
    }



}