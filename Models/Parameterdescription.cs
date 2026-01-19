using System;
using System.Collections.Generic;

namespace Bookstore.Models;

public partial class Parameterdescription
{
    public int Id { get; set; }

    public string? Describe { get; set; }

    public int? ThongSoId { get; set; }

    public virtual Specification? ThongSo { get; set; }
}
