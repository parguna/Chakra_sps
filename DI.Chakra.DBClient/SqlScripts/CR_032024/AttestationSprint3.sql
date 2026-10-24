USE [V5_IDMS_SP]
GO
/****** Object:  View [dbo].[vwAttestationTaskEntitlementUserComparision]    Script Date: 12/3/2024 5:06:30 pm ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER VIEW [dbo].[vwAttestationTaskEntitlementUserComparision]
AS
SELECT
ROW_NUMBER() OVER(ORDER BY Entity) as Id,
approverTask.Id AS TaskId,
atttestationSnapShot.Entity as Entitlement,
CASE WHEN atttestationSnapShot.EndpointType = 6 THEN atttestationSnapShot.Reserved1 ELSE ISNULL(atttestationSnapShot.Description,'') End as EntitlementDescription,
atttestationSnapShot.EntitlementType,
snapShotAction.Id AS SnapshotActionId,
atttestationSnapShot.DIUserDisplayName AS Name,
atttestationSnapShot.AccountName AS UserName,
usr.EmployeeId AS EmployeeNo,
usr.EmployeeType AS EmployeeType,
usr.TerminationDate AS TerminationDate,
atttestationSnapShot.UserId,
atttestationSnapShot.AccountType,
atttestationSnapShot.EndpointId AS EndpointSystemId,
atttestationSnapShot.EndpointName AS EndpointSystemName,
atttestationSnapShot.EndpointType AS EndpointType,
snapShotActionInterim.UnSavedApprove AS Approve,
snapShotActionInterim.UnSavedRevoke AS [Revoke],
snapShotAction.IsActionable
FROM DI_M_AttestationSnapshotAction snapShotAction
JOIN DI_M_AttestationSnapshotActionInterim snapShotActionInterim ON snapShotAction.Id = snapShotActionInterim.AttestationSnapshotActionId
JOIN DI_M_AttestationSnapshot atttestationSnapShot ON snapShotAction.AttestationSnapshotId = atttestationSnapShot.Id
JOIN DI_M_Task approverTask ON  snapShotAction.TaskId = approverTask.Id
JOIN DI_M_AttestationCampaign attetsationCampaign ON atttestationSnapShot.AttestationCampaignId = attetsationCampaign.Id
JOIN DI_M_Attestation attestation ON attetsationCampaign.AttestationID = attestation.Id
JOIN DI_M_User usr ON atttestationSnapShot.UserId = usr.Id
LEFT JOIN DI_M_User newApprover ON CAST(snapShotActionInterim.UnSavedReserved1 AS INT) = newApprover.Id
WHERE  IsVisible = 1 and Level=2
GO

GO
/****** Object:  View [dbo].[vwAttestationTaskHistoryEntitlementUserComparision]    Script Date: 12/3/2024 5:06:48 pm ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER VIEW [dbo].[vwAttestationTaskHistoryEntitlementUserComparision]
AS
SELECT
ROW_NUMBER() OVER(ORDER BY Entity) as Id,
approverTask.Id AS TaskId,
atttestationSnapShot.Entity as Entitlement,
CASE WHEN atttestationSnapShot.EndpointType = 6 THEN atttestationSnapShot.Reserved1 ELSE ISNULL(atttestationSnapShot.Description,'') End as EntitlementDescription,
atttestationSnapShot.EntitlementType,
snapShotAction.Id AS SnapshotActionId,
atttestationSnapShot.DIUserDisplayName AS Name,
atttestationSnapShot.AccountName AS UserName,
usr.EmployeeId AS EmployeeNo,
usr.EmployeeType AS EmployeeType,
usr.TerminationDate AS TerminationDate,
atttestationSnapShot.UserId,
atttestationSnapShot.AccountType,
atttestationSnapShot.EndpointId AS EndpointSystemId,
atttestationSnapShot.EndpointName AS EndpointSystemName,
atttestationSnapShot.EndpointType AS EndpointType,
snapShotHistory.Approve AS Approve,
snapShotHistory.[Revoke] AS [Revoke],
snapShotAction.IsActionable
FROM DI_M_AttestationSnapshotAction_History snapShotHistory
JOIN DI_M_AttestationSnapshotAction snapShotAction ON snapShotHistory.AttestationSnapshotActionId = snapShotAction.Id
JOIN DI_M_AttestationSnapshot atttestationSnapShot ON snapShotAction.AttestationSnapshotId = atttestationSnapShot.Id
JOIN DI_M_Task approverTask ON  snapShotHistory.TaskId = approverTask.Id 
JOIN DI_M_AttestationCampaign attetsationCampaign ON atttestationSnapShot.AttestationCampaignId = attetsationCampaign.Id
JOIN DI_M_Attestation attestation ON attetsationCampaign.AttestationID = attestation.Id
JOIN DI_M_User usr ON atttestationSnapShot.UserId = usr.Id
LEFT JOIN DI_M_User newApprover ON CAST(snapShotHistory.ReviewerId AS BIGINT) = newApprover.Id
where IsVisible = 1 and  Level=2
GO

GO

ALTER PROC [dbo].[USP_UpdateAttestationReviewAction]
(
@endpointId BIGINT,
@taskId BIGINT,
@userid BIGINT,
@entitlement NVARCHAR(1024),
@username NVARCHAR(1024),
@accountType TINYINT,
@accessType NVARCHAR(32),
@actionUserId BIGINT,
@level int
)
AS
BEGIN

	--DECLARE @taskId BIGINT
	--DECLARE @userid BIGINT
	--DECLARE @endpointId BIGINT
	--DECLARE @entitlement VARCHAR(1024)
	--DECLARE @username VARCHAR(1024)
	--DECLARE @accountType TINYINT
	--DECLARE @accessType VARCHAR(32)
	--DECLARE @actionUserId BIGINT

	DECLARE @campaignId bigint
	DECLARE @actionUserName VARCHAR(1024)
	SELECT @actionUserName = UserName from DI_M_User WHERE id  = @actionUserId
	SELECT @campaignId = AttestationCampaignId from DI_M_Task WHERE id  = @taskId

	IF EXISTS(SELECT 1 FROM DI_M_AttestationSnapshotTaskStatus  WHERE TaskId = @taskId)
	BEGIN
			UPDATE DI_M_AttestationSnapshotTaskStatus SET UnSavedChangesAvailable = 1 WHERE TaskId = @taskId

	END
	ELSE
	BEGIN
			INSERT INTO DI_M_AttestationSnapshotTaskStatus(AttestationId,CampaignId,CreatedBy,CreatedDate,TaskId,UnSavedChangesAvailable,LastModifiedBy,LastModifiedDate)
			SELECT AttestationId,AttestationCampaignId,@actionUserName,GETDATE(),Id,1,@actionUserName,GETDATE() from DI_M_Task where ID = @taskId
	END

	DECLARE @approve bit
	DECLARE @Revoke bit
	SET @approve = (CASE WHEN @accessType = 'Retain' THEN 1 ELSE 0 END);
	SET @Revoke = (CASE WHEN @approve = 1 THEN 0 ELSE 1 END);
	declare @parentId VARCHAR(1024)
	declare @snapShotActionId bigint

	DECLARE @IDs TABLE(Id bigint, snapShotId bigint);
	IF @level = 1
	BEGIN
		IF (@endpointId > 0)
		BEGIN
			INSERT INTO @IDs(Id,snapShotId)
			select b.Id,A.Id from DI_M_AttestationSnapshot a JOIN DI_M_AttestationSnapshotAction b ON a.id = b.AttestationSnapshotId WHERE AttestationCampaignId = @campaignId AND EndpointId = @endpointId AND UserId = @userid AND AccountName=@username AND AccountType = @accountType AND  Name= @entitlement and b.IsActionable = 1
			AND a.Level =  1
		END
		ELSE
		BEGIN
			INSERT INTO @IDs(Id,snapShotId)
			select b.Id,A.Id from DI_M_AttestationSnapshot a JOIN DI_M_AttestationSnapshotAction b ON a.id = b.AttestationSnapshotId WHERE AttestationCampaignId = @campaignId AND UserId = @userid AND AccountName=@username AND AccountType = @accountType AND  Name= @entitlement and b.IsActionable = 1
			AND a.Level =  1
		END
	END
	ELSE
	BEGIN
		INSERT INTO @IDs(Id,snapShotId)
			select b.Id,A.Id from DI_M_AttestationSnapshot a JOIN DI_M_AttestationSnapshotAction b ON a.id = b.AttestationSnapshotId WHERE AttestationCampaignId = @campaignId AND EndpointId = @endpointId AND UserId = @userid AND AccountName=@username AND AccountType = @accountType AND  Name= @entitlement and b.IsActionable = 1
	END

	UPDATE DI_M_AttestationSnapshotActionInterim SET UnSavedApprove = @approve , UnSavedRevoke = @Revoke, UnSavedIsChanged = 1, TaskId = @taskId, ReviewerId = @actionUserId, LastModifiedBy = @actionUserName WHERE AttestationSnapshotActionId IN(select Id from @IDs)

	IF(@level > 1 AND @approve = 1)
	BEGIN
		IF EXISTS(SELECT 1 from DI_M_AttestationSnapshotAction WHERE RootSnapshotId IN(SELECT snapShotId from @IDs) )
		BEGIN
				UPDATE DI_M_AttestationSnapshotActionInterim SET UnSavedEffectiveDate=null, UnSavedApprove = 1, UnSavedRevoke = 0 ,UnSavedIsChanged = 1, TaskId = @taskId, ReviewerId = @actionUserId, LastModifiedBy = @actionUserName
				WHERE AttestationSnapshotActionId in (SELECT ID from DI_M_AttestationSnapshotAction WHERE RootSnapshotId IN(SELECT snapShotId from @IDs));
		END

	END
	ELSE 
	BEGIN
				IF EXISTS(SELECT 1 from DI_M_AttestationSnapshotAction WHERE RootSnapshotId IN(SELECT snapShotId from @IDs))
				BEGIN
						UPDATE DI_M_AttestationSnapshotActionInterim SET UnSavedIsChanged = 1, UnSavedApprove = @approve , UnSavedRevoke = @Revoke, TaskId = @taskId, ReviewerId = @actionUserId, LastModifiedBy = @actionUserName 
						WHERE AttestationSnapshotActionId in (SELECT ID from DI_M_AttestationSnapshotAction WHERE RootSnapshotId IN(SELECT snapShotId from @IDs));
				END

	END
END
GO

IF EXISTS(SELECT 1 FROM sys.objects where name='USP_GETACCOUNTSTATUSBYTASKID' AND type='P')
BEGIN
/****** Object:  StoredProcedure [dbo].[USP_GETACCOUNTSTATUSBYTASKID]    Script Date: 19/4/2024 11:35:37 am ******/
DROP PROCEDURE [dbo].[USP_GETACCOUNTSTATUSBYTASKID]
END
GO

