DECLARE @PageId bigint
DECLARE @UrlId bigint
DECLARE @TabParentMenuId bigint

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_Office365','Office365' ,NULL,N'System', getdate(), N'System' , getdate())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, NULL,N'System', getdate(), N'System', getdate())

--GEMS
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/Office365/Users.aspx' ,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/Office365/Groups.aspx',N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/Office365/OU.aspx',N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/Office365/UserGroups.aspx' ,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/Office365/ROLEUSERS.aspx',N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/Office365/DormantAccounts.aspx',N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/Office365/LockedDisabledAccounts.aspx',N'System', getdate(), N'System' , getdate())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/Office365/AccountsWithNoGroups.aspx',N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/Office365/GroupsWithNoAccounts.aspx',N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/Office365/FuturePasswordExpiry.aspx',N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/Office365/FutureAccountExpiry.aspx',N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Report/Endpoint/Office365/GroupUsageByDepartment.aspx',SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Report/Endpoint/Office365/ActiveAccountsWithInActiveUser.aspx',SYSDATETIME() )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/Office365/Users.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_Office365_USERS','Active Directory Users' ,@UrlId,N'System', getdate(), N'System', getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/Office365/Groups.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_Office365_GROUPS','Active Directory Groups' ,@UrlId,N'System', getdate(), N'System', getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/Office365/OU.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_Office365_OUS','Active Directory OUs' ,@UrlId,N'System', getdate(), N'System', getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/Office365/UserGroups.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_Office365_USERGROUPS','Active Directory User Groups' ,@UrlId,N'System', getdate(), N'System', getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/Office365/ROLEUSERS.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_Office365_ROLEUSERS','Active Directory Group Users' ,@UrlId ,N'System', getdate(), N'System', getdate())
SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/Office365/DormantAccounts.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_Office365_DORMANTACCOUNTS', 'Active Directory Dormant Accounts', @UrlId, N'System', getdate(), N'System', getdate())
SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/Office365/LockedDisabledAccounts.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_Office365_LOCKEDDISABLEDACCOUNTS', 'Active Directory Locked or Disabled Accounts', @UrlId, N'System', getdate(), N'System', getdate())
SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/Office365/AccountsWithNoGroups.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_Office365_ACCOUNTSWITHNOGROUPS', 'Active Directory Accounts With No Groups', @UrlId, N'System', getdate(), N'System', getdate())
SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/Office365/GroupsWithNoAccounts.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_Office365_GROUPSWITHNOACCOUNTS', 'Active Directory Groups With No Accounts', @UrlId, N'System', getdate(), N'System', getdate())
SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/Office365/FuturePasswordExpiry.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_Office365_FUTUREPASSWORDEXPIRYACCOUNTS', 'Active Directory Future Password Expiry Accounts', @UrlId, N'System', getdate(), N'System', getdate())
SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/Office365/FutureAccountExpiry.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_Office365_FUTUREACCOUNTEXPIRYACCOUNTS', 'Active Directory Future Expiry Accounts', @UrlId, N'System', getdate(), N'System', getdate())
SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/Office365/GroupUsageByDepartment.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId],[CreatedDate])	VALUES ('REPORT_Office365_GROUPSUSAGEBYDEPARTMENT', 'Groups Usage By Department', @UrlId, SYSDATETIME())
SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/Office365/ActiveAccountsWithInActiveUser.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId],[CreatedDate])	VALUES ('REPORT_Office365_ActiveAccountsWithInActiveUser', 'Inactive User with Active Accounts', @UrlId, SYSDATETIME())




SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365_USERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/Office365/Users.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365_GROUPS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/Office365/Groups.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

 

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365_OUS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/Office365/OU.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

 

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365_USERGROUPS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/Office365/UserGroups.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

 

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365_ROLEUSERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/Office365/GroupUsers.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

 


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365_LOCKEDDISABLEDACCOUNTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/Office365/LockedDisabledAccounts.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

 

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365_ACCOUNTSWITHNOGROUPS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/Office365/AccountsWithNoGroups.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

 

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365_GROUPSWITHNOACCOUNTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/Office365/GroupsWithNoAccounts.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

 


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365_FUTUREPASSWORDEXPIRYACCOUNTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/Office365/FuturePasswordExpiry.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

 


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365_FUTUREACCOUNTEXPIRYACCOUNTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/Office365/FutureAccountExpiry.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365_GROUPSUSAGEBYDEPARTMENT'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/Office365/GroupUsageByDepartment.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365_ActiveAccountsWithInActiveUser'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/Office365/ActiveAccountsWithInActiveUser.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_ENDPOINT'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_Office365','Active Directory' , @PageId,N'System', getdate(), N'System' , getdate())

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_Office365'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365_USERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_Office365_USERS','Users' , @PageId,N'System', getdate(), N'System', getdate() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365_GROUPS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_Office365_GROUPS','Groups' , @PageId ,N'System', getdate(), N'System', getdate())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365_OUS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_Office365_OUS','OUs' , @PageId ,N'System', getdate(), N'System', getdate())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365_USERGROUPS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_Office365_USERGROUPS','User Groups' , @PageId,N'System', getdate(), N'System' , getdate())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365_ROLEUSERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_Office365_ROLEUSERS','Group Users' , @PageId,N'System', getdate(), N'System', getdate() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365_DORMANTACCOUNTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_Office365_DORMANTACCOUNTS','Dormant Accounts' , @PageId,N'System', getdate(), N'System', getdate() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365_LOCKEDDISABLEDACCOUNTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_Office365_LOCKEDDISABLEDACCOUNTS','Locked or Disabled Accounts' , @PageId,N'System', getdate(), N'System' , getdate())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365_ACCOUNTSWITHNOGROUPS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_Office365_ACCOUNTSWITHNOGROUPS','Accounts With No Groups' , @PageId,N'System', getdate(), N'System', getdate() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365_GROUPSWITHNOACCOUNTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_Office365_GROUPSWITHNOACCOUNTS','Groups With No Accounts' , @PageId,N'System', getdate(), N'System', getdate() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365_FUTUREPASSWORDEXPIRYACCOUNTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_Office365_FUTUREPASSWORDEXPIRYACCOUNTS','Future Password Expiry Accounts' , @PageId,N'System', getdate(), N'System', getdate() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365_FUTUREACCOUNTEXPIRYACCOUNTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_Office365_FUTUREACCOUNTEXPIRYACCOUNTS','Future Account Expiry Accounts' , @PageId,N'System', getdate(), N'System' , getdate())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365_GROUPSUSAGEBYDEPARTMENT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_Office365_GROUPSUSAGEBYDEPARTMENT','Groups Usage By Department' , @PageId,SYSDATETIME() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Office365_ActiveAccountsWithInActiveUser'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_Office365_ActiveAccountsWithInActiveUser','Inactive User with Active Accounts' , @PageId,SYSDATETIME() )


-----AccessRights
DECLARE @PagePermissionId int
DECLARE @RIDRole int 


SELECT @RIDRole = Id from DI_M_Role where RoleName='Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT pageperm.Id
FROM [DI_M_PagePermission] pageperm
inner join DI_M_Page pag on pag.Id = pageperm.PageId
where pag.uniquename in ('REPORT_Office365','REPORT_Office365_USERS','REPORT_Office365_GROUPS','REPORT_Office365_OUS','REPORT_Office365_USERGROUPS','REPORT_Office365_ROLEUSERS','REPORT_Office365_DORMANTACCOUNTS','REPORT_Office365_LOCKEDDISABLEDACCOUNTS', 'REPORT_Office365_ACCOUNTSWITHNOGROUPS', 'REPORT_Office365_GROUPSWITHNOACCOUNTS', 'REPORT_Office365_FUTUREPASSWORDEXPIRYACCOUNTS', 'REPORT_Office365_FUTUREACCOUNTEXPIRYACCOUNTS', 'REPORT_Office365_GROUPSUSAGEBYDEPARTMENT', 'REPORT_Office365_ActiveAccountsWithInActiveUser' )

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
