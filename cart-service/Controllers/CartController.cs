using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CartService.Models;

namespace CartService.Controllers
{
    [Route("cart")]
    public class CartController : Controller
    {
        [HttpGet("{cartId}")]
        public IActionResult GetCart(string cartId) {
            return StatusCode(500, "GetCart - Endpoint not implemented.");
        }

        [HttpPost("{cartId}/{itemId}/{quantity}")]
        public IActionResult Add(string cartId, string itemId, int quantity) {
            return StatusCode(500, "Add - Endpoint not implemented.");
        }

        [HttpPost("{cartId}/{tmpId}")]
        public IActionResult Set(string cartId, string tmpId) {
            return StatusCode(500, "Set - Endpoint not implemented.");
        }

        [HttpDelete("{cartId}/{itemId}/{quantity}")]
        public IActionResult Delete(string cartId, string itemId, int quantity) {
            return StatusCode(500, "Delete - Endpoint not implemented.");
        }

        [HttpPost("checkout/{cartId}")]
        public IActionResult Checkout(string cartId) {
            return StatusCode(500, "Checkout - Endpoint not implemented.");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
