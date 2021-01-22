using Microsoft.EntityFrameworkCore.Migrations;

namespace MovieApi.Migrations
{
    public partial class AdminRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.
                Sql(@"insert into [dbo].[AspNetRoles]([Id],[Name],[NormalizedName]) values 
('06f3eb69-f879-45f9-ad9b-2bad469dbf10','Admin','Admin')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.
                Sql(@"delete AspNetRoles where id='06f3eb69-f879-45f9-ad9b-2bad469dbf10'");
        }
    }
}
