namespace WAPP_Assignment_Module.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameModuleToOlyModule : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Modules", newName: "OlyModules");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.OlyModules", newName: "Modules");
        }
    }
}
