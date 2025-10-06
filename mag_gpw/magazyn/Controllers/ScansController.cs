using magazyn.Data;
using magazyn.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace magazyn.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScansController(AppDb db) : ControllerBase
{
    public record ScanDto(string Code, string? Type, DateTimeOffset? ScannedAt);

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ScanDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Code)) return BadRequest("Code required.");

        var ev = new ScanEvent
        {
            Code = dto.Code.Trim(),
            Type = dto.Type?.Trim(),
            ScannedAt = dto.ScannedAt ?? DateTimeOffset.UtcNow,
            User = User?.Identity?.Name ?? "anonymous"
        };

        db.ScanEvents.Add(ev);
        await db.SaveChangesAsync();
        return Ok(new { id = ev.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int take = 50)
        => Ok(await db.ScanEvents
            .OrderByDescending(x => x.ScannedAt)
            .Take(Math.Clamp(take, 1, 500))
            .ToListAsync());
}
