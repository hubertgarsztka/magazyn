namespace magazyn.Domain.Inventory;

public class ItemVariant
{
    public int Id { get; set; }
    public int ItemId { get; set; }

    public int DiameterMm { get; set; }
    public string MaterialGrade { get; set; } = ""; // C45 | S355 | CIAGNIONY
    public int? LengthMm { get; set; }
}