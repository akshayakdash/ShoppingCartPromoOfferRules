using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using AutoFixture;
using Moq;
using ShoppingCart.Models;
using AutoFixture.Xunit2;

namespace ShoppingCart.Tests
{
    public class CartTests
    {
        public CartTests()
        {

        }

        [Fact]
        public void Creating_Cart_Without_Any_CartItems_Should_Throw_Exception()
        {
            // arrange
            var lineItems = new List<CartItem>();
            var exceptionMessage = "Atleast one item needs to be added to cart.";
            var ex = new Exception("");

            // Act
            Action sut = () => Cart.CreateCart(lineItems);
            var exception = Assert.Throws<Exception>(sut);

            // Assert
            Assert.Throws<Exception>(sut);
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Theory]
        [AutoData]
        public void Creating_Cart_With_CartItems_Should_Create_Cart(List<CartItem> cartItems)
        {
            // Act
            var sut = Cart.CreateCart(cartItems);

            // Assert
            Assert.IsType<Cart>(sut);
            Assert.NotEmpty(sut.CartItems);
        }

        [Theory]
        [AutoData]
        public void Add_Item_To_Cart_Increases_Item_Count_CartItems(CartItem cartItem, List<CartItem> cartItems)
        {
            var sut = Cart.CreateCart(cartItems);

            sut.AddItemToCart(cartItem);

            Assert.NotEmpty(sut.CartItems);
            Assert.Equal(sut.CartItems.Count, cartItems.Count + 1);
        }

        [Theory]
        [AutoData]
        public void Add_Item_To_Cart_With_Negative_Quantity_Throws_Exception(CartItem cartItem, List<CartItem> cartItems)
        {
            var sut = Cart.CreateCart(cartItems);
            var exceptionMessage = "Quantity for cart item can not be less than 1.";

            // Act
            var exception = Assert.Throws<Exception>(() => sut.AddItemToCart(cartItem));

            // Assert
            Assert.Equal(exceptionMessage, exception.Message);
        }
    }
}
