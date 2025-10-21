using System;
using System.Collections.Generic;

namespace BussinessObject;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public string? OrderId { get; set; }

    public string? ProductId { get; set; }

    public int? Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? Discount { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
