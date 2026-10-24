namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SAPMSSLAllMatrix : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_SAP_MSSL_XL_CriticalTCodeMatrix",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Transaction = c.String(),
                        TransactionDescription = c.String(),
                        TransactionOwner = c.String(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_MSSL_XL_PositionsMatrix",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Remarks = c.String(),
                        PositionText = c.String(),
                        Position_MSP = c.String(),
                        Position_MCP = c.String(),
                        Prefix_Employees = c.String(),
                        Prefix_TempStaff = c.String(),
                        User_Group = c.String(),
                        Department_Field = c.String(),
                        Level1Approver = c.String(maxLength: 250),
                        Level1ApproverType = c.Byte(nullable: false),
                        Level2Approver = c.String(maxLength: 250),
                        Level2ApproverType = c.Byte(nullable: false),
                        Level3Approver = c.String(maxLength: 250),
                        Level3ApproverType = c.Byte(nullable: false),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_MSSL_XL_RolesMatrix",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Remarks = c.String(),
                        Application = c.String(),
                        Module = c.String(),
                        SubModule = c.String(),
                        RoleName = c.String(),
                        TechnicalRole = c.String(),
                        Level1Approver = c.String(maxLength: 250),
                        Level1ApproverType = c.Byte(nullable: false),
                        Level2Approver = c.String(maxLength: 250),
                        Level2ApproverType = c.Byte(nullable: false),
                        Level3Approver = c.String(maxLength: 250),
                        Level3ApproverType = c.Byte(nullable: false),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_SAP_MSSL_XL_RolesMatrix");
            DropTable("dbo.DI_SAP_MSSL_XL_PositionsMatrix");
            DropTable("dbo.DI_SAP_MSSL_XL_CriticalTCodeMatrix");
        }
    }
}
