using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Models;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartsController : ControllerBase
    {
        private readonly ShopContext _context;

        public ShoppingCartsController(ShopContext context)
        {
            _context = context;
        }

        // GET: api/ShoppingCarts/5
        [HttpGet("{page}")]
        public async Task<ActionResult<object>> GetShoppingCart([FromRoute]int page)
        {
            int pageSize = 3;
            int userId = int.Parse(User.Claims.First(x => x.Type == "id").Value);

            if (_context.ShoppingCarts == null)
            {
                return NotFound();
            }

            var query = (from cart in _context.ShoppingCarts
                         join products in _context.Products on cart.ProductId equals products.Id
                         join users in _context.Users on products.CreatorId equals users.Id
                         where cart.UserId == userId
                         select new
                         {
                             cart.Id,
                             productPic = products.ProductPic,
                             productName = products.ProductName,
                             Seller = users.Username,
                             productPrice = products.Price,
                             quantity = cart.Quantity
                         });

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            if (page < 1 || page > totalPages)
            {
                return BadRequest("Invalid page number.");
            }

            var result = await query.Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            if (result == null || result.Count == 0)
            {
                return NotFound();
            }

            return Ok(new
            {
                totalPages,
                query=result
            });
        }


        // POST: api/ShoppingCarts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostShoppingCart([FromForm] ShoppingCart shoppingCart)
        {
            shoppingCart.UserId = int.Parse(User.Claims.First(x => x.Type == "id").Value);
            if (_context.ShoppingCarts == null)
            {
                return Problem("Entity set 'ShopContext.ShoppingCarts'  is null.");
            }
            var existingItem = _context.ShoppingCarts.Where(x => x.UserId == shoppingCart.UserId 
            && x.ProductId == shoppingCart.ProductId).FirstOrDefault();
            if (existingItem != null)
            {
                existingItem.Quantity += shoppingCart.Quantity;
                _context.Entry(existingItem).State = EntityState.Modified;
            }
            else {
                _context.ShoppingCarts.Add(shoppingCart);
            }
            
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/ShoppingCarts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShoppingCart([FromRoute] int id)
        {
            if (_context.ShoppingCarts == null)
            {
                return NotFound();
            }
            var shoppingCart = await _context.ShoppingCarts.FindAsync(id);
            if (shoppingCart == null)
            {
                return NotFound();
            }

            _context.ShoppingCarts.Remove(shoppingCart);
            await _context.SaveChangesAsync();

            return NoContent();
        }

 
    }
}
