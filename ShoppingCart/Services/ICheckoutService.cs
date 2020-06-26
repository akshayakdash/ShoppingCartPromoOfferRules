using ShoppingCart.Dtos;
using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Services
{
    public interface ICheckoutService
    {
        /// <summary>
        /// Checkout cart with cart items
        /// </summary>
        /// <param name="cart"></param>
        CartDto Checkout(CartDto cart);
    }
}
