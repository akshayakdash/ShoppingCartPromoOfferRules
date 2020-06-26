using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.Dtos;

namespace ShoppingCart.Services
{
    public class CartService : ICartService
    {
        public CartDto AddItemToCart(Guid cartId, CartItemDto cartItem)
        {
            throw new NotImplementedException();
        }

        public List<CartItemDto> GetCartItems()
        {
            var cartDir = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\{"MasterData\\cart.json"}");
            var cartJson = System.IO.File.ReadAllText(cartDir);
            var cartDto = Newtonsoft.Json.JsonConvert.DeserializeObject<CartDto>(cartJson);
            return cartDto.CartItems;
        }
    }
}
