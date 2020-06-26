using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.Dtos;
using ShoppingCart.Models;

namespace ShoppingCart.Services
{
    public class ProductService : IProductService
    {
        public ProductDto AddProductToStore(ProductDto newProduct)
        {
            throw new NotImplementedException();
        }

        public List<ProductDto> GetProductsFromStore()
        {
            // get the products from json file. Will get it from the db
            var productDirectory = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\{"MasterData\\products.json"}");
            var productsJson = System.IO.File.ReadAllText(productDirectory);
            var productsDto = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProductDto>>(productsJson);
            return productsDto;

        }
    }
}
