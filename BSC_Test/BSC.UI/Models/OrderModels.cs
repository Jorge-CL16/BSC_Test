namespace BSC.UI.Models;

public sealed class OrderItemRequest
{
    public int ProductId { get; set; }

    public int Quantity { get; set; }
}

public sealed class CreateOrderRequest
{
    public string CustomerName { get; set; } = string.Empty;

    public List<OrderItemRequest> Items { get; set; } = [];
}

public sealed record OrderCreatedDto(
    int OrderId,
    string Status);

public sealed record OrderSummaryDto(
    int OrderId,
    string CustomerName,
    string Status,
    DateTime OrderDate,
    int SalespersonId,
    string SalespersonName,
    int ProductLines,
    int TotalItems);
