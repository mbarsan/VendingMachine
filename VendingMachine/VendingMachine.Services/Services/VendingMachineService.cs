using VendingMachine.Services.Data;
using VendingMachine.Services.Entities;
using VendingMachine.Services.Interfaces;

namespace VendingMachine.Services.Services
{
    public class VendingMachineService : IVendingMachineService
    {
        public VendingMachineService()
        {
            BalanceService.FillVendingMachine();
            ProductsService.Reset();
        }

        public async Task<decimal> AddCoinAsync(Coin payedCoin, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return BalanceService.Balance;
            }

            await Task.Delay(300, cancellationToken);
            BalanceService.AddCoin(payedCoin);

            return BalanceService.Balance;
        }

        public async Task RefundBalanceAsync()
        {
            await BalanceService.RefundBalanceAsync();
        }

        public string GetProductNames()
        {
            return string.Join(", ", ProductsService.GetProducts().Select(product => product.Id + ". " + product.Name + "-> Price =" + product.Price));
        }

        public async Task<(string ProductName, decimal change)> DeliverAsync(int productId, CancellationToken cancellationToken)
        {
            Product? product = ProductsService.GetProduct(productId);

            if (!BalanceService.IsProductPricePayed(product?.Price ?? 0))
            {
                return new ValueTuple<string, decimal>("0", 0);
            }
            await Task.Delay(300, cancellationToken);

            Product? removedProduct = ProductsService.RemoveProduct(productId);

            if (removedProduct == null)
            {
                return new ValueTuple<string, decimal>(string.Empty, 0);
            }

            List<Coin> coinsToRefund = await BalanceService.RefundChange(removedProduct.Price);
            if (coinsToRefund == null)
            {
                return new ValueTuple<string, decimal>(removedProduct.Name, -1);
            }

            decimal change = coinsToRefund.Sum(coin => coin.Value);

            return new ValueTuple<string, decimal>(removedProduct.Name, change);
        }

        public void Reset()
        {
            BalanceService.Reset();
            BalanceService.FillVendingMachine();
            ProductsService.Reset();
        }

        private decimal CalculatePayedSum(List<Coin> payedCoins)
        {
            decimal payedSum = 0;

            payedCoins.ForEach(coin =>
            {
                payedSum += coin.Value;
            });

            return payedSum;
        }
    }
}
