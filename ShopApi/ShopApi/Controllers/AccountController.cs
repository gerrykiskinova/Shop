using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ShopApi.Data;
using ShopApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly ShopContext _context;


        public AccountController(ShopContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private string GenerateToken(User user)
        {
            string[] roles = { "Admin", "User" };
            List<Claim> claims = new List<Claim>
            {
                new Claim("id",user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.Username),
                new Claim(ClaimTypes.Role,roles[user.Role]),
            };
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha384Signature);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(20),
                signingCredentials: credentials

                );
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
        [AllowAnonymous]
        [HttpPost("login")]

        public IActionResult Login([FromForm] User user)
        {

            var response = Unauthorized();
            var dbUser = _context.Users.Where(x => x.Username == user.Username
                && x.Password == user.Password).FirstOrDefault();
            if (dbUser != null)
            {
                user.Id = dbUser.Id;
                var token = GenerateToken(user);
                return Ok(new { token, dbUser.Role });

            }
            return response;

        }
        [Authorize]
        [HttpGet("isAuth")]
        public IActionResult isAuth()
        {

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("register")]

        public async Task<IActionResult> Register([FromForm] User user)
        {
            user.Role = 1;
            user.RegistrationDate = DateTime.Now;
            if (_context.Users == null)
            {
                return BadRequest("Entity set 'InstagramContext.Users'  is null.");
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            user.Id = _context.Users.OrderBy(item => item.Id).Last().Id;
            var token = GenerateToken(user);
            return Ok(new { token, user.Role });

        }

    }
}
