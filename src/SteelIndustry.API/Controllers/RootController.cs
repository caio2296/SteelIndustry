using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SteelIndustry.API.Controllers;

[ApiController]
[Route("")]
public class RootController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetRoot()
    {
        var response = new
        {
            api = "Steel Industry Operational Dashboard API",
            version = "1.0.0",
            status = "Operational",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            framework = ".NET 9.0 (C# 13)",
            architecture = "Domain-Driven Design (DDD)",
            telemetry = new
            {
                serverTimeUtc = DateTime.UtcNow,
                totalAllocatedMemoryBytes = GC.GetTotalMemory(forceFullCollection: false),
                activeProcessors = Environment.ProcessorCount
            },
            modules = new
            {
                materials = "/api/materials (Listagem em Array, CRUD e Compactador JSON)",
                compactor = "/api/materials/compact (POST - Minificação, Brotli, GZip)",
                users = "/api/users (Listagem em Array e Gestão de Usuários Industriais)",
                auth = "/api/auth/login (POST - Emissão de Token Bearer JWT)",
                ping = "/api/ping (GET - Teste de Conectividade)"
            },
            docs = "/swagger"
        };

        return Ok(response);
    }

    [HttpGet("api/ping")]
    [AllowAnonymous]
    public IActionResult Ping()
    {
        return Ok(new
        {
            status = "Online",
            message = "Conexão Frontend-Backend estabelecida com sucesso!",
            serverTime = DateTime.UtcNow,
            memoryAllocatedBytes = GC.GetTotalMemory(false)
        });
    }
}
