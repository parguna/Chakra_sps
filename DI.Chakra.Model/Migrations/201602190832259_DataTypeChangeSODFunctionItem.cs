namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataTypeChangeSODFunctionItem : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DI_M_SODFunctionItem", "SodFunctionId", c => c.Long());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DI_M_SODFunctionItem", "SodFunctionId", c => c.Int());
        }
    }
}
