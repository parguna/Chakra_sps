--Dashboard Table Insert Script
print 'start'

delete from DI_M_UserDashboardPreference
delete from DI_M_DefaultRoleDashboard
delete from DI_M_DashboardAccessRights
delete from DI_M_Dashboard

Delete DI_M_DefaultRoleDashboard
DBCC CHECKIDENT (DI_M_DefaultRoleDashboard, reseed, 1)

Delete [DI_M_DashboardAccessRights]
DBCC CHECKIDENT ([DI_M_DashboardAccessRights], reseed, 1)

Delete DI_M_Dashboard
DBCC CHECKIDENT (DI_M_Dashboard, reseed, 1)

-------------------Dashboards---------------------------
--------------------------------------------------------

INSERT INTO DI_M_Dashboard(DashboardUniqueName, DashboardCategory,DashboardName ,TargetUrl ,NavigateUrl,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES('ACCESS_REQUEST_BY_EVENTS','Provisioning','Access Request By Events' , '~/Dashboard/IACMPieChart.ascx' ,'~/Home/Home.aspx','1','System' , GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_Dashboard(DashboardUniqueName, DashboardCategory,DashboardName ,TargetUrl ,NavigateUrl,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES('ACCESS_REQUEST_BY_EVENTS_USER','Provisioning','My Access Request By Events' , '~/Dashboard/IACMPieChartUser.ascx' ,'~/Home/Home.aspx','1','System' , GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_Dashboard(DashboardUniqueName, DashboardCategory,DashboardName ,TargetUrl,NavigateUrl ,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES('PENDING_TASKS','Task','Pending Tasks' , '~/Dashboard/TasksUC.ascx' ,'~/Task/List.aspx','2','System' , GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_Dashboard(DashboardUniqueName, DashboardCategory,DashboardName ,TargetUrl,NavigateUrl ,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES('ACCESS_REQUEST_BY_MONTH','Provisioning','Access Request By Month' , '~/Dashboard/IACMColumnChart.ascx' ,'~/Home/Home.aspx','1','System' , GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_Dashboard(DashboardUniqueName, DashboardCategory,DashboardName ,TargetUrl,NavigateUrl ,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES('MY_REQUESTS','Provisioning','My Request' , '~/Dashboard/MyRequest.ascx' ,'~/Request/List.aspx','2','System' ,GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_Dashboard(DashboardUniqueName, DashboardCategory,DashboardName ,TargetUrl,NavigateUrl ,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES('ACCESS_REQUESTS_COLUMN_CHART_BY_USER','Provisioning','My Access Request By Month' , '~/Dashboard/AccessRequestColumnChartByUser.ascx' ,'~/Home/Home.aspx','1','System' ,GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_Dashboard(DashboardUniqueName, DashboardCategory,DashboardName ,TargetUrl,NavigateUrl ,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES('ACCESS_REQUEST_MONTHLY_PROGRESS_STATUS','Provisioning','My Access Request Progress Status' , '~/Dashboard/AccessRequestProgressstatusbyMonth.ascx' ,'~/Home/Home.aspx','1','System' ,GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_Dashboard(DashboardUniqueName, DashboardCategory,DashboardName ,TargetUrl,NavigateUrl ,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES('ACCESS_REQUEST_MONTHLY_PROGRESS_STATUS_ADMIN','Provisioning','Access Request Progress Status' , '~/Dashboard/AccessRequestProgressstatusbyMonthByAdmin.ascx' ,'~/Home/Home.aspx','1','System' ,GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_Dashboard(DashboardUniqueName, DashboardCategory,DashboardName ,TargetUrl,NavigateUrl ,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES('AD_HOC_ACCESS_REQUESTS_BY_MONTH','Provisioning','My AdhocAccess Request' , '~/Dashboard/AdhocAccesRequestbyMonth.ascx' ,'~/Home/Home.aspx','1','System' ,GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_Dashboard(DashboardUniqueName, DashboardCategory,DashboardName ,TargetUrl,NavigateUrl ,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES('AD_HOC_ACCESS_REQUESTS_BY_MONTH_ADMIN','Provisioning','AdhocAccess Request' , '~/Dashboard/AdhocAccesRequestbyMonthByAdmin.ascx' ,'~/Home/Home.aspx','1','System' ,GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_Dashboard(DashboardUniqueName, DashboardCategory,DashboardName ,TargetUrl,NavigateUrl ,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES('ALERTCENTRE_TASKS','Provisioning','Alert Centre' , '~/Dashboard/AlertCentre.ascx' ,'~/AlertCenter/List.aspx','2','System' ,GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_Dashboard(DashboardUniqueName, DashboardCategory,DashboardName ,TargetUrl,NavigateUrl ,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES('JAC_TASKS','Provisioning','Job Allocation Center' , '~/Dashboard/JobAllocationCenter.ascx' ,'~/JobAllocationCenter/List.aspx','2','System' ,GETDATE() ,'System' , GETDATE())
--INSERT INTO DI_M_Dashboard(DashboardCategory,DashboardName ,TargetUrl,NavigateUrl ,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES('Attestation','Task Summary' , '~/Dashboard/Attestation/TaskSummaryUC.ascx' ,'~/Home/Home.aspx','1','System' ,GETDATE() ,'System' , GETDATE())
--INSERT INTO DI_M_Dashboard(DashboardCategory,DashboardName ,TargetUrl,NavigateUrl ,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES('Attestation','Task Completeness' , '~/Dashboard/Attestation/TaskCompletenessUC.ascx' ,'~/Home/Home.aspx','2','System' ,GETDATE() ,'System' , GETDATE())
--INSERT INTO DI_M_Dashboard(DashboardCategory,DashboardName ,TargetUrl,NavigateUrl ,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES('Attestation','Task Assignee' , '~/Dashboard/Attestation/TaskAssigneeUC.ascx' ,'~/Home/Home.aspx','2','System' ,GETDATE() ,'System' , GETDATE())
--INSERT INTO DI_M_Dashboard(DashboardCategory,DashboardName ,TargetUrl,NavigateUrl ,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES('Attestation','Attestation Remediate Status' , '~/Dashboard/Attestation/AttestationRemediateStatusUC.ascx' ,'~/Home/Home.aspx','2','System' ,GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_Dashboard(DashboardUniqueName, DashboardCategory,DashboardName ,TargetUrl,NavigateUrl ,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES('ATTESTATION_TASK_GAUGE','Attestation','Attestation Task(Gauge)' ,'~/Dashboard/Attestation/AttestationcompletedTaskUC.ascx' ,'~/Home/Home.aspx','1','System' , GETDATE() ,'System' , GETDATE())

insert into DI_M_Dashboard(DashboardUniqueName ,DashboardCategory,DashboardName,TargetUrl,NavigateUrl,DashBoardType,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate)
values('GROUP_TASK','Task','Group Task','~/Dashboard/GroupTaskUC.ascx','~/Task/GroupTasks.aspx','2','System',GETDATE(),'System',GETDATE())

insert into DI_M_Dashboard (DashboardUniqueName,DashboardCategory,DashboardName,TargetUrl,NavigateUrl,DashBoardType,CreatedDate,LastModifiedDate,CreatedBy,LastModifiedBy)
values('ATTESTATION_TASK_PIE','Attestation','Attestation Task(Pie)','~/Dashboard/Attestation/AttestationOverallStatus.ascx','~/Home/Home.aspx','1',GETDATE(),GETDATE(),'System','System')

INSERT INTO DI_M_Dashboard(DashboardUniqueName ,DashboardCategory,DashboardName,TargetUrl,NavigateUrl,DashBoardType,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate)
VALUES('MY_ATTENTION','Provisioning','My Attention','~/Dashboard/MyAttention.ascx','~/Home/Home.aspx','2','System',GETDATE(),'System',GETDATE())

INSERT INTO DI_M_Dashboard(DashboardUniqueName ,DashboardCategory,DashboardName,TargetUrl,NavigateUrl,DashBoardType,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate)
VALUES('ADMIN_ATTENTION','Provisioning','Admin Attention','~/Dashboard/AdminAttention.ascx','~/Home/Home.aspx','2','System',GETDATE(),'System',GETDATE())

GO
-----------Administrator---------------------------------
-----------Access Rights
print 'Middle'

DECLARE @DashboardId int
DECLARE @RIDARole int 
SELECT @RIDARole = Id from DI_M_Role where RoleName='Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT Id
FROM [DI_M_Dashboard]

OPEN c1
 
FETCH NEXT FROM c1
INTO @DashboardId

WHILE @@FETCH_STATUS = 0
BEGIN

INSERT INTO [dbo].[DI_M_DashboardAccessRights](RoleId , DashboardId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDARole , @DashboardId ,'System' , GETDATE() ,'System' , GETDATE())

FETCH NEXT FROM c1
INTO @DashboardId

END

CLOSE c1
DEALLOCATE c1

---------Default DashBoard
SELECT @DashBoardId = [Id] FROM [DI_M_Dashboard] WHERE [DashboardUniqueName] = 'ACCESS_REQUEST_MONTHLY_PROGRESS_STATUS_ADMIN'
INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDARole , @DashBoardId ,'1','System' , GETDATE() ,'System' , GETDATE())

SELECT @DashBoardId = [Id] FROM [DI_M_Dashboard] WHERE [DashboardUniqueName] = 'PENDING_TASKS'
INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDARole , @DashBoardId ,'2','System' , GETDATE() ,'System' , GETDATE())

SELECT @DashBoardId = [Id] FROM [DI_M_Dashboard] WHERE [DashboardUniqueName] = 'ACCESS_REQUEST_BY_MONTH'
INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDARole , @DashBoardId ,'3','System' , GETDATE() ,'System' , GETDATE())
GO

--INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDARole , '7' ,'1','System' , GETDATE() ,'System' , GETDATE())
--INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDARole , '8' ,'2','System' , GETDATE() ,'System' , GETDATE())
--INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDARole , '9' ,'3','System' , GETDATE() ,'System' , GETDATE())
--GO


-----------User---------------------------------
-----------Access Rights
--DECLARE @DashboardId int
--DECLARE @RIDURole int 
--SELECT @RIDURole = Id from DI_M_Role where RoleName='User' 
--DECLARE c1 CURSOR READ_ONLY
--FOR
--SELECT Id
--FROM [DI_M_Dashboard] where id <> 1

--OPEN c1
 
--FETCH NEXT FROM c1
--INTO @DashboardId

--WHILE @@FETCH_STATUS = 0
--BEGIN

--INSERT INTO [dbo].[DI_M_DashboardAccessRights](RoleId , DashboardId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDURole , @DashboardId ,'System' , GETDATE() ,'System' , GETDATE())

--FETCH NEXT FROM c1
--INTO @DashboardId

--END

--CLOSE c1
--DEALLOCATE c1

---------Default Dashboard 
--SELECT @DashBoardId = [Id] FROM [DI_M_Dashboard] WHERE [DashboardUniqueName] = 'AD_HOC_ACCESS_REQUESTS_BY_MONTH'
--INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDURole , @DashBoardId ,'1','System' , GETDATE() ,'System' , GETDATE())

--SELECT @DashBoardId = [Id] FROM [DI_M_Dashboard] WHERE [DashboardUniqueName] = 'MY_REQUESTS'
--INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDURole , @DashBoardId ,'2','System' , GETDATE() ,'System' , GETDATE())

--SELECT @DashBoardId = [Id] FROM [DI_M_Dashboard] WHERE [DashboardUniqueName] = 'ACCESS_REQUEST_MONTHLY_PROGRESS_STATUS'
--INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDURole , @DashBoardId ,'3','System' , GETDATE() ,'System' , GETDATE())
--GO

----INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
----VALUES(@RIDURole , '3' ,'1','System' , GETDATE() ,'System' , GETDATE())
----INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
----VALUES(@RIDURole , '2' ,'2','System' , GETDATE() ,'System' , GETDATE())
----INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
----VALUES(@RIDURole , '5' ,'3','System' , GETDATE() ,'System' , GETDATE())
----GO



-------------Attestion Admin ---------------------------------
-------------Access Rights

--DECLARE @DashboardId int
--DECLARE @RIDAtAdRole int 
--SELECT @RIDAtAdRole = Id from DI_M_Role where RoleName='Attestation Administrator' 
--DECLARE c1 CURSOR READ_ONLY
--FOR
--SELECT Id
--FROM [DI_M_Dashboard]

--OPEN c1
 
--FETCH NEXT FROM c1
--INTO @DashboardId

--WHILE @@FETCH_STATUS = 0
--BEGIN

--INSERT INTO [dbo].[DI_M_DashboardAccessRights](RoleId , DashboardId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDAtAdRole , @DashboardId ,'System' , GETDATE() ,'System' , GETDATE())

--FETCH NEXT FROM c1
--INTO @DashboardId

--END

--CLOSE c1
--DEALLOCATE c1

-----------Default DashBoard
--SELECT @DashBoardId = [Id] FROM [DI_M_Dashboard] WHERE [DashboardUniqueName] = 'ACCESS_REQUEST_MONTHLY_PROGRESS_STATUS_ADMIN'
--INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDAtAdRole , @DashBoardId ,'1','System' , GETDATE() ,'System' , GETDATE())

--SELECT @DashBoardId = [Id] FROM [DI_M_Dashboard] WHERE [DashboardUniqueName] = 'PENDING_TASKS'
--INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDAtAdRole , @DashBoardId ,'2','System' , GETDATE() ,'System' , GETDATE())

--SELECT @DashBoardId = [Id] FROM [DI_M_Dashboard] WHERE [DashboardUniqueName] = 'ACCESS_REQUEST_BY_MONTH'
--INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDAtAdRole , @DashBoardId ,'3','System' , GETDATE() ,'System' , GETDATE())

--GO


-------------EndPoint Admin---------------------------------
-------------Access Rights

--DECLARE @DashboardId int
--DECLARE @RIDEpAdRole int 
--SELECT @RIDEpAdRole = Id from DI_M_Role where RoleName='Endpoint Administrator' 
--DECLARE c1 CURSOR READ_ONLY
--FOR
--SELECT Id
--FROM [DI_M_Dashboard]

--OPEN c1
 
--FETCH NEXT FROM c1
--INTO @DashboardId

--WHILE @@FETCH_STATUS = 0
--BEGIN

--INSERT INTO [dbo].[DI_M_DashboardAccessRights](RoleId , DashboardId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDEpAdRole , @DashboardId ,'System' , GETDATE() ,'System' , GETDATE())

--FETCH NEXT FROM c1
--INTO @DashboardId

--END

--CLOSE c1
--DEALLOCATE c1

-----------Default DashBoard

--SELECT @DashBoardId = [Id] FROM [DI_M_Dashboard] WHERE [DashboardUniqueName] = 'ACCESS_REQUEST_MONTHLY_PROGRESS_STATUS_ADMIN'
--INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDEpAdRole , @DashBoardId ,'1','System' , GETDATE() ,'System' , GETDATE())

--SELECT @DashBoardId = [Id] FROM [DI_M_Dashboard] WHERE [DashboardUniqueName] = 'PENDING_TASKS'
--INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDEpAdRole , @DashBoardId ,'2','System' , GETDATE() ,'System' , GETDATE())

--SELECT @DashBoardId = [Id] FROM [DI_M_Dashboard] WHERE [DashboardUniqueName] = 'ACCESS_REQUEST_BY_MONTH'
--INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDEpAdRole , @DashBoardId ,'3','System' , GETDATE() ,'System' , GETDATE())

--GO


-------------Provisioning Admin---------------------------------
-------------Access Rights

--DECLARE @DashboardId int
--DECLARE @RIDPdAdRole int 
--SELECT @RIDPdAdRole = Id from DI_M_Role where RoleName='Provisioning Administrator'  
--DECLARE c1 CURSOR READ_ONLY
--FOR
--SELECT Id
--FROM [DI_M_Dashboard]

--OPEN c1
 
--FETCH NEXT FROM c1
--INTO @DashboardId

--WHILE @@FETCH_STATUS = 0
--BEGIN

--INSERT INTO [dbo].[DI_M_DashboardAccessRights](RoleId , DashboardId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDPdAdRole , @DashboardId ,'System' , GETDATE() ,'System' , GETDATE())

--FETCH NEXT FROM c1
--INTO @DashboardId

--END

--CLOSE c1
--DEALLOCATE c1

-----------Default DashBoard

--SELECT @DashBoardId = [Id] FROM [DI_M_Dashboard] WHERE [DashboardUniqueName] = 'ACCESS_REQUEST_MONTHLY_PROGRESS_STATUS_ADMIN'
--INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDPdAdRole , @DashBoardId ,'1','System' , GETDATE() ,'System' , GETDATE())

--SELECT @DashBoardId = [Id] FROM [DI_M_Dashboard] WHERE [DashboardUniqueName] = 'PENDING_TASKS'
--INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDPdAdRole , @DashBoardId ,'2','System' , GETDATE() ,'System' , GETDATE())

--SELECT @DashBoardId = [Id] FROM [DI_M_Dashboard] WHERE [DashboardUniqueName] = 'ACCESS_REQUEST_BY_MONTH'
--INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDPdAdRole , @DashBoardId ,'3','System' , GETDATE() ,'System' , GETDATE())

GO


print 'End'