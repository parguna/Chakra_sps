namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SAPRoleToTcodes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_SAP_RoleToTCodes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RoleCode = c.String(),
                        TCodes = c.String(),
                        EndPointSystemId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_SAP_RoleToTCodes");
        }
    }
}
