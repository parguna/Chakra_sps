
SET IDENTITY_INSERT [dbo].[DI_M_NOrganisation] ON

IF NOT EXISTS (SELECT * FROM [DI_M_NOrganisation] WHERE  [Code] = 'ALL')
	INSERT INTO [dbo].[DI_M_NOrganisation]([Id],[Code],[Name],[CreatedBy],[CreatedDate],[IsActive],[InternalCompany])
	VALUES (1,'ALL','ALL','System' ,GETDATE(),1,0)

SET IDENTITY_INSERT [dbo].[DI_M_NOrganisation] OFF

GO

SET IDENTITY_INSERT [dbo].[DI_M_NDesignation] ON

IF NOT EXISTS (SELECT * FROM [DI_M_NDesignation] WHERE [Id]= 0 AND [Description] = 'All')
	INSERT INTO [dbo].[DI_M_NDesignation]([Id],[Code],[Description],[IsActive])VALUES(0,'0000','All',1)

SET IDENTITY_INSERT [dbo].[DI_M_NDesignation] OFF

GO

SET IDENTITY_INSERT [DI_M_NOrganisationScope] ON
GO

IF NOT EXISTS (SELECT * FROM [DI_M_NOrganisationScope] WHERE Scope='Company')
	insert into DI_M_NOrganisationScope (id,Scope,[Level],IsValidForApproval,IsActive,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate)
values('1','Company','1','1','1','System',getdate(),'System',getdate())

GO
IF NOT EXISTS (SELECT * FROM [DI_M_NOrganisationScope] WHERE Scope='BusinessUnit')
	insert into DI_M_NOrganisationScope (id,Scope,[Level],IsValidForApproval,IsActive,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate)
values('2','BusinessUnit','2','1','1','System',getdate(),'System',getdate())

GO
IF NOT EXISTS (SELECT * FROM [DI_M_NOrganisationScope] WHERE Scope='Department')
insert into DI_M_NOrganisationScope (id,Scope,[Level],IsValidForApproval,IsActive,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate)
values('3','Department','3','1','1','System',getdate(),'System',getdate())

GO
IF NOT EXISTS (SELECT * FROM [DI_M_NOrganisationScope] WHERE Scope='Location')
insert into DI_M_NOrganisationScope (id,Scope,[Level],IsValidForApproval,IsActive,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate)
values('4','Location','4','1','1','System',getdate(),'System',getdate())

GO

SET IDENTITY_INSERT [DI_M_NOrganisationScope] OFF

