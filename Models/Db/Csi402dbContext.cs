using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace WebApplication1.Models.Db;

public partial class Csi402dbContext : DbContext
{
    /*************  ✨ Windsurf Command ⭐  *************/
    /// <summary>
    /// A class that represents a database context and provides a
    /// convenient API for performing database operations.
    /// </summary>
    /*******  99bf6adf-0b70-4773-9fcf-5c81f0ae27b6  *******/
    public Csi402dbContext()
    {
    }

    public Csi402dbContext(DbContextOptions<Csi402dbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Labstudent> Labstudents { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductReview> ProductReviews { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=csi402db;user=root;password=Oom19898za", Microsoft.EntityFrameworkCore.ServerVersion.Parse("9.6.0-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_520_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Labstudent>(entity =>
        {
            entity.HasKey(e => e.StdId).HasName("PRIMARY");

            entity.ToTable("labstudent");

            entity.Property(e => e.StdId)
                .HasMaxLength(10)
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.StdKastname)
                .HasMaxLength(100)
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.StdName)
                .HasMaxLength(50)
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.StdPassword)
                .HasMaxLength(30)
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PRIMARY");
            entity.ToTable("orders");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.Property(e => e.ShippingAddress)
                .HasMaxLength(500) // ตรงตาม SQL
                .HasColumnName("shipping_address")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");

            entity.Property(e => e.Subtotal).HasPrecision(10, 2).HasColumnName("subtotal");
            entity.Property(e => e.ShippingCost).HasPrecision(10, 2).HasColumnName("shipping_cost");
            entity.Property(e => e.DiscountAmount).HasPrecision(10, 2).HasColumnName("discount_amount");
            entity.Property(e => e.NetAmount).HasPrecision(10, 2).HasColumnName("net_amount");

            entity.Property(e => e.TrackingStatus)
                .HasMaxLength(30)
                .HasColumnName("tracking_status")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");

            entity.Property(e => e.OrderDatetime)
                .HasColumnType("datetime")
                .HasColumnName("order_datetime");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PRIMARY");
            entity.ToTable("payments");

            entity.HasIndex(e => e.OrderId, "order_id").IsUnique();

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");

            entity.Property(e => e.Amount).HasPrecision(10, 2).HasColumnName("amount");

            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(200)
                .HasColumnName("payment_method")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");

            entity.Property(e => e.ReceiptImageUrl)
                .HasMaxLength(500)
                .HasColumnName("slip_image_url");

            entity.Property(e => e.PaymentStatus).HasColumnName("payment_status");
            entity.Property(e => e.Paymentdate).HasColumnName("paymentdate");

            entity.HasOne(d => d.Order)
                .WithOne(p => p.Payment)
                .HasForeignKey<Payment>(d => d.OrderId)
                .HasConstraintName("payments_ibfk_1");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PRIMARY");

            entity.ToTable("roles");

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("role_id");
            entity.Property(e => e.Permission)
                .HasMaxLength(30)
                .HasColumnName("permission")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.RoleName)
                .HasMaxLength(10)
                .HasColumnName("role_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.AddressMain)
                .HasMaxLength(50)
                .HasColumnName("address_main")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.Email)
                .HasMaxLength(20)
                .HasColumnName("email")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(30)
                .HasColumnName("password_hash")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .HasColumnName("phone_number")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Username)
                .HasMaxLength(30)
                .HasColumnName("username")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PRIMARY");

            entity.ToTable("products");

            entity.Property(e => e.ProductId)
                .ValueGeneratedOnAdd()
                .HasColumnName("product_id");

            entity.Property(e => e.ProductName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("product_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");

            entity.Property(e => e.ProductPrice)
                .HasPrecision(18, 2)
                .HasColumnName("product_price");

            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .HasColumnName("category_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");

            entity.Property(e => e.ImageUrl)
                .HasColumnName("imageURL")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");

            entity.Property(e => e.StockQuantity)
                .HasDefaultValueSql("'0'")
                .HasColumnName("stock_quantity");

            entity.Property(e => e.RatingScore)
                .HasPrecision(3, 1)
                .HasColumnName("RatingScore");

            entity.Property(e => e.ReviewCount)
                .HasColumnName("ReviewCount");
        });

        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PRIMARY");
            entity.ToTable("ProductReviews");

            entity.HasOne(d => d.Product)
                .WithMany() // ถ้าใน Product.cs มี ICollection ก็ใส่ WithMany(p => p.ProductReviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ProductReviews_Products");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PRIMARY");

            entity.ToTable("order_items");

            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");

            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.Property(e => e.UnitPrice)
                .HasPrecision(10, 2)
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Order)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("fk_order_items_orders");

            entity.HasOne(d => d.Product)
                .WithMany() // หรือ .WithMany(p => p.OrderItems) ถ้าใน Product.cs มี ICollection
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("fk_order_items_products");

        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.PromotionId).HasName("PRIMARY");
            entity.ToTable("promotions");

            entity.Property(e => e.PromotionId)
                  .HasColumnName("promotion_id");

            entity.Property(e => e.PromotionName)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");

            entity.Property(e => e.PromotionType)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("promotion_type")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");

            entity.Property(e => e.DiscountValue)
                .HasPrecision(10, 2)
                .HasColumnName("discount_value");

            entity.Property(e => e.IsPercentage)
                .HasColumnType("tinyint(1)")
                .HasColumnName("is_percentage");

            entity.Property(e => e.MinPurchaseAmount)
                .HasPrecision(10, 2)
                .HasColumnName("min_purchase_amount");

            entity.Property(e => e.TargetCategoryName)
                .HasMaxLength(50)
                .HasColumnName("target_category_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");

            entity.Property(e => e.BundleCategoryName)
                .HasMaxLength(50)
                .HasColumnName("bundle_category_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");

            entity.Property(e => e.IsActive)
                .HasColumnType("tinyint(1)")
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");

            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");

            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
