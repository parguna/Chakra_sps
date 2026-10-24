
DECLARE @PageId int
DECLARE @UrlId int
DECLARE @TabParentMenuId int

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('DATA','Data',@UrlId )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DATA'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (NULL, 'DATA','Data' , @PageId )


INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT','Report',NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (NULL, 'REPORT','Report' , @PageId )

--Report Submenu's
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_USER','User Report',NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_USER'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_USER','User' , @PageId )

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_ENDPOINT','Endpoint Systems Report',NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ENDPOINT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_ENDPOINT','Endpoint System' , @PageId )


INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_ATTESTATION','Attestation Report',NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ATTESTATION'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_ATTESTATION','Attestation' , @PageId )

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_PROVISIONING','Provisioning Report',NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PROVISIONING'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_PROVISIONING','Provisioning' , @PageId )

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_SECURITY','Security Report',NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_SECURITY'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_SECURITY','Security' , @PageId )

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_AUDIT','Audit Report',NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_AUDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_AUDIT','Audit' , @PageId )

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_COMPLIANCE','Compliance Report',NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_COMPLIANCE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_COMPLIANCE','Compliance' , @PageId )

---REPORT_USER Submenu's
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_ALLUSER','All User Report',NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ALLUSER'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_USER'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_ALLUSER','AllUser' , @PageId )

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_USERROLE','UserRole Report',NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_USERROLE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_USER'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_USERROLE','User - Role' , @PageId )

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_USERGROUP','UserGroup Report',NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_USERGROUP'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_USER'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_USERGROUP','User - Group' , @PageId )

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_ACTIVEEMPLOYEES','All User Report',NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ACTIVEEMPLOYEES'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_USER'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_ACTIVEEMPLOYEES','Active Employees' , @PageId )

