using Electro.Core.Entities.Chat;
using Electro.Core.Models;
using Electro.Core.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Reflection.Metadata.BlobBuilder;

namespace Electro.Reposatory.Data.Identity
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Banner> Banners { get; set; }
        public DbSet<BannerProduct> BannerProducts { get; set; }
        public DbSet<BannerCategory> BannerCategories { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<CommunicationMethods> CommunicationMethods { get; set; }
        public DbSet<Contact> Contact { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }

        // This method is called when the model is being created. 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // ✅ هذه السطر ضروري جدًا

            modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(450);
                entity.HasIndex(e => e.UserId);
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.HasOne(e => e.Cart)
                    .WithMany(c => c.CartItems)
                    .HasForeignKey(e => e.CartId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Favorites configuration
            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(450);
                entity.HasIndex(e => new { e.UserId, e.ProductId }).IsUnique();
                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Order configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(450);
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ShippingAddress).IsRequired().HasMaxLength(500);
                entity.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.OrderNumber).IsUnique();
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
                entity.HasOne(e => e.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
           
            modelBuilder.Entity<Notification>(b =>
            {
                b.Property(n => n.Title).HasMaxLength(200).IsRequired();
                b.Property(n => n.Message).HasMaxLength(1000).IsRequired();
                b.Property(n => n.Status).HasMaxLength(100);
                b.HasIndex(n => new { n.ReceiverId, n.IsRead });
            });

            modelBuilder.Entity<Product>()
       .HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // فلتر متوافق مع فلتر Product
            modelBuilder.Entity<ProductImage>()
                .HasQueryFilter(pi => pi.Product != null && !pi.Product.IsDeleted /* && !pi.IsDeleted */);

        }


        private static void SeedRoles(ModelBuilder modelBuilder)
        {
        }

    }

}
