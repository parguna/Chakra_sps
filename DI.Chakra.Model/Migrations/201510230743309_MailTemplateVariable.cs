namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MailTemplateVariable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_MailTemplateVariable",
                c => new
                    {
                        VariableId = c.Int(nullable: false, identity: true),
                        DisplayName = c.String(),
                        VariableValue = c.String(),
                        VariableValuePath = c.String(),
                        VariableScope = c.Byte(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.VariableId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_M_MailTemplateVariable");
        }
    }
}
