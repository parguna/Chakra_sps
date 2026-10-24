SET IDENTITY_INSERT [dbo].[DI_M_MailServerConfiguration] ON 

INSERT [dbo].[DI_M_MailServerConfiguration] ([Id], [EmailID], [UserName], [Password], [HostName], [PortNumber], [EnableSSL], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (1, N'devteam@deepidentity.com', N'devteam@deepidentity.com', N'DBOE+Wn/lQgGTjENRwTT2j221yhqnLKdcKlS4gizNsw=', N'smtp.gmail.com', N'25', 1, N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL)
SET IDENTITY_INSERT [dbo].[DI_M_MailServerConfiguration] OFF