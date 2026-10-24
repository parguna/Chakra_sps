
CREATE TRIGGER [dbo].[DI_M_AttestationReview_EndpointSystem_L3Detail_Trigger] ON [dbo].[DI_M_AttestationReview_EndpointSystem_L3Detail]
AFTER INSERT, UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO [dbo].[DI_M_AttestationReview_EndpointSystem_L3DetailHistory]
           ([L3DetailId]
		  ,[L2DetailId]
		  ,[AccessType]
		  ,[AccessName]
		  ,[ResourceType]
		  ,[ResourceCompletePath]
		  ,[Approve]
		  ,[Revoke]
		  ,[Hidden]
		  ,[BusinessOwner]
		  ,[Comments]
		  ,[IsChanged]
		  ,[PreviousAccess]
		  ,[LastModifiedBy]
		  ,[LastModifiedDate])
	SELECT [L3DetailId]
		  ,[L2DetailId]
		  ,[AccessType]
		  ,[AccessName]
		  ,[ResourceType]
		  ,[ResourceCompletePath]
		  ,[Approve]
		  ,[Revoke]
		  ,[Hidden]
		  ,[BusinessOwner]
		  ,[Comments]
		  ,[IsChanged]
		  ,[PreviousAccess]
		  ,[LastModifiedBy]
		  ,[LastModifiedDate]
		FROM INSERTED     
END
