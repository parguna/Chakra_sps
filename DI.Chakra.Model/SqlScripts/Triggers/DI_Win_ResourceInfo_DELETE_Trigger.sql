CREATE   TRIGGER [dbo].[DI_Win_ResourceInfo_DELETE_Trigger] ON [dbo].[DI_Win_ResourceInfo]  AFTER DELETE
AS
BEGIN
SET NOCOUNT ON;

      DECLARE @EndPointSystemId  int
      DECLARE @HistoryId Int
     
      SELECT  @EndPointSystemId =EndPointSystemId FROM DELETED
      SET @HistoryId=0
      INSERT INTO [dbo].[DI_Win_ResourceInfoHistory]
					([ResouceID]
					,[HistoryId]
					,[TenantId]
					,[EndPointSystemId]
					,[ParentID]
					,[Name]
					,[ResourceCompletePath]
					,[ResourceType]
					,[Resource_CreatedDate]
					,[Resource_LastAccessedDate]
					,[Resource_LasteWriteDate]
					,[IsShare]
					,[Level]
					,[CreatedBy]
					,[CreatedDate]
					,[LastModifiedBy]
					,[LastModifiedDate]
					,[Version]
					,[RecordStatus])
     SELECT    [ResouceID]
					,@HistoryId
					,[TenantId]
					,[EndPointSystemId]
					,[ParentID]
					,[Name]
					,[ResourceCompletePath]
					,[ResourceType]
					,[Resource_CreatedDate]
					,[Resource_LastAccessedDate]
					,[Resource_LasteWriteDate]
					,[IsShare]
					,[Level]
					,[CreatedBy]
					,[CreatedDate]
					,[LastModifiedBy]
					,[LastModifiedDate]
					,[Version]
			,'DELETED'
        FROM deleted
END