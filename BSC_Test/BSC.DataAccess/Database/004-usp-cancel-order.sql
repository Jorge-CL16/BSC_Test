CREATE OR ALTER PROCEDURE dbo.usp_CancelOrder
    @OrderId INT,
    @CancelledByUserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @CurrentStatus VARCHAR(20);

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Users
        WHERE UserId = @CancelledByUserId
          AND UserRoleId IN (1, 2)
          AND Active = 1
    )
        THROW 50011, 'El usuario no está autorizado para cancelar pedidos.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        SELECT @CurrentStatus = Status
        FROM dbo.Orders WITH (UPDLOCK, HOLDLOCK)
        WHERE OrderId = @OrderId;

        IF @CurrentStatus IS NULL
            THROW 50012, 'El pedido no existe.', 1;

        IF @CurrentStatus <> 'Confirmed'
            THROW 50013, 'Solo se pueden cancelar pedidos confirmados.', 1;

        IF EXISTS
        (
            SELECT 1
            FROM dbo.OrderDetails od
            INNER JOIN dbo.Products p WITH (UPDLOCK, HOLDLOCK)
                ON p.ProductId = od.ProductId
            WHERE od.OrderId = @OrderId
        )
        BEGIN
            DECLARE @InventoryChanges TABLE
            (
                ProductId INT NOT NULL,
                StockBefore INT NOT NULL,
                StockAfter INT NOT NULL,
                QuantityChange INT NOT NULL
            );

            UPDATE p
            SET StockQuantity = p.StockQuantity + od.Quantity
            OUTPUT
                inserted.ProductId,
                deleted.StockQuantity,
                inserted.StockQuantity,
                od.Quantity
            INTO @InventoryChanges (ProductId, StockBefore, StockAfter, QuantityChange)
            FROM dbo.Products p
            INNER JOIN dbo.OrderDetails od
                ON od.ProductId = p.ProductId
            WHERE od.OrderId = @OrderId;

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
                'Cancellation',
                QuantityChange,
                StockBefore,
                StockAfter,
                N'Order cancellation'
            FROM @InventoryChanges;
        END;

        UPDATE dbo.Orders
        SET Status = 'Cancelled'
        WHERE OrderId = @OrderId;

        COMMIT TRANSACTION;

        SELECT @OrderId AS OrderId, 'Cancelled' AS Status;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;
GO
