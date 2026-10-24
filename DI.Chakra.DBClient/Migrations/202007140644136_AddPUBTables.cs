namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPUBTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_PUB_Employees",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        DIUserId = c.Long(),
                        PUBFileId = c.Long(),
                        EmployeeId = c.String(),
                        EBSUserId = c.String(),
                        DisplayName = c.String(),
                        CompanyName = c.String(),
                        ActualTerminationDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        NewTerminationDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        EmployeeEventTriggerId = c.Long(),
                        Status = c.String(),
                        ValidationMsg = c.String(),
                        Remarks = c.String(),
                        AdditionalInfo = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_PUB_File",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FileDateTime = c.DateTime(precision: 7, storeType: "datetime2"),
                        PUBFileStatus = c.String(),
                        PUBFileSource = c.String(),
                        PUBFileType = c.String(),
                        ValidationMsg = c.String(),
                        Remarks = c.String(),
                        AdditionalInfo = c.String(),
                        ReportedDateTime = c.DateTime(precision: 7, storeType: "datetime2"),
                        ReportedStatus = c.String(),
                        FileName = c.String(),
                        ReportFileName = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_PUB_File");
            DropTable("dbo.DI_PUB_Employees");
        }
    }
}
