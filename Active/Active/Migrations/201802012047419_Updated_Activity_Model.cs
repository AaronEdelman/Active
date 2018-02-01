namespace Active.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updated_Activity_Model : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ActivityModels", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.ActivityModels", "Description", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ActivityModels", "Description", c => c.String());
            AlterColumn("dbo.ActivityModels", "Name", c => c.String());
        }
    }
}
