--Dashboard Table Insert Script
print 'start'

delete from DI_M_DefaultRoleDashboard
delete from DI_M_DashboardAccessRights
delete from DI_M_Dashboard

Delete DI_M_DefaultRoleDashboard
DBCC CHECKIDENT (DI_M_DefaultRoleDashboard, reseed, 0)

Delete [DI_M_DashboardAccessRights]
DBCC CHECKIDENT ([DI_M_DashboardAccessRights], reseed, 0)

Delete DI_M_Dashboard
DBCC CHECKIDENT (DI_M_Dashboard, reseed, 0)

-------------------Dashboards---------------------------
--------------------------------------------------------

INSERT INTO DI_M_Dashboard(DashboardCategory,DashboardName ,TargetUrl ,NavigateUrl,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES('Provisioning','Access Request By Events' , '~/Dashboard/IACMPieChart.ascx' ,'~/Home/Home.aspx','1','System' , GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_Dashboard(DashboardCategory,DashboardName ,TargetUrl,NavigateUrl ,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES('My Tasks','Pending Tasks' , '~/Dashboard/TasksUC.ascx' ,'~/Task/List.aspx','2','System' , GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_Dashboard(DashboardCategory,DashboardName ,TargetUrl,NavigateUrl ,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES('Provisioning','Access Request By Month' , '~/Dashboard/IACMColumnChart.ascx' ,'~/Home/Home.aspx','1','System' , GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_Dashboard(DashboardCategory,DashboardName ,TargetUrl,NavigateUrl ,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES('Provisioning','My Request' , '~/Dashboard/MyRequest.ascx' ,'~/Request/List.aspx','2','System' ,GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_Dashboard(DashboardCategory,DashboardName ,TargetUrl,NavigateUrl ,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES('Provisioning','My Access Request' , '~/Dashboard/AccessRequestColumnChartByUser.ascx' ,'~/Home/Home.aspx','2','System' ,GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_Dashboard(DashboardCategory,DashboardName ,TargetUrl,NavigateUrl ,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES('Provisioning','My Access Request' , '~/Dashboard/AccessRequestProgressstatusbyMonth.ascx' ,'~/Home/Home.aspx','2','System' ,GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_Dashboard(DashboardCategory,DashboardName ,TargetUrl,NavigateUrl ,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES('Provisioning','My AdhocAccess Request' , '~/Dashboard/AdhocAccesRequestbyMonth.ascx' ,'~/Home/Home.aspx','2','System' ,GETDATE() ,'System' , GETDATE())
--INSERT INTO DI_M_Dashboard(DashboardCategory,DashboardName ,TargetUrl,NavigateUrl ,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES('Attestation','Task Summary' , '~/Dashboard/Attestation/TaskSummaryUC.ascx' ,'~/Home/Home.aspx','1','System' ,GETDATE() ,'System' , GETDATE())
--INSERT INTO DI_M_Dashboard(DashboardCategory,DashboardName ,TargetUrl,NavigateUrl ,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES('Attestation','Task Completeness' , '~/Dashboard/Attestation/TaskCompletenessUC.ascx' ,'~/Home/Home.aspx','2','System' ,GETDATE() ,'System' , GETDATE())
--INSERT INTO DI_M_Dashboard(DashboardCategory,DashboardName ,TargetUrl,NavigateUrl ,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES('Attestation','Task Assignee' , '~/Dashboard/Attestation/TaskAssigneeUC.ascx' ,'~/Home/Home.aspx','2','System' ,GETDATE() ,'System' , GETDATE())
--INSERT INTO DI_M_Dashboard(DashboardCategory,DashboardName ,TargetUrl,NavigateUrl ,DashBoardType,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES('Attestation','Attestation Remediate Status' , '~/Dashboard/Attestation/AttestationRemediateStatusUC.ascx' ,'~/Home/Home.aspx','2','System' ,GETDATE() ,'System' , GETDATE())

update DI_M_Dashboard set DashboardName = 'AccessRequestProgressStatus' where TargetUrl = '~/Dashboard/AccessRequestProgressstatusbyMonth.ascx'
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

INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDARole , '1' ,'1','System' , GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDARole , '2' ,'2','System' , GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDARole , '3' ,'3','System' , GETDATE() ,'System' , GETDATE())
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
DECLARE @DashboardId int
DECLARE @RIDURole int 
SELECT @RIDURole = Id from DI_M_Role where RoleName='User' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT Id
FROM [DI_M_Dashboard] where id <> 1

OPEN c1
 
FETCH NEXT FROM c1
INTO @DashboardId

WHILE @@FETCH_STATUS = 0
BEGIN

INSERT INTO [dbo].[DI_M_DashboardAccessRights](RoleId , DashboardId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDURole , @DashboardId ,'System' , GETDATE() ,'System' , GETDATE())

FETCH NEXT FROM c1
INTO @DashboardId

END

CLOSE c1
DEALLOCATE c1

-------Default Dashboard 
INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDURole , '1' ,'1','System' , GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDURole , '2' ,'2','System' , GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDURole , '3' ,'3','System' , GETDATE() ,'System' , GETDATE())
GO

--INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDURole , '3' ,'1','System' , GETDATE() ,'System' , GETDATE())
--INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDURole , '2' ,'2','System' , GETDATE() ,'System' , GETDATE())
--INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDURole , '5' ,'3','System' , GETDATE() ,'System' , GETDATE())
--GO



-----------Attestion Admin ---------------------------------
-----------Access Rights

DECLARE @DashboardId int
DECLARE @RIDAtAdRole int 
SELECT @RIDAtAdRole = Id from DI_M_Role where RoleName='Attestation Administrator' 
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
VALUES(@RIDAtAdRole , @DashboardId ,'System' , GETDATE() ,'System' , GETDATE())

FETCH NEXT FROM c1
INTO @DashboardId

END

CLOSE c1
DEALLOCATE c1

---------Default DashBoard

INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDAtAdRole , '1' ,'1','System' , GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDAtAdRole , '2' ,'2','System' , GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDAtAdRole , '3' ,'3','System' , GETDATE() ,'System' , GETDATE())
GO


-----------EndPoint Admin---------------------------------
-----------Access Rights

DECLARE @DashboardId int
DECLARE @RIDEpAdRole int 
SELECT @RIDEpAdRole = Id from DI_M_Role where RoleName='Endpoint Administrator' 
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
VALUES(@RIDEpAdRole , @DashboardId ,'System' , GETDATE() ,'System' , GETDATE())

FETCH NEXT FROM c1
INTO @DashboardId

END

CLOSE c1
DEALLOCATE c1

---------Default DashBoard

INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDEpAdRole , '1' ,'1','System' , GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDEpAdRole , '2' ,'2','System' , GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDEpAdRole , '3' ,'3','System' , GETDATE() ,'System' , GETDATE())
GO



-----------Provisioning Admin---------------------------------
-----------Access Rights

DECLARE @DashboardId int
DECLARE @RIDPdAdRole int 
SELECT @RIDPdAdRole = Id from DI_M_Role where RoleName='Provisioning Administrator'  
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
VALUES(@RIDPdAdRole , @DashboardId ,'System' , GETDATE() ,'System' , GETDATE())

FETCH NEXT FROM c1
INTO @DashboardId

END

CLOSE c1
DEALLOCATE c1

---------Default DashBoard

INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDPdAdRole , '1' ,'1','System' , GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDPdAdRole , '2' ,'2','System' , GETDATE() ,'System' , GETDATE())
INSERT INTO DI_M_DefaultRoleDashboard(RoleId ,DashBoardId ,Position,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDPdAdRole , '3' ,'3','System' , GETDATE() ,'System' , GETDATE())




GO


print 'End'