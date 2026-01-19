using System;
using System.Collections.Generic;

namespace Bookstore.Models;

public partial class Image
{
    public int Id { get; set; }

    public string? ImgString { get; set; }

    public string? ImgBase64 { get; set; }

    public virtual ICollection<Productvariant> Productvariants { get; set; } = new List<Productvariant>();
}
