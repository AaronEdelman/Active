namespace Active.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updated_Rating_model_name : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.RatingToUserModels", newName: "RatingModels");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.RatingModels", newName: "RatingToUserModels");
        }
    }
}
