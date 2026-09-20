using System;
using System.Collections.Generic;

namespace BSC.DataAccess.Entities;


public partial class InventoryMovement
{
  
    public int InventoryMovementId { get; set; }

    public int ProductId { get; set; }

    public int? OrderId { get; set; }

    public string MovementType { get; set; } = null!;

    public int QuantityChange { get; set; }


    public int StockBefore { get; set; }


    public int StockAfter { get; set; }


    public string? Reason { get; set; }


    public DateTime CreatedAt { get; set; }

    public virtual Order? Order { get; set; }


    public virtual Product Product { get; set; } = null!;
}
