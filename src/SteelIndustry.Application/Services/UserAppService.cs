using SteelIndustry.Application.DTOs;
using SteelIndustry.Application.Interfaces;
using SteelIndustry.Application.Interfaces.Security;
using SteelIndustry.Domain.Entities;
using SteelIndustry.Domain.Repositories;

namespace SteelIndustry.Application.Services;

public class UserAppService : IUserAppService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserAppService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserDto[]> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        var dtos = new UserDto[users.Length];
        for (int i = 0; i < users.Length; i++)
        {
            var u = users[i];
            dtos[i] = new UserDto(
                u.Id,
                u.Username,
                u.FullName,
                u.Email,
                u.Role,
                u.IsActive,
                u.CreatedAt
            );
        }
        return dtos;
    }

    public async Task<UserDto?> GetByIdAsync(string id)
    {
        var u = await _userRepository.GetByIdAsync(id);
        if (u == null) return null;

        return new UserDto(
            u.Id,
            u.Username,
            u.FullName,
            u.Email,
            u.Role,
            u.IsActive,
            u.CreatedAt
        );
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        var hash = _passwordHasher.HashPassword(dto.Password);
        var user = new User(Guid.NewGuid().ToString(), dto.Username, hash, dto.FullName, dto.Email, dto.Role);
        await _userRepository.AddAsync(user);

        return new UserDto(
            user.Id,
            user.Username,
            user.FullName,
            user.Email,
            user.Role,
            user.IsActive,
            user.CreatedAt
        );
    }

    public async Task<UserDto?> UpdateAsync(string id, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return null;

        user.UpdateProfile(dto.FullName, dto.Email, dto.Role);
        user.SetActive(dto.IsActive);
        await _userRepository.UpdateAsync(user);

        return new UserDto(
            user.Id,
            user.Username,
            user.FullName,
            user.Email,
            user.Role,
            user.IsActive,
            user.CreatedAt
        );
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return false;

        await _userRepository.DeleteAsync(id);
        return true;
    }
}
