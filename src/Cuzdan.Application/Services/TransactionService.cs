using Cuzdan.Application.Exceptions;
using Cuzdan.Application.Interfaces;
using Cuzdan.Application.DTOs;
using Cuzdan.Domain.Entities;

namespace Cuzdan.Application.Services;


public class TransactionService(ITransactionRepository transactionRepository, IWalletRepository walletRepository, IUnitOfWork unitOfWork) : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository = transactionRepository;
    private readonly IWalletRepository _walletRepository = walletRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ApiResponse> TransferTransactionAsync(TransactionDto transactionDto, Guid UserId)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {


            var senderWallet = await _walletRepository.GetWalletByIdAsyc(transactionDto.FromId);

            var receiverWallet = await _walletRepository.GetWalletByIdAsyc(transactionDto.ToId);

            if (senderWallet == null || receiverWallet == null) throw new NotFoundException("Wallet not found.");

            if (senderWallet.UserId != UserId) throw new ForbiddenAccessException("Not your wallet");


            if (senderWallet.Balance < transactionDto.Amount) throw new InsufficientBalanceException("Not enough balance");


            if (transactionDto.Amount <= 0 | transactionDto.Amount > 1000000) throw new ArgumentOutOfRangeException("transactionDto.Amount", "Invalid Amount");




            senderWallet.Balance -= transactionDto.Amount;
            receiverWallet.Balance += transactionDto.Amount;

            var newTransaction = new Transaction
            {
                FromId = transactionDto.FromId,
                ToId = transactionDto.ToId,
                Amount = transactionDto.Amount,
            };
            await _transactionRepository.AddTransactionAsync(newTransaction);
            await _unitOfWork.SaveChangesAsync();

            await transaction.CommitAsync();

            var response = new ApiResponse
            {
                IsSuccessful = true,
                SuccessMessage = "Transaction done",
            };

            return response;

        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }

    }

}