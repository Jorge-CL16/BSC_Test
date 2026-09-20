namespace BSC.BusinessLogic.Models;

/* Objeto DTO para el reporte detallado del estado de existencias de un producto */
public sealed record StockReportDto(
    int ProductId,
    Guid ProductKey,
    string ProductName,
    int StockQuantity,
    bool IsActive,
    string StockStatus);
