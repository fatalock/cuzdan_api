using System;
using System.Threading.Tasks;
using Cuzdan.Application.DTOs;
using Cuzdan.Application.Interfaces;
using Cuzdan.Application.Services;
using Cuzdan.Domain.Entities;
using Cuzdan.Domain.Enums;
using Moq;
using Xunit;

namespace Cuzdan.Tests.Unit.Application.Services;
public class TransactionServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IWalletRepository> _walletRepoMock;
    private readonly Mock<ITransactionRepository> _transactionRepoMock;
    private readonly Mock<ICurrencyConversionService> _currencyConversionMock;
    private readonly TransactionService _service;

    public TransactionServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _walletRepoMock = new Mock<IWalletRepository>();
        _transactionRepoMock = new Mock<ITransactionRepository>();
        _currencyConversionMock = new Mock<ICurrencyConversionService>();

        // UnitOfWork -> Wallets, Transactions
        _unitOfWorkMock.Setup(u => u.Wallets).Returns(_walletRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Transactions).Returns(_transactionRepoMock.Object);

        _service = new TransactionService(
            Mock.Of<IPaymentGatewayService>(),
            _unitOfWorkMock.Object,
            _currencyConversionMock.Object
        );
    }

    [Fact]
    public async Task TransferTransactionAsync_ShouldReturnSuccess_WhenTransactionIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fromWalletId = Guid.NewGuid();
        var toWalletId = Guid.NewGuid();

        var senderWallet = new Wallet { WalletName = "senderWallet", Id = fromWalletId, UserId = userId, AvailableBalance = 1000, Currency = CurrencyType.USD };
        var receiverWallet = new Wallet { WalletName = "recieverWallet", Id = toWalletId, UserId = Guid.NewGuid(), AvailableBalance = 500, Currency = CurrencyType.EUR };

        var dto = new CreateTransactionDto { FromId = fromWalletId, ToId = toWalletId, Amount = 100 };

        _walletRepoMock.Setup(r => r.GetByIdAsync(fromWalletId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(senderWallet);
        _walletRepoMock.Setup(r => r.GetByIdAsync(toWalletId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(receiverWallet);

        _currencyConversionMock.Setup(c => c.GetConversionRateAsync(senderWallet.Currency, receiverWallet.Currency))
                               .ReturnsAsync(0.9m);

        _transactionRepoMock.Setup(r => r.TransferTransactionAsync(fromWalletId, toWalletId, dto.Amount, 0.9m))
                            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.TransferTransactionAsync(dto, userId);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal("Transaction done", result.SuccessMessage);

        _transactionRepoMock.Verify(r => r.TransferTransactionAsync(fromWalletId, toWalletId, dto.Amount, 0.9m), Times.Once);
    }
}
