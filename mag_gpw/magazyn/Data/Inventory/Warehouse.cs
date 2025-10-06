namespace magazyn.Domain.Inventory;

public class Warehouse
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
}

public class Location
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public string Code { get; set; } = "";
    public string? Description { get; set; }
}
