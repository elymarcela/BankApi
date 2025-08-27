using System.ComponentModel.DataAnnotations;

namespace BankApi.Models;

public class Customer
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = null!;

    [Required]
    public DateTime BirthDate { get; set; }

    [Required]
    public Gender Sex { get; set; }

    public decimal Income { get; set; }

    public ICollection<BankAccount> Accounts { get; set; } = new List<BankAccount>();
}
