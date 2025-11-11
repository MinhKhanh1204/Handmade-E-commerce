using BussinessObject;
using System.Collections.Generic;
using System.Linq;

namespace Repositories
{
    public interface IArticleRepository
    {
        IQueryable<Article> GetAllArticles();
        Article? GetArticleById(int articleId);
        List<Article> GetArticles();
        void SaveArticle(Article article);
        void UpdateArticle(Article article);
        void DeleteArticle(int articleId);
    }
}

