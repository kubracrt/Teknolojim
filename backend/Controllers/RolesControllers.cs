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
    public class RolesControllers : ControllerBase
    {
        private readonly eCommerceContext _context;

        public RolesControllers(eCommerceContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            if (roles.Count > 0)
            {
                return Ok(roles);
            }
            else
            {
                return BadRequest("Rol Bulunamadı");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound("Rol Bulunamadı");
            }
            return Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> PostRole([FromBody] Roles role)
        {
            var lastRole = await _context.Roles.OrderByDescending(r => r.ID).FirstOrDefaultAsync();
            if (lastRole != null)
            {
                role.ID = lastRole.ID + 1;
            }
            else
            {
                role.ID = 1;
            }
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return Ok(role);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(int id,[FromBody] Roles roles){
            if(id!=roles.ID){
                return BadRequest("Id'ler uyuşmuyor");
            }
            var role = await _context.Roles.FindAsync(id);
            if(role==null){
                return NotFound("Rol Bulunamadı");
            }
            role.RoleName=roles.RoleName;
            await _context.SaveChangesAsync();
            return Ok(role);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound("Rol Bulunamadı");
            }
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return Ok("Rol Silindi");
        }
    }
}