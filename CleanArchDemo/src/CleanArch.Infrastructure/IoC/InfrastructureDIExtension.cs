using CleanArch.Infrastructure.Concretes.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Data.SqlClient;

namespace CleanArch.Infrastructure.IoC;

public static class InfrastructureDiExtension
{
    public static IServiceCollection AddInfrastructureLayerServices(this IServiceCollection services, string connectionString)
    {

        services.AddTransient<IDbConnection>(sp => new SqlConnection(connectionString));
        services.AddTransient(typeof(ITradeRepositoryDapper) ,typeof(TradeRepositoryDapper));
        return services;
    }
}