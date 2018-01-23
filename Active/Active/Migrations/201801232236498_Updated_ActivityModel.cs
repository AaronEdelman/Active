namespace Active.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updated_ActivityModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ActivityModels", "ActivityLength", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ActivityModels", "ActivityLength");
        }
    }
}
