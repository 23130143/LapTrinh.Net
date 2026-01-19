using System;
using System.Collections.Generic;

namespace Bookstore.Models;

public partial class Categorytype
{
    public int Id { get; set; }

    public string? NameType { get; set; }

    public string? Image { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
