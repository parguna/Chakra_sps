
Delete [DI_M_Role]
DBCC CHECKIDENT ([DI_M_Role], reseed, 1)

SET IDENTITY_INSERT [dbo].[DI_M_Role] ON
INSERT INTO [dbo].[DI_M_Role](Id,[RoleName],[RoleDescription],[CreatedBy],[CreatedDate],[LastModifiedBy],[LastModifiedDate],IsDefaultRole)     VALUES(1,'Administrator','All Administrators','System',SYSDATETIME(),'System',SYSDATETIME(),1)
INSERT INTO [dbo].[DI_M_Role](Id,[RoleName],[RoleDescription],[CreatedBy],[CreatedDate],[LastModifiedBy],[LastModifiedDate],IsDefaultRole)     VALUES(2,'ID Administrator','ID Administrator','System',SYSDATETIME(),'System',SYSDATETIME(),1)
INSERT INTO [dbo].[DI_M_Role](Id,[RoleName],[RoleDescription],[CreatedBy],[CreatedDate],[LastModifiedBy],[LastModifiedDate],IsDefaultRole)     VALUES(3,'ID Administrator Supervisor','ID Administrator Supervisor','System',SYSDATETIME(),'System',SYSDATETIME(),1)
INSERT INTO [dbo].[DI_M_Role](Id,[RoleName],[RoleDescription],[CreatedBy],[CreatedDate],[LastModifiedBy],[LastModifiedDate],IsDefaultRole)     VALUES(4,'IDAS Product  Admin','IDAS Product  Admin','System',SYSDATETIME(),'System',SYSDATETIME(),1)
INSERT INTO [dbo].[DI_M_Role](Id,[RoleName],[RoleDescription],[CreatedBy],[CreatedDate],[LastModifiedBy],[LastModifiedDate],IsDefaultRole)     VALUES(5,'IDAS Technical  Admin','IDAS Technical  Admin','System',SYSDATETIME(),'System',SYSDATETIME(),1)
INSERT INTO [dbo].[DI_M_Role](Id,[RoleName],[RoleDescription],[CreatedBy],[CreatedDate],[LastModifiedBy],[LastModifiedDate],IsDefaultRole)     VALUES(6,'IDAS Business  Admin','IDAS Business  Admin','System',SYSDATETIME(),'System',SYSDATETIME(),1)
INSERT INTO [dbo].[DI_M_Role](Id,[RoleName],[RoleDescription],[CreatedBy],[CreatedDate],[LastModifiedBy],[LastModifiedDate],IsDefaultRole)     VALUES(7,'IDAS BU User','IDAS BU User','System',SYSDATETIME(),'System',SYSDATETIME(),1)
INSERT INTO [dbo].[DI_M_Role](Id,[RoleName],[RoleDescription],[CreatedBy],[CreatedDate],[LastModifiedBy],[LastModifiedDate],IsDefaultRole)     VALUES(8,'IDAS Audit User','IDAS Audit User','System',SYSDATETIME(),'System',SYSDATETIME(),1)
SET IDENTITY_INSERT [dbo].[DI_M_Role] OFF

GO
-------------- Administrator 
print 'IACM admin'
DECLARE @UserId bigint
DECLARE @RIDRole bigint 
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
DECLARE @PagePermissionId bigint
DECLARE @RIDRole bigint 
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
VALUES(@RIDRole ,  @PagePermissionId,'System' , SYSDATETIME(),'System' , SYSDATETIME())

FETCH NEXT FROM c1
INTO @PagePermissionId

END

CLOSE c1
DEALLOCATE c1

GO

--print 'User'
----USER

--DECLARE @UserId bigint
--DECLARE @RIDRole bigint 
--SELECT @RIDRole = Id from DI_M_Role where RoleName='User' 
--DECLARE c1 CURSOR READ_ONLY
--FOR
--SELECT Id
--FROM DI_M_User

--OPEN c1
 
--FETCH NEXT FROM c1
--INTO @UserId

--WHILE @@FETCH_STATUS = 0
--BEGIN

--INSERT INTO dbo.UserRole(Role_RoleId,User_UserId)
--VALUES(@RIDRole , @UserId)

--FETCH NEXT FROM c1
--INTO @UserId

--END

--CLOSE c1
--DEALLOCATE c1
----End

--GO


