using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace FriendBook.GroupService.API.DAL.Migrations
{
    /// <inheritdoc />
    public partial class GroupTaskSwitchDateTimeTZ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<OffsetDateTime>(
                name: "date_start_work",
                table: "group_tasks",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<OffsetDateTime>(
                name: "date_end_work",
                table: "group_tasks",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_start_work",
                table: "group_tasks",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(OffsetDateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_end_work",
                table: "group_tasks",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(OffsetDateTime),
                oldType: "timestamp with time zone");
        }
    }
}
