namespace Active.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updated_ApplicationUsers_Model : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Rating", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "RatingCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "RatingCount");
            DropColumn("dbo.AspNetUsers", "Rating");
        }
    }
}
