namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SMSEntityModelChanges : DbMigration
    {
        public override void Up()
        {
            
            
            CreateTable(
                "dbo.DI_SMS_BadgeInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        BadgeKey = c.Int(nullable: false),
                        BadgeNumber = c.Long(nullable: false),
                        UserId = c.Long(nullable: false),
                        STATUS = c.Int(nullable: false),
                        BadgeType = c.Int(nullable: false),
                        DEST_EXEMPT = c.Int(nullable: false),
                        ACTIVATE = c.String(),
                        DEACTIVATE = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_SMS_Company",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        CompanyKey = c.Int(nullable: false),
                        Name = c.String(),
                        SegmentID = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_SMS_UserInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        UserId = c.Int(nullable: false),
                        FIRSTNAME = c.String(),
                        LASTNAME = c.String(),
                        MIDNAME = c.String(),
                        SSNO = c.String(),
                        Addr1 = c.String(),
                        City = c.String(),
                        Zip = c.String(),
                        OPHONE = c.String(),
                        EMAIL = c.String(),
                        DEPT = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            AddColumn("dbo.DI_SMS_AccessLevel", "AccessLevelId", c => c.Long(nullable: false));
            AddColumn("dbo.DI_SMS_AccessLevel", "SMSUserInfo_Id", c => c.Long());
            AlterColumn("dbo.DI_SMS_AccessLevel", "EndPointSystemId", c => c.Long(nullable: false));
            AlterColumn("dbo.DI_SMS_AccessLevel", "SegmentID", c => c.Long(nullable: false));
            CreateIndex("dbo.DI_SMS_AccessLevel", "EndPointSystemId");
            CreateIndex("dbo.DI_SMS_AccessLevel", "SMSUserInfo_Id");
            AddForeignKey("dbo.DI_SMS_AccessLevel", "EndPointSystemId", "dbo.DI_M_EndpointSystem", "Id");
            AddForeignKey("dbo.DI_SMS_AccessLevel", "SMSUserInfo_Id", "dbo.DI_SMS_UserInfo", "Id");
            DropColumn("dbo.DI_SMS_AccessLevel", "TenantId");
            DropColumn("dbo.DI_SMS_AccessLevel", "Version");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_SMS_AccessLevel", "Version", c => c.Int());
            AddColumn("dbo.DI_SMS_AccessLevel", "TenantId", c => c.String());
            DropForeignKey("dbo.DI_SMS_AccessLevel", "SMSUserInfo_Id", "dbo.DI_SMS_UserInfo");
            DropForeignKey("dbo.DI_SMS_UserInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_SMS_Company", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_SMS_BadgeInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_SMS_AccessLevel", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
          
            DropIndex("dbo.DI_SMS_UserInfo", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_SMS_Company", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_SMS_BadgeInfo", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_SMS_AccessLevel", new[] { "SMSUserInfo_Id" });
            DropIndex("dbo.DI_SMS_AccessLevel", new[] { "EndPointSystemId" });
       
            AlterColumn("dbo.DI_SMS_AccessLevel", "SegmentID", c => c.Int(nullable: false));
            AlterColumn("dbo.DI_SMS_AccessLevel", "EndPointSystemId", c => c.Int());
            DropColumn("dbo.DI_SMS_AccessLevel", "SMSUserInfo_Id");
            DropColumn("dbo.DI_SMS_AccessLevel", "AccessLevelId");
            DropTable("dbo.DI_SMS_UserInfo");
            DropTable("dbo.DI_SMS_Company");
            DropTable("dbo.DI_SMS_BadgeInfo");
           
        }
    }
}
