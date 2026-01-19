using System;
using System.Collections.Generic;

namespace Bookstore.Models;

public partial class Productvariant
{
    public int Id { get; set; }

    public int? ImageId { get; set; }

    public int? PartVarients { get; set; }

    public int? StockQuantity { get; set; }

    public int? ProductId { get; set; }

    public bool? IsDefault { get; set; }

    public virtual Image? Image { get; set; }

    public virtual Partvarient? PartVarientsNavigation { get; set; }

    public virtual Product? Product { get; set; }
}
