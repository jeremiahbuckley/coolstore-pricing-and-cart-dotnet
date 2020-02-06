using CartService.Models;

namespace CartService.Services {
    public interface IShoppingCartService {
        ShoppingCart GetShoppingCart(string cartId);

        Product GetProduct(string itemId);

        ShoppingCart DeleteItem(string cartId, string itemId, int quantity);

        ShoppingCart Checkout(string cartId);

        ShoppingCart AddItem(string cartId, string itemId, int quantity);

        ShoppingCart Set(string cartId, string tmpId);

        void PriceShoppingCart(ShoppingCart sc);
    }

}