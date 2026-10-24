
SET IDENTITY_INSERT [dbo].[DI_M_Organisation] ON

IF NOT EXISTS (SELECT * FROM [DI_M_Organisation] WHERE [TenantId]='ABCCorp'AND [CreatedBy] = 'System')
	INSERT INTO [dbo].[DI_M_Organisation]([Id],[TenantId],[OrgUnitPrefix],[OrganisationName],[CreatedBy],[CreatedDate],isactive)
	VALUES (0,'ABCCorp','00000000','All','System' ,GETDATE(),1)

SET IDENTITY_INSERT [dbo].[DI_M_Organisation] OFF

GO

SET IDENTITY_INSERT [dbo].[DI_M_Designation] ON

IF NOT EXISTS (SELECT * FROM [DI_M_Designation] WHERE [Id]= 0 AND [DesignationDesc] = 'All')
	INSERT INTO [dbo].[DI_M_Designation]([Id],[DesignationCode],[DesignationDesc],[IsActive])VALUES(0,'0000','All',1)

SET IDENTITY_INSERT [dbo].[DI_M_Designation] OFF

GO


SET IDENTITY_INSERT [dbo].[DI_M_CostCenter] ON

IF NOT EXISTS (SELECT * FROM [DI_M_CostCenter] WHERE [TenantId]='ABCCorp'AND [CreatedBy] = 'System')
	INSERT INTO [dbo].[DI_M_CostCenter]([Id],[TenantId],[CostCenterCode],[CreatedBy],[CreatedDate],[CompanyId],[IsActive])
	VALUES(0,'ABCCorp','ALL','System',GETDATE(),0,1)

SET IDENTITY_INSERT [dbo].[DI_M_CostCenter] OFF

GO

SET IDENTITY_INSERT [dbo].[DI_M_Company] ON

IF NOT EXISTS (SELECT * FROM [DI_M_Company] WHERE [TenantId]='ABCCorp' AND [CreatedBy] = 'System')
	INSERT INTO [dbo].[DI_M_Company]([Id],[TenantId],[CompanyCode],[CompanyName],[CreatedBy],[CreatedDate],[IsActive])
	VALUES(0,'ABCCorp','ALL','All','System',GETDATE(),1)

SET IDENTITY_INSERT [dbo].[DI_M_Company] OFF

GO


SET IDENTITY_INSERT [dbo].[DI_M_Department] ON

IF NOT EXISTS (SELECT * FROM [DI_M_Department] WHERE [TenantId]='ABCCorp'AND [CreatedBy] = 'System')
	INSERT INTO [dbo].[DI_M_Department]([Id],[TenantId],[DepartmentCode],[DepartmentText],[CreatedBy],[CreatedDate],[IsActive])
	VALUES(0,'ABCCorp','ALL','All','System',GETDATE(),1)

SET IDENTITY_INSERT [dbo].[DI_M_Department] OFF

GO

SET IDENTITY_INSERT [dbo].[DI_M_Division] ON

IF NOT EXISTS (SELECT * FROM [DI_M_Division] WHERE [TenantId]='ABCCorp'AND [CreatedBy] = 'System')
	INSERT INTO [dbo].[DI_M_Division]([Id],[TenantId],[DivisionCode],[DivisionText],[CreatedBy],[CreatedDate],[DepartmentId],[IsActive])
	VALUES(0,'ABCCorp','ALL','All','System',GETDATE(),0,1)

SET IDENTITY_INSERT [dbo].[DI_M_Division] OFF

GO

