INSERT [dbo].[DI_M_NRuleset] ([RuleId], [Code], [TypeId], [Value], [Operator], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (8, N'Fin', 1, N'0023', N'Equals', NULL, NULL, NULL, NULL)
INSERT [dbo].[DI_M_NRuleset] ([RuleId], [Code], [TypeId], [Value], [Operator], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (9, N'Sal3', 1, N'0011', N'Equals', NULL, NULL, NULL, NULL)
INSERT [dbo].[DI_M_NRuleset] ([RuleId], [Code], [TypeId], [Value], [Operator], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (10, N'IT3', 1, N'0017', N'Equals', NULL, NULL, NULL, NULL)
Insert [dbo].[DI_M_NRuleset] ([RuleId], [Code], [TypeId], [Value], [Operator], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (11,'All Employees', 3, '*', 'Equals', null, getdate(), null, getdate())
GO

DECLARE @ADId bigint
DECLARE @LNSId bigint
DECLARE @SAPId bigint
SELECT @ADId =Id FROM [DI_M_EndpointSystem] WHERE [EndpointSystemTypeId]=1
SELECT @LNSId =Id FROM [DI_M_EndpointSystem] WHERE [EndpointSystemTypeId]=2
SELECT @SAPId =Id FROM [DI_M_EndpointSystem] WHERE [EndpointSystemTypeId]=3

DECLARE @RuleId bigint
SELECT @RuleId =[RuleId] FROM [DI_M_NRuleset] WHERE [Code]='Fin'
INSERT [dbo].[DI_M_NRulesetForApplication] ([EndpointSystemId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@ADId, NULL, NULL, NULL, NULL, @RuleId)
INSERT [dbo].[DI_M_NRulesetForApplication] ([EndpointSystemId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@LNSId, NULL, NULL, NULL, NULL, @RuleId)
INSERT [dbo].[DI_M_NRulesetForApplication] ([EndpointSystemId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@SAPId, NULL, NULL, NULL, NULL, @RuleId)

SELECT @RuleId =[RuleId] FROM [DI_M_NRuleset] WHERE [Code]='Sal3'
INSERT [dbo].[DI_M_NRulesetForApplication] ([EndpointSystemId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@ADId, NULL, NULL, NULL, NULL, @RuleId)
INSERT [dbo].[DI_M_NRulesetForApplication] ([EndpointSystemId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@LNSId, NULL, NULL, NULL, NULL, @RuleId)
INSERT [dbo].[DI_M_NRulesetForApplication] ([EndpointSystemId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@SAPId, NULL, NULL, NULL, NULL, @RuleId)

SELECT @RuleId =[RuleId] FROM [DI_M_NRuleset] WHERE [Code]='IT3'
INSERT [dbo].[DI_M_NRulesetForApplication] ([EndpointSystemId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@ADId, NULL, NULL, NULL, NULL, @RuleId)
INSERT [dbo].[DI_M_NRulesetForApplication] ([EndpointSystemId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@LNSId, NULL, NULL, NULL, NULL, @RuleId)
INSERT [dbo].[DI_M_NRulesetForApplication] ([EndpointSystemId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@SAPId, NULL, NULL, NULL, NULL, @RuleId)

GO
DECLARE @EPId bigint
SELECT @EPId =Id FROM [DI_M_EndpointSystem] WHERE [EndpointSystemTypeId]=1

DECLARE @G_FieldId nvarchar(max), @O_FieldId nvarchar(max)
SELECT @G_FieldId = Id from DI_M_NProvisioningField where FieldName='Groups' AND EndpointSystemId=@EPId
SELECT @O_FieldId = Id from DI_M_NProvisioningField where FieldName='OU' AND EndpointSystemId=@EPId


INSERT [dbo].[DI_M_NRulesetForProvisioning] ([EndpointsystemId], [AccountType], [ProvisioningFieldId], [Value], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@EPId, N'1', @G_FieldId, N'Finance', NULL, NULL, NULL, NULL, 8)
INSERT [dbo].[DI_M_NRulesetForProvisioning] ([EndpointsystemId], [AccountType], [ProvisioningFieldId], [Value], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@EPId, N'1', @O_FieldId, N'OU=Finance,OU=User,DC=diad,DC=com', NULL, NULL, NULL, NULL, 8)
INSERT [dbo].[DI_M_NRulesetForProvisioning] ([EndpointsystemId], [AccountType], [ProvisioningFieldId], [Value], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@EPId, N'1', @G_FieldId, N'Sales', NULL, NULL, NULL, NULL, 9)
INSERT [dbo].[DI_M_NRulesetForProvisioning] ([EndpointsystemId], [AccountType], [ProvisioningFieldId], [Value], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@EPId, N'1', @O_FieldId, N'OU=CIT,OU=User,DC=diad,DC=com', NULL, NULL, NULL, NULL, 9)
INSERT [dbo].[DI_M_NRulesetForProvisioning] ([EndpointsystemId], [AccountType], [ProvisioningFieldId], [Value], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@EPId, N'1', @G_FieldId, N'IT Support', NULL, NULL, NULL, NULL, 10)
INSERT [dbo].[DI_M_NRulesetForProvisioning] ([EndpointsystemId], [AccountType], [ProvisioningFieldId], [Value], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@EPId, N'1', @O_FieldId, N'OU=CIT,OU=User,DC=diad,DC=com', NULL, NULL, NULL, NULL, 10)
GO
DECLARE @EPId bigint
SELECT @EPId =Id FROM [DI_M_EndpointSystem] WHERE [EndpointSystemTypeId]=2

DECLARE @GRP_FieldId bigint, @CERTID_FieldId bigint, @TCERTID_FieldId bigint, @CERT_FieldId bigint
SELECT @GRP_FieldId = Id from DI_M_NProvisioningField where FieldName='Groups' AND EndpointSystemId=@EPId
SELECT @CERTID_FieldId = Id from DI_M_NProvisioningField where FieldName='CertifierIDFile' AND EndpointSystemId=@EPId
SELECT @TCERTID_FieldId = Id from DI_M_NProvisioningField where FieldName='TargetCertifierIdFile' AND EndpointSystemId=@EPId
SELECT @CERT_FieldId = Id from DI_M_NProvisioningField where FieldName='TargetCertifier' AND EndpointSystemId=@EPId

INSERT [dbo].[DI_M_NRulesetForProvisioning] ([EndpointsystemId], [AccountType], [ProvisioningFieldId], [Value], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@EPId, N'1', @GRP_FieldId, N'Finance', NULL, NULL, NULL, NULL, 8)
INSERT [dbo].[DI_M_NRulesetForProvisioning] ([EndpointsystemId], [AccountType], [ProvisioningFieldId], [Value], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@EPId, N'1', @CERTID_FieldId, N'F:\DeepIdentity\LotusNotes\IDFilesDIEP01\IDFiles\LNSCertifiers\finance.id', NULL, NULL, NULL, NULL, 8)
INSERT [dbo].[DI_M_NRulesetForProvisioning] ([EndpointsystemId], [AccountType], [ProvisioningFieldId], [Value], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@EPId, N'1', @TCERTID_FieldId, N'F:\DeepIdentity\LotusNotes\IDFilesDIEP01\IDFiles\LNSCertifiers\finance.id', NULL, NULL, NULL, NULL, 8)
INSERT [dbo].[DI_M_NRulesetForProvisioning] ([EndpointsystemId], [AccountType], [ProvisioningFieldId], [Value], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@EPId, N'1', @CERT_FieldId, N'OU=Finance/O=DeepIdentity', NULL, NULL, NULL, NULL, 8)
INSERT [dbo].[DI_M_NRulesetForProvisioning] ([EndpointsystemId], [AccountType], [ProvisioningFieldId], [Value], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@EPId, N'1', @GRP_FieldId, N'Sales', NULL, NULL, NULL, NULL, 9)
INSERT [dbo].[DI_M_NRulesetForProvisioning] ([EndpointsystemId], [AccountType], [ProvisioningFieldId], [Value], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@EPId, N'1', @CERTID_FieldId, N'F:\DeepIdentity\LotusNotes\IDFilesDIEP01\IDFiles\LNSCertifiers\sales.id', NULL, NULL, NULL, NULL, 9)
INSERT [dbo].[DI_M_NRulesetForProvisioning] ([EndpointsystemId], [AccountType], [ProvisioningFieldId], [Value], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@EPId, N'1', @TCERTID_FieldId, N'F:\DeepIdentity\LotusNotes\IDFilesDIEP01\IDFiles\LNSCertifiers\sales.id', NULL, NULL, NULL, NULL, 9)
INSERT [dbo].[DI_M_NRulesetForProvisioning] ([EndpointsystemId], [AccountType], [ProvisioningFieldId], [Value], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@EPId, N'1', @CERT_FieldId, N'OU=Sales/O=DeepIdentity', NULL, NULL, NULL, NULL, 9)
INSERT [dbo].[DI_M_NRulesetForProvisioning] ([EndpointsystemId], [AccountType], [ProvisioningFieldId], [Value], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@EPId, N'1', @GRP_FieldId, N'Technology', NULL, NULL, NULL, NULL, 10)
INSERT [dbo].[DI_M_NRulesetForProvisioning] ([EndpointsystemId], [AccountType], [ProvisioningFieldId], [Value], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@EPId, N'1', @CERTID_FieldId, N'F:\DeepIdentity\LotusNotes\IDFilesDIEP01\IDFiles\LNSCertifiers\technology.id', NULL, NULL, NULL, NULL, 10)
INSERT [dbo].[DI_M_NRulesetForProvisioning] ([EndpointsystemId], [AccountType], [ProvisioningFieldId], [Value], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@EPId, N'1', @TCERTID_FieldId, N'F:\DeepIdentity\LotusNotes\IDFilesDIEP01\IDFiles\LNSCertifiers\technology.id', NULL, NULL, NULL, NULL, 10)
INSERT [dbo].[DI_M_NRulesetForProvisioning] ([EndpointsystemId], [AccountType], [ProvisioningFieldId], [Value], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@EPId, N'1', @CERT_FieldId, N'OU=Technology/O=DeepIdentity', NULL, NULL, NULL, NULL, 10)

GO

DECLARE @EPId bigint
SELECT @EPId =Id FROM [DI_M_EndpointSystem] WHERE [EndpointSystemTypeId]=3

DECLARE @ROLE_FieldId bigint
SELECT @ROLE_FieldId = Id from DI_M_NProvisioningField where FieldName='Roles' AND EndpointSystemId=@EPId

INSERT [dbo].[DI_M_NRulesetForProvisioning] ([EndpointsystemId], [AccountType], [ProvisioningFieldId], [Value], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@EPId, N'1', @ROLE_FieldId, N'ZCRR_FINANCEDIRECTOR', NULL, NULL, NULL, NULL, 8)
INSERT [dbo].[DI_M_NRulesetForProvisioning] ([EndpointsystemId], [AccountType], [ProvisioningFieldId], [Value], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@EPId, N'1', @ROLE_FieldId, N'IDES_SCM_SALES', NULL, NULL, NULL, NULL, 9)
INSERT [dbo].[DI_M_NRulesetForProvisioning] ([EndpointsystemId], [AccountType], [ProvisioningFieldId], [Value], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [RuleId]) VALUES (@EPId, N'1', @ROLE_FieldId, N'SAP_CML_TECHNICAL_ADMIN', NULL, NULL, NULL, NULL, 10)

