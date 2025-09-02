namespace Cuzdan.Application.DTOs
{
    public class UserBalanceByCurrencyResponseDto
    {
        public required string Currency { get; set; } // Artık string
        public decimal TotalBalance { get; set; }
    }
}