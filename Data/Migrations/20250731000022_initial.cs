using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace connect4_backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "profiles",
                columns: table => new
                {
                    email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    firstName = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    lastName = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    avatarUrl = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    coverUrl = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__profiles__AB6E6165731325AE", x => x.email);
                });

            migrationBuilder.CreateTable(
                name: "friends",
                columns: table => new
                {
                    sender = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    receiver = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    closedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__friends__E805F0FA29B8F55C", x => new { x.sender, x.receiver });
                    table.ForeignKey(
                        name: "FK__friends__receive__3B75D760",
                        column: x => x.receiver,
                        principalTable: "profiles",
                        principalColumn: "email");
                    table.ForeignKey(
                        name: "FK__friends__sender__3A81B327",
                        column: x => x.sender,
                        principalTable: "profiles",
                        principalColumn: "email");
                });

            migrationBuilder.CreateTable(
                name: "matches",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    firstPlayer = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    secondPlayer = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    winner = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__matches__3213E83FBB0E952B", x => x.id);
                    table.ForeignKey(
                        name: "fk_firstPlayer",
                        column: x => x.firstPlayer,
                        principalTable: "profiles",
                        principalColumn: "email");
                    table.ForeignKey(
                        name: "fk_secondPlayer",
                        column: x => x.secondPlayer,
                        principalTable: "profiles",
                        principalColumn: "email");
                    table.ForeignKey(
                        name: "fk_winner",
                        column: x => x.winner,
                        principalTable: "profiles",
                        principalColumn: "email");
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    receiver = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    message = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    type = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    link = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__notifica__3213E83F725FFC79", x => x.id);
                    table.ForeignKey(
                        name: "fk_notifications_receiver",
                        column: x => x.receiver,
                        principalTable: "profiles",
                        principalColumn: "email");
                });

            migrationBuilder.CreateIndex(
                name: "IX_friends_receiver",
                table: "friends",
                column: "receiver");

            migrationBuilder.CreateIndex(
                name: "IX_matches_firstPlayer",
                table: "matches",
                column: "firstPlayer");

            migrationBuilder.CreateIndex(
                name: "IX_matches_secondPlayer",
                table: "matches",
                column: "secondPlayer");

            migrationBuilder.CreateIndex(
                name: "IX_matches_winner",
                table: "matches",
                column: "winner");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_receiver",
                table: "notifications",
                column: "receiver");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "friends");

            migrationBuilder.DropTable(
                name: "matches");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "profiles");
        }
    }
}
