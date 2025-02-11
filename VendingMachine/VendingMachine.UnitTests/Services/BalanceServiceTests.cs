using VendingMachine.Services.Data;
using VendingMachine.Services.Entities;

public class BalanceServiceTests
{
    [Test]
    public void AddCoin_IfNewCoinAdded_BalanceIsUpdated()
    {
        //Arrange
        BalanceService.Reset();
        Coin newCoin = new Coin(0.10M, 3);

        //Act
        BalanceService.AddCoin(newCoin);

        //Assert
        Assert.That(BalanceService.Balance, Is.EqualTo(0.30M));
    }

    [Test]
    public async Task Refund_IfCoinRefunded_BalanceIsUpdated()
    {
        //Arrange
        BalanceService.Reset();
        Coin newCoin = new Coin(0.10M, 3);
        Coin coinToRefund = new Coin(0.20M, 1);

        //Act
        BalanceService.AddCoin(newCoin);
        await BalanceService.Refund(coinToRefund);

        //Assert
        Assert.That(BalanceService.Balance, Is.EqualTo(0.10M));
    }

    [Test]
    public async Task RefundChange_IfNoCoinsToSplit_ReturnsNegativeValue()
    {
        //Arrange
        BalanceService.Reset();

        Coin newCoin = new Coin(1M, 3);
        BalanceService.AddCoin(newCoin);
        decimal balance = BalanceService.Balance;

        //Act
        List<Coin> coinsToRefund = await BalanceService.RefundChange(1.45M);

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(BalanceService.Balance, Is.EqualTo(balance));
            Assert.That(coinsToRefund, Is.Null);
        });
    }

    [Test]
    public async Task RefundChange_IfNoSumToRefund_ReturnsEmptyList()
    {
        //Arrange
        BalanceService.Reset();

        //Act
        List<Coin> coinsToRefund = await BalanceService.RefundChange(0);

        //Assert
        Assert.That(coinsToRefund.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task RefundChange_IfChangeRefunded_BalanceIsUpdated()
    {
        //Arrange
        BalanceService.Reset();
        BalanceService.FillVendingMachine();
        BalanceService.AddCoin(new Coin(1M, 3));

        //Act
        List<Coin> coinsToRefund = await BalanceService.RefundChange(1.45M);

        //Assert
        Assert.Multiple(() =>
        { 
            Assert.That(BalanceService.Balance, Is.EqualTo(0));
            Assert.That(coinsToRefund.Count, Is.EqualTo(3));
            Assert.That(coinsToRefund[0].Type, Is.EqualTo(1));
            Assert.That(coinsToRefund[1].Type, Is.EqualTo(0.50M));
            Assert.That(coinsToRefund[2].Type, Is.EqualTo(0.05M));
            Assert.That(coinsToRefund[0].Number, Is.EqualTo(1));
            Assert.That(coinsToRefund[1].Number, Is.EqualTo(1));
            Assert.That(coinsToRefund[2].Number, Is.EqualTo(1));
        });
    }
}