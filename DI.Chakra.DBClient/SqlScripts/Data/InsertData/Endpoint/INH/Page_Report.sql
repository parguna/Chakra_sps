DECLARE @PageId bigint
DECLARE @UrlId bigint
DECLARE @TabParentMenuId bigint

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_INH','Inhouse' ,NULL,N'System', getdate(), N'System' , getdate())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_INH'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, NULL,N'System', getdate(), N'System', getdate())

--INH
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/INH/Users.aspx' ,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/INH/Permissions.aspx',N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/INH/Functions.aspx',N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/INH/UserPermissions.aspx' ,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/INH/PermissionUsers.aspx',N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/INH/PermissionFunctions.aspx',N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/INH/FunctionPermissions.aspx',N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/INH/AccountsWithNoPermission.aspx',N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/INH/PermissionsWithNoAccounts.aspx',N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/INH/PermissionsWithNoFunctions.aspx',N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/INH/FunctionWithNoPermissions.aspx',N'System', getdate(), N'System', getdate() )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/INH/Users.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_INH_USERS','Inhouse Users' ,@UrlId,N'System', getdate(), N'System', getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/INH/Permissions.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_INH_PERMISSIONS','Inhouse Permissions' ,@UrlId,N'System', getdate(), N'System', getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/INH/Functions.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_INH_FUNCTIONS','Inhouse Functions' ,@UrlId,N'System', getdate(), N'System', getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/INH/UserPermissions.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_INH_USERPERMISSIONS','Inhouse User Permissions' ,@UrlId,N'System', getdate(), N'System', getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/INH/PermissionUsers.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_INH_PERMISSIONUSERS','Inhouse Permission Users' ,@UrlId ,N'System', getdate(), N'System', getdate())
SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/INH/PermissionFunctions.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_INH_PERMISSIONFUNCTIONS', 'Inhouse Permission Functions', @UrlId, N'System', getdate(), N'System', getdate())
SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/INH/FunctionPermissions.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_INH_FUNCTIONPERMISSIONS', 'Inhouse Function Permissions', @UrlId, N'System', getdate(), N'System', getdate())
SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/INH/AccountsWithNoPermission.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_INH_ACCOUNTSWITHNOPERMISSION', 'Inhouse Accounts With No Permission', @UrlId, N'System', getdate(), N'System', getdate())
SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/INH/PermissionsWithNoAccounts.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_INH_PERMISSIONSWITHNOACCOUNTS', 'Inhouse Permissions With No Accounts', @UrlId, N'System', getdate(), N'System', getdate())
SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/INH/PermissionsWithNoFunctions.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_INH_PERMISSIONSWITHNOFUNCTIONS', 'Inhouse Permissions With No Functions', @UrlId, N'System', getdate(), N'System', getdate())
SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/INH/FunctionWithNoPermissions.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_INH_FUNCTIONWITHNOPERMISSIONS', 'Inhouse Function With No Permissions', @UrlId, N'System', getdate(), N'System', getdate())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_INH_USERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/INH/Users.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_INH_PERMISSIONS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/INH/Permissions.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

 

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_INH_FUNCTIONS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/INH/Functions.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

 

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_INH_USERPERMISSIONS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/INH/UserPermissions.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

 

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_INH_PERMISSIONUSERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/INH/PermissionUsers.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

 

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_INH_PERMISSIONFUNCTIONS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/INH/PermissionFunctions.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

 

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_INH_FUNCTIONPERMISSIONS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/INH/FunctionPermissions.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

 

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_INH_ACCOUNTSWITHNOPERMISSION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/INH/AccountsWithNoPermission.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

 

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_INH_PERMISSIONSWITHNOACCOUNTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/INH/PermissionsWithNoAccounts.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

 


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_INH_PERMISSIONSWITHNOFUNCTIONS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/INH/PermissionsWithNoFunctions.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

 


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_INH_FUNCTIONWITHNOPERMISSIONS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/INH/FunctionWithNoPermissions.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_ENDPOINT'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_INH'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_INH','Inhouse' , @PageId,N'System', getdate(), N'System' , getdate())

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_INH'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_INH_USERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_INH_USERS','Inhouse Users' , @PageId,N'System', getdate(), N'System', getdate() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_INH_PERMISSIONS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_INH_PERMISSIONS','Inhouse Permissions' , @PageId ,N'System', getdate(), N'System', getdate())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_INH_FUNCTIONS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_INH_FUNCTIONS','Inhouse Functions' , @PageId ,N'System', getdate(), N'System', getdate())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_INH_USERPERMISSIONS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_INH_USERPERMISSIONS','Inhouse User Permissions' , @PageId,N'System', getdate(), N'System' , getdate())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_INH_PERMISSIONUSERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_INH_PERMISSIONUSERS','Inhouse Permission Users' , @PageId,N'System', getdate(), N'System', getdate() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_INH_PERMISSIONFUNCTIONS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_INH_PERMISSIONFUNCTIONS','Inhouse Permission Functions' , @PageId,N'System', getdate(), N'System', getdate() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_INH_FUNCTIONPERMISSIONS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_INH_FUNCTIONPERMISSIONS','Inhouse Function Permissions' , @PageId,N'System', getdate(), N'System' , getdate())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_INH_ACCOUNTSWITHNOPERMISSION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_INH_ACCOUNTSWITHNOPERMISSION','Inhouse Accounts With No Permission' , @PageId,N'System', getdate(), N'System', getdate() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_INH_PERMISSIONSWITHNOACCOUNTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_INH_PERMISSIONSWITHNOACCOUNTS','Inhouse Permissions With No Accounts' , @PageId,N'System', getdate(), N'System', getdate() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_INH_PERMISSIONSWITHNOFUNCTIONS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_INH_PERMISSIONSWITHNOFUNCTIONS','Inhouse Permissions With No Functions' , @PageId,N'System', getdate(), N'System', getdate() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_INH_FUNCTIONWITHNOPERMISSIONS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_INH_FUNCTIONWITHNOPERMISSIONS','Inhouse Function With No Permissions' , @PageId,N'System', getdate(), N'System' , getdate())

