using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoboCoffee.Application.Catalog;
using RoboCoffee.Infrastructure.Catalog;
using RoboCoffee.Application.Orders;
using RoboCoffee.Application.Integrations;
using RoboCoffee.Infrastructure.Orders;
using RoboCoffee.Infrastructure.Integrations;
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
        services.AddHttpClient<IAddressLookupService, ViaCepAddressLookupService>(client =>
            {
                client.BaseAddress = new Uri("https://viacep.com.br/ws/");
                client.Timeout = TimeSpan.FromSeconds(10);
            })
            .AddPolicyHandler(AddressLookupResiliencePolicies.CreateTimeoutPolicy())
            .AddPolicyHandler(AddressLookupResiliencePolicies.CreateRetryPolicy())
            .AddPolicyHandler(AddressLookupResiliencePolicies.CreateCircuitBreakerPolicy());

        return services;
    }
}
