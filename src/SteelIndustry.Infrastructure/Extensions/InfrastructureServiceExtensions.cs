using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SteelIndustry.Application.Interfaces.Security;
using SteelIndustry.Domain.Repositories;
using SteelIndustry.Domain.Services;
using SteelIndustry.Infrastructure.Repositories;
using SteelIndustry.Infrastructure.Security;
using SteelIndustry.Infrastructure.Services;

namespace SteelIndustry.Infrastructure.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Repositórios em memória (sem Entity Framework)
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<IMaterialRepository, InMemoryMaterialRepository>();

        // Serviços de Segurança
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        // Compactador de JSON de Materiais
        services.AddScoped<IMaterialJsonCompactorService, MaterialJsonCompactorService>();

        return services;
    }
}
