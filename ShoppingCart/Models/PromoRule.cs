using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Models
{
    public class PromoOffer
    {
        public string PromotionOfferId { get; set; }
        public string PromotionOfferDescription { get; set; }
        public PromoRule PromoRule { get; set; }
        public DateTime ValidTill { get; set; }
    }

    public class PromoRule
    {
        public List<string> SKUs { get; set; }
        public int Quantity { get; set; }
        public bool IsForDifferentItems { get; set; }
        public PromoResult PromoResult { get; set; }
    }

    public class PromoResult
    {
        public decimal OffFixedPrice { get; set; }
        public decimal OffPercentage { get; set; }
    }
}
