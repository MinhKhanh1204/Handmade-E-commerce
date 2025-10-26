using BussinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositories
{
    public class ProductRepository : IProductRepository
    {
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

        public Product? GetProductById(string id)
        {
            try
            {
                using var db = new MyStoreContext();
                return db.Products
                         .Include(p => p.Category)
                         .Include(p => p.ProductImages)
                         .FirstOrDefault(p => p.ProductId == id);
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

        public void DeleteProduct(string productId)
        {
            try
            {
                using var db = new MyStoreContext();
                var p = db.Products
                          .Include(p => p.ProductImages)
                          .FirstOrDefault(p => p.ProductId == productId);

                if (p != null)
                {
                    db.ProductImages.RemoveRange(p.ProductImages);
                    db.Products.Remove(p);
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