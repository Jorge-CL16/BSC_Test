namespace BSC.BusinessLogic.Models;

/*representa los datos expuestos de un producto */
public sealed record ProductDto(
    int ProductId,
    Guid ProductKey,
    string Name,
    int StockQuantity,
    bool IsActive);

/* Solicitud de entrada para registrar un nuevo producto en el catálogo */
public sealed record CreateProductRequest(
    string Name,
    int StockQuantity);

/* Solicitud para modificar el nombre o la visibilidad de un producto existente */
public sealed record UpdateProductRequest(
    string Name,
    bool IsActive);

/* Solicitud para aplicar un ajuste manual de existencias a un producto */
public sealed record AdjustInventoryRequest(
    int QuantityChange,
    string Reason);
