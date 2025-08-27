using System.ComponentModel.DataAnnotations;

namespace BankApi.Dtos;

public class CreateAccountDto
{
    [Required]
    public int CustomerId { get; set; }
    [Range(0, double.MaxValue)]
    public decimal InitialBalance { get; set; } = 0m;
    [Range(0, 1)]
    public decimal InterestRate { get; set; } = 0m;
}

public class MoneyOperationDto
{
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }
}

public class AccountResponseDto
{
    public string AccountNumber { get; set; } = null!;
    public decimal Balance { get; set; }
}
