using ShoppingCart.Dtos;
using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Services
{
    public interface IProductService
    {
        /// <summary>
        /// Description: Gets the list of products available in store
        /// </summary>
        /// <returns>List of Products</returns>
        List<ProductDto> GetProductsFromStore();
        /// <summary>
        /// Description: Inserts a new product to catalog
        /// </summary>
        /// <param name="newProduct">Product object to be added to Product store</param>
        /// <returns>Product created</returns>
        ProductDto AddProductToStore(ProductDto newProduct);
        
    }
}
