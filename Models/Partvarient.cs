using System;
using System.Collections.Generic;

namespace Bookstore.Models;

public partial class Partvarient
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<Productvariant> Productvariants { get; set; } = new List<Productvariant>();
}
