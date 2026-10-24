namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserExtensionSchema : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DI_M_User", "CompanyId", "dbo.DI_M_Company");
            DropForeignKey("dbo.DI_M_User", "DepartmentId", "dbo.DI_M_Department");
            DropForeignKey("dbo.DI_M_User", "DivisionId", "dbo.DI_M_Division");
            DropIndex("dbo.DI_M_User", new[] { "CompanyId" });
            DropIndex("dbo.DI_M_User", new[] { "DepartmentId" });
            DropIndex("dbo.DI_M_User", new[] { "DivisionId" });
            AddColumn("dbo.DI_M_User", "UserExtension", c => c.String());
            AlterColumn("dbo.DI_M_SODPolicyReport", "UserId", c => c.Long(nullable: false));
            DropColumn("dbo.DI_M_User", "EEGroup");
            DropColumn("dbo.DI_M_User", "EmployeeStatus");
            DropColumn("dbo.DI_M_User", "IsExecutiveEmployee");
            DropColumn("dbo.DI_M_User", "CompanyId");
            DropColumn("dbo.DI_M_User", "DepartmentId");
            DropColumn("dbo.DI_M_User", "DivisionId");
            DropColumn("dbo.DI_M_User", "CostCenter");
            DropColumn("dbo.DI_M_User", "CostCenterText");
            DropColumn("dbo.DI_M_User", "CostDistribution");
            DropColumn("dbo.DI_M_User", "CompanyCode");
            DropColumn("dbo.DI_M_User", "CompanyName");
            DropColumn("dbo.DI_M_User", "DepartmentCode");
            DropColumn("dbo.DI_M_User", "DepartmentName");
            DropColumn("dbo.DI_M_User", "DivisionCode");
            DropColumn("dbo.DI_M_User", "DivisionName");
            DropColumn("dbo.DI_M_User", "SectionCode");
            DropColumn("dbo.DI_M_User", "SectionName");
            DropColumn("dbo.DI_M_User", "PositionCode");
            DropColumn("dbo.DI_M_User", "PositionName");
            DropColumn("dbo.DI_M_User", "Supervisor");
            DropColumn("dbo.DI_M_User", "OrgCompany");
            DropColumn("dbo.DI_M_User", "OrgDepartment");
            DropColumn("dbo.DI_M_User", "OrgDivision");
            DropColumn("dbo.DI_M_User", "OrgBranch");
            DropColumn("dbo.DI_M_User", "OrgSection");
            DropColumn("dbo.DI_M_User", "OneKeyTokenId");
            DropColumn("dbo.DI_M_User", "FLINSLoginId");
            DropColumn("dbo.DI_M_User", "NeedEmailUpdateToHR");
            DropColumn("dbo.DI_M_User", "NeedFLINSLoginIdUpdateToHR");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_M_User", "NeedFLINSLoginIdUpdateToHR", c => c.Boolean(nullable: false));
            AddColumn("dbo.DI_M_User", "NeedEmailUpdateToHR", c => c.Boolean(nullable: false));
            AddColumn("dbo.DI_M_User", "FLINSLoginId", c => c.String());
            AddColumn("dbo.DI_M_User", "OneKeyTokenId", c => c.String());
            AddColumn("dbo.DI_M_User", "OrgSection", c => c.String());
            AddColumn("dbo.DI_M_User", "OrgBranch", c => c.String());
            AddColumn("dbo.DI_M_User", "OrgDivision", c => c.String());
            AddColumn("dbo.DI_M_User", "OrgDepartment", c => c.String());
            AddColumn("dbo.DI_M_User", "OrgCompany", c => c.String());
            AddColumn("dbo.DI_M_User", "Supervisor", c => c.String());
            AddColumn("dbo.DI_M_User", "PositionName", c => c.String());
            AddColumn("dbo.DI_M_User", "PositionCode", c => c.String());
            AddColumn("dbo.DI_M_User", "SectionName", c => c.String());
            AddColumn("dbo.DI_M_User", "SectionCode", c => c.String());
            AddColumn("dbo.DI_M_User", "DivisionName", c => c.String());
            AddColumn("dbo.DI_M_User", "DivisionCode", c => c.String());
            AddColumn("dbo.DI_M_User", "DepartmentName", c => c.String());
            AddColumn("dbo.DI_M_User", "DepartmentCode", c => c.String());
            AddColumn("dbo.DI_M_User", "CompanyName", c => c.String());
            AddColumn("dbo.DI_M_User", "CompanyCode", c => c.String());
            AddColumn("dbo.DI_M_User", "CostDistribution", c => c.String());
            AddColumn("dbo.DI_M_User", "CostCenterText", c => c.String());
            AddColumn("dbo.DI_M_User", "CostCenter", c => c.String());
            AddColumn("dbo.DI_M_User", "DivisionId", c => c.Long());
            AddColumn("dbo.DI_M_User", "DepartmentId", c => c.Long());
            AddColumn("dbo.DI_M_User", "CompanyId", c => c.Long());
            AddColumn("dbo.DI_M_User", "IsExecutiveEmployee", c => c.Boolean(nullable: false));
            AddColumn("dbo.DI_M_User", "EmployeeStatus", c => c.String());
            AddColumn("dbo.DI_M_User", "EEGroup", c => c.String());
            AlterColumn("dbo.DI_M_SODPolicyReport", "UserId", c => c.Int(nullable: false));
            DropColumn("dbo.DI_M_User", "UserExtension");
            CreateIndex("dbo.DI_M_User", "DivisionId");
            CreateIndex("dbo.DI_M_User", "DepartmentId");
            CreateIndex("dbo.DI_M_User", "CompanyId");
            AddForeignKey("dbo.DI_M_User", "DivisionId", "dbo.DI_M_Division", "Id");
            AddForeignKey("dbo.DI_M_User", "DepartmentId", "dbo.DI_M_Department", "Id");
            AddForeignKey("dbo.DI_M_User", "CompanyId", "dbo.DI_M_Company", "Id");
        }
    }
}
