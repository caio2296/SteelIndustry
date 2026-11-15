using SteelIndustry.Domain.Enums;

namespace SteelIndustry.Domain.Services;

public record CompactedMaterialResult(
    int OriginalSizeBytes,
    int CompactedSizeBytes,
    double ReductionRatioPercent,
    double ElapsedMs,
    CompressionStrategy StrategyUsed,
    string CompactedData
);

public interface IMaterialJsonCompactorService
{
    Task<CompactedMaterialResult> CompactAsync(string rawJson, CompressionStrategy strategy);
    Task<string> DecompactAsync(string compactedData, CompressionStrategy strategy);
}
