using BSC.BusinessLogic.Models;

namespace BSC.BusinessLogic.Services;

/*
 Contrato del servicio de negocio para la gestión, validación y consulta de pedidos
 */
public interface IOrderService
{
    /*Validar reglas de negocio y crea el pedido con sus partidas y reserva de stock*/
    Task<OrderCreatedDto> CreateAsync(
        int salespersonId,
        CreateOrderRequest request,
        CancellationToken cancellationToken = default);

    /*Cancelar un pedido existente y reintegra las existencias al inventario*/
    Task CancelAsync(
        int orderId,
        int cancelledByUserId,
        CancellationToken cancellationToken = default);

    /*Ver el resumen de todos los pedidos */
    Task<IReadOnlyList<OrderSummaryDto>> GetSummariesAsync(
        CancellationToken cancellationToken = default);
}
