

--------User
SET IDENTITY_INSERT [dbo].DI_M_User ON

IF NOT EXISTS (SELECT * FROM DI_M_User WHERE [CreatedBy] = 'System')
	INSERT [dbo].[DI_M_User] ([Id], [UserName], [Password], [FirstName], [MiddleName], [LastName], [DisplayName], [KnownAs], [EmployeeId], [FullTimeEmployee], [NOrganisationId], [NBusinessRoleId], [ManagerId], [EmailId], [TelephoneNumber], [HomephoneNumber], [MobileNumber], [JoinDate], [RehireDate], [TerminationDate], [IsActive], [Ethinicity], [Country], [EmployeeType], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [AuthenticationMode],  [Reserved1], [Reserved2], [Reserved3], [Reserved4], [Reserved5], [Reserved6], [Reserved7], [Reserved8], [Reserved9], [Reserved10],  [Fax], [PostalCode], [PostalAddress], [LastLogonDate],[EndpointSystemId],[GoldBar],[CSELException]) 
	VALUES (1, N'IACMAdmin', N'+GNcBxodDhLAhKsu0oaGDQ==', N'IACM', N'', N'Admin', N'IACM Administrator',  N'Admin', NULL, NULL,  NULL, NULL, NULL, N'iacmadmin@abccorp.com', NULL, NULL, N'2198311985', GETDATE(), NULL, NULL, 1, NULL, NULL, 1, N'System', GETDATE(), N'System', GETDATE(), N'Local', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, GETDATE(),0,0,0)

SET IDENTITY_INSERT [dbo].[DI_M_User] OFF

--------Group 
SET IDENTITY_INSERT [dbo].[DI_M_Group] ON

IF NOT EXISTS (SELECT * FROM [DI_M_Group] WHERE Id=1 AND [CreatedBy] = 'System')
	INSERT [dbo].[DI_M_Group] ([Id],[GroupName], [GroupOwner], [GroupDescription], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], IsDefaultGroup) 
	VALUES (1,N'IT Security', 1, N'IT Security', N'System', GETDATE(), N'IACMAdmin', GETDATE(),1)

SET IDENTITY_INSERT [dbo].[DI_M_Group] OFF


SET IDENTITY_INSERT [dbo].[DI_M_Group] ON

IF NOT EXISTS (SELECT * FROM [DI_M_Group] WHERE [GroupName] ='ITCCTEAM')
	INSERT [dbo].[DI_M_Group] ([Id],[GroupName], [GroupOwner], [GroupDescription], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], IsDefaultGroup) 
	VALUES (2,N'ITCCTEAM', 1, N'ITCCTEAM', N'System', GETDATE(), N'System', GETDATE(),1)

SET IDENTITY_INSERT [dbo].[DI_M_Group] OFF

--------PasswordPolicy
SET IDENTITY_INSERT [dbo].[DI_M_PasswordPolicy] ON 

IF NOT EXISTS (SELECT * FROM [DI_M_PasswordPolicy] WHERE Id=1 AND [CreatedBy] = 'System')
	INSERT [dbo].[DI_M_PasswordPolicy] ([Id], [Name], [Description], [PasswordLength], [PasswordMinimumLength], [PasswordMaximumLength], [NoOfSpecialCharacters], [SpecialCharNotAllowed], [NoOfLowerCaseAlphabets], [NoOfUpperCaseAlphabets], [AlphabetsNotAllowed], [NoOfNumericCharacters], [NumericCharacterNotAllowed], [NotAllowedCharacters], [NoOfDaysPasswordChangeAllowed], [NotAllowedKeywords], [UserNameNotAllowedInPassword], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
	VALUES (1, N'IACM_DEFAULT_POLICY', N'Password Policy For IACM', NULL, 8, 10, 1, 0, 1, 1, 0, 1, 0, N'', NULL, N'', 0, 1, N'System', GETDATE(), NULL, NULL)

SET IDENTITY_INSERT [dbo].[DI_M_PasswordPolicy] OFF