---AccessRights
DECLARE @PagePermissionId int
DECLARE @RIDRole int 

--SELECT @RIDRole = Id from DI_M_Role where RoleName='Endpoint Administrator' 
--DECLARE c1 CURSOR READ_ONLY
--FOR
--SELECT pageperm.Id
--FROM [DI_M_PagePermission] pageperm
--inner join DI_M_Page pag on pag.Id = pageperm.PageId
--where pag.uniquename in ('REPORT_INH','REPORT_INH_USERS','REPORT_INH_PERMISSIONS','REPORT_INH_FUNCTIONS','REPORT_INH_USERPERMISSIONS','REPORT_INH_PERMISSIONUSERS','REPORT_INH_PERMISSIONFUNCTIONS','REPORT_INH_FUNCTIONPERMISSIONS','REPORT_INH_ACCOUNTSWITHNOPERMISSION','REPORT_INH_PERMISSIONSWITHNOACCOUNTS','REPORT_INH_PERMISSIONSWITHNOFUNCTIONS','REPORT_INH_FUNCTIONWITHNOPERMISSIONS' )

--OPEN c1
 
--FETCH NEXT FROM c1
--INTO @PagePermissionId

--WHILE @@FETCH_STATUS = 0
--BEGIN

--INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDRole ,  @PagePermissionId,'System' , GETDATE() ,'System' , GETDATE())

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
where pag.uniquename in ('REPORT_INH','REPORT_INH_USERS','REPORT_INH_PERMISSIONS','REPORT_INH_FUNCTIONS','REPORT_INH_USERPERMISSIONS','REPORT_INH_PERMISSIONUSERS','REPORT_INH_PERMISSIONFUNCTIONS','REPORT_INH_FUNCTIONPERMISSIONS','REPORT_INH_ACCOUNTSWITHNOPERMISSION','REPORT_INH_PERMISSIONSWITHNOACCOUNTS','REPORT_INH_PERMISSIONSWITHNOFUNCTIONS','REPORT_INH_FUNCTIONWITHNOPERMISSIONS' )

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
