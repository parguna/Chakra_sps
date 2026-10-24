namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SAPSODConflictingRole : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_SAP_SODConflictingRole",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BusinessRole = c.String(),
                        ConflictingBusinessRole = c.String(),
                        TechnicalRole = c.String(),
                        ConflictingTechnicalRole = c.String(),
                        ConflictingRemarks = c.String(),
                        SODOwner = c.String(),
                        EndpointSystemName = c.String(),
                        IsModified = c.Boolean(),
                        IsDeleted = c.Boolean(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                        FileName = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_SAP_SODConflictingRole");
        }
    }
}
