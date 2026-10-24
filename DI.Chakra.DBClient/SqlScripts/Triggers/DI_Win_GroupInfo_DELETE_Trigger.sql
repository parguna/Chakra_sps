CREATE   TRIGGER [dbo].[DI_Win_GroupInfo_DELETE_Trigger] ON [dbo].[DI_Win_GroupInfo]  AFTER DELETE
AS
BEGIN
SET NOCOUNT ON;

      DECLARE @EndPointSystemId  int
      DECLARE @HistoryId Int
     
      SELECT  @EndPointSystemId =EndPointSystemId FROM DELETED
      SET @HistoryId=0
               
      INSERT INTO [dbo].[DI_Win_GroupInfoHistory]
					([GroupId]
					,[HistoryId]
					,
					,[EndPointSystemId]
					,[groupType]
					,[Name]
					,[Description]
					,[objectSid]
					,[CreatedBy]
					,[CreatedDate]
					,[LastModifiedBy]
					,[LastModifiedDate]
					,[Version]
					,[RecordStatus])
     SELECT  [GroupId]
					,@HistoryId
					,
					,[EndPointSystemId]
					,[groupType]
					,[Name]
					,[Description]
					,[objectSid]
					,[CreatedBy]
					,[CreatedDate]
					,[LastModifiedBy]
					,[LastModifiedDate]
					,[Version]
		,'DELETED'
        FROM deleted
END