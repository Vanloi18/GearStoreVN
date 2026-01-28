using GearStore.Application.DTOs.User;
using GearStore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GearStore.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(new UserDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Role = roles.FirstOrDefault() ?? "None", // Assuming single role per user
                // JoinedAt not in IdentityUser by default unless CustomUser. Use defaults or omitted?
                // Dto has JoinedAt. I'll pass DateTime.MinValue or handle if CustomUser exists.
                // Assuming standard IdentityUser based on Program.cs: AddIdentity<IdentityUser...
                // So no JoinedAt. I will remove it from DTO or set to dummy.
                JoinedAt = DateTime.MinValue 
            });
        }

        return userDtos;
    }

    public async Task UpdateUserRoleAsync(string userId, string newRole)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new Exception("User not found");

        if (!await _roleManager.RoleExistsAsync(newRole))
            throw new Exception($"Role {newRole} does not exist");

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, newRole);
    }
}
