using BussinessObject;
using DTO;
using System.Collections.Generic;

namespace Services
{
    public interface IArticleService
    {
        Article? GetArticleById(int articleId);
        List<ArticleDTO> GetArticles();
        List<ArticleDTO> GetArticlesByStatus(string? status);
        void SaveArticle(Article article);
        void UpdateArticle(Article article);
        void DeleteArticle(int articleId);
        ArticleDTO? GetArticleDTOById(int articleId);
    }
}

