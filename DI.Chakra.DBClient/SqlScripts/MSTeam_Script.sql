DECLARE @EPID BIGINT
SELECT @EPID = ID FROM DI_M_EndpointSystem WHERE EndpointSystemTypeId=30



IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='DefaultTeamDialPlanPolicy')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'DefaultTeamDialPlanPolicy', N'Default Teams IDD Policy', N'SP-DialPlan-Teams', 1, N'System', Getdate(), NULL, NULL)
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='DefaultOnlineVoiceRoutingPolicy')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'DefaultOnlineVoiceRoutingPolicy', N'Default Teams  OnlineVoiceRoutingPolicy', N'VP-SP-Teams-Local', 1, N'System', Getdate(), NULL, NULL)
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='IDDTenantDialPlanPolicy')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'IDDTenantDialPlanPolicy', N'Teams IDD Policy', N'SP-DialPlan-Teams', 1, N'System', Getdate(), NULL, NULL)
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='IDDOnlineVoiceRoutingPolicy')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'IDDOnlineVoiceRoutingPolicy', N'IDD OnlineVoiceRoutingPolicy', N'VP-SP-Teams-Intl', 1, N'System', Getdate(), NULL, NULL)
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='IDDDialPlanWithResponseGroupPolicy')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'IDDDialPlanWithResponseGroupPolicy', N'Teams IDD IDDDialPlanPolicy For Response Group', N'SP-DialPlan-Teams-Hotline', 1, N'System', Getdate(), NULL, NULL)
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='IDDOnlineVoiceRoutingWithResponseGroupPolicy')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'IDDOnlineVoiceRoutingWithResponseGroupPolicy', N'IDD OnlineVoiceRoutingPolicy For Response Group', N'VP-SP-Teams-Intl-Hotline', 1, N'System', Getdate(), NULL, NULL)
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='TeamsUpgradePolicy')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'TeamsUpgradePolicy', N'TeamsUpgradePolicy', N'UpgradeToTeams', 1, N'System', Getdate(), NULL, NULL)
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='ON_GRANT_CS_TDP')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'ON_GRANT_CS_TDP', N'Check whether user is exists in Distribution Group', N'Grant-CsTenantDialPlan -identity "@@#UPN#@@" -policyname "@@#TENANTDIALPLAN#@@"', 1, N'system', GetDate(), NULL, NULL)
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='OP_ENABLE_CSUSER')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'OP_ENABLE_CSUSER', N'Enable command to create MSTeam account', N'Enable-CsUser -identity "@@#UPN#@@" -sipaddress "@@#USRSIPADDRESS#@@" -hostingproviderproxyfqdn "sipfed.online.lync.com"', 1, N'System', GetDate(), NULL, NULL)
END

IF EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='ON_SetCsUser')
BEGIN
	DELETE FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='ON_SetCsUser'
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='ON_SetCsUser')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy],[LastModifiedDate]) VALUES (30, @EPID, 1, N'ON_SetCsUser', N'Online command',N'Set-CsUser -Identity "@@#UPN#@@" -EnterpriseVoiceEnabled $true -OnPremLineURI "@@#LINEURI#@@" -HostedVoiceMail $true', 1, N'system', GetDate(), NULL, NULL)
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='OP_SetCsUser')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'OP_SetCsUser', N'OnPrem SetCsUser Command', N'Set-CsUser -Identity "@@#UPN#@@" -EnterpriseVoiceEnabled $true -LineURI "@@#LINEURI#@@"', 1, N'System', GetDate(), NULL, NULL)
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='OP_GRANT_CSVP')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'OP_GRANT_CSVP', N'Online command', N'Grant-CsVoicePolicy -Identity "@@#UPN#@@" -PolicyName "@@#VOICEPOLICY#@@"', 1, N'system', GetDate(), NULL, NULL)
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='OP_GRANT_CSVRP')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'OP_GRANT_CSVRP', N'OnPrem SetCsUser Command', N'Grant-CsVoiceRoutingPolicy -Identity "@@#UPN#@@" -PolicyName "@@#VOICEROUTINGPOLICY#@@"', 1, N'System', GetDate(), NULL, NULL)
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='OP_GRANT_CSDP')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'OP_GRANT_CSDP', N'Online command', N'Grant-CsDialPlan -Identity "@@#UPN#@@" -PolicyName "@@#DIALPLAN#@@"', 1, N'system', GetDate(), NULL, NULL)
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='ON_GRANT_CS_OVRP')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'ON_GRANT_CS_OVRP', N'OnPrem SetCsUser Command', N'Grant-CsOnlineVoiceRoutingPolicy -Identity "@@#UPN#@@" -PolicyName "@@#ONLINEVOICEROUTINGPOLICY#@@"', 1, N'System', GetDate(), NULL, NULL)
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='ON_GRANT_CS_TUP')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value],[IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'ON_GRANT_CS_TUP', N'Online command',N'Grant-CsTeamsUpgradePolicy -Identity "@@#UPN#@@" -PolicyName UpgradeToTeams', 1, N'system', GetDate(), NULL, NULL)
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='ON_GRANT_CS_EAP')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'ON_GRANT_CS_EAP', N'OnPrem SetCsUser Command', N'Grant-CsExternalAccessPolicy -Identity "@@#UPN#@@" -PolicyName "FederationOnly"', 1, N'System', GetDate(), NULL, NULL)
END

