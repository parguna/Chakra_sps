namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdColumnDatatypeChange_121 : DbMigration
    {
        public override void Up()
        {
            
            DropPrimaryKey("dbo.DI_M_ApprovalUserTypes");
            DropPrimaryKey("dbo.DI_M_EmployeeTerm");
            DropPrimaryKey("dbo.DI_M_RoleTypes");

          
            AlterColumn("dbo.DI_M_ApprovalUserTypes", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_EmployeeTerm", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_RoleTypes", "Id", c => c.Long(nullable: false, identity: true));
            
            
            AddPrimaryKey("dbo.DI_M_ApprovalUserTypes", "Id");
            AddPrimaryKey("dbo.DI_M_EmployeeTerm", "Id");
            AddPrimaryKey("dbo.DI_M_RoleTypes", "Id");

            
        }
        
        public override void Down()
        {
           
            DropPrimaryKey("dbo.DI_M_RoleTypes");
            DropPrimaryKey("dbo.DI_M_EmployeeTerm");
            DropPrimaryKey("dbo.DI_M_ApprovalUserTypes");
          
          
            AlterColumn("dbo.DI_M_RoleTypes", "Id", c => c.Byte(nullable: false));
            AlterColumn("dbo.DI_M_EmployeeTerm", "Id", c => c.Byte(nullable: false));
            AlterColumn("dbo.DI_M_ApprovalUserTypes", "Id", c => c.Byte(nullable: false));
           
            AddPrimaryKey("dbo.DI_M_RoleTypes", "Id");
            AddPrimaryKey("dbo.DI_M_EmployeeTerm", "Id");
            AddPrimaryKey("dbo.DI_M_ApprovalUserTypes", "Id");
           
        }
    }
}