/****** Object:  StoredProcedure [dbo].[USP_GETACCOUNTSTATUSBYTASKID]    Script Date: 19/4/2024 11:35:37 am ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*
NAME: USP_GETACCOUNTSTATUSBYTASKID
AUTHOR: Sivanandan Ravindranathan
DESC:	Used to show the employee accounts for action(Revoke/Retain)
*/
CREATE PROC [dbo].[USP_GETACCOUNTSTATUSBYTASKID]
(
@taskId bigint,
@accountType int,
@userId bigint,
@accountname nvarchar(1024)=NULL,
@endpointSystemIds XML = NULL
)
AS
BEGIN
SET NOCOUNT ON;
	
	DECLARE @TaskStatus int
	SELECT @TaskStatus = [Action] from DI_M_Task where Id = @taskId
	

	IF (@TaskStatus = 1)
	BEGIN

			IF ((ISNULL(@accountname,'')<>'') AND LEN(LTRIM(RTRIM(@accountname)))> 0)
			BEGIN
					SELECT
					DISTINCT 
						Emp.KnownAs AS Name,
						Emp.EmployeeId as AdditionalInfo,
						UPPER(atttestationSnapShot.AccountName) as UserName,
						atttestationSnapShot.UserId as Id,
						atttestationSnapShot.EndpointId,
						atttestationSnapShot.EndpointName as EndpointName,
						snapShotActionInterim.UnSavedApprove As AccountStatus,
						snapShotAction.IsActionable As IsActionable
					FROM DI_M_AttestationSnapshotAction snapShotAction
					JOIN DI_M_AttestationSnapshotActionInterim snapShotActionInterim
					ON snapShotAction.Id = snapShotActionInterim.AttestationSnapshotActionId
					JOIN DI_M_AttestationSnapshot atttestationSnapShot
					ON snapShotAction.AttestationSnapshotId = atttestationSnapShot.Id
					JOIN DI_M_Task approverTask
					ON  snapShotAction.TaskId = approverTask.Id
					JOIN DI_M_AttestationCampaign attetsationCampaign
					on atttestationSnapShot.AttestationCampaignId = attetsationCampaign.Id
					JOIN DI_M_User Emp on atttestationSnapShot.UserId = Emp.Id
					WHERE approverTask.Id = @taskId 
					AND  atttestationSnapShot.UserId = @userId
					AND atttestationSnapShot.AccountName = @accountname
					AND atttestationSnapShot.AccountType = @accountType
					AND atttestationSnapShot.Level = 1
					ORDER BY Emp.KnownAs
			END
			ELSE
			BEGIN
					SELECT
					DISTINCT 
						Emp.KnownAs AS Name,
						Emp.EmployeeId as AdditionalInfo,
						UPPER(atttestationSnapShot.AccountName) as UserName,
						atttestationSnapShot.UserId as Id,
						atttestationSnapShot.EndpointId,
						atttestationSnapShot.EndpointName as EndpointName,
						snapShotActionInterim.UnSavedApprove As AccountStatus,
						snapShotAction.IsActionable As IsActionable
					FROM DI_M_AttestationSnapshotAction snapShotAction
					JOIN DI_M_AttestationSnapshotActionInterim snapShotActionInterim
					ON snapShotAction.Id = snapShotActionInterim.AttestationSnapshotActionId
					JOIN DI_M_AttestationSnapshot atttestationSnapShot
					ON snapShotAction.AttestationSnapshotId = atttestationSnapShot.Id
					JOIN DI_M_Task approverTask
					ON  snapShotAction.TaskId = approverTask.Id
					JOIN DI_M_AttestationCampaign attetsationCampaign
					on atttestationSnapShot.AttestationCampaignId = attetsationCampaign.Id
					JOIN DI_M_User Emp on atttestationSnapShot.UserId = Emp.Id
					WHERE approverTask.Id = @taskId 
					AND atttestationSnapShot.UserId = @userId
					AND atttestationSnapShot.AccountType = @accountType
					AND atttestationSnapShot.Level = 1
					ORDER BY Emp.KnownAs
			END
	END
	ELSE
	BEGIN
			IF ((ISNULL(@accountname,'')<>'') AND LEN(LTRIM(RTRIM(@accountname)))> 0)
			BEGIN
					SELECT
					DISTINCT 
						Emp.KnownAs AS Name,
						Emp.EmployeeId as AdditionalInfo,
						UPPER(atttestationSnapShot.AccountName) as UserName,
						atttestationSnapShot.UserId as Id,
						atttestationSnapShot.EndpointId,
						atttestationSnapShot.EndpointName as EndpointName,
						H.Approve As AccountStatus,
						snapShotAction.IsActionable As IsActionable
					FROM DI_M_AttestationSnapshotAction_History H
					JOIN DI_M_AttestationSnapshotAction snapShotAction on H.AttestationSnapshotActionId = snapShotAction.Id
					JOIN DI_M_AttestationSnapshot atttestationSnapShot
					ON snapShotAction.AttestationSnapshotId = atttestationSnapShot.Id
					JOIN DI_M_Task approverTask
					ON  snapShotAction.TaskId = approverTask.Id
					JOIN DI_M_AttestationCampaign attetsationCampaign
					on atttestationSnapShot.AttestationCampaignId = attetsationCampaign.Id
					JOIN DI_M_User Emp on atttestationSnapShot.UserId = Emp.Id
					WHERE approverTask.Id = @taskId  
					AND atttestationSnapShot.UserId = @userId
					AND atttestationSnapShot.AccountName = @accountname
					AND atttestationSnapShot.AccountType = @accountType
					AND atttestationSnapShot.Level = 1
					ORDER BY Emp.KnownAs
			END
			ELSE
			BEGIN
					SELECT
					DISTINCT 
						Emp.KnownAs AS Name,
						Emp.EmployeeId as AdditionalInfo,
						UPPER(atttestationSnapShot.AccountName) as UserName,
						atttestationSnapShot.UserId as Id,
						atttestationSnapShot.EndpointId,
						atttestationSnapShot.EndpointName as EndpointName,
						H.Approve As AccountStatus,
						snapShotAction.IsActionable As IsActionable
					FROM DI_M_AttestationSnapshotAction_History H
					JOIN DI_M_AttestationSnapshotAction snapShotAction on H.AttestationSnapshotActionId = snapShotAction.Id
					JOIN DI_M_AttestationSnapshot atttestationSnapShot
					ON snapShotAction.AttestationSnapshotId = atttestationSnapShot.Id
					JOIN DI_M_Task approverTask
					ON  snapShotAction.TaskId = approverTask.Id
					JOIN DI_M_AttestationCampaign attetsationCampaign
					on atttestationSnapShot.AttestationCampaignId = attetsationCampaign.Id
					JOIN DI_M_User Emp on atttestationSnapShot.UserId = Emp.Id
					WHERE approverTask.Id = @taskId 
					AND atttestationSnapShot.UserId = @userId
					AND atttestationSnapShot.AccountType = @accountType
					AND atttestationSnapShot.Level = 1
					ORDER BY Emp.KnownAs


			END
	END
		
