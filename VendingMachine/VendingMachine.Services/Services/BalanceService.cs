using VendingMachine.Services.Entities;

namespace VendingMachine.Services.Data
{
    public static class BalanceService
    {
        private static decimal _balance;
        private static List<Coin> _coins = new List<Coin>();

        public static decimal Balance => _balance;

        public static void Reset()
        {
            _balance = 0;
            _coins.Clear();
        }

        public static void FillVendingMachine()
        {
            _coins.Add(new Coin(0.05M, 2));
            _coins.Add(new Coin(0.10M, 2));
            _coins.Add(new Coin(0.20M, 2));
            _coins.Add(new Coin(0.50M, 2));
            _coins.Add(new Coin(1, 2));
            _coins.Add(new Coin(2, 2));
        }

        public static void AddCoin(Coin newCoin)
        {
            Coin? existingCoin = _coins.FirstOrDefault(coin => coin.Type == newCoin.Type);

            if (existingCoin != null)
            {
                existingCoin.Number += newCoin.Number;
            }
            else
            {
                _coins.Add(newCoin);
            }

            _balance += newCoin.Value;
        }

        public async static Task Refund(Coin coinToRefund)
        {
            await Task.Delay(1000);
            RemoveCoin(coinToRefund);
            _balance -= coinToRefund.Value;
        }

        //Returns list of coin types to be refunded
        public async static Task<List<Coin>> RefundChange(decimal productPrice)
        {
            decimal sumToRefund = _balance - productPrice;

            if (sumToRefund == 0)
            {
                return new List<Coin>();
            }
            List<Coin> coinsToRemove = [];
            foreach (var coin in _coins.OrderByDescending(coin => coin.Type))
            {
               int numberOfCoinsOfTypeToRefund = (int)(sumToRefund/ coin.Type);

                if (numberOfCoinsOfTypeToRefund == 0)
                {
                    continue;
                }
                
                if (numberOfCoinsOfTypeToRefund >= coin.Number)
                {
                    numberOfCoinsOfTypeToRefund = coin.Number;
                }

                coinsToRemove.Add(new Coin(coin.Type, numberOfCoinsOfTypeToRefund));

                sumToRefund -= coin.Type;
                if (sumToRefund <= 0)
                {
                    break;
                }
            }

            //returns null if the coins are not available for the sum to be refunded
            if (sumToRefund > 0)
            {
                return null;
            }

            foreach (var coin in coinsToRemove)
            {
                await Task.Delay(100);
                RemoveCoin(coin);
            }
            _balance = 0;

            return coinsToRemove;
        }

        public async static Task RefundBalanceAsync()
        {
            await RefundChange(_balance);
        }

        public static bool IsProductPricePayed(decimal price)
        {
            return _balance - price >= 0;
        }

        private static void RemoveCoin(Coin coinToRemove)
        {
            Coin? existingCoin = _coins.FirstOrDefault(coin => coin.Type == coinToRemove.Type);

            if (existingCoin == null)
            {
                return;
            }

            existingCoin.Number -= coinToRemove.Number;
            if (existingCoin.Number <= 0)
            {
                _coins.Remove(coinToRemove);
            }
        }
    }
}
