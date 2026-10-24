DECLARE @PageId int
DECLARE @UrlId int
DECLARE @TabParentMenuId int
DECLARE @RIDRole int 
DECLARE @PagePermissionId int

---URL
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/InHouse/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/InHouse/Permission/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Configuration/Data/Endpoint/INH/INHCatalog/List.aspx' )

---Page
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#' 
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('DATA_INH','Inhouse Data' ,@UrlId )


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DATA_INH'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/List.aspx' 
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('DATA_INH_USER','Inhouse User' ,@UrlId )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Permission/List.aspx' 
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('DATA_INH_ENTITLEMENT','Inhouse Entitlement' ,@UrlId )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Configuration/Data/Endpoint/INH/INHCatalog/List.aspx' 
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('DATA_INH_CATALOG','InHouse Catalog' ,@UrlId )

---PagePermisssion

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DATA_INH_USER'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 17, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DATA_INH_ENTITLEMENT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 17, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DATA_INH_CATALOG'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 17, NULL)

----Menu
SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='DATA'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DATA_INH'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'DATA_INH' ,'Inhouse Data' , @PageId )

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='DATA_INH'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DATA_INH_USER'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'DATA_INH_USER' ,'Inhouse User' , @PageId )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DATA_INH_ENTITLEMENT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'DATA_INH_ENTITLEMENT' ,'Inhouse Entitlement' , @PageId )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DATA_INH_CATALOG'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'DATA_INH_CATALOG' ,'InHouse Catalog' , @PageId )

----AccessRights

SELECT @RIDRole = Id from DI_M_Role where RoleName='Endpoint Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT pageperm.Id
FROM [DI_M_PagePermission] pageperm
inner join DI_M_Page pag on pag.Id = pageperm.PageId
where pag.uniquename in ('DATA_INH','DATA_INH_USER','DATA_INH_ENTITLEMENT','DATA_INH_CATALOG')

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
where pag.uniquename in ('DATA_INH','DATA_INH_USER','DATA_INH_ENTITLEMENT','DATA_INH_CATALOG')

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
