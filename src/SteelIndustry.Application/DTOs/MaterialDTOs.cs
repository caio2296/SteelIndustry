using SteelIndustry.Domain.Enums;

namespace SteelIndustry.Application.DTOs;

public record MaterialDto(
    string Id,
    string Code,
    string SteelGrade,
    string Standard,
    string HeatNumber,
    string SpecificationsJson,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateMaterialDto(
    string Code,
    string SteelGrade,
    string Standard,
    string HeatNumber,
    string SpecificationsJson
);

public record UpdateMaterialDto(
    string SteelGrade,
    string Standard,
    string HeatNumber,
    string SpecificationsJson
);

public record CompactMaterialRequestDto(
    string? MaterialId,
    string? RawJson,
    CompressionStrategy Strategy = CompressionStrategy.MinificationOnly
);

public record CompactedMaterialResponseDto(
    int OriginalSizeBytes,
    int CompactedSizeBytes,
    double ReductionRatioPercent,
    double ElapsedMs,
    string StrategyUsed,
    string CompactedData
);
