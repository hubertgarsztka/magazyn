using magazyn.Domain.Common;
using magazyn.Domain.Inventory;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace magazyn.Data;

public class AppDb(DbContextOptions<AppDb> options) : DbContext(options)
{
    public DbSet<ScanEvent> ScanEvents => Set<ScanEvent>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<StockUnit> StockUnits => Set<StockUnit>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<ScanEvent>().HasIndex(x => x.ScannedAt);

        b.Entity<Location>()
            .HasIndex(x => new { x.WarehouseId, x.Code })
            .IsUnique();

        b.Entity<StockUnit>()
            .HasIndex(x => x.LocationId);

        b.Entity<StockUnit>().HasIndex(x => x.Barcode).IsUnique();
        b.Entity<StockUnit>().HasIndex(x => new { x.LocationId, x.LengthMm });
        b.Entity<StockUnit>().HasIndex(x => x.DiameterMm);
        b.Entity<StockUnit>().HasIndex(x => x.MaterialGrade);

    }
}
