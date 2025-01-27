using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using posts_back.DTO;
using posts_back.Services.Interfaces;

namespace posts_back.Services;

public class UserService(UserManager<IdentityUser<Guid>> userManager, SignInManager<IdentityUser<Guid>> signInManager)
    : IUserService
{
    public async Task<IdentityResult> RegisterAsync(RegisterDto request)
    {
        var errors = new List<IdentityError>();

        var emailValid = await userManager.FindByEmailAsync(request.Email);
        if (emailValid != null)
            errors.Add(new IdentityError
            {
                Code = "email",
                Description = "The email is already taken."
            });

        var usernameValid = await userManager.FindByNameAsync(request.Username);
        if (usernameValid != null)
            errors.Add(new IdentityError
            {
                Code = "username",
                Description = "The username is already taken."
            });

        var passwordValidator = new PasswordValidator<IdentityUser<Guid>>();
        var passwordValidationResult = await passwordValidator.ValidateAsync(userManager, null!, request.Password);

        if (!passwordValidationResult.Succeeded) errors.Add(passwordValidationResult.Errors.First());

        if (errors.Count != 0) return IdentityResult.Failed(errors.ToArray());

        var user = new IdentityUser<Guid>
        {
            UserName = request.Username,
            Email = request.Email
        };

        var createResult = await userManager.CreateAsync(user, request.Password);

        if (createResult.Succeeded) return IdentityResult.Success;
        errors.AddRange(createResult.Errors);
        return IdentityResult.Failed(errors.ToArray());

    }


    public async Task<(bool success, string message, IdentityUser<Guid>? user)> LoginAsync(UserDto request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return (false, "Invalid email or password", null);

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            return (false, "Invalid username or password", null);
        await signInManager.SignInAsync(user, false);

        return (true, "Login successful", user);
    }

    public async Task LogOutAsync()
    {
        await signInManager.SignOutAsync();
    }


    public async Task<(bool isAuthenticated, string? username, string? email)> CheckAuthAsync(ClaimsPrincipal user)
    {
        var identityUser = await userManager.GetUserAsync(user);
        return identityUser == null ? (false, null, null) : (true, identityUser.UserName, identityUser.Email);
    }
}