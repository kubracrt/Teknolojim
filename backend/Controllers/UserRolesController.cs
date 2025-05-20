using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
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
        

        [HttpGet]
        public async Task<IActionResult> GetUserRoles()
        {

            var userRoles = await _context.UserRoles
                .Include(userRol => userRol.User)
                .Include(userRol => userRol.Role)
                .Select(userRol => new
                {
                    userRol.UserID,
                    userRol.RoleID,
                    userName = userRol.User.Username,
                    rolName = userRol.Role.RoleName
                })
                .ToListAsync();

            if (userRoles.Count > 0)
            {
                return Ok(userRoles);
            }
            else
            {
                return BadRequest("");
            }



        }


        [HttpGet("{UserID}")]
        public async Task<IActionResult> GetUserRole(int UserID)
        {
            var userRoles = await _context.UserRoles
            .Where(userRol => userRol.UserID == UserID)
                    .Include(userRol => userRol.User)
                    .Include(userRol => userRol.Role)
                    .Select(userRol => new
                    {
                        userRol.ID,
                        userRol.UserID,
                        userRol.RoleID,
                        userName = userRol.User.Username,
                        rolName = userRol.Role.RoleName
                    })
                    .ToListAsync();

            if (userRoles == null)
            {
                return NotFound("Kullanıcı Bulunamadı");
            }
            return Ok(userRoles);
        }

        
        [HttpPut("{UserID}")]
        
        public async Task<IActionResult> PutUserRoles(int UserID,UserRoles updateUserRoles){
         if(UserID!=updateUserRoles.UserID){
              return BadRequest("ID ler uyuşmuyor");
         }

         var userRoles= await _context.UserRoles.FindAsync(UserID);
         if(userRoles==null){
            return BadRequest("Ürün Bulunamadı");
         }

         userRoles.RoleID=updateUserRoles.RoleID;

         await _context.SaveChangesAsync();

         return Ok(new { message = "Rol güncelleme başarılı", userRoles });


        }



    }
}