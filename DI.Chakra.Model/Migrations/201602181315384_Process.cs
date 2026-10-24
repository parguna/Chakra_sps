namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Process : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DI_M_Process", "ProcessTypeKeyId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DI_M_Process", "ProcessTypeKeyId", c => c.Int(nullable: false));
        }
    }
}
