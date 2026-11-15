using SteelIndustry.Application.DTOs;

namespace SteelIndustry.Application.Interfaces;

public interface IUserAppService
{
    Task<UserDto[]> GetAllAsync();
    Task<UserDto?> GetByIdAsync(string id);
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task<UserDto?> UpdateAsync(string id, UpdateUserDto dto);
    Task<bool> DeleteAsync(string id);
}

public interface IAuthAppService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto);
}

public interface IMaterialAppService
{
    Task<MaterialDto[]> GetAllAsync();
    Task<MaterialDto?> GetByIdAsync(string id);
    Task<MaterialDto?> GetByCodeAsync(string code);
    Task<MaterialDto> CreateAsync(CreateMaterialDto dto);
    Task<MaterialDto?> UpdateAsync(string id, UpdateMaterialDto dto);
    Task<bool> DeleteAsync(string id);
    Task<CompactedMaterialResponseDto> CompactMaterialAsync(CompactMaterialRequestDto dto);
    Task<string> DecompactMaterialAsync(string compactedData, SteelIndustry.Domain.Enums.CompressionStrategy strategy);
}
