using GearStore.Application.DTOs.User;

namespace GearStore.Application.Interfaces.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task UpdateUserRoleAsync(string userId, string newRole);
}
