
CREATE OR ALTER TRIGGER dbo.trg_Products_PreventNegativeStock
ON dbo.Products
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM inserted
        WHERE StockQuantity < 0
    )
        THROW 50021, 'Las existencias del producto no pueden ser negativas.', 1;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_Orders_ValidateStatusChange
ON dbo.Orders
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM deleted d
        INNER JOIN inserted i ON i.OrderId = d.OrderId
        WHERE NOT
        (
            d.Status = i.Status
            OR (d.Status = 'Pending' AND i.Status IN ('Confirmed', 'Cancelled'))
            OR (d.Status = 'Confirmed' AND i.Status = 'Cancelled')
        )
    )
        THROW 50022, 'La transición de estado del pedido no está permitida.', 1;
END;
GO
