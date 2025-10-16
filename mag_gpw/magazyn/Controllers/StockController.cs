using magazyn.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace magazyn.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockController(AppDb db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? location = null)
    {
        var q = db.StockUnits.AsQueryable();
        if (!string.IsNullOrWhiteSpace(location))
        {
            var loc = await db.Locations.FirstOrDefaultAsync(l => l.Code == location);
            if (loc is null) return NotFound($"Location '{location}' not found.");
            q = q.Where(s => s.LocationId == loc.Id);
        }
        var data = await q.OrderByDescending(s => s.Id)
            .Select(s => new { s.Id, s.Barcode, s.LengthMm, s.UoM, s.Qty, s.Status, s.LocationId })
            .Take(200)
            .ToListAsync();
        return Ok(data);
    }
}
