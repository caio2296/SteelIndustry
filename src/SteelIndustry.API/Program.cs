using Microsoft.OpenApi;
using SteelIndustry.API.Middlewares;
using SteelIndustry.Application.Extensions;
using SteelIndustry.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 1. Registro dos Controllers REST
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();

// 2. Configuração do Swagger com suporte a Autenticação Bearer JWT
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Steel Industry Operational Dashboard API",
        Version = "v1",
        Description = "API de alto desempenho (.NET 9 / DDD) para o ecossistema siderúrgico com telemetria fina de memória/tempo, validação JWT customizada e compactador de JSON de materiais."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT gerado no login (com ou sem o prefixo 'Bearer ')."
    });

    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", doc),
            new List<string>()
        }
    });
});

// 3. Configuração de CORS para o Frontend Angular com Exposição de Headers de Telemetria
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClientPolicy", policy =>
    {
        policy.SetIsOriginAllowed(origin => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithExposedHeaders("X-Response-Time-Ms", "X-Memory-Allocated-Bytes");
    });
});

// 4. Injeção de Dependências por Camada (DDD)
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// 5. Pipeline de Middlewares
app.UseRouting();
app.UseCors("AngularClientPolicy");

if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Steel Industry API v1");
        c.RoutePrefix = "swagger";
    });
}

// Middleware 1: Telemetria de Memória e Tempo de Execução
app.UseMiddleware<PerformanceMetricsMiddleware>();

// Middleware 2: Análise de Requisição e Validação JWT Customizada (Bypass na raiz '/')
app.UseMiddleware<JwtRequestAnalyzerMiddleware>();

app.MapControllers();

app.Run();

public partial class Program { }
