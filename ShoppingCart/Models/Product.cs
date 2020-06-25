using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Models
{
    public class Product
    {
        public string SKU { get; private set; }
        public decimal Price { get; private set; }

        public Product(string sku, decimal price)
        {
            if (string.IsNullOrEmpty(sku))
                throw new Exception("SKU is required for Product.");
            if (price == default(decimal) || price < 0)
                throw new Exception("Invalid price for product.");

            SKU = sku;
            Price = price;
        }
    }
}
