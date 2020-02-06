using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PricingService.Models;
using PricingServiceModel;

namespace PricingService.Controllers
{
    [Route("server")]
    public class PricingController : Controller
    {

        public struct ShippingRule {
            public double Total_greater_than_or_equal;
            public double Total_less_than;
            public double ShippingTotal;
        }

        public static IList<ShippingRule> shippingRules;

        static PricingController() {
            shippingRules = new List<ShippingRule>();
            shippingRules.Add(new ShippingRule{ Total_greater_than_or_equal = 0.0,   Total_less_than = 25.0,      ShippingTotal = 2.99  });
            shippingRules.Add(new ShippingRule{ Total_greater_than_or_equal = 25.0,  Total_less_than = 50.0,      ShippingTotal = 4.99  });
            shippingRules.Add(new ShippingRule{ Total_greater_than_or_equal = 50.0,  Total_less_than = 75.0,      ShippingTotal = 6.99  });
            shippingRules.Add(new ShippingRule{ Total_greater_than_or_equal = 75.0,  Total_less_than = 100.0,     ShippingTotal = 8.99  });
            shippingRules.Add(new ShippingRule{ Total_greater_than_or_equal = 100.0, Total_less_than = 1000000.0, ShippingTotal = 10.99 });
        }

        [HttpPost]
        public IActionResult Index([FromBody] ShoppingCart shoppingCart)
        {
            Action<ShoppingCartItem, IList<PromoEvent>> applyCartItemPromotions = new Action<ShoppingCartItem, IList<PromoEvent>>((ShoppingCartItem sci, IList<PromoEvent> promosList) => {
                foreach(PromoEvent pe in promosList) {
                    if (sci.ItemId == pe.ItemId) {
                        double pctOff = pe.PercentOff;
                        sci.PromoSavings = sci.Price * pctOff;
                        sci.Price *= (1 - pctOff);
                    }
                }
            });

            Action<ShoppingCart> applyShippingRules = new Action<ShoppingCart>(sc => {
                bool assigned = false;
                foreach(ShippingRule sr in shippingRules)
                {
                    if (!assigned && sc.CartItemTotal > sr.Total_greater_than_or_equal && sc.CartItemTotal < sr.Total_less_than) {
                        sc.ShippingTotal = sr.ShippingTotal;
                        assigned = true;
                    }
                }
            });

            Action<ShoppingCart, double> freeShippingPromotion = new Action<ShoppingCart, double>((ShoppingCart sc, double promotionThreshhold) => {
                if (sc.CartItemTotal >= promotionThreshhold) {
                    sc.ShippingPromoSavings = sc.ShippingTotal * -1;
                    sc.ShippingTotal = 0;
                }
            });

            Action<ShoppingCart> freeShippingAfterSeventyFive = new Action<ShoppingCart>(sc => {
                freeShippingPromotion(sc, 75.00);
            });

            Action<ShoppingCart> totalShoppingCartItems = new Action<ShoppingCart>(sc => {
                IList<ShoppingCartItem> sciList = sc.ShoppingCartItemList;
                foreach(ShoppingCartItem sci in sciList) {
                    sc.CartItemTotal += sci.Price * sci.Quantity;
                    sc.CartItemPromoSavings += sc.CartItemPromoSavings + sci.PromoSavings;
                }
            });

            Action<ShoppingCart> totalShoppingCart = new Action<ShoppingCart>(sc => {
                sc.CartTotal = sc.CartItemTotal + sc.ShippingTotal;
                sc.CartItemPromoSavings *= -1;
            });

            // applyCartItemPromotions(shoppingCart);
            applyShippingRules(shoppingCart);
            freeShippingAfterSeventyFive(shoppingCart);
            totalShoppingCartItems(shoppingCart);
            totalShoppingCart(shoppingCart);

            return Json(shoppingCart);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
