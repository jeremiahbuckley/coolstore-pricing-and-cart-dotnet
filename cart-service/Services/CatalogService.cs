using System;
using System.Collections.Generic;

using CartService.Models;

namespace CartService.Services
{
    public class CatalogService {
        public IList<Product> Products {
            get {
                // @FeignClient(name = "catalogService", url = "${CATALOG_ENDPOINT}")
                // @RequestMapping(method = RequestMethod.GET, value = "/api/products")
                throw new NotImplementedException("CatalogService.Products - should pull fromp catalogService/api/products");
            }
        }

    }
}