IF  EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='ON_GRANT_CS_CONFPLY')
BEGIN
	DELETE FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='ON_GRANT_CS_CONFPLY'
END
IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='ON_GRANT_CS_CONFPLY')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'ON_GRANT_CS_CONFPLY', N'Online command', N'Grant-CsConferencingPolicy -identity "@@#UPN#@@" -policyname "BPosSAllModalityNoFT"', 1, N'system', GetDate(), NULL, NULL)
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='OP_GET_CSUSER')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'OP_GET_CSUSER', N'OnPrem SetCsUser Command', N'Get-CsUser -ResultSize unlimited', 1, N'System', GetDate(), NULL, NULL)
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='ON_GET_CSUSER')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'ON_GET_CSUSER', N'Get Online User', N'Get-CsOnlineUser -ResultSize unlimited | Select SamAccountName,UserPrincipalName,FirstName,LastName,WindowsEmailAddress,Sid,DisplayName,LineURI,SipAddress,Enabled,TenantId,TargetRegistrarPool,VoicePolicy,VoiceRoutingPolicy,DialPlan,RegistrarPool,ExternalAccessPolicy,ConferencingPolicy,HostingProvider,ObjectId,Name,DistinguishedName,Identity,Guid,ObjectCategory,ObjectClass,WhenChanged,WhenCreated,OriginatingServer,IsValid,ObjectState,TenantDialPlan,OnlineVoiceRoutingPolicy,TeamsUpgradePolicy', 1, N'system', GetDate(), NULL, NULL)
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='OP_MoveUser')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'OP_MoveUser', N'Grant Online Voice Routing Policy', N'Move-CsUser -Identity "@@#UPN#@@" -Target "@@#POOLNAME#@@" -HostedMigrationOverrideUrl "@@#HOSTED_MIGRATION_OVERRIDEURL#@@" -DomainController "@@#DOMAINCONTROLLER#@@" -Confirm:$false -UseOAuth', 1, N'System', GetDate(), NULL, NULL)
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='ON_CsCallQueueScript')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'ON_CsCallQueueScript', N'Get Response Group', N'Get-CsCallQueue| Select Identity,Name,DistributionLists', 1, N'system', GetDate(), NULL, NULL)
END

