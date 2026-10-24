


DECLARE @PageId int
DECLARE @UrlId int
DECLARE @TabParentMenuId int
DECLARE @RIDRole int 
 DECLARE @PagePermissionId int
 
 --- URL
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Configuration/Data/Endpoint/ActiveDirectory/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Configuration/Data/Endpoint/ActiveDirectory/ADSCatalog/List.aspx' )


---Page
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#' 
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('DATA_ADS','Active Directory' ,@UrlId )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DATA_ADS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Configuration/Data/Endpoint/ActiveDirectory/List.aspx' 
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('DATA_ADS_UPLOAD','Active Directory Data' ,@UrlId )


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Configuration/Data/Endpoint/ActiveDirectory/ADSCatalog/List.aspx' 
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('DATA_ADS_CATALOG','Active Directory Catalog' ,@UrlId )


---Pagepermission
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DATA_ADS_UPLOAD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 17, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DATA_ADS_CATALOG'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 17, NULL)



-----Menu

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='DATA'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DATA_ADS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'DATA_ADS' ,'Active Directory' , @PageId )

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='DATA_ADS'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DATA_ADS_UPLOAD'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'DATA_ADS_UPLOAD' ,'Active Directory Data' , @PageId )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DATA_ADS_CATALOG'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'DATA_ADS_CATALOG' ,'Active Directory Catalog' , @PageId )


----AccessRights

SELECT @RIDRole = Id from DI_M_Role where RoleName='Endpoint Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT pageperm.Id
FROM [DI_M_PagePermission] pageperm
inner join DI_M_Page pag on pag.Id = pageperm.PageId
where pag.uniquename in ('DATA_ADS','DATA_ADS_UPLOAD','DATA_ADS_CATALOG')

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


SELECT @RIDRole = Id from DI_M_Role where RoleName='Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT pageperm.Id
FROM [DI_M_PagePermission] pageperm
inner join DI_M_Page pag on pag.Id = pageperm.PageId
where pag.uniquename in ('DATA_ADS','DATA_ADS_UPLOAD','DATA_ADS_CATALOG')

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
