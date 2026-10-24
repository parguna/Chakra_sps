CREATE   TRIGGER [dbo].[DI_LNS_Groups_DELETE_Trigger] ON [dbo].[DI_LNS_Groups]  AFTER DELETE
AS
BEGIN
SET NOCOUNT ON;

      DECLARE @EndPointSystemId  int
      DECLARE @HistoryId Int
     
      SELECT  @EndPointSystemId =EndPointSystemId FROM DELETED
      SET @HistoryId=0
               
      INSERT INTO [dbo].[DI_LNS_GroupsHistory]
           	([Id]
			,[HistoryId]
			,[TenantId]
			,[EndPointSystemId]
			,[Group]
			,[Category]
			,[Description]
			,[CreatedBy]
			,[CreatedDate]
			,[LastModifiedBy]
			,[LastModifiedDate]
			,[Version]
			,[RecordStatus])
     SELECT [Id]
			,@HistoryId
			,[TenantId]
			,[EndPointSystemId]
			,[Group]
			,[Category]
			,[Description]
			,[CreatedBy]
			,[CreatedDate]
			,[LastModifiedBy]
			,[LastModifiedDate]
			,[Version]
			,'Deleted'
        FROM deleted

END