IF EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='ON_DGMemberCheck')
BEGIN
	DELETE FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='ON_DGMemberCheck'
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='ON_DGMemberCheck')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'ON_DGMemberCheck', N'Check distribution member exists', N'Get-DistributionGroupMember -Identity "@@#GROUPIDENTITY#@@" | where {$_.Name -eq "@@#UPN#@@"}', 1, N'system', GetDate(), NULL, NULL)
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='OP_MoveOnLine')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'OP_MoveOnLine', N'Move user to Online', N'Move-CsUser -Identity "@@#UPN#@@" -Target "@@#POOLNAME#@@" -HostedMigrationOverrideUrl "@@#HOSTED_MIGRATION_OVERRIDEURL#@@" -DomainController "@@#DOMAINCONTROLLER#@@" -Confirm:$false -UseOAuth', 1, N'System', GetDate(), NULL, NULL)
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='ON_FindUserLineURI')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'ON_FindUserLineURI', N'Find user by property', N'Get-CSOnlineUser -filter {(LineURI -eq "@@#LINEURI#@@") -or (OnPremLineURI -eq "@@#LINEURI#@@")} | Select UserPrincipalName,LineURI', 1, N'System', GetDate(), NULL, NULL)
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='ON_UserHasResponseGroup')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'ON_UserHasResponseGroup', N'Check whether user has response group', N'$DistributionGroups= Get-DistributionGroup | where { (Get-DistributionGroupMember $_.Name | foreach {$_.Name}) -contains "@@#SAMACCOUNTNAME#@@"} \n $DistributionGroups', 1, N'System', GetDate(), NULL, NULL)
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='OP_FindUserLineURI')
BEGIN
		DELETE FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='OP_FindUserLineURI'
END
IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='OP_FindUserLineURI')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'OP_FindUserLineURI', N'Check Line URI in OnPrimeURI before assign number', N'Get-CSOnlineUser -filter {(LineURI -eq "@@#LINEURI#@@")} | Select UserPrincipalName,LineURI', 1, N'System', GetDate(), NULL, NULL)
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='ON_GETCS_SPECIFIC_USER')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'ON_GETCS_SPECIFIC_USER', N'Get the specific user', N'Get-CsOnlineUser -Identity "@@#UPN#@@"', 1, N'System', GetDate(), NULL, NULL)
END

IF NOT EXISTS(SELECT 1 FROM  DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='OP_GETCS_SPECIFIC_USER')
BEGIN
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, @EPID, 1, N'OP_GETCS_SPECIFIC_USER', NULL, N'Get-CsUser -Identity "@@#UPN#@@"', 1, N'system', GetDate(), NULL, NULL)
END

IF NOT EXISTS(select 1 from DI_M_NProvisioningField WHERE EndPointSystemTypeId=30 AND EndPointSystemId=@EPID AND FieldName='OP_DISABLE_MSTeam_User')
BEGIN
		INSERT INTO DI_M_NProvisioningField(EndPointSystemTypeId,EndPointSystemId,AccountType,FieldName,Description,Value,IsActive,CreatedBy,CreatedDate)
		VALUES(30,@EPID,1,'OP_DISABLE_MSTeam_User','Disable SFB user','Disable-CsUser -Identity "@@#UPN#@@" -Confirm:$false',1,'System',GETDATE())
END
GO

IF NOT EXISTS(SELECT 1 FROM dbo.DI_M_IMCONFIGURATIONMASTER WHERE CONFIGKEY='Office365PSModuleForMSTeam')
BEGIN
INSERT INTO dbo.DI_M_IMCONFIGURATIONMASTER (ConfigKey,ConfigValue,IsUiDisplay,Description,CreatedBy,CreatedDate)
VALUES('Office365PSModuleForMSTeam','C:\Program Files\WindowsPowerShell\Modules\ExchangeOnlineManagement\2.0.5\ExchangeOnlineManagement.psd1',1,'V2 Module fetching response group','System',GETDATE())
END

GO

IF EXISTS(SELECT 1 FROM [dbo].[DI_M_MailTemplate] WHERE [TemplateName]='NotifyMSTeamsDelegation')
BEGIN
DELETE FROM [dbo].[DI_M_MailTemplate] WHERE [TemplateName]='NotifyMSTeamsDelegation'
END

IF NOT EXISTS(SELECT 1 FROM [dbo].[DI_M_MailTemplate] WHERE [TemplateName]='NotifyMSTeamsDelegation')
BEGIN

INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'NotifyMSTeamsDelegation', N'Notify the Email Team on Take action for Delegation', N'securityadmin@spgroup.com.sg', N'', NULL, NULL, N'IDMS: @@REQUESTNUMBER@@- @@APPLICATIONNAME@@ - @@ACTIONNAME@@ - action required', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
    <head>
        <title></title>
        <style type="text/css">
            .body {              font-family: Courier New;              font-size: 13px;              font-weight: normal;              margin-left: 5px;              }              .marlef {              margin-left: 40px;              }              .marlef1 {              margin-left: 50px;              }              .marlef2 {              margin-left: 50px;              }
        </style>
    </head>
    <body class="body" jquery191031130699542479323="28" jquery19105777307022835423="28" _events="[object Object]">
        <p style="margin-left: 40px;"><span style="text-align: left; text-transform: none; text-indent: 0px; letter-spacing: normal; font-style: normal; font-weight: 400; word-spacing: 0px; float: none; white-space: pre-wrap; orphans: 2; widows: 2; background-color: #ffffff; text-decoration-color: initial; font-variant-ligatures: normal; font-variant-caps: normal; -webkit-text-stroke-width: 0px; text-decoration-style: initial; display: inline !important; font-family: ''Courier New''; font-size: 13px; color: #000000;">Dear Email Team,</span></p>
        <p style="margin-left: 40px;"><span style="text-align: left; text-transform: none; text-indent: 0px; letter-spacing: normal; font-style: normal; font-weight: 400; word-spacing: 0px; float: none; white-space: pre-wrap; orphans: 2; widows: 2; background-color: #ffffff; text-decoration-color: initial; font-variant-ligatures: normal; font-variant-caps: normal; -webkit-text-stroke-width: 0px; text-decoration-style: initial; display: inline !important; font-family: ''Courier New''; font-size: 13px; color: #000000;">The following user required @@ACTIONNAME@@ action.</span></p>
<p style="margin-left: 40px;">
    <b>IDMS Request Number </b>: @@REQUESTNUMBER@@ <br>
    <b>Endpoint System  </b>: @@APPLICATIONNAME@@ <br>
    <b>UserPrincipalName</b>: @@UPN@@ <br>
	<b>RequestSource    </b>: @@REQUESTSOURCE@@ <br>
    <b>Request For      </b>: @@REQUESTFOR@@ <br>
    <b>Requestby        </b>: @@REQUESTBY@@ <br>
    <b>Action           </b>: @@ACTIONNAME@@ <br>
	<b>LineURI          </b>: @@LINEURI@@ <br>
	<b>EmailId          </b>: @@EMAIL@@ <br>
 </p>    
        <p style="margin-left: 40px;"><strong>Response Group(s)</strong></p>
        <p style="margin-left: 40px;">@@RESPONSEGROUPNAMES@@</p>
        
        
        
        <p style="font-family: arial; margin-left: 40px;">&nbsp;<span style="font-size: small; white-space: pre-wrap; background-color: #ffffff; font-family: ''Courier New''; color: infotext;"></span></p>
        
        <p style="margin-left: 40px;">Regards,</p>
        <p style="margin-left: 40px;">@@REGARDS@@</p>
        <p style="margin-left: 40px;"><span style="font-family: ''Courier New''; font-size: 13px; color: infotext;">&nbsp;</span></p>
        <p style="margin-left: 40px;"><span style="font-family: ''Courier New''; font-size: 13px; color: infotext;">*** This is a system-generated message. Please do not reply to this message.***</span></p>
    
</body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
END
GO


