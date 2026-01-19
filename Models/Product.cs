using System;
using System.Collections.Generic;

namespace Bookstore.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal? Price { get; set; }

    public string? ImageUrl { get; set; }

    public int? CategoriesId { get; set; }

    public string? Status { get; set; }

    public string? MaSp { get; set; }

    public int? Discount { get; set; }

    public int? BrandId { get; set; }

    public float? SoSaoDanhGia { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual Category? Categories { get; set; }

    public virtual ICollection<Orderdetail> Orderdetails { get; set; } = new List<Orderdetail>();

    public virtual Productdetail? Productdetail { get; set; }

    public virtual ICollection<Productvariant> Productvariants { get; set; } = new List<Productvariant>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Shoppingcart> Shoppingcarts { get; set; } = new List<Shoppingcart>();
}
