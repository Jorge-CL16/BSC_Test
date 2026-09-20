using BSC.DataAccess.Context;
using BSC.DataAccess.Entities;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace BSC.DataAccess.Repositories;

/*
 mplementación del repositorio de productos y procedimientos almacenados
 */
public sealed class ProductRepository(BscDbContext dbContext) : IProductRepository
{
    /*Agrega un producto a la bd*/
    public async Task<Product> AddAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);
        return product;
    }

    /*Guarda cambios de un producto modificado*/
    public async Task UpdateAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /*Ejecuta el procedimiento almacenado dbo.usp_AdjustInventory*/
    public async Task<int> AdjustInventoryAsync(
        int productId,
        int quantityChange,
        string reason,
        CancellationToken cancellationToken = default)
    {
        await using var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "dbo.usp_AdjustInventory";
        command.CommandType = CommandType.StoredProcedure;

        AddParameter(command, "@ProductId", productId);
        AddParameter(command, "@QuantityChange", quantityChange);
        AddParameter(command, "@Reason", reason);

        if (command.Connection!.State != ConnectionState.Open)
            await command.Connection.OpenAsync(cancellationToken);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

    private static void AddParameter(
        System.Data.Common.DbCommand command,
        string name,
        object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }

    /*Consulta un producto por Id*/
    public async Task<Product?> GetByIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(
                product => product.ProductId == productId,
                cancellationToken);
    }

    /*Consulta productos con IsActive = true*/
    public async Task<IReadOnlyList<Product>> GetActiveAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .AsNoTracking()
            .Where(product => product.IsActive)
            .OrderBy(product => product.Name)
            .ToListAsync(cancellationToken);
    }

    /*usa vw_ProductStock para verificar stock */
    public async Task<IReadOnlyList<VwProductStock>> GetStockReportAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.VwProductStocks
            .AsNoTracking()
            .OrderBy(product => product.ProductName)
            .ToListAsync(cancellationToken);
    }
}
