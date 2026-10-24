namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class provformchanges1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DI_M_ProvisioningForm", "ProvisioningConfiguration_Id", "dbo.DI_M_ProvisioningConfiguration");
            DropForeignKey("dbo.DI_M_ProvisioningForm", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropIndex("dbo.DI_M_ProvisioningForm", new[] { "EndpointSystemId" });
            DropIndex("dbo.DI_M_ProvisioningForm", new[] { "ProvisioningConfiguration_Id" });
            CreateTable(
                "dbo.DI_M_ProvisioningNameConstructor",
                c => new
                    {
                        LogicId = c.Int(nullable: false, identity: true),
                        BusinessLogicSource = c.Byte(nullable: false),
                        BusinessLogicId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LogicId);
            
            AddColumn("dbo.DI_M_ProvisioningFormAD", "AccountNameLogicId", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningFormLINUX", "AccountNameLogicId", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningFormLNS", "ShortNameLogicId", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningFormLNS", "LongNameLogicId", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningFormLNS", "MailFileLogicId", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningFormLNS", "UserIdFileLogicId", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningFormLNS", "InternetAddressNameLogicId", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningFormOracle11g", "AccountNameLogicId", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningFormOracleEBS", "AccountNameLogicId", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningFormSAPEP", "AccountNameLogicId", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningFormSAP", "AccountNameLogicId", c => c.Int());
            AlterColumn("dbo.DI_M_ProvisioningForm", "EndpointSystemId", c => c.Int(nullable: false));
            CreateIndex("dbo.DI_M_ProvisioningForm", "EndpointSystemId");
            CreateIndex("dbo.DI_M_ProvisioningFormAD", "AccountNameLogicId");
            CreateIndex("dbo.DI_M_ProvisioningFormLINUX", "AccountNameLogicId");
            CreateIndex("dbo.DI_M_ProvisioningFormLNS", "ShortNameLogicId");
            CreateIndex("dbo.DI_M_ProvisioningFormLNS", "LongNameLogicId");
            CreateIndex("dbo.DI_M_ProvisioningFormLNS", "MailFileLogicId");
            CreateIndex("dbo.DI_M_ProvisioningFormLNS", "UserIdFileLogicId");
            CreateIndex("dbo.DI_M_ProvisioningFormLNS", "InternetAddressNameLogicId");
            CreateIndex("dbo.DI_M_ProvisioningFormOracle11g", "AccountNameLogicId");
            CreateIndex("dbo.DI_M_ProvisioningFormOracleEBS", "AccountNameLogicId");
            CreateIndex("dbo.DI_M_ProvisioningFormSAPEP", "AccountNameLogicId");
            CreateIndex("dbo.DI_M_ProvisioningFormSAP", "AccountNameLogicId");
            AddForeignKey("dbo.DI_M_ProvisioningFormAD", "AccountNameLogicId", "dbo.DI_M_ProvisioningNameConstructor", "LogicId");
            AddForeignKey("dbo.DI_M_ProvisioningFormLINUX", "AccountNameLogicId", "dbo.DI_M_ProvisioningNameConstructor", "LogicId");
            AddForeignKey("dbo.DI_M_ProvisioningFormLNS", "InternetAddressNameLogicId", "dbo.DI_M_ProvisioningNameConstructor", "LogicId");
            AddForeignKey("dbo.DI_M_ProvisioningFormLNS", "LongNameLogicId", "dbo.DI_M_ProvisioningNameConstructor", "LogicId");
            AddForeignKey("dbo.DI_M_ProvisioningFormLNS", "MailFileLogicId", "dbo.DI_M_ProvisioningNameConstructor", "LogicId");
            AddForeignKey("dbo.DI_M_ProvisioningFormLNS", "ShortNameLogicId", "dbo.DI_M_ProvisioningNameConstructor", "LogicId");
            AddForeignKey("dbo.DI_M_ProvisioningFormLNS", "UserIdFileLogicId", "dbo.DI_M_ProvisioningNameConstructor", "LogicId");
            AddForeignKey("dbo.DI_M_ProvisioningFormOracle11g", "AccountNameLogicId", "dbo.DI_M_ProvisioningNameConstructor", "LogicId");
            AddForeignKey("dbo.DI_M_ProvisioningFormOracleEBS", "AccountNameLogicId", "dbo.DI_M_ProvisioningNameConstructor", "LogicId");
            AddForeignKey("dbo.DI_M_ProvisioningFormSAPEP", "AccountNameLogicId", "dbo.DI_M_ProvisioningNameConstructor", "LogicId");
            AddForeignKey("dbo.DI_M_ProvisioningFormSAP", "AccountNameLogicId", "dbo.DI_M_ProvisioningNameConstructor", "LogicId");
            AddForeignKey("dbo.DI_M_ProvisioningForm", "EndpointSystemId", "dbo.DI_M_EndpointSystem", "EndpointSystemId", cascadeDelete: true);
            DropColumn("dbo.DI_M_ProvisioningForm", "EndpointSystemType");
            DropColumn("dbo.DI_M_ProvisioningForm", "DependentOnFormId");
            DropColumn("dbo.DI_M_ProvisioningForm", "FormOwnerUserId");
            DropColumn("dbo.DI_M_ProvisioningForm", "PostActivityList");
            DropColumn("dbo.DI_M_ProvisioningForm", "Extra");
            DropColumn("dbo.DI_M_ProvisioningForm", "ProvisioningConfiguration_Id");
            DropTable("dbo.DI_M_ProvisioningConfiguration");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.DI_M_ProvisioningConfiguration",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserNameSource = c.Byte(nullable: false),
                        UserNameGeneratorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.DI_M_ProvisioningForm", "ProvisioningConfiguration_Id", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningForm", "Extra", c => c.String());
            AddColumn("dbo.DI_M_ProvisioningForm", "PostActivityList", c => c.String());
            AddColumn("dbo.DI_M_ProvisioningForm", "FormOwnerUserId", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningForm", "DependentOnFormId", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningForm", "EndpointSystemType", c => c.Int());
            DropForeignKey("dbo.DI_M_ProvisioningForm", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_ProvisioningFormSAP", "AccountNameLogicId", "dbo.DI_M_ProvisioningNameConstructor");
            DropForeignKey("dbo.DI_M_ProvisioningFormSAPEP", "AccountNameLogicId", "dbo.DI_M_ProvisioningNameConstructor");
            DropForeignKey("dbo.DI_M_ProvisioningFormOracleEBS", "AccountNameLogicId", "dbo.DI_M_ProvisioningNameConstructor");
            DropForeignKey("dbo.DI_M_ProvisioningFormOracle11g", "AccountNameLogicId", "dbo.DI_M_ProvisioningNameConstructor");
            DropForeignKey("dbo.DI_M_ProvisioningFormLNS", "UserIdFileLogicId", "dbo.DI_M_ProvisioningNameConstructor");
            DropForeignKey("dbo.DI_M_ProvisioningFormLNS", "ShortNameLogicId", "dbo.DI_M_ProvisioningNameConstructor");
            DropForeignKey("dbo.DI_M_ProvisioningFormLNS", "MailFileLogicId", "dbo.DI_M_ProvisioningNameConstructor");
            DropForeignKey("dbo.DI_M_ProvisioningFormLNS", "LongNameLogicId", "dbo.DI_M_ProvisioningNameConstructor");
            DropForeignKey("dbo.DI_M_ProvisioningFormLNS", "InternetAddressNameLogicId", "dbo.DI_M_ProvisioningNameConstructor");
            DropForeignKey("dbo.DI_M_ProvisioningFormLINUX", "AccountNameLogicId", "dbo.DI_M_ProvisioningNameConstructor");
            DropForeignKey("dbo.DI_M_ProvisioningFormAD", "AccountNameLogicId", "dbo.DI_M_ProvisioningNameConstructor");
            DropIndex("dbo.DI_M_ProvisioningFormSAP", new[] { "AccountNameLogicId" });
            DropIndex("dbo.DI_M_ProvisioningFormSAPEP", new[] { "AccountNameLogicId" });
            DropIndex("dbo.DI_M_ProvisioningFormOracleEBS", new[] { "AccountNameLogicId" });
            DropIndex("dbo.DI_M_ProvisioningFormOracle11g", new[] { "AccountNameLogicId" });
            DropIndex("dbo.DI_M_ProvisioningFormLNS", new[] { "InternetAddressNameLogicId" });
            DropIndex("dbo.DI_M_ProvisioningFormLNS", new[] { "UserIdFileLogicId" });
            DropIndex("dbo.DI_M_ProvisioningFormLNS", new[] { "MailFileLogicId" });
            DropIndex("dbo.DI_M_ProvisioningFormLNS", new[] { "LongNameLogicId" });
            DropIndex("dbo.DI_M_ProvisioningFormLNS", new[] { "ShortNameLogicId" });
            DropIndex("dbo.DI_M_ProvisioningFormLINUX", new[] { "AccountNameLogicId" });
            DropIndex("dbo.DI_M_ProvisioningFormAD", new[] { "AccountNameLogicId" });
            DropIndex("dbo.DI_M_ProvisioningForm", new[] { "EndpointSystemId" });
            AlterColumn("dbo.DI_M_ProvisioningForm", "EndpointSystemId", c => c.Int());
            DropColumn("dbo.DI_M_ProvisioningFormSAP", "AccountNameLogicId");
            DropColumn("dbo.DI_M_ProvisioningFormSAPEP", "AccountNameLogicId");
            DropColumn("dbo.DI_M_ProvisioningFormOracleEBS", "AccountNameLogicId");
            DropColumn("dbo.DI_M_ProvisioningFormOracle11g", "AccountNameLogicId");
            DropColumn("dbo.DI_M_ProvisioningFormLNS", "InternetAddressNameLogicId");
            DropColumn("dbo.DI_M_ProvisioningFormLNS", "UserIdFileLogicId");
            DropColumn("dbo.DI_M_ProvisioningFormLNS", "MailFileLogicId");
            DropColumn("dbo.DI_M_ProvisioningFormLNS", "LongNameLogicId");
            DropColumn("dbo.DI_M_ProvisioningFormLNS", "ShortNameLogicId");
            DropColumn("dbo.DI_M_ProvisioningFormLINUX", "AccountNameLogicId");
            DropColumn("dbo.DI_M_ProvisioningFormAD", "AccountNameLogicId");
            DropTable("dbo.DI_M_ProvisioningNameConstructor");
            CreateIndex("dbo.DI_M_ProvisioningForm", "ProvisioningConfiguration_Id");
            CreateIndex("dbo.DI_M_ProvisioningForm", "EndpointSystemId");
            AddForeignKey("dbo.DI_M_ProvisioningForm", "EndpointSystemId", "dbo.DI_M_EndpointSystem", "EndpointSystemId");
            AddForeignKey("dbo.DI_M_ProvisioningForm", "ProvisioningConfiguration_Id", "dbo.DI_M_ProvisioningConfiguration", "Id");
        }
    }
}
