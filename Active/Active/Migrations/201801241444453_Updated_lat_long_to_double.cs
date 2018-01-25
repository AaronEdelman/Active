namespace Active.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updated_lat_long_to_double : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ActivityModels", "Latitude", c => c.Double(nullable: false));
            AlterColumn("dbo.ActivityModels", "Longitude", c => c.Double(nullable: false));
            AlterColumn("dbo.ActivityModels", "Area", c => c.Double(nullable: false));
            AlterColumn("dbo.CheckinModels", "Latitude", c => c.Double(nullable: false));
            AlterColumn("dbo.CheckinModels", "Longitude", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CheckinModels", "Longitude", c => c.Single(nullable: false));
            AlterColumn("dbo.CheckinModels", "Latitude", c => c.Single(nullable: false));
            AlterColumn("dbo.ActivityModels", "Area", c => c.Single(nullable: false));
            AlterColumn("dbo.ActivityModels", "Longitude", c => c.Single(nullable: false));
            AlterColumn("dbo.ActivityModels", "Latitude", c => c.Single(nullable: false));
        }
    }
}
