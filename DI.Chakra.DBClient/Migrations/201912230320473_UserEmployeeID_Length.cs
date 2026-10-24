namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserEmployeeID_Length : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DI_M_User", "EmployeeId", c => c.String(maxLength: 32));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DI_M_User", "EmployeeId", c => c.String());
        }
    }
}
