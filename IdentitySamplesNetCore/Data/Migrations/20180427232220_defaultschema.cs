using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace IdentitySamplesNetCore.Data.Migrations
{
    public partial class defaultschema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.EnsureSchema(
                name: "IdentityCore");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newSchema: "IdentityCore");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newSchema: "IdentityCore");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newSchema: "IdentityCore");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newSchema: "IdentityCore");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newSchema: "IdentityCore");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newSchema: "IdentityCore");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newSchema: "IdentityCore");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "IdentityCore",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "IdentityCore",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                schema: "IdentityCore",
                table: "AspNetUserTokens",
                column: "UserId",
                principalSchema: "IdentityCore",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                schema: "IdentityCore",
                table: "AspNetUserTokens");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                schema: "IdentityCore",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                schema: "IdentityCore",
                table: "AspNetRoles");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                schema: "IdentityCore");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                schema: "IdentityCore");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                schema: "IdentityCore");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                schema: "IdentityCore");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                schema: "IdentityCore");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                schema: "IdentityCore");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                schema: "IdentityCore");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName");
        }
    }
}
