namespace VendingMachine.Services.Entities
{
    public class Coin
    {
        public decimal Type { get; set; }

        public int Number { get; set; }

        public Coin(decimal type, int number)
        {
            Type = type;
            Number = number;
        }

        public decimal Value => Number * Type;
    }
}
