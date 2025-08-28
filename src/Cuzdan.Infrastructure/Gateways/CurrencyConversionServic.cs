using Cuzdan.Application.Interfaces;
using Cuzdan.Domain.Enums;
using Microsoft.Extensions.Caching.Memory;
using RestSharp;
using System;
using System.Threading.Tasks;


namespace Cuzdan.Infrastructure.Gateways;

public class CurrencyConversionService(IMemoryCache memoryCache, RestClient client) : ICurrencyConversionService
{
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly string _appId = "ed98b5e6f5284516850d2c9969c4d94a";
    private readonly RestClient _client = client;
    private readonly string[] _supportedCurrencies = Enum.GetNames<CurrencyType>();
    private const string BaseCurrency = "USD";
    public CurrencyConversionService() : this(null!, null!)
    {
        Console.WriteLine("!!! UYARI: Parametresiz constructor çağrıldı, bu DI hatasına işaret edebilir !!!");
    }
    public async Task<decimal> GetConversionRateAsync(CurrencyType fromCurrency, CurrencyType toCurrency)
    {
        if (fromCurrency == toCurrency)
        {
            return 1m;
        }
        var cacheKey = $"Rate_{fromCurrency}_{toCurrency}";

        if (_memoryCache.TryGetValue(cacheKey, out decimal rate))
        {
            return rate;
        }

        await SendApiCallAndPopulateAsync();

        if (_memoryCache.TryGetValue(cacheKey, out decimal finalRate))
        {
            return finalRate;
        }

        throw new Exception($"Döviz kuru bilgisi bulunamadı: {fromCurrency} -> {toCurrency}");

    }
    private async Task SendApiCallAndPopulateAsync()
    {
        // API'den sadece desteklediğimiz kurları, USD bazlı olarak istiyoruz.
        var symbols = string.Join(",", _supportedCurrencies.Where(c => c != BaseCurrency));

        var request = new RestRequest("api/latest.json");
        request.AddParameter("app_id", _appId);
        request.AddParameter("base", BaseCurrency);
        request.AddParameter("symbols", symbols);

        var response = await _client.GetAsync<ExchangeRateResponse>(request);

        
        if (response == null || response.Rates == null)
        {
            throw new Exception("Harici API'den döviz kuru verisi alınamadı.");
        }

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(45));
        response.Rates[BaseCurrency] = 1m;
        foreach (var fromCurr in _supportedCurrencies)
        {
            foreach (var toCurr in _supportedCurrencies)
            {
                var rateKey = $"Rate_{fromCurr}_{toCurr}";

                var usdToFromRate = response.Rates[fromCurr];
                var usdToToRate = response.Rates[toCurr];
                var conversionRate = usdToToRate / usdToFromRate;

                _memoryCache.Set(rateKey, conversionRate, cacheOptions);
            }
        }
    }
}
    

public class ExchangeRateResponse
{
    public Dictionary<string, decimal>? Rates { get; set; }
}