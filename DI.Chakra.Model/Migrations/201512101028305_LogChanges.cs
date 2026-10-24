namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogChanges : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.DI_Log_Attestation");
            DropTable("dbo.DI_Log_Discovery");
            DropTable("dbo.DI_Log_Email");
            DropTable("dbo.DI_Log_Provisioning");
            DropTable("dbo.DI_LOG_Scheduler");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.DI_LOG_Scheduler",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LogLevel = c.String(),
                        JobType = c.String(),
                        NoOfJobs = c.Int(nullable: false),
                        Message = c.String(),
                        ExceptionMessage = c.String(),
                        StackTrace = c.String(),
                        ExceptionSource = c.String(),
                        InnerExceptionMessage = c.String(),
                        LogDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_Log_Provisioning",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccessRequestId = c.Long(),
                        LogLevel = c.String(),
                        EventType = c.Int(nullable: false),
                        EndpointSystemTypeName = c.String(),
                        EndpointSystemId = c.Int(nullable: false),
                        Message = c.String(),
                        ExceptionMessage = c.String(),
                        StackTrace = c.String(),
                        ExceptionSource = c.String(),
                        InnerExceptionMessage = c.String(),
                        LogDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_Log_Email",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmailId = c.Int(),
                        EmailSubject = c.String(),
                        LogLevel = c.String(),
                        Message = c.String(),
                        ExceptionMessage = c.String(),
                        StackTrace = c.String(),
                        ExceptionSource = c.String(),
                        InnerExceptionMessage = c.String(),
                        LogDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_Log_Discovery",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DiscoveryProcessId = c.Int(),
                        LogLevel = c.String(),
                        EndpointSystemId = c.Int(nullable: false),
                        EndpointSystemType = c.Int(nullable: false),
                        EndpointSystemName = c.String(),
                        Message = c.String(),
                        ExceptionMessage = c.String(),
                        StackTrace = c.String(),
                        ExceptionSource = c.String(),
                        InnerExceptionMessage = c.String(),
                        LogDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_Log_Attestation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AttestationId = c.Int(),
                        AttestationName = c.String(),
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
    }
}
