using DTO;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Linq;

namespace HandicraftShop_Prodject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesApiController : ControllerBase
    {
        private readonly IArticleService _articleService;

        public ArticlesApiController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        // GET: api/ArticlesApi
        [HttpGet]
        public IActionResult GetArticles(string? search = null, string? category = null, int page = 1, int pageSize = 6)
        {
            var articles = _articleService.GetArticles()
                .Where(a => a.Status == "Published")
                .AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(search))
            {
                articles = articles.Where(a =>
                    (a.Title != null && a.Title.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (a.Content != null && a.Content.Contains(search, StringComparison.OrdinalIgnoreCase))
                );
            }

            // Filter by category
            if (!string.IsNullOrEmpty(category))
            {
                articles = articles.Where(a => a.Category == category);
            }

            // Order by created date
            articles = articles.OrderByDescending(a => a.CreatedAt);

            // Pagination
            var totalItems = articles.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var pagedArticles = articles.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Get categories
            var categories = _articleService.GetArticles()
                .Where(a => a.Status == "Published" && !string.IsNullOrEmpty(a.Category))
                .Select(a => a.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            return Ok(new
            {
                Articles = pagedArticles,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize,
                Categories = categories
            });
        }

        // GET: api/ArticlesApi/5
        [HttpGet("{id}")]
        public IActionResult GetArticle(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid article ID" });

            var article = _articleService.GetArticleDTOById(id);
            
            if (article == null)
                return NotFound(new { message = "Article not found" });

            // Only return published articles
            if (article.Status != "Published")
                return NotFound(new { message = "Article not found" });

            return Ok(article);
        }

        // GET: api/ArticlesApi/categories
        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            var categories = _articleService.GetArticles()
                .Where(a => a.Status == "Published" && !string.IsNullOrEmpty(a.Category))
                .Select(a => a.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            return Ok(categories);
        }
    }
}

