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

namespace RoboCoffee.IntegrationTests;

public sealed class OrderCreationTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
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
                        options.UseInMemoryDatabase("RoboCoffeeOrderTests"));
                });
            });

        _client = _factory.CreateClient();
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task CreateOrderCalculatesTotalAndCopiesProductSnapshot()
    {
        var product = await CreateProductAsync("Espresso", 12m, true);

        var response = await _client.PostAsJsonAsync("/api/orders", new CreateOrderRequest(
            "Maria",
            null,
            "01001000",
            [new CreateOrderItemRequest(product.Id, 2, "Sem acucar")]));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var order = await response.Content.ReadFromJsonAsync<OrderResponse>();
        Assert.NotNull(order);
        Assert.Equal("Maria", order.CustomerName);
        Assert.Equal("Received", order.Status);
        Assert.Equal(24m, order.Total);

        var item = Assert.Single(order.Items);
        Assert.Equal(product.Id, item.ProductId);
        Assert.Equal("Espresso", item.ProductNameSnapshot);
        Assert.Equal(12m, item.UnitPriceSnapshot);
        Assert.Equal(24m, item.Subtotal);
    }

    [Fact]
    public async Task CreateOrderWithoutItemsReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/orders", new CreateOrderRequest(
            "Maria",
            null,
            null,
            []));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateOrderWithQuantityBelowOneReturnsBadRequest()
    {
        var product = await CreateProductAsync("Espresso", 12m, true);

        var response = await _client.PostAsJsonAsync("/api/orders", new CreateOrderRequest(
            "Maria",
            null,
            null,
            [new CreateOrderItemRequest(product.Id, 0, null)]));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateOrderWithUnknownProductReturnsNotFound()
    {
        var response = await _client.PostAsJsonAsync("/api/orders", new CreateOrderRequest(
            "Maria",
            null,
            null,
            [new CreateOrderItemRequest(Guid.NewGuid(), 1, null)]));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateOrderWithUnavailableProductReturnsConflict()
    {
        var product = await CreateProductAsync("Espresso", 12m, false);

        var response = await _client.PostAsJsonAsync("/api/orders", new CreateOrderRequest(
            "Maria",
            null,
            null,
            [new CreateOrderItemRequest(product.Id, 1, null)]));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task ChangingProductDoesNotRecalculatePersistedOrderSnapshot()
    {
        var product = await CreateProductAsync("Espresso", 12m, true);

        var createResponse = await _client.PostAsJsonAsync("/api/orders", new CreateOrderRequest(
            "Maria",
            null,
            null,
            [new CreateOrderItemRequest(product.Id, 2, null)]));

        var order = await createResponse.Content.ReadFromJsonAsync<OrderResponse>();
        Assert.NotNull(order);

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/products/{product.Id}",
            new UpdateProductRequest("Espresso Especial", "Coffee", "P", 20m, true, null));
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RoboCoffeeDbContext>();
        var persistedOrder = await context.Orders
            .Include(candidate => candidate.Items)
            .SingleAsync(candidate => candidate.Id == order.Id);

        var persistedItem = Assert.Single(persistedOrder.Items);
        Assert.Equal("Espresso", persistedItem.ProductNameSnapshot);
        Assert.Equal(12m, persistedItem.UnitPriceSnapshot);
        Assert.Equal(24m, persistedItem.Subtotal);
        Assert.Equal(24m, persistedOrder.Total);
    }

    [Fact]
    public async Task AdvanceStatusFollowsReceivedPreparingReadyDeliveredWorkflow()
    {
        var order = await CreateOrderAsync();

        var preparing = await _client.PostAsync(
            $"/api/orders/{order.Id}/advance",
            null);
        Assert.Equal(HttpStatusCode.OK, preparing.StatusCode);
        Assert.Equal("Preparing", (await preparing.Content.ReadFromJsonAsync<OrderResponse>())?.Status);

        var ready = await _client.PostAsync(
            $"/api/orders/{order.Id}/advance",
            null);
        Assert.Equal(HttpStatusCode.OK, ready.StatusCode);
        Assert.Equal("Ready", (await ready.Content.ReadFromJsonAsync<OrderResponse>())?.Status);

        var delivered = await _client.PostAsync(
            $"/api/orders/{order.Id}/advance",
            null);
        Assert.Equal(HttpStatusCode.OK, delivered.StatusCode);
        Assert.Equal("Delivered", (await delivered.Content.ReadFromJsonAsync<OrderResponse>())?.Status);

        var invalid = await _client.PostAsync(
            $"/api/orders/{order.Id}/advance",
            null);
        Assert.Equal(HttpStatusCode.Conflict, invalid.StatusCode);
    }

    [Fact]
    public async Task CancelOrderSucceedsOnlyInReceivedStatus()
    {
        var order = await CreateOrderAsync();

        var cancel = await _client.PostAsync(
            $"/api/orders/{order.Id}/cancel",
            null);
        Assert.Equal(HttpStatusCode.OK, cancel.StatusCode);
        Assert.Equal("Cancelled", (await cancel.Content.ReadFromJsonAsync<OrderResponse>())?.Status);

        var preparingOrder = await CreateOrderAsync();
        var advance = await _client.PostAsync(
            $"/api/orders/{preparingOrder.Id}/advance",
            null);
        Assert.Equal(HttpStatusCode.OK, advance.StatusCode);

        var invalidCancel = await _client.PostAsync(
            $"/api/orders/{preparingOrder.Id}/cancel",
            null);
        Assert.Equal(HttpStatusCode.Conflict, invalidCancel.StatusCode);
    }

    [Fact]
    public async Task AdvanceUnknownOrderReturnsNotFound()
    {
        var response = await _client.PostAsync(
            $"/api/orders/{Guid.NewGuid()}/advance",
            null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CancelUnknownOrderReturnsNotFound()
    {
        var response = await _client.PostAsync(
            $"/api/orders/{Guid.NewGuid()}/cancel",
            null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<OrderResponse> CreateOrderAsync()
    {
        var product = await CreateProductAsync("Espresso", 12m, true);
        var response = await _client.PostAsJsonAsync("/api/orders", new CreateOrderRequest(
            "Maria",
            null,
            null,
            [new CreateOrderItemRequest(product.Id, 1, null)]));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var order = await response.Content.ReadFromJsonAsync<OrderResponse>();
        Assert.NotNull(order);
        return order;
    }

    private async Task<ProductResponse> CreateProductAsync(
        string name,
        decimal price,
        bool isAvailable)
    {
        var response = await _client.PostAsJsonAsync("/api/products",
            new CreateProductRequest(name, "Coffee", "P", price, isAvailable, null));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var product = await response.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.NotNull(product);
        return product;
    }
}
