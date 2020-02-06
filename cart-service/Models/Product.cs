namespace CartService.Models
{
    public class Product {
        public string ItemId { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public double Price { get; set; }

        public Product() {
            
        }
        
        public Product(string itemId, string name, string desc, double price) {
            this.ItemId = itemId;
            this.Name = name;
            this.Desc = desc;
            this.Price = price;
        }
    }
}