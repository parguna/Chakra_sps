namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdColumnDatatypeChange_20 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DI_M_PasswordPolicyEndpoint", "PolicyId", "dbo.DI_M_PasswordPolicy");
            DropIndex("dbo.DI_M_PasswordPolicyEndpoint", new[] { "PolicyId" });
            DropPrimaryKey("dbo.DI_M_PasswordPolicy");
            DropPrimaryKey("dbo.DI_M_PasswordPolicyEndpoint");
            AlterColumn("dbo.DI_M_PasswordPolicy", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_PasswordPolicyEndpoint", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_PasswordPolicyEndpoint", "PolicyId", c => c.Long(nullable: false));
            AddPrimaryKey("dbo.DI_M_PasswordPolicy", "Id");
            AddPrimaryKey("dbo.DI_M_PasswordPolicyEndpoint", "Id");
            CreateIndex("dbo.DI_M_PasswordPolicyEndpoint", "PolicyId");
            AddForeignKey("dbo.DI_M_PasswordPolicyEndpoint", "PolicyId", "dbo.DI_M_PasswordPolicy", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_PasswordPolicyEndpoint", "PolicyId", "dbo.DI_M_PasswordPolicy");
            DropIndex("dbo.DI_M_PasswordPolicyEndpoint", new[] { "PolicyId" });
            DropPrimaryKey("dbo.DI_M_PasswordPolicyEndpoint");
            DropPrimaryKey("dbo.DI_M_PasswordPolicy");
            AlterColumn("dbo.DI_M_PasswordPolicyEndpoint", "PolicyId", c => c.Int(nullable: false));
            AlterColumn("dbo.DI_M_PasswordPolicyEndpoint", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_PasswordPolicy", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_PasswordPolicyEndpoint", "Id");
            AddPrimaryKey("dbo.DI_M_PasswordPolicy", "Id");
            CreateIndex("dbo.DI_M_PasswordPolicyEndpoint", "PolicyId");
            AddForeignKey("dbo.DI_M_PasswordPolicyEndpoint", "PolicyId", "dbo.DI_M_PasswordPolicy", "Id", cascadeDelete: true);
        }
    }
}
