using BSC.BusinessLogic.Models;
using BSC.DataAccess.Repositories;

namespace BSC.BusinessLogic.Services;

/*
 Implementación de la lógica de negocio para pedidos, validaciones de partidas, clientes y transacciones de venta.
 */
public sealed class OrderService(IOrderRepository orderRepository) : IOrderService
{
    /*Crea un pedido validando cliente, partidas, duplicados y cantidades mayores a cero*/
    public async Task<OrderCreatedDto> CreateAsync(
        int salespersonId,
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var customerName = request.CustomerName?.Trim();

        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("El nombre del cliente es obligatorio.", nameof(request));

        if (request.Items is null || request.Items.Count == 0)
            throw new ArgumentException("Se requiere al menos un producto.", nameof(request));

        if (request.Items.Any(item => item.ProductId <= 0 || item.Quantity <= 0))
            throw new ArgumentException(
                "Los identificadores y las cantidades de los productos deben ser mayores que cero.",
                nameof(request));

        if (request.Items.GroupBy(item => item.ProductId).Any(group => group.Count() > 1))
            throw new ArgumentException(
                "No se puede repetir un producto en el mismo pedido.",
                nameof(request));

        var items = request.Items.ToDictionary(item => item.ProductId, item => item.Quantity);
        var orderId = await orderRepository.CreateOrderAsync(
            salespersonId,
            customerName,
            items,
            cancellationToken);

        return new OrderCreatedDto(orderId, "Confirmed");
    }

    /*Cancelar un pedido existente*/
    public async Task CancelAsync(
        int orderId,
        int cancelledByUserId,
        CancellationToken cancellationToken = default)
    {
        if (orderId <= 0)
            throw new ArgumentException("El identificador del pedido debe ser mayor que cero.", nameof(orderId));

        await orderRepository.CancelOrderAsync(
            orderId,
            cancelledByUserId,
            cancellationToken);
    }

    /*Obtiener el listado de resúmenes de pedidos*/
    public async Task<IReadOnlyList<OrderSummaryDto>> GetSummariesAsync(
        CancellationToken cancellationToken = default)
    {
        var summaries = await orderRepository.GetSummariesAsync(cancellationToken);

        return summaries
            .Select(order => new OrderSummaryDto(
                order.OrderId,
                order.CustomerName,
                order.Status,
                order.OrderDate,
                order.SalespersonId,
                order.SalespersonName,
                order.ProductLines ?? 0,
                order.TotalItems ?? 0))
            .ToList();
    }
}
