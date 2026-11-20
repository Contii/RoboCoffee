using Microsoft.EntityFrameworkCore;
using RoboCoffee.Infrastructure.Persistence;

namespace RoboCoffee.UnitTests;

public sealed class DbContextModelTests
{
    [Fact]
    public void DomainEntitiesAreMappedAsPostgresTables()
    {
        var options = new DbContextOptionsBuilder<RoboCoffeeDbContext>()
            .UseNpgsql("Host=localhost;Database=robocoffee")
            .Options;

        using var context = new RoboCoffeeDbContext(options);
        var model = context.Model;

        Assert.NotNull(model.FindEntityType(
            typeof(RoboCoffee.Domain.Catalog.Product)));
        Assert.NotNull(model.FindEntityType(
            typeof(RoboCoffee.Domain.Orders.Order)));
        Assert.NotNull(model.FindEntityType(
            typeof(RoboCoffee.Domain.Orders.OrderItem)));

        Assert.Equal(
            "products",
            model.FindEntityType(typeof(RoboCoffee.Domain.Catalog.Product))!.GetTableName());
        Assert.Equal(
            "orders",
            model.FindEntityType(typeof(RoboCoffee.Domain.Orders.Order))!.GetTableName());
        Assert.Equal(
            "order_items",
            model.FindEntityType(typeof(RoboCoffee.Domain.Orders.OrderItem))!.GetTableName());
    }
}
