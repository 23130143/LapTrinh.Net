using System;
using System.Collections.Generic;

namespace Bookstore.Models;

public partial class Specification
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Parameterdescription> Parameterdescriptions { get; set; } = new List<Parameterdescription>();

    public virtual ICollection<Productdetail> Productdetails { get; set; } = new List<Productdetail>();
}
