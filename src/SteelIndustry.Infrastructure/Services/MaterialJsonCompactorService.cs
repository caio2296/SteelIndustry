using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using SteelIndustry.Domain.Enums;
using SteelIndustry.Domain.Services;

namespace SteelIndustry.Infrastructure.Services;

public class MaterialJsonCompactorService : IMaterialJsonCompactorService
{
    public Task<CompactedMaterialResult> CompactAsync(string rawJson, CompressionStrategy strategy)
    {
        if (string.IsNullOrWhiteSpace(rawJson))
        {
            throw new ArgumentException("JSON do material não pode ser vazio.", nameof(rawJson));
        }

        var sw = Stopwatch.StartNew();
        var originalBytes = Encoding.UTF8.GetBytes(rawJson);
        var originalSizeBytes = originalBytes.Length;

        // 1. Minificação Estrutural (remove espaços, identações e normaliza)
        string minifiedJson = MinifyJson(rawJson);
        var minifiedBytes = Encoding.UTF8.GetBytes(minifiedJson);

        string compactedData;
        int compactedSizeBytes;

        switch (strategy)
        {
            case CompressionStrategy.GZip:
                var gzipBytes = CompressGZip(minifiedBytes);
                compactedSizeBytes = gzipBytes.Length;
                compactedData = Convert.ToBase64String(gzipBytes);
                break;

            case CompressionStrategy.Brotli:
                var brotliBytes = CompressBrotli(minifiedBytes);
                compactedSizeBytes = brotliBytes.Length;
                compactedData = Convert.ToBase64String(brotliBytes);
                break;

            case CompressionStrategy.MinificationOnly:
            default:
                compactedSizeBytes = minifiedBytes.Length;
                compactedData = minifiedJson;
                break;
        }

        sw.Stop();

        double reductionRatio = originalSizeBytes > 0
            ? Math.Max(0.0, Math.Round(((originalSizeBytes - compactedSizeBytes) / (double)originalSizeBytes) * 100.0, 2))
            : 0.0;

        var result = new CompactedMaterialResult(
            OriginalSizeBytes: originalSizeBytes,
            CompactedSizeBytes: compactedSizeBytes,
            ReductionRatioPercent: reductionRatio,
            ElapsedMs: Math.Round(sw.Elapsed.TotalMilliseconds, 3),
            StrategyUsed: strategy,
            CompactedData: compactedData
        );

        return Task.FromResult(result);
    }

    public async Task<string> DecompactAsync(string compactedData, CompressionStrategy strategy)
    {
        if (string.IsNullOrWhiteSpace(compactedData))
        {
            return string.Empty;
        }

        switch (strategy)
        {
            case CompressionStrategy.GZip:
                var gzipBytes = Convert.FromBase64String(compactedData);
                return await DecompressGZipAsync(gzipBytes);

            case CompressionStrategy.Brotli:
                var brotliBytes = Convert.FromBase64String(compactedData);
                return await DecompressBrotliAsync(brotliBytes);

            case CompressionStrategy.MinificationOnly:
            default:
                return compactedData;
        }
    }

    private static string MinifyJson(string json)
    {
        using var doc = JsonDocument.Parse(json);
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false }))
        {
            doc.WriteTo(writer);
        }
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    private static byte[] CompressGZip(byte[] input)
    {
        using var ms = new MemoryStream();
        using (var gzip = new GZipStream(ms, CompressionLevel.Optimal, leaveOpen: true))
        {
            gzip.Write(input, 0, input.Length);
        }
        return ms.ToArray();
    }

    private static async Task<string> DecompressGZipAsync(byte[] compressed)
    {
        using var ms = new MemoryStream(compressed);
        using var gzip = new GZipStream(ms, CompressionMode.Decompress);
        using var reader = new StreamReader(gzip, Encoding.UTF8);
        return await reader.ReadToEndAsync();
    }

    private static byte[] CompressBrotli(byte[] input)
    {
        using var ms = new MemoryStream();
        using (var brotli = new BrotliStream(ms, CompressionLevel.Optimal, leaveOpen: true))
        {
            brotli.Write(input, 0, input.Length);
        }
        return ms.ToArray();
    }

    private static async Task<string> DecompressBrotliAsync(byte[] compressed)
    {
        using var ms = new MemoryStream(compressed);
        using var brotli = new BrotliStream(ms, CompressionMode.Decompress);
        using var reader = new StreamReader(brotli, Encoding.UTF8);
        return await reader.ReadToEndAsync();
    }
}
