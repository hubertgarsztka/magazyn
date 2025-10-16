using magazyn.Data;
using magazyn.Domain.Inventory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace magazyn.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReceiptsController(AppDb db) : ControllerBase
{
    // proste PZ: przyjęcie wałków 6 m na wskazaną lokację
    public record ReceiptDto(string LocationCode, int Pieces, int LengthMm = 6000);

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ReceiptDto dto)
    {
        if (dto.Pieces <= 0) return BadRequest("Pieces must be > 0.");

        // znajdź lokację po kodzie (np. MAT/A-01)
        var loc = await db.Locations.FirstOrDefaultAsync(l => l.Code == dto.LocationCode);

        if (loc is null) return NotFound($"Location '{dto.LocationCode}' not found. Najpierw dodaj lokację w seederze.");

        var created = new List<long>();

        for (int i = 0; i < dto.Pieces; i++)
        {
            var su = new StockUnit
            {
                ItemId = 1,            // na razie placeholder – rozbudujemy gdy dodamy Item
                LocationId = loc.Id,
                Qty = 1,
                UoM = "SZT",
                LengthMm = dto.LengthMm,
                Status = "OK",
                Barcode = ""           // nadamy po zapisie, gdy będzie Id
            };
            db.StockUnits.Add(su);
            await db.SaveChangesAsync();

            su.Barcode = $"SU:{su.Id:D8}";
            await db.SaveChangesAsync();
            created.Add(su.Id);
        }

        return Ok(new { ok = true, created, location = dto.LocationCode });
    }
}
