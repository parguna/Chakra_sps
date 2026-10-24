namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNPACDataModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_NPAC_DesignationInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        NPACDesignation = c.String(),
                        HRDesignationName = c.String(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_NPAC_LevelInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        Level = c.Int(),
                        Description = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_NPAC_UserInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        Emp_Id = c.String(),
                        UserIdentity = c.String(),
                        UserName = c.String(),
                        Passwds = c.String(),
                        Designation = c.String(),
                        Phone = c.String(),
                        Pager = c.String(),
                        Fax = c.String(),
                        Groups = c.String(),
                        Section = c.String(),
                        Grade = c.String(),
                        Zone = c.String(),
                        Create_Date = c.DateTime(),
                        Invalid_Date = c.DateTime(),
                        Encrypt_Pwd = c.String(),
                        Valid_Date = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_NPAC_SectionInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        NPACSection = c.String(),
                        HRSectionName = c.String(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.NPACUserInfoNPACLevelInfo",
                c => new
                    {
                        NPACUserInfo_Id = c.Long(nullable: false),
                        NPACLevelInfo_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.NPACUserInfo_Id, t.NPACLevelInfo_Id })
                .ForeignKey("dbo.DI_NPAC_UserInfo", t => t.NPACUserInfo_Id)
                .ForeignKey("dbo.DI_NPAC_LevelInfo", t => t.NPACLevelInfo_Id)
                .Index(t => t.NPACUserInfo_Id)
                .Index(t => t.NPACLevelInfo_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_NPAC_SectionInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.NPACUserInfoNPACLevelInfo", "NPACLevelInfo_Id", "dbo.DI_NPAC_LevelInfo");
            DropForeignKey("dbo.NPACUserInfoNPACLevelInfo", "NPACUserInfo_Id", "dbo.DI_NPAC_UserInfo");
            DropForeignKey("dbo.DI_NPAC_UserInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_NPAC_LevelInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_NPAC_DesignationInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropIndex("dbo.NPACUserInfoNPACLevelInfo", new[] { "NPACLevelInfo_Id" });
            DropIndex("dbo.NPACUserInfoNPACLevelInfo", new[] { "NPACUserInfo_Id" });
            DropIndex("dbo.DI_NPAC_SectionInfo", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_NPAC_UserInfo", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_NPAC_LevelInfo", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_NPAC_DesignationInfo", new[] { "EndPointSystemId" });
            DropTable("dbo.NPACUserInfoNPACLevelInfo");
            DropTable("dbo.DI_NPAC_SectionInfo");
            DropTable("dbo.DI_NPAC_UserInfo");
            DropTable("dbo.DI_NPAC_LevelInfo");
            DropTable("dbo.DI_NPAC_DesignationInfo");
        }
    }
}
