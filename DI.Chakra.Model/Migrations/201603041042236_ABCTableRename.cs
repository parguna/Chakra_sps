namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ABCTableRename : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.DI_ABC_Configuration", newName: "DI_COP_Configuration");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.DI_COP_Configuration", newName: "DI_ABC_Configuration");
        }
    }
}
