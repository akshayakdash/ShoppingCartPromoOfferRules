using Microsoft.Extensions.DependencyInjection;
using ShoppingCart.Dtos;
using ShoppingCart.Models;
using ShoppingCart.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCart.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IProductService, ProductService>();
            serviceCollection.AddSingleton<ICartService, CartService>();
            serviceCollection.AddSingleton<IPromoRuleService, PromoRuleService>();
            serviceCollection.AddSingleton<ICheckoutService, CheckoutService>();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            Console.WriteLine("---------Products---------");
            Console.WriteLine("SKU \t Price");
            var products = serviceProvider
                .GetService<IProductService>()
                .GetProductsFromStore();
            products?.ForEach(product =>
            {
                Console.WriteLine(product.SKU + "\t" + "Rs. " + product.Price);
            });
            Console.WriteLine();
            Console.WriteLine("---------Active Promotions-------");
            var validPromoOffers = serviceProvider
                .GetService<IPromoRuleService>()
                .GetPromoRules()
                .Where(promo => promo.ValidTill >= DateTime.Now)
                .ToList();
            validPromoOffers?.ForEach(promo =>
            {
                Console.WriteLine(promo.PromotionOfferDescription);
            });

            Console.WriteLine();
            Console.WriteLine("Please add items to your cart.");
            Console.WriteLine("Press c to checkout.");
            Console.WriteLine("Press e to exit.");

            var checkoutService = serviceProvider
                .GetService<ICheckoutService>();

            List<CartItemDto> cartItems = new List<CartItemDto>();
            var incorrectInput = false;
            while (true)
            {
                Console.WriteLine("Add Item:");
                Console.WriteLine("Product \t Unit");
                string line = Console.ReadLine();
                if (line?.Trim().ToLower() == "e".ToLower())
                {
                    break;
                }

                if (line?.Trim().ToLower() == "c".ToLower())
                {
                    // checkout
                    var result = checkoutService.Checkout(new Dtos.CartDto { CartId = Guid.NewGuid().ToString(), CartItems = cartItems });
                    Console.WriteLine("SKU \t OfferPrice");
                    result.CartItems.ForEach(r => {
                        Console.WriteLine(r.SKU + "\t" + r.OfferPrice);
                    });
                    break;
                }
                var splittedInput = line.Split(" ");
                if (splittedInput.Length < 2)
                    incorrectInput = true;
                var productSku = splittedInput[0];
                var quantity = Convert.ToInt32(splittedInput[1]);
                var unitPrice = products
                    .FirstOrDefault(p => p.SKU.ToLower() == productSku.ToLower())
                    .Price;
                CartItemDto item = new CartItemDto { SKU = productSku, Quantity = quantity, UnitPrice = unitPrice };
                cartItems.Add(item);
            }
            Console.ReadKey();
        }
    }
}
