using System;
using System.Collections.Generic;

namespace Bookstore.Models;

public partial class Review
{
    public int Id { get; set; }

    public string? Reviewer { get; set; }

    public float? Star { get; set; }

    public string? Comment { get; set; }

    public int? ProductId { get; set; }

    public virtual Product? Product { get; set; }
}