------Access Rights For User
--DECLARE @PagePermissionId bigint
--DECLARE @RIDRole bigint 
--SELECT @RIDRole = Id from DI_M_Role where RoleName='User' 
--DECLARE c1 CURSOR READ_ONLY
--FOR
--SELECT pageperm.Id
--FROM [DI_M_PagePermission] pageperm
--inner join DI_M_Page pag on pag.Id = pageperm.PageId
--where pag.uniquename in ('HOME',
--'HOME_MYPROFILE',
--'HOME_MYTASK','HOME_MYGROUPTASK','HOME_MYTASKDETAIL',
--'HOME_MYNOTIFICATIONS','HOME_MYNOTIFICATIONS_DETAIL',
--'HOME_MYDELEGATIONS','HOME_MYDELEGATIONS_DETAILDELEGATIONS','HOME_MYDELEGATIONS_ADDDELEGATIONS','HOME_MYDELEGATIONS_EDITDELEGATIONS',
--'HOME_MYREQUESTS','HOME_MYREQUESTS_DETAIL',
--'HOME_ACCESSREQUESTS',
--'PASSWORDMANAGEMENT','ACCOUNTRECOVERY','PASSWORDMGMT_PRIVATEQUESTION','PASSWORDMANAGEMENT_PASSWORDRESET','HOME_MYTASKDETAIL','PREFERENCES_USERPREFERENCE',
--'CATALOG_MANAGEMENT',
--'CATALOG_MANAGEMENT_ADD',
--'CATALOG_MANAGEMENT_EDIT',
--'CATALOG_MANAGEMENT_DETAIL','ADMIN_Report','ADMIN_ReportResult',
--'ADMIN_Report_ADD',
--'ADMIN_Report_EDIT',
--'ADMIN_Report_DETAIL')
--OPEN c1
 
--FETCH NEXT FROM c1
--INTO @PagePermissionId

--WHILE @@FETCH_STATUS = 0
--BEGIN

--INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDRole ,  @PagePermissionId,'System' , SYSDATETIME(),'System' , SYSDATETIME())

--FETCH NEXT FROM c1
--INTO @PagePermissionId

--END

--CLOSE c1
--DEALLOCATE c1
--GO

Declare @UrlId bigint
DECLARE @RIDRole bigint 
DECLARE @PagePermissionId bigint
DECLARE @PageId bigint
SELECT @RIDRole = Id from DI_M_Role where RoleName='Administrator'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Attestation/NReview.aspx'

insert into DI_M_AccessRights
SELECT @RIDRole, ID, 'system', GETDATE(), 'system', GETDATE() FROM [dbo].[DI_M_PagePermission] WHERE UrlID = @UrlId

SELECT @PageId =Id FROM [DI_M_Page] where UniqueName like '%HOME_MYTASKDETAIL%'
insert into DI_M_AccessRights
SELECT @RIDRole, ID, 'system', GETDATE(), 'system', GETDATE() FROM [dbo].[DI_M_PagePermission] WHERE PageId = @PageId

SELECT @PageId = Id FROM[DI_M_Page] where UniqueName like '%ADMIN_PROVISIONING_MYTASKDETAIL%'  
insert into DI_M_AccessRights  
SELECT @RIDRole, ID, 'system', GETDATE(), 'system', GETDATE() FROM[dbo].[DI_M_PagePermission] WHERE PageId = @PageId 



SELECT @RIDRole = Id from DI_M_Role where RoleName='ID Administrator'
insert into DI_M_AccessRights
SELECT @RIDRole, ID, 'system', GETDATE(), 'system', GETDATE() FROM [dbo].[DI_M_PagePermission] WHERE UrlID = @UrlId

SELECT @PageId =Id FROM [DI_M_Page] where UniqueName like '%HOME_MYTASKDETAIL%'
insert into DI_M_AccessRights
SELECT @RIDRole, ID, 'system', GETDATE(), 'system', GETDATE() FROM [dbo].[DI_M_PagePermission] WHERE PageId = @PageId

SELECT @PageId = Id FROM[DI_M_Page] where UniqueName like '%ADMIN_PROVISIONING_MYTASKDETAIL%'  
insert into DI_M_AccessRights  
SELECT @RIDRole, ID, 'system', GETDATE(), 'system', GETDATE() FROM[dbo].[DI_M_PagePermission] WHERE PageId = @PageId 


