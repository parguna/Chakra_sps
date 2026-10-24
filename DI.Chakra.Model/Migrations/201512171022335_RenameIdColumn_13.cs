namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameIdColumn_13 : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.DI_M_AccessRequestCategory", "AccessRequestTypeId", "Id");
            RenameColumn("dbo.DI_M_AccountActionCategory", "AccountActionTypeId", "Id");
            RenameColumn("dbo.DI_M_AccountCategory", "AccountTypeId", "Id");
            RenameColumn("dbo.DI_M_ApprovalUserTypes", "ApprovalUserTypeId", "Id");
            RenameColumn("dbo.DI_M_Designation", "BusinessRoleId", "Id");
            RenameColumn("dbo.DI_M_EmployeeTerm", "EmployeeTermId", "Id");
            //RenameColumn("dbo.DI_M_EndpointSystemType", "EndpointSystemTypeId", "Id");
            RenameColumn("dbo.DI_M_OffboardingAccountAction", "OffboardingAccountActionId", "Id");
            RenameColumn("dbo.DI_M_RequestTypes", "RequestTypeId", "Id");
            RenameColumn("dbo.DI_M_RoleTypes", "RoleTypeId", "Id");
            RenameColumn("dbo.DI_M_Company", "CompanyId", "Id");
            RenameColumn("dbo.DI_M_CostCenter", "CostCenterId", "Id");
            RenameColumn("dbo.DI_M_Department", "DepartmentId", "Id");
            RenameColumn("dbo.DI_M_Division", "DivisionId", "Id");
            RenameColumn("dbo.DI_M_Organisation", "OrganisationId", "Id");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.DI_M_AccessRequestCategory", "Id", "AccessRequestTypeId");
            RenameColumn("dbo.DI_M_AccountActionCategory", "Id", "AccountActionTypeId");
            RenameColumn("dbo.DI_M_AccountCategory", "Id", "AccountTypeId");
            RenameColumn("dbo.DI_M_ApprovalUserTypes", "Id", "ApprovalUserTypeId");
            RenameColumn("dbo.DI_M_Designation", "Id", "BusinessRoleId");
            RenameColumn("dbo.DI_M_EmployeeTerm", "Id", "EmployeeTermId");
            //RenameColumn("dbo.DI_M_EndpointSystemType", "Id", "EndpointSystemTypeId");
            RenameColumn("dbo.DI_M_OffboardingAccountAction", "Id", "OffboardingAccountActionId");
            RenameColumn("dbo.DI_M_RequestTypes", "Id", "RequestTypeId");
            RenameColumn("dbo.DI_M_RoleTypes", "Id", "RoleTypeId");
            RenameColumn("dbo.DI_M_Company", "Id", "CompanyId");
            RenameColumn("dbo.DI_M_CostCenter", "Id", "CostCenterId");
            RenameColumn("dbo.DI_M_Department", "Id", "DepartmentId");
            RenameColumn("dbo.DI_M_Division", "Id", "DivisionId");
            RenameColumn("dbo.DI_M_Organisation", "Id", "OrganisationId");
        }
    }
}
