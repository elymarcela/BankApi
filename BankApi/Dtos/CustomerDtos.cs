using BankApi.Models;
using System.ComponentModel.DataAnnotations;

namespace BankApi.Dtos;

public class CreateCustomerDto
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = null!;
    [Required]
    public DateTime BirthDate { get; set; }
    [Required]
    public Gender Sex { get; set; }
    public decimal Income { get; set; }
}

public class CustomerResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}
