using Bookstore.Models;

public class CartViewModel
{
    public List<Shoppingcart> CartItems { get; set; } = new List<Shoppingcart>();

    // Tổng tiền của cả giỏ hàng
    public decimal GrandTotal => CartItems.Sum(x => x.Total);

    // Tổng số lượng mặt hàng
    public int TotalQuantity => CartItems.Sum(x => x.Quantity ?? 0);
}