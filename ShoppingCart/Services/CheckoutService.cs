using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.Dtos;
using ShoppingCart.Models;

namespace ShoppingCart.Services
{
    public class CheckoutService : ICheckoutService
    {
        private ICartService _cartService;
        private IPromoRuleService _promoRuleService;
        private PromoOfferManager _promoCalculator;
        private IProductService _productService;

        public CheckoutService(ICartService cartService,
            IPromoRuleService promoRuleService,
            IProductService productService)
        {
            _cartService = cartService;
            _promoRuleService = promoRuleService;
            _productService = productService;
            _promoCalculator = PromoOfferManager.Instance;
        }
        public CartDto Checkout(CartDto cart)
        {
            var products = _productService.GetProductsFromStore();
            var cartItems = cart.CartItems != null && cart.CartItems.Count() > 0 ? cart.CartItems : _cartService.GetCartItems(); // TO DO: This will be the cart.CartItems
            cartItems.ForEach(item =>
            {
                if (!products.Select(p => p.SKU).Contains(item.SKU))
                {
                    throw new Exception("Invalid item added to Cart.");
                }
                item.UnitPrice = products.First(p => p.SKU == item.SKU).Price;
            });
            var promoOffers = _promoRuleService.GetPromoRules();
            var cartItemsWithOfferPrice = _promoCalculator.CalculateOfferPrice(_promoCalculator.ApplyPromoRule(cartItems, promoOffers), promoOffers);
            var cartDto = new CartDto
            {
                CartId = !string.IsNullOrEmpty(cart.CartId) ? cart.CartId : Guid.NewGuid().ToString(),
                CartItems = cartItemsWithOfferPrice
            };
            return cartDto;
        }
    }
}
