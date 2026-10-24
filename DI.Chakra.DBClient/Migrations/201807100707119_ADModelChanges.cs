namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ADModelChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_ADS_UserInfo", "otherPager", c => c.String());
            AddColumn("dbo.DI_ADS_UserInfo", "otherFascimileTelephoneNumber", c => c.String());
            AddColumn("dbo.DI_ADS_UserInfo", "extensionAttribute4", c => c.String());
            AddColumn("dbo.DI_ADS_UserInfo", "NeedO365License", c => c.Boolean(nullable: false));
            AddColumn("dbo.DI_M_OUChangeSnapshot", "AccountName", c => c.String());
            AddColumn("dbo.DI_M_OUChangeSnapshot", "EmployeeeEventType", c => c.Byte(nullable: false));
            DropColumn("dbo.DI_ADS_UserInfo", "CardSerialNumber");
            DropColumn("dbo.DI_ADS_UserInfo", "EmployeeId");
            DropColumn("dbo.DI_ADS_UserInfo", "Last4DigitNRIC");
            DropColumn("dbo.DI_ADS_UserInfo", "DateOfBirth");
            DropColumn("dbo.DI_ADS_UserInfo", "CostCenterCode");
            DropColumn("dbo.DI_ADS_UserInfo", "CostCenterDesc");
            DropColumn("dbo.DI_ADS_UserInfo", "PrivilegeAccount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_ADS_UserInfo", "PrivilegeAccount", c => c.String());
            AddColumn("dbo.DI_ADS_UserInfo", "CostCenterDesc", c => c.String());
            AddColumn("dbo.DI_ADS_UserInfo", "CostCenterCode", c => c.String());
            AddColumn("dbo.DI_ADS_UserInfo", "DateOfBirth", c => c.String());
            AddColumn("dbo.DI_ADS_UserInfo", "Last4DigitNRIC", c => c.String());
            AddColumn("dbo.DI_ADS_UserInfo", "EmployeeId", c => c.String());
            AddColumn("dbo.DI_ADS_UserInfo", "CardSerialNumber", c => c.String());
            DropColumn("dbo.DI_M_OUChangeSnapshot", "EmployeeeEventType");
            DropColumn("dbo.DI_M_OUChangeSnapshot", "AccountName");
            DropColumn("dbo.DI_ADS_UserInfo", "NeedO365License");
            DropColumn("dbo.DI_ADS_UserInfo", "extensionAttribute4");
            DropColumn("dbo.DI_ADS_UserInfo", "otherFascimileTelephoneNumber");
            DropColumn("dbo.DI_ADS_UserInfo", "otherPager");
        }
    }
}
