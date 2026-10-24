namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SAPXLProvMatrix : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_SAP_XLProvMatrix", "Role", c => c.String());
            DropColumn("dbo.DI_SAP_XLProvMatrix", "DefaultHRMSGeneralRoles");
            DropColumn("dbo.DI_SAP_XLProvMatrix", "DefaultEPGroup");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_SAP_XLProvMatrix", "DefaultEPGroup", c => c.String());
            AddColumn("dbo.DI_SAP_XLProvMatrix", "DefaultHRMSGeneralRoles", c => c.String());
            DropColumn("dbo.DI_SAP_XLProvMatrix", "Role");
        }
    }
}
