using BussinessObject;
using DataAccessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly MyStoreContext _context;

        public ArticleRepository(MyStoreContext context)
        {
            _context = context;
        }

        public IQueryable<Article> GetAllArticles()
        {
            return _context.Articles
                .Include(a => a.ArticleImages)
                .Include(a => a.Author)
                    .ThenInclude(s => s.StaffNavigation);
        }

        public Article? GetArticleById(int articleId)
        {
            return _context.Articles
                .Include(a => a.ArticleImages)
                .Include(a => a.Author)
                    .ThenInclude(s => s.StaffNavigation)
                .FirstOrDefault(a => a.ArticleId == articleId);
        }

        public List<Article> GetArticles()
        {
            try
            {
                using var db = new MyStoreContext();
                return db.Articles
                         .Include(a => a.Author)
                         .Include(a => a.ArticleImages)
                         .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void SaveArticle(Article article)
        {
            try
            {
                using var db = new MyStoreContext();

                // Store images separately IMMEDIATELY to avoid collection modification issues
                // Copy image data to a new list before any EF operations
                var imagesToAdd = new List<ArticleImage>();
                if (article.ArticleImages != null)
                {
                    foreach (var img in article.ArticleImages)
                    {
                        imagesToAdd.Add(new ArticleImage
                        {
                            ImageUrl = img.ImageUrl,
                            IsMain = img.IsMain
                        });
                    }
                }

                // Create new article without images to avoid tracking issues
                var newArticle = new Article
                {
                    AuthorId = article.AuthorId,
                    Title = article.Title,
                    Content = article.Content,
                    Category = article.Category,
                    Status = article.Status,
                    CreatedAt = article.CreatedAt,
                    UpdatedAt = article.UpdatedAt
                };

                // Save article first to get ArticleId
                db.Articles.Add(newArticle);
                db.SaveChanges();

                // Add article images after ArticleId is generated
                if (imagesToAdd.Count > 0)
                {
                    foreach (var img in imagesToAdd)
                    {
                        var newImage = new ArticleImage
                        {
                            ArticleId = newArticle.ArticleId,
                            ImageUrl = img.ImageUrl,
                            IsMain = img.IsMain
                        };
                        db.ArticleImages.Add(newImage);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateArticle(Article article)
        {
            try
            {
                using var db = new MyStoreContext();

                var existing = db.Articles
                                 .Include(a => a.ArticleImages)
                                 .FirstOrDefault(a => a.ArticleId == article.ArticleId);

                if (existing == null) throw new Exception($"Article '{article.ArticleId}' not found.");

                // Map scalar fields
                existing.Title = article.Title;
                existing.Content = article.Content;
                existing.Category = article.Category;
                existing.Status = article.Status;
                existing.UpdatedAt = DateTime.Now;

                // Replace images only when new ones provided
                if (article.ArticleImages != null && article.ArticleImages.Count > 0)
                {
                    // Get existing images as list first to avoid collection modification issues
                    var existingImagesList = existing.ArticleImages?.ToList() ?? new List<ArticleImage>();
                    
                    if (existingImagesList.Count > 0)
                    {
                        db.ArticleImages.RemoveRange(existingImagesList);
                    }

                    foreach (var img in article.ArticleImages)
                    {
                        if (!img.ArticleId.HasValue)
                            img.ArticleId = existing.ArticleId;

                        db.ArticleImages.Add(new ArticleImage
                        {
                            ArticleId = img.ArticleId,
                            ImageUrl = img.ImageUrl,
                            IsMain = img.IsMain
                        });
                    }
                }

                db.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Soft-delete: set Status = "Deleted" (do not remove DB row)
        public void DeleteArticle(int articleId)
        {
            try
            {
                using var db = new MyStoreContext();
                var article = db.Articles
                          .FirstOrDefault(a => a.ArticleId == articleId);

                if (article != null)
                {
                    article.Status = "Deleted";
                    article.UpdatedAt = DateTime.Now;
                    db.Articles.Update(article);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

