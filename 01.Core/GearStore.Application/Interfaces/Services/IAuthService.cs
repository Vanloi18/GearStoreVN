using GearStore.Application.DTOs.Auth;

namespace GearStore.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<AuthResponseDto> GetCurrentUserAsync(string userId);
}
