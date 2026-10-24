namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdColumnDatatypeChange_19 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.DI_M_AccessRightHistory");
            DropPrimaryKey("dbo.DI_M_AccessRights");
            DropPrimaryKey("dbo.DI_M_SqlConfiguration");
            DropPrimaryKey("dbo.DI_M_Config_General_TrustedSource");

            AlterColumn("dbo.DI_M_AccessRightHistory", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_AccessRights", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_SqlConfiguration", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_Config_General_TrustedSource", "Id", c => c.Long(nullable: false, identity: true));
           
            AddPrimaryKey("dbo.DI_M_AccessRightHistory", "Id");
            AddPrimaryKey("dbo.DI_M_AccessRights", "Id");
            AddPrimaryKey("dbo.DI_M_SqlConfiguration", "Id");
            AddPrimaryKey("dbo.DI_M_Config_General_TrustedSource", "Id");
           
           
        }
        
        public override void Down()
        {
          
            DropPrimaryKey("dbo.DI_M_Config_General_TrustedSource");
            DropPrimaryKey("dbo.DI_M_SqlConfiguration");
            DropPrimaryKey("dbo.DI_M_AccessRights");
            DropPrimaryKey("dbo.DI_M_AccessRightHistory");
           
            AlterColumn("dbo.DI_M_Config_General_TrustedSource", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_SqlConfiguration", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_AccessRights", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_AccessRightHistory", "Id", c => c.Int(nullable: false, identity: true));
            
            AddPrimaryKey("dbo.DI_M_Config_General_TrustedSource", "Id");
            AddPrimaryKey("dbo.DI_M_SqlConfiguration", "Id");
            AddPrimaryKey("dbo.DI_M_AccessRights", "Id");
            AddPrimaryKey("dbo.DI_M_AccessRightHistory", "Id");
    
        }
    }
}
