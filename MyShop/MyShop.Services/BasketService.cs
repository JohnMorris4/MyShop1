﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MyShop.Core.Contracts;
using MyShop.Core.Models;

namespace MyShop.Services
{
   public class BasketService
   {
        IRepository<Product> productContext;
        IRepository<Basket> basketContext;

       public const string BasketSessionName = "eCommerceBasket";

       public BasketService(IRepository<Product> ProductContext, IRepository<Basket> BasketContext)
       {
           this.basketContext = BasketContext;
           this.productContext = ProductContext;
       }

       private Basket GetBasket(HttpContextBase httpContext, bool createIfNull)
       {
           HttpCookie cookie = httpContext.Request.Cookies.Get(BasketSessionName);

           Basket basket = new Basket();

           if (cookie != null)
           {
               string basketId = cookie.Value;
               if (!string.IsNullOrEmpty(basketId))
               {
                   basket = basketContext.Find(basketId);
               }
               else
               {
                   if (createIfNull)
                   {
                       basket = CreateNewBasket(httpContext);
                   }
               }

           }
           else
           {
               if (createIfNull)
               {
                   basket = CreateNewBasket(httpContext);
               }
            }

           return basket;
       }

       private Basket CreateNewBasket(HttpContextBase httpContext)
       {
            Basket basket = new Basket();
           basketContext.Insert(basket);
           basketContext.Commit();

           HttpCookie cookie = new HttpCookie(BasketSessionName);
           cookie.Value = basket.Id;
           cookie.Expires = DateTime.Now.AddDays(3);
           httpContext.Response.Cookies.Add(cookie);
           return basket;
       }

       public void AddToBasket(HttpContextBase httpContext, string productId)
       {
           Basket basket = GetBasket(httpContext, true);
           BasketItem item = basket.BasketItems.FirstOrDefault(i => i.ProductId == productId);

           if (item == null)
           {
               item = new BasketItem()
               {
                   BasketId = basket.Id,
                   ProductId = productId,
                   Quanity = 1

               };
               basket.BasketItems.Add(item);
           }
           else
           {
               item.Quanity = item.Quanity + 1;
           }
           basketContext.Commit();
       }

       public void RemoveFromBasket(HttpContextBase httpContext, string itemId)
       {
           Basket basket = GetBasket(httpContext, true);
           BasketItem item = basket.BasketItems.FirstOrDefault(i => i.Id == itemId);

           if (item != null)
           {
               basket.BasketItems.Remove(item);
               basketContext.Commit();
           }
       }
   }
}
