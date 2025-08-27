namespace BankApi.Dtos
{
    public class TransactionViewDto
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = null!;
        public int Type { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
