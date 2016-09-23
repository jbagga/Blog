using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Blog.Data.Migrations
{
    public partial class CommentList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CommentAuthor = table.Column<string>(nullable: true),
                    CommentBody = table.Column<string>(maxLength: 1000, nullable: true),
                    CommentDate = table.Column<DateTime>(nullable: false),
                    PostID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Comment_Post_PostID",
                        column: x => x.PostID,
                        principalTable: "Post",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Post",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Post",
                maxLength: 60,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comment_PostID",
                table: "Comment",
                column: "PostID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Post",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Post",
                nullable: true);
        }
    }
}
