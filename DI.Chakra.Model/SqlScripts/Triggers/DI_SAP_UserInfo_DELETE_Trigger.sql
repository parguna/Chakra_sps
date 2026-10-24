CREATE   TRIGGER [dbo].[DI_SAP_UserInfo_DELETE_Trigger] ON [dbo].[DI_SAP_UserInfo]  AFTER DELETE
AS
BEGIN
SET NOCOUNT ON;

      DECLARE @EndPointSystemId  int
      DECLARE @HistoryId Int
     
      SELECT  @EndPointSystemId =EndPointSystemId FROM DELETED
      SET @HistoryId=0
             
			 
	  INSERT INTO [dbo].[DI_SAP_USERHISTORYINFO]
					([UserId]
					,[HistoryId]
					,[TenantId]
					,[EndPointSystemId]
					,[Firstname]
					,[Lastname]
					,[Initials]
					,[Department]
					,[E_Mail]
					,[Pers_No]
					,[Tel1_Numbr]
					,[Function0]
					,[Title]
					,[City]
					,[Namcountry]
					,[Pboxcit_No]
					,[Postl_Cod1]
					,[Street]
					,[Start_Menu]
					,[ActivityGroup]
					,[LastLoginTime]
					,[UserLogonName]
					,[AccntLockNoPwd]
					,[AccntLocDurPwd]
					,[LOCKEDBYINTRUDER]
					,[EMPLOYEEID]
					,[EMPLOYEETYPE]
					,[PASSWORDEXPTIME]
					,[LOGINDISABLED]
					,[IsActive]
					,[DomainName]
					,[TargetSysId]
					,[SpecialAccount]
					,[CreatedDate]
					,[LastModifiedDate]
					,[CreatedBy]
					,[LastModifiedBy]
					,[Version]
					,[RecordStatus])

				SELECT [UserId]
					,@HistoryId
					,[TenantId]
					,[EndPointSystemId]
					,[Firstname]
					,[Lastname]
					,[Initials]
					,[Department]
					,[E_Mail]
					,[Pers_No]
					,[Tel1_Numbr]
					,[Function0]
					,[Title]
					,[City]
					,[Namcountry]
					,[Pboxcit_No]
					,[Postl_Cod1]
					,[Street]
					,[Start_Menu]
					,[ActivityGroup]
					,[LastLoginTime]
					,[UserLogonName]
					,[AccntLockNoPwd]
					,[AccntLocDurPwd]
					,[LOCKEDBYINTRUDER]
					,[EMPLOYEEID]
					,[EMPLOYEETYPE]
					,[PASSWORDEXPTIME]
					,[LOGINDISABLED]
					,[IsActive]
					,[DomainName]
					,[TargetSysId]
					,[SpecialAccount]
					,[CreatedDate]
					,[LastModifiedDate]
					,[CreatedBy]
					,[LastModifiedBy]
					 ,[Version]
					,'Deleted'
					FROM deleted
			   
     
END
