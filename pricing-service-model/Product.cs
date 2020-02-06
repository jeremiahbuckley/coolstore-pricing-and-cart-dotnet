using System;

namespace PricingServiceModel
{
    public class Product
    {
        // ? @org.kie.api.definition.type.Position(2)
        public string Desc { get; set; }

        // ? @org.kie.api.definition.type.Position(0)
        public string ItemId { get; set; }

        // ? @org.kie.api.definition.type.Position(1)
        public string Name { get; set; }

        // ? @org.kie.api.definition.type.Position(3)
        public double Price { get; set; }

        public Product()
        {
        }

        public Product(string desc, string itemId,
                string name, double price)
        {
            this.Desc = desc;
            this.ItemId = itemId;
            this.Name = name;
            this.Price = price;
        }
    }
}