using System;
using System.Collections.Generic;

namespace Bookstore.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public int? CategoryTypeId { get; set; }

    public int? Level { get; set; }

    public virtual Categorytype? CategoryType { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
