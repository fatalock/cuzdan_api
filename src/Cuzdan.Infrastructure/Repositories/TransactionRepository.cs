using Cuzdan.Application.Interfaces;
using Cuzdan.Domain.Entities;
using Cuzdan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cuzdan.Infrastructure.Repositories;

public class TransactionRepository(CuzdanContext context) : Repository<Transaction>(context), ITransactionRepository
{

}
