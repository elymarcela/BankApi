namespace BankApi.Dtos;

public class AccountDetailsDto
{
    public string AccountNumber { get; set; } = null!;
    public int CustomerId { get; set; }
    public decimal Balance { get; set; }
    public decimal InterestRate { get; set; }
}
