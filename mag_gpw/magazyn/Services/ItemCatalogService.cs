using magazyn.Data;
using magazyn.Domain.Inventory;

public class ItemCatalogService(AppDb db)
{
    public async Task<(Item item, ItemVariant variant)> GetOrCreateAsync(
        string type, int diameterMm, string grade, int? variantLengthMm = null)
    {
        type = type.ToUpperInvariant();
        grade = Normalize(grade);

        var typeCode = type switch
        {
            "RAW_BAR" => "RB",
            "CUT_PIECE" => "CP",
            "FINISHED" => "FN",
            _ => "IT"
        };

        // SKU: RB-040-C45  lub RB-040-CIAGNIONY
        var sku = $"{typeCode}-{diameterMm:000}-{grade}";

        var item = await db.Items.FirstOrDefaultAsync(i => i.Sku == sku && i.Type == type);
        if (item is null)
        {
            item = new Item
            {
                Sku = sku,
                Type = type,
                Name = type switch
                {
                    "RAW_BAR" => $"Wałek Ø{diameterMm} {grade}",
                    "CUT_PIECE" => $"Ciętka Ø{diameterMm} {grade}",
                    _ => $"Artykuł Ø{diameterMm} {grade}"
                }
            };
            db.Items.Add(item);
            await db.SaveChangesAsync();
        }

        var variant = await db.ItemVariants.FirstOrDefaultAsync(v =>
            v.ItemId == item.Id &&
            v.DiameterMm == diameterMm &&
            v.MaterialGrade == grade &&
            v.LengthMm == variantLengthMm);

        if (variant is null)
        {
            variant = new ItemVariant
            {
                ItemId = item.Id,
                DiameterMm = diameterMm,
                MaterialGrade = grade,
                LengthMm = variantLengthMm
            };
            db.ItemVariants.Add(variant);
            await db.SaveChangesAsync();
        }

        return (item, variant);
    }

    private static string Normalize(string input)
        => input.Trim().ToUpperInvariant()
                .Replace("Ą", "A").Replace("Ć", "C").Replace("Ę", "E").Replace("Ł", "L")
                .Replace("Ń", "N").Replace("Ó", "O").Replace("Ś", "S").Replace("Ż", "Z").Replace("Ź", "Z");
}
