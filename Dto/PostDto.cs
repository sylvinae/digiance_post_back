using System.ComponentModel.DataAnnotations;

namespace posts_back.DTO;

public class PostDto
{
    [Required] public int Id { get; set; }
    [Required] public int UserId { get; set; }
    [Required] public string Title { get; set; } = null!;
    [Required] public string Body { get; set; } = null!;
}