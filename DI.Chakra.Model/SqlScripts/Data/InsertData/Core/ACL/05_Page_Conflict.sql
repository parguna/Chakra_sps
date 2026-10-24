DECLARE @PageId int
DECLARE @UrlId int
DECLARE @TabParentMenuId int

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE'

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Comp/Conflict/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Comp/Conflict/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Comp/Conflict/Edit.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Comp/Conflict/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Comp/Conflict/GeneratePair.aspx' )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Conflict/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('COMPLIANCE_CONFLICT','Compliance Conflict' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Conflict/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('COMPLIANCE_CONFLICT_ADD','Add Compliance Conflict' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Conflict/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('COMPLIANCE_CONFLICT_EDIT','Edit Compliance Conflict' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Conflict/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('COMPLIANCE_CONFLICT_Detail','Detail Compliance Conflict' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Conflict/GeneratePair.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('COMPLIANCE_CONFLICT_Pair','Generate Pair' ,@UrlId )


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_CONFLICT'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Conflict/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Conflict/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Conflict/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Conflict/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Conflict/GeneratePair.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 13, @UrlId)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_CONFLICT_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_CONFLICT_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_CONFLICT_Detail'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_CONFLICT_Pair'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
 --MENU

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='COMPLIANCE'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_CONFLICT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'COMPLIANCE_CONFLICT', 'Conflict' , @PageId )
