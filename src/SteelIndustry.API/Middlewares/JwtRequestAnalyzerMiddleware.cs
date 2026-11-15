using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace SteelIndustry.API.Middlewares;

public class JwtRequestAnalyzerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtRequestAnalyzerMiddleware> _logger;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtRequestAnalyzerMiddleware(
        RequestDelegate next, 
        ILogger<JwtRequestAnalyzerMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _secretKey = configuration["Jwt:SecretKey"] ?? "SteelIndustrySuperSecretKeyForIndustrialTelemetria2026!";
        _issuer = configuration["Jwt:Issuer"] ?? "SteelIndustryAPI";
        _audience = configuration["Jwt:Audience"] ?? "SteelIndustryDashboard";
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Bypass explícito para requisições Preflight (OPTIONS) do navegador (CORS)
        if (HttpMethods.IsOptions(context.Request.Method))
        {
            await _next(context);
            return;
        }

        var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;

        // 2. Bypass explícito para rota raiz (/), login, swagger ou qualquer endpoint com [AllowAnonymous]
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Authorization.IAllowAnonymous>() != null || IsPublicRoute(path))
        {
            await _next(context);
            return;
        }

        // 2. Extração do cabeçalho Authorization
        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader) ||
            string.IsNullOrWhiteSpace(authHeader))
        {
            await RespondUnauthorizedAsync(context, "Cabeçalho 'Authorization' ausente. Forneça um token no formato 'Bearer <token>'.");
            return;
        }

        string authHeaderValue = authHeader.ToString().Trim();

        // Normalização flexível do Token:
        // Trata "Bearer <token>", "Bearer Bearer <token>" (duplicação comum no Swagger UI) e token direto
        string token = authHeaderValue;
        while (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            token = token.Substring("Bearer ".Length).Trim();
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            await RespondUnauthorizedAsync(context, "Token JWT ausente ou inválido no cabeçalho 'Authorization'.");
            return;
        }

        // 3. Validação manual e extração de Claims via JwtSecurityTokenHandler
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secretKey);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            // Vincula o usuário autenticado ao contexto da requisição
            context.User = principal;

            var username = principal.Identity?.Name ?? "desconhecido";
            _logger.LogDebug("[JWT ANALYZER] Requisição autenticada com sucesso para o usuário: {Username} na rota {Path}", username, path);

            await _next(context);
        }
        catch (SecurityTokenExpiredException)
        {
            await RespondUnauthorizedAsync(context, "O token de autenticação JWT expirou. Realize login novamente.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[JWT ANALYZER] Falha na validação do token JWT na rota {Path}", path);
            await RespondUnauthorizedAsync(context, "Token JWT inválido ou corrompido.");
        }
    }

    private static bool IsPublicRoute(string path)
    {
        return path == "/" 
            || path == "/index.html"
            || path == string.Empty
            || path.StartsWith("/api/auth/login")
            || path.StartsWith("/swagger");
    }

    private static async Task RespondUnauthorizedAsync(HttpContext context, string message)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        var payload = new
        {
            status = 401,
            error = "Unauthorized",
            message,
            path = context.Request.Path.Value,
            timestamp = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
        await context.Response.WriteAsync(json);
    }
}
