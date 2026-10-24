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
ISNULL(atttestationSnapShot.Description,'') as EntitlementDescription,
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
WHERE  IsVisible = 1 and IsActionable = 1 and Level=2
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
ISNULL(atttestationSnapShot.Description,'') as EntitlementDescription,
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
where IsVisible = 1 and IsActionable = 1 and Level=2
GO

GO

/****** Object:  StoredProcedure [dbo].[DI_USP_GetAttestationEmployeeByTaskid]    Script Date: 12/3/2024 5:55:51 pm ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




GO

ALTER PROCEDURE [dbo].[DI_USP_GetAttestationEmployeeByTaskid]
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


