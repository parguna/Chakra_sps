namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HRModelChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_NCostCenter", "CompanyId", c => c.Long(nullable: false));
            DropTable("dbo.DI_HRMS_CostCenter");
            DropTable("dbo.DI_HRMS_Designation");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.DI_HRMS_Designation",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointSystemId = c.Long(nullable: false),
                        Code = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_HRMS_CostCenter",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointSystemId = c.Long(nullable: false),
                        CompanyId = c.Long(nullable: false),
                        Code = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.DI_M_NCostCenter", "CompanyId");
        }
    }
}
