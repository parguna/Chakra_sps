namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdColumnDatatypeChange_5 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.DI_M_ConfigurationMaster");
            AlterColumn("dbo.DI_M_ConfigurationMaster", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_ConfigurationMaster", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.DI_M_ConfigurationMaster");
            AlterColumn("dbo.DI_M_ConfigurationMaster", "Id", c => c.Short(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_ConfigurationMaster", "Id");
        }
    }
}
