using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;
using Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserControllers : ControllerBase
    {
        private readonly eCommerceContext _context;

        public UserControllers(eCommerceContext context)
        {
            _context = context;
        }

        [HttpGet]

        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            if (users.Count < 0)
            {
                return BadRequest("Kullanıcı Bulunamadı");
            }
            else
            {
                return Ok(users);
            }
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetUser(int id)
        {
            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound("Kullanıcı Bulunamadı");
            }
            else
            {
                return Ok(users);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] User user)
        {

            var lastUser = await _context.Users.OrderByDescending(u => u.Id).FirstOrDefaultAsync();
            if (lastUser != null)
            {
                user.Id = lastUser.Id + 1;
            }
            else
            {
                user.Id = 1;
            }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("Kullanıcı Bulunamadı");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Kullanıcı Silindi" });
        }


        [HttpPut("{id}")]

        public async Task<IActionResult> PutUser(int id, User updatedUser)
        {


            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return BadRequest("Kullanıcı Bulunamadı");
            }

            user.Username = updatedUser.Username;
            user.Password = updatedUser.Password;
            user.isAdmin = updatedUser.isAdmin;

            await _context.SaveChangesAsync();
            return Ok(user);


        }

    }
}