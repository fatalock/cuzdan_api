using Cuzdan.Api.Models;
using Microsoft.EntityFrameworkCore;
using Cuzdan.Api.Schemas;
using Cuzdan.Api.Data;
using Azure;
using Cuzdan.Api.Interfaces;


namespace Cuzdan.Api.Services;


public class TransactionService(CuzdanContext context) : ITransactionService
{
    private readonly CuzdanContext _context = context;

    public async Task<ApiResponse> TransferTransactionAsync(TransactionDto TransactionDto, Guid UserId)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {


            var senderWallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.Id == TransactionDto.FromId);

            var receiverWallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.Id == TransactionDto.ToId);

            if (senderWallet == null || receiverWallet == null) throw new Exception("Wallet not found.");

            if (senderWallet.UserId != UserId) throw new Exception("Not your wallet");


            if (senderWallet.Balance < TransactionDto.Amount) throw new Exception("Not enough balance");


            if (TransactionDto.Amount <= 0) throw new Exception("Invalid amount");




            senderWallet.Balance -= TransactionDto.Amount; // Gönderenden düş
            receiverWallet.Balance += TransactionDto.Amount; // Alıcıya ekle

            var newTransaction = new Transaction
            {
                FromId = TransactionDto.FromId,
                ToId = TransactionDto.ToId,
                Amount = TransactionDto.Amount,
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
        catch (System.Exception ex)
        {
            await transaction.RollbackAsync();
            var response = new ApiResponse
            {
                IsSuccessful = false,
                ErrorMessage = ex.Message
            };
            return response;
        }

    }

}