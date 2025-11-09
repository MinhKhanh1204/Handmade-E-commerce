using BussinessObject;
using DataAccessObject;
using DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly MyStoreContext _context;

        public ProductRepository(MyStoreContext context)
        {
            _context = context;
        }

        public IQueryable<Product> GetAllProducts()
        {
            return _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Category)
                .Include(p => p.Feedbacks)
                .Where(p => p.Status == "Active");
        }

        public Product? GetProductById(string productId)
        {
            return _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Category)
                .Include(p => p.Feedbacks)
                    .ThenInclude(f => f.Customer)          // include Customer
                    .ThenInclude(c => c.CustomerNavigation) // include Account
                .Include(p => p.Feedbacks)
                    .ThenInclude(f => f.FeedbackImages)    // include FeedbackImages
                .FirstOrDefault(p => p.ProductId == productId);
        }
        public List<Product> GetProducts()
        {
            try
            {
                using var db = new MyStoreContext();
                return db.Products
                         .Include(p => p.Category)
                         .Include(p => p.ProductImages)
                         .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void SaveProduct(Product product)
        {
            try
            {
                using var db = new MyStoreContext();

                // Add product
                db.Products.Add(product);

                // Add product images ensuring ProductId
                if (product.ProductImages != null)
                {
                    foreach (var img in product.ProductImages)
                    {
                        if (string.IsNullOrEmpty(img.ProductId))
                        {
                            img.ProductId = product.ProductId;
                        }
                        db.ProductImages.Add(img);
                    }
                }

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateProduct(Product product)
        {
            try
            {
                using var db = new MyStoreContext();

                var existing = db.Products
                                 .Include(p => p.ProductImages)
                                 .FirstOrDefault(p => p.ProductId == product.ProductId);

                if (existing == null) throw new Exception($"Product '{product.ProductId}' not found.");

                // Map scalar fields
                existing.ProductName = product.ProductName;
                existing.Description = product.Description;
                existing.Material = product.Material;
                existing.Price = product.Price;
                existing.Discount = product.Discount;
                existing.StockQuantity = product.StockQuantity;
                existing.Status = product.Status;
                existing.CategoryId = product.CategoryId;

                // Replace images only when new ones provided
                if (product.ProductImages != null && product.ProductImages.Count > 0)
                {
                    if (existing.ProductImages != null && existing.ProductImages.Count > 0)
                    {
                        db.ProductImages.RemoveRange(existing.ProductImages);
                        existing.ProductImages.Clear();
                    }

                    foreach (var img in product.ProductImages)
                    {
                        if (string.IsNullOrEmpty(img.ProductId))
                            img.ProductId = existing.ProductId;

                        db.ProductImages.Add(new ProductImage
                        {
                            ProductId = img.ProductId,
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

        // Soft-delete: set Status = "Inactive" (do not remove DB row or product files)
        public void DeleteProduct(string productId)
        {
            try
            {
                using var db = new MyStoreContext();
                var p = db.Products
                          .FirstOrDefault(p => p.ProductId == productId);

                if (p != null)
                {
                    p.Status = "Inactive";
                    db.Products.Update(p);
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
