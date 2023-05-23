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
    public class OrdersController : ControllerBase
    {
        private readonly ShopContext _context;

        public OrdersController(ShopContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var userId = int.Parse(User.Claims.First(x => x.Type == "id").Value);
            if (_context.Orders == null)
          {
              return NotFound();
          }
            return await _context.Orders.Where(x=>x.UserId== userId).ToListAsync();
        }


        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder()
        {
            Order order = new();
            var userId = int.Parse(User.Claims.First(x => x.Type == "id").Value);
            order.UserId=userId;
            var shopCartItems = from cart in _context.ShoppingCarts
                                join products in _context.Products on cart.ProductId equals products.Id
                                join users in _context.Users on products.CreatorId equals users.Id
                                select new {
                                    cart.Id,
                                    productName =products.ProductName,
                                    productPrice=products.Price,
                                    quantity=cart.Quantity
                                };
            foreach (var item in shopCartItems)
            {
                order.OrderName += item.productName+",";
                order.TotalAmount += item.productPrice*item.quantity;
            }
            order.OrderDate = DateTime.Now;

            if (_context.Orders == null)
            {
                return Problem("Entity set 'ShopContext.Orders'  is null.");
            }
            var shoppingCartsToDelete = _context.ShoppingCarts.Where(cart => cart.UserId == userId);
            _context.ShoppingCarts.RemoveRange(shoppingCartsToDelete);

            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder([FromRoute]int id)
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}
