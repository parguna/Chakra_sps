
Delete [DI_M_Role]
DBCC CHECKIDENT ([DI_M_Role], reseed, 0)

SET IDENTITY_INSERT [dbo].[DI_M_Role] ON
INSERT INTO [dbo].[DI_M_Role](Id,[RoleName],[RoleDescription],[CreatedBy],[CreatedDate],[LastModifiedBy],[LastModifiedDate],IsDefaultRole)     VALUES(1,'Administrator','All Administrators','System',GETDATE(),'System',GETDATE(),1)
INSERT INTO [dbo].[DI_M_Role](Id,[RoleName],[RoleDescription],[CreatedBy],[CreatedDate],[LastModifiedBy],[LastModifiedDate],IsDefaultRole)     VALUES(2,'User','All Users','System',GETDATE(),'System',GETDATE(),1)
INSERT INTO [dbo].[DI_M_Role](Id,[RoleName],[RoleDescription],[CreatedBy],[CreatedDate],[LastModifiedBy],[LastModifiedDate],IsDefaultRole)     VALUES(3,'Attestation Administrator','','System',GETDATE(),'System',GETDATE(),1)
INSERT INTO [dbo].[DI_M_Role](Id,[RoleName],[RoleDescription],[CreatedBy],[CreatedDate],[LastModifiedBy],[LastModifiedDate],IsDefaultRole)     VALUES(4,'Endpoint Administrator','','System',GETDATE(),'System',GETDATE(),1)
INSERT INTO [dbo].[DI_M_Role](Id,[RoleName],[RoleDescription],[CreatedBy],[CreatedDate],[LastModifiedBy],[LastModifiedDate],IsDefaultRole)     VALUES(5,'Provisioning Administrator','','System',GETDATE(),'System',GETDATE(),1)
SET IDENTITY_INSERT [dbo].[DI_M_Role] OFF

GO
-------------- Administrator 
print 'IACM admin'
DECLARE @UserId int
DECLARE @RIDRole int 
SELECT @RIDRole = Id from DI_M_Role where RoleName='Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT Id
FROM DI_M_User where username ='iacmadmin'

OPEN c1
 
FETCH NEXT FROM c1
INTO @UserId

WHILE @@FETCH_STATUS = 0
BEGIN

INSERT INTO dbo.UserRole(Role_RoleId,User_UserId)
VALUES(@RIDRole , @UserId)

FETCH NEXT FROM c1
INTO @UserId

END

CLOSE c1
DEALLOCATE c1
--End

GO


----Access Rights For Administrator
DECLARE @PagePermissionId int
DECLARE @RIDRole int 
SELECT @RIDRole = Id from DI_M_Role where RoleName='Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR

SELECT Id
FROM [DI_M_PagePermission]

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

print 'User'
--USER

DECLARE @UserId int
DECLARE @RIDRole int 
SELECT @RIDRole = Id from DI_M_Role where RoleName='User' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT Id
FROM DI_M_User

OPEN c1
 
FETCH NEXT FROM c1
INTO @UserId

WHILE @@FETCH_STATUS = 0
BEGIN

INSERT INTO dbo.UserRole(Role_RoleId,User_UserId)
VALUES(@RIDRole , @UserId)

FETCH NEXT FROM c1
INTO @UserId

END

CLOSE c1
DEALLOCATE c1
--End

GO


