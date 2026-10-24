CREATE   TRIGGER [dbo].[DI_ADS_GroupInfo_DELETE_Trigger] ON [dbo].[DI_ADS_GroupInfo]  AFTER DELETE
AS
BEGIN
SET NOCOUNT ON;

      DECLARE @EndPointSystemId  int
      DECLARE @HistoryId Int
     
      SELECT  @EndPointSystemId =EndPointSystemId FROM DELETED
      SET @HistoryId=0
               
     INSERT INTO [dbo].[DI_ADS_GroupInfoHistory]
					([GroupId]
					,[HistoryId]
					,[TenantId]
					,[EndPointSystemId]
					,[cn]
					,[description]
					,[member]
					,[distinguishedName]
					,[instanceType]
					,[whenCreated]
					,[whenChanged]
					,[uSNCreated]
					,[uSNChanged]
					,[name]
					,[objectGUID]
					,[objectSid]
					,[sAMAccountName]
					,[sAMAccountType]
					,[groupType]
					,[objectCategory]
					,[nTSecurityDescriptor]
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
					,[cn]
					,[description]
					,[member]
					,[distinguishedName]
					,[instanceType]
					,[whenCreated]
					,[whenChanged]
					,[uSNCreated]
					,[uSNChanged]
					,[name]
					,[objectGUID]
					,[objectSid]
					,[sAMAccountName]
					,[sAMAccountType]
					,[groupType]
					,[objectCategory]
					,[nTSecurityDescriptor]
					,[CreatedBy]
					,[CreatedDate]
					,[LastModifiedBy]
					,[LastModifiedDate]
					,[Version]
					,'DELETED'
					FROM Inserted

END


