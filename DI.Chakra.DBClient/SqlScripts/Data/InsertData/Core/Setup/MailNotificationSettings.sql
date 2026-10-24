
SET IDENTITY_INSERT [DI_M_MailNotificationSettings] ON
GO

IF NOT EXISTS (SELECT * FROM [DI_M_MailNotificationSettings] WHERE [TriggerName]='NewJoinee')
	INSERT [dbo].[DI_M_MailNotificationSettings] ([Id],[TriggerName], [MailTriggerFromJob],[MailTriggerOnEffectiveDate], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [From], [To],[CC],[PreConfigureNotificationDays]) 
	VALUES (1,'NewJoinee', 1,1, N'System', getdate(), N'System', getdate(), '', '','',0)

	IF NOT EXISTS (SELECT * FROM DI_M_MailNotificationSettings WHERE [TriggerName]='Promotion')
	INSERT [dbo].DI_M_MailNotificationSettings ([Id],[TriggerName], [MailTriggerFromJob],[MailTriggerOnEffectiveDate], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [From], [To],[CC],[PreConfigureNotificationDays]) 
	VALUES (2,'Promotion', 1,1, N'System', getdate(), N'System', getdate(), '', '','',0)

	IF NOT EXISTS (SELECT * FROM DI_M_MailNotificationSettings WHERE [TriggerName]='OUChange')
	INSERT [dbo].DI_M_MailNotificationSettings ([Id],[TriggerName], [MailTriggerFromJob],[MailTriggerOnEffectiveDate], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [From], [To],[CC],[PreConfigureNotificationDays]) 
	VALUES (3,'OUChange', 1,1, N'System', getdate(), N'System', getdate(), '', '','',0)

	IF NOT EXISTS (SELECT * FROM DI_M_MailNotificationSettings WHERE [TriggerName]='AdditionalRole')
	INSERT [dbo].DI_M_MailNotificationSettings ([Id],[TriggerName], [MailTriggerFromJob],[MailTriggerOnEffectiveDate], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [From], [To],[CC],[PreConfigureNotificationDays]) 
	VALUES (4,'AdditionalRole', 1,1, N'System', getdate(), N'System', getdate(), '', '','',0)

	IF NOT EXISTS (SELECT * FROM DI_M_MailNotificationSettings WHERE [TriggerName]='Sabbatical')
	INSERT [dbo].DI_M_MailNotificationSettings ([Id],[TriggerName], [MailTriggerFromJob],[MailTriggerOnEffectiveDate], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [From], [To],[CC],[PreConfigureNotificationDays]) 
	VALUES (5,'Sabbatical', 1,1, N'System', getdate(), N'System', getdate(), '', '','',0)

	IF NOT EXISTS (SELECT * FROM DI_M_MailNotificationSettings WHERE [TriggerName]='Resignation')
	INSERT [dbo].DI_M_MailNotificationSettings ([Id],[TriggerName], [MailTriggerFromJob],[MailTriggerOnEffectiveDate], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [From], [To],[CC],[PreConfigureNotificationDays]) 
	VALUES (6,'Resignation', 1,1, N'System', getdate(), N'System', getdate(), '', '','',0)

	IF NOT EXISTS (SELECT * FROM DI_M_MailNotificationSettings WHERE [TriggerName]='AdhocRequest')
	INSERT [dbo].DI_M_MailNotificationSettings ([Id],[TriggerName], [MailTriggerFromJob],[MailTriggerOnEffectiveDate], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [From], [To],[CC],[PreConfigureNotificationDays]) 
	VALUES (7,'AdhocRequest', 1,1, N'System', getdate(), N'System', getdate(), '', '','',0)

	IF NOT EXISTS (SELECT * FROM DI_M_MailNotificationSettings WHERE [TriggerName]='ReOrganisation')
	INSERT [dbo].DI_M_MailNotificationSettings ([Id],[TriggerName], [MailTriggerFromJob],[MailTriggerOnEffectiveDate], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [From], [To],[CC],[PreConfigureNotificationDays]) 
	VALUES (8,'ReOrganisation', 1,1, N'System', getdate(), N'System', getdate(), '', '','',0)

	IF NOT EXISTS (SELECT * FROM DI_M_MailNotificationSettings WHERE [TriggerName]='Propagation')
	INSERT [dbo].DI_M_MailNotificationSettings ([Id],[TriggerName], [MailTriggerFromJob],[MailTriggerOnEffectiveDate], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [From], [To],[CC],[PreConfigureNotificationDays]) 
	VALUES (9,'Propagation', 1,1, N'System', getdate(), N'System', getdate(), '', '','',0)

	IF NOT EXISTS (SELECT * FROM DI_M_MailNotificationSettings WHERE [TriggerName]='PostResignation')
	INSERT [dbo].DI_M_MailNotificationSettings ([Id],[TriggerName], [MailTriggerFromJob],[MailTriggerOnEffectiveDate], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [From], [To],[CC],[PreConfigureNotificationDays]) 
	VALUES (10,'PostResignation', 1,1, N'System', getdate(), N'System', getdate(), '', '','',0)

	IF NOT EXISTS (SELECT * FROM DI_M_MailNotificationSettings WHERE [TriggerName]='Rehire')
	INSERT [dbo].DI_M_MailNotificationSettings ([Id],[TriggerName], [MailTriggerFromJob],[MailTriggerOnEffectiveDate], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [From], [To],[CC],[PreConfigureNotificationDays]) 
	VALUES (11,'Rehire', 1,1, N'System', getdate(), N'System', getdate(), '', '','',0)

	IF NOT EXISTS (SELECT * FROM DI_M_MailNotificationSettings WHERE [TriggerName]='CSELGoldBar')
	INSERT [dbo].DI_M_MailNotificationSettings ([Id],[TriggerName], [MailTriggerFromJob],[MailTriggerOnEffectiveDate], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [From], [To],[CC],[PreConfigureNotificationDays]) 
	VALUES (12,'CSELGoldBar', 1,1, N'System', getdate(), N'System', getdate(), '', '','',0)

	SET IDENTITY_INSERT [DI_M_MailNotificationSettings] OFF


		