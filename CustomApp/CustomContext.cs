using System;
using System.Collections.Generic;
using CustomApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomApp;

public partial class CustomContext : DbContext
{
    public CustomContext()
    {
    }

    public CustomContext(DbContextOptions<CustomContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Agent> Agents { get; set; }

    public virtual DbSet<Fee> Fees { get; set; }

    public virtual DbSet<Good> Goods { get; set; }

    public virtual DbSet<GoodType> GoodTypes { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agent>(entity =>
        {
            entity.HasKey(e => e.AgentId).HasName("PK__Agents__9AC3BFF181DEC78A");

            entity.HasIndex(e => e.IdNumber, "UQ__Agents__62DF803382CBC56E").IsUnique();

            entity.Property(e => e.AgentId).ValueGeneratedNever();
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.IdNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Fee>(entity =>
        {
            entity.HasKey(e => e.FeeId).HasName("PK__Fees__B387B2291095316B");

            entity.HasIndex(e => e.DocumentNumber, "UQ__Fees__689939187AB0ECBE").IsUnique();

            entity.Property(e => e.FeeId).ValueGeneratedNever();
            entity.Property(e => e.DocumentNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FeeAmount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Agent).WithMany(p => p.Fees)
                .HasForeignKey(d => d.AgentId)
                .HasConstraintName("FK__Fees__AgentId__48CFD27E");

            entity.HasOne(d => d.Good).WithMany(p => p.Fees)
                .HasForeignKey(d => d.GoodId)
                .HasConstraintName("FK__Fees__GoodId__47DBAE45");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Fees)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("FK__Fees__WarehouseI__46E78A0C");
        });

        modelBuilder.Entity<Good>(entity =>
        {
            entity.HasKey(e => e.GoodId).HasName("PK__Goods__043AE53D4C7FF9A3");

            entity.Property(e => e.GoodId).ValueGeneratedNever();
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.GoodType).WithMany(p => p.Goods)
                .HasForeignKey(d => d.GoodTypeId)
                .HasConstraintName("FK__Goods__GoodTypeI__4316F928");
        });

        modelBuilder.Entity<GoodType>(entity =>
        {
            entity.HasKey(e => e.GoodTypeId).HasName("PK__GoodType__FF5B0BE5150CEE25");

            entity.Property(e => e.GoodTypeId).ValueGeneratedNever();
            entity.Property(e => e.AmountOfFee).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Measurement)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.WarehouseId).HasName("PK__Warehous__2608AFF9C94B2F1A");

            entity.HasIndex(e => e.WarehouseNumber, "UQ__Warehous__EE1D3B419F85AF9E").IsUnique();

            entity.Property(e => e.WarehouseId).ValueGeneratedNever();
            entity.Property(e => e.WarehouseNumber)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasMany(d => d.GoodTypes).WithMany(p => p.Warehouses)
                .UsingEntity<Dictionary<string, object>>(
                    "WarehouseGoodType",
                    r => r.HasOne<GoodType>().WithMany()
                        .HasForeignKey("GoodTypeId")
                        .HasConstraintName("FK__Warehouse__GoodT__403A8C7D"),
                    l => l.HasOne<Warehouse>().WithMany()
                        .HasForeignKey("WarehouseId")
                        .HasConstraintName("FK__Warehouse__Wareh__3F466844"),
                    j =>
                    {
                        j.HasKey("WarehouseId", "GoodTypeId").HasName("PK__Warehous__69FD1F478A9FA6D7");
                        j.ToTable("Warehouse_GoodTypes");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
