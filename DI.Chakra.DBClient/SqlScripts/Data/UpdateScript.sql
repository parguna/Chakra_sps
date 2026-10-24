/*
SNC Configuration for endpoints
*/

INSERT INTO DI_M_EndpointConfigurationKey(KeyName, Caption, CreatedBy, CreatedDate) VALUES('SNCMode','SNCMode','System',GETDATE())
INSERT INTO DI_M_EndpointConfigurationKey(KeyName, Caption, CreatedBy, CreatedDate) VALUES('SNCQop','SNCQop','System',GETDATE())
INSERT INTO DI_M_EndpointConfigurationKey(KeyName, Caption, CreatedBy, CreatedDate) VALUES('SNCLib','SNCLib','System',GETDATE())
INSERT INTO DI_M_EndpointConfigurationKey(KeyName, Caption, CreatedBy, CreatedDate) VALUES('SNCPartnerName','SNCPartnerName','System',GETDATE())

DECLARE @EndpointSytemTypeId int
SELECT @EndpointSytemTypeId=Id FROM DI_M_EndpointSystemType where DisplayName ='SAP'

DECLARE @SNCModeKeyId int, @SNCQopKeyId int, @SNCLibKeyId int, @SNCPartnerNameKeyId int

SELECT @SNCModeKeyId=id from DI_M_EndpointConfigurationKey WHERE KeyName='SNCMode'
SELECT @SNCQopKeyId=id from DI_M_EndpointConfigurationKey WHERE KeyName='SNCQop'
SELECT @SNCLibKeyId=id from DI_M_EndpointConfigurationKey WHERE KeyName='SNCLib'
SELECT @SNCPartnerNameKeyId=id from DI_M_EndpointConfigurationKey WHERE KeyName='SNCPartnerName'

INSERT INTO DI_M_EndpointKeyConfiguration(EndpointSystemTypeId, KeyId, [Order], DataType,IsRequired, CreatedBy, CreatedDate) VALUES(@EndpointSytemTypeId, @SNCModeKeyId, 6,'bit', 0,'System', GETDATE())
INSERT INTO DI_M_EndpointKeyConfiguration(EndpointSystemTypeId, KeyId, [Order], DataType,IsRequired, CreatedBy, CreatedDate) VALUES(@EndpointSytemTypeId, @SNCQopKeyId, 7,'varchar',0, 'System', GETDATE())
INSERT INTO DI_M_EndpointKeyConfiguration(EndpointSystemTypeId, KeyId, [Order], DataType,IsRequired, CreatedBy, CreatedDate) VALUES(@EndpointSytemTypeId, @SNCLibKeyId, 8,'varchar',0, 'System', GETDATE())
INSERT INTO DI_M_EndpointKeyConfiguration(EndpointSystemTypeId, KeyId, [Order], DataType,IsRequired, CreatedBy, CreatedDate) VALUES(@EndpointSytemTypeId, @SNCPartnerNameKeyId, 9,'varchar',0, 'System', GETDATE())

INSERT INTO DI_M_IMConfigurationMaster(ConfigKey,ConfigValue,CreatedDate,CreatedBy,IsUiDisplay)
values('O365Uri','https://ps.outlook.com/PowerShell-LiveID?PSVersion=2.0',GETDATE(),'System',1) ,
('CanCheckUserActiveForProvisioning','1',getdate(),'System',1)

INSERT INTO DI_M_IMConfigurationMaster(ConfigKey,ConfigValue,CreatedDate,CreatedBy,IsUiDisplay,[Description])
values('PDPAConsentStatement','By submitting this form, you represent that you have obtained the consent of your staff/workers named in this form, for SP Group and its employees, agents and representatives, to collect, use and disclose all information as contained in the form for all purposes relevant in the context of this form and as notified to you by SP Group''''s Sponsor, as well to comply with all laws and other legal requirements relevant to SP Group from time to time. ',GETDATE(),'System',1,'This message will be shown in NonEmployee Add/Edit Page') 

INSERT INTO DI_M_IACMConfigurationMaster(ConfigKey,ConfigValue,Description,CreatedBy,CreatedDate,IsUiDisplay)
VALUES('DISCOVERY_SVCCONTROLLER2','DIChakraDiscoveryService2','Discovery service 2','system',GETDATE(),1)

INSERT INTO DI_M_IMConfigurationMaster(ConfigKey,ConfigValue,CreatedDate,CreatedBy,IsUiDisplay)
values('TriggerHRMSUserDataUpdate','0',GETDATE(),'System',1) ,
('AccountNameMinimumLength','7',getdate(),'System',1)