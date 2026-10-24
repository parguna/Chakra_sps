namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SMSProvisioningMatrix : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_SMS_AccessLevel",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(),
                        SegmentID = c.Int(nullable: false),
                        Name = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_SMS_AccessLevel");
        }
    }
}
