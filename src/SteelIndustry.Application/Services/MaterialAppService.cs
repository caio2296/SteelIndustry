using SteelIndustry.Application.DTOs;
using SteelIndustry.Application.Interfaces;
using SteelIndustry.Domain.Entities;
using SteelIndustry.Domain.Enums;
using SteelIndustry.Domain.Repositories;
using SteelIndustry.Domain.Services;

namespace SteelIndustry.Application.Services;

public class MaterialAppService : IMaterialAppService
{
    private readonly IMaterialRepository _materialRepository;
    private readonly IMaterialJsonCompactorService _compactorService;

    public MaterialAppService(
        IMaterialRepository materialRepository, 
        IMaterialJsonCompactorService compactorService)
    {
        _materialRepository = materialRepository;
        _compactorService = compactorService;
    }

    public async Task<MaterialDto[]> GetAllAsync()
    {
        var materials = await _materialRepository.GetAllAsync();
        var dtos = new MaterialDto[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            var m = materials[i];
            dtos[i] = new MaterialDto(
                m.Id,
                m.Code,
                m.SteelGrade,
                m.Standard,
                m.HeatNumber,
                m.SpecificationsJson,
                m.CreatedAt,
                m.UpdatedAt
            );
        }
        return dtos;
    }

    public async Task<MaterialDto?> GetByIdAsync(string id)
    {
        var m = await _materialRepository.GetByIdAsync(id);
        if (m == null) return null;

        return new MaterialDto(
            m.Id,
            m.Code,
            m.SteelGrade,
            m.Standard,
            m.HeatNumber,
            m.SpecificationsJson,
            m.CreatedAt,
            m.UpdatedAt
        );
    }

    public async Task<MaterialDto?> GetByCodeAsync(string code)
    {
        var m = await _materialRepository.GetByCodeAsync(code);
        if (m == null) return null;

        return new MaterialDto(
            m.Id,
            m.Code,
            m.SteelGrade,
            m.Standard,
            m.HeatNumber,
            m.SpecificationsJson,
            m.CreatedAt,
            m.UpdatedAt
        );
    }

    public async Task<MaterialDto> CreateAsync(CreateMaterialDto dto)
    {
        var material = new Material(
            Guid.NewGuid().ToString(),
            dto.Code,
            dto.SteelGrade,
            dto.Standard,
            dto.HeatNumber,
            dto.SpecificationsJson
        );

        await _materialRepository.AddAsync(material);

        return new MaterialDto(
            material.Id,
            material.Code,
            material.SteelGrade,
            material.Standard,
            material.HeatNumber,
            material.SpecificationsJson,
            material.CreatedAt,
            material.UpdatedAt
        );
    }

    public async Task<MaterialDto?> UpdateAsync(string id, UpdateMaterialDto dto)
    {
        var material = await _materialRepository.GetByIdAsync(id);
        if (material == null) return null;

        material.Update(dto.SteelGrade, dto.Standard, dto.HeatNumber, dto.SpecificationsJson);
        await _materialRepository.UpdateAsync(material);

        return new MaterialDto(
            material.Id,
            material.Code,
            material.SteelGrade,
            material.Standard,
            material.HeatNumber,
            material.SpecificationsJson,
            material.CreatedAt,
            material.UpdatedAt
        );
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var material = await _materialRepository.GetByIdAsync(id);
        if (material == null) return false;

        await _materialRepository.DeleteAsync(id);
        return true;
    }

    public async Task<CompactedMaterialResponseDto> CompactMaterialAsync(CompactMaterialRequestDto dto)
    {
        string rawJson;

        if (!string.IsNullOrWhiteSpace(dto.RawJson))
        {
            rawJson = dto.RawJson;
        }
        else if (!string.IsNullOrWhiteSpace(dto.MaterialId))
        {
            var material = await _materialRepository.GetByIdAsync(dto.MaterialId);
            if (material == null)
            {
                throw new KeyNotFoundException($"Material com ID '{dto.MaterialId}' não encontrado.");
            }
            rawJson = material.SpecificationsJson;
        }
        else
        {
            throw new ArgumentException("É necessário informar 'MaterialId' ou 'RawJson'.");
        }

        var result = await _compactorService.CompactAsync(rawJson, dto.Strategy);

        return new CompactedMaterialResponseDto(
            OriginalSizeBytes: result.OriginalSizeBytes,
            CompactedSizeBytes: result.CompactedSizeBytes,
            ReductionRatioPercent: result.ReductionRatioPercent,
            ElapsedMs: result.ElapsedMs,
            StrategyUsed: result.StrategyUsed.ToString(),
            CompactedData: result.CompactedData
        );
    }

    public async Task<string> DecompactMaterialAsync(string compactedData, CompressionStrategy strategy)
    {
        return await _compactorService.DecompactAsync(compactedData, strategy);
    }
}
