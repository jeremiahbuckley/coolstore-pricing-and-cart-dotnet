using System.Collections.Generic;

namespace CartService.Models
{
    public class ShoppingCart {

        public double CartItemTotal { get; set; }
        public double CartItemPromoSavings { get; set; }
        public double ShippingTotal { get; set; }
        public double ShippingPromoSavings { get; set; }
        public double CartTotal { get; set; }
        public string CartId { get; set; }

        private IList<ShoppingCartItem> shoppingCartItemList;
        public IList<ShoppingCartItem> ShoppingCartItemList { 
            get {
                return shoppingCartItemList;
            }

            set {
                shoppingCartItemList = value;
            }
        }

        public ShoppingCart() {
        }

        public ShoppingCart(string cartId) {
            this.CartId = cartId;
        }

        public void ResetShoppingCartItemList() {
            shoppingCartItemList = new List<ShoppingCartItem>();
        }

        public void AddShoppingCartItem(ShoppingCartItem sci) {
            if (sci != null) {
                shoppingCartItemList.Add(sci);
            }
        }
        
        public bool RemoveShoppingCartItem(ShoppingCartItem sci) {
            return (sci != null)
                ? shoppingCartItemList.Remove(sci)
                : false;
        }
    }
}

