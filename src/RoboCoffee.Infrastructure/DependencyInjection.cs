using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoboCoffee.Application.Catalog;
using RoboCoffee.Infrastructure.Catalog;
using RoboCoffee.Application.Orders;
using RoboCoffee.Infrastructure.Orders;
using RoboCoffee.Infrastructure.Persistence;

namespace RoboCoffee.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "The PostgreSQL connection string is not configured.");
        }

        services.AddDbContext<RoboCoffeeDbContext>(options =>
            options.UseNpgsql(connectionString));
        services.AddScoped<IProductCatalogService, ProductCatalogService>();
        services.AddScoped<IOrderService, OrderService>();

        return services;
    }
}
