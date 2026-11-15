using SteelIndustry.Domain.Enums;
using SteelIndustry.Infrastructure.Services;
using Xunit;

namespace SteelIndustry.Tests.Unit;

public class MaterialCompactorServiceTests
{
    private readonly MaterialJsonCompactorService _sut = new();

    private const string SampleSteelJson = """
    {
      "material": "Aço Carbono SAE 1020",
      "tipoProduto": "Barra Laminada a Quente",
      "dimensoes": {
        "diametroMm": 50.8,
        "comprimentoMm": 6000,
        "toleranciaDiametro": "ISO h11"
      },
      "composicaoQuimicaPercentual": {
        "carbono": 0.21,
        "manganes": 0.45,
        "silicio": 0.22,
        "fosforo": 0.018,
        "enxofre": 0.021,
        "cromo": 0.04,
        "niquel": 0.02
      },
      "propriedadesMecanicas": {
        "limiteEscoamentoMpa": 245,
        "limiteResistenciaTracaoMpa": 420,
        "alongamentoPercentual": 28.5,
        "durezaBrinell": 131
      },
      "certificacao": {
        "laboratorio": "Laboratorio Metalurgico Central",
        "emissao": "2026-11-01T08:30:00Z",
        "statusInspecao": "Aprovado"
      }
    }
    """;

    [Fact]
    public async Task CompactAsync_MinificationOnly_ShouldReduceSizeWithoutDataLoss()
    {
        // Act
        var result = await _sut.CompactAsync(SampleSteelJson, CompressionStrategy.MinificationOnly);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.OriginalSizeBytes > result.CompactedSizeBytes);
        Assert.True(result.ReductionRatioPercent > 0);
        Assert.DoesNotContain("\n", result.CompactedData);
        Assert.DoesNotContain("  ", result.CompactedData);
    }

    [Fact]
    public async Task CompactAsync_Brotli_ShouldDrasticallyCompressAndAllowDecompact()
    {
        // Act
        var result = await _sut.CompactAsync(SampleSteelJson, CompressionStrategy.Brotli);
        var decompacted = await _sut.DecompactAsync(result.CompactedData, CompressionStrategy.Brotli);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ReductionRatioPercent > 40.0, $"Redução esperada > 40%, obtida: {result.ReductionRatioPercent}%");
        Assert.Contains("SAE 1020", decompacted);
        Assert.Contains("carbono", decompacted);
    }

    [Fact]
    public async Task CompactAsync_GZip_ShouldCompressAndDecompressCorrectly()
    {
        // Act
        var result = await _sut.CompactAsync(SampleSteelJson, CompressionStrategy.GZip);
        var decompacted = await _sut.DecompactAsync(result.CompactedData, CompressionStrategy.GZip);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.CompactedSizeBytes < result.OriginalSizeBytes);
        Assert.Contains("SAE 1020", decompacted);
    }
}
