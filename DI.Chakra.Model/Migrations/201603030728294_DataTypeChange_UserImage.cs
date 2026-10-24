namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataTypeChange_UserImage : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DI_M_UserImage", "UserId", c => c.Long());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DI_M_UserImage", "UserId", c => c.Int());
        }
    }
}
