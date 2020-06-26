using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.Models;

namespace ShoppingCart.Services
{
    public class PromoRuleService : IPromoRuleService
    {
        public List<PromoOffer> GetPromoRules()
        {
            var promoRuleDirectory = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\{"MasterData\\promorules.json"}");
            var promoRuleJson = System.IO.File.ReadAllText(promoRuleDirectory);
            var promoOffers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PromoOffer>>(promoRuleJson);
            return promoOffers;
        }
    }
}
