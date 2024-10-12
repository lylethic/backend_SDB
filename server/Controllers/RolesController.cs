using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Dtos;
using server.IService;

namespace server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class RolesController : ControllerBase
  {
    private readonly IRole _roleRepo;

    public RolesController(IRole roleRepo)
    {
      _roleRepo = roleRepo;
    }

    // GET: api/Roles`  
    [HttpGet]
    public async Task<IActionResult> GetRoles(int pageNumber = 1, int pageSize = 50)
    {
      try
      {
        var roles = await _roleRepo.GetRoles(pageNumber, pageSize);
        if (roles == null)
        {
          return NotFound(); // 404
        }
        return Ok(roles); // 200
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return StatusCode(500, $"Server error: {ex.Message}"); // 500
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
    [Authorize(Policy = "SuperAdminAndAdmin")]
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
    [Authorize(Policy = "SuperAdminAndAdmin")]
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

    // DELETE: api/Roles/5
    [Authorize(Policy = "SuperAdminAndAdmin")]
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

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpDelete("bulkdelete")]
    public async Task<IActionResult> BulkDelete(List<int> ids)
    {
      var result = await _roleRepo.BulkDelete(ids);

      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }

      return Ok(result);
    }

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPost("upload")]
    public async Task<IActionResult> ImportExcelFile(IFormFile file)
    {
      try
      {
        var result = await _roleRepo.ImportExcel(file);

        if (result.Contains("Successfully"))
        {
          return Ok(result);
        }

        return BadRequest(result);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Server Error: {ex.Message}");
      }
    }
  }
}
