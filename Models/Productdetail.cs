using System;
using System.Collections.Generic;

namespace Bookstore.Models;

public partial class Productdetail
{
    public int ProductId { get; set; }

    public string? ShortDescription { get; set; }

    public int? Specification { get; set; }

    public string? MotaDai { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Specification? SpecificationNavigation { get; set; }
}
