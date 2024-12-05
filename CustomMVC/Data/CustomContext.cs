using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CustomMVC.Models;

namespace CustomMVC.Data;

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
}

