using SteelIndustry.Domain.Enums;

namespace SteelIndustry.Application.DTOs;

public record UserDto(
    string Id,
    string Username,
    string FullName,
    string Email,
    UserRole Role,
    bool IsActive,
    DateTime CreatedAt
);

public record CreateUserDto(
    string Username,
    string Password,
    string FullName,
    string Email,
    UserRole Role
);

public record UpdateUserDto(
    string FullName,
    string Email,
    UserRole Role,
    bool IsActive
);

public record LoginRequestDto(
    string Username,
    string Password
);

public record LoginResponseDto(
    string Token,
    string Username,
    string FullName,
    string Role,
    DateTime ExpiresAt
);
