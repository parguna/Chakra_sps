DECLARE @EPTYPEId bigint
DECLARE @EPId bigint
SELECT @EPTYPEId =EndpointSystemTypeName FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName=11
SELECT @EPId =Id FROM [DI_M_EndpointSystem] WHERE EndpointSystemTypeId=@EPTYPEId

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES ( @EPTYPEId, @EPId, 1, N'DisableIMAP', NULL, N'0', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'DisablePOP3', NULL, N'0', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'EnableLitigationHold', NULL, N'1', 3, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'LitigationDuration', NULL, N'2555', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'EnableArchiving', NULL, N'1', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'EnableMailBoxAuditing', NULL, N'1', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'Groups', NULL, N'', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'Licenses', NULL, N'<?xml version="1.0" encoding="utf-16"?>
<ProvLicenseInfo xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <LicenseDetails>
    <ProvLicenseInfoDetail>
      <License>sppower:ENTERPRISEPREMIUM_NOPSTNCONF</License>
      <ServiceName>FLOW_O365_P3,TEAMS1,ADALLOM_S_O365,EQUIVIO_ANALYTICS,LOCKBOX_ENTERPRISE,EXCHANGE_ANALYTICS,ATP_ENTERPRISE,MCOEV,INTUNE_O365,PROJECTWORKMANAGEMENT,RMS_S_ENTERPRISE,YAMMER_ENTERPRISE,OFFICESUBSCRIPTION,MCOSTANDARD,EXCHANGE_S_ENTERPRISE,SHAREPOINTENTERPRISE,SHAREPOINTWAC</ServiceName>
    </ProvLicenseInfoDetail>
    <ProvLicenseInfoDetail>
      <License>sppower:EMS</License>
      <ServiceName>RMS_S_PREMIUM,INTUNE_A,AAD_PREMIUM,MFA_PREMIUM</ServiceName>
    </ProvLicenseInfoDetail>
  </LicenseDetails>
</ProvLicenseInfo>', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'RoleAssignmentPolicy', NULL, N'Default Role Assignment Policy', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'RetentionPolicy', NULL, N'Default MRM Policy', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'SharingPolicy', NULL, N'Default Sharing Policy', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'RetainDeletedItemsForDays', NULL, N'30', 1, 'System', GETDATE(), NULL, NULL)


/* Shared Account */
INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES ( @EPTYPEId, @EPId, 3, N'DisableIMAP', NULL, N'0', 1, NULL, GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 3, N'DisablePOP3', NULL, N'0', 1, NULL, GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 3, N'EnableLitigationHold', NULL, N'1', 3, NULL, GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 3, N'LitigationDuration', NULL, N'2555', 1, NULL, GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 3, N'EnableArchiving', NULL, N'1', 1, NULL, GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 3, N'EnableMailBoxAuditing', NULL, N'1', 1, NULL, GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 3, N'Groups', NULL, N'', 1, NULL, GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 3, N'Licenses', NULL, N'<?xml version="1.0" encoding="utf-16"?>
<ProvLicenseInfo xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <LicenseDetails>
    <ProvLicenseInfoDetail>
      <License>sppower:ENTERPRISEPREMIUM_NOPSTNCONF</License>
      <ServiceName>FLOW_O365_P3,TEAMS1,ADALLOM_S_O365,EQUIVIO_ANALYTICS,LOCKBOX_ENTERPRISE,EXCHANGE_ANALYTICS,ATP_ENTERPRISE,MCOEV,INTUNE_O365,PROJECTWORKMANAGEMENT,RMS_S_ENTERPRISE,YAMMER_ENTERPRISE,OFFICESUBSCRIPTION,MCOSTANDARD,EXCHANGE_S_ENTERPRISE,SHAREPOINTENTERPRISE,SHAREPOINTWAC</ServiceName>
    </ProvLicenseInfoDetail>
    <ProvLicenseInfoDetail>
      <License>sppower:EMS</License>
      <ServiceName>RMS_S_PREMIUM,INTUNE_A,AAD_PREMIUM,MFA_PREMIUM</ServiceName>
    </ProvLicenseInfoDetail>
  </LicenseDetails>
</ProvLicenseInfo>', 1, NULL, GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 3, N'RoleAssignmentPolicy', NULL, N'Default Role Assignment Policy', 1, NULL, GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 3, N'RetentionPolicy', NULL, N'Default MRM Policy', 1, NULL, GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 3, N'SharingPolicy', NULL, N'Default Sharing Policy', 1, NULL, GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 3, N'RetainDeletedItemsForDays', NULL, N'30', 1, NULL, GETDATE(), NULL, NULL)

GO