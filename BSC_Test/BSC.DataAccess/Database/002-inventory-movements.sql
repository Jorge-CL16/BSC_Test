

IF OBJECT_ID(N'dbo.InventoryMovements', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.InventoryMovements
    (
        InventoryMovementId INT IDENTITY(1,1) NOT NULL,
        ProductId INT NOT NULL,
        OrderId INT NULL,
        MovementType VARCHAR(20) NOT NULL,
        QuantityChange INT NOT NULL,
        StockBefore INT NOT NULL,
        StockAfter INT NOT NULL,
        Reason NVARCHAR(250) NULL,
        CreatedAt DATETIME2(0) NOT NULL
            CONSTRAINT DF_InventoryMovements_CreatedAt DEFAULT (SYSUTCDATETIME()),

        CONSTRAINT PK_InventoryMovements PRIMARY KEY (InventoryMovementId),
        CONSTRAINT CK_InventoryMovements_Type
            CHECK (MovementType IN ('InitialStock', 'Sale', 'Cancellation', 'Adjustment')),
        CONSTRAINT CK_InventoryMovements_QuantityChange CHECK (QuantityChange <> 0),
        CONSTRAINT CK_InventoryMovements_StockBefore CHECK (StockBefore >= 0),
        CONSTRAINT CK_InventoryMovements_StockAfter CHECK (StockAfter >= 0),
        CONSTRAINT FK_InventoryMovements_Products FOREIGN KEY (ProductId)
            REFERENCES dbo.Products (ProductId),
        CONSTRAINT FK_InventoryMovements_Orders FOREIGN KEY (OrderId)
            REFERENCES dbo.Orders (OrderId)
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_InventoryMovements_ProductId_CreatedAt'
      AND object_id = OBJECT_ID(N'dbo.InventoryMovements')
)
BEGIN
    CREATE INDEX IX_InventoryMovements_ProductId_CreatedAt
        ON dbo.InventoryMovements (ProductId, CreatedAt DESC);
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_InventoryMovements_OrderId'
      AND object_id = OBJECT_ID(N'dbo.InventoryMovements')
)
BEGIN
    CREATE INDEX IX_InventoryMovements_OrderId
        ON dbo.InventoryMovements (OrderId);
END;
GO
