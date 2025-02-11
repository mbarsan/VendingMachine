using VendingMachine.Services.Entities;
using VendingMachine.Services.Interfaces;

namespace VendingMachine.Simulator
{
    public class VendingMachineSimulator
    {
        private readonly IVendingMachineService _vendingMachineService;

        public VendingMachineSimulator(IVendingMachineService vendingMachineService)
        {
            _vendingMachineService = vendingMachineService;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            bool restart = true;
            bool fromTheStart = true;

            do
            {
                if (fromTheStart)
                {
                    int start = StartProcedure();
                    if (start == -1)
                    {
                        return;
                    }
                    if (start == 0)
                    {
                        fromTheStart = true;
                        continue;
                    }
                }

                bool proceed = await RequestAddCoinsAsync(cancellationToken);
                if (!proceed)
                {
                    restart = RestartConfirmation();
                    if (restart)
                    {
                        fromTheStart = false;
                        continue;
                    }
                }

                int productId = await RequestTheProductSelectionAsync(cancellationToken);
                if (productId == -1)
                {
                    restart = RestartConfirmation();
                    if (restart)
                    {
                        fromTheStart = false;
                        continue;
                    }
                }

                await DeliverTheProductAsync(productId, cancellationToken);
                restart = RestartConfirmation();
                if (restart)
                {
                    fromTheStart = false;
                    continue;
                }
                restart = true;
                fromTheStart = true;
            } while (restart);
        }

        private async Task<bool> RequestAddCoinsAsync(CancellationToken cancellationToken)
        {
            decimal[] acceptedCoinValues = [0.05M, 0.10M, 0.20M, 0.50M, 1, 2];
            var coinValuesString = string.Join(", ", acceptedCoinValues);
            string? option = "y";
            do
            {
                Console.Write("Coin type:");
                decimal value = 0;

                do
                {
                    var coinType = Console.ReadLine();
                    bool isDecimal = decimal.TryParse(coinType, out value);

                    if (isDecimal)
                    {
                        if (!acceptedCoinValues.Contains(value))
                        {
                            Console.WriteLine("Please enter a valid option.");
                            Console.WriteLine($"Possible values are: {coinValuesString}.");
                            value = 0;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Please enter a valid option.");
                        Console.WriteLine($"Possible values are: {coinValuesString}.");
                        value = 0;
                    }
                } while (value == 0);

                decimal currentBalance = await _vendingMachineService.AddCoinAsync(new Coin(value, 1), cancellationToken);
                Console.WriteLine($"Your balance is {currentBalance}");
                Console.WriteLine("Do you want another coin? (y/n/c)");
                option = WaitForConfirmation();

            } while (option == "y");

            if (option == "c")
            {
                await _vendingMachineService.RefundBalanceAsync();
                return false;
            }

            return true;
        }

        private async Task<int> RequestTheProductSelectionAsync(CancellationToken cancellationToken)
        {
            string? option = "y";
            string? id;
            do
            {
                Console.WriteLine("Select the product from the list.");
                Console.WriteLine(_vendingMachineService.GetProductNames());

                id = Console.ReadLine();

                while (id != "1" && id != "2" && id != "3")
                {
                    Console.Write("Please enter a valid option. Possible values are: 1, 2, and 3.");
                    id = Console.ReadLine();
                }

                Console.WriteLine("Do you confirm the selection? (y/n/c)");

                option = WaitForConfirmation();
                if (option == "c")
                {
                    await _vendingMachineService.RefundBalanceAsync();
                    Console.WriteLine("Have a nice day!");
                    return -1;
                }
            } while (option == "n");

            return int.Parse(id);
        }

        private async Task DeliverTheProductAsync(int productId, CancellationToken cancellationToken)
        {
            do
            {
                (string delivered, decimal change) = await _vendingMachineService.DeliverAsync(productId, cancellationToken);
                if (!string.IsNullOrEmpty(delivered))
                {
                    if (delivered == "0")
                    {
                        Console.WriteLine($"You don't have enough balance for this product.");
                        bool proceed = await RequestAddCoinsAsync(cancellationToken);
                        if (proceed)
                        {
                            continue;
                        }
                        else
                        {
                            return;
                        }
                    }

                    Console.WriteLine($"Enjoy your {delivered}.");
                    if (change == -1)
                    {
                        Console.WriteLine("No available coins to deliver change. Please contact support.");
                    }
                    if (change > 0)
                    {
                        Console.WriteLine($"Please take your change {change}.");
                    }
                    break;
                }
                else
                {
                    Console.WriteLine("This product is no longer available, Please select another product.");
                    productId = await RequestTheProductSelectionAsync(cancellationToken);
                    if (productId == -1)
                    {
                        return;
                    }
                }
            } while (true);
        }

        private string WaitForConfirmation()
        {
            var option = Console.ReadLine();
            while (option != "y" && option != "n" && option != "c")
            {
                Console.Write("Please enter a valid option. y for yes n for no or c for cancel.");
                option = Console.ReadLine();
            }

            return option;
        }

        private bool RestartConfirmation()
        {
            Console.WriteLine("Do you want to order another product (y/n)?");
            var option = Console.ReadLine();
            return option == "y";
        }

        //returns -1 to exit app, 0 to contiune to next iteration, 1 to continue into loop
        private int StartProcedure()
        {
            Console.WriteLine("Welcome to Vending Machine.");
            Console.WriteLine("Actions available: Press m for maintanance, p to get product, any key to exit.");
            var option = Console.ReadLine();

            if (option == "m")
            {
                ResetVedingMachine();
                return 0;
            }

            if (option == "p")
            {
                return 1;
            }

            return -1;
        }

        private void ResetVedingMachine()
        {
            _vendingMachineService.Reset();
        }
    }
}