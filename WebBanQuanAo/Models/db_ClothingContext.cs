using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace WebBanQuanAo.Models
{
    public partial class db_ClothingContext : DbContext
    {
        public db_ClothingContext()
        {
        }

        public db_ClothingContext(DbContextOptions<db_ClothingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Gender> Genders { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Size> Sizes { get; set; }
        public virtual DbSet<TransactStatus> TransactStatuses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=TRANVUHAO;Database=db_Clothing;Integrated Security=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.FullName).HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(100);

                entity.Property(e => e.Phone)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.Salt)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_Accounts_Roles");
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.Property(e => e.BrandId).HasColumnName("BrandID");

                entity.Property(e => e.BrandName).HasMaxLength(50);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CatId);

                entity.Property(e => e.CatId).HasColumnName("CatID");

                entity.Property(e => e.CatName).HasMaxLength(50);
            });

            modelBuilder.Entity<Gender>(entity =>
            {
                entity.Property(e => e.GenderId).HasColumnName("GenderID");

                entity.Property(e => e.GenderName).HasMaxLength(10);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.ShipDate).HasColumnType("datetime");

                entity.Property(e => e.TransactStatusId).HasColumnName("TransactStatusID");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_Orders_Accounts");

                entity.HasOne(d => d.TransactStatus)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.TransactStatusId)
                    .HasConstraintName("FK_Orders_TransactStatus");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.SizeId).HasColumnName("SizeID");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrderDetails_Orders");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_OrderDetails_Products");

                entity.HasOne(d => d.Size)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.SizeId)
                    .HasConstraintName("FK_OrderDetails_Sizes");
            });

            modelBuilder.Entity<Page>(entity =>
            {
                entity.Property(e => e.PageId).HasColumnName("PageID");

                entity.Property(e => e.AuthorName).HasMaxLength(100);

                entity.Property(e => e.CreateDate).HasColumnType("date");

                entity.Property(e => e.Thumb).HasMaxLength(250);

                entity.Property(e => e.Title).HasMaxLength(250);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.BrandId).HasColumnName("BrandID");

                entity.Property(e => e.CatId).HasColumnName("CatID");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.GenderId).HasColumnName("GenderID");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ProductName).HasMaxLength(150);

                entity.Property(e => e.ShortDesc).HasMaxLength(250);

                entity.Property(e => e.Thumb).HasMaxLength(250);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_Products_Brands");

                entity.HasOne(d => d.Cat)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CatId)
                    .HasConstraintName("FK_Products_Categories");

                entity.HasOne(d => d.Gender)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.GenderId)
                    .HasConstraintName("FK_Products_Genders");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.RoleName).HasMaxLength(50);
            });

            modelBuilder.Entity<Size>(entity =>
            {
                entity.Property(e => e.SizeId).HasColumnName("SizeID");

                entity.Property(e => e.Description).HasMaxLength(150);

                entity.Property(e => e.SizeName).HasMaxLength(50);
            });

            modelBuilder.Entity<TransactStatus>(entity =>
            {
                entity.ToTable("TransactStatus");

                entity.Property(e => e.TransactStatusId).HasColumnName("TransactStatusID");

                entity.Property(e => e.Status).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
