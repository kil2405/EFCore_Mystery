using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MysteryBox.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_click_count_log",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    log_date = table.Column<long>(type: "bigint", nullable: false),
                    click_count = table.Column<int>(type: "int", nullable: false),
                    total_count = table.Column<long>(type: "bigint", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_click_count_log", x => new { x.user_id, x.log_date });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_global_ranking",
                columns: table => new
                {
                    region = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    count = table.Column<long>(type: "bigint", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_global_ranking", x => x.region);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_gold_acq_log",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    log_date = table.Column<long>(type: "bigint", nullable: false),
                    add_value = table.Column<int>(type: "int", nullable: false),
                    total_value = table.Column<long>(type: "bigint", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_gold_acq_log", x => new { x.user_id, x.log_date });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_item_acq_log",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    log_date = table.Column<long>(type: "bigint", nullable: false),
                    item_id = table.Column<int>(type: "int", nullable: false),
                    add_count = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_item_acq_log", x => new { x.user_id, x.log_date });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_item_reward",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    appear_num = table.Column<int>(type: "int", nullable: false),
                    count = table.Column<long>(type: "bigint", nullable: false),
                    is_reward = table.Column<bool>(type: "bit(1)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_item_reward", x => new { x.user_id, x.appear_num });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_reward_gold",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    gold = table.Column<int>(type: "int", nullable: false),
                    reward_time = table.Column<long>(type: "bigint", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_reward_gold", x => x.user_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_user",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    device_id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nickname = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    region = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    gold = table.Column<long>(type: "bigint", nullable: false),
                    click_count = table.Column<long>(type: "bigint", nullable: false),
                    terms_agree = table.Column<bool>(type: "bit(1)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_user", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_user_connect_log",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    log_date = table.Column<long>(type: "bigint", nullable: false),
                    ip = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    country = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_user_connect_log", x => new { x.user_id, x.log_date });
                    table.ForeignKey(
                        name: "FK_tb_user_connect_log_tb_user_user_id",
                        column: x => x.user_id,
                        principalTable: "tb_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_user_item",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    item_id = table.Column<int>(type: "int", nullable: false),
                    count = table.Column<int>(type: "int", nullable: false),
                    is_equip = table.Column<bool>(type: "bit(1)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_user_item", x => new { x.user_id, x.item_id });
                    table.ForeignKey(
                        name: "FK_tb_user_item_tb_user_user_id",
                        column: x => x.user_id,
                        principalTable: "tb_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_tb_user_device_id",
                table: "tb_user",
                column: "device_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_user_nickname",
                table: "tb_user",
                column: "nickname");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_click_count_log");

            migrationBuilder.DropTable(
                name: "tb_global_ranking");

            migrationBuilder.DropTable(
                name: "tb_gold_acq_log");

            migrationBuilder.DropTable(
                name: "tb_item_acq_log");

            migrationBuilder.DropTable(
                name: "tb_item_reward");

            migrationBuilder.DropTable(
                name: "tb_reward_gold");

            migrationBuilder.DropTable(
                name: "tb_user_connect_log");

            migrationBuilder.DropTable(
                name: "tb_user_item");

            migrationBuilder.DropTable(
                name: "tb_user");
        }
    }
}
