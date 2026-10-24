namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Office365PendingEntittyChanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_Office365_LicenseInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        AccountSkuId = c.String(),
                        ActiveUnits = c.Int(nullable: false),
                        WarningUnits = c.Int(nullable: false),
                        ConsumedUnits = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_M_Office365MassLicense",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        IsMassAdd = c.Boolean(nullable: false),
                        IsCompleted = c.Boolean(nullable: false),
                        Licenses = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_Office365_RetentionPolicy",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        Name = c.String(),
                        PolicyId = c.String(),
                        DistinguishedName = c.String(),
                        IsDefault = c.Boolean(),
                        Identity = c.String(),
                        WhenCreated = c.DateTime(),
                        IsValid = c.Boolean(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_Office365_RoleAssignmentPolicy",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        Name = c.String(),
                        PolicyId = c.String(),
                        DistinguishedName = c.String(),
                        IsDefault = c.Boolean(),
                        Description = c.String(),
                        Identity = c.String(),
                        WhenCreated = c.DateTime(),
                        IsValid = c.Boolean(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_Office365_ServicePlan",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        AccountSkuId = c.String(),
                        ServiceName = c.String(),
                        ServiceType = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_Office365_SharingPolicy",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        Name = c.String(),
                        PolicyId = c.String(),
                        DistinguishedName = c.String(),
                        IsDefault = c.Boolean(),
                        IsEnabled = c.Boolean(),
                        Identity = c.String(),
                        WhenCreated = c.DateTime(),
                        IsValid = c.Boolean(),
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
            DropForeignKey("dbo.DI_Office365_SharingPolicy", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_Office365_ServicePlan", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_Office365_RoleAssignmentPolicy", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_Office365_RetentionPolicy", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_Office365MassLicense", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_Office365_LicenseInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropIndex("dbo.DI_Office365_SharingPolicy", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_Office365_ServicePlan", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_Office365_RoleAssignmentPolicy", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_Office365_RetentionPolicy", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_M_Office365MassLicense", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_Office365_LicenseInfo", new[] { "EndPointSystemId" });
            DropTable("dbo.DI_Office365_SharingPolicy");
            DropTable("dbo.DI_Office365_ServicePlan");
            DropTable("dbo.DI_Office365_RoleAssignmentPolicy");
            DropTable("dbo.DI_Office365_RetentionPolicy");
            DropTable("dbo.DI_M_Office365MassLicense");
            DropTable("dbo.DI_Office365_LicenseInfo");
        }
    }
}
