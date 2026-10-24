CREATE  TRIGGER [dbo].[DI_SAPEP_RoleInfo_DELETE_Trigger] ON [dbo].[DI_SAPEP_RoleInfo]   AFTER DELETE
AS
BEGIN
SET NOCOUNT ON;

      DECLARE @EndPointSystemId  int
      DECLARE @HistoryId Int
	 
     
      SELECT  @EndPointSystemId =EndPointSystemId FROM DELETED
      SET @HistoryId=0
               
      INSERT INTO [dbo].[DI_SAPEP_RoleInfoHistory]
			([Id]
			,[HistoryId]
			,
			,[EndPointSystemId]
			,[RoleId]
			,[RoleName]
			,[RoleDescription]
			,[Extar1]
			,[CreatedBy]
			,[CreatedDate]
			,[LastModifiedBy]
			,[LastModifiedDate]
			,[Version]
		    ,[RecordStatus])
     SELECT [Id]
            ,@HistoryId
			,
			,[EndPointSystemId]
			,[RoleId]
			,[RoleName]
			,[RoleDescription]
			,[Extar1]
			,[CreatedBy]
			,[CreatedDate]
			,[LastModifiedBy]
			,[LastModifiedDate]
			,[Version]
			,'DELETED'
        FROM deleted

END