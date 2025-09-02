using Xunit;
using Moq;
using Microsoft.Extensions.Caching.Memory;
using RichardSzalay.MockHttp;
using FluentAssertions;
using Cuzdan.Infrastructure.Gateways;
using Cuzdan.Domain.Enums;
using System.Threading.Tasks;
using Castle.Core.Configuration;
using Microsoft.Extensions.Configuration;


    public class CurrencyConversionServiceTests
    {
        [Fact] // Bu, xUnit'e bunun bir test metodu olduğunu söyler
        public async Task GetConversionRateAsync_Should_FetchFromApi_WhenCacheIsEmpty()
        {
            // ARRANGE (Hazırlık) --------------------------------------------------

/*             var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(...).Respond(...);
            var httpClient = mockHttp.ToHttpClient(); */

            var realCache = new MemoryCache(new MemoryCacheOptions());


            var realHttpClient = new HttpClient();
            realHttpClient.BaseAddress = new Uri("https://openexchangerates.org");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            
            // 3. Gerçek bir Logger oluştur (isteğe bağlı, ama constructor için gerekli).
            // Basit bir konsol logger'ı oluşturuyoruz.


            // 4. Servisi, TAMAMEN GERÇEK bağımlılıklarla oluştur.
            var service = new CurrencyConversionService(realCache, realHttpClient,configuration);
            

            // ACT (Eylem) ---------------------------------------------------------
            var result = await service.GetConversionRateAsync(CurrencyType.USD, CurrencyType.TRY);


            // ASSERT (Doğrulama) ------------------------------------------------

            // 1. Sonuç doğru mu?
        result.Should().BeGreaterThan(0);

            // 2. API'ye gerçekten 1 kez gidildi mi?

            // 3. Cache'e doğru değer yazıldı mı? (İleri seviye, ama önemli)
            // Bu, Moq.Protected veya cache'in iç yapısına göre daha karmaşık olabilir,
            // ama en azından sonucun doğru olduğunu bilmek iyi bir başlangıç.
        }


    }
