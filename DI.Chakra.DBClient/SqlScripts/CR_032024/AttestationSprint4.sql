USE [V5_IDMS_SP]
GO
GO
IF NOT EXISTS(select 1 from sys.objects where name ='DI_M_AttestationReportList')
BEGIN
	CREATE TABLE [dbo].[DI_M_AttestationReportList](
		[id] [bigint] IDENTITY(1,1) NOT NULL,
		[taskId] [bigint] NULL,
		[CampaignId] [bigint] NULL,
		[ReportType] [tinyint] NULL,
		[Attestationtype] [tinyint] NULL,
		[IsActive] [bit] NULL,
		[CreatedDate] [datetime] NULL,
		[CreatedBy] [nvarchar](max) NULL
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	ALTER TABLE [dbo].[DI_M_AttestationReportList] ADD  DEFAULT ((1)) FOR [IsActive]

	ALTER TABLE [dbo].[DI_M_AttestationReportList] ADD  DEFAULT (getdate()) FOR [CreatedDate]

	ALTER TABLE [dbo].[DI_M_AttestationReportList] ADD  DEFAULT ('system') FOR [CreatedBy]

END
GO
IF EXISTS(SELECT 1 FROM sys.objects where name='DI_USP_GetAttestationEmployeeByTaskid' AND type='P')
BEGIN
/****** Object:  StoredProcedure [dbo].[DI_USP_GetAttestationEmployeeByTaskid]    Script Date: 19/4/2024 11:35:37 am ******/
DROP PROCEDURE [dbo].[DI_USP_GetAttestationEmployeeByTaskid]
END
GO

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
						ON  H.TaskId = approverTask.Id
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
						ON  snapShotActionH.TaskId = approverTask.Id
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
/****** Object:  StoredProcedure [dbo].[USP_UpdateAttestationReviewAction]    Script Date: 15/5/2024 5:22:53 pm ******/
DROP PROCEDURE [dbo].[USP_UpdateAttestationReviewAction]
GO

/****** Object:  StoredProcedure [dbo].[USP_UpdateAttestationReviewAction]    Script Date: 15/5/2024 5:22:53 pm ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE PROC [dbo].[USP_UpdateAttestationReviewAction]
(
@endpointId BIGINT,
@taskId BIGINT,
@userid BIGINT,
@entitlement NVARCHAR(1024),
@username NVARCHAR(1024),
@accountType TINYINT,
@accessType NVARCHAR(32),
@actionUserId BIGINT,
@level int,
@entitlementType int
)
AS
BEGIN


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
			AND a.Level =  1 AND A.EntitlementType = @entitlementType
		END
		ELSE
		BEGIN
			INSERT INTO @IDs(Id,snapShotId)
			SELECT b.Id,A.Id from DI_M_AttestationSnapshot a JOIN DI_M_AttestationSnapshotAction b ON a.id = b.AttestationSnapshotId WHERE AttestationCampaignId = @campaignId AND UserId = @userid AND AccountName=@username AND AccountType = @accountType AND  Name= @entitlement and b.IsActionable = 1
			AND a.Level =  1  AND A.EntitlementType = @entitlementType
		END
	END
	ELSE
	BEGIN
			INSERT INTO @IDs(Id,snapShotId)
			SELECT b.Id,A.Id from DI_M_AttestationSnapshot a JOIN DI_M_AttestationSnapshotAction b ON a.id = b.AttestationSnapshotId 
			WHERE	AttestationCampaignId = @campaignId AND EndpointId = @endpointId AND UserId = @userid AND AccountName=@username AND AccountType = @accountType AND  Name= @entitlement and b.IsActionable = 1
					AND A.EntitlementType = @entitlementType
			IF (@approve = 1)
			BEGIN
					INSERT INTO @IDs(Id,snapShotId)
					SELECT b.Id,A.Id from DI_M_AttestationSnapshot a JOIN DI_M_AttestationSnapshotAction b ON a.id = b.AttestationSnapshotId 
					WHERE AttestationCampaignId = @campaignId AND EndpointId = @endpointId AND UserId = @userid AND AccountName=@username AND AccountType = @accountType AND   Level=1 and b.IsActionable = 1
					AND A.EntitlementType = @entitlementType
			END

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

IF NOT EXISTS(SELECT 1 from DI_M_IMConfigurationMaster WHERE ConfigKey='ShowReviewAttestationNotes')
begin
INSERT INTO DI_M_IMConfigurationMaster(ConfigKey,ConfigValue,CreatedDate,CreatedBy,IsUiDisplay,[Description])
values('ShowReviewAttestationNotes','0',GETDATE(),'System',1,'Show or hide review attestationNotes') 
End
GO

IF NOT EXISTS(SELECT 1 from DI_M_IMConfigurationMaster WHERE ConfigKey='ShowUserComparisionLinkForOnlyOtherSystem')
begin
INSERT INTO DI_M_IMConfigurationMaster(ConfigKey,ConfigValue,CreatedDate,CreatedBy,IsUiDisplay,[Description])
values('ShowUserComparisionLinkForOnlyOtherSystem','0',GETDATE(),'System',1,'Enable User Comparision View Only for Other System') 
End
GO
IF NOT EXISTS(SELECT 1 from DI_M_IMConfigurationMaster WHERE ConfigKey='UseOldReviewPage')
begin
INSERT INTO DI_M_IMConfigurationMaster(ConfigKey,ConfigValue,CreatedDate,CreatedBy,IsUiDisplay,[Description])
values('UseOldReviewPage','1',GETDATE(),'System',1,'Used to handle access review page redirection to default review page') 
End
GO

DECLARE @UrlId bigint
DECLARE @PageId bigint
DECLARE @TabParentMenuId bigint

IF NOT EXISTS(SELECT 1 from [DI_M_PageUrl] WHERE [Url]='/Report/Attestation/AttestationUserComparisionReport.aspx' )
BEGIN
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Report/Attestation/AttestationUserComparisionReport.aspx'  , SYSDATETIME())
END
ELSE
BEGIN
PRINT '/Report/Attestation/AttestationUserComparisionReport.aspx Exists'
END
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Attestation/AttestationUserComparisionReport.aspx'

IF NOT EXISTS(SELECT 1 from [DI_M_Page] WHERE [UniqueName]='ATTESTATION_USERPROFILE_CAMPAIGN_REPORT' AND [Name]='User Comparision Report' AND [PageUrlId]=@UrlId)
BEGIN
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ATTESTATION_USERPROFILE_CAMPAIGN_REPORT','User Comparision Report',@UrlId , SYSDATETIME())
END
BEGIN
print @UrlId
PRINT 'ATTESTATION_USERPROFILE_CAMPAIGN_REPORT Exists'
END


SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ATTESTATION_CAMPAIGN'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ATTESTATION_USERPROFILE_CAMPAIGN_REPORT'

IF NOT EXISTS(SELECT 1 FROM [dbo].[DI_M_TabMenu] WHERE [ParentTabMenuId] = @TabParentMenuId AND [UniqueName]='ATTESTATION_USERPROFILE_CAMPAIGN_REPORT' AND [MenuName]='User Comparision Report' AND [PageId] = @PageId)
BEGIN
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ATTESTATION_USERPROFILE_CAMPAIGN_REPORT', 'User Comparision Report' , @PageId, SYSDATETIME())
END
ELSE
BEGIN
PRINT 'TAB MENU EXISTS ATTESTATION_USERPROFILE_CAMPAIGN_REPORT'
END

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ATTESTATION_USERPROFILE_CAMPAIGN_REPORT'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Attestation/AttestationUserComparisionReport.aspx' 

DECLARE @PAGEPERMISSION BIGINT
IF NOT EXISTS(SELECT 1 FROM [DI_M_PagePermission] WHERE PageId = @PageId AND PermissionId = 1 AND UrlId = @UrlId)
BEGIN
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SET @PAGEPERMISSION = @@ROWCOUNT

END
ELSE
BEGIN
	PRINT 'PAGEPERMISSION EXISTS..'
	SELECT @PAGEPERMISSION = Id FROM [DI_M_PagePermission] where PageId = @PageId AND PermissionId = 1 AND UrlId = @UrlId
END
IF NOT EXISTS(SELECT 1 FROM [DI_M_AccessRights] WHERE RoleId = 1 AND PagePermissionId = @PAGEPERMISSION)
BEGIN
	INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
	VALUES(1, @PAGEPERMISSION, 'System' , SYSDATETIME(),'System' , SYSDATETIME())
END


IF NOT EXISTS(SELECT 1 FROM [DI_M_PagePermission] WHERE PageId = @PageId AND PermissionId = 12 AND UrlId = @UrlId)
BEGIN
	INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())
	SET @PAGEPERMISSION = @@ROWCOUNT
END
ELSE
BEGIN
	PRINT 'PAGEPERMISSION EXISTS..'
	SELECT @PAGEPERMISSION = Id FROM [DI_M_PagePermission] where PageId = @PageId AND PermissionId = 12 AND UrlId = @UrlId
END
IF NOT EXISTS(SELECT 1 FROM [DI_M_AccessRights] WHERE RoleId = 1 AND PagePermissionId = @PAGEPERMISSION)
BEGIN
	INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
	VALUES(1, @PAGEPERMISSION, 'System' , SYSDATETIME(),'System' , SYSDATETIME())
END
