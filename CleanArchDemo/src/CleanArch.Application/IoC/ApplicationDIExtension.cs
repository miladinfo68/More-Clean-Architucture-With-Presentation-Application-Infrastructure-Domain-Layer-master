using CleanArch.Application.Interfaces.Services;
using CleanArch.Infrastructure.IoC;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Application.IoC;

public static class ApplicationDiExtension
{
    public static IServiceCollection AddApplicationLayerServices(this IServiceCollection services, string connectionString)
    {

        services.AddScoped<ITradeService, TradeService>();

        services.AddInfrastructureLayerServices(connectionString);

        return services;
    }
}