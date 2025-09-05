using System.Net.Http.Json;
using Cuzdan.Application.Interfaces;
using Cuzdan.Domain.Enums;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;



namespace Cuzdan.Infrastructure.Gateways;

public class CurrencyConversionService(IMemoryCache memoryCache, HttpClient httpClient, IConfiguration configuration) : ICurrencyConversionService
{
    private readonly string _appId = configuration["ExternalApis:OpenExchangeRates:AppId"];
    private readonly string[] _supportedCurrencies = Enum.GetNames<CurrencyType>();
    private const string BaseCurrency = "USD";

    public async Task<decimal> GetConversionRateAsync(CurrencyType fromCurrency, CurrencyType toCurrency)
    {
        if (fromCurrency == toCurrency)
        {
            return 1m;
        }
        var cacheKey = $"Rate_{fromCurrency}_{toCurrency}";

        if (memoryCache.TryGetValue(cacheKey, out decimal rate))
        {
            return rate;
        }

        await SendApiCallAndPopulateAsync();

        if (memoryCache.TryGetValue(cacheKey, out decimal finalRate))
        {
            return finalRate;
        }

        throw new Exception($"Döviz kuru bilgisi bulunamadı: {fromCurrency} -> {toCurrency}");

    }
    private async Task SendApiCallAndPopulateAsync()
    {
        var symbols = string.Join(",", _supportedCurrencies.Where(c => c != BaseCurrency));

        var requestUrl = $"api/latest.json?app_id={_appId}&base={BaseCurrency}&symbols={symbols}";

        var response = await httpClient.GetFromJsonAsync<ExchangeRateResponse>(requestUrl);

        
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

                memoryCache.Set(rateKey, conversionRate, cacheOptions);
            }
        }
    }
}
    

public class ExchangeRateResponse
{
    public Dictionary<string, decimal>? Rates { get; set; }
}