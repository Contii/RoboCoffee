using RoboCoffee.Domain.Catalog;
using RoboCoffee.Domain.Exceptions;
using RoboCoffee.Domain.Orders;

namespace RoboCoffee.UnitTests;

public sealed class DomainModelTests
{
    [Fact]
    public void ProductRejectsEmptyName()
    {
        Assert.Throws<DomainException>(() => new Product(
            Guid.NewGuid(),
            " ",
            ProductCategory.Coffee,
            ProductSize.M,
            12m));
    }

    [Fact]
    public void ProductRejectsZeroPrice()
    {
        Assert.Throws<DomainException>(() => new Product(
            Guid.NewGuid(),
            "Espresso",
            ProductCategory.Coffee,
            ProductSize.P,
            0m));
    }

    [Fact]
    public void OrderRejectsEmptyCustomerName()
    {
        Assert.Throws<DomainException>(() => Order.Create(
            Guid.NewGuid(),
            " ",
            null,
            null));
    }

    [Fact]
    public void OrderRejectsEmptyItemList()
    {
        var order = Order.Create(Guid.NewGuid(), "Joao", null, null);

        Assert.Throws<DomainException>(() => order.EnsureHasItems());
    }

    [Fact]
    public void OrderItemRejectsQuantityBelowOne()
    {
        var product = CreateAvailableProduct();

        Assert.Throws<DomainException>(() =>
            OrderItem.Create(Guid.NewGuid(), product, 0));
    }

    [Fact]
    public void OrderItemCopiesCurrentProductNameAndPrice()
    {
        var product = CreateAvailableProduct();
        var order = Order.Create(Guid.NewGuid(), "Maria", null, null);

        order.AddItem(product, 2);

        var item = Assert.Single(order.Items);
        Assert.Equal(product.Id, item.ProductId);
        Assert.Equal(product.Name, item.ProductNameSnapshot);
        Assert.Equal(product.Price, item.UnitPriceSnapshot);
        Assert.Equal(product.Price * 2, item.Subtotal);
        Assert.Equal(item.Subtotal, order.Total);
    }

    [Fact]
    public void ChangingProductPriceDoesNotRecalculateOrderItem()
    {
        var product = CreateAvailableProduct();
        var order = Order.Create(Guid.NewGuid(), "Maria", null, null);

        order.AddItem(product, 2);
        product.UpdatePrice(20m);

        var item = Assert.Single(order.Items);
        Assert.Equal(12m, item.UnitPriceSnapshot);
        Assert.Equal(24m, item.Subtotal);
        Assert.Equal(24m, order.Total);
    }

    [Fact]
    public void UnavailableProductCannotBeAdded()
    {
        var product = CreateAvailableProduct();
        product.SetAvailability(false);
        var order = Order.Create(Guid.NewGuid(), "Maria", null, null);

        Assert.Throws<DomainException>(() => order.AddItem(product, 1));
    }

    [Fact]
    public void OrderStatusFollowsExpectedWorkflow()
    {
        var order = CreateOrderWithItem();

        Assert.Equal(OrderStatus.Received, order.Status);

        order.AdvanceStatus();
        Assert.Equal(OrderStatus.Preparing, order.Status);

        order.AdvanceStatus();
        Assert.Equal(OrderStatus.Ready, order.Status);

        order.AdvanceStatus();
        Assert.Equal(OrderStatus.Delivered, order.Status);

        Assert.Throws<DomainException>(() => order.AdvanceStatus());
    }

    [Fact]
    public void OnlyReceivedOrderCanBeCancelled()
    {
        var order = CreateOrderWithItem();

        order.Cancel();

        Assert.Equal(OrderStatus.Cancelled, order.Status);
        Assert.Throws<DomainException>(() => order.AdvanceStatus());

        var preparingOrder = CreateOrderWithItem();
        preparingOrder.AdvanceStatus();
        Assert.Throws<DomainException>(() => preparingOrder.Cancel());
    }

    [Theory]
    [InlineData(OrderStatus.Preparing)]
    [InlineData(OrderStatus.Ready)]
    [InlineData(OrderStatus.Delivered)]
    [InlineData(OrderStatus.Cancelled)]
    public void OnlyReceivedOrderCanBeCancelledForEveryRestrictedStatus(
        OrderStatus targetStatus)
    {
        var order = CreateOrderInStatus(targetStatus);

        Assert.Throws<DomainException>(() => order.Cancel());
    }

    [Theory]
    [InlineData(OrderStatus.Delivered)]
    [InlineData(OrderStatus.Cancelled)]
    public void InvalidAdvanceStatusIsRejected(
        OrderStatus targetStatus)
    {
        var order = CreateOrderInStatus(targetStatus);

        Assert.Throws<DomainException>(() => order.AdvanceStatus());
    }

    [Fact]
    public void InactiveProductCannotBeAdded()
    {
        var product = CreateAvailableProduct();
        product.Deactivate();
        var order = Order.Create(Guid.NewGuid(), "Maria", null, null);

        Assert.Throws<DomainException>(() => order.AddItem(product, 1));
    }

    [Theory]
    [InlineData(OrderStatus.Preparing)]
    [InlineData(OrderStatus.Ready)]
    [InlineData(OrderStatus.Delivered)]
    [InlineData(OrderStatus.Cancelled)]
    public void ItemsCannotBeAddedAfterReceivedStatus(
        OrderStatus targetStatus)
    {
        var order = CreateOrderInStatus(targetStatus);
        var product = CreateAvailableProduct();

        Assert.Throws<DomainException>(() => order.AddItem(product, 1));
    }

    private static Product CreateAvailableProduct()
    {
        return new Product(
            Guid.NewGuid(),
            "Espresso",
            ProductCategory.Coffee,
            ProductSize.P,
            12m);
    }

    private static Order CreateOrderWithItem()
    {
        var product = CreateAvailableProduct();
        var order = Order.Create(Guid.NewGuid(), "Maria", null, null);
        order.AddItem(product, 1);
        return order;
    }

    private static Order CreateOrderInStatus(OrderStatus targetStatus)
    {
        var order = CreateOrderWithItem();

        return targetStatus switch
        {
            OrderStatus.Received => order,
            OrderStatus.Preparing => AdvanceOrder(order, 1),
            OrderStatus.Ready => AdvanceOrder(order, 2),
            OrderStatus.Delivered => AdvanceOrder(order, 3),
            OrderStatus.Cancelled => CancelOrder(order),
            _ => throw new ArgumentOutOfRangeException(nameof(targetStatus))
        };
    }

    private static Order AdvanceOrder(Order order, int advances)
    {
        for (var index = 0; index < advances; index++)
        {
            order.AdvanceStatus();
        }

        return order;
    }

    private static Order CancelOrder(Order order)
    {
        order.Cancel();
        return order;
    }
}
