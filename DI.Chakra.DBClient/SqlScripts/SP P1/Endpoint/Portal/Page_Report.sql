DECLARE @PageId bigint
DECLARE @UrlId bigint
DECLARE @TabParentMenuId bigint

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_NPAC','NPAC' ,NULL,N'System', getdate(), N'System' , getdate())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, NULL,N'System', getdate(), N'System', getdate())

--NPAC
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/NPAC/Users.aspx' ,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/NPAC/Groups.aspx',N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/NPAC/OU.aspx',N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/NPAC/UserGroups.aspx' ,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/NPAC/ROLEUSERS.aspx',N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/NPAC/DormantAccounts.aspx',N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/NPAC/LockedDisabledAccounts.aspx',N'System', getdate(), N'System' , getdate())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/NPAC/AccountsWithNoGroups.aspx',N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/NPAC/GroupsWithNoAccounts.aspx',N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/NPAC/FuturePasswordExpiry.aspx',N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/NPAC/FutureAccountExpiry.aspx',N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Report/Endpoint/NPAC/GroupUsageByDepartment.aspx',SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Report/Endpoint/NPAC/ActiveAccountsWithInActiveUser.aspx',SYSDATETIME() )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/NPAC/Users.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_NPAC_USERS','Active Directory Users' ,@UrlId,N'System', getdate(), N'System', getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/NPAC/Groups.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_NPAC_GROUPS','Active Directory Groups' ,@UrlId,N'System', getdate(), N'System', getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/NPAC/OU.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_NPAC_OUS','Active Directory OUs' ,@UrlId,N'System', getdate(), N'System', getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/NPAC/UserGroups.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_NPAC_USERGROUPS','Active Directory User Groups' ,@UrlId,N'System', getdate(), N'System', getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/NPAC/ROLEUSERS.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_NPAC_ROLEUSERS','Active Directory Group Users' ,@UrlId ,N'System', getdate(), N'System', getdate())
SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/NPAC/DormantAccounts.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_NPAC_DORMANTACCOUNTS', 'Active Directory Dormant Accounts', @UrlId, N'System', getdate(), N'System', getdate())
SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/NPAC/LockedDisabledAccounts.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_NPAC_LOCKEDDISABLEDACCOUNTS', 'Active Directory Locked or Disabled Accounts', @UrlId, N'System', getdate(), N'System', getdate())
SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/NPAC/AccountsWithNoGroups.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_NPAC_ACCOUNTSWITHNOGROUPS', 'Active Directory Accounts With No Groups', @UrlId, N'System', getdate(), N'System', getdate())
SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/NPAC/GroupsWithNoAccounts.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_NPAC_GROUPSWITHNOACCOUNTS', 'Active Directory Groups With No Accounts', @UrlId, N'System', getdate(), N'System', getdate())
SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/NPAC/FuturePasswordExpiry.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_NPAC_FUTUREPASSWORDEXPIRYACCOUNTS', 'Active Directory Future Password Expiry Accounts', @UrlId, N'System', getdate(), N'System', getdate())
SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/NPAC/FutureAccountExpiry.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_NPAC_FUTUREACCOUNTEXPIRYACCOUNTS', 'Active Directory Future Expiry Accounts', @UrlId, N'System', getdate(), N'System', getdate())
SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/NPAC/GroupUsageByDepartment.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId],[CreatedDate])	VALUES ('REPORT_NPAC_GROUPSUSAGEBYDEPARTMENT', 'Groups Usage By Department', @UrlId, SYSDATETIME())
SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/NPAC/ActiveAccountsWithInActiveUser.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId],[CreatedDate])	VALUES ('REPORT_NPAC_ActiveAccountsWithInActiveUser', 'Inactive User with Active Accounts', @UrlId, SYSDATETIME())




SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC_USERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/NPAC/Users.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC_GROUPS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/NPAC/Groups.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

 

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC_OUS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/NPAC/OU.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

 

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC_USERGROUPS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/NPAC/UserGroups.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

 

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC_ROLEUSERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/NPAC/GroupUsers.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

 


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC_LOCKEDDISABLEDACCOUNTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/NPAC/LockedDisabledAccounts.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

 

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC_ACCOUNTSWITHNOGROUPS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/NPAC/AccountsWithNoGroups.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

 

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC_GROUPSWITHNOACCOUNTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/NPAC/GroupsWithNoAccounts.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

 


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC_FUTUREPASSWORDEXPIRYACCOUNTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/NPAC/FuturePasswordExpiry.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

 


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC_FUTUREACCOUNTEXPIRYACCOUNTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/NPAC/FutureAccountExpiry.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC_GROUPSUSAGEBYDEPARTMENT'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/NPAC/GroupUsageByDepartment.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC_ActiveAccountsWithInActiveUser'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/NPAC/ActiveAccountsWithInActiveUser.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_ENDPOINT'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_NPAC','Active Directory' , @PageId,N'System', getdate(), N'System' , getdate())

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_NPAC'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC_USERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_NPAC_USERS','Users' , @PageId,N'System', getdate(), N'System', getdate() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC_GROUPS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_NPAC_GROUPS','Groups' , @PageId ,N'System', getdate(), N'System', getdate())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC_OUS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_NPAC_OUS','OUs' , @PageId ,N'System', getdate(), N'System', getdate())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC_USERGROUPS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_NPAC_USERGROUPS','User Groups' , @PageId,N'System', getdate(), N'System' , getdate())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC_ROLEUSERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_NPAC_ROLEUSERS','Group Users' , @PageId,N'System', getdate(), N'System', getdate() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC_DORMANTACCOUNTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_NPAC_DORMANTACCOUNTS','Dormant Accounts' , @PageId,N'System', getdate(), N'System', getdate() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC_LOCKEDDISABLEDACCOUNTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_NPAC_LOCKEDDISABLEDACCOUNTS','Locked or Disabled Accounts' , @PageId,N'System', getdate(), N'System' , getdate())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC_ACCOUNTSWITHNOGROUPS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_NPAC_ACCOUNTSWITHNOGROUPS','Accounts With No Groups' , @PageId,N'System', getdate(), N'System', getdate() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC_GROUPSWITHNOACCOUNTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_NPAC_GROUPSWITHNOACCOUNTS','Groups With No Accounts' , @PageId,N'System', getdate(), N'System', getdate() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC_FUTUREPASSWORDEXPIRYACCOUNTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_NPAC_FUTUREPASSWORDEXPIRYACCOUNTS','Future Password Expiry Accounts' , @PageId,N'System', getdate(), N'System', getdate() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC_FUTUREACCOUNTEXPIRYACCOUNTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_NPAC_FUTUREACCOUNTEXPIRYACCOUNTS','Future Account Expiry Accounts' , @PageId,N'System', getdate(), N'System' , getdate())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC_GROUPSUSAGEBYDEPARTMENT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_NPAC_GROUPSUSAGEBYDEPARTMENT','Groups Usage By Department' , @PageId,SYSDATETIME() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_NPAC_ActiveAccountsWithInActiveUser'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_NPAC_ActiveAccountsWithInActiveUser','Inactive User with Active Accounts' , @PageId,SYSDATETIME() )


-----AccessRights
DECLARE @PagePermissionId int
DECLARE @RIDRole int 


SELECT @RIDRole = Id from DI_M_Role where RoleName='Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT pageperm.Id
FROM [DI_M_PagePermission] pageperm
inner join DI_M_Page pag on pag.Id = pageperm.PageId
where pag.uniquename in ('REPORT_NPAC','REPORT_NPAC_USERS','REPORT_NPAC_GROUPS','REPORT_NPAC_OUS','REPORT_NPAC_USERGROUPS','REPORT_NPAC_ROLEUSERS','REPORT_NPAC_DORMANTACCOUNTS','REPORT_NPAC_LOCKEDDISABLEDACCOUNTS', 'REPORT_NPAC_ACCOUNTSWITHNOGROUPS', 'REPORT_NPAC_GROUPSWITHNOACCOUNTS', 'REPORT_NPAC_FUTUREPASSWORDEXPIRYACCOUNTS', 'REPORT_NPAC_FUTUREACCOUNTEXPIRYACCOUNTS', 'REPORT_NPAC_GROUPSUSAGEBYDEPARTMENT', 'REPORT_NPAC_ActiveAccountsWithInActiveUser' )

OPEN c1
 
FETCH NEXT FROM c1
INTO @PagePermissionId

WHILE @@FETCH_STATUS = 0
BEGIN

INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDRole ,  @PagePermissionId,'System' , GETDATE() ,'System' , GETDATE())

FETCH NEXT FROM c1
INTO @PagePermissionId

END

CLOSE c1
DEALLOCATE c1
GO
