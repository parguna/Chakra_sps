namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReferenceIdDataTypeChangeinServiceLog : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DI_Log_ServiceLog", "ReferenceId", c => c.Long());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DI_Log_ServiceLog", "ReferenceId", c => c.Int());
        }
    }
}
