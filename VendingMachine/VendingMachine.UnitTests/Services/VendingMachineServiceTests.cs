using Moq;
using VendingMachine.Services.Data;
using VendingMachine.Services.Entities;
using VendingMachine.Services.Services;

public class VendingMachineServiceTests
{
    private VendingMachineService _vendingMachineService;

    [SetUp]
    public void Setup()
    {
        _vendingMachineService = new();
    }

    [Test]
    public async Task AddCoinAsync_IfCalcellation_RefundIsCalled()
    {
        //Arrange
        BalanceService.Reset();
        BalanceService.FillVendingMachine();
        decimal initialBalance = BalanceService.Balance;

        //Act
        decimal balance = await _vendingMachineService.AddCoinAsync(new Coin(1, 1), new CancellationToken(true));

        //Assert
        Assert.That(BalanceService.Balance, Is.EqualTo(initialBalance));
    }

    [Test]
    public async Task AddCoinAsync_IfNoCalcellation_BalanceUpdates()
    {
        //Arrange
        BalanceService.Reset();
        BalanceService.FillVendingMachine();
        Coin newCoin = new Coin(1, 1);
        decimal initialBalance = BalanceService.Balance;

        //Act
        decimal balance = await _vendingMachineService.AddCoinAsync(newCoin, new CancellationToken(false));

        //Assert
        Assert.That(BalanceService.Balance, Is.EqualTo(initialBalance + 1));
    }

    [Test]
    public async Task DeliverAsync_IfNoBalanceForPrice_ReturnsZero()
    {
        //Arrange
        BalanceService.Reset();
        BalanceService.FillVendingMachine();
        Coin newCoin = new Coin(1, 1);
        decimal initialBalance = BalanceService.Balance;

        //Act
        (string productName, decimal change) = await _vendingMachineService.DeliverAsync(2, It.IsAny<CancellationToken>());

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(productName, Is.EqualTo("0"));
            Assert.That(change, Is.EqualTo(0));
        });
    }

    [Test]
    public async Task DeliverAsync_IfZeroQuantityForProduct_ReturnsEmpty()
    {
        //Arrange
        ProductsService.Reset();
        ProductsService.RemoveProduct(1);
        ProductsService.RemoveProduct(1);

        //Act
        (string productName, decimal change) = await _vendingMachineService.DeliverAsync(1, It.IsAny<CancellationToken>());

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(productName, Is.EqualTo(string.Empty));
            Assert.That(change, Is.EqualTo(0));
        });
    }

    [Test]
    public async Task DeliverAsync_IfNoCoinsForRefund_ReturnsMinusOne()
    {
        //Arrange
        ProductsService.Reset();
        BalanceService.Reset();
        Coin newCoin = new Coin(1M, 3);
        BalanceService.AddCoin(newCoin);

        //Act
        (string productName, decimal change) = await _vendingMachineService.DeliverAsync(1, It.IsAny<CancellationToken>());

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(productName, Is.EqualTo("Coke"));
            Assert.That(change, Is.EqualTo(-1));
        });
    }

    [Test]
    public async Task DeliverAsync_IfAllGood_ReturnsNameAndChange()
    {
        //Arrange
        ProductsService.Reset();
        BalanceService.Reset();
        BalanceService.FillVendingMachine();
        Coin newCoin = new Coin(1M, 3);
        BalanceService.AddCoin(newCoin);

        //Act
        (string productName, decimal change) = await _vendingMachineService.DeliverAsync(2, It.IsAny<CancellationToken>());

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(productName, Is.EqualTo("Pepsi"));
            Assert.That(change, Is.EqualTo(1.55));
        });
    }
}