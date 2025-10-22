using System;
using System.Collections.Generic;

namespace BussinessObject;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public string? CustomerId { get; set; }

    public string? ProductId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<FeedbackImage> FeedbackImages { get; set; } = new List<FeedbackImage>();

    public virtual Product? Product { get; set; }
}
