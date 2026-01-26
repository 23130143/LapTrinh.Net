using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Bookstore.Models;

public partial class QuanlybansachContext : DbContext
{
    public QuanlybansachContext()
    {
    }

    public QuanlybansachContext(DbContextOptions<QuanlybansachContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Categorytype> Categorytypes { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Deliveryinformation> Deliveryinformations { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Orderdetail> Orderdetails { get; set; }

    public virtual DbSet<Parameterdescription> Parameterdescriptions { get; set; }

    public virtual DbSet<Partvarient> Partvarients { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Productdetail> Productdetails { get; set; }

    public virtual DbSet<Productvariant> Productvariants { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Shippingfee> Shippingfees { get; set; }

    public virtual DbSet<Shoppingcart> Shoppingcarts { get; set; }

    public virtual DbSet<Specification> Specifications { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=quanlybansach;user=root;password=123456", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.44-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("brands");

            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PRIMARY");

            entity.ToTable("categories");

            entity.HasIndex(e => e.CategoryTypeId, "CategoryTypeId");

            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.CategoryType).WithMany(p => p.Categories)
                .HasForeignKey(d => d.CategoryTypeId)
                .HasConstraintName("categories_ibfk_1");
        });

        modelBuilder.Entity<Categorytype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("categorytypes");

            entity.Property(e => e.Image)
                .HasMaxLength(500)
                .HasColumnName("image");
            entity.Property(e => e.NameType).HasMaxLength(255);
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("contacts");

            entity.Property(e => e.BusinessId).HasMaxLength(50);
            entity.Property(e => e.BusinessName).HasMaxLength(255);
            entity.Property(e => e.TaxCode).HasMaxLength(50);
        });

        modelBuilder.Entity<Deliveryinformation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("deliveryinformations");

            entity.HasIndex(e => e.UserId, "UserID");

            entity.Property(e => e.Adress).HasColumnType("text");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Deliveryinformations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("deliveryinformations_ibfk_1");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("discounts");

            entity.HasIndex(e => e.CodeDiscount, "codeDiscount").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CodeDiscount)
                .HasMaxLength(50)
                .HasColumnName("codeDiscount");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Describe).HasColumnType("text");
            entity.Property(e => e.Discount1).HasColumnName("Discount");
            entity.Property(e => e.Expired)
                .HasColumnType("datetime")
                .HasColumnName("expired");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("image");

            entity.Property(e => e.ImgBase64).HasColumnName("imgBase64");
            entity.Property(e => e.ImgString).HasMaxLength(500);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("orders");

            entity.HasIndex(e => e.DeliveryInformationId, "DeliveryInformationId");

            entity.HasIndex(e => e.ShippingFeeId, "ShippingFeeId");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.DiscountId).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.TotalAmount).HasPrecision(15, 2);

            entity.HasOne(d => d.DeliveryInformation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.DeliveryInformationId)
                .HasConstraintName("orders_ibfk_1");

            entity.HasOne(d => d.ShippingFee).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ShippingFeeId)
                .HasConstraintName("orders_ibfk_2");
        });

        modelBuilder.Entity<Orderdetail>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.ProductId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("orderdetails");

            entity.HasIndex(e => e.ProductId, "ProductId");

            entity.HasOne(d => d.Order).WithMany(p => p.Orderdetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orderdetails_ibfk_1");

            entity.HasOne(d => d.Product).WithMany(p => p.Orderdetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orderdetails_ibfk_2");
        });

        modelBuilder.Entity<Parameterdescription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("parameterdescriptions");

            entity.HasIndex(e => e.ThongSoId, "ThongSoID");

            entity.Property(e => e.Describe).HasColumnType("text");
            entity.Property(e => e.ThongSoId).HasColumnName("ThongSoID");

            entity.HasOne(d => d.ThongSo).WithMany(p => p.Parameterdescriptions)
                .HasForeignKey(d => d.ThongSoId)
                .HasConstraintName("parameterdescriptions_ibfk_1");
        });

        modelBuilder.Entity<Partvarient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("partvarients");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("products");

            entity.HasIndex(e => e.BrandId, "BrandId");

            entity.HasIndex(e => e.CategoriesId, "CategoriesID");

            entity.Property(e => e.CategoriesId).HasColumnName("CategoriesID");
            entity.Property(e => e.Discount).HasColumnName("%Discount");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.MaSp)
                .HasMaxLength(50)
                .HasColumnName("MaSP");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Price).HasPrecision(15, 2);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("products_ibfk_2");

            entity.HasOne(d => d.Categories).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoriesId)
                .HasConstraintName("products_ibfk_1");
        });

        modelBuilder.Entity<Productdetail>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PRIMARY");

            entity.ToTable("productdetails");

            entity.HasIndex(e => e.Specification, "Specification");

            entity.Property(e => e.ProductId)
                .ValueGeneratedNever()
                .HasColumnName("ProductID");
            entity.Property(e => e.MotaDai).HasColumnName("motaDai");
            entity.Property(e => e.ShortDescription).HasColumnType("text");

            entity.HasOne(d => d.Product).WithOne(p => p.Productdetail)
                .HasForeignKey<Productdetail>(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("productdetails_ibfk_1");

            entity.HasOne(d => d.SpecificationNavigation).WithMany(p => p.Productdetails)
                .HasForeignKey(d => d.Specification)
                .HasConstraintName("productdetails_ibfk_2");
        });

        modelBuilder.Entity<Productvariant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("productvariants");

            entity.HasIndex(e => e.ImageId, "ImageId");

            entity.HasIndex(e => e.PartVarients, "PartVarients");

            entity.HasIndex(e => e.ProductId, "ProductId");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.HasOne(d => d.Image).WithMany(p => p.Productvariants)
                .HasForeignKey(d => d.ImageId)
                .HasConstraintName("productvariants_ibfk_1");

            entity.HasOne(d => d.PartVarientsNavigation).WithMany(p => p.Productvariants)
                .HasForeignKey(d => d.PartVarients)
                .HasConstraintName("productvariants_ibfk_3");

            entity.HasOne(d => d.Product).WithMany(p => p.Productvariants)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("productvariants_ibfk_2");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("reviews");

            entity.HasIndex(e => e.ProductId, "ProductId");

            entity.Property(e => e.Comment).HasColumnType("text");
            entity.Property(e => e.Reviewer).HasMaxLength(255);

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("reviews_ibfk_1");
        });

        modelBuilder.Entity<Shippingfee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("shippingfees");

            entity.Property(e => e.GiaTien).HasPrecision(15, 2);
        });

        modelBuilder.Entity<Shoppingcart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("shoppingcarts");

            entity.HasIndex(e => e.ProductId, "ProductID");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Product).WithMany(p => p.Shoppingcarts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("shoppingcarts_ibfk_1");
        });

        modelBuilder.Entity<Specification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("specifications");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user");

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Role).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
