
using CleanArch.Application.IoC;
using System.Data;
using System.Data.SqlClient;

namespace CleanArch.WebAPI.IoC;

public static class WebApiDiExtension
{
    public static IServiceCollection AddWebApiLayerServices(this IServiceCollection services, string connectionString)
    {

        //services.AddTransient<IDbConnection>(sp => new SqlConnection(connectionString));

        services.AddApplicationLayerServices(connectionString);

        return services;
    }
}