SELECT @RIDRole = Id from DI_M_Role where RoleName='ID Administrator Supervisor'
insert into DI_M_AccessRights
SELECT @RIDRole, ID, 'system', GETDATE(), 'system', GETDATE() FROM [dbo].[DI_M_PagePermission] WHERE UrlID = @UrlId

SELECT @PageId =Id FROM [DI_M_Page] where UniqueName like '%HOME_MYTASKDETAIL%'
insert into DI_M_AccessRights
SELECT @RIDRole, ID, 'system', GETDATE(), 'system', GETDATE() FROM [dbo].[DI_M_PagePermission] WHERE PageId = @PageId

SELECT @PageId = Id FROM[DI_M_Page] where UniqueName like '%ADMIN_PROVISIONING_MYTASKDETAIL%'  
insert into DI_M_AccessRights  
SELECT @RIDRole, ID, 'system', GETDATE(), 'system', GETDATE() FROM[dbo].[DI_M_PagePermission] WHERE PageId = @PageId 


SELECT @RIDRole = Id from DI_M_Role where RoleName='IDAS Product  Admin'
insert into DI_M_AccessRights
SELECT @RIDRole, ID, 'system', GETDATE(), 'system', GETDATE() FROM [dbo].[DI_M_PagePermission] WHERE UrlID = @UrlId

SELECT @PageId =Id FROM [DI_M_Page] where UniqueName like '%HOME_MYTASKDETAIL%'
insert into DI_M_AccessRights
SELECT @RIDRole, ID, 'system', GETDATE(), 'system', GETDATE() FROM [dbo].[DI_M_PagePermission] WHERE PageId = @PageId

SELECT @PageId = Id FROM[DI_M_Page] where UniqueName like '%ADMIN_PROVISIONING_MYTASKDETAIL%'  
insert into DI_M_AccessRights  
SELECT @RIDRole, ID, 'system', GETDATE(), 'system', GETDATE() FROM[dbo].[DI_M_PagePermission] WHERE PageId = @PageId 



SELECT @RIDRole = Id from DI_M_Role where RoleName='IDAS Technical  Admin'
insert into DI_M_AccessRights
SELECT @RIDRole, ID, 'system', GETDATE(), 'system', GETDATE() FROM [dbo].[DI_M_PagePermission] WHERE UrlID = @UrlId

SELECT @PageId =Id FROM [DI_M_Page] where UniqueName like '%HOME_MYTASKDETAIL%'
insert into DI_M_AccessRights
SELECT @RIDRole, ID, 'system', GETDATE(), 'system', GETDATE() FROM [dbo].[DI_M_PagePermission] WHERE PageId = @PageId

SELECT @PageId = Id FROM[DI_M_Page] where UniqueName like '%ADMIN_PROVISIONING_MYTASKDETAIL%'  
insert into DI_M_AccessRights  
SELECT @RIDRole, ID, 'system', GETDATE(), 'system', GETDATE() FROM[dbo].[DI_M_PagePermission] WHERE PageId = @PageId 


SELECT @RIDRole = Id from DI_M_Role where RoleName='IDAS Business  Admin'
insert into DI_M_AccessRights
SELECT @RIDRole, ID, 'system', GETDATE(), 'system', GETDATE() FROM [dbo].[DI_M_PagePermission] WHERE UrlID = @UrlId

SELECT @PageId =Id FROM [DI_M_Page] where UniqueName like '%HOME_MYTASKDETAIL%'
insert into DI_M_AccessRights
SELECT @RIDRole, ID, 'system', GETDATE(), 'system', GETDATE() FROM [dbo].[DI_M_PagePermission] WHERE PageId = @PageId

SELECT @PageId = Id FROM[DI_M_Page] where UniqueName like '%ADMIN_PROVISIONING_MYTASKDETAIL%'  
insert into DI_M_AccessRights  
SELECT @RIDRole, ID, 'system', GETDATE(), 'system', GETDATE() FROM[dbo].[DI_M_PagePermission] WHERE PageId = @PageId 


SELECT @RIDRole = Id from DI_M_Role where RoleName='IDAS BU User'
insert into DI_M_AccessRights
SELECT @RIDRole, ID, 'system', GETDATE(), 'system', GETDATE() FROM [dbo].[DI_M_PagePermission] WHERE UrlID = @UrlId

