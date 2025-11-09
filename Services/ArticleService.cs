using BussinessObject;
using DTO;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;

        public ArticleService(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public Article? GetArticleById(int articleId)
        {
            return _articleRepository.GetArticleById(articleId);
        }

        public List<ArticleDTO> GetArticles()
        {
            var articles = _articleRepository.GetAllArticles()
                .Where(a => a.Status != "Deleted")
                .OrderByDescending(a => a.CreatedAt)
                .ToList();

            return articles.Select(a => new ArticleDTO
            {
                ArticleId = a.ArticleId,
                AuthorId = a.AuthorId,
                Title = a.Title,
                Content = a.Content,
                Category = a.Category,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt,
                Status = a.Status,
                AuthorName = a.Author?.FullName,
                ArticleImages = a.ArticleImages?.Select(img => new ArticleImageDTO
                {
                    ImageId = img.ImageId,
                    ArticleId = img.ArticleId,
                    ImageUrl = img.ImageUrl,
                    IsMain = img.IsMain
                }).ToList()
            }).ToList();
        }

        public List<ArticleDTO> GetArticlesByStatus(string? status)
        {
            var query = _articleRepository.GetAllArticles()
                .Where(a => a.Status != "Deleted");

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(a => a.Status == status);
            }

            var articles = query.OrderByDescending(a => a.CreatedAt).ToList();

            return articles.Select(a => new ArticleDTO
            {
                ArticleId = a.ArticleId,
                AuthorId = a.AuthorId,
                Title = a.Title,
                Content = a.Content,
                Category = a.Category,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt,
                Status = a.Status,
                AuthorName = a.Author?.FullName,
                ArticleImages = a.ArticleImages?.Select(img => new ArticleImageDTO
                {
                    ImageId = img.ImageId,
                    ArticleId = img.ArticleId,
                    ImageUrl = img.ImageUrl,
                    IsMain = img.IsMain
                }).ToList()
            }).ToList();
        }

        public ArticleDTO? GetArticleDTOById(int articleId)
        {
            var article = _articleRepository.GetArticleById(articleId);
            if (article == null) return null;

            return new ArticleDTO
            {
                ArticleId = article.ArticleId,
                AuthorId = article.AuthorId,
                Title = article.Title,
                Content = article.Content,
                Category = article.Category,
                CreatedAt = article.CreatedAt,
                UpdatedAt = article.UpdatedAt,
                Status = article.Status,
                AuthorName = article.Author?.FullName,
                ArticleImages = article.ArticleImages?.Select(img => new ArticleImageDTO
                {
                    ImageId = img.ImageId,
                    ArticleId = img.ArticleId,
                    ImageUrl = img.ImageUrl,
                    IsMain = img.IsMain
                }).ToList()
            };
        }

        public void SaveArticle(Article article)
        {
            article.CreatedAt = DateTime.Now;
            article.UpdatedAt = DateTime.Now;
            if (string.IsNullOrEmpty(article.Status))
            {
                article.Status = "Draft";
            }
            _articleRepository.SaveArticle(article);
        }

        public void UpdateArticle(Article article)
        {
            article.UpdatedAt = DateTime.Now;
            _articleRepository.UpdateArticle(article);
        }

        public void DeleteArticle(int articleId)
        {
            _articleRepository.DeleteArticle(articleId);
        }
    }
}

