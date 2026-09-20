using BSC.BusinessLogic.Models;
using BSC.DataAccess.Repositories;

namespace BSC.BusinessLogic.Services;

/*
 Implementación de la lógica de negocio para la administración de productos y ajustes de existencias
 */
public sealed class ProductService(IProductRepository productRepository) : IProductService
{
    /*Validar y crea un producto con su stock inicial 0*/
    public async Task<ProductDto> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var name = request.Name?.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del producto es obligatorio.", nameof(request));

        if (request.StockQuantity < 0)
            throw new ArgumentException("La cantidad de existencias no puede ser negativa.", nameof(request));

        var product = new BSC.DataAccess.Entities.Product
        {
            ProductKey = Guid.NewGuid(),
            Name = name,
            StockQuantity = request.StockQuantity,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createdProduct = await productRepository.AddAsync(product, cancellationToken);

        return new ProductDto(
            createdProduct.ProductId,
            createdProduct.ProductKey,
            createdProduct.Name,
            createdProduct.StockQuantity,
            createdProduct.IsActive);
    }

    /*Actualiza nombre y visibilidad de un producto*/
    public async Task<ProductDto?> UpdateAsync(
        int productId,
        UpdateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(productId, cancellationToken);

        if (product is null)
            return null;

        var name = request.Name?.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del producto es obligatorio.", nameof(request));

        product.Name = name;
        product.IsActive = request.IsActive;

        await productRepository.UpdateAsync(product, cancellationToken);

        return new ProductDto(
            product.ProductId,
            product.ProductKey,
            product.Name,
            product.StockQuantity,
            product.IsActive);
    }

    /*Aplica un ajuste manual al inventario vía procedimiento almacenado*/
    public async Task<ProductDto?> AdjustInventoryAsync(
        int productId,
        AdjustInventoryRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.QuantityChange == 0)
            throw new ArgumentException("El cambio de cantidad no puede ser cero.", nameof(request));

        if (string.IsNullOrWhiteSpace(request.Reason))
            throw new ArgumentException("El motivo del ajuste es obligatorio.", nameof(request));

        var product = await productRepository.GetByIdAsync(productId, cancellationToken);

        if (product is null)
            return null;

        await productRepository.AdjustInventoryAsync(
            productId,
            request.QuantityChange,
            request.Reason.Trim(),
            cancellationToken);

        product = await productRepository.GetByIdAsync(productId, cancellationToken);

        return product is null
            ? null
            : new ProductDto(
                product.ProductId,
                product.ProductKey,
                product.Name,
                product.StockQuantity,
                product.IsActive);
    }

    /*Obtiene producto por Id*/
    public async Task<ProductDto?> GetByIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(productId, cancellationToken);

        return product is null
            ? null
            : new ProductDto(
                product.ProductId,
                product.ProductKey,
                product.Name,
                product.StockQuantity,
                product.IsActive);
    }

    /*Obtiene catálogo activo*/
    public async Task<IReadOnlyList<ProductDto>> GetActiveAsync(
        CancellationToken cancellationToken = default)
    {
        var products = await productRepository.GetActiveAsync(cancellationToken);

        return products
            .Select(product => new ProductDto(
                product.ProductId,
                product.ProductKey,
                product.Name,
                product.StockQuantity,
                product.IsActive))
            .ToList();
    }

    /*Genera reporte de stock*/
    public async Task<IReadOnlyList<StockReportDto>> GetStockReportAsync(
        CancellationToken cancellationToken = default)
    {
        var report = await productRepository.GetStockReportAsync(cancellationToken);

        return report
            .Select(product => new StockReportDto(
                product.ProductId,
                product.ProductKey,
                product.ProductName,
                product.StockQuantity,
                product.IsActive,
                product.StockStatus))
            .ToList();
    }
}
