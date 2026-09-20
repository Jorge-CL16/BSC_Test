
CREATE OR ALTER PROCEDURE dbo.usp_AdjustInventory
    @ProductId INT,
    @QuantityChange INT,
    @Reason NVARCHAR(250)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @QuantityChange = 0
        THROW 50031, 'El cambio de cantidad no puede ser cero.', 1;

    IF NULLIF(LTRIM(RTRIM(@Reason)), N'') IS NULL
        THROW 50032, 'El motivo del ajuste es obligatorio.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @StockBefore INT;

        SELECT @StockBefore = StockQuantity
        FROM dbo.Products WITH (UPDLOCK, HOLDLOCK)
        WHERE ProductId = @ProductId
          AND IsActive = 1;

        IF @StockBefore IS NULL
            THROW 50033, 'El producto no existe o está inactivo.', 1;

        IF @StockBefore + @QuantityChange < 0
            THROW 50034, 'El ajuste produciría existencias negativas.', 1;

        UPDATE dbo.Products
        SET StockQuantity = StockQuantity + @QuantityChange
        WHERE ProductId = @ProductId;

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
            @ProductId,
            NULL,
            'Adjustment',
            @QuantityChange,
            @StockBefore,
            StockQuantity,
            LTRIM(RTRIM(@Reason))
        FROM dbo.Products
        WHERE ProductId = @ProductId;

        COMMIT TRANSACTION;

        SELECT StockQuantity
        FROM dbo.Products
        WHERE ProductId = @ProductId;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;
GO
