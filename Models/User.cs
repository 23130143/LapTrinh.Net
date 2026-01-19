using System;
using System.Collections.Generic;

namespace Bookstore.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Phone { get; set; }

    public string? Role { get; set; }

    public virtual ICollection<Deliveryinformation> Deliveryinformations { get; set; } = new List<Deliveryinformation>();
}
