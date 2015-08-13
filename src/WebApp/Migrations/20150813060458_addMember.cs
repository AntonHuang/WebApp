using System.Collections.Generic;
using Microsoft.Data.Entity.Relational.Migrations;
using Microsoft.Data.Entity.Relational.Migrations.Builders;
using Microsoft.Data.Entity.Relational.Migrations.Operations;

namespace WebApp.Migrations
{
    public partial class addMember : Migration
    {
        public override void Up(MigrationBuilder migration)
        {
            migration.AddColumn(
                name: "IDCard",
                table: "Member",
                type: "nvarchar(max)",
                nullable: true);
            migration.AddColumn(
                name: "Sex",
                table: "Member",
                type: "int",
                nullable: true);
            migration.AlterColumn(
                name: "ChangedPassword",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: 0);
        }
        
        public override void Down(MigrationBuilder migration)
        {
            migration.DropColumn(name: "IDCard", table: "Member");
            migration.DropColumn(name: "Sex", table: "Member");
            migration.AlterColumn(
                name: "ChangedPassword",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: 0);
        }
    }
}
