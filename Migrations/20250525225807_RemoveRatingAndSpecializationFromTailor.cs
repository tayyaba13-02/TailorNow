using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TailorrNow.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRatingAndSpecializationFromTailor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Tailors");

            migrationBuilder.DropColumn(
                name: "Specialization",
                table: "Tailors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Rating",
                table: "Tailors",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Specialization",
                table: "Tailors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
