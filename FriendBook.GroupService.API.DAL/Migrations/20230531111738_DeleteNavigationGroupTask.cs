using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FriendBook.GroupService.API.DAL.Migrations
{
    /// <inheritdoc />
    public partial class DeleteNavigationGroupTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ account_status_groups_group_tasks_group_id",
                table: " account_status_groups");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_group_tasks_group_id",
                table: "group_tasks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_group_tasks_group_id",
                table: "group_tasks",
                column: "group_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ account_status_groups_group_tasks_group_id",
                table: " account_status_groups",
                column: "group_id",
                principalTable: "group_tasks",
                principalColumn: "group_id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
