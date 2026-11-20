using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RoboCoffee.Contracts.Catalog;
using RoboCoffee.Infrastructure.Persistence;

namespace RoboCoffee.IntegrationTests;

public sealed class ProductCatalogTests : IAsyncLifetime
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
                        options.UseInMemoryDatabase("RoboCoffeeCatalogTests"));
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
    public async Task CreateAndGetProduct()
    {
        var request = new CreateProductRequest(
            "Espresso",
            "Coffee",
            "P",
            12m,
            true,
            null);

        var createResponse = await _client.PostAsJsonAsync("/api/products", request);
        if (!createResponse.IsSuccessStatusCode)
        {
            Assert.Fail(await createResponse.Content.ReadAsStringAsync());
        }
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.NotNull(created);
        Assert.Equal("Espresso", created!.Name);
        Assert.Equal("Coffee", created.Category);

        var getResponse = await _client.GetAsync($"/api/products/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var found = await getResponse.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.NotNull(found);
        Assert.Equal(created.Id, found!.Id);
    }

    [Fact]
    public async Task ListProductsIsPaginated()
    {
        await _client.PostAsJsonAsync("/api/products", new CreateProductRequest(
            "Latte",
            "Coffee",
            "M",
            18m,
            true,
            null));

        var response = await _client.GetAsync("/api/products?page=1&pageSize=10");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var page = await response.Content.ReadFromJsonAsync<PagedResult<ProductResponse>>();
        Assert.NotNull(page);
        Assert.Equal(1, page.Page);
        Assert.Equal(10, page.PageSize);
        Assert.Equal(1, page.TotalItems);
        Assert.Single(page.Items);
    }

    [Fact]
    public async Task UpdateProduct()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/products",
            new CreateProductRequest("Tea", "Tea", "P", 8m, true, null));
        var created = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.NotNull(created);

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/products/{created!.Id}",
            new UpdateProductRequest("Green Tea", "Tea", "G", 10m, true, null));

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.NotNull(updated);
        Assert.Equal("Green Tea", updated.Name);
        Assert.Equal(10m, updated.Price);
    }

    [Fact]
    public async Task DeactivateProductReturnsNotFoundAfterDelete()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/products",
            new CreateProductRequest("Cake", "Dessert", "G", 15m, true, null));
        var created = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.NotNull(created);

        var deleteResponse = await _client.DeleteAsync($"/api/products/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/products/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task UnknownProductReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateProductWithInvalidRequestReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/products",
            new CreateProductRequest("", "Invalid", "X", 0m, true, -1));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.True(problem.Errors.ContainsKey("Name"));
        Assert.True(problem.Errors.ContainsKey("Category"));
        Assert.True(problem.Errors.ContainsKey("Size"));
        Assert.True(problem.Errors.ContainsKey("Price"));
        Assert.True(problem.Errors.ContainsKey("StockQuantity"));
    }

    [Fact]
    public async Task UpdateProductWithInvalidRequestReturnsBadRequest()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/products",
            new CreateProductRequest("Tea", "Tea", "P", 8m, true, null));
        var created = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.NotNull(created);

        var response = await _client.PutAsJsonAsync(
            $"/api/products/{created!.Id}",
            new UpdateProductRequest("", "Invalid", "X", -1m, true, -1));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.True(problem.Errors.ContainsKey("Name"));
        Assert.True(problem.Errors.ContainsKey("Category"));
        Assert.True(problem.Errors.ContainsKey("Size"));
        Assert.True(problem.Errors.ContainsKey("Price"));
        Assert.True(problem.Errors.ContainsKey("StockQuantity"));
    }
}
