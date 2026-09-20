

IF TYPE_ID(N'dbo.OrderItemType') IS NULL
BEGIN
    EXEC(N'
        CREATE TYPE dbo.OrderItemType AS TABLE
        (
            ProductId INT NOT NULL,
            Quantity INT NOT NULL
        );
    ');
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_CreateOrder
    @UserId INT,
    @CustomerName NVARCHAR(150),
    @Items dbo.OrderItemType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @OrderId INT;

    IF NULLIF(LTRIM(RTRIM(@CustomerName)), N'') IS NULL
        THROW 50001, 'El nombre del cliente es obligatorio.', 1;

    IF NOT EXISTS (SELECT 1 FROM @Items)
        THROW 50002, 'Se requiere al menos un artículo en el pedido.', 1;

    IF EXISTS (SELECT 1 FROM @Items WHERE Quantity <= 0)
        THROW 50003, 'Las cantidades del pedido deben ser mayores que cero.', 1;

    IF EXISTS
    (
        SELECT ProductId
        FROM @Items
        GROUP BY ProductId
        HAVING COUNT(*) > 1
    )
        THROW 50004, 'No se puede repetir un producto en el mismo pedido.', 1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Users
        WHERE UserId = @UserId
          AND UserRoleId = 3
          AND Active = 1
    )
        THROW 50005, 'El usuario no es un vendedor activo.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS
        (
            SELECT 1
            FROM @Items i
            LEFT JOIN dbo.Products p WITH (UPDLOCK, HOLDLOCK)
                ON p.ProductId = i.ProductId
               AND p.IsActive = 1
            WHERE p.ProductId IS NULL
        )
            THROW 50006, 'Uno o más productos no son válidos o están inactivos.', 1;

        IF EXISTS
        (
            SELECT 1
            FROM @Items i
            INNER JOIN dbo.Products p WITH (UPDLOCK, HOLDLOCK)
                ON p.ProductId = i.ProductId
            WHERE p.StockQuantity < i.Quantity
        )
            THROW 50007, 'No hay existencias suficientes para uno o más productos.', 1;

        INSERT INTO dbo.Orders (UserId, CustomerName, Status)
        VALUES (@UserId, LTRIM(RTRIM(@CustomerName)), 'Confirmed');

        SET @OrderId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.OrderDetails (OrderId, ProductId, Quantity)
        SELECT @OrderId, ProductId, Quantity
        FROM @Items;

        DECLARE @InventoryChanges TABLE
        (
            ProductId INT NOT NULL,
            StockBefore INT NOT NULL,
            StockAfter INT NOT NULL,
            QuantityChange INT NOT NULL
        );

        UPDATE p
        SET StockQuantity = p.StockQuantity - i.Quantity
        OUTPUT
            inserted.ProductId,
            deleted.StockQuantity,
            inserted.StockQuantity,
            -i.Quantity
        INTO @InventoryChanges (ProductId, StockBefore, StockAfter, QuantityChange)
        FROM dbo.Products p
        INNER JOIN @Items i ON i.ProductId = p.ProductId;

        INSERT INTO dbo.InventoryMovements
        (
            ProductId,
            OrderId,
            MovementType,
            QuantityChange,
            StockBefore,
            StockAfter,
            Reason
        )
        SELECT
            ProductId,
            @OrderId,
            'Sale',
            QuantityChange,
            StockBefore,
            StockAfter,
            N'Order confirmation'
        FROM @InventoryChanges;

        COMMIT TRANSACTION;

        SELECT @OrderId AS OrderId;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;
GO
