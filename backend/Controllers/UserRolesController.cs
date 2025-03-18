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
    public class UserRolesController : ControllerBase
    {

        private readonly eCommerceContext _context;

        public UserRolesController(eCommerceContext context)
        {
            _context = context;
        }
        
       [HttpPost]
       public async Task<IActionResult> AddUserRole([FromBody] UserRoles userRoles)
       {
           var lastUserRole = await _context.UserRoles.OrderByDescending(u => u.ID).FirstOrDefaultAsync();
           if (lastUserRole != null)
           {
               userRoles.ID = lastUserRole.ID + 1;
           }
           else
           {
               userRoles.ID = 1;
           }
           _context.UserRoles.Add(userRoles);
           await _context.SaveChangesAsync();
           return Ok(userRoles);
       }


    }
}