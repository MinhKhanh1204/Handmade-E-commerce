﻿using System;
using System.Collections.Generic;

namespace BussinessObject;

public partial class Voucher
{
    public int VoucherId { get; set; }

    public string? VoucherName { get; set; }

    public string? Code { get; set; }

    public string? Description { get; set; }

    public decimal? DiscountPercentage { get; set; }

    public decimal? MaxReducing { get; set; }

    public int? Quantity { get; set; }

    public int? UsageCount { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public bool? IsActive { get; set; }

    public decimal? MinOrderValue { get; set; }

    public int? MaxUsagePerUser { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
