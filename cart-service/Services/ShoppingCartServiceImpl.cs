using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Logging;

using CartService.Models;
using PsmProduct = PricingServiceModel.Product; 

namespace CartService.Services {
    public class BasicCacheContainer {
        public BasicCacheContainer() {
            throw new Exception("BasiceCacheContainer: Temporary object written to enable the code to build. Replace before run time.");
        }

        public IDictionary<string, ShoppingCart> GetCache(string s) {
            throw new Exception("BasicCacheContainer.GetCache - not implemented.");
        }
    }

    public class RemoteCacheManager: BasicCacheContainer {
        public RemoteCacheManager(object obj = null) {
            throw new Exception("RemoteCacheManager - Temporary object written to enable the code to build. Replace before run time.");
        }
    }

    public class ShoppingCartServiceImpl: IShoppingCartService {

        ILogger<ShoppingCartServiceImpl> log;

        // @Autowired
        protected ShippingService ss;

        // @Autowired
        protected CatalogService catalogServie;

        // @Autowired
        protected PromoService ps;

        protected IDictionary<string, ShoppingCart> carts;

        protected IDictionary<string, Product> productMap = new Dictionary<string, Product>();

        public ShoppingCartServiceImpl(ILogger<ShoppingCartServiceImpl> logger) {
            log = logger;
        }
        public void init() {
            string host = Environment.GetEnvironmentVariable("DATAGRID_HOST");
            string port = Environment.GetEnvironmentVariable("DATAGRID_PORT");

            if (!string.IsNullOrWhiteSpace(host) && !string.IsNullOrWhiteSpace(port)) {
                try {
                    // ConfigurationBuilder builder = new ConfigurationBuilder();
                    // builder.addServer()
                    //         .host(host)
                    //         .port(Integer.ParseInt(port));
                    BasicCacheContainer manager = new RemoteCacheManager(); //builder.build());
                    carts = manager.GetCache("cart");

                    log.LogInformation("Using remote JBoss Data Grid (Hot Rod) on {0}:{1} for cart data", host, port);
                } catch (Exception ex) {
                    log.LogError("Failed to connect to remote JBoss Data Grid (Hot Rod) on {0}:{1}", host, port);
                }
            }

            if (carts == null) {
                log.LogInformation("Using local cache for cart data");
                carts = new Dictionary<string, ShoppingCart>();
            }
        }

        public ShoppingCart GetShoppingCart(string cartId) {
            if (!carts.ContainsKey(cartId)) {
                ShoppingCart cart1 = new ShoppingCart(cartId);
                carts[cartId] =cart1;
                return cart1;
            }

            ShoppingCart cart = carts[cartId];
            PriceShoppingCart(cart);
            carts[cartId] = cart;
            return cart;
        }

        public virtual void PriceShoppingCart(ShoppingCart sc) {
            if (sc != null) {
                InitShoppingCartForPricing(sc);

                if (sc.ShoppingCartItemList != null && sc.ShoppingCartItemList.Count > 0) {
                    ps.ApplyCartItemPromotions(sc);

                    sc.ShoppingCartItemList.ToList().ForEach(sci => {
                        sc.CartItemPromoSavings = sc.CartItemPromoSavings + sci.PromoSavings * sci.Quantity;
                        sc.CartItemTotal = sc.CartItemTotal + sci.Price * sci.Quantity;
                    });

                    ss.CalculateShipping(sc);
                }

                ps.ApplyShippingPromotions(sc);

                sc.CartTotal = sc.CartItemTotal + sc.ShippingTotal;
            }
        }

        protected void InitShoppingCartForPricing(ShoppingCart sc) {
            sc.CartItemTotal = 0;
            sc.CartItemPromoSavings = 0;
            sc.ShippingTotal = 0;
            sc.ShippingPromoSavings = 0;
            sc.CartTotal = 0;

            foreach(var sci in sc.ShoppingCartItemList) {
                Product p = GetProduct(sci.Product.ItemId);

                //if product exist, create new product to reset price
                if (p != null) {
                    sci.Product = new Product(p.ItemId, p.Name, p.Desc, p.Price);
                    sci.Price = p.Price;
                }

                sci.PromoSavings = 0;
            }
        }

