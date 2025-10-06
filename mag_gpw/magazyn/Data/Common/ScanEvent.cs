namespace magazyn.Domain.Common;

public class ScanEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = "";
    public string? Type { get; set; }   // "stockUnit" | "operation" | "workOrder" | "raw"
    public DateTimeOffset ScannedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? User { get; set; }
    public string? Source { get; set; } = "HID-PC";
}
