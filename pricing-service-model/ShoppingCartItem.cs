using System;

namespace PricingServiceModel
{
    public class ShoppingCartItem
    {
        // ? @org.kie.api.definition.type.Position(0)
        public string ItemId { get; set; }

        // ? @org.kie.api.definition.type.Position(1)
        public string Name { get; set; }

        // ? @org.kie.api.definition.type.Position(2)
        public double Price { get; set; }

        // ? @org.kie.api.definition.type.Position(4)
        public double PromoSavings { get; set; }

        // ? @org.kie.api.definition.type.Position(3)
        public int Quantity { get; set; }

        // ? @org.kie.api.definition.type.Position(5)
        public ShoppingCart ShoppingCart { get; set; }

        public ShoppingCartItem()
        {
        }

        public ShoppingCartItem(string itemId, string name,
                double price, double promoSavings, int quantity,
                ShoppingCart shoppingCart)
        {
            this.ItemId = itemId;
            this.Name = name;
            this.Price = price;
            this.PromoSavings = promoSavings;
            this.Quantity = quantity;
            this.ShoppingCart = shoppingCart;
        }

        public ShoppingCartItem(string itemId, string name,
                double price, int quantity, double promoSavings,
                ShoppingCart shoppingCart)
        {
            this.ItemId = itemId;
            this.Name = name;
            this.Price = price;
            this.Quantity = quantity;
            this.PromoSavings = promoSavings;
            this.ShoppingCart = shoppingCart;
        }
    }

}