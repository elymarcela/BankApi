using System.ComponentModel.DataAnnotations;

namespace BankApi.Dtos;

public class CustomerListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public int Sex { get; set; }
    public decimal Income { get; set; }
    public List<AccountItemDto> Accounts { get; set; } = new();
    public decimal TotalBalance { get; set; }
}

public class AccountItemDto
{
    [Required]
    public string AccountNumber { get; set; } = null!;
    public decimal Balance { get; set; }
}

public class CustomerDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public int Sex { get; set; }
    public decimal Income { get; set; }
    public List<AccountItemDto> Accounts { get; set; } = new();
}


