using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace magazyn.Migrations
{
    /// <inheritdoc />
    public partial class AddBarMaterialParams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Barcode",
                table: "StockUnits",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "DiameterMm",
                table: "StockUnits",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaterialGrade",
                table: "StockUnits",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockUnits_Barcode",
                table: "StockUnits",
                column: "Barcode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockUnits_DiameterMm",
                table: "StockUnits",
                column: "DiameterMm");

            migrationBuilder.CreateIndex(
                name: "IX_StockUnits_LocationId_LengthMm",
                table: "StockUnits",
                columns: new[] { "LocationId", "LengthMm" });

            migrationBuilder.CreateIndex(
                name: "IX_StockUnits_MaterialGrade",
                table: "StockUnits",
                column: "MaterialGrade");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StockUnits_Barcode",
                table: "StockUnits");

            migrationBuilder.DropIndex(
                name: "IX_StockUnits_DiameterMm",
                table: "StockUnits");

            migrationBuilder.DropIndex(
                name: "IX_StockUnits_LocationId_LengthMm",
                table: "StockUnits");

            migrationBuilder.DropIndex(
                name: "IX_StockUnits_MaterialGrade",
                table: "StockUnits");

            migrationBuilder.DropColumn(
                name: "DiameterMm",
                table: "StockUnits");

            migrationBuilder.DropColumn(
                name: "MaterialGrade",
                table: "StockUnits");

            migrationBuilder.AlterColumn<string>(
                name: "Barcode",
                table: "StockUnits",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
