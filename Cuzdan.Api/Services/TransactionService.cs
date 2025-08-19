using Cuzdan.Api.Models;
using Microsoft.EntityFrameworkCore;
using Cuzdan.Api.Schemas;
using Cuzdan.Api.Data;
using Azure;
using Cuzdan.Api.Interfaces;
using Cuzdan.Api.Exceptions;


namespace Cuzdan.Api.Services;


public class TransactionService(CuzdanContext context) : ITransactionService
{
    private readonly CuzdanContext _context = context;

    public async Task<ApiResponse> TransferTransactionAsync(TransactionDto transactionDto, Guid UserId)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {


            var senderWallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.Id == transactionDto.FromId);

            var receiverWallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.Id == transactionDto.ToId);

            if (senderWallet == null || receiverWallet == null) throw new NotFoundException("Wallet not found.");

            if (senderWallet.UserId != UserId) throw new ForbiddenAccessException("Not your wallet");


            if (senderWallet.Balance < transactionDto.Amount) throw new InsufficientBalanceException("Not enough balance");


            if (transactionDto.Amount <= 0 | transactionDto.Amount > 1000000) throw new ArgumentOutOfRangeException("transactionDto.Amount","Invalid Amount");




            senderWallet.Balance -= transactionDto.Amount;
            receiverWallet.Balance += transactionDto.Amount;

            var newTransaction = new Transaction
            {
                FromId = transactionDto.FromId,
                ToId = transactionDto.ToId,
                Amount = transactionDto.Amount,
            };
            _context.Transactions.Add(newTransaction);
            await _context.SaveChangesAsync();

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