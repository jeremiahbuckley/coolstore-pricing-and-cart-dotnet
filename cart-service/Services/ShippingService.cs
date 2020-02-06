using CartService.Models;

namespace CartService.Services {
    public class ShippingService {

        public void CalculateShipping(ShoppingCart sc) {
            if ( sc != null ) {
                if ( sc.CartItemTotal >= 0 && sc.CartItemTotal < 25) {
                    sc.ShippingTotal = 2.99;
                } else if ( sc.CartItemTotal >= 25 && sc.CartItemTotal < 50) {
                    sc.ShippingTotal = 4.99;
                } else if ( sc.CartItemTotal >= 50 && sc.CartItemTotal < 75) {
                    sc.ShippingTotal = 6.99;
                } else if ( sc.CartItemTotal >= 75 && sc.CartItemTotal < 100) {
                    sc.ShippingTotal = 8.99;				
                } else if ( sc.CartItemTotal >= 100 && sc.CartItemTotal < 10000) {
                    sc.ShippingTotal = 10.99;
                }
            }
        }
    }

}