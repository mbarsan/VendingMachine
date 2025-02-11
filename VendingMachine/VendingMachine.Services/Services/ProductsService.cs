using VendingMachine.Services.Entities;

namespace VendingMachine.Services.Data
{
    public static class ProductsService
    {
        private static List<Product> _products = new List<Product>();

        public static void Reset()
        {
            _products.Clear();
            _products.Add(new Product(1, "Coke", 1.50M, 2));
            _products.Add(new Product(2, "Pepsi", 1.45M, 5));
            _products.Add(new Product(3, "Water", 0.90M, 10));
        }

        public static Product? RemoveProduct(int productId)
        {
            Product? existingProduct = _products.FirstOrDefault(p => p.Id == productId);

            if (existingProduct == null || existingProduct.Quantity == 0)
            {
                return null;
            }
    
            existingProduct.Quantity -= 1;

            return existingProduct;
        }

        public static List<Product> GetProducts()
        {
            return _products;
        }

        public static Product? GetProduct(int productId)
        {
            return _products.FirstOrDefault(prod => prod.Id == productId);
        }
    }
}
