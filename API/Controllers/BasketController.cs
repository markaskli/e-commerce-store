using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    
    public class BasketController : BaseApiController
    {
        private readonly StoreContext _context;
        public BasketController(StoreContext context)
        {
            _context = context;
        }

        [HttpGet(Name = "GetBasket")]
        public async Task<ActionResult<BasketDTO>> GetBasket()
        {
            var basket = await RetrieveBasket();

            if (basket == null)
            {
                return NotFound();
            }

            return MapBasketToDTO(basket);
        }


        [HttpPost] // api/basket?productId=3&quanity=2
        public async Task<ActionResult<BasketDTO>> AddItemToBasket(int productId, int quantity)
        {
            var basket = await RetrieveBasket();
            if (basket == null) basket = CreateBasket();

            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            basket.AddItem(product, quantity);
            var result = await _context.SaveChangesAsync() > 0;
            if (result) return CreatedAtRoute("GetBasket", MapBasketToDTO(basket));
            
            return BadRequest(new ProblemDetails{Title = "Problem saving item to basket"});
        }



        [HttpDelete]
        public async Task<ActionResult> RemoveBasketItem(int productId, int quantity) 
        {
            var basket = await RetrieveBasket();
            if (basket == null) return NotFound();
            basket.RemoveItem(productId, quantity);
            var changes = await _context.SaveChangesAsync() > 0;
            if (changes) return Ok();
            return BadRequest(new ProblemDetails{Title = "Problem removing an item from the basket"});
        }


        private async Task<Basket> RetrieveBasket()
        {
            return await _context.Baskets
                    .Include(i => i.Items)
                    .ThenInclude(p => p.Product)
                    .FirstOrDefaultAsync(x => x.BuyerId == Request.Cookies["buyerId"]);
        }

        private Basket CreateBasket()
        {
            var buyerId = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions {IsEssential = true, Expires = DateTime.Now.AddDays(30)};
            Response.Cookies.Append("buyerId", buyerId, cookieOptions);
            var basket = new Basket{BuyerId = buyerId};
            _context.Baskets.Add(basket);
            return basket;
        }


        private BasketDTO MapBasketToDTO(Basket basket)
        {
            return new BasketDTO
            {
                Id = basket.Id,
                BuyerId = basket.BuyerId,
                Items = basket.Items.Select(item => new BasketItemDTO
                {
                    ProductId = item.ProductId,
                    Name = item.Product.Name,
                    Price = item.Product.Price,
                    PictureUrl = item.Product.PictureUrl,
                    Brand = item.Product.Brand,
                    Quantity = item.Quantity
                }).ToList()
            };
        }
    }
}