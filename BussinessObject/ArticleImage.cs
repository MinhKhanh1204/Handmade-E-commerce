using System;
using System.Collections.Generic;

namespace BussinessObject;

public partial class ArticleImage
{
    public int ImageId { get; set; }

    public int? ArticleId { get; set; }

    public string? ImageUrl { get; set; }

    public bool? IsMain { get; set; }

    public virtual Article? Article { get; set; }
}
