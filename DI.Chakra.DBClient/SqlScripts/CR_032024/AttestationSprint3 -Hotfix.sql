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

