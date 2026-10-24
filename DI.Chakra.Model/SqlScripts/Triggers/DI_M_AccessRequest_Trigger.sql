
CREATE TRIGGER [dbo].[DI_M_AccessRequest_Trigger] ON [dbo].[DI_M_AccessRequest]
AFTER INSERT, UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @action as varchar(20);
	SET @Action = (CASE WHEN EXISTS(SELECT * FROM INSERTED)
							AND EXISTS(SELECT * FROM DELETED)
						THEN 'Updated'  -- Set Action to Updated.
						WHEN EXISTS(SELECT * FROM INSERTED)
						THEN 'Inserted'  -- Set Action to Insert.
						ELSE NULL -- Skip. It may have been a "failed delete".   
						END)

	IF(@action = 'Updated')
	BEGIN
	
		DECLARE @PrevUser VARCHAR(255)
		DECLARE @CurrUser VARCHAR(255)

		SELECT @PrevUser = ISNULL(LastModifiedBy, '') FROM deleted
		SELECT @CurrUser = ISNULL(LastModifiedBy, '') FROM inserted

		IF(@PrevUser <> @CurrUser)
		BEGIN
			INSERT INTO [dbo].[DI_M_AccessRequestHistory]
				([AccessRequestId]
				,[TenantId]
				,[RequesterId]
				,[UserId]
				,[Description]
				,[ProvisioningRoleId]
				,[RequestStatus]
				,[ProgressStatus]
				,[RequestSource]
				,[ActivationDate]
				,[DeactivationDate]
				,[ExecutionTime]
				,[Remarks]
				,[ApproverType]
				,[IsRequestedToRemove]
				,[IsNewUser]
				,[NewUserId]
				,[CreatedBy]
				,[CreatedDate]
				,[LastModifiedBy]
				,[LastModifiedDate]
				,[IsCurrentLevelApproved]
				,[TransactionId]
				,[Comments])
			SELECT
				[AccessRequestId]
				,[TenantId]
				,[RequesterId]
				,[UserId]
				,[Description]
				,[ProvisioningRoleId]
				,[RequestStatus]
				,[ProgressStatus]
				,[RequestSource]
				,[ActivationDate]
				,[DeactivationDate]
				,[ExecutionTime]
				,[Remarks]
				,[ApproverType]
				,[IsRequestedToRemove]
				,[IsNewUser]
				,[NewUserId]
				,[CreatedBy]
				,[CreatedDate]
				,[LastModifiedBy]
				,[LastModifiedDate]
				,[IsCurrentLevelApproved]
				,[TransactionId]
				,[Comments]
			FROM INSERTED
		END  
	END  
	ELSE
	BEGIN
		INSERT INTO [dbo].[DI_M_AccessRequestHistory]
			([AccessRequestId]
			,[TenantId]
			,[RequesterId]
			,[UserId]
			,[Description]
			,[ProvisioningRoleId]
			,[RequestStatus]
			,[ProgressStatus]
			,[ActivationDate]
			,[DeactivationDate]
			,[ExecutionTime]
			,[Remarks]
			,[ApproverType]
			,[IsRequestedToRemove]
			,[IsNewUser]
			,[NewUserId]
			,[CreatedBy]
			,[CreatedDate]
			,[LastModifiedBy]
			,[LastModifiedDate]
			,[IsCurrentLevelApproved]
			,[TransactionId]
			,[Comments])
		SELECT
			[AccessRequestId]
			,[TenantId]
			,[RequesterId]
			,[UserId]
			,[Description]
			,[ProvisioningRoleId]
			,[RequestStatus]
			,[ProgressStatus]
			,[ActivationDate]
			,[DeactivationDate]
			,[ExecutionTime]
			,[Remarks]
			,[ApproverType]
			,[IsRequestedToRemove]
			,[IsNewUser]
			,[NewUserId]
			,[CreatedBy]
			,[CreatedDate]
			,[LastModifiedBy]
			,[LastModifiedDate]
			,[IsCurrentLevelApproved]
			,[TransactionId]
			,[Comments]
		FROM INSERTED
	END    
END