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
        private readonly IPromoRuleService _promoRuleServices;
        private PromoOfferManager _promoCalculator;

        public ShoppingCartController(IProductService productService,
            ICartService cartService,
            IPromoRuleService promoRuleService)
        {
            _productService = productService;
            _cartService = cartService;
            _promoRuleServices = promoRuleService;
            _promoCalculator = PromoOfferManager.Instance;
        }
        [HttpGet]
        public ActionResult<IEnumerable<ProductDto>> Products()
        {
            var products = _productService.GetProductsFromStore();
            return products;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PromoOffer>> PromoOffers()
        {
            var promoRules = _promoRuleServices.GetPromoRules();
            return promoRules;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PromoOffer>> ActivePromoOffers()
        {
            var promoRules = _promoRuleServices.GetPromoRules()
                .Where(p => p.ValidTill > DateTime.Today)
                .ToList();
            return promoRules;
        }
    }
}
