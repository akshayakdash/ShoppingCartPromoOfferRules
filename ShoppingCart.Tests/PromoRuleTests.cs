﻿using ShoppingCart.Dtos;
using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace ShoppingCart.Tests
{
    public class PromoRuleTests
    {
        private PromoOfferManager _promoCalculator;

        public PromoRuleTests()
        {
            _promoCalculator = PromoOfferManager.Instance;
        }

        [Fact]
        public void Given_An_Unmatched_Promo_Rule_For_An_Item_With_SKU_Should_Not_Apply_Promo_To_The_Item()
        {
            var promoRules = new List<PromoOffer> {
                new PromoOffer
                {
                    PromotionOfferId = "1",
                    PromotionOfferDescription = "3 D's for 130",
                    PromoRule = new PromoRule { IsForDifferentItems = false, Quantity = 3, SKUs = new List<string>{ "D" }, PromoResult = new PromoResult { OffFixedPrice = 130 } },
                },
            };

            var cartItems = new List<Dtos.CartItemDto> {
                    new CartItemDto { SKU = "A", Quantity = 3, UnitPrice = 50 },
            };
            var cartItemGroupedByPromo = _promoCalculator.ApplyPromoRule(cartItems, promoRules);
            foreach (var item in cartItemGroupedByPromo)
            {
                Assert.NotEqual(item.Key, promoRules.First().PromotionOfferId);
                Assert.False(item.First().PromoApplied);
            }

        }
    }
}