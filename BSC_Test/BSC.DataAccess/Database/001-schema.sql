/*


    UserRoleId values:
        1 = Administrator
        2 = AdministrativeStaff
        3 = Salesperson
*/

IF OBJECT_ID(N'dbo.OrderDetails', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.OrderDetails
    (
        OrderDetailId INT IDENTITY(1,1) NOT NULL,
        OrderId INT NOT NULL,
        ProductId INT NOT NULL,
        Quantity INT NOT NULL,

        CONSTRAINT PK_OrderDetails PRIMARY KEY (OrderDetailId),
        CONSTRAINT CK_OrderDetails_Quantity_Positive CHECK (Quantity > 0),
        CONSTRAINT UQ_OrderDetails_Order_Product UNIQUE (OrderId, ProductId)
    );
END;
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        UserId INT IDENTITY(1,1) NOT NULL,
        UserRoleId TINYINT NOT NULL,
        Name NVARCHAR(150) NOT NULL,
        Email NVARCHAR(254) NOT NULL,
        PasswordHash NVARCHAR(500) NOT NULL,
        Active BIT NOT NULL CONSTRAINT DF_Users_Active DEFAULT (1),

        CONSTRAINT PK_Users PRIMARY KEY (UserId),
        CONSTRAINT UQ_Users_Email UNIQUE (Email),
        CONSTRAINT CK_Users_UserRoleId CHECK (UserRoleId IN (1, 2, 3))
    );
END;
GO

IF OBJECT_ID(N'dbo.Products', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Products
    (
        ProductId INT IDENTITY(1,1) NOT NULL,
        ProductKey UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT DF_Products_ProductKey DEFAULT (NEWSEQUENTIALID()),
        Name NVARCHAR(150) NOT NULL,
        StockQuantity INT NOT NULL CONSTRAINT DF_Products_StockQuantity DEFAULT (0),
        IsActive BIT NOT NULL CONSTRAINT DF_Products_IsActive DEFAULT (1),
        CreatedAt DATETIME2(0) NOT NULL
            CONSTRAINT DF_Products_CreatedAt DEFAULT (SYSUTCDATETIME()),

        CONSTRAINT PK_Products PRIMARY KEY (ProductId),
        CONSTRAINT UQ_Products_ProductKey UNIQUE (ProductKey),
        CONSTRAINT CK_Products_StockQuantity_NonNegative CHECK (StockQuantity >= 0)
    );
END;
GO

IF OBJECT_ID(N'dbo.Orders', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Orders
    (
        OrderId INT IDENTITY(1,1) NOT NULL,
        UserId INT NOT NULL,
        CustomerName NVARCHAR(150) NOT NULL,
        Status VARCHAR(20) NOT NULL CONSTRAINT DF_Orders_Status DEFAULT ('Pending'),
        OrderDate DATETIME2(0) NOT NULL
            CONSTRAINT DF_Orders_OrderDate DEFAULT (SYSUTCDATETIME()),

        CONSTRAINT PK_Orders PRIMARY KEY (OrderId),
        CONSTRAINT CK_Orders_Status CHECK (Status IN ('Pending', 'Confirmed', 'Cancelled')),
        CONSTRAINT FK_Orders_Users FOREIGN KEY (UserId)
            REFERENCES dbo.Users (UserId)
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_OrderDetails_Orders'
      AND parent_object_id = OBJECT_ID(N'dbo.OrderDetails')
)
BEGIN
    ALTER TABLE dbo.OrderDetails
        ADD CONSTRAINT FK_OrderDetails_Orders FOREIGN KEY (OrderId)
            REFERENCES dbo.Orders (OrderId);
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_OrderDetails_Products'
      AND parent_object_id = OBJECT_ID(N'dbo.OrderDetails')
)
BEGIN
    ALTER TABLE dbo.OrderDetails
        ADD CONSTRAINT FK_OrderDetails_Products FOREIGN KEY (ProductId)
            REFERENCES dbo.Products (ProductId);
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Orders_UserId_OrderDate'
      AND object_id = OBJECT_ID(N'dbo.Orders')
)
BEGIN
    CREATE INDEX IX_Orders_UserId_OrderDate
        ON dbo.Orders (UserId, OrderDate DESC);
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_OrderDetails_ProductId'
      AND object_id = OBJECT_ID(N'dbo.OrderDetails')
)
BEGIN
    CREATE INDEX IX_OrderDetails_ProductId
        ON dbo.OrderDetails (ProductId);
END;
GO