GO
Update DI_M_CustomReport set Name='MicrosoftTeams User', ReportQuery='SELECT  CASE WHEN UserType=26 THEN ''Default'' WHEN UserType=27 THEN ''DCC'' WHEN UserType=28 THEN ''DCCS'' WHEN UserType=29 THEN ''IDD'' ELSE ''NONE'' END as UserType,[TenantDialPlan],[OnlineVoiceRoutingPolicy],[TeamsUpgradePolicy],[SamAccountName], [UserPrincipalName], [FirstName], [LastName], [WindowsEmailAddress],  [LineServerURI],  [DisplayName], [ProxyAddresses],  [EnabledForFederation], [EnabledForInternetAccess], [PublicNetworkEnabled], [EnterpriseVoiceEnabled], [EnabledForRichPresence], [ExchangeArchivingPolicy], [LineURI], [SipAddress], [Enabled], [TenantId], [UserRoutingGroupId], [TargetRegistrarPool], [VoicePolicy], [MobilityPolicy], [ConferencingPolicy], [PresencePolicy], [VoiceRoutingPolicy], [RegistrarPool], [DialPlan], [LocationPolicy], [ClientPolicy], [ClientVersionPolicy], [ArchivingPolicy],  [HostingProvider], [Name], [DistinguishedName], [Identity],  [WhenChanged], [WhenCreated],  [LineURIExtension], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] FROM dbo.DI_SFB_UserInfo' where UniqueName='REPORT_SFB_USERS' and name='SFB Users'
GO
Update DI_M_CustomReport set  Name='MicrosoftTeams UserGroup',ReportQuery='SELECT R.Name,U.UserPrincipalName,U.WindowsEmailAddress, U.SamAccountName,U.DisplayName,U.DistinguishedName,R.DistributionGroupName,R.DistributionGroupAddress from dbo.DI_SFB_ResponseGroupInfo R inner join dbo.SkypeForBusinessUserInfoSkypeForBusinessResponseGroupInfo UG  on R.Id = UG.SkypeForBusinessResponseGroupInfo_Id  Inner join dbo.DI_SFB_UserInfo U on UG.SkypeForBusinessUserInfo_Id = U.Id Order by R.Name' where UniqueName='REPORT_SKYPEFORBUSINESS_-_USER' and name='SFB - UserGroup'
GO
Update DI_M_CustomReport set  Name='MicrosoftTeams ResponseGroup',ReportQuery='SELECT Name,[Identity],[DistributionGroupAddress],[DistributionGroupName],Description,CreatedDate,LastModifiedDate from dbo.DI_SFB_ResponseGroupInfo ' where UniqueName='REPORT_SFB_-_GROUP(S)' and name='SFB - Group(s)'
GO
Update DI_M_TabMenu set MenuName='MicrosoftTeams' where UniqueName='REPORT_SFB'
GO
Update DI_M_TabMenu set MenuName='User' where UniqueName='REPORT_SFB_USERS'
GO
Update DI_M_TabMenu set MenuName='UserGroup' where UniqueName='REPORT_SKYPEFORBUSINESS_-_USER'
GO
Update DI_M_TabMenu set MenuName='ResponseGroup' where UniqueName='REPORT_SFB_-_GROUP(S)'
GO

Update DI_M_TabMenu Set MenuName='Users' where UniqueName='REPORT_SFB_USERS' and MenuName='SFB Users'
GO
Update DI_M_TabMenu Set MenuName='UserGroup' where UniqueName='REPORT_SKYPEFORBUSINESS_-_USER' and MenuName='SFB - UserGroup'
GO
Update DI_M_TabMenu Set MenuName='ResponseGroup' where UniqueName='REPORT_SFB_-_GROUP(S)' and MenuName='SFB - Group(s)'
GO
Update DI_M_TabMenu Set MenuName='MicrosoftTeams' where UniqueName='REPORT_SFB' and MenuName='Skype For Business'
GO
Update DI_M_TabMenu Set MenuName='MicrosoftTeams' where UniqueName='SFB' and MenuName='Skype For Business'
GO



Update DI_M_Page Set Name='Microsoft Teams' where UniqueName='REPORT_SFB' and name='Skype For Business'
GO
Update DI_M_Page Set Name='Microsoft Teams' where UniqueName='SFB' and name='Skype For Business'
GO


GO
IF EXISTS(SELECT 1 FROM [dbo].[DI_M_MailTemplate] WHERE [TemplateName]='NotifyMSTeamIDDRemovalOnHREvent')
BEGIN
DELETE FROM [dbo].[DI_M_MailTemplate] WHERE [TemplateName]='NotifyMSTeamIDDRemovalOnHREvent'
END

