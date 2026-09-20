using BSC.DataAccess.Entities;

namespace BSC.DataAccess.Repositories;

/*
 Contrato del repositorio para operaciones de creación, cancelación y consulta de pedidos
 */
public interface IOrderRepository
{
    /*Invoca dbo.usp_CreateOrder*/
    Task<int> CreateOrderAsync(
        int userId,
        string customerName,
        IReadOnlyDictionary<int, int> items,
        CancellationToken cancellationToken = default);

    /*Invoca dbo.usp_CancelOrder para cancelar un pedido y reintegrar el stock al inventario*/
    Task CancelOrderAsync(
        int orderId,
        int cancelledByUserId,
        CancellationToken cancellationToken = default);

    /*Obtiene el listado consolidado de pedidos consultando la vista vw_OrderSummary*/
    Task<IReadOnlyList<VwOrderSummary>> GetSummariesAsync(
        CancellationToken cancellationToken = default);
}
