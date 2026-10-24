delete from DI_M_DefaultRoleDashboard
delete from DI_M_UserDashboardPreference
declare @tblRoles table(id int identity, roleId int)

insert into @tblRoles
select Id from DI_M_Role

declare @i int = 1;
declare @len int;
select @len = count(1) from @tblRoles
declare @roleId int
while(@i <= @len)
begin
select @roleId = roleId from @tblRoles where id = @i

if not exists(select 1 from DI_M_DefaultRoleDashboard where RoleId = @roleId and DashBoardId = 5)
begin
INSERT INTO DI_M_DefaultRoleDashboard (RoleId,Position,DashBoardId, CreatedBy, CreatedDate)
values (@roleId, 1, 5, 'System', GETDATE())
end

if not exists(select 1 from DI_M_DefaultRoleDashboard where RoleId = @roleId and DashBoardId = 3)
begin
INSERT INTO DI_M_DefaultRoleDashboard (RoleId,Position,DashBoardId, CreatedBy, CreatedDate)
values (@roleId, 2, 3, 'System', GETDATE())
end

if not exists(select 1 from DI_M_DefaultRoleDashboard where RoleId = @roleId and DashBoardId = 3)
begin
INSERT INTO DI_M_DefaultRoleDashboard (RoleId,Position,DashBoardId, CreatedBy, CreatedDate)
values (@roleId, 2, 14, 'System', GETDATE())
end

set @i = @i + 1

end

update DI_M_Page set name = 'New Access Request' where Name = 'New Acccess Request'
update DI_M_TabMenu set MenuName = 'New Access Request' where MenuName = 'New Acccess Request'