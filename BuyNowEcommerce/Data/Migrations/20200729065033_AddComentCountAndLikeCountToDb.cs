using Microsoft.EntityFrameworkCore.Migrations;

namespace BuyNowEcommerce.Data.Migrations
{
    public partial class AddComentCountAndLikeCountToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommentCount",
                columns: table => new
                {
                    CommentCountId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    PostId = table.Column<int>(nullable: false),
                    Count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentCount", x => x.CommentCountId);
                    table.ForeignKey(
                        name: "FK_CommentCount_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LikeCount",
                columns: table => new
                {
                    LikesCountId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    PostId = table.Column<int>(nullable: false),
                    Count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikeCount", x => x.LikesCountId);
                    table.ForeignKey(
                        name: "FK_LikeCount_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentCount_PostId",
                table: "CommentCount",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_LikeCount_PostId",
                table: "LikeCount",
                column: "PostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentCount");

            migrationBuilder.DropTable(
                name: "LikeCount");
        }
    }
}
