using VendingMachine.Services.Data;
using VendingMachine.Services.Entities;

public class ProductServiceTests
{
    [Test]
    public void RemoveProduct_IfNoProductFound_ReturnsNull()
    {
        //Act
        Product? removed = ProductsService.RemoveProduct(4);

        //Assert
        Assert.That(removed, Is.Null);
    }

    [Test]
    public void RemoveProduct_ProductFound_QuantityDecreases()
    {
        //Arrange
        ProductsService.Reset();
        int initQty = ProductsService.GetProduct(1).Quantity;

        //Act
        Product? removed = ProductsService.RemoveProduct(1);
        int endQty = ProductsService.GetProduct(1).Quantity;

        //Assert
        Assert.That(endQty, Is.EqualTo(initQty - 1));
    }

    [Test]
    public void RemoveProduct_ProductFoundZeroQuantity_ProductRemainsQuantityStaysZero()
    {
        //Arrange
        ProductsService.Reset();
        int firstQty = ProductsService.GetProduct(1).Quantity;

        //Act
        Product? removed = ProductsService.RemoveProduct(1);
        int secondQty = ProductsService.GetProduct(1).Quantity;
        removed = ProductsService.RemoveProduct(1);
        int thirdQty = ProductsService.GetProduct(1).Quantity;
        removed = ProductsService.RemoveProduct(1);
        int endQty = ProductsService.GetProduct(1).Quantity;

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(secondQty, Is.EqualTo(1));
            Assert.That(thirdQty, Is.EqualTo(0));
            Assert.That(endQty, Is.EqualTo(0));
        });
    }
}