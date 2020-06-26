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

        public CheckoutService(ICartService cartService,
            IPromoRuleService promoRuleService)
        {
            _cartService = cartService;
            _promoRuleService = promoRuleService;
            _promoCalculator = PromoOfferManager.Instance;
        }
        public CartDto Checkout(CartDto cart)
        {
            var cartItems = _cartService.GetCartItems();
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
