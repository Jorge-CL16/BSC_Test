using System;
using System.Collections.Generic;

namespace BSC.DataAccess.Entities;

public partial class VwProductStock
{

    public int ProductId { get; set; }


    public Guid ProductKey { get; set; }


    public string ProductName { get; set; } = null!;

    public int StockQuantity { get; set; }


    public bool IsActive { get; set; }

    public string StockStatus { get; set; } = null!;


    public DateTime CreatedAt { get; set; }
}
