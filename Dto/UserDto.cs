namespace posts_back.DTO;

public class UserDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class RegisterDto : UserDto
{
    public required string Username { get; set; }
}