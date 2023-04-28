using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FriendBook.GroupService.API.DAL.Migrations
{
    /// <inheritdoc />
    public partial class add_creater_id : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "creater_account_id",
                table: " account_status_groups",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "creater_account_id",
                table: " account_status_groups");
        }
    }
}
