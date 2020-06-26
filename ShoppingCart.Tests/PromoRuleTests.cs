using Moq;
using ShoppingCart.Dtos;
using ShoppingCart.Models;
using ShoppingCart.Services;
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
        private Mock<IProductService> _mockProductService;
        private Mock<ICartService> _mockCartService;
        private Mock<IPromoRuleService> _mockPromoRuleService;

        public PromoRuleTests()
        {
            _promoCalculator = PromoOfferManager.Instance;
            _mockProductService = new Mock<IProductService>();
            _mockCartService = new Mock<ICartService>();
            _mockPromoRuleService = new Mock<IPromoRuleService>();
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
        public void Expired_Promo_Should_Not_Be_Applied_To_CartItem_For_Matching_SKU_And_Quantity()
        {
            // arrange
            _mockProductService.Setup(method => method.GetProductsFromStore())
               .Returns(new List<ProductDto> {
                    new Dtos.ProductDto{ SKU = "A", Price = 50 },
                    new Dtos.ProductDto{ SKU = "B", Price = 30 },
               });
            var products = _mockProductService.Object.GetProductsFromStore();

            _mockCartService.Setup(method => method.GetCartItems())
               .Returns(new List<Dtos.CartItemDto> {
                    new CartItemDto { SKU = "A", Quantity = 3, UnitPrice = 50 },
               });
            var cartItems = _mockCartService.Object.GetCartItems();
            var promoOffers = new List<PromoOffer> {
                new PromoOffer
                {
                    PromotionOfferId = "1",
                    PromotionOfferDescription = "3 A's for 130",
                    ValidTill = DateTime.Today.AddMonths(-1),
                    PromoRule = new PromoRule { IsForDifferentItems = false, Quantity = 3, SKUs = new List<string>{ "A" }, PromoResult = new PromoResult { OffFixedPrice = 130 } },
                },
            };
            var appliedPromoOffer = promoOffers.First();
            // act
            var cartItemsWithOfferPrice = _promoCalculator.CalculateOfferPrice(_promoCalculator.ApplyPromoRule(cartItems, promoOffers), promoOffers);
            var cartItem_SKU_A = cartItemsWithOfferPrice.FirstOrDefault(p => p.SKU == "A");

            // assert
            Assert.False(cartItem_SKU_A.PromoApplied);
            Assert.Equal(cartItem_SKU_A.ActualPrice, cartItem_SKU_A.UnitPrice * cartItem_SKU_A.Quantity);
            Assert.NotEqual(cartItem_SKU_A.OfferPrice, appliedPromoOffer.PromoRule.PromoResult.OffFixedPrice + ((cartItem_SKU_A.Quantity - appliedPromoOffer.PromoRule.Quantity) * cartItem_SKU_A.UnitPrice));
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
        public void Offer_Price_For_After_Valid_Promo_Applied_To_Ite_Should_Be_Less_Than_Actual_Price()
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
        public void Offer_Price_In_PromoRule_Cannot_Be_Negative()
        {
            var promoRules = new List<PromoOffer> {
                new PromoOffer
                {
                    PromotionOfferId = "1",
                    PromotionOfferDescription = "3 A's for 130",
                    ValidTill = DateTime.Today.AddDays(10),
                    PromoRule = new PromoRule { IsForDifferentItems = false, Quantity = 3, SKUs = new List<string>{ "A" }, PromoResult = new PromoResult { OffFixedPrice = -130 } },
                },
            };

            var cartItems = new List<Dtos.CartItemDto> {
                    new CartItemDto { SKU = "A", Quantity = 3, UnitPrice = 50 },
            };
            Action cartItemGroupedByPromo = () => _promoCalculator.ApplyPromoRule(cartItems, promoRules);
            var exceptionMessage = "One or more invalid promo rules present. Can't apply promo to cart items.";

            var exception = Assert.Throws<Exception>(cartItemGroupedByPromo);
            Assert.Equal(exception.Message, exceptionMessage);
        }

        [Fact]
        public void Promo_Rule_With_Negative_Match_Quantity_Should_Not_Be_Applied_To_CartItem()
        {
            var promoRules = new List<PromoOffer> {
                new PromoOffer
                {
                    PromotionOfferId = "1",
                    PromotionOfferDescription = "3 A's for 130",
                    ValidTill = DateTime.Today.AddDays(10),
                    PromoRule = new PromoRule { IsForDifferentItems = false, Quantity = -3, SKUs = new List<string>{ "A" }, PromoResult = new PromoResult { OffFixedPrice = 130 } },
                },
            };

            var cartItems = new List<Dtos.CartItemDto> {
                    new CartItemDto { SKU = "A", Quantity = 3, UnitPrice = 50 },
            };
            Action cartItemGroupedByPromo = () => _promoCalculator.ApplyPromoRule(cartItems, promoRules);
            var exceptionMessage = "One or more invalid promo rules present. Can't apply promo to cart items.";

            var exception = Assert.Throws<Exception>(cartItemGroupedByPromo);
            Assert.Equal(exception.Message, exceptionMessage);
        }

        [Fact]
        public void Promo_Rule_With_Negative_Match_Quantity_Or_Negative_Offer_Price_Should_Show_Validation_Error_While_Applying_To_CartItems()
        {
            var promoRules = new List<PromoOffer> {
                new PromoOffer
                {
                    PromotionOfferId = "1",
                    PromotionOfferDescription = "3 A's for 130",
                    ValidTill = DateTime.Today.AddDays(10),
                    PromoRule = new PromoRule { IsForDifferentItems = false, Quantity = -3, SKUs = new List<string>{ "A" }, PromoResult = new PromoResult { OffFixedPrice = 130 } },
                },
            };

            var cartItems = new List<Dtos.CartItemDto> {
                    new CartItemDto { SKU = "A", Quantity = 3, UnitPrice = 50 },
            };
            Action cartItemGroupedByPromo = () => _promoCalculator.ApplyPromoRule(cartItems, promoRules);
            var exceptionMessage = "One or more invalid promo rules present. Can't apply promo to cart items.";

            var exception = Assert.Throws<Exception>(cartItemGroupedByPromo);
            Assert.Equal(exception.Message, exceptionMessage);
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

        [Fact]
        public void Price_Of_CarItem_With_SKU_Should_Be_Equal_With_Product_UnitPrice()
        {
            // arrange
            _mockProductService.Setup(method => method.GetProductsFromStore())
               .Returns(new List<ProductDto> {
                    new Dtos.ProductDto{ SKU = "A", Price = 50 },
                    new Dtos.ProductDto{ SKU = "B", Price = 30 },
               });
            var products = _mockProductService.Object.GetProductsFromStore();

            _mockCartService.Setup(method => method.GetCartItems())
               .Returns(new List<Dtos.CartItemDto> {
                    new CartItemDto { SKU = "A", Quantity = 4, UnitPrice = 50 },
               });
            var cartItems = _mockCartService.Object.GetCartItems();

            // act
            var cartItemPriceForSKU_A = cartItems.FirstOrDefault(p => p.SKU == "A").UnitPrice;
            var productPriceSKU_A = products.FirstOrDefault(p => p.SKU == "A").Price;

            // assert
            Assert.Equal(cartItemPriceForSKU_A, productPriceSKU_A);
        }

        [Fact]
        public void Total_Price_Of_CarItem_Calculated_As_UnitPrice_Multiplied_By_Quantity_If_No_Promo_Offer_Available()
        {
            // arrange
            _mockProductService.Setup(method => method.GetProductsFromStore())
               .Returns(new List<ProductDto> {
                    new Dtos.ProductDto{ SKU = "A", Price = 50 },
                    new Dtos.ProductDto{ SKU = "B", Price = 30 },
               });
            var products = _mockProductService.Object.GetProductsFromStore();

            _mockCartService.Setup(method => method.GetCartItems())
               .Returns(new List<Dtos.CartItemDto> {
                    new CartItemDto { SKU = "A", Quantity = 4, UnitPrice = 50 },
               });
            var cartItems = _mockCartService.Object.GetCartItems();
            var promoOffers = new List<PromoOffer>();

            // act
            var cartItemsWithOfferPrice = _promoCalculator.CalculateOfferPrice(_promoCalculator.ApplyPromoRule(cartItems, promoOffers), promoOffers);
            var cartItem_SKU_A = cartItemsWithOfferPrice.FirstOrDefault(p => p.SKU == "A");

            // assert
            Assert.Equal(cartItem_SKU_A.ActualPrice, cartItem_SKU_A.UnitPrice * cartItem_SKU_A.Quantity);
            Assert.False(cartItem_SKU_A.PromoApplied);
        }

        /// <summary>
        /// 3 A's 130
        /// </summary>
        [Fact]
        public void Promo_Should_Be_Applied_To_CartItem_For_Matching_SKU_And_Quantity()
        {
            // arrange
            _mockProductService.Setup(method => method.GetProductsFromStore())
               .Returns(new List<ProductDto> {
                    new Dtos.ProductDto{ SKU = "A", Price = 50 },
                    new Dtos.ProductDto{ SKU = "B", Price = 30 },
               });
            var products = _mockProductService.Object.GetProductsFromStore();

            _mockCartService.Setup(method => method.GetCartItems())
               .Returns(new List<Dtos.CartItemDto> {
                    new CartItemDto { SKU = "A", Quantity = 3, UnitPrice = 50 },
               });
            var cartItems = _mockCartService.Object.GetCartItems();
            var promoOffers = new List<PromoOffer> {
                new PromoOffer
                {
                    PromotionOfferId = "1",
                    PromotionOfferDescription = "3 A's for 130",
                    ValidTill = DateTime.Today.AddMonths(1),
                    PromoRule = new PromoRule { IsForDifferentItems = false, Quantity = 3, SKUs = new List<string>{ "A" }, PromoResult = new PromoResult { OffFixedPrice = 130 } },
                },

            };

            // act
            var cartItemsWithOfferPrice = _promoCalculator.CalculateOfferPrice(_promoCalculator.ApplyPromoRule(cartItems, promoOffers), promoOffers);
            var cartItem_SKU_A = cartItemsWithOfferPrice.FirstOrDefault(p => p.SKU == "A");

            // assert
            Assert.NotEqual(cartItem_SKU_A.OfferPrice, cartItem_SKU_A.UnitPrice * cartItem_SKU_A.Quantity);
            Assert.Equal(cartItem_SKU_A.OfferPrice, promoOffers.First().PromoRule.PromoResult.OffFixedPrice);
            Assert.True(cartItem_SKU_A.PromoApplied);
        }

        /// <summary>
        /// C & D for 30
        /// </summary>
        [Fact]
        public void Promo_Should_Be_Applied_To_CartItem_For_Matching_Different_Combined_SKUs_And_Quantity()
        {
            // arrange
            _mockProductService.Setup(method => method.GetProductsFromStore())
               .Returns(new List<ProductDto> {
                    new Dtos.ProductDto{ SKU = "A", Price = 50 },
                    new Dtos.ProductDto{ SKU = "B", Price = 30 },
                    new Dtos.ProductDto{ SKU = "C", Price = 20 },
                    new Dtos.ProductDto{ SKU = "D", Price = 15 },
               });
            var products = _mockProductService.Object.GetProductsFromStore();

            _mockCartService.Setup(method => method.GetCartItems())
               .Returns(new List<Dtos.CartItemDto> {
                    new CartItemDto { SKU = "A", Quantity = 3, UnitPrice = 50 },
                    new CartItemDto { SKU = "C", Quantity = 1, UnitPrice = 20 },
                    new CartItemDto { SKU = "D", Quantity = 1, UnitPrice = 15 },
               });
            var cartItems = _mockCartService.Object.GetCartItems();
            var promoOffers = new List<PromoOffer> {
                new PromoOffer
                {
                    PromotionOfferId = "3",
                    PromotionOfferDescription = "C + D for 30",
                    ValidTill = DateTime.Today.AddDays(2),
                    PromoRule = new PromoRule{ IsForDifferentItems=true, Quantity = 1, SKUs = new List<string>{ "C", "D" }, PromoResult = new PromoResult{ OffFixedPrice = 30} }
                },
            };
            var appliedPromoOffer = promoOffers.First();
            // act
            var cartItemsWithOfferPrice = _promoCalculator.CalculateOfferPrice(_promoCalculator.ApplyPromoRule(cartItems, promoOffers), promoOffers);
            var cartItem_SKU_C = cartItemsWithOfferPrice.FirstOrDefault(p => p.SKU == "C");
            var cartItem_SKU_D = cartItemsWithOfferPrice.FirstOrDefault(p => p.SKU == "D");

            // assert
            Assert.True(cartItem_SKU_C.PromoApplied);
            Assert.True(cartItem_SKU_D.PromoApplied);
            Assert.NotEqual((cartItem_SKU_C.UnitPrice * cartItem_SKU_C.Quantity), cartItem_SKU_C.OfferPrice);
            Assert.NotEqual((cartItem_SKU_D.UnitPrice * cartItem_SKU_D.Quantity), cartItem_SKU_D.OfferPrice);
            Assert.Equal(cartItem_SKU_C.OfferPrice + cartItem_SKU_D.OfferPrice, appliedPromoOffer.PromoRule.PromoResult.OffFixedPrice);
        }

        /// <summary>
        /// 3 A's 130
        ///        3A + A  + A
        /// 5 A = 130 + 50 + 50
        /// </summary>
        [Fact]
        public void Promo_Should_Be_Applied_To_CartItem_For_Matching_SKU_And_Quantity_And_Promo_Not_Calculated_For_Remaining_Quantity()
        {
            // arrange
            _mockProductService.Setup(method => method.GetProductsFromStore())
               .Returns(new List<ProductDto> {
                    new Dtos.ProductDto{ SKU = "A", Price = 50 },
                    new Dtos.ProductDto{ SKU = "B", Price = 30 },
               });
            var products = _mockProductService.Object.GetProductsFromStore();

            _mockCartService.Setup(method => method.GetCartItems())
               .Returns(new List<Dtos.CartItemDto> {
                    new CartItemDto { SKU = "A", Quantity = 3, UnitPrice = 50 },
               });
            var cartItems = _mockCartService.Object.GetCartItems();
            var promoOffers = new List<PromoOffer> {
                new PromoOffer
                {
                    PromotionOfferId = "1",
                    PromotionOfferDescription = "3 A's for 130",
                    ValidTill = DateTime.Today.AddMonths(1),
                    PromoRule = new PromoRule { IsForDifferentItems = false, Quantity = 3, SKUs = new List<string>{ "A" }, PromoResult = new PromoResult { OffFixedPrice = 130 } },
                },
            };
            var appliedPromoOffer = promoOffers.First();
            // act
            var cartItemsWithOfferPrice = _promoCalculator.CalculateOfferPrice(_promoCalculator.ApplyPromoRule(cartItems, promoOffers), promoOffers);
            var cartItem_SKU_A = cartItemsWithOfferPrice.FirstOrDefault(p => p.SKU == "A");

            // assert
            Assert.NotEqual(cartItem_SKU_A.OfferPrice, cartItem_SKU_A.UnitPrice * cartItem_SKU_A.Quantity);
            Assert.Equal(cartItem_SKU_A.OfferPrice, appliedPromoOffer.PromoRule.PromoResult.OffFixedPrice + ((cartItem_SKU_A.Quantity - appliedPromoOffer.PromoRule.Quantity) * cartItem_SKU_A.UnitPrice));
        }
    }
}
