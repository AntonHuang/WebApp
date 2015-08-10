using System.Collections.Generic;
using Microsoft.Data.Entity.Relational.Migrations;
using Microsoft.Data.Entity.Relational.Migrations.Builders;
using Microsoft.Data.Entity.Relational.Migrations.Operations;

namespace WebApp.Migrations
{
    public partial class MyFirstMigration : Migration
    {
        public override void Up(MigrationBuilder migration)
        {
            migration.CreateTable(
                name: "Member",
                columns: table => new
                {
                    MemberID = table.Column(type: "nvarchar(450)", nullable: false),
                    ReferenceMemberID = table.Column(type: "nvarchar(450)", nullable: true),
                    RegisterTime = table.Column(type: "datetime2", nullable: false),
                    TransactionPassword = table.Column(type: "nvarchar(max)", nullable: true),
                    Type = table.Column(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.MemberID);
                    table.ForeignKey(
                        name: "FK_Member_Member_ReferenceMemberID",
                        columns: x => x.ReferenceMemberID,
                        referencedTable: "Member",
                        referencedColumn: "MemberID");
                });
            migration.AddColumn(
                name: "MemberInfoMemberID",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);
            migration.AddForeignKey(
                name: "FK_ApplicationUser_Member_MemberInfoMemberID",
                table: "AspNetUsers",
                column: "MemberInfoMemberID",
                referencedTable: "Member",
                referencedColumn: "MemberID");
        }
        
        public override void Down(MigrationBuilder migration)
        {
            migration.DropForeignKey(name: "FK_ApplicationUser_Member_MemberInfoMemberID", table: "AspNetUsers");
            migration.DropColumn(name: "MemberInfoMemberID", table: "AspNetUsers");
            migration.DropTable("Member");
        }
    }
}
