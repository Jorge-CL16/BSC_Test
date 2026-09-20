using System;
using System.Collections.Generic;

namespace BSC.DataAccess.Entities;


public partial class Order
{
    public int OrderId { get; set; }


    public int UserId { get; set; }


    public string CustomerName { get; set; } = null!;

    public string Status { get; set; } = null!;


    public DateTime OrderDate { get; set; }

    public virtual ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();

 
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();


    public virtual User User { get; set; } = null!;
}
