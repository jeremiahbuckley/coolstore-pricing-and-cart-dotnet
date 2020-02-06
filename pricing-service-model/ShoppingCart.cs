using System;
using System.Collections.Generic;

namespace PricingServiceModel
{
    public class ShoppingCart
    {
        // ? @org.kie.api.definition.type.Position(1)
        public double CartItemPromoSavings { get; set; }

        // ? @org.kie.api.definition.type.Position(0)
        public double CartItemTotal;

        // ? @org.kie.api.definition.type.Position(4)
        public double CartTotal { get; set; }

        // ? @org.kie.api.definition.type.Position(3)
        public double ShippingPromoSavings;

        // ? @org.kie.api.definition.type.Position(2)
        public double ShippingTotal { get; set; }

        // ? @org.kie.api.definition.type.Position(5)
        public IList<ShoppingCartItem> ShoppingCartItemList { get; set; }

        public ShoppingCart()
        {
        }

        public ShoppingCart(
                double cartItemPromoSavings,
                double cartItemTotal,
                double cartTotal,
                double shippingPromoSavings,
                double shippingTotal,
                IList<ShoppingCartItem> shoppingCartItemList)
        {
            this.CartItemPromoSavings = cartItemPromoSavings;
            this.CartItemTotal = cartItemTotal;
            this.CartTotal = cartTotal;
            this.ShippingPromoSavings = shippingPromoSavings;
            this.ShippingTotal = shippingTotal;
            this.ShoppingCartItemList = shoppingCartItemList;
        }
    }
}