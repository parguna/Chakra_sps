DECLARE @PageId bigint
DECLARE @UrlId bigint
DECLARE @TabParentMenuId bigint

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='MITIGATION'

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Comp/Mitigation/List.aspx',SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Comp/Mitigation/Add.aspx',SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ,[CreatedDate]) VALUES ('/Comp/Mitigation/Edit.aspx' ,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Comp/Mitigation/Detail.aspx',SYSDATETIME() )


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Mitigation/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('COMPLIANCE_MITIGATION','Mitigation' ,@UrlId ,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Mitigation/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('COMPLIANCE_MITIGATION_ADD','Add Mitigation' ,@UrlId ,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Mitigation/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('COMPLIANCE_MITIGATION_EDIT','Edit Mitigation' ,@UrlId ,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Mitigation/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('COMPLIANCE_MITIGATION_Detail','Detail Mitigation' ,@UrlId ,SYSDATETIME())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_MITIGATION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Mitigation/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Mitigation/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 3, @UrlId,SYSDATETIME() )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Mitigation/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 5, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Mitigation/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 2, @UrlId,SYSDATETIME())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_MITIGATION_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_MITIGATION_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_MITIGATION_Detail'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())


 --MENU

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='COMPLIANCE'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_MITIGATION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'COMPLIANCE_MITIGATION', 'Mitigation' , @PageId,SYSDATETIME() )
