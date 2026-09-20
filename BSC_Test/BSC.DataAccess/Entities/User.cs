using System;
using System.Collections.Generic;

namespace BSC.DataAccess.Entities;


public partial class User
{
    
    public int UserId { get; set; }

    
    public byte UserRoleId { get; set; }

    
    public string Name { get; set; } = null!;

   
    public string Email { get; set; } = null!;

  
    public string PasswordHash { get; set; } = null!;

    
    public bool Active { get; set; }


    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
