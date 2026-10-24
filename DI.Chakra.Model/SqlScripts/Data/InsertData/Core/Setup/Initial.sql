-------Tenant
TRUNCATE TABLE [DI_M_Tenant]
INSERT [dbo].[DI_M_Tenant] ([TenantId], [TenantName], [TenantDescription], [Address1], [Address2], [Address3], [CreatedBy], [CreateDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'ABCCorp', N'ABCCorp', N'ABCCorp', N'Singapore', N'ABCCorp', N'Singapore', N'System', getdate(), N'System', getdate())

--------User
SET IDENTITY_INSERT [dbo].DI_M_User ON

IF NOT EXISTS (SELECT * FROM DI_M_User WHERE [TenantId]='ABCCorp'AND [CreatedBy] = 'System')
	INSERT [dbo].[DI_M_User] ([Id], [NRIC], [FIN], [PassportNo], [UserName], [Password], [TenantId], [FirstName], [MiddleName], [LastName], [DisplayName], [KnownAs], [EmployeeId], [FullTimeEmployee], [OrganisationId], [BusinessRoleId], [ManagerId], [EmailId], [TelephoneNumber], [HomephoneNumber], [MobileNumber], [JoinDate], [RehireDate], [TerminationDate], [IsActive], [Ethinicity], [Country], [EmployeeType], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [AuthenticationMode],  [Reserved1], [Reserved2], [Reserved3], [Reserved4], [Reserved5], [Reserved6], [Reserved7], [Reserved8], [Reserved9], [Reserved10],  [Fax], [PostalCode], [PostalAddress], [LastLogonDate],[UserExtension]) 
	VALUES (1, NULL, NULL, NULL, N'IACMAdmin', N'+GNcBxodDhLAhKsu0oaGDQ==', N'ABCCorp', N'IACM', N'', N'Admin', N'IACM Administrator', NULL, NULL, NULL,  NULL, NULL, NULL, N'iacmadmin@abccorp.com', NULL, NULL, NULL, GETDATE(), NULL, NULL, 1, NULL, NULL, 1, N'System', GETDATE(), N'System', GETDATE(), N'Local', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, GETDATE(),'<?xml version="1.0" encoding="utf-16"?><UserExtension xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"></UserExtension>')

SET IDENTITY_INSERT [dbo].DI_M_User OFF

--------Group 
SET IDENTITY_INSERT [dbo].[DI_M_Group] ON

IF NOT EXISTS (SELECT * FROM [DI_M_Group] WHERE Id=1 AND [CreatedBy] = 'System')
	INSERT [dbo].[DI_M_Group] ([Id],[GroupName], [GroupOwner], [GroupDescription], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], IsDefaultGroup) VALUES (1,N'IT Security', 1, N'IT Security', N'System', GETDATE(), N'IACMAdmin', GETDATE(),1)

SET IDENTITY_INSERT [dbo].[DI_M_Group] OFF

GO
