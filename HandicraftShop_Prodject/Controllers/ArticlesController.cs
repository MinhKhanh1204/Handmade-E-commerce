using DTO;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Linq;

namespace HandicraftShop_Prodject.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly IArticleService _articleService;

        public ArticlesController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        // GET: Articles
        public IActionResult Index(string? search, string? category, int page = 1, int pageSize = 6)
        {
            var articles = _articleService.GetArticles()
                .Where(a => a.Status == "Published") // Only show published articles
                .AsQueryable();

            // Search by title or content
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

            // Get distinct categories for filter
            var categories = _articleService.GetArticles()
                .Where(a => a.Status == "Published" && !string.IsNullOrEmpty(a.Category))
                .Select(a => a.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            // Order by created date (newest first)
            articles = articles.OrderByDescending(a => a.CreatedAt);

            // Pagination
            var totalItems = articles.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var pagedArticles = articles.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Search = search;
            ViewBag.Category = category;
            ViewBag.Categories = categories;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = pageSize;

            // Return partial view for AJAX requests
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ArticleListPartial", pagedArticles);
            }

            return View(pagedArticles);
        }

        // GET: Articles/Details/5
        public IActionResult Details(int id)
        {
            if (id <= 0)
                return BadRequest();

            var article = _articleService.GetArticleDTOById(id);
            
            if (article == null)
                return NotFound();

            // Only show published articles to customers
            if (article.Status != "Published")
                return NotFound();

            return View(article);
        }
    }
}

