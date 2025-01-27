using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace posts_back.Data.Entities;

public class PostEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    [Required] public int UserId { get; set; }
    [Required] [MaxLength(255)] public string Title { get; set; } = null!;
    [Required] [MaxLength(10000)] public string Body { get; set; } = null!;
    [Required] [MaxLength(64)] public string Hash { get; set; } = null!;
}