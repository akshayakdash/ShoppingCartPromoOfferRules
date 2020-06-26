using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Dtos
{
    public class CartDto
    {
        public string CartId { get; set; }
        public List<CartItemDto> CartItems { get; set; }
        public decimal TotalPrice
        {
            get
            {
                return CartItems.Sum(p => p.OfferPrice);
            }
        }
    }

    public class CartItemDto
    {
        [Required]
        public string SKU { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }
        [Required]
        public int Quantity { get; set; }
        public decimal OfferPrice { get; set; }
        public decimal ActualPrice
        {
            get
            {
                return UnitPrice * Quantity;
            }
        }
        public bool PromoApplied { get; set; }
        public string PromoId { get; set; }
    }
}
