
CREATE TRIGGER [dbo].[DI_M_AttestationReview_EndpointSystem_Trigger] ON [dbo].[DI_M_AttestationReview_EndpointSystem]
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
			INSERT INTO [dbo].[DI_M_AttestationReview_EndpointSystemHistory]
					   ([ReviewEndpointId]
					  ,[EndpointUserId]
					  ,[EndpointSystemId]
					  ,[HostName]
					  ,[EndpointUserName]
					  ,[EndpointSystemTypeId]
					  ,[EndpointSystemTypeName]
					  ,[Approve]
					  ,[Revoke]
					  ,[Comments]
					  ,[IsCurrentLevelApproved]
					  ,[IsChanged]
					  ,[PreviousAccess]
					  ,[LastModifiedBy]
					  ,[LastModifiedDate])
				SELECT [ReviewEndpointId]
					  ,[EndpointUserId]
					  ,[EndpointSystemId]
					  ,[HostName]
					  ,[EndpointUserName]
					  ,[EndpointSystemTypeId]
					  ,[EndpointSystemTypeName]
					  ,[Approve]
					  ,[Revoke]
					  ,[Comments]
					  ,[IsCurrentLevelApproved]
					  ,[IsChanged]
					  ,[PreviousAccess]
					  ,[LastModifiedBy]
					  ,[LastModifiedDate]
				FROM INSERTED
		END
	END
	ELSE
	BEGIN
		INSERT INTO [dbo].[DI_M_AttestationReview_EndpointSystemHistory]
			   ([ReviewEndpointId]
			  ,[EndpointUserId]
			  ,[EndpointSystemId]
			  ,[HostName]
			  ,[EndpointUserName]
			  ,[EndpointSystemTypeId]
			  ,[EndpointSystemTypeName]
			  ,[Approve]
			  ,[Revoke]
			  ,[Comments]
			  ,[IsCurrentLevelApproved]
			  ,[IsChanged]
			  ,[PreviousAccess]
			  ,[LastModifiedBy]
			  ,[LastModifiedDate])
		SELECT [ReviewEndpointId]
			  ,[EndpointUserId]
			  ,[EndpointSystemId]
			  ,[HostName]
			  ,[EndpointUserName]
			  ,[EndpointSystemTypeId]
			  ,[EndpointSystemTypeName]
			  ,[Approve]
			  ,[Revoke]
			  ,[Comments]
			  ,[IsCurrentLevelApproved]
			  ,[IsChanged]
			  ,[PreviousAccess]
			  ,[LastModifiedBy]
			  ,[LastModifiedDate]
		FROM INSERTED  
	END
	   
END