----Access Rights For User
DECLARE @PagePermissionId int
DECLARE @RIDRole int 
SELECT @RIDRole = Id from DI_M_Role where RoleName='User' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT pageperm.Id
FROM [DI_M_PagePermission] pageperm
inner join DI_M_Page pag on pag.Id = pageperm.PageId
where pag.uniquename in ('HOME',
'HOME_MYPROFILE',
'HOME_MYTASK','HOME_MYGROUPTASK','HOME_MYTASKDETAIL',
'HOME_MYDELEGATIONS','HOME_MYDELEGATIONS_DETAILDELEGATIONS','HOME_MYDELEGATIONS_ADDDELEGATIONS','HOME_MYDELEGATIONS_EDITDELEGATIONS',
'HOME_MYREQUESTS','HOME_MYREQUESTS_DETAIL',
'HOME_ACCESSREQUESTS','HOME_SPECIALACCESSREQUESTS',
'PASSWORDMANAGEMENT','PASSWORDMGMT_PRIVATEQUESTION','PASSWORDMANAGEMENT_PASSWORDRESET','HOME_MYTASKDETAIL')
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


print 'Endpoint admin'

--Endpoint Administrator

DECLARE @UserId int
DECLARE @RIDRole int 
SELECT @RIDRole = Id from DI_M_Role where RoleName='Endpoint Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT Id
FROM DI_M_User  where username ='00001276'

OPEN c1
 
FETCH NEXT FROM c1
INTO @UserId

WHILE @@FETCH_STATUS = 0
BEGIN

INSERT INTO dbo.UserRole(Role_RoleId,User_UserId)
VALUES(@RIDRole , @UserId)

FETCH NEXT FROM c1
INTO @UserId

END

CLOSE c1
DEALLOCATE c1
--End

GO

----Access Rights For Endpoint Administrator
DECLARE @PagePermissionId int
DECLARE @RIDRole int 
SELECT @RIDRole = Id from DI_M_Role where RoleName='Endpoint Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT pageperm.Id
FROM [DI_M_PagePermission] pageperm
inner join DI_M_Page pag on pag.Id = pageperm.PageId
where pag.uniquename in ('HOME','ADMINISTRATION','ADMIN_ENDPOINTSYSTEM','ADMIN_JOB', 
'HOME_MYPROFILE',
'HOME_MYTASK','HOME_MYGROUPTASK','HOME_MYTASKDETAIL',
'HOME_MYDELEGATIONS','HOME_MYDELEGATIONS_DETAILDELEGATIONS','HOME_MYDELEGATIONS_ADDDELEGATIONS','HOME_MYDELEGATIONS_EDITDELEGATIONS',
'HOME_MYREQUESTS','HOME_MYREQUESTS_DETAIL',
'HOME_ACCESSREQUESTS',
'ADMIN_ENPOINT_DETAILENDPOINT','ADMIN_ENPOINT_ADDENDPOINT','ADMIN_ENDPOINT_EDITENDPOINT',
'ADMIN_DISCOVERYJOB','ADMIN_DISCOVERYJOB_DETAILDISCOVERY','ADMIN_DISCOVERYJOB_ADDDISCOVERY','ADMIN_DISCOVERYJOB_EDITDISCOVERY',
'DATA','REPORT','REPORT_USER','REPORT_ENDPOINT')



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

print 'Attestation admin'

--Attestation Administrator

DECLARE @UserId int
DECLARE @RIDRole int 
SELECT @RIDRole = Id from DI_M_Role where RoleName='Attestation Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT Id
FROM DI_M_User where username ='00001277' 

OPEN c1
 
FETCH NEXT FROM c1
INTO @UserId

WHILE @@FETCH_STATUS = 0
BEGIN

INSERT INTO dbo.UserRole(Role_RoleId,User_UserId)
VALUES(@RIDRole , @UserId)

FETCH NEXT FROM c1
INTO @UserId

END

CLOSE c1
DEALLOCATE c1
--End

GO


