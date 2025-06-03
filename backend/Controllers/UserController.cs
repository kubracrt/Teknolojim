using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly eCommerceContext _context;
        private readonly IConfiguration _configuration;

        private readonly ILogger<UserController> _logger;

        public UserController(eCommerceContext context, IConfiguration configuration, ILogger<UserController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.
            ToListAsync();
            if (users.Count > 0)
            {
                return Ok(users);
            }
            else
            {
                return NotFound("Kullanıcı Bulunamadı");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("Kullanıcı Bulunamadı");
            }
            return Ok(user);
        }



        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] User user, int roleID)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                return BadRequest("Bu e-posta adresi zaten kullanılıyor.");
            }

            var newUser = new User
            {
                Username = user.Username,
                Email = user.Email,
                Password = HashPassword(user.Password),
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var userRoles = new UserRoles
            {
                UserID = newUser.ID,
                RoleID = roleID
            };

            _context.UserRoles.Add(userRoles);
            await _context.SaveChangesAsync();

            return Ok(newUser);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] User loginUser)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginUser.Email);
            if (user == null)
            {
                _logger.LogWarning("Kullanıcı bulunamadı: {Email}", loginUser.Email);
                return BadRequest("Geçersiz email veya password");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(3),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            _logger.LogInformation("Kullanıcı giriş yaptı: {Username}", user.Username);

            return Ok(new
            {
                Token = tokenString,
                Id = user.ID,
                Username = user.Username,
                Email = user.Email
            });
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            string hashedInput = HashPassword(inputPassword);
            return hashedInput == hashedPassword;
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User Bulunamadı");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Ürün Silindi", user });
        }

        [HttpPut("{id}")]
        public async Task <IActionResult> PutUser(int id, User updateUser){
            if(id !=updateUser.ID){
                return BadRequest("ID ler uyuşmuyor");
            }

            var user=await _context.Users.FindAsync(id);
            if(user==null){
                return BadRequest("Ürün Bulunamadı");
            }

            user.Username=updateUser.Username;
            user.Email=updateUser.Email;
            user.Password=updateUser.Password;

            await _context.SaveChangesAsync();

            return Ok(new {message="User Güncelleme başarılı",user});
        }
    }
}