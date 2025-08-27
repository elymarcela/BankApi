[Fact]
public async Task GetBalance_After_Operations_IsCorrect()
{
    using var db = InMemoryDb();
    var custSvc = new CustomerService(db);
    var accSvc = new AccountService(db);

    var c = await custSvc.CreateCustomerAsync(new CreateCustomerDto
    {
        Name = "User",
        BirthDate = new DateTime(1990, 1, 1),
        Sex = Models.Gender.Male,
        Income = 1000
    });

    var acc = await accSvc.CreateAccountAsync(new CreateAccountDto { CustomerId = c.Id, InitialBalance = 200m });
    await accSvc.DepositAsync(acc.AccountNumber, 150m);   // 350
    await accSvc.WithdrawAsync(acc.AccountNumber, 50m);   // 300

    var bal = await accSvc.GetBalanceAsync(acc.AccountNumber);
    Assert.Equal(300m, bal);
}

[Fact]
public async Task Transactions_Return_In_Order_With_BalanceAfter()
{
    using var db = InMemoryDb();
    var custSvc = new CustomerService(db);
    var accSvc = new AccountService(db);

    var c = await custSvc.CreateCustomerAsync(new CreateCustomerDto
    {
        Name = "User",
        BirthDate = new DateTime(1990, 1, 1),
        Sex = Models.Gender.Female,
        Income = 2000
    });

    var acc = await accSvc.CreateAccountAsync(new CreateAccountDto { CustomerId = c.Id, InitialBalance = 100m });
    await accSvc.DepositAsync(acc.AccountNumber, 20m);   // 120
    await accSvc.WithdrawAsync(acc.AccountNumber, 10m);  // 110

    var list = await accSvc.GetTransactionsAsync(acc.AccountNumber);
    Assert.Equal(3, list.Count);
    Assert.True(list[0].Timestamp <= list[1].Timestamp && list[1].Timestamp <= list[2].Timestamp);
    Assert.Equal(110m, list[^1].BalanceAfter);
}
