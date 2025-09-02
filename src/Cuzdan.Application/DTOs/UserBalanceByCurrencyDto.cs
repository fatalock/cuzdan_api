using Cuzdan.Domain.Enums;

namespace Cuzdan.Application.DTOs
{
    public class UserBalanceByCurrencyDto
    {
        public CurrencyType CurrencyType { get; set; }

        public decimal TotalBalance { get; set; }
    }
}