SELECT @PageId =Id FROM [DI_M_Page] where UniqueName like '%HOME_MYTASKDETAIL%'
insert into DI_M_AccessRights
SELECT @RIDRole, ID, 'system', GETDATE(), 'system', GETDATE() FROM [dbo].[DI_M_PagePermission] WHERE PageId = @PageId

SELECT @PageId = Id FROM[DI_M_Page] where UniqueName like '%ADMIN_PROVISIONING_MYTASKDETAIL%'  
insert into DI_M_AccessRights  
SELECT @RIDRole, ID, 'system', GETDATE(), 'system', GETDATE() FROM[dbo].[DI_M_PagePermission] WHERE PageId = @PageId 


SELECT @RIDRole = Id from DI_M_Role where RoleName='IDAS Audit User'
insert into DI_M_AccessRights
SELECT @RIDRole, ID, 'system', GETDATE(), 'system', GETDATE() FROM [dbo].[DI_M_PagePermission] WHERE UrlID = @UrlId

SELECT @PageId =Id FROM [DI_M_Page] where UniqueName like '%HOME_MYTASKDETAIL%'
insert into DI_M_AccessRights
SELECT @RIDRole, ID, 'system', GETDATE(), 'system', GETDATE() FROM [dbo].[DI_M_PagePermission] WHERE PageId = @PageId

SELECT @PageId = Id FROM[DI_M_Page] where UniqueName like '%ADMIN_PROVISIONING_MYTASKDETAIL%'  
insert into DI_M_AccessRights  
SELECT @RIDRole, ID, 'system', GETDATE(), 'system', GETDATE() FROM[dbo].[DI_M_PagePermission] WHERE PageId = @PageId 




GO


--print 'Endpoint admin'

----Endpoint Administrator

--DECLARE @UserId bigint
--DECLARE @RIDRole bigint 
--SELECT @RIDRole = Id from DI_M_Role where RoleName='Endpoint Administrator' 
--DECLARE c1 CURSOR READ_ONLY
--FOR
--SELECT Id
--FROM DI_M_User  where username ='iacmadmin'

--OPEN c1
 
--FETCH NEXT FROM c1
--INTO @UserId

--WHILE @@FETCH_STATUS = 0
--BEGIN

--INSERT INTO dbo.UserRole(Role_RoleId,User_UserId)
--VALUES(@RIDRole , @UserId)

--FETCH NEXT FROM c1
--INTO @UserId

--END

--CLOSE c1
--DEALLOCATE c1
----End

--GO

------Access Rights For Endpoint Administrator
--DECLARE @PagePermissionId bigint
--DECLARE @RIDRole bigint 
--SELECT @RIDRole = Id from DI_M_Role where RoleName='Endpoint Administrator' 
--DECLARE c1 CURSOR READ_ONLY
--FOR
--SELECT pageperm.Id
--FROM [DI_M_PagePermission] pageperm
--inner join DI_M_Page pag on pag.Id = pageperm.PageId
--where pag.uniquename in ('HOME','ADMINISTRATION','ADMIN_ENDPOINTSYSTEM','ADMIN_JOB', 
--'HOME_MYPROFILE',
--'HOME_MYTASK','HOME_MYGROUPTASK','HOME_MYTASKDETAIL',
--'HOME_MYDELEGATIONS','HOME_MYDELEGATIONS_DETAILDELEGATIONS','HOME_MYDELEGATIONS_ADDDELEGATIONS','HOME_MYDELEGATIONS_EDITDELEGATIONS',
--'HOME_MYREQUESTS','HOME_MYREQUESTS_DETAIL',
--'HOME_ACCESSREQUESTS',
--'ADMIN_ENPOINT_DETAILENDPOINT','ADMIN_ENPOINT_ADDENDPOINT','ADMIN_ENDPOINT_EDITENDPOINT',
--'ADMIN_DISCOVERYJOB','ADMIN_DISCOVERYJOB_DETAILDISCOVERY','ADMIN_DISCOVERYJOB_ADDDISCOVERY','ADMIN_DISCOVERYJOB_EDITDISCOVERY',
--'DATA','REPORT','REPORT_USER','REPORT_ENDPOINT','PREFERENCES_USERPREFERENCE',
--'REPORT_PROVISIONING','REPORT_Prov','REPORT_Access_Matrix','REPORT_Organisation','REPORT_Attestation_Detail', 
--'REPORT_Attestation_Campaign_Status', 'REPORT_Attestation_Campaign_Report', 'REPORT_Attestation_Campaign_Task',
--'REPORT_LoginStatus')



