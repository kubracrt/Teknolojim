
using backend.Models;
using Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {

        private readonly eCommerceContext _context;

        public UserController(eCommerceContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var user = await _context.Users.ToListAsync();
            if (user.Count > 0)
            {
                return Ok(user);
            }
            else
            {
                return BadRequest("Kullanıcı Bulunamadı");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] User user,int roleID)
        {
        
            var newUser = new User
            {
                Username = user.Username,
                Email = user.Email,
                Password = user.Password,
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
    }

}