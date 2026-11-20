using Microsoft.EntityFrameworkCore;
using RoboCoffee.Domain.Catalog;
using RoboCoffee.Domain.Orders;

namespace RoboCoffee.Infrastructure.Persistence;

public sealed class RoboCoffeeDbContext : DbContext
{
    public RoboCoffeeDbContext(DbContextOptions<RoboCoffeeDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(RoboCoffeeDbContext).Assembly);
    }
}
