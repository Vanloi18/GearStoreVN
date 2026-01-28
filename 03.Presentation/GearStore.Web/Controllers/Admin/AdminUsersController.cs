using GearStore.Application.Common;
using GearStore.Application.DTOs.Admin;
using GearStore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GearStore.Web.Controllers.Admin;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public class AdminUsersController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminUsersController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<AdminUserDto>>>> GetAll()
    {
        var users = await _adminService.GetAllUsersAsync();
        return Ok(ApiResponse<IEnumerable<AdminUserDto>>.Ok(users));
    }

    [HttpPut("{id}/role")]
    public async Task<IActionResult> UpdateRole(string id, [FromQuery] string role)
    {
        try
        {
            await _adminService.UpdateUserRoleAsync(id, role);
            return Ok(ApiResponse<object>.Ok(null, "User role updated successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }
}
