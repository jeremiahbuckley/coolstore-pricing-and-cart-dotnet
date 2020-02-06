namespace CartService.Models
{
    public class ShoppingCartItem {
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double PromoSavings { get; set; }
        public Product Product { get; set; }
        
        public ShoppingCartItem() {
            
        }
    }
}
