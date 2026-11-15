using System.Diagnostics;

namespace SteelIndustry.API.Middlewares;

public class PerformanceMetricsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMetricsMiddleware> _logger;

    public PerformanceMetricsMiddleware(RequestDelegate _next, ILogger<PerformanceMetricsMiddleware> logger)
    {
        this._next = _next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Captura de memória antes da requisição
        long initialMemory = GC.GetTotalMemory(forceFullCollection: false);
        var stopwatch = Stopwatch.StartNew();

        // 2. Injeta cabeçalhos customizados na resposta antes do envio dos headers
        context.Response.OnStarting(() =>
        {
            stopwatch.Stop();
            long finalMemory = GC.GetTotalMemory(forceFullCollection: false);
            long memoryDelta = Math.Max(0, finalMemory - initialMemory);
            double elapsedMs = Math.Round(stopwatch.Elapsed.TotalMilliseconds, 2);

            context.Response.Headers.Append("X-Response-Time-Ms", elapsedMs.ToString("F2"));
            context.Response.Headers.Append("X-Memory-Allocated-Bytes", memoryDelta.ToString());

            return Task.CompletedTask;
        });

        try
        {
            await _next(context);
        }
        finally
        {
            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();
            }

            long finalMemory = GC.GetTotalMemory(forceFullCollection: false);
            long memoryDelta = Math.Max(0, finalMemory - initialMemory);
            double elapsedMs = Math.Round(stopwatch.Elapsed.TotalMilliseconds, 2);

            _logger.LogInformation(
                "[TELEMETRIA INDUSTRIAL] {Method} {Path} -> Status {StatusCode} | Duração: {ElapsedMs}ms | Delta de Memória: {MemoryDelta} bytes",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                elapsedMs,
                memoryDelta
            );
        }
    }
}
