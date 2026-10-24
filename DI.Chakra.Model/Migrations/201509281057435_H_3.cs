namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class H_3 : DbMigration
    {
        public override void Up()
        {
            /* This is intentionally left blank because it generate duplicate migration as H_2*/

            //AddColumn("dbo.DI_M_CompliancePolicy", "CreatedBy", c => c.String());
            //AddColumn("dbo.DI_M_CompliancePolicy", "CreatedDate", c => c.DateTime());
            //AddColumn("dbo.DI_M_CompliancePolicy", "LastModifiedBy", c => c.String());
            //AddColumn("dbo.DI_M_CompliancePolicy", "LastModifiedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            /* This is intentionally left blank because it generate duplicate migration as H_2*/

            //DropColumn("dbo.DI_M_CompliancePolicy", "LastModifiedDate");
            //DropColumn("dbo.DI_M_CompliancePolicy", "LastModifiedBy");
            //DropColumn("dbo.DI_M_CompliancePolicy", "CreatedDate");
            //DropColumn("dbo.DI_M_CompliancePolicy", "CreatedBy");
        }
    }
}
