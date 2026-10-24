IF EXISTS (SELECT * FROM [dbo].[sysobjects]
       WHERE ID = object_id(N'[dbo].[DI_M_AttestationSnapshotAction_Trigger]') AND
         OBJECTPROPERTY(id, N'IsTrigger') = 1)
    DROP TRIGGER [dbo].[DI_M_AttestationSnapshotAction_Trigger]
GO

CREATE TRIGGER [dbo].[DI_M_AttestationSnapshotAction_Trigger] ON [dbo].[DI_M_AttestationSnapshotAction]
AFTER UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE	actionHistory
	SET		actionHistory.[Approve] = i.[Approve],
			actionHistory.[Revoke] = i.[Revoke],
			actionHistory.[Comment] = REPLACE(ISNULL(i.[Comment], ''), ISNULL(d.[Comment], ''), ''),
			actionHistory.[EffectiveDate] = i.[EffectiveDate],
			actionHistory.[ReviewerId] = i.[ReviewerId],
			actionHistory.[LastModifiedBy] = i.[LastModifiedBy],
			actionHistory.[LastModifiedDate] = i.[LastModifiedDate]
	FROM		[dbo].[DI_M_AttestationSnapshotAction_History] AS actionHistory
	INNER JOIN	INSERTED AS i ON i.TaskId = actionHistory.TaskId AND i.Id = actionHistory.AttestationsnapshotActionId
	INNER JOIN DELETED d ON i.[Id] = d.[Id]
		WHERE 
		((i.[Approve] <> d.[Approve]) OR
		(i.[Revoke] <> d.[Revoke]) OR
		(ISNULL(i.[Comment], '') <> ISNULL(d.[Comment], '')) OR
		(ISNULL(i.[EffectiveDate], CONVERT(date, getdate())) <> ISNULL(d.[EffectiveDate], CONVERT(date, getdate()))))


	INSERT INTO [dbo].[DI_M_AttestationSnapshotAction_History]
           (  [Version]
			  ,[AttestationSnapshotActionId]
			  ,[UserId]
			  ,[TaskId]
			  ,[Approve]
			  ,[Revoke]
			  ,[Comment]
			  ,[EffectiveDate]
			  ,[ReviewerId]
			  ,[LastModifiedBy]
			  ,[LastModifiedDate])

	SELECT (select (ISNULL(Max([version]), 0) + 1) from DI_M_AttestationSnapshotAction_History where AttestationSnapshotActionid = i.[Id])
		  ,i.[Id]
		  ,(select UserId from DI_M_AttestationSnapshot where Id = i.[AttestationSnapshotId])
		  ,i.[TaskId]
		  ,i.[Approve]
		  ,i.[Revoke]
		  ,REPLACE(ISNULL(i.[Comment], ''), ISNULL(d.[Comment], ''), '')
		  ,i.[EffectiveDate]
		  ,i.[ReviewerId]
		  ,i.[LastModifiedBy]
		  ,i.[LastModifiedDate]
		FROM INSERTED i
		JOIN DELETED d ON i.[Id] = d.[Id]
		WHERE 
		(((i.[Approve] <> d.[Approve]) OR
		(i.[Revoke] <> d.[Revoke]) OR
		(ISNULL(i.[Comment], '') <> ISNULL(d.[Comment], '')) OR		
		(ISNULL(i.[TaskId], 0) <> ISNULL(d.[TaskId], 0)) OR
		(ISNULL(i.[EffectiveDate], CONVERT(date, getdate())) <> ISNULL(d.[EffectiveDate], CONVERT(date, getdate()))))
		AND
		(NOT EXISTS (SELECT 1 FROM [DI_M_AttestationSnapshotAction_History] WHERE TaskId = I.TaskId AND AttestationsnapshotActionId = i.Id)))
		
END

GO

DROP TRIGGER [dbo].[DI_M_AttestationSnapshotAction_Trigger]

GO