using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;
using server.Models;

namespace server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class RolesController : ControllerBase
  {
    private readonly SoDauBaiContext _context;
    private readonly IRole _roleRepo;

    public RolesController(SoDauBaiContext context, IRole roleRepo)
    {
      _context = context;
      _roleRepo = roleRepo;
    }

    // GET: api/Roles`  
    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
      try
      {
        var roles = await _roleRepo.GetRoles();
        if (roles == null)
        {
          return NotFound(); // 404
        }
        return Ok(roles); // 200
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return StatusCode(500, "Server error"); // 500
      }
    }

    // GET: api/Roles/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRole(int id)
    {
      var role = await _roleRepo.GetRole(id);

      if (role.StatusCode != 200)
      {
        return BadRequest(role);
      }

      return Ok(role);
    }

    // PUT: api/Roles/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutRole(int id, RoleDto role)
    {
      var result = await _roleRepo.UpdateRole(id, role);
      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }

      return Ok(result);
    }

    // POST: api/Roles
    [HttpPost]
    public async Task<IActionResult> PostRole(RoleDto role)
    {
      var result = await _roleRepo.AddRole(role);
      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }

      return Ok(result);
    }

    //// DELETE: api/Roles/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
      var result = await _roleRepo.DeleteRole(id);
      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }

      return Ok(result);
    }
  }
}
