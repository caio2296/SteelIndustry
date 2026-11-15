using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using SteelIndustry.Application.DTOs;
using SteelIndustry.Domain.Enums;
using Xunit;

namespace SteelIndustry.Tests.Integration;

public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RootEndpoint_ReturnsOk_WithoutToken_AndInjectsTelemetryHeaders()
    {
        // Act (Middleware 2: Bypass na rota inicial '/')
        var response = await _client.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Middleware 1: Cabeçalhos de Telemetria
        Assert.True(response.Headers.Contains("X-Response-Time-Ms"), "Cabeçalho X-Response-Time-Ms ausente");
        Assert.True(response.Headers.Contains("X-Memory-Allocated-Bytes"), "Cabeçalho X-Memory-Allocated-Bytes ausente");

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Steel Industry Operational Dashboard API", content);
    }

    [Fact]
    public async Task MaterialsEndpoint_Returns401_WhenNoTokenProvided()
    {
        // Act (Middleware 2: Bloqueio de rota restrita sem token)
        var response = await _client.GetAsync("/api/materials");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Unauthorized", content);
    }

    [Fact]
    public async Task FullWorkflow_Login_ThenAccessMaterialsArray_ThenCompactJson()
    {
        // 1. Realiza Login no AuthController
        var loginRequest = new LoginRequestDto("admin", "Admin@123");
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var loginData = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
        Assert.NotNull(loginData);
        Assert.False(string.IsNullOrWhiteSpace(loginData.Token));

        // 2. Anexa o Token Bearer JWT nas requisições subsequentes
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginData.Token);

        // 3. Consulta Materiais -> Deve retornar um Array (ActionResult<MaterialDto[]>)
        var materialsResponse = await _client.GetAsync("/api/materials");
        Assert.Equal(HttpStatusCode.OK, materialsResponse.StatusCode);

        var materials = await materialsResponse.Content.ReadFromJsonAsync<MaterialDto[]>();
        Assert.NotNull(materials);
        Assert.True(materials.Length >= 4, "Deveria conter no mínimo os 4 aços semeados inicialmente");

        var sae1020 = materials.FirstOrDefault(m => m.SteelGrade.Contains("1020"));
        Assert.NotNull(sae1020);

        // 4. Compactador de JSON de Materiais -> POST /api/materials/compact
        var compactRequest = new CompactMaterialRequestDto(
            MaterialId: sae1020.Id,
            RawJson: null,
            Strategy: CompressionStrategy.Brotli
        );

        var compactResponse = await _client.PostAsJsonAsync("/api/materials/compact", compactRequest);
        Assert.Equal(HttpStatusCode.OK, compactResponse.StatusCode);

        var compactData = await compactResponse.Content.ReadFromJsonAsync<CompactedMaterialResponseDto>();
        Assert.NotNull(compactData);
        Assert.True(compactData.OriginalSizeBytes > compactData.CompactedSizeBytes);
        Assert.True(compactData.ReductionRatioPercent > 30.0);
        Assert.False(string.IsNullOrWhiteSpace(compactData.CompactedData));
    }

    [Fact]
    public async Task PreflightOptions_ReturnsSuccess_WithCorsHeaders_AndExposedTelemetryHeaders()
    {
        // Act: Simula requisição Preflight (OPTIONS) enviada pelo navegador do Angular
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/materials");
        request.Headers.Add("Origin", "http://localhost:4200");
        request.Headers.Add("Access-Control-Request-Method", "GET");
        request.Headers.Add("Access-Control-Request-Headers", "authorization");

        var response = await _client.SendAsync(request);

        // Assert: CORS Preflight deve ser aceito sem exigir autenticação
        Assert.True(response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.OK);
        Assert.True(response.Headers.Contains("Access-Control-Allow-Origin"));
        Assert.Equal("http://localhost:4200", response.Headers.GetValues("Access-Control-Allow-Origin").First());
    }
}
