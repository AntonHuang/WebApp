using System.Collections.Generic;
using Microsoft.Data.Entity.Relational.Migrations;
using Microsoft.Data.Entity.Relational.Migrations.Builders;
using Microsoft.Data.Entity.Relational.Migrations.Operations;

namespace WebApp.Migrations
{
    public partial class Init : Migration
    {
        public override void Up(MigrationBuilder migration)
        {
            migration.CreateTable(
                name: "Member",
                columns: table => new
                {
                    MemberID = table.Column(type: "nvarchar(450)", nullable: false),
                    Address = table.Column(type: "nvarchar(max)", nullable: true),
                    LastModifyDate = table.Column(type: "datetime2", nullable: false),
                    Level = table.Column(type: "nvarchar(max)", nullable: true),
                    MemberMemberID = table.Column(type: "nvarchar(450)", nullable: true),
                    Name = table.Column(type: "nvarchar(max)", nullable: true),
                    RegisterDate = table.Column(type: "datetime2", nullable: false),
                    TransactionPassword = table.Column(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.MemberID);
                    table.ForeignKey(
                        name: "FK_Member_Member_MemberMemberID",
                        columns: x => x.MemberMemberID,
                        referencedTable: "Member",
                        referencedColumn: "MemberID");
                });
            migration.AddColumn(
                name: "ChangedPassword",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: 0);
            migration.AddForeignKey(
                name: "FK_ApplicationUser_Member_Id",
                table: "AspNetUsers",
                column: "Id",
                referencedTable: "Member",
                referencedColumn: "MemberID");
        }
        
        public override void Down(MigrationBuilder migration)
        {
            migration.DropForeignKey(name: "FK_ApplicationUser_Member_Id", table: "AspNetUsers");
            migration.DropColumn(name: "ChangedPassword", table: "AspNetUsers");
            migration.DropTable("Member");
        }
    }
}
