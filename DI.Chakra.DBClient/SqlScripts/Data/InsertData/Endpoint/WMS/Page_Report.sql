DECLARE @PageId bigint
DECLARE @UrlId bigint
DECLARE @TabParentMenuId bigint

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_WMS','WMS' ,NULL ,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, NULL,N'System', getdate(), N'System', getdate())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] , [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('/Report/Endpoint/WMS/Users.aspx' ,N'System', getdate(), N'System', getdate())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] , [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('/Report/Endpoint/WMS/Roles.aspx' ,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] , [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('/Report/Endpoint/WMS/Functions.aspx' ,N'System', getdate(), N'System', getdate())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] , [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('/Report/Endpoint/WMS/PrivillegeAcctTrans.aspx' ,N'System', getdate(), N'System', getdate())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate ) VALUES ('/Report/Endpoint/WMS/RoleUser.aspx' ,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate ) VALUES ('/Report/Endpoint/WMS/UserRole.aspx' ,N'System', getdate(), N'System', getdate())


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate ) VALUES ('/Report/Endpoint/WMS/RoleFunction.aspx' ,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate ) VALUES ('/Report/Endpoint/WMS/FunctionRole.aspx' ,N'System', getdate(), N'System', getdate())



SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/WMS/Users.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_WMS_USERS','Users' ,@UrlId ,N'System', getdate(), N'System', getdate())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/WMS/Roles.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_WMS_ROLES','Roles' ,@UrlId ,N'System', getdate(), N'System', getdate())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/WMS/Functions.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_WMS_FUNCTIONS','Functions' ,@UrlId ,N'System', getdate(), N'System', getdate())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/WMS/PrivillegeAcctTrans.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_WMS_PRIVILLEGEACCTTRANS','PrivillegeAcctTrans' ,@UrlId ,N'System', getdate(), N'System', getdate())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/WMS/RoleUser.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_WMS_ROLEUSER','RoleUser' ,@UrlId,N'System', getdate(), N'System', getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/WMS/UserRole.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_WMS_USERROLE','UserRole' ,@UrlId,N'System', getdate(), N'System', getdate() )


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/WMS/RoleFunction.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_WMS_ROLEFUNCTION','RoleFunction' ,@UrlId,N'System', getdate(), N'System', getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/WMS/FunctionRole.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_WMS_FUNCTIONROLE','FunctionRole' ,@UrlId,N'System', getdate(), N'System', getdate() )


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS_USERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/WMS/Users.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS_ROLES'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/WMS/Roles.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS_FUNCTIONS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/WMS/Functions.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS_PRIVILLEGEACCTTRANS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/WMS/PrivillegeAcctTrans.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())




SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS_ROLEUSER'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/WMS/RoleUser.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS_USERROLE'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/WMS/UserRole.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS_ROLEFUNCTION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/WMS/RoleFunction.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS_FUNCTIONROLE'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/WMS/FunctionRole.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_ENDPOINT'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_WMS','WMS' , @PageId,N'System', getdate(), N'System', getdate() )

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_WMS'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS_USERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_WMS_USERS','Users' , @PageId ,N'System', getdate(), N'System', getdate())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS_ROLES'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_WMS_ROLES','Roles	' , @PageId ,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS_FUNCTIONS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_WMS_FUNCTIONS','Functions' , @PageId ,N'System', getdate(), N'System', getdate())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS_PRIVILLEGEACCTTRANS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_WMS_PRIVILLEGEACCTTRANS','PrivillegeAcctTrans', @PageId ,N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS_ROLEUSER'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_WMS_ROLEUSER','RoleUser' , @PageId ,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS_USERROLE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_WMS_USERROLE','UserRole' , @PageId ,N'System', getdate(), N'System', getdate())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS_ROLEFUNCTION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_WMS_ROLEFUNCTION','RoleFunction' , @PageId ,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS_FUNCTIONROLE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_WMS_FUNCTIONROLE','FunctionRole' , @PageId ,N'System', getdate(), N'System', getdate())




--- Accounts With No  Roles----


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/WMS/AccountsWithNoRoles.aspx',N'System', getdate(), N'System', getdate() )


SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/WMS/AccountsWithNoRoles.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_WMS_ACCOUNTSWITHNOROLES', 'CMS Accounts With No Roles', @UrlId, N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS_ACCOUNTSWITHNOROLES'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/WMS/AccountsWithNoRoles.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_WMS'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS_ACCOUNTSWITHNOROLES'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_WMS_ACCOUNTSWITHNOROLES','Accounts With No Roles' , @PageId,N'System', getdate(), N'System', getdate() )


--- Roles With No Accounts ----

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/WMS/RolesWithNoAccounts.aspx',N'System', getdate(), N'System', getdate() )


SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/WMS/RolesWithNoAccounts.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_WMS_ROLESWITHNOACCOUNTS', 'CMS Roles With No Accounts', @UrlId, N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS_ROLESWITHNOACCOUNTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/WMS/RolesWithNoAccounts.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_WMS'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS_ROLESWITHNOACCOUNTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_WMS_ROLESWITHNOACCOUNTS','Roles With No Accounts' , @PageId,N'System', getdate(), N'System', getdate() )


--- Roles With No Functions ----

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/WMS/RolesWithNoFunctions.aspx',N'System', getdate(), N'System', getdate() )


SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/WMS/RolesWithNoFunctions.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_WMS_ROLESWITHNOFUNCTIONS', 'CMS Roles With No Functions', @UrlId, N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS_ROLESWITHNOFUNCTIONS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/WMS/RolesWithNoFunctions.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_WMS'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS_ROLESWITHNOFUNCTIONS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_WMS_ROLESWITHNOFUNCTIONS','Roles With No Functions' , @PageId,N'System', getdate(), N'System', getdate() )


--- Dormant Account ----

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/WMS/DormantAccounts.aspx',N'System', getdate(), N'System', getdate() )


SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/WMS/DormantAccounts.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_WMS_DORMANTACCOUNT', 'CMS Dormant Accounts', @UrlId, N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS_DORMANTACCOUNT'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/WMS/DormantAccounts.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_WMS'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_WMS_DORMANTACCOUNT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_WMS_DORMANTACCOUNT','Dormant Accounts' , @PageId,N'System', getdate(), N'System', getdate() )







---AccessRights
DECLARE @PagePermissionId int
DECLARE @RIDRole int 
--SELECT @RIDRole = Id from DI_M_Role where RoleName='Endpoint Administrator' 
--DECLARE c1 CURSOR READ_ONLY
--FOR
--SELECT pageperm.Id
--FROM [DI_M_PagePermission] pageperm
--inner join DI_M_Page pag on pag.Id = pageperm.PageId
--where pag.uniquename in ('REPORT_WMS','REPORT_WMS_USERS','REPORT_WMS_ROLES','REPORT_WMS_FUNCTIONS','REPORT_WMS_PRIVILLEGEACCTTRANS','REPORT_WMS_ROLEUSER','REPORT_WMS_USERROLE','REPORT_WMS_ROLEFUNCTION','REPORT_WMS_FUNCTIONROLE')

--OPEN c1
 
--FETCH NEXT FROM c1
--INTO @PagePermissionId

--WHILE @@FETCH_STATUS = 0
--BEGIN

--INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDRole ,  @PagePermissionId,'System' , getdate() ,'System' , getdate())

--FETCH NEXT FROM c1
--INTO @PagePermissionId

--END

--CLOSE c1
--DEALLOCATE c1

SELECT @RIDRole = Id from DI_M_Role where RoleName='Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT pageperm.Id
FROM [DI_M_PagePermission] pageperm
inner join DI_M_Page pag on pag.Id = pageperm.PageId
where pag.uniquename in ('REPORT_WMS','REPORT_WMS_USERS','REPORT_WMS_ROLES','REPORT_WMS_FUNCTIONS','REPORT_WMS_PRIVILLEGEACCTTRANS','REPORT_WMS_ROLEUSER','REPORT_WMS_USERROLE','REPORT_WMS_ROLEFUNCTION','REPORT_WMS_FUNCTIONROLE')

OPEN c1
 
FETCH NEXT FROM c1
INTO @PagePermissionId

WHILE @@FETCH_STATUS = 0
BEGIN

INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDRole ,  @PagePermissionId,'System' , getdate() ,'System' , getdate())

FETCH NEXT FROM c1
INTO @PagePermissionId

END

CLOSE c1
DEALLOCATE c1


GO