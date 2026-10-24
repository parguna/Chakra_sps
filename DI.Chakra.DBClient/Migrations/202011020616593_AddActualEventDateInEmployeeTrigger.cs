namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActualEventDateInEmployeeTrigger : DbMigration
    {
        public override void Up()
        {
 
            AddColumn("dbo.DI_M_EmployeeEventTrigger", "ActualEventDate", c => c.DateTime());
        }
        
        public override void Down()
        {
          
            DropColumn("dbo.DI_M_EmployeeEventTrigger", "ActualEventDate"); 
        }
    }
}
