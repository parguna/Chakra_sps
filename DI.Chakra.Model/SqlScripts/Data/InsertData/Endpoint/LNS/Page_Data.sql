DECLARE @PageId int
DECLARE @UrlId int
DECLARE @TabParentMenuId int
DECLARE @PagePermissionId int
DECLARE @RIDRole int 

 
 --- URL
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Configuration/Data/Endpoint/LotusNotes/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Configuration/Data/Endpoint/LotusNotes/LNSCatalog/List.aspx' )


---Page
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#' 
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('DATA_LNS','Lotus Notes' ,@UrlId )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DATA_LNS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Configuration/Data/Endpoint/LotusNotes/List.aspx' 
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('DATA_LNS_UPLOAD','Lotus Notes Data' ,@UrlId )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Configuration/Data/Endpoint/LotusNotes/LNSCatalog/List.aspx' 
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('DATA_LNS_CATALOG','Lotus Notes Catalog' ,@UrlId )

---Pagepermission
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DATA_LNS_UPLOAD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 17, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DATA_LNS_CATALOG'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 17, NULL)

-----Menu
SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='DATA'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DATA_LNS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'DATA_LNS' ,'Lotus Notes' , @PageId )

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='DATA_LNS'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DATA_LNS_UPLOAD'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'DATA_LNS_UPLOAD' ,'Lotus Notes Data' , @PageId )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DATA_LNS_CATALOG'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'DATA_LNS_CATALOG' ,'Lotus Notes catalog' , @PageId )

----AccessRights
SELECT @RIDRole = Id from DI_M_Role where RoleName='Endpoint Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT pageperm.Id
FROM [DI_M_PagePermission] pageperm
inner join DI_M_Page pag on pag.Id = pageperm.PageId
where pag.uniquename in ('DATA_LNS','DATA_LNS_UPLOAD','DATA_LNS_CATALOG')

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
where pag.uniquename in ('DATA_LNS','DATA_LNS_UPLOAD','DATA_LNS_CATALOG')

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