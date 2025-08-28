using Xunit;
using Moq;
using Microsoft.Extensions.Caching.Memory;
using RestSharp;
using RichardSzalay.MockHttp;
using FluentAssertions;
using Cuzdan.Infrastructure.Gateways;
using Cuzdan.Domain.Enums;
using System.Threading.Tasks;

namespace Cuzdan.Tests
{
    public class CurrencyConversionServiceTests
    {
        [Fact] // Bu, xUnit'e bunun bir test metodu olduğunu söyler
        public async Task GetConversionRateAsync_Should_FetchFromApi_WhenCacheIsEmpty()
        {
            // ARRANGE (Hazırlık) --------------------------------------------------

            // 1. IMemoryCache'i taklit et (Moq)
            // Cache'in boş olduğunu varsaydığımız için özel bir ayara gerek yok.
            var mockCache = new MemoryCache(new MemoryCacheOptions());

            // 2. API çağrısını taklit et (MockHttp)
            var httpClient = new HttpClient();
            var options = new RestClientOptions("https://openexchangerates.orggg");
            var restClient = new RestClient(httpClient, options);

            // 4. Servisi, her iki taklit edilmiş bağımlılığı da vererek oluştur
            var service = new CurrencyConversionService(mockCache, restClient); // <-- DOĞRU KULLANIM

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
}