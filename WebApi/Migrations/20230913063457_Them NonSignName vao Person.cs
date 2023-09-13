using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class ThemNonSignNamevaoPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropColumn(
            //     name: "Birthdate",
            //     table: "Person");

            // migrationBuilder.AddColumn<string>(
            //     name: "NonSignName",
            //     table: "Person",
            //     nullable: true);

                migrationBuilder.Sql(
                @"
                    UPDATE Person
                    SET NonSignName = FullName;
                ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
        }
    }
}
