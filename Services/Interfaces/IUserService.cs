using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using posts_back.DTO;

namespace posts_back.Services.Interfaces;

public interface IUserService
{
    Task<IdentityResult> RegisterAsync(RegisterDto request);
    Task<(bool success, string message, IdentityUser<Guid>? user)> LoginAsync(UserDto request);
    Task LogOutAsync();
    Task<(bool isAuthenticated, string? username, string? email)> CheckAuthAsync(ClaimsPrincipal httpContextUser);
}