using BankApi.Dtos;
using BankApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _svc;
    public AccountsController(IAccountService svc) => _svc = svc;

    [HttpPost]
    public async Task<ActionResult<AccountResponseDto>> Create([FromBody] CreateAccountDto dto)
    {
        try
        {
            var acc = await _svc.CreateAccountAsync(dto);
            return CreatedAtAction(nameof(GetBalance), new { accountNumber = acc.AccountNumber },
                new AccountResponseDto { AccountNumber = acc.AccountNumber, Balance = acc.Balance });
        }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }
    [HttpGet("{accountNumber}/transactions")]
    public async Task<ActionResult<List<TransactionViewDto>>> GetTransactions(string accountNumber)
    {
        try
        {
            var txs = await _svc.GetTransactionsAsync(accountNumber);
            var result = txs.Select(t => new TransactionViewDto
            {
                Id = t.Id,
                AccountNumber = accountNumber,
                Type = (int)t.Type,
                Amount = t.Amount,
                BalanceAfter = t.BalanceAfter,
                Timestamp = t.Timestamp
            }).ToList();

            return Ok(result);
        }
        catch (InvalidOperationException ex) { return NotFound(ex.Message); }
    }
    [HttpGet("{accountNumber}/balance")]
    public async Task<ActionResult<AccountResponseDto>> GetBalance(string accountNumber)
    {
        try
        {
            var bal = await _svc.GetBalanceAsync(accountNumber);
            return Ok(new AccountResponseDto { AccountNumber = accountNumber, Balance = bal });
        }
        catch (InvalidOperationException ex) { return NotFound(ex.Message); }
    }

    [HttpPost("{accountNumber}/deposit")]
    public async Task<ActionResult> Deposit(string accountNumber, [FromBody] MoneyOperationDto dto)
    {
        try
        {
            var tx = await _svc.DepositAsync(accountNumber, dto.Amount);
            return Ok(tx);
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpPost("{accountNumber}/withdraw")]
    public async Task<ActionResult> Withdraw(string accountNumber, [FromBody] MoneyOperationDto dto)
    {
        try
        {
            var tx = await _svc.WithdrawAsync(accountNumber, dto.Amount);
            return Ok(tx);
        }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    // 👇 Nuevo: detalles de la cuenta (incluye CustomerId)
    [HttpGet("{accountNumber}")]
    public async Task<ActionResult<AccountDetailsDto>> GetDetails(string accountNumber)
    {
        try
        {
            var dto = await _svc.GetDetailsAsync(accountNumber);
            return Ok(dto);
        }
        catch (InvalidOperationException ex) { return NotFound(ex.Message); }
    }

    [HttpPost("{accountNumber}/apply-interest")]
    public async Task<ActionResult<AccountResponseDto>> ApplyInterest(string accountNumber)
    {
        try
        {
            var bal = await _svc.ApplyInterestAsync(accountNumber);
            return Ok(new AccountResponseDto { AccountNumber = accountNumber, Balance = bal });
        }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }
}
