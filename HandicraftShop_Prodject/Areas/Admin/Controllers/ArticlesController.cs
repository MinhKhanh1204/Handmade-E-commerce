using BussinessObject;
using DTO;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;

namespace HandicraftShop_Prodject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArticlesController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ArticlesController(IArticleService articleService, IWebHostEnvironment webHostEnvironment)
        {
            _articleService = articleService;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(string searchString)
        {
            var articles = _articleService.GetArticles().AsQueryable();

            // Search theo Title, Category hoặc Author
            if (!string.IsNullOrEmpty(searchString))
            {
                articles = articles.Where(a =>
                    (a.Title != null && a.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                    (a.Category != null && a.Category.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                    (a.AuthorName != null && a.AuthorName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                );
            }

            ViewBag.SearchString = searchString;

            return View(articles.ToList());
        }

        public IActionResult Create()
        {
            var articleDto = new ArticleDTO
            {
                AuthorId = HttpContext.Session.GetString("StaffId") ?? HttpContext.Session.GetString("AccountId"),
                Status = "Draft"
            };
            return View(articleDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleDTO articleDto, List<IFormFile>? imageFiles, List<bool>? isMainFlags)
        {
            try
            {
                // Get AuthorId from session or DTO
                var authorId = articleDto.AuthorId ?? HttpContext.Session.GetString("StaffId") ?? HttpContext.Session.GetString("AccountId");
                
                // If AuthorId is still null, set a default or return error
                if (string.IsNullOrEmpty(authorId))
                {
                    ModelState.AddModelError("", "Author ID is required. Please login first.");
                    articleDto.AuthorId = null;
                    articleDto.Status = "Draft";
                    return View(articleDto);
                }

                if (!ModelState.IsValid)
                {
                    articleDto.AuthorId = authorId;
                    return View(articleDto);
                }

                // Get files from Request.Form.Files - same approach as Product controller
                var files = (imageFiles != null && imageFiles.Count > 0)
                            ? imageFiles
                            : Request.Form.Files.Where(f => f.Name.StartsWith("imageFiles")).ToList();

                var article = new Article
                {
                    AuthorId = authorId,
                    Title = articleDto.Title,
                    Content = articleDto.Content,
                    Category = articleDto.Category,
                    Status = articleDto.Status ?? "Draft"
                };

                // Upload images if provided - using same approach as Product controller
                article.ArticleImages = new List<ArticleImage>();
                
                if (files != null && files.Count > 0)
                {
                    // Use same path structure as Product: wwwroot/uploads/articles
                    var uploadsRoot = Path.Combine(_webHostEnvironment.WebRootPath ?? "wwwroot", "uploads", "articles");
                    Directory.CreateDirectory(uploadsRoot);

                    for (int i = 0; i < files.Count; i++)
                    {
                        var file = files[i];
                        if (file != null && file.Length > 0)
                        {
                            var ext = Path.GetExtension(file.FileName);
                            var fileName = $"{Guid.NewGuid():N}{ext}";
                            var filePath = Path.Combine(uploadsRoot, fileName);

                            // Use System.IO.File.Create() like Product controller does
                            using (var stream = System.IO.File.Create(filePath))
                            {
                                await file.CopyToAsync(stream);
                            }

                            // Create relative URL like Product: /uploads/articles/{fileName}
                            var relativeUrl = $"/uploads/articles/{fileName}";

                            article.ArticleImages.Add(new ArticleImage
                            {
                                ImageUrl = relativeUrl,
                                IsMain = isMainFlags != null && i < isMainFlags.Count && isMainFlags[i]
                            });
                        }
                    }
                }

                // Only save article if there are no critical errors
                if (ModelState.IsValid || article.ArticleImages.Count > 0)
                {
                    _articleService.SaveArticle(article);
                    if (article.ArticleImages.Count > 0)
                    {
                        TempData["SuccessMessage"] = $"Article created successfully with {article.ArticleImages.Count} image(s)!";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Article created successfully, but no images were uploaded.";
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Return to view with errors
                    articleDto.AuthorId = authorId;
                    articleDto.Status = articleDto.Status ?? "Draft";
                    return View(articleDto);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                articleDto.AuthorId = HttpContext.Session.GetString("StaffId") ?? HttpContext.Session.GetString("AccountId");
                articleDto.Status = "Draft";
                return View(articleDto);
            }
        }

        public IActionResult Edit(int id)
        {
            if (id <= 0)
                return BadRequest();

            var articleDto = _articleService.GetArticleDTOById(id);
            if (articleDto == null)
                return NotFound();

            return View(articleDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ArticleDTO articleDto, List<IFormFile>? imageFiles, List<bool>? isMainFlags, List<string>? existingImageUrls)
        {
            if (!ModelState.IsValid)
                return View(articleDto);

            var article = _articleService.GetArticleById(articleDto.ArticleId);
            if (article == null)
                return NotFound();

            // Update scalar fields
            article.Title = articleDto.Title;
            article.Content = articleDto.Content;
            article.Category = articleDto.Category;
            article.Status = articleDto.Status;

            // Handle images: keep existing URLs and add new uploaded files
            article.ArticleImages = new List<ArticleImage>();
            
            // Keep existing images if provided
            if (existingImageUrls != null && existingImageUrls.Count > 0)
            {
                for (int i = 0; i < existingImageUrls.Count; i++)
                {
                    if (!string.IsNullOrEmpty(existingImageUrls[i]))
                    {
                        article.ArticleImages.Add(new ArticleImage
                        {
                            ImageUrl = existingImageUrls[i],
                            IsMain = isMainFlags != null && i < isMainFlags.Count && isMainFlags[i]
                        });
                    }
                }
            }

            // Upload new images if provided - using same approach as Product controller
            var newFiles = (imageFiles != null && imageFiles.Count > 0)
                          ? imageFiles
                          : Request.Form.Files.Where(f => f.Name.StartsWith("imageFiles")).ToList();
            
            if (newFiles != null && newFiles.Count > 0)
            {
                // Use same path structure as Product: wwwroot/uploads/articles
                var uploadsRoot = Path.Combine(_webHostEnvironment.WebRootPath ?? "wwwroot", "uploads", "articles");
                Directory.CreateDirectory(uploadsRoot);

                int startIndex = existingImageUrls != null ? existingImageUrls.Count : 0;
                
                for (int i = 0; i < newFiles.Count; i++)
                {
                    var file = newFiles[i];
                    if (file != null && file.Length > 0)
                    {
                        var ext = Path.GetExtension(file.FileName);
                        var fileName = $"{Guid.NewGuid():N}{ext}";
                        var filePath = Path.Combine(uploadsRoot, fileName);

                        // Use System.IO.File.Create() like Product controller does
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Create relative URL like Product: /uploads/articles/{fileName}
                        var relativeUrl = $"/uploads/articles/{fileName}";
                        
                        int flagIndex = startIndex + i;
                        article.ArticleImages.Add(new ArticleImage
                        {
                            ImageUrl = relativeUrl,
                            IsMain = isMainFlags != null && flagIndex < isMainFlags.Count && isMainFlags[flagIndex]
                        });
                    }
                }
            }

            _articleService.UpdateArticle(article);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            if (id <= 0)
                return BadRequest();

            var articleDto = _articleService.GetArticleDTOById(id);
            if (articleDto == null)
                return NotFound();

            return View(articleDto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _articleService.DeleteArticle(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            if (id <= 0)
                return BadRequest();

            var articleDto = _articleService.GetArticleDTOById(id);
            if (articleDto == null)
                return NotFound();

            return View(articleDto);
        }

        // Test action to check file upload capability
        [HttpPost]
        public async Task<IActionResult> TestUpload(IFormFile testFile)
        {
            try
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath ?? "wwwroot", "uploads", "articles");
                var fileReceived = testFile != null && testFile.Length > 0;
                string? savedPath = null;
                bool fileExists = false;
                long fileSize = 0;

                if (fileReceived)
                {
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var fileName = "test_" + Guid.NewGuid().ToString("N") + Path.GetExtension(testFile.FileName);
                    savedPath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = System.IO.File.Create(savedPath))
                    {
                        await testFile.CopyToAsync(stream);
                    }

                    fileExists = System.IO.File.Exists(savedPath);
                    if (fileExists)
                    {
                        fileSize = new FileInfo(savedPath).Length;
                    }
                }

                var result = new
                {
                    WebRootPath = _webHostEnvironment.WebRootPath,
                    TestFolder = uploadsFolder,
                    FileReceived = fileReceived,
                    FileName = testFile?.FileName,
                    FileLength = testFile?.Length ?? 0,
                    FileContentType = testFile?.ContentType,
                    SavedPath = savedPath,
                    FileExists = fileExists,
                    FileSize = fileSize
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }
    }
}


