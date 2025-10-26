using System;
using System.Collections.Generic;

namespace BussinessObject;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int? CartId { get; set; }

    public string? ProductId { get; set; }

    public int? Quantity { get; set; }

    public virtual Cart? Cart { get; set; }

    public virtual Product? Product { get; set; }
}
