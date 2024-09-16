using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.Models;

namespace server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class RolesController : ControllerBase
  {
    private readonly SoDauBaiContext _context;

    public RolesController(SoDauBaiContext context)
    {
      _context = context;
    }

    // GET: api/Roles`  
    [HttpGet]
    public async Task<IQueryable<RoleDto>> GetRoles()
    {
      var roles = from role in _context.Roles
                  select new RoleDto()
                  {
                    RoleId = role.RoleId,
                    NameRole = role.NameRole,
                    Description = role.Description,
                  };
      return roles;
    }

    // GET: api/Roles/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Role>> GetRole(int id)
    {
      var role = await _context.Roles.FindAsync(id);

      if (role == null)
      {
        return NotFound();
      }

      return role;
    }

    // PUT: api/Roles/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutRole(int id, RoleDto role)
    {
      if (id != role.RoleId)
      {
        return BadRequest();
      }

      _context.Entry(role).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!RoleExists(id))
        {
          return NotFound();
        }
        else
        {
          throw;
        }
      }

      return NoContent();
    }

    // POST: api/Roles
    [HttpPost]
    public async Task<ActionResult<RoleDto>> PostRole(RoleDto role)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      try
      {
        var roleModel = new Role()
        {
          RoleId = role.RoleId,
          NameRole = role.NameRole,
          Description = role.Description,
        };

        _context.Roles.Add(roleModel);
        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        return BadRequest(ex);
      }


      return Ok();
    }

    //// DELETE: api/Roles/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
      var role = await _context.Roles.FindAsync(id);
      if (role == null)
      {
        return NotFound();
      }

      _context.Roles.Remove(role);
      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool RoleExists(int id)
    {
      return _context.Roles.Any(e => e.RoleId == id);
    }
  }
}
