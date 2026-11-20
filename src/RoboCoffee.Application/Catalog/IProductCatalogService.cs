using RoboCoffee.Contracts.Catalog;

namespace RoboCoffee.Application.Catalog;

public interface IProductCatalogService
{
    Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken);
    Task<PagedResult<ProductResponse>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<ProductResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken);
    Task DeactivateAsync(Guid id, CancellationToken cancellationToken);
}
