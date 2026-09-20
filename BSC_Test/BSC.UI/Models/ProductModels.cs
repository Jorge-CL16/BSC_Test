namespace BSC.UI.Models;

public sealed record ProductDto(
    int ProductId,
    Guid ProductKey,
    string Name,
    int StockQuantity,
    bool IsActive);

public sealed class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;

    public int StockQuantity { get; set; }
}

public sealed class UpdateProductRequest
{
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}
