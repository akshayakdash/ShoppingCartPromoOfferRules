using ShoppingCart.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Services
{
    public interface ICartService
    {
        /// <summary>
        /// Description: Gets all cart items for user provided by a unique cart id
        /// </summary>
        /// <returns>List of CartItemDto</returns>
        List<CartItemDto> GetCartItems();
        /// <summary>
        /// Description: Add a cart item to cart
        /// </summary>
        /// <param name="cartId">Unique cart id created for user</param>
        /// <param name="cartItem">CartItem to be added to cart</param>
        /// <returns>CartDto with updated cart items</returns>
        CartDto AddItemToCart(Guid cartId, CartItemDto cartItem);
    }
}
