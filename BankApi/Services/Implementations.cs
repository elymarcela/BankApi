using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using BankApi.Data;
using BankApi.Dtos;
using BankApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Services;

public class CustomerService : ICustomerService
{
    private readonly BankDbContext _db;
    public CustomerService(BankDbContext db) => _db = db;

    public async Task<Customer> CreateCustomerAsync(CreateCustomerDto dto)
    {
        var c = new Customer
        {
            Name = dto.Name,
            BirthDate = dto.BirthDate,
            Sex = dto.Sex,
            Income = dto.Income
        };
        _db.Customers.Add(c);
        await _db.SaveChangesAsync();
        return c;
    }

    public async Task<List<Customer>> GetCustomersWithAccountsAsync()
    {
        return await _db.Customers
            .Include(c => c.Accounts)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Customer?> GetCustomerWithAccountsAsync(int id)
    {
        return await _db.Customers
            .Include(c => c.Accounts)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<BankAccount>> GetAccountsByCustomerAsync(int customerId)
    {
        return await _db.BankAccounts
            .Where(a => a.CustomerId == customerId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Transaction>> GetTransactionsByCustomerAsync(int customerId)
    {
        return await _db.Transactions
            .Include(t => t.BankAccount)
            .Where(t => t.BankAccount.CustomerId == customerId)
            .OrderBy(t => t.Timestamp)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> DeleteCustomerAsync(int id)
    {
        var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if (customer is null) return false;

        // Eliminación robusta: borra transacciones y cuentas explícitamente
        var accounts = await _db.BankAccounts
            .Where(a => a.CustomerId == id)
            .Include(a => a.Transactions)
            .ToListAsync();

        _db.Transactions.RemoveRange(accounts.SelectMany(a => a.Transactions));
        _db.BankAccounts.RemoveRange(accounts);
        _db.Customers.Remove(customer);

        await _db.SaveChangesAsync();
        return true;
    }
}

public class AccountService : IAccountService
{
    private readonly BankDbContext _db;
    public AccountService(BankDbContext db) => _db = db;
    public async Task<AccountDetailsDto> GetDetailsAsync(string accountNumber)
    {
        var acc = await _db.BankAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber)
            ?? throw new InvalidOperationException("Cuenta no encontrada.");

        return new AccountDetailsDto
        {
            AccountNumber = acc.AccountNumber,
            CustomerId = acc.CustomerId,
            Balance = acc.Balance,
            InterestRate = acc.InterestRate
        };
    }
    public async Task<BankAccount> CreateAccountAsync(CreateAccountDto dto)
    {
        var customer = await _db.Customers.FindAsync(dto.CustomerId)
                       ?? throw new InvalidOperationException("Customer not found.");

        var account = new BankAccount
        {
            CustomerId = customer.Id,
            AccountNumber = GenerateAccountNumber(),
            Balance = dto.InitialBalance,
            InterestRate = dto.InterestRate
        };

        _db.BankAccounts.Add(account);
        if (dto.InitialBalance > 0)
        {
            _db.Transactions.Add(new Transaction
            {
                BankAccount = account,
                Type = TransactionType.Deposit,
                Amount = dto.InitialBalance,
                BalanceAfter = dto.InitialBalance,
                Timestamp = DateTime.UtcNow
            });
        }

        await _db.SaveChangesAsync();
        return account;
    }

    public async Task<decimal> GetBalanceAsync(string accountNumber)
    {
        var acc = await FindAccount(accountNumber);
        return acc.Balance;
    }

    public async Task<Transaction> DepositAsync(string accountNumber, decimal amount)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        var acc = await FindAccount(accountNumber);

        acc.Balance += amount;

        var tx = new Transaction
        {
            BankAccountId = acc.Id,
            Type = TransactionType.Deposit,
            Amount = amount,
            BalanceAfter = acc.Balance,
            Timestamp = DateTime.UtcNow
        };

        _db.Transactions.Add(tx);
        await _db.SaveChangesAsync();
        return tx;
    }

    public async Task<Transaction> WithdrawAsync(string accountNumber, decimal amount)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        var acc = await FindAccount(accountNumber);

        if (acc.Balance < amount)
            throw new InvalidOperationException("Fondos insuficientes.");

        acc.Balance -= amount;

        var tx = new Transaction
        {
            BankAccountId = acc.Id,
            Type = TransactionType.Withdrawal,
            Amount = amount,
            BalanceAfter = acc.Balance,
            Timestamp = DateTime.UtcNow
        };

        _db.Transactions.Add(tx);
        await _db.SaveChangesAsync();
        return tx;
    }

    public async Task<IReadOnlyList<Transaction>> GetTransactionsAsync(string accountNumber)
    {
        var acc = await FindAccount(accountNumber);
        return await _db.Transactions
            .Where(t => t.BankAccountId == acc.Id)
            .OrderBy(t => t.Timestamp)
            .ToListAsync();
    }

    public async Task<decimal> ApplyInterestAsync(string accountNumber)
    {
        var acc = await FindAccount(accountNumber);
        if (acc.InterestRate <= 0) return acc.Balance;

        var interest = Math.Round(acc.Balance * acc.InterestRate, 2, MidpointRounding.AwayFromZero);
        if (interest == 0) return acc.Balance;

        acc.Balance += interest;

        _db.Transactions.Add(new Transaction
        {
            BankAccountId = acc.Id,
            Type = TransactionType.Interest,
            Amount = interest,
            BalanceAfter = acc.Balance,
            Timestamp = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return acc.Balance;
    }

    private async Task<BankAccount> FindAccount(string accountNumber)
    {
        var acc = await _db.BankAccounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        return acc ?? throw new InvalidOperationException("Cuenta no encontrada.");
    }

    private static string GenerateAccountNumber()
        => DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");

}
