using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Dtos;
using ShoppingCart.Models;
using ShoppingCart.Services;

namespace ShoppingCart.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly IPromoRuleService _promoRuleService;
        private readonly ICheckoutService _checkoutService;
        private PromoOfferManager _promoCalculator;

        public ShoppingCartController(IProductService productService,
            ICartService cartService,
            IPromoRuleService promoRuleService,
            ICheckoutService checkoutService)
        {
            _productService = productService;
            _cartService = cartService;
            _promoRuleService = promoRuleService;
            _promoCalculator = PromoOfferManager.Instance;
            _checkoutService = checkoutService;
        }

        /// <summary>
        /// Gets Products from store
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<ProductDto>> Products()
        {
            var products = _productService.GetProductsFromStore();
            return products;
        }

        /// <summary>
        /// Gets promo offers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<PromoOffer>> PromoOffers()
        {
            var promoRules = _promoRuleService.GetPromoRules();
            return promoRules;
        }

        /// <summary>
        /// Gets active promo offers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<PromoOffer>> ActivePromoOffers()
        {
            var promoRules = _promoRuleService.GetPromoRules()
                .Where(p => p.ValidTill > DateTime.Today)
                .ToList();
            return promoRules;
        }

        /// <summary>
        /// Gets default cartitems stored in json file
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<CartItemDto>> CartItems()
        {
            var cartItems = _cartService.GetCartItems();
            return cartItems;
        }

        /// <summary>
        /// Checkout a cart with valid cart items
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<CartDto> CheckOut(CartDto cart)
        {
            var defaultCartItems = _cartService.GetCartItems();
            var cartDto = new CartDto
            {
                CartId = Guid.NewGuid().ToString(),
                CartItems = cart.CartItems != null && cart.CartItems.Count() > 0 ? cart.CartItems : defaultCartItems
            };
            var result = _checkoutService.Checkout(cartDto);
            return result;
        }
    }
}
