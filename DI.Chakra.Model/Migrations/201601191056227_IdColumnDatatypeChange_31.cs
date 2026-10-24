namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdColumnDatatypeChange_31 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DI_M_SODRunPolicySelection", "SODRunPolicyId", "dbo.DI_M_SODRunPolicy");
            Sql("ALTER TABLE [dbo].[DI_M_SODException] drop CONSTRAINT [FK_dbo.DI_M_SODException_dbo.DI_M_SODRunPolicy_SODRunPolicy_SODRunPolicyId]");
            Sql("DROP INDEX [dbo].[DI_M_SODException].[IX_SODRunPolicy_SODRunPolicyId]");
            Sql("ALTER TABLE DI_M_SODException DROP COLUMN SODRunPolicy_Id");
            //DropForeignKey("dbo.DI_M_SODException", "SODRunPolicy_Id", "dbo.DI_M_SODRunPolicy");
            DropForeignKey("dbo.DI_M_SODReport", "SODRunPolicyId", "dbo.DI_M_SODRunPolicy");
           // DropIndex("dbo.DI_M_SODException", new[] { "SODRunPolicy_Id" });
            DropIndex("dbo.DI_M_SODReport", new[] { "SODRunPolicyId" });
            DropIndex("dbo.DI_M_SODRunPolicySelection", new[] { "SODRunPolicyId" });
            DropPrimaryKey("dbo.DI_M_SODReport");
            DropPrimaryKey("dbo.DI_M_SODRunPolicy");
            DropPrimaryKey("dbo.DI_M_SODRunPolicySelection");
            DropPrimaryKey("dbo.DI_M_SODExecutionConfig");
            DropPrimaryKey("dbo.DI_M_SODPolicy");
            DropPrimaryKey("dbo.DI_M_SODPolicyReport");
            //AlterColumn("dbo.DI_M_SODException", "SODRunPolicy_Id", c => c.Long());
            AlterColumn("dbo.DI_M_SODReport", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_SODReport", "SODRunPolicyId", c => c.Long());
            AlterColumn("dbo.DI_M_SODRunPolicy", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_SODRunPolicySelection", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_SODRunPolicySelection", "SODRunPolicyId", c => c.Long(nullable: false));
            AlterColumn("dbo.DI_M_SODExecutionConfig", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_SODPolicy", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_SODPolicyReport", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_SODReport", "Id");
            AddPrimaryKey("dbo.DI_M_SODRunPolicy", "Id");
            AddPrimaryKey("dbo.DI_M_SODRunPolicySelection", "Id");
            AddPrimaryKey("dbo.DI_M_SODExecutionConfig", "Id");
            AddPrimaryKey("dbo.DI_M_SODPolicy", "Id");
            AddPrimaryKey("dbo.DI_M_SODPolicyReport", "Id");
            //CreateIndex("dbo.DI_M_SODException", "SODRunPolicy_Id");
            CreateIndex("dbo.DI_M_SODReport", "SODRunPolicyId");
            CreateIndex("dbo.DI_M_SODRunPolicySelection", "SODRunPolicyId");
            AddForeignKey("dbo.DI_M_SODRunPolicySelection", "SODRunPolicyId", "dbo.DI_M_SODRunPolicy", "Id", cascadeDelete: true);
            //AddForeignKey("dbo.DI_M_SODException", "SODRunPolicy_Id", "dbo.DI_M_SODRunPolicy", "Id");
            AddForeignKey("dbo.DI_M_SODReport", "SODRunPolicyId", "dbo.DI_M_SODRunPolicy", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_SODReport", "SODRunPolicyId", "dbo.DI_M_SODRunPolicy");
           // DropForeignKey("dbo.DI_M_SODException", "SODRunPolicy_Id", "dbo.DI_M_SODRunPolicy");
            DropForeignKey("dbo.DI_M_SODRunPolicySelection", "SODRunPolicyId", "dbo.DI_M_SODRunPolicy");
            DropIndex("dbo.DI_M_SODRunPolicySelection", new[] { "SODRunPolicyId" });
            DropIndex("dbo.DI_M_SODReport", new[] { "SODRunPolicyId" });
            //DropIndex("dbo.DI_M_SODException", new[] { "SODRunPolicy_Id" });
            DropPrimaryKey("dbo.DI_M_SODPolicyReport");
            DropPrimaryKey("dbo.DI_M_SODPolicy");
            DropPrimaryKey("dbo.DI_M_SODExecutionConfig");
            DropPrimaryKey("dbo.DI_M_SODRunPolicySelection");
            DropPrimaryKey("dbo.DI_M_SODRunPolicy");
            DropPrimaryKey("dbo.DI_M_SODReport");
            AlterColumn("dbo.DI_M_SODPolicyReport", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_SODPolicy", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_SODExecutionConfig", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_SODRunPolicySelection", "SODRunPolicyId", c => c.Int(nullable: false));
            AlterColumn("dbo.DI_M_SODRunPolicySelection", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_SODRunPolicy", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_SODReport", "SODRunPolicyId", c => c.Int());
            AlterColumn("dbo.DI_M_SODReport", "Id", c => c.Int(nullable: false, identity: true));
            //AlterColumn("dbo.DI_M_SODException", "SODRunPolicy_Id", c => c.Int());
            AddPrimaryKey("dbo.DI_M_SODPolicyReport", "Id");
            AddPrimaryKey("dbo.DI_M_SODPolicy", "Id");
            AddPrimaryKey("dbo.DI_M_SODExecutionConfig", "Id");
            AddPrimaryKey("dbo.DI_M_SODRunPolicySelection", "Id");
            AddPrimaryKey("dbo.DI_M_SODRunPolicy", "Id");
            AddPrimaryKey("dbo.DI_M_SODReport", "Id");
            CreateIndex("dbo.DI_M_SODRunPolicySelection", "SODRunPolicyId");
            CreateIndex("dbo.DI_M_SODReport", "SODRunPolicyId");
            //CreateIndex("dbo.DI_M_SODException", "SODRunPolicy_Id");
            AddForeignKey("dbo.DI_M_SODReport", "SODRunPolicyId", "dbo.DI_M_SODRunPolicy", "Id");
            //AddForeignKey("dbo.DI_M_SODException", "SODRunPolicy_Id", "dbo.DI_M_SODRunPolicy", "Id");
            AddForeignKey("dbo.DI_M_SODRunPolicySelection", "SODRunPolicyId", "dbo.DI_M_SODRunPolicy", "Id", cascadeDelete: true);
        }
    }
}
