using System.Linq;
using System.Threading.Tasks;
using Cuzdan.Application.DTOs;
using Cuzdan.Application.Exceptions;
using Cuzdan.Application.Interfaces;
using Cuzdan.Domain.Entities;
using Cuzdan.Infrastructure.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.EntityFrameworkCore;
using Cuzdan.Domain.Enums;


// Bu sınıf, tüm testler için API'yi ve veritabanını ayağa kaldırır.
public class TransferConcurrencyTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory = factory;

    [Fact]
    public async Task TransferTransactionAsync_WhenTwoConcurrentTransfersAreMade_PessimisticLockPreventsOverdraft()
    {
        // ARRANGE (HAZIRLIK)
        //--------------------------------------------------------------------------------
        // Her test için temiz bir servis ve veritabanı kapsamı (scope) oluşturuyoruz.
        using var scope = _factory.Services.CreateScope();
        var transactionService = scope.ServiceProvider.GetRequiredService<ITransactionService>();
        var context = scope.ServiceProvider.GetRequiredService<CuzdanContext>();

        // 1. Test için bir kullanıcı ve cüzdan oluştur.
        var user = new User
        {
            Name = "Test User",
            Email = $"test{Guid.NewGuid()}@example.com",
            PasswordHash = "test_hash",
            Role = UserRole.User
        };
        var senderWallet = new Wallet
        {
            UserId = user.Id,
            Balance = 100, // Başlangıç bakiyesi
            AvailableBalance = 100, // Başlangıç kullanılabilir bakiye
            WalletName = "Sender Wallet"
        };
        var receiverWallet = new Wallet
        {
            UserId = user.Id, // Basitlik için aynı kullanıcı
            Balance = 0,
            AvailableBalance = 0,
            WalletName = "Receiver Wallet"
        };
        
        context.Users.Add(user);
        context.Wallets.Add(senderWallet);
        context.Wallets.Add(receiverWallet);
        await context.SaveChangesAsync();
        
        // 2. İki farklı transfer işlemi için DTO'ları hazırla.
        // Toplamları (80 + 70 = 150), bakiyeden (100) daha fazla.
        var transferRequestA = new CreateTransactionDto { FromId = senderWallet.Id, ToId = receiverWallet.Id, Amount = 80 };
        var transferRequestB = new CreateTransactionDto { FromId = senderWallet.Id, ToId = receiverWallet.Id, Amount = 70 };


        // ACT (EYLEM) - YARIŞI BAŞLATMA
        //--------------------------------------------------------------------------------
        
        // İki ayrı isteği simüle etmek için iki ayrı scope (kapsam) oluşturuyoruz.
        // Her scope, kendi DbContext'ine sahip olacak.
        var scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();

        using var scopeA = scopeFactory.CreateScope();
        var transactionServiceA = scopeA.ServiceProvider.GetRequiredService<ITransactionService>();

        using var scopeB = scopeFactory.CreateScope();
        var transactionServiceB = scopeB.ServiceProvider.GetRequiredService<ITransactionService>();

        // Görevleri, kendi izole servislerini kullanarak tanımlıyoruz.
        var taskA = transactionServiceA.TransferTransactionAsync(transferRequestA, user.Id);
        var taskB = transactionServiceB.TransferTransactionAsync(transferRequestB, user.Id);

        // 4. Bu iki görevi aynı anda çalıştır ve sonuçlarını bekle.
        // Birinin kesinlikle hata vermesini bekliyoruz.
        var results = await Task.WhenAll(
            taskA.ContinueWith(t => t), // Hata fırlatsa bile görevin kendisini yakala
            taskB.ContinueWith(t => t)
        );

        var successfulTask = results.FirstOrDefault(t => t.IsCompletedSuccessfully);
        var failedTask = results.FirstOrDefault(t => t.IsFaulted);


        // ASSERT (DOĞRULAMA)
        //--------------------------------------------------------------------------------

        // 5. Sonuçları doğrula.
        successfulTask.Should().NotBeNull("İşlemlerden biri başarılı olmalıydı.");
        failedTask.Should().NotBeNull("İşlemlerden biri hata fırlatmalıydı.");
        
        // 6. Hata fırlatan görevin doğru hatayı (InsufficientBalanceException) fırlattığını kontrol et.
        // AggregateException, Task içindeki asıl hatayı sarar.
        var exception = failedTask.Exception.InnerException;
        exception.Should().BeOfType<InsufficientBalanceException>("İkinci işlem yetersiz bakiye hatası almalıydı.");

        // 7. Veritabanına gidip son durumu tekrar kontrol et.
        var finalSenderWalletState = await context.Wallets
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == senderWallet.Id);
        var finalReceiverWalletState = await context.Wallets
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == receiverWallet.Id);
        var transactionCount = await context.Transactions
            .AsNoTracking()
            .CountAsync(t => t.FromId == senderWallet.Id);

        // 8. Son bakiyelerin doğru olduğunu doğrula.
        finalSenderWalletState.Balance.Should().Be(20, "Başarılı olan 80 TL'lik işlem sonrası bakiye 20 olmalıydı.");
        finalSenderWalletState.AvailableBalance.Should().Be(20, "Kullanılabilir bakiye de 20 olmalıydı.");
        finalReceiverWalletState.Balance.Should().Be(80, "Alıcı cüzdanın bakiyesi 80 olmalıydı.");
        
        // 9. Sadece bir tane başarılı işlem kaydı olduğunu doğrula.
        transactionCount.Should().Be(1, "Veritabanında sadece bir tane başarılı transfer kaydı olmalıydı.");
    }
}
