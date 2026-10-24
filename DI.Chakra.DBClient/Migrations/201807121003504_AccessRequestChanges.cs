namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccessRequestChanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_EventUndoRequest",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointType = c.Int(nullable: false),
                        EndpointSystemId = c.Long(nullable: false),
                        EndpointUserId = c.Long(nullable: false),
                        EmployeeEventType = c.Byte(nullable: false),
                        TechnicalRoles = c.String(),
                        TechnicalGroups = c.String(),
                        TechnicalPositions = c.String(),
                        AdditionalInfo = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystemId)
                .Index(t => t.EndpointSystemId);
            
            AddColumn("dbo.DI_M_AccessRequest", "OrgCompany", c => c.String());
            AddColumn("dbo.DI_M_AccessRequest", "OrgDepartment", c => c.String());
            AddColumn("dbo.DI_M_AccessRequest", "OrgDivision", c => c.String());
            AddColumn("dbo.DI_M_AccessRequest", "OrgBranch", c => c.String());
            AddColumn("dbo.DI_M_AccessRequest", "OrgSection", c => c.String());
            AddColumn("dbo.DI_M_AccessRequest", "OrgUnit", c => c.String());
            CreateIndex("dbo.DI_M_AccessRequest", "EventUndoRequestId");
            AddForeignKey("dbo.DI_M_AccessRequest", "EventUndoRequestId", "dbo.DI_M_EventUndoRequest", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_AccessRequest", "EventUndoRequestId", "dbo.DI_M_EventUndoRequest");
            DropForeignKey("dbo.DI_M_EventUndoRequest", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropIndex("dbo.DI_M_EventUndoRequest", new[] { "EndpointSystemId" });
            DropIndex("dbo.DI_M_AccessRequest", new[] { "EventUndoRequestId" });
            DropColumn("dbo.DI_M_AccessRequest", "OrgUnit");
            DropColumn("dbo.DI_M_AccessRequest", "OrgSection");
            DropColumn("dbo.DI_M_AccessRequest", "OrgBranch");
            DropColumn("dbo.DI_M_AccessRequest", "OrgDivision");
            DropColumn("dbo.DI_M_AccessRequest", "OrgDepartment");
            DropColumn("dbo.DI_M_AccessRequest", "OrgCompany");
            DropTable("dbo.DI_M_EventUndoRequest");
        }
    }
}
