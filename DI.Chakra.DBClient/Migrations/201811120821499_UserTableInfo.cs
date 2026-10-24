namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserTableInfo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_UserTableInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TableName = c.String(),
                        Columns = c.String(),
                        Caption = c.String(),
                        ColumnType = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_M_UserTableInfo");
        }
    }
}
