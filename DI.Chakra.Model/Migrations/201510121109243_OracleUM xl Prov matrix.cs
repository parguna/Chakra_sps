namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OracleUMxlProvmatrix : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.DI_M_ProvisioningFormOracle11g", newName: "DI_M_ProvisioningFormORCLUM");
            DropForeignKey("dbo.DI_M_RequestApprovalConfiguration", "WorkflowConfigurationId", "dbo.DI_M_WorkflowConfiguration");
            DropIndex("dbo.DI_M_RequestApprovalConfiguration", new[] { "WorkflowConfigurationId" });
            RenameColumn(table: "dbo.DI_M_ProvisioningFormORCLUM", name: "AccountNameLogicId", newName: "AccountNameConstructor_LogicId");
            RenameIndex(table: "dbo.DI_M_ProvisioningFormORCLUM", name: "IX_AccountNameLogicId", newName: "IX_AccountNameConstructor_LogicId");
            CreateTable(
                "dbo.DI_ORA_XLProvMatrix",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyCode = c.String(),
                        DepartmentCode = c.String(),
                        DivisionCode = c.String(),
                        OrganisationId = c.String(),
                        OrganisationName = c.String(),
                        Roles = c.String(),
                        CostCenterCode = c.String(),
                        CostDistributionCode = c.String(),
                        DefaultRoles = c.String(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
          //  AddColumn("dbo.DI_M_AccessRequest", "IsRequestedToRemove", c => c.Boolean());
            AddColumn("dbo.DI_M_ProvisioningForm", "EndpointSystem", c => c.Int());
            AddColumn("dbo.DI_M_RequestApprovalConfiguration", "IsApprovalRequired", c => c.Boolean(nullable: false));
            AlterColumn("dbo.DI_M_RequestApprovalConfiguration", "WorkflowConfigurationId", c => c.Short());
            CreateIndex("dbo.DI_M_RequestApprovalConfiguration", "WorkflowConfigurationId");
            AddForeignKey("dbo.DI_M_RequestApprovalConfiguration", "WorkflowConfigurationId", "dbo.DI_M_WorkflowConfiguration", "WorkflowConfigurationId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_RequestApprovalConfiguration", "WorkflowConfigurationId", "dbo.DI_M_WorkflowConfiguration");
            DropIndex("dbo.DI_M_RequestApprovalConfiguration", new[] { "WorkflowConfigurationId" });
            AlterColumn("dbo.DI_M_RequestApprovalConfiguration", "WorkflowConfigurationId", c => c.Short(nullable: false));
            DropColumn("dbo.DI_M_RequestApprovalConfiguration", "IsApprovalRequired");
            DropColumn("dbo.DI_M_ProvisioningForm", "EndpointSystem");
            DropColumn("dbo.DI_M_AccessRequest", "IsRequestedToRemove");
            DropTable("dbo.DI_ORA_XLProvMatrix");
            RenameIndex(table: "dbo.DI_M_ProvisioningFormORCLUM", name: "IX_AccountNameConstructor_LogicId", newName: "IX_AccountNameLogicId");
            RenameColumn(table: "dbo.DI_M_ProvisioningFormORCLUM", name: "AccountNameConstructor_LogicId", newName: "AccountNameLogicId");
            CreateIndex("dbo.DI_M_RequestApprovalConfiguration", "WorkflowConfigurationId");
            AddForeignKey("dbo.DI_M_RequestApprovalConfiguration", "WorkflowConfigurationId", "dbo.DI_M_WorkflowConfiguration", "WorkflowConfigurationId", cascadeDelete: true);
            RenameTable(name: "dbo.DI_M_ProvisioningFormORCLUM", newName: "DI_M_ProvisioningFormOracle11g");
        }
    }
}
