using System;
using System.Collections.Generic;

namespace Bookstore.Models;

public partial class Discount
{
    public int Id { get; set; }

    public int? Type { get; set; }

    public float? Discount1 { get; set; }

    public float? MaxDiscount { get; set; }

    public string? CodeDiscount { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? Expired { get; set; }

    public string? Describe { get; set; }

    public float? MaxApply { get; set; }

    public string? Name { get; set; }
}
