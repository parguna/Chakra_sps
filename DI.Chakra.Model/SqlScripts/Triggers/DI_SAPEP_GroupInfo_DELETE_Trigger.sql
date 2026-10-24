
CREATE   TRIGGER [dbo].[DI_SAPEP_GroupInfo_DELETE_Trigger] ON [dbo].[DI_SAPEP_GroupInfo]  AFTER DELETE
AS
BEGIN
SET NOCOUNT ON;

      DECLARE @EndPointSystemId  int
      DECLARE @HistoryId Int
     
      SELECT  @EndPointSystemId =EndPointSystemId FROM DELETED
     SET @HistoryId=0
               
     INSERT INTO [dbo].[DI_SAPEP_GroupInfoHistory]
					([Id]
					,[HistoryId]
					,[TenantId]
					,[EndPointSystemId]
					,[GroupId]
					,[GroupName]
					,[GroupDescription]
					,[Extar1]
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
					,[GroupId]
					,[GroupName]
					,[GroupDescription]
					,[Extar1]
					,[CreatedBy]
					,[CreatedDate]
					,[LastModifiedBy]
					,[LastModifiedDate]
					,[Version]
					,'DELETED'
					FROM deleted

END


