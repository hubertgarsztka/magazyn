namespace magazyn.Domain.Inventory;

public class StockUnit
{
    public long Id { get; set; }
    public int? DiameterMm { get; set; }          // średnica wałka (mm)
    public string? MaterialGrade { get; set; }    // gatunek: "C45", "S355", ...
    public int ItemId { get; set; }                 // na razie prosto; rozbudujemy później
    public int? ItemVariantId { get; set; }
    public int? BatchId { get; set; }
    public int LocationId { get; set; }
    public int Qty { get; set; } = 1;
    public string UoM { get; set; } = "SZT";
    public int? LengthMm { get; set; }
    public string Barcode { get; set; } = "";       // np. "SU:00000123"
    public string Status { get; set; } = "OK";
}
