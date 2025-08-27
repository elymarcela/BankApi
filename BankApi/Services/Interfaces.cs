using BankApi.Dtos;
using BankApi.Models;

namespace BankApi.Services;

public interface ICustomerService
{
    Task<Customer> CreateCustomerAsync(CreateCustomerDto dto);
    Task<List<Customer>> GetCustomersWithAccountsAsync();
    Task<Customer?> GetCustomerWithAccountsAsync(int id);
    Task<List<BankAccount>> GetAccountsByCustomerAsync(int customerId);
    Task<List<Transaction>> GetTransactionsByCustomerAsync(int customerId);
    Task<bool> DeleteCustomerAsync(int id); 
}

public interface IAccountService
{
    Task<BankAccount> CreateAccountAsync(CreateAccountDto dto);
    Task<decimal> GetBalanceAsync(string accountNumber);
    Task<Transaction> DepositAsync(string accountNumber, decimal amount);
    Task<Transaction> WithdrawAsync(string accountNumber, decimal amount);
    Task<IReadOnlyList<Transaction>> GetTransactionsAsync(string accountNumber);
    Task<decimal> ApplyInterestAsync(string accountNumber);
    Task<AccountDetailsDto> GetDetailsAsync(string accountNumber);
}
