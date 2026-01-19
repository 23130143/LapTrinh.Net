using System;
using System.Collections.Generic;

namespace Bookstore.Models;

public partial class Order
{
    public int Id { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? Status { get; set; }

    public string? DiscountId { get; set; }

    public int? DeliveryInformationId { get; set; }

    public decimal? TotalAmount { get; set; }

    public int? ShippingFeeId { get; set; }

    public virtual Deliveryinformation? DeliveryInformation { get; set; }

    public virtual ICollection<Orderdetail> Orderdetails { get; set; } = new List<Orderdetail>();

    public virtual Shippingfee? ShippingFee { get; set; }
}
