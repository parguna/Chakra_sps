namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class H_12 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_SODConflictFunctions", "CompliancePolicyType", c => c.String());
            DropColumn("dbo.DI_INH_UserInfo", "AccountName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_INH_UserInfo", "AccountName", c => c.String());
            DropColumn("dbo.DI_M_SODConflictFunctions", "CompliancePolicyType");
        }
    }
}
