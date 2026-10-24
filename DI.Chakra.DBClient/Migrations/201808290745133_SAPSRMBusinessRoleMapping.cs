namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SAPSRMBusinessRoleMapping : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_SAP_SAPSRMBusinessRoleMapping",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        UserName = c.String(),
                        AccountType = c.Byte(nullable: false),
                        BusinessRole = c.String(),
                        POrg = c.String(),
                        PGrp = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_SAP_SAPSRMBusinessRoleMapping");
        }
    }
}
