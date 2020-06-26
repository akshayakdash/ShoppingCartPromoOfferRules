using ShoppingCart.Dtos;
using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart
{
    public class PromoOfferManager
    {
        static PromoOfferManager instance;
        public static PromoOfferManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new PromoOfferManager();
                return instance;
            }
        }
        public PromoOfferManager()
        {

        }

        public IEnumerable<IGrouping<string, CartItemDto>> ApplyPromoRule(List<CartItemDto> orderItems, List<PromoOffer> offers)
        {
            var promoOffers = offers.Where(promo => promo.ValidTill >= DateTime.Now).ToList();
            foreach (var orderItem in orderItems)
            {
                var prOffer = promoOffers.Where(p => p.PromoRule.SKUs.Contains(orderItem.SKU)).FirstOrDefault();
                if (prOffer != null)
                {
                    if (prOffer.PromoRule.IsForDifferentItems)
                    {
                        var orderContainsAllMatchingItems = prOffer.PromoRule.SKUs.All(t2 => orderItems.Select(p => p.SKU).Contains(t2));
                        // check the quantity is matched or not
                        var matchingDifferentItems = orderItems.Where(p => prOffer.PromoRule.SKUs.Contains(p.SKU));
                        var orderItemQuantityMatched = matchingDifferentItems.All(p => (p.Quantity / prOffer.PromoRule.Quantity) > 0);
                        if (orderContainsAllMatchingItems && orderItemQuantityMatched)
                        {
                            orderItem.PromoId = prOffer.PromotionOfferId;
                            orderItem.PromoApplied = true;
                        }
                    }
                    else if (orderItem.Quantity >= prOffer.PromoRule.Quantity)
                    {
                        orderItem.PromoId = prOffer.PromotionOfferId;
                        orderItem.PromoApplied = true;
                    }
                }
            }
            return orderItems.GroupBy(p => p.PromoId).ToList();
        }
    }
}
