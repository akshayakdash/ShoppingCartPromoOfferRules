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
        /// <summary>
        /// Groups cart item based on promo rule applied
        /// </summary>
        /// <param name="orderItems"></param>
        /// <param name="offers"></param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Calculate offer price for items based on promo rule applied
        /// </summary>
        /// <param name="groupedCartItems"></param>
        /// <param name="offers"></param>
        /// <returns></returns>
        public List<CartItemDto> CalculateOfferPrice(IEnumerable<IGrouping<string, CartItemDto>> groupedCartItems, List<PromoOffer> offers)
        {
            if (groupedCartItems != null && groupedCartItems.Any())
            {
                foreach (var promoGroup in groupedCartItems)
                {
                    var offer = offers.FirstOrDefault(p => p.PromotionOfferId == promoGroup.Key);
                    if (offer != null)
                    {
                        // for same item
                        if (promoGroup.Count() == 1)
                        {
                            var orderItem = promoGroup.First();
                            if (orderItem.Quantity >= offer.PromoRule.Quantity)
                            {
                                var promoFrequency = orderItem.Quantity / offer.PromoRule.Quantity;
                                var promoNotMatchingItemCount = orderItem.Quantity % offer.PromoRule.Quantity;
                                decimal itemPrice = default(decimal);
                                if (offer.PromoRule.PromoResult.OffFixedPrice > 0)
                                {
                                    // offer price for item counts
                                    itemPrice = promoFrequency * offer.PromoRule.PromoResult.OffFixedPrice;
                                }
                                else if (offer.PromoRule.PromoResult.OffPercentage > 0)
                                {
                                    itemPrice = (offer.PromoRule.Quantity * orderItem.UnitPrice) * Convert.ToDecimal(offer.PromoRule.PromoResult.OffPercentage / 100);
                                }
                                // unit price for item not matching counts
                                if (promoNotMatchingItemCount > 0)
                                    itemPrice = itemPrice + Convert.ToDecimal(promoNotMatchingItemCount * orderItem.UnitPrice);

                                orderItem.OfferPrice = itemPrice;
                            }
                        }
                        else if (promoGroup.Count() > 1)
                        {
                            // for different items
                            var items = promoGroup.ToList();
                            var lastItem = items.Last();
                            items.ForEach(item =>
                            {
                                if (item.SKU == lastItem.SKU)
                                {
                                    item.OfferPrice = offer.PromoRule.PromoResult.OffFixedPrice;
                                    // apply price for remainder item
                                    if (item.Quantity > offer.PromoRule.Quantity)
                                    {
                                        var promoNotMatchingItemCount = item.Quantity - offer.PromoRule.Quantity;
                                        if (promoNotMatchingItemCount > 0)
                                            item.OfferPrice += Convert.ToDecimal(promoNotMatchingItemCount * item.UnitPrice);
                                    }
                                }
                                else
                                {
                                    item.OfferPrice = 0;
                                    // apply price for remainder item
                                    if (item.Quantity > offer.PromoRule.Quantity)
                                    {
                                        var promoNotMatchingItemCount = item.Quantity - offer.PromoRule.Quantity;
                                        if (promoNotMatchingItemCount > 0)
                                            item.OfferPrice += Convert.ToDecimal(promoNotMatchingItemCount * item.UnitPrice);
                                    }
                                }
                            });
                        }
                    }
                    else
                    {
                        promoGroup.ToList().ForEach(item =>
                        {
                            item.OfferPrice = item.UnitPrice * item.Quantity;
                        });
                    }
                }
            }
            var groupedItems = groupedCartItems.SelectMany(p => p.ToList()).ToList();

            // check if any cart item offer price is greater than the actual price
            return groupedItems;
        }
    }
}
