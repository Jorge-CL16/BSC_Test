using BSC.DataAccess.Entities;

namespace BSC.DataAccess.Repositories;

/*
 Contrato del repositorio para operaciones de persistencia y consulta de productos e inventarios
 */
public interface IProductRepository
{
    /*Agrega un nuevo producto a la bd*/
    Task<Product> AddAsync(
        Product product,
        CancellationToken cancellationToken = default);

    /*Actualiza los cambios en un producto existente en la base de datos*/
    Task UpdateAsync(
        Product product,
        CancellationToken cancellationToken = default);

    /*Ejecuta dbo.usp_AdjustInventory para registrar un movimiento y actualizar existencias*/
    Task<int> AdjustInventoryAsync(
        int productId,
        int quantityChange,
        string reason,
        CancellationToken cancellationToken = default);

    /*Obtener un producto por su identificador único*/
    Task<Product?> GetByIdAsync(int productId, CancellationToken cancellationToken = default);

    /*Obtener los productos que tienen la bandera de activo habilitada, ordenados por nombre*/
    Task<IReadOnlyList<Product>> GetActiveAsync(
        CancellationToken cancellationToken = default);

    /*Consulta la vista vw_ProductStock para obtener el reporte consolidado de existencias y estatus*/
    Task<IReadOnlyList<VwProductStock>> GetStockReportAsync(
        CancellationToken cancellationToken = default);
}
