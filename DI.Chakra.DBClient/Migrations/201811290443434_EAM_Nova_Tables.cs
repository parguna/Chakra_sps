namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EAM_Nova_Tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_SAP_EamNovaUserInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        UserId = c.Long(nullable: false),
                        FLINS_UserLogonName = c.String(),
                        FLINS_SAPUserInfoId = c.Long(nullable: false),
                        FLINS_BI_UserLogonName = c.String(),
                        FLINS_BI_SAPUserInfoId = c.Long(nullable: false),
                        AD_UserLogonName = c.String(),
                        ADS_UserInfoId = c.Long(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(),
                        CreatedBy = c.String(),
                        LastModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_SAP_EamNovaUserRoleInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SAPEamNovaUserInfoId = c.Long(nullable: false),
                        RoleName = c.String(),
                        FLINS_TCodes = c.String(),
                        FLINS_BI_TCodes = c.String(),
                        AD_Groups = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(),
                        CreatedBy = c.String(),
                        LastModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_SAP_EamNovaUserInfo", t => t.SAPEamNovaUserInfoId)
                .Index(t => t.SAPEamNovaUserInfoId);
            
            CreateTable(
                "dbo.DI_SAP_SectionMappingInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        HRMSSectionCode = c.String(),
                        SAPSectionCode = c.String(),
                        VBUKR = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_SAP_EamNovaUserRoleInfo", "SAPEamNovaUserInfoId", "dbo.DI_SAP_EamNovaUserInfo");
            DropForeignKey("dbo.DI_SAP_EamNovaUserInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropIndex("dbo.DI_SAP_EamNovaUserRoleInfo", new[] { "SAPEamNovaUserInfoId" });
            DropIndex("dbo.DI_SAP_EamNovaUserInfo", new[] { "EndPointSystemId" });
            DropTable("dbo.DI_SAP_SectionMappingInfo");
            DropTable("dbo.DI_SAP_EamNovaUserRoleInfo");
            DropTable("dbo.DI_SAP_EamNovaUserInfo");
        }
    }
}
