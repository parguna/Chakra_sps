namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SkypeForBusinessUnAssignedNumberUpdate : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DI_SFB_UnAssignedNumber", "AnnouncementServiceId");
            DropColumn("dbo.DI_SFB_UnAssignedNumber", "AnnouncementServerFqdn");
            DropColumn("dbo.DI_SFB_UnAssignedNumber", "AnnouncementName");
            DropColumn("dbo.DI_SFB_UnAssignedNumber", "AnnouncementId");
            DropColumn("dbo.DI_SFB_UnAssignedNumber", "Identity");
            DropColumn("dbo.DI_SFB_UnAssignedNumber", "NumberRangeStart");
            DropColumn("dbo.DI_SFB_UnAssignedNumber", "NumberRangeEnd");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_SFB_UnAssignedNumber", "NumberRangeEnd", c => c.String(nullable: false));
            AddColumn("dbo.DI_SFB_UnAssignedNumber", "NumberRangeStart", c => c.String(nullable: false));
            AddColumn("dbo.DI_SFB_UnAssignedNumber", "Identity", c => c.String(nullable: false));
            AddColumn("dbo.DI_SFB_UnAssignedNumber", "AnnouncementId", c => c.Guid());
            AddColumn("dbo.DI_SFB_UnAssignedNumber", "AnnouncementName", c => c.String());
            AddColumn("dbo.DI_SFB_UnAssignedNumber", "AnnouncementServerFqdn", c => c.String());
            AddColumn("dbo.DI_SFB_UnAssignedNumber", "AnnouncementServiceId", c => c.String());
        }
    }
}
