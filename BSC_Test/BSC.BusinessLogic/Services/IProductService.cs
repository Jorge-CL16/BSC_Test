using BSC.BusinessLogic.Models;

namespace BSC.BusinessLogic.Services;

/*
 Contrato del servicio de negocio para operaciones del catálogo de productos y control de inventarios
 */
public interface IProductService
{
    /*Validar datos y crea un nuevo producto en el catálogo*/
    Task<ProductDto> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default);

    /*Actualiza el nombre o la visibilidad de un producto existente*/
    Task<ProductDto?> UpdateAsync(
        int productId,
        UpdateProductRequest request,
        CancellationToken cancellationToken = default);

    /*Registrar un ajuste de existencias para un producto específico*/
    Task<ProductDto?> AdjustInventoryAsync(
        int productId,
        AdjustInventoryRequest request,
        CancellationToken cancellationToken = default);

    /*Obtiener un producto por su identificador primario*/
    Task<ProductDto?> GetByIdAsync(
        int productId,
        CancellationToken cancellationToken = default);

    /*Obtiener la lista de todos los productos activos en el catálogo
     */
    Task<IReadOnlyList<ProductDto>> GetActiveAsync(
        CancellationToken cancellationToken = default);

    /*Crear reporte completo de existencias de todos los productos*/
    Task<IReadOnlyList<StockReportDto>> GetStockReportAsync(
        CancellationToken cancellationToken = default);
}
