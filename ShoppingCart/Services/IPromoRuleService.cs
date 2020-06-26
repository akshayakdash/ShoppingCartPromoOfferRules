using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Services
{
    public interface IPromoRuleService
    {
        /// <summary>
        /// Description: Gets all promotion rules for shopping
        /// </summary>
        /// <returns>List of PromoOffer</returns>
        List<PromoOffer> GetPromoRules();
    }
}
