namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccessRequestSourceClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_AccessRequestSource",
                c => new
                    {
                        Id = c.Byte(nullable: false),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_M_AccessRequestSource");
        }
    }
}
