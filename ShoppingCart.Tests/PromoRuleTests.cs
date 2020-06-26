using ShoppingCart.Dtos;
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
                // should not group items based on promoid
                Assert.NotEqual(item.Key, promoRules.First().PromotionOfferId);
                // should not apply any promo to cart item
                Assert.False(item.First().PromoApplied);
            }
        }

        [Fact]
        public void Given_An_Expired_Promo_Rule_For_An_Item_With_SKU_Should_Not_Apply_Promo_To_The_Item()
        {
            var promoRules = new List<PromoOffer> {
                new PromoOffer
                {
                    PromotionOfferId = "1",
                    PromotionOfferDescription = "3 A's for 130",
                    ValidTill = DateTime.Today.AddDays(-10),
                    PromoRule = new PromoRule { IsForDifferentItems = false, Quantity = 3, SKUs = new List<string>{ "A" }, PromoResult = new PromoResult { OffFixedPrice = 130 } },
                },
            };

            var cartItems = new List<Dtos.CartItemDto> {
                    new CartItemDto { SKU = "A", Quantity = 3, UnitPrice = 50 },
            };
            var cartItemGroupedByPromo = _promoCalculator.ApplyPromoRule(cartItems, promoRules);
            foreach (var item in cartItemGroupedByPromo)
            {
                // should not group items based on promoid
                Assert.NotEqual(item.Key, promoRules.First().PromotionOfferId);
                // should not apply any promo to cart item
                Assert.False(item.First().PromoApplied);
            }
        }

        [Fact]
        public void Valid_Promo_Should_Be_Applied_To_CartItem()
        {
            var promoRules = new List<PromoOffer> {
                new PromoOffer
                {
                    PromotionOfferId = "1",
                    PromotionOfferDescription = "3 A's for 130",
                    ValidTill = DateTime.Today.AddDays(10),
                    PromoRule = new PromoRule { IsForDifferentItems = false, Quantity = 3, SKUs = new List<string>{ "A" }, PromoResult = new PromoResult { OffFixedPrice = 130 } },
                },
            };

            var cartItems = new List<Dtos.CartItemDto> {
                    new CartItemDto { SKU = "A", Quantity = 3, UnitPrice = 50 },
            };
            var cartItemGroupedByPromo = _promoCalculator.ApplyPromoRule(cartItems, promoRules);
            foreach (var item in cartItemGroupedByPromo)
            {
                // should not group items based on promoid
                Assert.Equal(item.Key, promoRules.First().PromotionOfferId);
                // should not apply any promo to cart item
                Assert.True(item.First().PromoApplied);
            }
        }

        [Fact]
        public void Offer_Price_ShouldBe_Calculated_For_CartItem_With_Matching_PromoRules()
        {
            var promoRules = new List<PromoOffer> {
                new PromoOffer
                {
                    PromotionOfferId = "1",
                    PromotionOfferDescription = "3 A's for 130",
                    ValidTill = DateTime.Today.AddDays(10),
                    PromoRule = new PromoRule { IsForDifferentItems = false, Quantity = 3, SKUs = new List<string>{ "A" }, PromoResult = new PromoResult { OffFixedPrice = 130 } },
                },
            };

            var cartItems = new List<Dtos.CartItemDto> {
                    new CartItemDto { SKU = "A", Quantity = 3, UnitPrice = 50 },
            };
            var cartItemGroupedByPromo = _promoCalculator.ApplyPromoRule(cartItems, promoRules);
            var calculatedCartItems = _promoCalculator.CalculateOfferPrice(cartItemGroupedByPromo, promoRules);

            Assert.False(calculatedCartItems.First().OfferPrice < 0);
        }

        [Fact]
        public void Offer_Price_For_Item_With_Valid_Promo_Should_Be_Less_Than_Actual_Price()
        {
            var promoRules = new List<PromoOffer> {
                new PromoOffer
                {
                    PromotionOfferId = "1",
                    PromotionOfferDescription = "3 A's for 130",
                    ValidTill = DateTime.Today.AddDays(10),
                    PromoRule = new PromoRule { IsForDifferentItems = false, Quantity = 3, SKUs = new List<string>{ "A" }, PromoResult = new PromoResult { OffFixedPrice = 130 } },
                },
            };

            var cartItems = new List<Dtos.CartItemDto> {
                    new CartItemDto { SKU = "A", Quantity = 3, UnitPrice = 50 },
            };
            var cartItemGroupedByPromo = _promoCalculator.ApplyPromoRule(cartItems, promoRules);
            var calculatedCartItems = _promoCalculator.CalculateOfferPrice(cartItemGroupedByPromo, promoRules);

            Assert.True(calculatedCartItems.First().OfferPrice <= cartItems.First().Quantity * cartItems.First().UnitPrice);
        }

        [Fact]
        public void Offer_Price_For_Item_With_Valid_Promo_Should_Not_Be_In_Negative()
        {
            var promoRules = new List<PromoOffer> {
                new PromoOffer
                {
                    PromotionOfferId = "1",
                    PromotionOfferDescription = "3 A's for 130",
                    ValidTill = DateTime.Today.AddDays(10),
                    PromoRule = new PromoRule { IsForDifferentItems = false, Quantity = 3, SKUs = new List<string>{ "A" }, PromoResult = new PromoResult { OffFixedPrice = 130 } },
                },
            };

            var cartItems = new List<Dtos.CartItemDto> {
                    new CartItemDto { SKU = "A", Quantity = 3, UnitPrice = 50 },
            };
            var cartItemGroupedByPromo = _promoCalculator.ApplyPromoRule(cartItems, promoRules);
            var calculatedCartItems = _promoCalculator.CalculateOfferPrice(cartItemGroupedByPromo, promoRules);

            Assert.False(calculatedCartItems.First().OfferPrice < 0);
        }
    }
}
