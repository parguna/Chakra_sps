namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OtherSystemReverseEngineering : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_AccountEntitlementAuditTrial",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        DisplayName = c.String(),
                        UserName = c.String(maxLength: 512),
                        Permission = c.String(),
                        AccountType = c.String(maxLength: 32),
                        Remarks = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_M_AccountEntitlementAuditTrial");
        }
    }
}
