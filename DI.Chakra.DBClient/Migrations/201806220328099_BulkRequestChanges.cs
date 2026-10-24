namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BulkRequestChanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_BulkRequestDetail",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BulkRequestId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Name = c.String(),
                        UserName = c.String(),
                        EmployeeId = c.String(),
                        EmailId = c.String(),
                        Designation = c.String(),
                        OrgCompany = c.String(),
                        OrgDepartment = c.String(),
                        OrgDivision = c.String(),
                        OrgBranch = c.String(),
                        OrgSection = c.String(),
                        OrgUnit = c.String(),
                        OneKey = c.String(),
                        AccountName = c.String(),
                        EndpointUserId = c.Int(),
                        BulkRequest_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_BulkRequest", t => t.BulkRequest_Id)
                .Index(t => t.BulkRequest_Id);
            
            CreateTable(
                "dbo.DI_M_BulkRequest",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RequestedBy = c.Long(nullable: false),
                        EndpointType = c.Int(nullable: false),
                        EndpointSystemId = c.Long(nullable: false),
                        Description = c.String(),
                        AdhocActionType = c.Byte(nullable: false),
                        ApplicationType = c.Byte(nullable: false),
                        EntitlementDetail = c.String(),
                        IsEmployee = c.Boolean(nullable: false),
                        ActualSupervisor = c.Long(),
                        SelectedSupervisor = c.Long(),
                        SupervisorChanged = c.Boolean(nullable: false),
                        RequestStatus = c.Long(nullable: false),
                        ProgressStatus = c.Long(nullable: false),
                        RequestSource = c.Long(nullable: false),
                        ActivationDate = c.DateTime(),
                        DeactivationDate = c.DateTime(),
                        ExecutionTime = c.DateTime(),
                        Remarks = c.String(),
                        AdditionalInfo = c.String(),
                        Comments = c.String(),
                        IsCurrentLevelApproved = c.Boolean(nullable: false),
                        IsRequestedToRemove = c.Boolean(nullable: false),
                        ApproverType = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystemId)
                .Index(t => t.EndpointSystemId);
            
            AddColumn("dbo.DI_M_Task", "BulkRequestId", c => c.Long());
            AddColumn("dbo.DI_M_Task", "EmployeeEventTriggerId", c => c.Long());
            CreateIndex("dbo.DI_M_Task", "BulkRequestId");
            CreateIndex("dbo.DI_M_Task", "EmployeeEventTriggerId");
            AddForeignKey("dbo.DI_M_Task", "BulkRequestId", "dbo.DI_M_BulkRequest", "Id");
            AddForeignKey("dbo.DI_M_Task", "EmployeeEventTriggerId", "dbo.DI_M_EmployeeEventTrigger", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_Task", "EmployeeEventTriggerId", "dbo.DI_M_EmployeeEventTrigger");
            DropForeignKey("dbo.DI_M_Task", "BulkRequestId", "dbo.DI_M_BulkRequest");
            DropForeignKey("dbo.DI_M_BulkRequest", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_BulkRequestDetail", "BulkRequest_Id", "dbo.DI_M_BulkRequest");
            DropIndex("dbo.DI_M_Task", new[] { "EmployeeEventTriggerId" });
            DropIndex("dbo.DI_M_Task", new[] { "BulkRequestId" });
            DropIndex("dbo.DI_M_BulkRequest", new[] { "EndpointSystemId" });
            DropIndex("dbo.DI_M_BulkRequestDetail", new[] { "BulkRequest_Id" });
            DropColumn("dbo.DI_M_Task", "EmployeeEventTriggerId");
            DropColumn("dbo.DI_M_Task", "BulkRequestId");
            DropTable("dbo.DI_M_BulkRequest");
            DropTable("dbo.DI_M_BulkRequestDetail");
        }
    }
}
