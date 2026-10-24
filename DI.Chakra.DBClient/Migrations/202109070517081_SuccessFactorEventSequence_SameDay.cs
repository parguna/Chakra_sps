namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SuccessFactorEventSequence_SameDay : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_EmployeeEventTrigger", "EventSequence", c => c.Int(nullable: false, defaultValue: 1));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_M_EmployeeEventTrigger", "EventSequence");
        }
    }
}