----Access Rights For Attestation Administrator
DECLARE @PagePermissionId int
DECLARE @RIDRole int 
SELECT @RIDRole = Id from DI_M_Role where RoleName='Attestation Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT pageperm.Id
FROM [DI_M_PagePermission] pageperm
inner join DI_M_Page pag on pag.Id = pageperm.PageId
where pag.uniquename in 
('HOME','ADMINISTRATION','ADMIN_ATTESTATION','ADMIN_JOB', 
'HOME_MYPROFILE',
'HOME_MYTASK','HOME_MYGROUPTASK','HOME_MYTASKDETAIL',
'HOME_MYDELEGATIONS','HOME_MYDELEGATIONS_DETAILDELEGATIONS','HOME_MYDELEGATIONS_ADDDELEGATIONS','HOME_MYDELEGATIONS_EDITDELEGATIONS',
'HOME_MYREQUESTS','HOME_MYREQUESTS_DETAIL',
'HOME_ACCESSREQUESTS',
'ADMIN_DETAILATTESTATION','ADMIN_ADDATTESTATION','ADMIN_EDITATTESTATION',
'ADMIN_ATTESTATIONJOB','ADMIN_ATTESTATIONJOB_DETAILATTESTATION','ADMIN_ATTESTATIONJOB_ADDATTESTATION','ADMIN_ATTESTATIONJOB_EDITATTESTATION')



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

print 'Provin admin'
----Provisioning Administrator

DECLARE @UserId int
DECLARE @RIDRole int 
SELECT @RIDRole = Id from DI_M_Role where RoleName='Provisioning Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT Id
FROM DI_M_User where username ='00001278' 

OPEN c1
 
FETCH NEXT FROM c1
INTO @UserId

WHILE @@FETCH_STATUS = 0
BEGIN

INSERT INTO dbo.UserRole(Role_RoleId,User_UserId)
VALUES(@RIDRole , @UserId)

FETCH NEXT FROM c1
INTO @UserId

END

CLOSE c1
DEALLOCATE c1
--End

GO

----Access Rights For Provisioning Administrator
DECLARE @PagePermissionId int
DECLARE @RIDRole int 
SELECT @RIDRole = Id from DI_M_Role where RoleName='Provisioning Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT pageperm.Id
FROM [DI_M_PagePermission] pageperm
inner join DI_M_Page pag on pag.Id = pageperm.PageId
where pag.uniquename in 
('HOME','ADMINISTRATION','ADMIN_PROVISIONING', 
'HOME_MYPROFILE',
'HOME_MYTASK','HOME_MYGROUPTASK','HOME_MYTASKDETAIL',
'HOME_MYDELEGATIONS','HOME_MYDELEGATIONS_DETAILDELEGATIONS','HOME_MYDELEGATIONS_ADDDELEGATIONS','HOME_MYDELEGATIONS_EDITDELEGATIONS',
'HOME_MYREQUESTS','HOME_MYREQUESTS_DETAIL',
'HOME_ACCESSREQUESTS',
'ADMIN_PROVISIONING_FORM','ADMIN_PROVISIONING_FORM_DETAILFORM','ADMIN_PROVISIONING_FORM_ADDFORM','ADMIN_PROVISIONING_FORM_EDITFORM',
'ADMIN_PROVISIONING_ROLE','ADMIN_PROVISIONING_ROLE_DETAILROLE','ADMIN_PROVISIONING_ROLE_ADDROLE','ADMIN_PROVISIONING_ROLE_EDITROLE',
'ADMIN_ROLEMATRIX','ADMIN_ROLEMATRIX_DETAIL_ROLEMATRIX','ADMIN_ROLEMATRIX_ADD_ROLEMATRIX','ADMIN_ROLEMATRIX_EDIT_ROLEMATRIX',
'ADMIN_PROVISIONING_MYREQUESTS',
'ADMIN_PROVISIONING_MYTASK','ADMIN_PROVISIONING_MYTASKDETAILTYPE','ADMIN_PROVISIONING_MYTASKDETAIL',
'ADMIN_TRIGGERS','ADMIN_TRIGGERS_DETAILTRIGGERS','ADMIN_TRIGGERS_ADDTRIGGERS','ADMIN_TRIGGERS_EDITTRIGGERS',
'ADMIN_ACCESSSNAPSHOT','ADMIN_ACCESSSNAPSHOT_DETAILSNAPSHOT')

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



