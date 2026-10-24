namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedActionDateInTask : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_Task", "ActionDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_M_Task", "ActionDate");
        }
    }
}
