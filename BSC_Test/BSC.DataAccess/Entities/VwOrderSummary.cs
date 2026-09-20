using System;
using System.Collections.Generic;

namespace BSC.DataAccess.Entities;


public partial class VwOrderSummary
{

    public int OrderId { get; set; }


    public string CustomerName { get; set; } = null!;

  
    public string Status { get; set; } = null!;

  
    public DateTime OrderDate { get; set; }

  
    public int SalespersonId { get; set; }


    public string SalespersonName { get; set; } = null!;

  
    public int? ProductLines { get; set; }

    
    public int? TotalItems { get; set; }
}
