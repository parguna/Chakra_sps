namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class M2 : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.DI_Log_ServiceLog");
            CreateTable(
                "dbo.DI_Log_ServiceLog",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReferenceId = c.Int(),
                        ReferenceType = c.Int(nullable: false),
                        LogLevel = c.String(),
                        Message = c.String(),
                        ExceptionMessage = c.String(),
                        StackTrace = c.String(),
                        ExceptionSource = c.String(),
                        InnerExceptionMessage = c.String(),
                        LogDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_Log_ServiceLog");
        }
    }
}
