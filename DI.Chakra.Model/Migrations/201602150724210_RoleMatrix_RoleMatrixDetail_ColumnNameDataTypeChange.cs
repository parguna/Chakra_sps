namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoleMatrix_RoleMatrixDetail_ColumnNameDataTypeChange : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.DI_M_RoleMatrixDetail", "RoleMatrixDetailId", "Id");

            AlterColumn("dbo.DI_M_RoleMatrix", "CostCenterId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            RenameColumn("dbo.DI_M_RoleMatrixDetail", "Id", "RoleMatrixDetailId");

            AlterColumn("dbo.DI_M_RoleMatrix", "CostCenterId", c => c.Int(nullable: false));
        }
    }
}
