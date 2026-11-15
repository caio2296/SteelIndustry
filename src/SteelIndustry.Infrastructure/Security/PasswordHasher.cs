using System.Security.Cryptography;
using System.Text;
using SteelIndustry.Application.Interfaces.Security;

namespace SteelIndustry.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));

        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordHash)) return false;

        var computedHash = HashPassword(password);
        return string.Equals(computedHash, passwordHash, StringComparison.Ordinal);
    }
}
