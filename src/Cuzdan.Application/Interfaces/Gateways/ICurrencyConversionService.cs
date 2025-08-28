using Cuzdan.Domain.Enums;

namespace Cuzdan.Application.Interfaces;

public interface ICurrencyConversionService
{
    Task<decimal> GetConversionRateAsync(CurrencyType fromCurrency, CurrencyType toCurrency);
}