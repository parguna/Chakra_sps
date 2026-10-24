DECLARE @PageId int
DECLARE @UrlId int
DECLARE @TabParentMenuId int

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('RISK','Risk',NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='RISK'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Risk/Level/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Risk/Score/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Risk/Code/List.aspx' )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Risk/Level/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('RISK_LEVEL','Risk Level' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Risk/Score/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('RISK_SCORE','Risk Score' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Risk/Code/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('RISK_CODE','Risk Code' ,@UrlId )


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='RISK_LEVEL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='RISK_SCORE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='RISK_CODE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)




--MENU

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='RISK'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId]) VALUES (NULL, 'RISK', 'Risk' ,@PageId )

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='RISK'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='RISK_LEVEL'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId,'RISK_LEVEL', 'Level' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='RISK_SCORE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId,'RISK_SCORE', 'Score' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='RISK_CODE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'RISK_CODE', 'Code' , @PageId )