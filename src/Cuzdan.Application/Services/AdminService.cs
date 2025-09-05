using Cuzdan.Application.Exceptions;
using Cuzdan.Application.Interfaces;
using Cuzdan.Application.DTOs;
using Cuzdan.Domain.Entities;
using Cuzdan.Application.Extensions;
using System.Linq.Expressions;
using Cuzdan.Domain.Enums;
using Cuzdan.Domain.Errors;

namespace Cuzdan.Application.Services;

public class AdminService(IUnitOfWork unitOfWork) : IAdminService
{
    public async Task<Result<PagedResult<UserDto>>> GetAllUsersProfileAsync(UserFilterDto filter)
    {
        if (filter.PageNumber < 1) filter.PageNumber = 1;
        if (filter.PageSize < 1 || filter.PageSize > 100) filter.PageSize = 10;

        var predicate = PredicateBuilder.True<User>();

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            predicate = predicate.And(u => u.Name.Contains(filter.Name));
        }

        if (!string.IsNullOrWhiteSpace(filter.Email))
        {
            predicate = predicate.And(u => u.Email.Contains(filter.Email));
        }

        if (filter.Role.HasValue)
        {
            predicate = predicate.And(u => u.Role == filter.Role.Value);
        }

        if (filter.StartDate.HasValue)
        {
            predicate = predicate.And(u => u.CreatedAt >= filter.StartDate.Value);
        }

        if (filter.EndDate.HasValue)
        {
            var endDate = filter.EndDate.Value.Date.AddDays(1).AddTicks(-1);
            predicate = predicate.And(u => u.CreatedAt <= endDate);
        }

        Expression<Func<User, object>>? orderByExpr = filter.OrderBy switch
        {
            UserSortField.Name => u => u.Name,
            UserSortField.Email => u => u.Email,
            UserSortField.CreatedAt => u => u.CreatedAt,
            _ => u => u.CreatedAt,
        };

        var pagedUsers = await unitOfWork.Users.FindAsync(
            predicate: predicate,
            pageNumber: filter.PageNumber,
            pageSize: filter.PageSize,
            orderBy: orderByExpr,
            isDescending: filter.IsDescending ?? true
        );

