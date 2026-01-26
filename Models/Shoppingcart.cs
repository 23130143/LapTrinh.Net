using System;
using System.Collections.Generic;

namespace Bookstore.Models;

public partial class Shoppingcart
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    // Thuộc tính tính toán (Read-only)
    public decimal Total => (Quantity ?? 0) * UnitPrice;

    public virtual Product? Product { get; set; }

    // Nếu hệ thống có đăng nhập, nên có UserId để lưu giỏ hàng vào Database lâu dài
    public string? UserId { get; set; }
}


