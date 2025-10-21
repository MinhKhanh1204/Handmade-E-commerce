using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using BussinessObject;

namespace BussinessAccessObject;

public partial class MyStoreContext : DbContext
{
    public MyStoreContext()
    {
    }

    public MyStoreContext(DbContextOptions<MyStoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<ArticleImage> ArticleImages { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<FeedbackImage> FeedbackImages { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Staff> Staffs { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=HandicraftShop;User Id=sa;Password=123456;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__ACCOUNTS__349DA5860747B0D6");

            entity.ToTable("ACCOUNTS");

            entity.HasIndex(e => e.Username, "UQ__ACCOUNTS__536C85E4A297DFCB").IsUnique();

            entity.Property(e => e.AccountId)
                .HasMaxLength(20)
                .HasColumnName("AccountID");
            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PasswordRecoveryToken).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("PK__ARTICLES__9C6270C8C55D8378");

            entity.ToTable("ARTICLES");

            entity.Property(e => e.ArticleId).HasColumnName("ArticleID");
            entity.Property(e => e.AuthorId)
                .HasMaxLength(20)
                .HasColumnName("AuthorID");
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Draft");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Author).WithMany(p => p.Articles)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK__ARTICLES__Author__04E4BC85");
        });

        modelBuilder.Entity<ArticleImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__ARTICLE___7516F4EC46D783E8");

            entity.ToTable("ARTICLE_IMAGES");

            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.ArticleId).HasColumnName("ArticleID");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("ImageURL");
            entity.Property(e => e.IsMain).HasDefaultValue(false);

            entity.HasOne(d => d.Article).WithMany(p => p.ArticleImages)
                .HasForeignKey(d => d.ArticleId)
                .HasConstraintName("FK__ARTICLE_I__Artic__08B54D69");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__CART__51BCD7970F7DB3DC");

            entity.ToTable("CART");

            entity.Property(e => e.CartId).HasColumnName("CartID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(20)
                .HasColumnName("CustomerID");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Customer).WithMany(p => p.Carts)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__CART__CustomerID__5DCAEF64");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("PK__CART_ITE__488B0B2A99071EE6");

            entity.ToTable("CART_ITEMS");

            entity.Property(e => e.CartItemId).HasColumnName("CartItemID");
            entity.Property(e => e.CartId).HasColumnName("CartID");
            entity.Property(e => e.ProductId)
                .HasMaxLength(20)
                .HasColumnName("ProductID");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .HasConstraintName("FK__CART_ITEM__CartI__60A75C0F");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__CART_ITEM__Produ__619B8048");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__CATEGORI__19093A2B2318EE2D");

            entity.ToTable("CATEGORIES");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__CUSTOMER__A4AE64B8E60C3E9E");

            entity.ToTable("CUSTOMERS");

            entity.Property(e => e.CustomerId)
                .HasMaxLength(20)
                .HasColumnName("CustomerID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.CustomerNavigation).WithOne(p => p.Customer)
                .HasForeignKey<Customer>(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CUSTOMERS__Custo__44FF419A");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__FEEDBACK__6A4BEDF6085CC051");

            entity.ToTable("FEEDBACKS");

            entity.Property(e => e.FeedbackId).HasColumnName("FeedbackID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(20)
                .HasColumnName("CustomerID");
            entity.Property(e => e.ProductId)
                .HasMaxLength(20)
                .HasColumnName("ProductID");

            entity.HasOne(d => d.Customer).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__FEEDBACKS__Custo__76969D2E");

            entity.HasOne(d => d.Product).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__FEEDBACKS__Produ__778AC167");
        });

        modelBuilder.Entity<FeedbackImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__FEEDBACK__7516F4EC1FB693C0");

            entity.ToTable("FEEDBACK_IMAGES");

            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.FeedbackId).HasColumnName("FeedbackID");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("ImageURL");

            entity.HasOne(d => d.Feedback).WithMany(p => p.FeedbackImages)
                .HasForeignKey(d => d.FeedbackId)
                .HasConstraintName("FK__FEEDBACK___Feedb__7A672E12");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__ORDERS__C3905BAF8E1637B7");

            entity.ToTable("ORDERS");

            entity.Property(e => e.OrderId)
                .HasMaxLength(20)
                .HasColumnName("OrderID");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(20)
                .HasColumnName("CustomerID");
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.OrderDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.ShippingAddress).HasMaxLength(255);
            entity.Property(e => e.ShippingStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.StaffId)
                .HasMaxLength(20)
                .HasColumnName("StaffID");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.VoucherId).HasColumnName("VoucherID");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__ORDERS__Customer__6D0D32F4");

            entity.HasOne(d => d.Staff).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StaffId)
                .HasConstraintName("FK__ORDERS__StaffID__6E01572D");

            entity.HasOne(d => d.Voucher).WithMany(p => p.Orders)
                .HasForeignKey(d => d.VoucherId)
                .HasConstraintName("FK__ORDERS__VoucherI__6EF57B66");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__ORDER_IT__57ED06A1DB1BBA1C");

            entity.ToTable("ORDER_ITEMS");

            entity.Property(e => e.OrderItemId).HasColumnName("OrderItemID");
            entity.Property(e => e.Discount).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.OrderId)
                .HasMaxLength(20)
                .HasColumnName("OrderID");
            entity.Property(e => e.ProductId)
                .HasMaxLength(20)
                .HasColumnName("ProductID");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__ORDER_ITE__Order__71D1E811");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__ORDER_ITE__Produ__72C60C4A");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__PRODUCTS__B40CC6ED7E6CC9D6");

            entity.ToTable("PRODUCTS");

            entity.Property(e => e.ProductId)
                .HasMaxLength(20)
                .HasColumnName("ProductID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Discount).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Material).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductName).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_PRODUCTS_Catego_4E88ABD4");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__PRODUCT___7516F4EC70486543");

            entity.ToTable("PRODUCT_IMAGES");

            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("ImageURL");
            entity.Property(e => e.IsMain).HasDefaultValue(false);
            entity.Property(e => e.ProductId)
                .HasMaxLength(20)
                .HasColumnName("ProductID");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__PRODUCT_I__Produ__52593CB8");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__ROLES__8AFACE3A999A648E");

            entity.ToTable("ROLES");

            entity.HasIndex(e => e.RoleName, "UQ__ROLES__8A2B61608736395D").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK__STAFFS__96D4AAF785C91E7A");

            entity.ToTable("STAFFS");

            entity.Property(e => e.StaffId)
                .HasMaxLength(20)
                .HasColumnName("StaffID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CitizenId)
                .HasMaxLength(20)
                .HasColumnName("CitizenID");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.StaffNavigation).WithOne(p => p.Staff)
                .HasForeignKey<Staff>(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__STAFFS__StaffID__48CFD27E");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.VoucherId).HasName("PK__VOUCHERS__3AEE79C1339EAE59");

            entity.ToTable("VOUCHERS");

            entity.HasIndex(e => e.VoucherName, "UQ__VOUCHERS__91A6F629DC6CDB76").IsUnique();

            entity.Property(e => e.VoucherId).HasColumnName("VoucherID");
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MaxReducing).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MinOrderValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UsageCount).HasDefaultValue(0);
            entity.Property(e => e.VoucherName).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
