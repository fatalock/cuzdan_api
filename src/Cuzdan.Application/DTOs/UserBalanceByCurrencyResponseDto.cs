using Cuzdan.Domain.Enums;

namespace Cuzdan.Application.DTOs
{
    public class UserBalanceByCurrencyResponseDto
    {
        public required CurrencyType Currency { get; set; }
        public decimal TotalBalance { get; set; }
    }
}