END
GO

IF EXISTS(SELECT 1 FROM sys.objects where name='DI_USP_GetAttestationEmployeeByTaskid' AND type='P')
BEGIN
/****** Object:  StoredProcedure [dbo].[DI_USP_GetAttestationEmployeeByTaskid]    Script Date: 19/4/2024 11:35:37 am ******/
DROP PROCEDURE [dbo].[DI_USP_GetAttestationEmployeeByTaskid]
END
GO

/****** Object:  StoredProcedure [dbo].[DI_USP_GetAttestationEmployeeByTaskid]    Script Date: 19/4/2024 11:40:03 am ******/

GO

/****** Object:  StoredProcedure [dbo].[DI_USP_GetAttestationEmployeeByTaskid]    Script Date: 19/4/2024 11:40:03 am ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/*

EXEC [dbo].[DI_USP_GetAttestationEmployeeByTaskid] 268753,'<xml><ArrayOfLong><long>3</long></ArrayOfLong></xml>'

*/
CREATE PROCEDURE [dbo].[DI_USP_GetAttestationEmployeeByTaskid]
    @taskId bigint,
	@endpointSystemIds XML = NULL,
	@accountType tinyint=1
AS
BEGIN
SET NOCOUNT ON;
	
	DECLARE @TaskStatus int
	SELECT @TaskStatus = [Action] from DI_M_Task where Id = @taskId
	
	CREATE TABLE #tblEndPoints(endpointId bigint)
	
	INSERT INTO #tblEndPoints(endpointId)
		SELECT node.query('.').value('.','bigint') as FileId 
		FROM @endpointSystemIds.nodes('/ArrayOfLong/long')as result(node)


		IF NOT EXISTS(SELECT 1 FROM #tblEndPoints)
		BEGIN

				IF (@TaskStatus = 1)
				BEGIN
						SELECT
						DISTINCT 
							Emp.KnownAs AS Name,
							Emp.EmployeeId as AdditionalInfo,
							atttestationSnapShot.AccountName as UserName,
							atttestationSnapShot.UserId as Id,
							atttestationSnapShot.EndpointId,
							atttestationSnapShot.EndpointName as EndpointName
						FROM DI_M_AttestationSnapshotAction snapShotAction
						JOIN DI_M_AttestationSnapshotActionInterim snapShotActionInterim
						ON snapShotAction.Id = snapShotActionInterim.AttestationSnapshotActionId
						JOIN DI_M_AttestationSnapshot atttestationSnapShot
						ON snapShotAction.AttestationSnapshotId = atttestationSnapShot.Id
						JOIN DI_M_Task approverTask
						ON  snapShotAction.TaskId = approverTask.Id
						JOIN DI_M_AttestationCampaign attetsationCampaign
						on atttestationSnapShot.AttestationCampaignId = attetsationCampaign.Id
						JOIN DI_M_User Emp on atttestationSnapShot.UserId = Emp.Id
						WHERE approverTask.Id = @taskId 
						AND atttestationSnapShot.AccountType = @accountType AND atttestationSnapShot.UserId IS nOT NULL AND atttestationSnapShot.UserId > 0
						ORDER BY Emp.KnownAs
				END
				ELSE
				BEGIN
						SELECT
						DISTINCT 
							Emp.KnownAs AS Name,
							Emp.EmployeeId as AdditionalInfo,
							atttestationSnapShot.AccountName as UserName,
							atttestationSnapShot.UserId as Id,
							atttestationSnapShot.EndpointId,
							atttestationSnapShot.EndpointName as EndpointName
						FROM DI_M_AttestationSnapshotAction_History H
						JOIN DI_M_AttestationSnapshotAction snapShotAction on H.AttestationSnapshotActionId = snapShotAction.Id
						JOIN DI_M_AttestationSnapshot atttestationSnapShot
						ON snapShotAction.AttestationSnapshotId = atttestationSnapShot.Id
						JOIN DI_M_Task approverTask
						ON  snapShotAction.TaskId = approverTask.Id
						JOIN DI_M_AttestationCampaign attetsationCampaign
						on atttestationSnapShot.AttestationCampaignId = attetsationCampaign.Id
						JOIN DI_M_User Emp on atttestationSnapShot.UserId = Emp.Id
						WHERE approverTask.Id = @taskId 
						AND atttestationSnapShot.AccountType = @accountType AND atttestationSnapShot.UserId IS nOT NULL AND atttestationSnapShot.UserId > 0
						ORDER BY Emp.KnownAs

				END
		END
		ELSE
		BEGIN

				IF (@TaskStatus = 1)
				BEGIN
						SELECT
						DISTINCT 
							Emp.KnownAs AS Name,
							Emp.EmployeeId as AdditionalInfo,
							atttestationSnapShot.AccountName as UserName,
							atttestationSnapShot.UserId as Id,
							atttestationSnapShot.EndpointId,
							atttestationSnapShot.EndpointName as EndpointName
						FROM DI_M_AttestationSnapshotAction snapShotAction
						JOIN DI_M_AttestationSnapshotActionInterim snapShotActionInterim
						ON snapShotAction.Id = snapShotActionInterim.AttestationSnapshotActionId
						JOIN DI_M_AttestationSnapshot atttestationSnapShot
						ON snapShotAction.AttestationSnapshotId = atttestationSnapShot.Id
						JOIN DI_M_Task approverTask
						ON  snapShotAction.TaskId = approverTask.Id
						JOIN DI_M_AttestationCampaign attetsationCampaign
						on atttestationSnapShot.AttestationCampaignId = attetsationCampaign.Id
						JOIN DI_M_User Emp on atttestationSnapShot.UserId = Emp.Id
						WHERE approverTask.Id = @taskId AND atttestationSnapShot.EndpointId IN(SELECT endpointId FROM #tblEndPoints) 
						AND atttestationSnapShot.AccountType = @accountType AND atttestationSnapShot.UserId IS nOT NULL AND atttestationSnapShot.UserId > 0
						ORDER BY Emp.KnownAs
				END
				ELSE
				BEGIN

						SELECT
						DISTINCT 
							Emp.KnownAs AS Name,
							Emp.EmployeeId as AdditionalInfo,
							atttestationSnapShot.AccountName as UserName,
							atttestationSnapShot.UserId as Id,
							atttestationSnapShot.EndpointId,
							atttestationSnapShot.EndpointName as EndpointName
						FROM DI_M_AttestationSnapshotAction_History snapShotActionH
						JOIN DI_M_AttestationSnapshotAction snapShotAction
						ON snapShotActionH.AttestationSnapshotActionId = snapShotAction.Id
						JOIN DI_M_AttestationSnapshot atttestationSnapShot
						ON snapShotAction.AttestationSnapshotId = atttestationSnapShot.Id
						JOIN DI_M_Task approverTask
						ON  snapShotAction.TaskId = approverTask.Id
						JOIN DI_M_AttestationCampaign attetsationCampaign
						on atttestationSnapShot.AttestationCampaignId = attetsationCampaign.Id
						JOIN DI_M_User Emp on atttestationSnapShot.UserId = Emp.Id
						WHERE approverTask.Id = @taskId AND atttestationSnapShot.EndpointId IN(SELECT endpointId FROM #tblEndPoints) 
						AND atttestationSnapShot.AccountType = @accountType AND atttestationSnapShot.UserId IS nOT NULL AND atttestationSnapShot.UserId > 0
						ORDER BY Emp.KnownAs
				END

		END
END


GO

IF EXISTS(SELECT 1 FROM sys.objects where name='NINX_EndpointId_AccountType_UserId' )
BEGIN
DROP INDEX [NINX_EndpointId_AccountType_UserId] ON [dbo].[DI_M_AttestationSnapshot]
END
GO
CREATE NONCLUSTERED INDEX [NINX_EndpointId_AccountType_UserId] ON [dbo].[DI_M_AttestationSnapshot]
(
	[EndpointId] ASC,
	[AccountType] ASC,
	[UserId] ASC,
	[Id] ASC
)
INCLUDE([AccountName],[EndpointName]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]

GO
IF EXISTS(SELECT 1 FROM sys.objects where name='NINX_TaskId_AttestationSnapshotId ' )
BEGIN
DROP INDEX NINX_TaskId_AttestationSnapshotId ON [dbo].[DI_M_AttestationSnapshotAction]
END
GO

CREATE NONCLUSTERED INDEX NINX_TaskId_AttestationSnapshotId ON [dbo].[DI_M_AttestationSnapshotAction]
(
	[TaskId] ASC,
	[IsVisible] ASC,
	[Id] ASC,
	[AttestationSnapshotId] ASC
)
INCLUDE([IsActionable]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]

GO
IF EXISTS(SELECT 1 FROM sys.objects where name='NINX_TaskId_AttestationSnapshotActionInterim' )
BEGIN

DROP INDEX NINX_TaskId_AttestationSnapshotActionInterim ON [dbo].[DI_M_AttestationSnapshotActionInterim]
END
GO

CREATE NONCLUSTERED INDEX NINX_TaskId_AttestationSnapshotActionInterim ON [dbo].[DI_M_AttestationSnapshotActionInterim]
(
	[TaskId] ASC
)
INCLUDE([Id],[UnSavedApprove],[UnSavedRevoke],[UnSavedComment],[UnSavedEffectiveDate],[UnSavedIsChanged],[UnSavedReserved1],[UnSavedReserved2],[UnSavedReserved3]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO

GO
IF EXISTS(SELECT 1 FROM sys.objects where name='DI_USP_UpdateINHReconEntitlementDescription' AND type='P')
BEGIN
/****** Object:  StoredProcedure [dbo].[DI_USP_GetAttestationEmployeeByTaskid]    Script Date: 19/4/2024 11:35:37 am ******/
DROP PROCEDURE [dbo].DI_USP_UpdateINHReconEntitlementDescription
END
GO

CREATE OR ALTER PROC DI_USP_UpdateINHReconEntitlementDescription
@endPointSystemId bigint=0
AS
BEGIN
		Update er SET er.EntitlementDescription=ISNULL(inp.Description,'') FROM DI_M_EntitlementReconcilation er join 
		DI_INH_Permission INP ON INP.Name=er.Entitlement and inp.EndPointSystemId=er.EndpointSystemId WHERE er.EndpointSystemId=@endPointSystemId
		AND inp.EndPointSystemId=@endPointSystemId AND er.Entitlement IS NOT NULL AND INP.Name IS NOT NULL AND er.IsDeleted=0
END
GO

IF EXISTS(SELECT 1 FROM sys.objects where name='DI_USP_UpdateINHReconEntitlementDescription' AND type='P')
BEGIN
/****** Object:  StoredProcedure [dbo].[DI_USP_GetAttestationEmployeeByTaskid]    Script Date: 19/4/2024 11:35:37 am ******/
DROP PROCEDURE [dbo].DI_USP_UpdateINHReconEntitlementDescription
END
GO

CREATE OR ALTER PROC DI_USP_UpdateINHReconEntitlementDescription
@endPointSystemId bigint=0
AS
BEGIN
		Update er SET er.EntitlementDescription=inp.Description FROM DI_M_EntitlementReconcilation er join 
		DI_INH_Permission INP ON INP.Name=er.Entitlement and inp.EndPointSystemId=er.EndpointSystemId WHERE er.EndpointSystemId=@endPointSystemId
		AND inp.EndPointSystemId=@endPointSystemId AND er.Entitlement IS NOT NULL AND INP.Name IS NOT NULL AND er.IsDeleted=0
END
GO

CREATE NONCLUSTERED INDEX [NINX_SNAPSHOTHISTORY]
ON [dbo].[DI_M_AttestationSnapshotAction_History] ([TaskId])
INCLUDE ([AttestationSnapshotActionId],[Approve],[Revoke])
GO
