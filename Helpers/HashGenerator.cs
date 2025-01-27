using System.Security.Cryptography;
using System.Text;
using posts_back.DTO;

namespace posts_back.Helpers;

public class HashGenerator
{
    public static string HashPost(PostDto post)
    {
        var inputString = $"{post.Title}{post.Body}{post.UserId}";
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(inputString));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}