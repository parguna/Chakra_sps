namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SOD_CriticalTCodeReport : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_SAP_ConflictingRoleReport",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        EndpointSystemName = c.String(),
                        AccountName = c.String(),
                        Displayname = c.String(),
                        BusinessRole = c.String(),
                        TechnicalRole = c.String(),
                        From_Position = c.String(),
                        ConflictingPosition = c.String(),
                        ConflictingRole = c.String(),
                        ConflictingTechnicalRole = c.String(),
                        SoDRemarks = c.String(),
                        AccountType = c.Byte(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_SAP_CriticalTcodeReport",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        EndpointSystemName = c.String(),
                        AccountName = c.String(),
                        Displayname = c.String(),
                        Transaction = c.String(),
                        Description = c.String(),
                        BusinessRole = c.String(),
                        TechnicalRole = c.String(),
                        FromPosition = c.String(),
                        AccountType = c.Byte(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_SAP_CriticalTcodeReport", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_SAP_ConflictingRoleReport", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropIndex("dbo.DI_SAP_CriticalTcodeReport", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_SAP_ConflictingRoleReport", new[] { "EndPointSystemId" });
            DropTable("dbo.DI_SAP_CriticalTcodeReport");
            DropTable("dbo.DI_SAP_ConflictingRoleReport");
        }
    }
}