        var userDtos = (pagedUsers.Items ?? Enumerable.Empty<User>()).Select(user => new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        }).ToList();

        var result = new PagedResult<UserDto>
        {
            Page = pagedUsers.Page,
            PageSize = pagedUsers.PageSize,
            TotalCount = pagedUsers.TotalCount,
            Items = userDtos
        };
        return Result<PagedResult<UserDto>>.Success(result);
    }
    public async Task<Result<UserDto>> GetUserProfileAsync(Guid userId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) return Result<UserDto>.Failure(DomainErrors.User.NotFound);

        var result = new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };

        return Result<UserDto>.Success(result);
    }

    public async Task<Result<PagedResult<WalletDto>>> GetAllWalletsAsync(WalletFilterDto filter)
    {
        if (filter.PageNumber < 1) filter.PageNumber = 1;
        if (filter.PageSize < 1 || filter.PageSize > 100) filter.PageSize = 10;

        var predicate = PredicateBuilder.True<Wallet>();

        if (!string.IsNullOrWhiteSpace(filter.WalletName))
        {
            predicate = predicate.And(w => w.WalletName.Contains(filter.WalletName));
        }

        if (filter.Currency.HasValue)
        {
            predicate = predicate.And(w => w.Currency == filter.Currency.Value);
        }

        if (filter.MinBalance.HasValue)
        {
            predicate = predicate.And(w => w.Balance >= filter.MinBalance.Value);
        }

        if (filter.MaxBalance.HasValue)
        {
            predicate = predicate.And(w => w.Balance <= filter.MaxBalance.Value);
        }

        if (filter.StartDate.HasValue)
        {
            predicate = predicate.And(w => w.CreatedAt >= filter.StartDate.Value);
        }

        if (filter.EndDate.HasValue)
        {
            var endDate = filter.EndDate.Value.Date.AddDays(1).AddTicks(-1);
            predicate = predicate.And(w => w.CreatedAt <= endDate);
        }

        Expression<Func<Wallet, object>>? orderByExpr = filter.OrderBy switch
        {
            WalletSortField.WalletName => w => w.WalletName,
            WalletSortField.Balance => w => w.Balance,
            WalletSortField.Currency => w => w.Currency,
            WalletSortField.CreatedAt => w => w.CreatedAt,
            _ => w => w.CreatedAt,
        };

        var pagedWallets = await unitOfWork.Wallets.FindAsync(
            predicate: predicate,
            pageNumber: filter.PageNumber,
            pageSize: filter.PageSize,
            orderBy: orderByExpr,
            isDescending: filter.IsDescending ?? true
        );

        var walletDtos = (pagedWallets.Items ?? Enumerable.Empty<Wallet>()).Select(wallet => new WalletDto
        {
            Id = wallet.Id,
            WalletName = wallet.WalletName,
            Balance = wallet.Balance,
            Currency = wallet.Currency,
            CreatedAt = wallet.CreatedAt
        }).ToList();

        var result = new PagedResult<WalletDto>
        {
            Page = pagedWallets.Page,
            PageSize = pagedWallets.PageSize,
            TotalCount = pagedWallets.TotalCount,
            Items = walletDtos
        };
        return Result<PagedResult<WalletDto>>.Success(result);
    }

    public async Task<Result<List<WalletDto>>> GetUserWalletsAsync(Guid userId)
    {
        var wallets = await unitOfWork.Wallets.GetWalletsAsync(userId);
        var result = wallets.Select(wallet => new WalletDto
        {
            Id = wallet.Id,
            WalletName = wallet.WalletName,
            Balance = wallet.Balance,
            AvailableBalance = wallet.AvailableBalance,
            Currency = wallet.Currency,
            CreatedAt = wallet.CreatedAt

        }).ToList();

        return Result<List<WalletDto>>.Success(result);
    }


    public async Task<Result<PagedResult<TransactionDto>>> GetAllTransactionsAsync(
        TransactionFilterDto filter
    )
    {
        var predicate = PredicateBuilder.True<Transaction>();
        if (filter.Type.HasValue)
        {
            predicate = predicate.And(t => t.Type == filter.Type.Value);
        }

        if (filter.Status.HasValue)
        {
            predicate = predicate.And(t => t.Status == filter.Status.Value);
        }

        if (filter.MinAmount.HasValue)
        {
            predicate = predicate.And(t => t.OriginalAmount >= filter.MinAmount.Value);
        }

        if (filter.MaxAmount.HasValue)
        {
            predicate = predicate.And(t => t.OriginalAmount <= filter.MaxAmount.Value);
        }

        if (filter.StartDate.HasValue)
        {
            predicate = predicate.And(t => t.CreatedAt >= filter.StartDate.Value);
        }

        if (filter.EndDate.HasValue)
        {
            var endDate = filter.EndDate.Value.Date.AddDays(1).AddTicks(-1);
            predicate = predicate.And(t => t.CreatedAt <= endDate);
        }

        Expression<Func<Transaction, object>>? orderByExpr = filter.OrderBy switch
        {
            TransactionSortField.Amount => t => t.OriginalAmount,
            TransactionSortField.CreatedAt => t => t.CreatedAt,
            null => t => t.CreatedAt,
            _ => t => t.CreatedAt,
        };

        var pagedTransactions = await unitOfWork.Transactions.FindAsync(
            predicate: predicate,
            pageNumber: filter.PageNumber,
            pageSize: filter.PageSize,
            orderBy: orderByExpr,
            isDescending: filter.IsDescending ?? true
        );



        var transactionDtos = (pagedTransactions.Items ?? Enumerable.Empty<Transaction>()).Select(t => new TransactionDto
        {
            Id = t.Id,
            FromId = t.FromId,
            ToId = t.ToId,
            Amount = t.OriginalAmount,
            OriginalCurrency = t.OriginalCurrency,
            ConvertedAmount = t.ConvertedAmount,
            TargetCurrency = t.TargetCurrency,
            ConversionRate = t.ConversionRate,
            CreatedAt = t.CreatedAt,
            Status = t.Status,
            Type = t.Type

        }).ToList();

        var result = new PagedResult<TransactionDto>
        {
            Page = pagedTransactions.Page,
            PageSize = pagedTransactions.PageSize,
            TotalCount = pagedTransactions.TotalCount,
            Items = transactionDtos
        };
        return Result<PagedResult<TransactionDto>>.Success(result);
    }

    public async Task<Result<PagedResult<TransactionDto>>> GetTransactionsByWalletAsync(

        Guid walletId,
        TransactionFilterDto filter

    )
    {
        var predicate = PredicateBuilder.True<Transaction>();
        predicate = predicate.And(t => t.FromId == walletId || t.ToId == walletId);
        if (filter.Status.HasValue)
        {
            predicate = predicate.And(t => t.Status == filter.Status.Value);
        }

        if (filter.Type.HasValue)
        {
            predicate = predicate.And(t => t.Type == filter.Type.Value);
        }

        if (filter.MinAmount.HasValue)
        {
            predicate = predicate.And(t => t.OriginalAmount >= filter.MinAmount.Value);
        }

        if (filter.MaxAmount.HasValue)
        {
            predicate = predicate.And(t => t.OriginalAmount <= filter.MaxAmount.Value);
        }

        if (filter.StartDate.HasValue)
        {
            predicate = predicate.And(t => t.CreatedAt >= filter.StartDate.Value);
        }

        if (filter.EndDate.HasValue)
        {
            var endDate = filter.EndDate.Value.Date.AddDays(1).AddTicks(-1);
            predicate = predicate.And(t => t.CreatedAt <= endDate);
        }

        Expression<Func<Transaction, object>>? orderByExpr = filter.OrderBy switch
        {
            TransactionSortField.Amount => t => t.OriginalAmount,
            TransactionSortField.CreatedAt => t => t.CreatedAt,
            null => t => t.CreatedAt,
            _ => t => t.CreatedAt,
        };

        var pagedTransactions = await unitOfWork.Transactions.FindAsync(
            predicate: predicate,
            pageNumber: filter.PageNumber,
            pageSize: filter.PageSize,
            orderBy: orderByExpr,
            isDescending: filter.IsDescending ?? true
        );



        var transactionDtos = (pagedTransactions.Items ?? Enumerable.Empty<Transaction>()).Select(t => new TransactionDto
        {
            Id = t.Id,
            FromId = t.FromId,
            ToId = t.ToId,
            Amount = t.OriginalAmount,
            OriginalCurrency = t.OriginalCurrency,
            ConvertedAmount = t.ConvertedAmount,
            TargetCurrency = t.TargetCurrency,
            ConversionRate = t.ConversionRate,
            CreatedAt = t.CreatedAt,
            Status = t.Status,
            Type = t.Type

        }).ToList();

        var result = new PagedResult<TransactionDto>
        {
            Page = pagedTransactions.Page,
            PageSize = pagedTransactions.PageSize,
            TotalCount = pagedTransactions.TotalCount,
            Items = transactionDtos
        };
        return Result<PagedResult<TransactionDto>>.Success(result);
    }
    public async Task<Result<TransactionDto>> GetTransactionAsync(
        Guid transactionId
    )
    {
        var t = await unitOfWork.Transactions.GetByIdAsync(transactionId);
        if (t == null) return Result<TransactionDto>.Failure(DomainErrors.Transaction.NotFound);
        var result = new TransactionDto
        {
            Id = t.Id,
            FromId = t.FromId,
            ToId = t.ToId,
            Amount = t.OriginalAmount,
            OriginalCurrency = t.OriginalCurrency,
            ConvertedAmount = t.ConvertedAmount,
            TargetCurrency = t.TargetCurrency,
            ConversionRate = t.ConversionRate,
            CreatedAt = t.CreatedAt,
            Status = t.Status,
            Type = t.Type
        };
        return Result<TransactionDto>.Success(result);
    }


}
