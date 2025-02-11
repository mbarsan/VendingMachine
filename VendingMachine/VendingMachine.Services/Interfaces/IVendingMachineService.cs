using VendingMachine.Services.Entities;

namespace VendingMachine.Services.Interfaces
{
    public interface IVendingMachineService
    {
        Task<decimal> AddCoinAsync(Coin payedCoin, CancellationToken cancellationToken);

        Task RefundBalanceAsync();

        string GetProductNames();

        Task<(string ProductName, decimal change)> DeliverAsync(int productId, CancellationToken cancellationToken);

        void Reset();
    }
}
