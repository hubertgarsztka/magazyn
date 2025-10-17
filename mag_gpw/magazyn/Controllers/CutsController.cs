using System.ComponentModel.DataAnnotations;
using magazyn.Data;
using magazyn.Domain.Inventory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace magazyn.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CutsController(AppDb db) : ControllerBase
{
    // DTO żądania cięcia
    public record CutLine([Required] int TargetLengthMm, [Range(1, 1_000)] int Qty);
    public record CutRequest(
        [Required] long FromStockUnitId,
        [Required] string TargetLocationCode,           // np. "B-01" (magazyn CIĘTKA)
        [Range(0, 50)] int KerfMm = 3,                  // szerokość piły na jedno cięcie (mm)
        bool IncludeLastCutKerf = true,                 // czy doliczać kerf za ostatnie cięcie (zwykle TAK)
        int MinRemainderMm = 150,                       // minimalna resztka do zachowania
        [Required] List<CutLine> Lines = null!          // np. [{1500,2},{2000,1}]
    );

    public record CutResponse(
        long SourceId,
        int SourceOriginalLengthMm,
        int TotalPieces,
        int ConsumedKerfMm,
        int UsedLengthMm,
        int? RemainderMm,
        long? RemainderStockUnitId,
        List<long> CreatedPieceIds
    );

    [HttpPost]
    public async Task<ActionResult<CutResponse>> Post([FromBody] CutRequest req)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        // 1) Załaduj wałek źródłowy
        var src = await db.StockUnits.FirstOrDefaultAsync(s => s.Id == req.FromStockUnitId);
        if (src is null) return NotFound($"StockUnit {req.FromStockUnitId} nie istnieje.");
        if (src.LengthMm is null || src.LengthMm <= 0) return BadRequest("Źródłowy StockUnit nie ma ustawionej długości.");
        if (src.Status is "CONSUMED") return BadRequest("Źródłowy StockUnit jest już zużyty.");

        var srcLen = src.LengthMm.Value;

        // 2) Obliczenia sum
        var totalPieces = req.Lines.Sum(l => l.Qty);
        var totalLength = req.Lines.Sum(l => l.TargetLengthMm * l.Qty);

        var kerfCount = Math.Max(0, totalPieces - (req.IncludeLastCutKerf ? 0 : 1));
        // jeśli IncludeLastCutKerf = true -> kerfCount = totalPieces
        // jeśli false -> kerfCount = totalPieces - 1
        if (req.IncludeLastCutKerf) kerfCount = totalPieces;

        var kerfConsumed = req.KerfMm * kerfCount;
        var used = totalLength + kerfConsumed;

        if (used > srcLen)
            return BadRequest($"Suma cięć {totalLength} mm + kerf {kerfConsumed} mm przekracza dostępne {srcLen} mm.");

        // 3) Znajdź lokację docelową
        var loc = await db.Locations.FirstOrDefaultAsync(l => l.Code == req.TargetLocationCode);
        if (loc is null) return NotFound($"Lokacja '{req.TargetLocationCode}' nie istnieje. Dodaj w seederze.");

        var createdIds = new List<long>();
        long? remainderId = null;
        int remainder = srcLen - used;

        using var tx = await db.Database.BeginTransactionAsync();

        try
        {
            // 4) Utwórz odcinki
            foreach (var line in req.Lines)
            {
                for (int i = 0; i < line.Qty; i++)
                {
                    var piece = new StockUnit
                    {
                        ItemId = src.ItemId,              // na razie dziedziczymy Id asortymentu
                        ItemVariantId = src.ItemVariantId, // i wariant
                        BatchId = src.BatchId,
                        LocationId = loc.Id,
                        Qty = 1,
                        UoM = "SZT",
                        LengthMm = line.TargetLengthMm,
                        Status = "OK",
                        Barcode = ""
                    };
                    db.StockUnits.Add(piece);
                    await db.SaveChangesAsync();

                    piece.Barcode = $"SU:{piece.Id:D8}";
                    await db.SaveChangesAsync();
                    createdIds.Add(piece.Id);
                }
            }

            // 5) Resztka (jeśli się opłaca)
            if (remainder >= req.MinRemainderMm)
            {
                var rem = new StockUnit
                {
                    ItemId = src.ItemId,
                    ItemVariantId = src.ItemVariantId,
                    BatchId = src.BatchId,
                    LocationId = loc.Id,        // możesz zostawić w tej samej strefie „CIE”
                    Qty = 1,
                    UoM = "SZT",
                    LengthMm = remainder,
                    Status = "OK",
                    Barcode = ""
                };
                db.StockUnits.Add(rem);
                await db.SaveChangesAsync();

                rem.Barcode = $"SU:{rem.Id:D8}";
                await db.SaveChangesAsync();

                remainderId = rem.Id;
            }

            // 6) Oznacz źródłowy wałek jako zużyty
            src.Status = "CONSUMED";
            await db.SaveChangesAsync();

            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }

        var resp = new CutResponse(
            SourceId: src.Id,
            SourceOriginalLengthMm: srcLen,
            TotalPieces: totalPieces,
            ConsumedKerfMm: kerfConsumed,
            UsedLengthMm: used,
            RemainderMm: remainderId is null ? null : remainder,
            RemainderStockUnitId: remainderId,
            CreatedPieceIds: createdIds
        );

        return Ok(resp);
    }
}
