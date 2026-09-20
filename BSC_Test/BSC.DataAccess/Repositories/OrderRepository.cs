using System.Data;
using BSC.DataAccess.Context;
using BSC.DataAccess.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BSC.DataAccess.Repositories;

/*
 Implementación del repositorio de pedidos que interactúa con los procedimientos almacenados 
 */
public sealed class OrderRepository(BscDbContext dbContext) : IOrderRepository
{
    /*Crea una orden ejecutando dbo.usp_CreateOrder*/
    public async Task<int> CreateOrderAsync(
        int userId,
        string customerName,
        IReadOnlyDictionary<int, int> items,
        CancellationToken cancellationToken = default)
    {
        await using var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "dbo.usp_CreateOrder";
        command.CommandType = CommandType.StoredProcedure;

        AddParameter(command, "@UserId", userId);
        AddParameter(command, "@CustomerName", customerName);

        var table = new DataTable();
        table.Columns.Add("ProductId", typeof(int));
        table.Columns.Add("Quantity", typeof(int));

        foreach (var item in items)
            table.Rows.Add(item.Key, item.Value);

        var itemsParameter = new SqlParameter("@Items", SqlDbType.Structured)
        {
            TypeName = "dbo.OrderItemType",
            Value = table
        };
        command.Parameters.Add(itemsParameter);

        await OpenConnectionAsync(command, cancellationToken);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

    /* Cancela una orden mediante dbo.usp_CancelOrder. */
    public async Task CancelOrderAsync(
        int orderId,
        int cancelledByUserId,
        CancellationToken cancellationToken = default)
    {
        await using var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "dbo.usp_CancelOrder";
        command.CommandType = CommandType.StoredProcedure;

        AddParameter(command, "@OrderId", orderId);
        AddParameter(command, "@CancelledByUserId", cancelledByUserId);

        await OpenConnectionAsync(command, cancellationToken);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    /*Consultar resúmenes de pedidos desde vw_OrderSummary*/
    public async Task<IReadOnlyList<VwOrderSummary>> GetSummariesAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.VwOrderSummaries
            .AsNoTracking()
            .OrderByDescending(order => order.OrderDate)
            .ToListAsync(cancellationToken);
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

    private static async Task OpenConnectionAsync(
        System.Data.Common.DbCommand command,
        CancellationToken cancellationToken)
    {
        if (command.Connection!.State != ConnectionState.Open)
            await command.Connection.OpenAsync(cancellationToken);
    }
}
