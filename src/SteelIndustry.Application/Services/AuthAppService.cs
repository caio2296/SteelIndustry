using SteelIndustry.Application.DTOs;
using SteelIndustry.Application.Interfaces;
using SteelIndustry.Application.Interfaces.Security;
using SteelIndustry.Domain.Repositories;

namespace SteelIndustry.Application.Services;

public class AuthAppService : IAuthAppService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthAppService(
        IUserRepository userRepository, 
        IPasswordHasher passwordHasher, 
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto)
    {
        var user = await _userRepository.GetByUsernameAsync(dto.Username);
        if (user == null || !user.IsActive)
        {
            return null;
        }

        if (!_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
        {
            return null;
        }

        var token = _jwtTokenGenerator.GenerateToken(user, out var expiresAt);

        return new LoginResponseDto(
            Token: token,
            Username: user.Username,
            FullName: user.FullName,
            Role: user.Role.ToString(),
            ExpiresAt: expiresAt
        );
    }
}