--OPEN c1
 
--FETCH NEXT FROM c1
--INTO @PagePermissionId

--WHILE @@FETCH_STATUS = 0
--BEGIN

--INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDRole ,  @PagePermissionId,'System' , SYSDATETIME() ,'System' , SYSDATETIME())

--FETCH NEXT FROM c1
--INTO @PagePermissionId

--END

--CLOSE c1
--DEALLOCATE c1

--GO

--print 'Attestation admin'

----Attestation Administrator

--DECLARE @UserId bigint
--DECLARE @RIDRole bigint 
--SELECT @RIDRole = Id from DI_M_Role where RoleName='Attestation Administrator' 
--DECLARE c1 CURSOR READ_ONLY
--FOR
--SELECT Id
--FROM DI_M_User where username ='iacmadmin' 

--OPEN c1
 
--FETCH NEXT FROM c1
--INTO @UserId

--WHILE @@FETCH_STATUS = 0
--BEGIN

--INSERT INTO dbo.UserRole(Role_RoleId,User_UserId)
--VALUES(@RIDRole , @UserId)

--FETCH NEXT FROM c1
--INTO @UserId

--END

--CLOSE c1
--DEALLOCATE c1
----End

--GO


------Access Rights For Attestation Administrator
--DECLARE @PagePermissionId bigint
--DECLARE @RIDRole bigint 
--SELECT @RIDRole = Id from DI_M_Role where RoleName='Attestation Administrator' 
--DECLARE c1 CURSOR READ_ONLY
--FOR
--SELECT pageperm.Id
--FROM [DI_M_PagePermission] pageperm
--inner join DI_M_Page pag on pag.Id = pageperm.PageId
--where pag.uniquename in 
--('HOME','ADMINISTRATION','ADMIN_ATTESTATION','ADMIN_JOB', 
--'HOME_MYPROFILE',
--'HOME_MYTASK','HOME_MYGROUPTASK','HOME_MYTASKDETAIL',
--'HOME_MYDELEGATIONS','HOME_MYDELEGATIONS_DETAILDELEGATIONS','HOME_MYDELEGATIONS_ADDDELEGATIONS','HOME_MYDELEGATIONS_EDITDELEGATIONS',
--'HOME_MYREQUESTS','HOME_MYREQUESTS_DETAIL',
--'HOME_ACCESSREQUESTS',
--'ADMIN_DETAILATTESTATION','ADMIN_ADDATTESTATION','ADMIN_EDITATTESTATION',
--'ADMIN_ATTESTATIONJOB','ADMIN_ATTESTATIONJOB_DETAILATTESTATION','ADMIN_ATTESTATIONJOB_ADDATTESTATION','ADMIN_ATTESTATIONJOB_EDITATTESTATION','PREFERENCES_USERPREFERENCE',
--'REPORT_PROVISIONING','REPORT_Prov','REPORT_Access_Matrix','REPORT_Organisation','REPORT_Attestation_Detail', 
--'REPORT_Attestation_Campaign_Status', 'REPORT_Attestation_Campaign_Report', 'REPORT_Attestation_Campaign_Task')



--OPEN c1
 
--FETCH NEXT FROM c1
--INTO @PagePermissionId

--WHILE @@FETCH_STATUS = 0
--BEGIN

--INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDRole ,  @PagePermissionId,'System' , SYSDATETIME() ,'System' , SYSDATETIME())

--FETCH NEXT FROM c1
--INTO @PagePermissionId

--END

--CLOSE c1
--DEALLOCATE c1

--GO

--print 'Provin admin'
------Provisioning Administrator

--DECLARE @UserId bigint
--DECLARE @RIDRole bigint 
--SELECT @RIDRole = Id from DI_M_Role where RoleName='Provisioning Administrator' 
--DECLARE c1 CURSOR READ_ONLY
--FOR
--SELECT Id
--FROM DI_M_User where username ='iacmadmin' 

--OPEN c1
 
--FETCH NEXT FROM c1
--INTO @UserId

--WHILE @@FETCH_STATUS = 0
--BEGIN

