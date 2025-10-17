using magazyn.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace magazyn.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockController(AppDb db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] string? location = null, 
        [FromQuery] int? minLen = null, 
        [FromQuery] int? maxLen = null, 
        [FromQuery] int? diameterMm = null,
        [FromQuery] string? grade = null
     )
    {
        var q = db.StockUnits.AsQueryable();
        if (!string.IsNullOrWhiteSpace(location))
        {
            var loc = await db.Locations.FirstOrDefaultAsync(l => l.Code == location);
            if (loc is null) return NotFound($"Location '{location}' not found.");
            q = q.Where(s => s.LocationId == loc.Id);
        }
        if (minLen is not null) q = q.Where(s => s.LengthMm >= minLen);
        if (maxLen is not null) q = q.Where(s => s.LengthMm <= maxLen);
        if (diameterMm is not null) q = q.Where(s => s.DiameterMm == diameterMm);
        if (!string.IsNullOrWhiteSpace(grade)) q = q.Where(s => s.MaterialGrade == grade.ToUpper());


        var data = await q.OrderByDescending(s => s.Id)
            .Select(s => new { s.Id, s.Barcode, s.LengthMm, s.DiameterMm, s.MaterialGrade, s.UoM, s.Qty, s.Status, s.LocationId })
            .Take(200)
            .ToListAsync();
        return Ok(data);
    }

}
