SET IDENTITY_INSERT [DI_M_EndpointSystemType] ON
GO

IF NOT EXISTS (SELECT * FROM [DI_M_EndpointSystemType] WHERE [EndpointSystemTypeName]=19)
	INSERT [dbo].[DI_M_EndpointSystemType] ([Id],[EndpointSystemTypeName], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [DisplayName], [Description],[ObjectName],[CanBeTrustedSource]) 
	VALUES (19,19, 1, N'System', getdate(), N'System', getdate(), N'Exchange', N'Exchange','ExUserInfo',0)

GO
SET IDENTITY_INSERT [DI_M_EndpointSystemType] OFF

DECLARE @TypeId int
SELECT @TypeId =Id FROM [DI_M_EndpointSystemType] WHERE [EndpointSystemTypeName]=19
DELETE FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=19

INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, N'User', N'System', getdate(), N'System', getdate())
INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, N'Group', N'System', getdate(), N'System', getdate())



alter table [ExUserInfoExGroupInfo] add EndpointSystemId bigint not null,Version int null 


DECLARE @EndpointSystemTypeId bigint
DECLARE @KeyId bigint

-- Exchange
SELECT @EndpointSystemTypeId =Id FROM [DI_M_EndpointSystemType] WHERE DisplayName ='Exchange'
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='IPAddress'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,1,'varchar','[0-9\.\ \-]*',1,'onkeypress:return IPAddressKeyOnly(event);#onblur:return ValidateIPaddress(this)', 'System',GETDATE(),'System', GETDATE())
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='DomainName'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,2,'varchar','',1,'', 'System',GETDATE(),'System', GETDATE())
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='UserName'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,3,'varchar','',1,'', 'System',GETDATE(),'System', GETDATE())
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='Password'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,4,'varchar','',1,'onkeypress:return Preventspaces(event);', 'System',GETDATE(),'System', GETDATE())
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='HostName'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,5,'varchar','',1,'', 'System',GETDATE(),'System', GETDATE())
-- End

INSERT INTO [dbo].[DI_M_EndpointUserViewConfiguration]([EndpointSystemTypeId], [EndpointSystemId], [KeyName], [Caption], [Value],[Order],  [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,NULL,'AccountName','Account Name','sAMAccountName',1,'System',GETDATE(),'System', GETDATE())
INSERT INTO [dbo].[DI_M_EndpointUserViewConfiguration]([EndpointSystemTypeId], [EndpointSystemId], [KeyName], [Caption], [Value],[Order],  [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,NULL,'DisplayName','Display Name','DisplayName',2,'System',GETDATE(),'System', GETDATE())
INSERT INTO [dbo].[DI_M_EndpointUserViewConfiguration]([EndpointSystemTypeId], [EndpointSystemId], [KeyName], [Caption], [Value], [Order], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,NULL,'description','Description','description',3,'System',GETDATE(),'System', GETDATE())
INSERT INTO [dbo].[DI_M_EndpointUserViewConfiguration]([EndpointSystemTypeId], [EndpointSystemId], [KeyName], [Caption], [Value],[Order],  [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,NULL,'Email','E-Mail','mail',4,'System',GETDATE(),'System', GETDATE())
-- End


-- Default Mail Quota Roles

INSERT INTO [dbo].[DI_Ex_MailBoxQuotaInfo]([EndPointSystemId],[Name],[Description],[IssueWarningQuota],[ProhibitSendQuota],[ProhibitSendReceiveQuota],[CreatedBy],[CreatedDate],[LastModifiedBy],[LastModifiedDate],[CanRequest])
SELECT -1 , '1.4 GB', 'Top Management (1.4GB)','1.318 GB', '1.465 GB (1,572,864,000 bytes)',0,'System',GETDATE(),'System',GETDATE(),1
--FROM DI_M_EndpointSystem WHERE EndpointSystemTypeId = @EndpointSystemTypeId

INSERT INTO [dbo].[DI_Ex_MailBoxQuotaInfo]([EndPointSystemId],[Name],[Description],[IssueWarningQuota],[ProhibitSendQuota],[ProhibitSendReceiveQuota],[CreatedBy],[CreatedDate],[LastModifiedBy],[LastModifiedDate],[CanRequest])
SELECT -1 , '750 MB', 'VIP (750MB)','675 MB', '750 MB (786,432,000 bytes)',0,'System',GETDATE(),'System',GETDATE(),1
--FROM DI_M_EndpointSystem WHERE EndpointSystemTypeId = @EndpointSystemTypeId

INSERT INTO [dbo].[DI_Ex_MailBoxQuotaInfo]([EndPointSystemId],[Name],[Description],[IssueWarningQuota],[ProhibitSendQuota],[ProhibitSendReceiveQuota],[CreatedBy],[CreatedDate],[LastModifiedBy],[LastModifiedDate],[CanRequest])
SELECT -1 , '300 MB', 'Default (300MB)','260 MB', '290 MB',0,'System',GETDATE(),'System',GETDATE(),1
--FROM DI_M_EndpointSystem WHERE EndpointSystemTypeId = @EndpointSystemTypeId

INSERT INTO [dbo].[DI_Ex_MailBoxQuotaInfo]([EndPointSystemId],[Name],[Description],[IssueWarningQuota],[ProhibitSendQuota],[ProhibitSendReceiveQuota],[CreatedBy],[CreatedDate],[LastModifiedBy],[LastModifiedDate],[CanRequest])
SELECT -1 , '500 MB', '500 MB','450 MB', '500 MB (524,288,000 bytes)',0,'System',GETDATE(),'System',GETDATE(),1
--FROM DI_M_EndpointSystem WHERE EndpointSystemTypeId = @EndpointSystemTypeId

GO

Delete from DI_M_EndpointEntitlmentType where EndpointSystemTypeId = 19

insert into DI_M_EndpointEntitlmentType values (19,2, 'System', SYSDATETIME(),null,null)
insert into DI_M_EndpointEntitlmentType values (19,20, 'System', SYSDATETIME(),null,null)

GO
