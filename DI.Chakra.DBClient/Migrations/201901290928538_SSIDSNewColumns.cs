namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SSIDSNewColumns : DbMigration
    {
        public override void Up()
        {           
            AddColumn("dbo.DI_SSIDS_UserInfo", "Emp_Id", c => c.String());
            AddColumn("dbo.DI_SSIDS_UserInfo", "OrgUnit", c => c.String());
            AddColumn("dbo.DI_SSIDS_UserInfo", "Department", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_SSIDS_UserInfo", "Department");
            DropColumn("dbo.DI_SSIDS_UserInfo", "OrgUnit");
            DropColumn("dbo.DI_SSIDS_UserInfo", "Emp_Id");         
        }
    }
}
