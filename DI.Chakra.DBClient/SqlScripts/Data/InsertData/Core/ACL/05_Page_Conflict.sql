DECLARE @PageId bigint
DECLARE @UrlId bigint
DECLARE @TabParentMenuId bigint

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE'

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ,[CreatedDate]) VALUES ('/Comp/Conflict/List.aspx',SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ,[CreatedDate]) VALUES ('/Comp/Conflict/Add.aspx',SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Comp/Conflict/Edit.aspx',SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ,[CreatedDate]) VALUES ('/Comp/Conflict/Detail.aspx' ,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ,[CreatedDate]) VALUES ('/Comp/Conflict/GeneratePair.aspx',SYSDATETIME() )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Conflict/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('COMPLIANCE_CONFLICT','Compliance Conflicts' ,@UrlId,SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Conflict/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('COMPLIANCE_CONFLICT_ADD','Add Compliance Conflict' ,@UrlId,SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Conflict/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('COMPLIANCE_CONFLICT_EDIT','Edit Compliance Conflict' ,@UrlId ,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Conflict/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('COMPLIANCE_CONFLICT_Detail','Compliance Conflict Detail' ,@UrlId,SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Conflict/GeneratePair.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('COMPLIANCE_CONFLICT_Pair','Generate Pair' ,@UrlId,SYSDATETIME() )


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_CONFLICT'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Conflict/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Conflict/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Conflict/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 3, @UrlId ,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Conflict/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 5, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Conflict/GeneratePair.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 13, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Conflict/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_CONFLICT_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_CONFLICT_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_CONFLICT_Detail'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_CONFLICT_Pair'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 6, NULL,SYSDATETIME())
 --MENU

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='COMPLIANCE'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_CONFLICT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'COMPLIANCE_CONFLICT', 'Conflicts' , @PageId,SYSDATETIME() )
