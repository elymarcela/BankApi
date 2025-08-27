using System.ComponentModel.DataAnnotations.Schema;

namespace BankApi.Models;

public class Transaction
{
    public int Id { get; set; }

    public int BankAccountId { get; set; }
    public BankAccount BankAccount { get; set; } = null!;

    public TransactionType Type { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal BalanceAfter { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