        public Product GetProduct(string itemId) {
            if (!productMap.ContainsKey(itemId)) {
                // Fetch and cache products. TODO: Cache should expire at some point!
                IList<Product> products = catalogServie.Products;
                foreach(Product p in products) {
                    productMap[p.ItemId] = p;
                }
            }

            return productMap[itemId];
        }

        public ShoppingCart DeleteItem(string cartId, string itemId, int quantity) {
            IList<ShoppingCartItem> toRemoveList = new List<ShoppingCartItem>();

            ShoppingCart cart = GetShoppingCart(cartId);

            foreach(ShoppingCartItem sci in cart.ShoppingCartItemList) {
                if (sci.Product.ItemId == itemId) {
                    if (quantity >= sci.Quantity) {
                        toRemoveList.Add(sci);
                    } else {
                        sci.Quantity = sci.Quantity - quantity;
                    }
                }
            }

            foreach(var sci in toRemoveList) {
                cart.RemoveShoppingCartItem(sci);
            }
            PriceShoppingCart(cart);
            carts[cartId] = cart;

            return cart;
        }

        public ShoppingCart Checkout(string cartId) {
            ShoppingCart cart = GetShoppingCart(cartId);
            cart.ResetShoppingCartItemList();
            PriceShoppingCart(cart);
            carts[cartId] = cart;
            return cart;
        }

        public ShoppingCart AddItem(string cartId, string itemId, int quantity) {
            ShoppingCart cart = GetShoppingCart(cartId);
            Product product = GetProduct(itemId);

            if (product == null) {
                log.LogWarning("Invalid product {0} request to get added to the shopping cart. No product added", itemId);
                return cart;
            }

            ShoppingCartItem sci = new ShoppingCartItem();
            sci.Product = product;
            sci.Quantity = quantity;
            sci.Price = product.Price;
            cart.AddShoppingCartItem(sci);

            try {
                PriceShoppingCart(cart);
                cart.ShoppingCartItemList = DedupeCartItems(cart);
            } catch (Exception ex) {
                cart.RemoveShoppingCartItem(sci);
                throw ex;
            }

            carts[cartId] = cart;
            return cart;
        }

        public ShoppingCart Set(string cartId, string tmpId) {

            ShoppingCart cart = GetShoppingCart(cartId);
            ShoppingCart tmpCart = GetShoppingCart(tmpId);

            if (tmpCart != null) {
                cart.ResetShoppingCartItemList();
                cart.ShoppingCartItemList = tmpCart.ShoppingCartItemList;
            }

            try {
                PriceShoppingCart(cart);
                cart.ShoppingCartItemList = DedupeCartItems(cart);
            } catch (Exception ex) {
                throw ex;
            }

            carts[cartId] = cart;
            return cart;
        }

        IList<ShoppingCartItem> DedupeCartItems(ShoppingCart sc) {
            IList<ShoppingCartItem> result = new List<ShoppingCartItem>();
            IDictionary<string, int> quantityMap = new Dictionary<string, int>();

            foreach(var sci in sc.ShoppingCartItemList) {
                if (quantityMap.ContainsKey(sci.Product.ItemId)) {
                    quantityMap[sci.Product.ItemId] = quantityMap[sci.Product.ItemId] + sci.Quantity;
                } else {
                    quantityMap[sci.Product.ItemId] = sci.Quantity;
                }
            }

            foreach(var itemId in quantityMap.Keys) {
                Product p = GetProduct(itemId);
                ShoppingCartItem newItem = new ShoppingCartItem();
                newItem.Quantity = quantityMap[itemId];
                newItem.Price = p.Price;
                newItem.Product = p;
                result.Add(newItem);
            }

            return result;
        }
    }
}