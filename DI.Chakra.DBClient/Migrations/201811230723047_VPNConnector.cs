namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VPNConnector : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_VPN_ApplicationEnviornment",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        MatrixId = c.Long(nullable: false),
                        MatrixType = c.Int(nullable: false),
                        EnviornmentType = c.Int(nullable: false),
                        ADGroups = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_VPN_Matrix",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        ProductionADgroups = c.String(),
                        QAADGroups = c.String(),
                        DevelpomentADGroups = c.String(),
                        ADGroups = c.String(),
                        VPNMatrixType = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Level1Approver = c.String(),
                        Level1ApproverType = c.Byte(nullable: false),
                        Level2Approver = c.String(),
                        Level2ApproverType = c.Byte(nullable: false),
                        Level3Approver = c.String(),
                        Level3ApproverType = c.Byte(nullable: false),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.VPNUserApplicationEnviornment",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointUserId = c.Long(nullable: false),
                        MatrixId = c.Long(nullable: false),
                        Name = c.String(nullable: false),
                        MatrixType = c.Int(nullable: false),
                        EnviornmentType = c.Int(nullable: false),
                        ADGroups = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_VPN_Matrix", t => t.MatrixId)
                .Index(t => t.MatrixId);
            
            CreateTable(
                "dbo.DI_VPN_UserInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OneKeyTokenId = c.String(),
                        UserName = c.String(),
                        UserId = c.Long(nullable: false),
                        DisplayName = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        EndPointSystemId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            DropTable("dbo.DI_M_UserTableInfo");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.DI_M_UserTableInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TableName = c.String(),
                        Columns = c.String(),
                        Caption = c.String(),
                        ColumnType = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.DI_VPN_UserInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.VPNUserApplicationEnviornment", "MatrixId", "dbo.DI_VPN_Matrix");
            DropIndex("dbo.DI_VPN_UserInfo", new[] { "EndPointSystemId" });
            DropIndex("dbo.VPNUserApplicationEnviornment", new[] { "MatrixId" });
            DropTable("dbo.DI_VPN_UserInfo");
            DropTable("dbo.VPNUserApplicationEnviornment");
            DropTable("dbo.DI_VPN_Matrix");
            DropTable("dbo.DI_VPN_ApplicationEnviornment");
        }
    }
}
