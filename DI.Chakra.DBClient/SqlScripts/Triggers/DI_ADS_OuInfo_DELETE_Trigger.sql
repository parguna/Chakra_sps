CREATE  TRIGGER [dbo].[DI_ADS_OuInfo_DELETE_Trigger] ON [dbo].[DI_ADS_OuInfo]   AFTER DELETE
AS
BEGIN
SET NOCOUNT ON;

      DECLARE @EndPointSystemId  int
      DECLARE @HistoryId Int
	 
     
      SELECT  @EndPointSystemId =EndPointSystemId FROM INSERTED
      SET @HistoryId=0
               
      INSERT INTO [dbo].[DI_ADS_OuInfoHistory]
           ([Ouid]
           ,[HistoryId]
           ,
           ,[EndPointSystemId]
           ,[ou]
           ,[description]
           ,[distinguishedName]
           ,[instanceType]
           ,[whenCreated]
           ,[whenChanged]
		   ,[uSNCreated]
		   ,[uSNChanged]
		   ,[showInAdvancedViewOnly]
		   ,[name]
		   ,[objectGUID]
		   ,[systemFlags]
		   ,[objectCategory]
		   ,[isCriticalSystemObject]
		   ,[gPLink]
		   ,[nTSecurityDescriptor]
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
           ,[ou]
           ,[description]
           ,[distinguishedName]
           ,[instanceType]
           ,[whenCreated]
           ,[whenChanged]
		   ,[uSNCreated]
		   ,[uSNChanged]
		   ,[showInAdvancedViewOnly]
		   ,[name]
		   ,[objectGUID]
		   ,[systemFlags]
		   ,[objectCategory]
		   ,[isCriticalSystemObject]
		   ,[gPLink]
		   ,[nTSecurityDescriptor]
		   ,[CreatedBy]
		   ,[CreatedDate]
		   ,[LastModifiedBy]
		   ,[LastModifiedDate]
			,[Version]
		   ,'DELETED'
        FROM deleted

END