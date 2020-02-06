using System.Collections.Generic;
using System.Linq;

using CartService.Models;

namespace CartService.Services {

public class PromoService {
        private ISet<Promotion> promotionSet = null;

        public PromoService() {
            promotionSet = new HashSet<Promotion>();
            promotionSet.Add(new Promotion("329299", .25));
        }
                
        public void ApplyCartItemPromotions(ShoppingCart shoppingCart) {
            if ( shoppingCart != null && shoppingCart.ShoppingCartItemList.Count > 0 ) {
                IDictionary<string, Promotion> promoMap = new Dictionary<string, Promotion>(); 
                this.Promotions.ToList().ForEach( promo => promoMap[promo.ItemId] = promo );
                
                shoppingCart.ShoppingCartItemList.ToList().ForEach( sci => {
                    string productId = sci.Product.ItemId;
                    Promotion promo = promoMap[productId];
                    if ( promo != null ) {
                        sci.PromoSavings = (sci.Product.Price * promo.PercentOff * -1);
                        sci.Price = (sci.Product.Price * (1-promo.PercentOff));
                    };
                });
            }
            
        }
        
        public void ApplyShippingPromotions(ShoppingCart shoppingCart) {
            if ( shoppingCart != null ) {
                //PROMO: if cart total is greater than 75, free shipping
                if ( shoppingCart.CartItemTotal >= 75) {
                    shoppingCart.ShippingPromoSavings = shoppingCart.ShippingTotal * -1;
                    shoppingCart.ShippingTotal = 0;
                    
                }
                
            }
            
        }	

        public ISet<Promotion> Promotions {
            get {
                if ( promotionSet == null ) {
                    promotionSet = new HashSet<Promotion>();
                }
                
                return promotionSet;
            }

            set {
                if ( value != null ) {
                    this.promotionSet = new HashSet<Promotion>(value);
                } else {
                    this.promotionSet = new HashSet<Promotion>();
                }

            }
        }
    }
}