using Microsoft.Extensions.DependencyInjection;
using SteelIndustry.Application.Interfaces;
using SteelIndustry.Application.Services;

namespace SteelIndustry.Application.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserAppService, UserAppService>();
        services.AddScoped<IAuthAppService, AuthAppService>();
        services.AddScoped<IMaterialAppService, MaterialAppService>();

        return services;
    }
}
