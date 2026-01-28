using GearStore.Application.DTOs.Admin;

namespace GearStore.Application.Interfaces.Services;

public interface IAdminService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
    Task<IEnumerable<AdminUserDto>> GetAllUsersAsync();
    Task UpdateUserRoleAsync(string userId, string newRole);

}
