using magazyn.Domain.Inventory;

namespace magazyn.Data;

public static class DbSeeder
{
    public static void Seed(AppDb db)
    {
        if (!db.Warehouses.Any())
        {
            var mat = new Warehouse { Code = "MAT", Name = "Materiał" };
            var cie = new Warehouse { Code = "CIE", Name = "Ciętka" };
            var got = new Warehouse { Code = "GOT", Name = "Gotowe" };
            db.Warehouses.AddRange(mat, cie, got);
            db.SaveChanges();

            db.Locations.AddRange(
                new Location { WarehouseId = mat.Id, Code = "A-01", Description = "Regał A-01" },
                new Location { WarehouseId = cie.Id, Code = "B-01", Description = "Regał B-01" },
                new Location { WarehouseId = got.Id, Code = "C-01", Description = "Regał C-01" }
            );
            db.SaveChanges();
        }
    }
}
