using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RoboCoffee.Contracts.Catalog;
using RoboCoffee.Contracts.Orders;
using RoboCoffee.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

namespace RoboCoffee.IntegrationTests;

public sealed class PostgreSqlPersistenceTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres =
        new PostgreSqlBuilder("postgres:16")
            .WithDatabase("robocoffee_tests")
            .WithUsername("robocoffee")
            .WithPassword("robocoffee")
            .Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Development");
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<RoboCoffeeDbContext>();
                    services.RemoveAll<DbContextOptions<RoboCoffeeDbContext>>();
                    services.RemoveAll<IDbContextOptionsConfiguration<RoboCoffeeDbContext>>();

                    services.AddDbContext<RoboCoffeeDbContext>(options =>
                        options.UseNpgsql(_postgres.GetConnectionString()));
                });
            });

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RoboCoffeeDbContext>();
        await context.Database.MigrateAsync();

        _client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task MigrationsAndApiCreateAndPersistProductAndOrder()
    {
        var productResponse = await _client.PostAsJsonAsync("/api/products",
            new CreateProductRequest(
                "PostgreSQL Espresso",
                "Coffee",
                "P",
                12m,
                true,
                null));

        Assert.Equal(HttpStatusCode.Created, productResponse.StatusCode);
        var product = await productResponse.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.NotNull(product);

        var orderResponse = await _client.PostAsJsonAsync("/api/orders",
            new CreateOrderRequest(
                "PostgreSQL Customer",
                null,
                "01001000",
                [new CreateOrderItemRequest(product.Id, 2, "Persisted order")]));

        Assert.Equal(HttpStatusCode.Created, orderResponse.StatusCode);
        var order = await orderResponse.Content.ReadFromJsonAsync<OrderResponse>();
        Assert.NotNull(order);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RoboCoffeeDbContext>();

        var persistedProduct = await context.Products
            .SingleAsync(candidate => candidate.Id == product.Id);
        var persistedOrder = await context.Orders
            .Include(candidate => candidate.Items)
            .SingleAsync(candidate => candidate.Id == order.Id);

        Assert.Equal("PostgreSQL Espresso", persistedProduct.Name);
        Assert.Equal(24m, persistedOrder.Total);
        var persistedItem = Assert.Single(persistedOrder.Items);
        Assert.Equal("PostgreSQL Espresso", persistedItem.ProductNameSnapshot);
        Assert.Equal(12m, persistedItem.UnitPriceSnapshot);
        Assert.Equal(24m, persistedItem.Subtotal);
    }

    [Fact]
    public async Task MigrationCreatesExpectedTables()
    {
        await using var connection = new Npgsql.NpgsqlConnection(
            _postgres.GetConnectionString());
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT COUNT(*)
            FROM information_schema.tables
            WHERE table_schema = 'public'
              AND table_name IN ('products', 'orders', 'order_items');
            """;

        var count = (long)(await command.ExecuteScalarAsync())!;
        Assert.Equal(3, count);
    }
}
