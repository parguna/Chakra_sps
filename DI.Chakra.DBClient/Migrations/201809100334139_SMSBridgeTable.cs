namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SMSBridgeTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DI_SMS_AccessLevel", "SMSUserInfo_Id", "dbo.DI_SMS_UserInfo");
            DropIndex("dbo.DI_SMS_AccessLevel", new[] { "SMSUserInfo_Id" });
            CreateTable(
                "dbo.SMSUserInfoSMSAccessLevel",
                c => new
                    {
                        SMSUserInfo_Id = c.Long(nullable: false),
                        SMSAccessLevel_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.SMSUserInfo_Id, t.SMSAccessLevel_Id })
                .ForeignKey("dbo.DI_SMS_UserInfo", t => t.SMSUserInfo_Id)
                .ForeignKey("dbo.DI_SMS_AccessLevel", t => t.SMSAccessLevel_Id)
                .Index(t => t.SMSUserInfo_Id)
                .Index(t => t.SMSAccessLevel_Id);
            
            DropColumn("dbo.DI_SMS_AccessLevel", "SMSUserInfo_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_SMS_AccessLevel", "SMSUserInfo_Id", c => c.Long());
            DropForeignKey("dbo.SMSUserInfoSMSAccessLevel", "SMSAccessLevel_Id", "dbo.DI_SMS_AccessLevel");
            DropForeignKey("dbo.SMSUserInfoSMSAccessLevel", "SMSUserInfo_Id", "dbo.DI_SMS_UserInfo");
            DropIndex("dbo.SMSUserInfoSMSAccessLevel", new[] { "SMSAccessLevel_Id" });
            DropIndex("dbo.SMSUserInfoSMSAccessLevel", new[] { "SMSUserInfo_Id" });
            DropTable("dbo.SMSUserInfoSMSAccessLevel");
            CreateIndex("dbo.DI_SMS_AccessLevel", "SMSUserInfo_Id");
            AddForeignKey("dbo.DI_SMS_AccessLevel", "SMSUserInfo_Id", "dbo.DI_SMS_UserInfo", "Id");
        }
    }
}
