using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ShopApi.Data;
using ShopApi.Models;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _context;
        private static Random random = new Random();
        private readonly IWebHostEnvironment _env;

        public ProductsController(ShopContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/Products
        [HttpGet("{page}")]
        public ActionResult GetProducts([FromRoute]int page)
        {
            int pageSize = 3;
            if (_context.Products == null)
            {
                return NotFound();
            }

            int userId = int.Parse(User.Claims.First(x => x.Type == "id").Value);

            var query = from products in _context.Products
                        join users in _context.Users on products.CreatorId equals users.Id
                        select new
                        {
                            products.Id,
                            productPic = products.ProductPic,
                            productName = products.ProductName,
                            Seller = users.Username,
                            productPrice = products.Price,
                            isCreatedByUser = products.CreatorId == userId
                        };
            int totalCount = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var pagedQuery = query.Skip((page - 1) * pageSize).Take(pageSize);

            return Ok(new { query = pagedQuery, totalPageCount = totalPages });
        }

        [HttpGet("user-products")]
        public ActionResult GetProduct()
        {
            int userId = int.Parse(User.Claims.First(x => x.Type == "id").Value);
            if (_context.Products == null)
            {
                return NotFound();
            }
            var query = from products in _context.Products
                        join users in _context.Users on products.CreatorId equals users.Id
                        where products.CreatorId == userId
                        select new
                        {
                            products.Id,
                            productPic = products.ProductPic,
                            productName = products.ProductName,
                            Seller = users.Username,
                            productPrice = products.Price,
                            isCreatedByUser = products.CreatorId == userId

                        };

            return Ok(new { query });
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutProduct([FromForm] Product product)
        {

            Product oldProduct = _context.Products.Find(product.Id);
            if (oldProduct == null)
            {
                return BadRequest();
            }
            oldProduct.Price=product.Price;
            oldProduct.ProductName = product.ProductName;
            if (product.Image != null)
            {
                var extension = Path.GetExtension(product.Image.FileName);
                var imageUrl = Path.Combine("Images/", RandomString(16) + extension);
                oldProduct.ProductPic = "/" + imageUrl;
                var realPath = Path.Combine(_env.WebRootPath, imageUrl);

                using (var stream = new FileStream(realPath, FileMode.Create))
                {
                    product.Image.CopyTo(stream);
                }
            }
            _context.Entry(oldProduct).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw ex;
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromForm]Product product)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'ShopContext.Products'  is null.");
            }
            product.CreatorId =int.Parse(User.Claims.First(x => x.Type == "id").Value);
            if (product.Image != null)
            {
                var extension = Path.GetExtension(product.Image.FileName);
                var imageUrl = Path.Combine("Images/", RandomString(16) + extension);
                product.ProductPic = "/" + imageUrl;
                var realPath = Path.Combine(_env.WebRootPath, imageUrl);

                using (var stream = new FileStream(realPath, FileMode.Create))
                {
                    product.Image.CopyTo(stream);
                }
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute]int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
