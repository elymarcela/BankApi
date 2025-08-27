using BankApi.Dtos;
using BankApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _svc;
    public CustomersController(ICustomerService svc) => _svc = svc;

    [HttpPost]
    public async Task<ActionResult<CustomerResponseDto>> Create([FromBody] CreateCustomerDto dto)
    {
        var c = await _svc.CreateCustomerAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = c.Id },
            new CustomerResponseDto { Id = c.Id, Name = c.Name });
    }

    // Lista de clientes con sus cuentas
    [HttpGet]
    public async Task<ActionResult<List<CustomerListDto>>> GetAll()
    {
        var list = await _svc.GetCustomersWithAccountsAsync();
        var result = list.Select(c => new CustomerListDto
        {
            Id = c.Id,
            Name = c.Name,
            BirthDate = c.BirthDate,
            Sex = (int)c.Sex,
            Income = c.Income,
            Accounts = c.Accounts.Select(a => new AccountItemDto
            {
                AccountNumber = a.AccountNumber,
                Balance = a.Balance
            }).ToList(),
            TotalBalance = c.Accounts.Sum(a => a.Balance)
        }).ToList();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerDetailsDto>> GetById(int id)
    {
        var c = await _svc.GetCustomerWithAccountsAsync(id);
        if (c is null) return NotFound("Cliente no encontrado.");
        var dto = new CustomerDetailsDto
        {
            Id = c.Id,
            Name = c.Name,
            BirthDate = c.BirthDate,
            Sex = (int)c.Sex,
            Income = c.Income,
            Accounts = c.Accounts.Select(a => new AccountItemDto
            {
                AccountNumber = a.AccountNumber,
                Balance = a.Balance
            }).ToList()
        };
        return Ok(dto);
    }

    [HttpGet("{id:int}/accounts")]
    public async Task<ActionResult<List<AccountItemDto>>> GetAccounts(int id)
    {
        var accounts = await _svc.GetAccountsByCustomerAsync(id);
        var result = accounts.Select(a => new AccountItemDto
        {
            AccountNumber = a.AccountNumber,
            Balance = a.Balance
        }).ToList();
        return Ok(result);
    }

    [HttpGet("{id:int}/transactions")]
    public async Task<ActionResult<List<TransactionViewDto>>> GetTransactionsByCustomer(int id)
    {
        var txs = await _svc.GetTransactionsByCustomerAsync(id);
        var result = txs.Select(t => new TransactionViewDto
        {
            Id = t.Id,
            AccountNumber = t.BankAccount.AccountNumber,
            Type = (int)t.Type,
            Amount = t.Amount,
            BalanceAfter = t.BalanceAfter,
            Timestamp = t.Timestamp
        }).ToList();
        return Ok(result);
    }

    // Eliminar cliente (y en cascada sus cuentas/transacciones)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _svc.DeleteCustomerAsync(id);
        if (!ok) return NotFound("Cliente no encontrado.");
        return NoContent();
    }
}
