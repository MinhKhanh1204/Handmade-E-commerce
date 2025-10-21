using System;
using System.Collections.Generic;

namespace BussinessObject;

public partial class Article
{
    public int ArticleId { get; set; }

    public string? AuthorId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public string? Category { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<ArticleImage> ArticleImages { get; set; } = new List<ArticleImage>();

    public virtual Staff? Author { get; set; }
}
