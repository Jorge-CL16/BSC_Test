namespace BSC.BusinessLogic.Models;

/* especificando el producto y la cantidad requerida */
public sealed record OrderItemRequest(
    int ProductId,
    int Quantity);

/* Solicitud para registrar una nueva orden de compra o pedido en el sistema */
public sealed record CreateOrderRequest(
    string CustomerName,
    IReadOnlyList<OrderItemRequest> Items);

/* Objeto de transferencia que confirma la creación exitosa de un pedido */
public sealed record OrderCreatedDto(
    int OrderId,
    string Status);

/* visualización de tablas e historiales */
public sealed record OrderSummaryDto(
    int OrderId,
    string CustomerName,
    string Status,
    DateTime OrderDate,
    int SalespersonId,
    string SalespersonName,
    int ProductLines,
    int TotalItems);
