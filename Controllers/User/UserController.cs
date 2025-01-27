using Microsoft.AspNetCore.Mvc;
using posts_back.DTO;
using posts_back.Services.Interfaces;

namespace posts_back.Controllers.User;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService, ILogger<UserController> log) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        var result = await userService.RegisterAsync(request);
        if (result.Succeeded) return Ok(new { Message = "User registered successfully!" });

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserDto request)
    {
        var (success, message, user) = await userService.LoginAsync(request);
        if (!success) return BadRequest(new { Message = message });

        return Ok(new
        {
            Message = message,
            User = new
            {
                username = user!.UserName,
                user.Email
            }
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await userService.LogOutAsync();
        return Ok(new { Message = "Logged out successfully!" });
    }

    [HttpGet("check-auth")]
    public async Task<IActionResult> CheckAuth()
    {
        log.LogInformation("check-auth called");
        var (isAuthenticated, username, email) = await userService.CheckAuthAsync(HttpContext.User);
        log.LogInformation("{} {} {}", isAuthenticated.ToString(), username, email);
        return Ok(new
        {
            IsAuthenticated = isAuthenticated,
            User = isAuthenticated ? new { Username = username, Email = email } : null
        });
    }
}