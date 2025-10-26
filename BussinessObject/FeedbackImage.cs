using System;
using System.Collections.Generic;

namespace BussinessObject;

public partial class FeedbackImage
{
    public int ImageId { get; set; }

    public int? FeedbackId { get; set; }

    public string? ImageUrl { get; set; }

    public virtual Feedback? Feedback { get; set; }
}
