using SteelIndustry.Domain.Entities;

namespace SteelIndustry.Domain.Repositories;

public interface IMaterialRepository
{
    Task<Material?> GetByIdAsync(string id);
    Task<Material?> GetByCodeAsync(string code);
    Task<Material[]> GetAllAsync();
    Task AddAsync(Material material);
    Task UpdateAsync(Material material);
    Task DeleteAsync(string id);
}
