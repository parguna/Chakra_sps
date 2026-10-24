namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MSTeam : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_SFB_ResponseGroupInfo", "DistributionGroupName", c => c.String());
            AddColumn("dbo.DI_SFB_UserInfo", "TenantDialPlan", c => c.String());
            AddColumn("dbo.DI_SFB_UserInfo", "OnlineVoiceRoutingPolicy", c => c.String());
            AddColumn("dbo.DI_SFB_UserInfo", "TeamsUpgradePolicy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_SFB_UserInfo", "TeamsUpgradePolicy");
            DropColumn("dbo.DI_SFB_UserInfo", "OnlineVoiceRoutingPolicy");
            DropColumn("dbo.DI_SFB_UserInfo", "TenantDialPlan");
            DropColumn("dbo.DI_SFB_ResponseGroupInfo", "DistributionGroupName");
        }
    }
}
