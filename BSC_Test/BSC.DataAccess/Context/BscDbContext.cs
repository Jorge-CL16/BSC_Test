using System;
using System.Collections.Generic;
using BSC.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace BSC.DataAccess.Context;


public partial class BscDbContext : DbContext
{

    public BscDbContext(DbContextOptions<BscDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<InventoryMovement> InventoryMovements { get; set; }


    public virtual DbSet<Order> Orders { get; set; }


    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Product> Products { get; set; }


    public virtual DbSet<User> Users { get; set; }


    public virtual DbSet<VwOrderSummary> VwOrderSummaries { get; set; }


    public virtual DbSet<VwProductStock> VwProductStocks { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InventoryMovement>(entity =>
        {
            entity.HasIndex(e => e.OrderId, "IX_InventoryMovements_OrderId");

            entity.HasIndex(e => new { e.ProductId, e.CreatedAt }, "IX_InventoryMovements_ProductId_CreatedAt").IsDescending(false, true);

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_InventoryMovements_CreatedAt");
            entity.Property(e => e.MovementType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Reason).HasMaxLength(250);

            entity.HasOne(d => d.Order).WithMany(p => p.InventoryMovements)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_InventoryMovements_Orders");

            entity.HasOne(d => d.Product).WithMany(p => p.InventoryMovements)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InventoryMovements_Products");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable(tb => tb.HasTrigger("trg_Orders_ValidateStatusChange"));

            entity.HasIndex(e => new { e.UserId, e.OrderDate }, "IX_Orders_UserId_OrderDate").IsDescending(false, true);

            entity.Property(e => e.CustomerName).HasMaxLength(150);
            entity.Property(e => e.OrderDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Orders_OrderDate");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending", "DF_Orders_Status");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Users");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasIndex(e => e.ProductId, "IX_OrderDetails_ProductId");

            entity.HasIndex(e => new { e.OrderId, e.ProductId }, "UQ_OrderDetails_Order_Product").IsUnique();

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Orders");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Products");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable(tb => tb.HasTrigger("trg_Products_PreventNegativeStock"));

            entity.HasIndex(e => e.ProductKey, "UQ_Products_ProductKey").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Products_CreatedAt");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Products_IsActive");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.ProductKey).HasDefaultValueSql("(newsequentialid())", "DF_Products_ProductKey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.Property(e => e.Active).HasDefaultValue(true, "DF_Users_Active");
            entity.Property(e => e.Email).HasMaxLength(254);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
        });

        modelBuilder.Entity<VwOrderSummary>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_OrderSummary");

            entity.Property(e => e.CustomerName).HasMaxLength(150);
            entity.Property(e => e.OrderDate).HasPrecision(0);
            entity.Property(e => e.SalespersonName).HasMaxLength(150);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwProductStock>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_ProductStock");

            entity.Property(e => e.CreatedAt).HasPrecision(0);
            entity.Property(e => e.ProductId).ValueGeneratedOnAdd();
            entity.Property(e => e.ProductName).HasMaxLength(150);
            entity.Property(e => e.StockStatus)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
