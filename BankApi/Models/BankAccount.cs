using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApi.Models;

public class BankAccount
{
    public int Id { get; set; }

    [Required, MaxLength(30)]
    public string AccountNumber { get; set; } = null!;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Balance { get; set; }

    [Column(TypeName = "decimal(5,4)")]
    public decimal InterestRate { get; set; } = 0m;

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
