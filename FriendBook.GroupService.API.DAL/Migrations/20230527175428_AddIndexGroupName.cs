using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FriendBook.GroupService.API.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexGroupName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_groups_name",
                table: "groups",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_groups_name",
                table: "groups");
        }
    }
}
