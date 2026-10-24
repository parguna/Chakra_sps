
CREATE TRIGGER [dbo].[DI_M_AttestationReview_EndpointSystem_L2Detail_Trigger] ON [dbo].[DI_M_AttestationReview_EndpointSystem_L2Detail]
AFTER INSERT, UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO [dbo].[DI_M_AttestationReview_EndpointSystem_L2DetailHistory]
           ([L2DetailId]
		  ,[ReviewEndpointId]
		  ,[AccessType]
		  ,[AccessName]
		  ,[ResourceType]
		  ,[ResourceCompletePath]
		  ,[Approve]
		  ,[Revoke]
		  ,[BusinessOwner]
		  ,[Comments]
		  ,[IsChanged]
		  ,[PreviousAccess]
		  ,[LastModifiedBy]
		  ,[LastModifiedDate])

	SELECT [L2DetailId]
		  ,[ReviewEndpointId]
		  ,[AccessType]
		  ,[AccessName]
		  ,[ResourceType]
		  ,[ResourceCompletePath]
		  ,[Approve]
		  ,[Revoke]
		  ,[BusinessOwner]
		  ,[Comments]
		  ,[IsChanged]
		  ,[PreviousAccess]
		  ,[LastModifiedBy]
		  ,[LastModifiedDate]
		FROM INSERTED      
END