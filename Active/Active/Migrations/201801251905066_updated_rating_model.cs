namespace Active.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updated_rating_model : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RatingModels", "ReviewerId", c => c.String(maxLength: 128));
            CreateIndex("dbo.RatingModels", "ReviewerId");
            AddForeignKey("dbo.RatingModels", "ReviewerId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RatingModels", "ReviewerId", "dbo.AspNetUsers");
            DropIndex("dbo.RatingModels", new[] { "ReviewerId" });
            DropColumn("dbo.RatingModels", "ReviewerId");
        }
    }
}
