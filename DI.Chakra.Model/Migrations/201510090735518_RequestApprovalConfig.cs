namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequestApprovalConfig : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_RequestApprovalConfiguration",
                c => new
                    {
                        ConfigId = c.Short(nullable: false, identity: true),
                        RequestSource = c.Byte(nullable: false),
                        WorkflowConfigurationId = c.Short(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ConfigId)
                .ForeignKey("dbo.DI_M_WorkflowConfiguration", t => t.WorkflowConfigurationId, cascadeDelete: true)
                .Index(t => t.WorkflowConfigurationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_RequestApprovalConfiguration", "WorkflowConfigurationId", "dbo.DI_M_WorkflowConfiguration");
            DropIndex("dbo.DI_M_RequestApprovalConfiguration", new[] { "WorkflowConfigurationId" });
            DropTable("dbo.DI_M_RequestApprovalConfiguration");
        }
    }
}
