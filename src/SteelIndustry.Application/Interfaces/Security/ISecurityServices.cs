using SteelIndustry.Domain.Entities;

namespace SteelIndustry.Application.Interfaces.Security;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}

public interface IJwtTokenGenerator
{
    string GenerateToken(User user, out DateTime expiresAt);
}
