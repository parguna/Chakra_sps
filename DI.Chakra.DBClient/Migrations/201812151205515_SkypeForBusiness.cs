namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SkypeForBusiness : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_SFB_ResponseGroupInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OwnerPool = c.String(),
                        AgentsByUri = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        ParticipationPolicy = c.String(),
                        AgentAlertTime = c.String(),
                        RoutingMethod = c.String(),
                        DistributionGroupAddress = c.String(),
                        Identity = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        EndPointSystemId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_SFB_UserInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SamAccountName = c.String(),
                        UserPrincipalName = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        WindowsEmailAddress = c.String(),
                        Sid = c.String(),
                        LineServerURI = c.String(),
                        OriginatorSid = c.String(),
                        AudioVideoDisabled = c.String(),
                        IPPBXSoftPhoneRoutingEnabled = c.String(),
                        RemoteCallControlTelephonyEnabled = c.String(),
                        PrivateLine = c.String(),
                        AcpInfo = c.String(),
                        HostedVoiceMail = c.String(),
                        DisplayName = c.String(),
                        ProxyAddresses = c.String(),
                        HomeServer = c.String(),
                        TargetServerIfMoving = c.String(),
                        EnabledForFederation = c.String(),
                        EnabledForInternetAccess = c.String(),
                        PublicNetworkEnabled = c.String(),
                        EnterpriseVoiceEnabled = c.String(),
                        EnabledForRichPresence = c.String(),
                        ExchangeArchivingPolicy = c.String(),
                        LineURI = c.String(),
                        SipAddress = c.String(),
                        Enabled = c.String(),
                        TenantId = c.String(),
                        UserRoutingGroupId = c.String(),
                        TargetRegistrarPool = c.String(),
                        VoicePolicy = c.String(),
                        MobilityPolicy = c.String(),
                        ConferencingPolicy = c.String(),
                        PresencePolicy = c.String(),
                        VoiceRoutingPolicy = c.String(),
                        RegistrarPool = c.String(),
                        DialPlan = c.String(),
                        LocationPolicy = c.String(),
                        ClientPolicy = c.String(),
                        ClientVersionPolicy = c.String(),
                        ArchivingPolicy = c.String(),
                        LegalInterceptPolicy = c.String(),
                        PinPolicy = c.String(),
                        CallViaWorkPolicy = c.String(),
                        GraphPolicy = c.String(),
                        ExternalAccessPolicy = c.String(),
                        HostedVoicemailPolicy = c.String(),
                        PersistentChatPolicy = c.String(),
                        UserServicesPolicy = c.String(),
                        ExperiencePolicy = c.String(),
                        AddressBookPolicy = c.String(),
                        ThirdPartyVideoSystemPolicy = c.String(),
                        HostingProvider = c.String(),
                        ObjectId = c.String(),
                        ExUmEnabled = c.String(),
                        Name = c.String(),
                        DistinguishedName = c.String(),
                        Identity = c.String(),
                        Guid = c.String(),
                        ObjectCategory = c.String(),
                        ObjectClass = c.String(),
                        WhenChanged = c.DateTime(),
                        WhenCreated = c.DateTime(),
                        OriginatingServer = c.String(),
                        IsByPassValidation = c.String(),
                        IsValid = c.String(),
                        ObjectState = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        EndPointSystemId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.SkypeForBusinessUserInfoSkypeForBusinessResponseGroupInfo",
                c => new
                    {
                        SkypeForBusinessUserInfo_Id = c.Long(nullable: false),
                        SkypeForBusinessResponseGroupInfo_Id = c.Long(nullable: false),
                        EndpointSystemId = c.Long(nullable: true),
                    })
                .PrimaryKey(t => new { t.SkypeForBusinessUserInfo_Id, t.SkypeForBusinessResponseGroupInfo_Id })
                .ForeignKey("dbo.DI_SFB_UserInfo", t => t.SkypeForBusinessUserInfo_Id)
                .ForeignKey("dbo.DI_SFB_ResponseGroupInfo", t => t.SkypeForBusinessResponseGroupInfo_Id)
                .Index(t => t.SkypeForBusinessUserInfo_Id)
                .Index(t => t.SkypeForBusinessResponseGroupInfo_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SkypeForBusinessUserInfoSkypeForBusinessResponseGroupInfo", "SkypeForBusinessResponseGroupInfo_Id", "dbo.DI_SFB_ResponseGroupInfo");
            DropForeignKey("dbo.SkypeForBusinessUserInfoSkypeForBusinessResponseGroupInfo", "SkypeForBusinessUserInfo_Id", "dbo.DI_SFB_UserInfo");
            DropForeignKey("dbo.DI_SFB_UserInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_SFB_ResponseGroupInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropIndex("dbo.SkypeForBusinessUserInfoSkypeForBusinessResponseGroupInfo", new[] { "SkypeForBusinessResponseGroupInfo_Id" });
            DropIndex("dbo.SkypeForBusinessUserInfoSkypeForBusinessResponseGroupInfo", new[] { "SkypeForBusinessUserInfo_Id" });
            DropIndex("dbo.DI_SFB_UserInfo", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_SFB_ResponseGroupInfo", new[] { "EndPointSystemId" });
            DropTable("dbo.SkypeForBusinessUserInfoSkypeForBusinessResponseGroupInfo");
            DropTable("dbo.DI_SFB_UserInfo");
            DropTable("dbo.DI_SFB_ResponseGroupInfo");
        }
    }
}
