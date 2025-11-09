using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BussinessObject;

namespace DTO
{
    public class ArticleDTO
    {
        public int ArticleId { get; set; }

        public string? AuthorId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        public string? Content { get; set; }

        [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters.")]
        public string? Category { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string? Status { get; set; }

        // Navigation properties
        public string? AuthorName { get; set; }
        public List<ArticleImageDTO>? ArticleImages { get; set; }
    }

    public class ArticleImageDTO
    {
        public int ImageId { get; set; }
        public int? ArticleId { get; set; }
        public string? ImageUrl { get; set; }
        public bool? IsMain { get; set; }
    }
}

