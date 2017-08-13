namespace FailTracker.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIssue : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Issues", name: "CreatedBy_Id", newName: "ApplicationUser_Id");
            RenameIndex(table: "dbo.Issues", name: "IX_CreatedBy_Id", newName: "IX_ApplicationUser_Id");
            AddColumn("dbo.Issues", "IssueType", c => c.Int(nullable: false));
            AddColumn("dbo.Issues", "AssignedTo_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Issues", "Creator_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Issues", "AssignedTo_Id");
            CreateIndex("dbo.Issues", "Creator_Id");
            AddForeignKey("dbo.Issues", "AssignedTo_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Issues", "Creator_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Issues", "Creator_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Issues", "AssignedTo_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Issues", new[] { "Creator_Id" });
            DropIndex("dbo.Issues", new[] { "AssignedTo_Id" });
            DropColumn("dbo.Issues", "Creator_Id");
            DropColumn("dbo.Issues", "AssignedTo_Id");
            DropColumn("dbo.Issues", "IssueType");
            RenameIndex(table: "dbo.Issues", name: "IX_ApplicationUser_Id", newName: "IX_CreatedBy_Id");
            RenameColumn(table: "dbo.Issues", name: "ApplicationUser_Id", newName: "CreatedBy_Id");
        }
    }
}
