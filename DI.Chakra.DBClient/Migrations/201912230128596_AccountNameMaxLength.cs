namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccountNameMaxLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DI_M_User", "UserName", c => c.String(maxLength: 32));
            AlterColumn("dbo.DI_M_User", "ROEmployeeId", c => c.String(maxLength: 32));
            AlterColumn("dbo.DI_M_User", "COEmployeeId", c => c.String(maxLength: 32));
            AlterColumn("dbo.DI_M_User", "Reserved5", c => c.String(maxLength: 32));
            AlterColumn("dbo.DI_M_AccountReconcilation", "AccountName", c => c.String(maxLength: 128));
            AlterColumn("dbo.DI_M_EntitlementReconcilation", "AccountName", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DI_M_EntitlementReconcilation", "AccountName", c => c.String());
            AlterColumn("dbo.DI_M_AccountReconcilation", "AccountName", c => c.String());
            AlterColumn("dbo.DI_M_User", "Reserved5", c => c.String());
            AlterColumn("dbo.DI_M_User", "COEmployeeId", c => c.String());
            AlterColumn("dbo.DI_M_User", "ROEmployeeId", c => c.String());
            AlterColumn("dbo.DI_M_User", "UserName", c => c.String());
        }
    }
}
