declare @uniqueName nvarchar(100) = 'HOME_MYPROFILE'
declare @permissionsToDelete nvarchar(100) = '6'

declare @pageId int
select @pageId = PageId from DI_M_TabMenu where UniqueName = @uniqueName
delete from DI_M_AccessRights where PagePermissionId in (select Id from DI_M_PagePermission where PageId = @pageId and PermissionId in (select Item from dbo.Split(@permissionsToDelete, ',')))
delete from DI_M_PagePermission where PageId = @pageId and PermissionId in (select Item from dbo.Split(@permissionsToDelete, ','))

------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
go
declare @uniqueName nvarchar(100) = 'HOME_MYREQUESTS'
declare @permissionsToDelete nvarchar(100) = '2,6,36'

declare @pageId int
select @pageId = PageId from DI_M_TabMenu where UniqueName = @uniqueName
delete from DI_M_AccessRights where PagePermissionId in (select Id from DI_M_PagePermission where PageId = @pageId and PermissionId in (select Item from dbo.Split(@permissionsToDelete, ',')))
delete from DI_M_PagePermission where PageId = @pageId and PermissionId in (select Item from dbo.Split(@permissionsToDelete, ','))

------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
go
declare @uniqueName nvarchar(100) = 'ADMIN_PROVISIONING_MYREQUESTS'
declare @permissionsToDelete nvarchar(100) = '2,6,19'

declare @pageId int
select @pageId = PageId from DI_M_TabMenu where UniqueName = @uniqueName
delete from DI_M_AccessRights where PagePermissionId in (select Id from DI_M_PagePermission where PageId = @pageId and PermissionId in (select Item from dbo.Split(@permissionsToDelete, ',')))
delete from DI_M_PagePermission where PageId = @pageId and PermissionId in (select Item from dbo.Split(@permissionsToDelete, ','))

------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
go
declare @uniqueName nvarchar(100) = 'USER_EMPLOYEE'
declare @permissionsToDelete nvarchar(100) = '2,3,4,6'

declare @pageId int
select @pageId = PageId from DI_M_TabMenu where UniqueName = @uniqueName
delete from DI_M_AccessRights where PagePermissionId in (select Id from DI_M_PagePermission where PageId = @pageId and PermissionId in (select Item from dbo.Split(@permissionsToDelete, ',')))
delete from DI_M_PagePermission where PageId = @pageId and PermissionId in (select Item from dbo.Split(@permissionsToDelete, ','))

------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
go
declare @uniqueName nvarchar(100) = 'USER_NONEMPLOYEE'
declare @permissionsToDelete nvarchar(100) = '2,4,6,37'

declare @pageId int
select @pageId = PageId from DI_M_TabMenu where UniqueName = @uniqueName
delete from DI_M_AccessRights where PagePermissionId in (select Id from DI_M_PagePermission where PageId = @pageId and PermissionId in (select Item from dbo.Split(@permissionsToDelete, ',')))
delete from DI_M_PagePermission where PageId = @pageId and PermissionId in (select Item from dbo.Split(@permissionsToDelete, ','))

------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
go
declare @uniqueName nvarchar(100) = 'HOME_MYTASK'
declare @permissionsToDelete nvarchar(100) = '2'

declare @pageId int
select @pageId = PageId from DI_M_TabMenu where UniqueName = @uniqueName
delete from DI_M_AccessRights where PagePermissionId in (select Id from DI_M_PagePermission where PageId = @pageId and PermissionId in (select Item from dbo.Split(@permissionsToDelete, ',')))
delete from DI_M_PagePermission where PageId = @pageId and PermissionId in (select Item from dbo.Split(@permissionsToDelete, ','))

------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
go
declare @uniqueName nvarchar(100) = 'USER_ACCESS_DETAIL_MATRIX'
declare @permissionsToDelete nvarchar(100) = '2'

declare @pageId int
select @pageId = PageId from DI_M_TabMenu where UniqueName = @uniqueName
delete from DI_M_AccessRights where PagePermissionId in (select Id from DI_M_PagePermission where PageId = @pageId and PermissionId in (select Item from dbo.Split(@permissionsToDelete, ',')))
delete from DI_M_PagePermission where PageId = @pageId and PermissionId in (select Item from dbo.Split(@permissionsToDelete, ','))

------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

go
declare @uniqueName nvarchar(100) = 'ADMIN_TRIGGERS'
declare @permissionsToDelete nvarchar(100) = '2'

declare @pageId int
select @pageId = PageId from DI_M_TabMenu where UniqueName = @uniqueName
delete from DI_M_AccessRights where PagePermissionId in (select Id from DI_M_PagePermission where PageId = @pageId and PermissionId in (select Item from dbo.Split(@permissionsToDelete, ',')))
delete from DI_M_PagePermission where PageId = @pageId and PermissionId in (select Item from dbo.Split(@permissionsToDelete, ','))

------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
update DI_M_TabMenu set MenuName = 'New Acccess Request' where UniqueName = 'HOME_ACCESSREQUESTS'
update DI_M_Page set Name = 'New Acccess Request' where UniqueName = 'HOME_ACCESSREQUESTS'