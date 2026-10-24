namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MissingMigration : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DI_M_CompliancePolicy", "CreatedBy");
            DropColumn("dbo.DI_M_CompliancePolicy", "CreatedDate");
            DropColumn("dbo.DI_M_CompliancePolicy", "LastModifiedBy");
            DropColumn("dbo.DI_M_CompliancePolicy", "LastModifiedDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_M_CompliancePolicy", "LastModifiedDate", c => c.DateTime());
            AddColumn("dbo.DI_M_CompliancePolicy", "LastModifiedBy", c => c.String());
            AddColumn("dbo.DI_M_CompliancePolicy", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.DI_M_CompliancePolicy", "CreatedBy", c => c.String());
        }
    }
}
