using Cuzdan.Application.Exceptions;
using Cuzdan.Application.Interfaces;
using Cuzdan.Application.DTOs;
using Cuzdan.Domain.Entities;

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
        };

        await _walletRepository.AddWalletAsync(newWallet);
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



    public async Task<PagedResult<TransactionDto>> GetWalletTransactionsAsync(
        Guid userId,
        Guid walletId,
        string type,
        int page,
        int pageSize)
    {
        // 1. Sayfa ve sayfa boyutu validasyonu
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        // 2. Önce cüzdanın varlığını ve kullanıcıya ait olduğunu doğrula.
        // Bu sorgu çok hızlıdır çünkü sadece UserId'yi çeker.
        var walletBelongsToUser = await _walletRepository.DoesWalletBelongToUserAsync(walletId, userId);
        
        if (!walletBelongsToUser)
        {
            throw new NotFoundException("Wallet not found or access is denied.");
        }

        // 3. Veri erişimini repository'e devret
        return await _walletRepository.GetTransactionsByWalletIdAsync(walletId, type, page, pageSize);
    }


}