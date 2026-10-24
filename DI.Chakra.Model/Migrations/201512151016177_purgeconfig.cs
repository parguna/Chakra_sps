namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class purgeconfig : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_PurgeConfig",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TableName = c.String(),
                        ObjectName = c.String(),
                        Description = c.String(),
                        DaysToKeep = c.Int(nullable: false),
                        Status = c.String(),
                        PurgeMode = c.Byte(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        LastRunOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_M_PurgeConfig");
        }
    }
}
