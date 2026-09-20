namespace BSC.UI.Models;

public sealed record StockReportDto(
    int ProductId,
    Guid ProductKey,
    string ProductName,
    int StockQuantity,
    bool IsActive,
    string StockStatus);

public sealed class AdjustInventoryRequest
{
    public int QuantityChange { get; set; }

    public string Reason { get; set; } = string.Empty;
}
