using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyShop.Core.Contracts;

namespace MyShop.WebUI.Controllers
{
    public class BasketController : Controller
    {
        private IBasketService basketService;

        public BasketController(IBasketService BasketService)
        {
            this.basketService = BasketService;

        }
        // GET: Basket
        public ActionResult Index()
        {
            var model = basketService.GetBasketItems(this.HttpContext);
            return View(model);
        }
    }
}