--INSERT INTO dbo.UserRole(Role_RoleId,User_UserId)
--VALUES(@RIDRole , @UserId)

--FETCH NEXT FROM c1
--INTO @UserId

--END

--CLOSE c1
--DEALLOCATE c1
----End

--GO

------Access Rights For Provisioning Administrator
--DECLARE @PagePermissionId bigint
--DECLARE @RIDRole bigint 
--SELECT @RIDRole = Id from DI_M_Role where RoleName='Provisioning Administrator' 
--DECLARE c1 CURSOR READ_ONLY
--FOR
--SELECT pageperm.Id
--FROM [DI_M_PagePermission] pageperm
--inner join DI_M_Page pag on pag.Id = pageperm.PageId
--where pag.uniquename in 
--('HOME','ADMINISTRATION','ADMIN_PROVISIONING', 
--'HOME_MYPROFILE',
--'HOME_MYTASK','HOME_MYGROUPTASK','HOME_MYTASKDETAIL',
--'HOME_MYDELEGATIONS','HOME_MYDELEGATIONS_DETAILDELEGATIONS','HOME_MYDELEGATIONS_ADDDELEGATIONS','HOME_MYDELEGATIONS_EDITDELEGATIONS',
--'HOME_MYREQUESTS','HOME_MYREQUESTS_DETAIL',
--'HOME_ACCESSREQUESTS',
--'ADMIN_PROVISIONING_FORM','ADMIN_PROVISIONING_FORM_DETAILFORM','ADMIN_PROVISIONING_FORM_ADDFORM','ADMIN_PROVISIONING_FORM_EDITFORM',
--'ADMIN_ROLEMATRIX','ADMIN_ROLEMATRIX_DETAIL_ROLEMATRIX','ADMIN_ROLEMATRIX_ADD_ROLEMATRIX','ADMIN_ROLEMATRIX_EDIT_ROLEMATRIX',
--'ADMIN_PROVISIONING_MYREQUESTS',
--'ADMIN_PROVISIONING_MYTASK','ADMIN_PROVISIONING_MYTASKDETAILTYPE','ADMIN_PROVISIONING_MYTASKDETAIL',
--'ADMIN_TRIGGERS','ADMIN_TRIGGERS_DETAILTRIGGERS','ADMIN_TRIGGERS_ADDTRIGGERS','ADMIN_TRIGGERS_EDITTRIGGERS',
--'ADMIN_ACCESSSNAPSHOT','ADMIN_ACCESSSNAPSHOT_DETAILSNAPSHOT','PREFERENCES_USERPREFERENCE',
--'REPORT_PROVISIONING','REPORT_Prov','REPORT_Access_Matrix','REPORT_Organisation','REPORT_Attestation_Detail', 
--'REPORT_Attestation_Campaign_Status', 'REPORT_Attestation_Campaign_Report', 'REPORT_Attestation_Campaign_Task',
--'REPORT_LoginStatus')

--OPEN c1
 
--FETCH NEXT FROM c1
--INTO @PagePermissionId

--WHILE @@FETCH_STATUS = 0
--BEGIN

--INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDRole ,  @PagePermissionId,'System' , SYSDATETIME() ,'System' , SYSDATETIME())

--FETCH NEXT FROM c1
--INTO @PagePermissionId

--END

--CLOSE c1
--DEALLOCATE c1

--GO

--INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(1 ,  682,'System' , SYSDATETIME() ,'System' , SYSDATETIME())


--INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(1 ,  712,'System' , SYSDATETIME() ,'System' , SYSDATETIME())


Declare @PageId bigint
Declare @TabParentMenuId bigint

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ROLEACCESS'
SELECT @TabParentMenuId =Id FROM [DI_M_PagePermission] WHERE [PageId] = @PageId and [PermissionId] = 1

INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(1 , @TabParentMenuId ,'System' , GETDATE() ,'System' , GETDATE())
SELECT @TabParentMenuId =Id FROM [DI_M_PagePermission] WHERE [PageId] = @PageId and [PermissionId] = 12
INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(1 , @TabParentMenuId ,'System' , GETDATE() ,'System' , GETDATE())
SELECT @TabParentMenuId =Id FROM [DI_M_PagePermission] WHERE [PageId] = @PageId and [PermissionId] = 24
INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(1 , @TabParentMenuId ,'System' , GETDATE() ,'System' , GETDATE())

