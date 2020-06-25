using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using AutoFixture;
using Xunit;

namespace ShoppingCart.Tests
{
    public class ProductTests
    {
        public ProductTests()
        {

        }

        [Fact]
        public void New_Product_With_Empty_SKU_Should_ThrowException()
        {
            // Arrange
            var sku = "";
            decimal price = 20M;
            Exception ex = new Exception("SKU is required for Product.");
            // Act
            Action sut = () => new Product(sku, price);

            // Assert
            Assert.Throws<Exception>(sut);
        }

        [Fact]
        public void New_Product_With_Empty_SKU_Should_ThrowException_With_Required_SKU_Message()
        {
            // Arrange
            var sku = "";
            decimal price = 20M;
            var exceptionMessage = "SKU is required for Product.";

            // Act
            //var exception = Record.Exception(() => new Product(sku, price));
            var exception = Assert.Throws<Exception>(() => new Product(sku, price));

            // Assert
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Fact]
        public void New_Product_With_Invalid_Price_ThrowsException()
        {
            // Arrange
            var sku = "A";
            decimal price = 0;
            Exception ex = new Exception("Invalid price for product.");
            // Act
            Action sut = () => new Product(sku, price);

            // Assert
            Assert.Throws<Exception>(sut);
        }

        [Fact]
        public void New_Product_With_Invalid_Price_ThrowsException_With_Message_Invalid_Price_For_Product()
        {
            // Arrange
            var sku = "A";
            decimal price = 0;
            var exceptionMessage = "Invalid price for product.";

            // Act
            var exception = Assert.Throws<Exception>(() => new Product(sku, price));

            // Assert
            Assert.Equal(exceptionMessage, exception.Message);
        }
    }
}
