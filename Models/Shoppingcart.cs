using System;
using System.Collections.Generic;

namespace Bookstore.Models;

public partial class Shoppingcart
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public virtual Product? Product { get; set; }
}
