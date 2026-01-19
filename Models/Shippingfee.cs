using System;
using System.Collections.Generic;

namespace Bookstore.Models;

public partial class Shippingfee
{
    public int Id { get; set; }

    public float? KhoangCach { get; set; }

    public decimal? GiaTien { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
