using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RapGame.API.Migrations
{
    /// <inheritdoc />
    public partial class RenameAlbumParticipacaoToAlbumParticipacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlbumParticipacao");

            migrationBuilder.CreateTable(
                name: "AlbumParticipacoes",
                columns: table => new
                {
                    AlbumId = table.Column<int>(type: "int", nullable: false),
                    ArtistaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumParticipacoes", x => new { x.AlbumId, x.ArtistaId });
                    table.ForeignKey(
                        name: "FK_AlbumParticipacoes_Albuns_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlbumParticipacoes_Artistas_ArtistaId",
                        column: x => x.ArtistaId,
                        principalTable: "Artistas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlbumParticipacoes_ArtistaId",
                table: "AlbumParticipacoes",
                column: "ArtistaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlbumParticipacoes");

            migrationBuilder.CreateTable(
                name: "AlbumParticipacao",
                columns: table => new
                {
                    AlbumId = table.Column<int>(type: "int", nullable: false),
                    ArtistaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumParticipacao", x => new { x.AlbumId, x.ArtistaId });
                    table.ForeignKey(
                        name: "FK_AlbumParticipacao_Albuns_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlbumParticipacao_Artistas_ArtistaId",
                        column: x => x.ArtistaId,
                        principalTable: "Artistas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlbumParticipacao_ArtistaId",
                table: "AlbumParticipacao",
                column: "ArtistaId");
        }
    }
}