IF NOT EXISTS(SELECT 1 FROM [dbo].[DI_M_MailTemplate] WHERE [TemplateName]='NotifyMSTeamIDDRemovalOnHREvent')
BEGIN
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'NotifyMSTeamIDDRemovalOnHREvent', N'Notify the IDD Removal on HREvent(Offboard/Transfer)', N'securityadmin@spgroup.com.sg', N'', NULL, NULL, N'IDMS: @@REQUESTNUMBER@@ - @@APPLICATIONNAME@@ - IDD(@@REQUESTSOURCE@@) action required', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
    <head>
        <title></title>
        <style type="text/css">
            .body {              font-family: Courier New;              font-size: 13px;              font-weight: normal;              margin-left: 5px;              }              .marlef {              margin-left: 40px;              }              .marlef1 {              margin-left: 50px;              }              .marlef2 {              margin-left: 50px;              }
        </style>
    </head>
    <body class="body" jquery191031130699542479323="28" jquery19105777307022835423="28" _events="[object Object]">
        <p style="margin-left: 40px;"><span style="text-align: left; text-transform: none; text-indent: 0px; letter-spacing: normal; font-style: normal; font-weight: 400; word-spacing: 0px; float: none; white-space: pre-wrap; orphans: 2; widows: 2; background-color: #ffffff; text-decoration-color: initial; font-variant-ligatures: normal; font-variant-caps: normal; -webkit-text-stroke-width: 0px; text-decoration-style: initial; display: inline !important; font-family: ''Courier New''; font-size: 13px; color: #000000;">Dear Email Team,</span></p>
        <p style="margin-left: 40px;"><span style="text-align: left; text-transform: none; text-indent: 0px; letter-spacing: normal; font-style: normal; font-weight: 400; word-spacing: 0px; float: none; white-space: pre-wrap; orphans: 2; widows: 2; background-color: #ffffff; text-decoration-color: initial; font-variant-ligatures: normal; font-variant-caps: normal; -webkit-text-stroke-width: 0px; text-decoration-style: initial; display: inline !important; font-family: ''Courier New''; font-size: 13px; color: #000000;">For your action. The following action(@@REQUESTSOURCE@@) required for IDD User:@@USER@@.</span></p>
<p style="margin-left: 40px;">
    <b>IDMS Request Number  </b>: @@REQUESTNUMBER@@ <br>
    <b>Endpoint System  </b>: @@APPLICATIONNAME@@ <br>
	<b>Request Source   </b>: @@REQUESTSOURCE@@ <br>
    <b>Request For      </b>: @@REQUESTFOR@@ <br>
	<b>Request By      </b>: @@REQUESTBY@@ <br>
	<b>UserPrincipalName</b>: @@UPN@@ <br>
    <b>LineURI          </b>: @@LINEURI@@ <br>
	<b>EmailId          </b>: @@EMAIL@@ <br>
 </p>    
        <p style="font-family: arial; margin-left: 40px;">&nbsp;<span style="font-size: small; white-space: pre-wrap; background-color: #ffffff; font-family: ''Courier New''; color: infotext;"></span></p>
        
        <p style="margin-left: 40px;">Regards,</p>
        <p style="margin-left: 40px;">@@REGARDS@@</p>
        <p style="margin-left: 40px;"><span style="font-family: ''Courier New''; font-size: 13px; color: infotext;">&nbsp;</span></p>
        <p style="margin-left: 40px;"><span style="font-family: ''Courier New''; font-size: 13px; color: infotext;">*** This is a system-generated message. Please do not reply to this message.***</span></p>
    
</body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
END
GO

IF NOT EXISTS(SELECT 1 FROM DI_M_IMConfigurationMaster WHERE ConfigKey='AmendmentRemoveDCCMessage')
BEGIN
	INSERT INTO DI_M_IMConfigurationMaster(ConfigKey,ConfigValue,IsUiDisplay,Description,CreatedBy,CreatedDate)
	VALUES('AmendmentRemoveDCCMessage','New DID number will be assigned upon removal of DCC',1,'Show warning message to DCC Removal','System',GETDATE())
END
GO
IF NOT EXISTS(SELECT 1 FROM DI_M_IMConfigurationMaster WHERE ConfigKey='AmendmentAddDCCMessage')
BEGIN
	INSERT INTO DI_M_IMConfigurationMaster(ConfigKey,ConfigValue,IsUiDisplay,Description,CreatedBy,CreatedDate)
	VALUES('AmendmentAddDCCMessage','Current DID number will be re-assigned to other users upon request for DCC',1,'Show warning message to DCC Request','System',GETDATE())
END

GO
IF NOT EXISTS(SELECT 1 FROM DI_M_IACMConfigurationMaster WHERE ConfigKey='EmailTeam')
BEGIN
		INSERT INTO DI_M_IACMConfigurationMaster(ConfigKey,ConfigValue,Description,IsUiDisplay,CreatedBy,CreatedDate)
		VALUES('EmailTeam','Email Team','Used for sending email on DCC Removal and IDD request Notification',1,'System',GETDATE())
END
GO

