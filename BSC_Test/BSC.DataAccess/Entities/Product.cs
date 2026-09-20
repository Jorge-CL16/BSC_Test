using System;
using System.Collections.Generic;

namespace BSC.DataAccess.Entities;


public partial class Product
{
  
    public int ProductId { get; set; }

  
    public Guid ProductKey { get; set; }


    public string Name { get; set; } = null!;


    public int StockQuantity { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

  
    public virtual ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();

   
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
