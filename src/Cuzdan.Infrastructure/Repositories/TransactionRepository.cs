using Cuzdan.Application.DTOs;
using Cuzdan.Application.Interfaces;
using Cuzdan.Domain.Entities;
using Cuzdan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cuzdan.Infrastructure.Repositories;

public class TransactionRepository(CuzdanContext context) : Repository<Transaction>(context), ITransactionRepository
{
    public async Task TransferTransactionAsync(Guid FromId, Guid ToId, decimal Amount, decimal conversionRate)
    {
        
        await Context.Database.ExecuteSqlInterpolatedAsync($@"
            CALL transfer_funds(
                {FromId}, 
                {ToId},  
                {Amount}, 
                {conversionRate})
        ");
    }
}
