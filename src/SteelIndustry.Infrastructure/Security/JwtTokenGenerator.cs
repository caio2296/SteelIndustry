using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SteelIndustry.Application.Interfaces.Security;
using SteelIndustry.Domain.Entities;

namespace SteelIndustry.Infrastructure.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _secretKey = configuration["Jwt:SecretKey"] ?? "SteelIndustrySuperSecretKeyForIndustrialTelemetria2026!";
        _issuer = configuration["Jwt:Issuer"] ?? "SteelIndustryAPI";
        _audience = configuration["Jwt:Audience"] ?? "SteelIndustryDashboard";
        _expirationMinutes = int.TryParse(configuration["Jwt:ExpirationMinutes"], out var min) ? min : 120;
    }

    public string GenerateToken(User user, out DateTime expiresAt)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secretKey);
        expiresAt = DateTime.UtcNow.AddMinutes(_expirationMinutes);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.GivenName, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            }),
            Expires = expiresAt,
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
