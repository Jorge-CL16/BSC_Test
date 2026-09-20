CREATE OR ALTER VIEW dbo.vw_ProductStock
AS
SELECT
    p.ProductId,
    p.ProductKey,
    p.Name AS ProductName,
    p.StockQuantity,
    p.IsActive,
    CASE
        WHEN p.StockQuantity = 0 THEN 'OutOfStock'
        ELSE 'Available'
    END AS StockStatus,
    p.CreatedAt
FROM dbo.Products AS p;
GO

CREATE OR ALTER VIEW dbo.vw_OrderSummary
AS
SELECT
    o.OrderId,
    o.CustomerName,
    o.Status,
    o.OrderDate,
    o.UserId AS SalespersonId,
    u.Name AS SalespersonName,
    COUNT(od.OrderDetailId) AS ProductLines,
    COALESCE(SUM(od.Quantity), 0) AS TotalItems
FROM dbo.Orders AS o
INNER JOIN dbo.Users AS u
    ON u.UserId = o.UserId
LEFT JOIN dbo.OrderDetails AS od
    ON od.OrderId = o.OrderId
GROUP BY
    o.OrderId,
    o.CustomerName,
    o.Status,
    o.OrderDate,
    o.UserId,
    u.Name;
GO
