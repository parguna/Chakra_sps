namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MissingAuditColumns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_SAP_BIRoleCategories", "CreatedBy", c => c.String());
            AddColumn("dbo.DI_SAP_BIRoleCategories", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.DI_SAP_BIRoleCategories", "LastModifiedBy", c => c.String());
            AddColumn("dbo.DI_SAP_BIRoleCategories", "LastModifiedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_SAP_BIRoleCategories", "LastModifiedDate");
            DropColumn("dbo.DI_SAP_BIRoleCategories", "LastModifiedBy");
            DropColumn("dbo.DI_SAP_BIRoleCategories", "CreatedDate");
            DropColumn("dbo.DI_SAP_BIRoleCategories", "CreatedBy");
        }
    }
}
