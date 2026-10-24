DECLARE @PageId bigint
DECLARE @UrlId bigint
DECLARE @TabParentMenuId bigint

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('COMPLIANCE','Compliance',NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Comp/Function/List.aspx',SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate]  ) VALUES ('/Comp/Function/Add.aspx' ,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate]  ) VALUES ('/Comp/Function/Edit.aspx',SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Comp/Function/Detail.aspx',SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Comp/Function/Clone.aspx',SYSDATETIME() )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Function/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId] ,[CreatedDate]) VALUES ('COMPLIANCE_FUNCTION','Compliance Function' ,@UrlId,SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Function/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('COMPLIANCE_FUNCTION_ADD','Add Compliance Function' ,@UrlId,SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Function/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('COMPLIANCE_FUNCTION_EDIT','Edit ComplianceFunction' ,@UrlId,SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Function/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('COMPLIANCE_FUNCTION_DETAIL','Detail Compliance Function' ,@UrlId,SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Function/Clone.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('COMPLIANCE_FUNCTION_CLONE','Clone Compliance Function' ,@UrlId,SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_FUNCTION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Function/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Function/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Function/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Function/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Function/Clone.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 16, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_FUNCTION_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL ,SYSDATETIME())
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,) VALUES( @PageId , 6, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_FUNCTION_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_FUNCTION_DETAIL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_FUNCTION_CLONE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
 
 --MENU

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (NULL, 'COMPLIANCE', 'Compliance' ,@PageId,SYSDATETIME() )

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='COMPLIANCE'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_FUNCTION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'COMPLIANCE_FUNCTION', 'Function' , @PageId,SYSDATETIME() )



