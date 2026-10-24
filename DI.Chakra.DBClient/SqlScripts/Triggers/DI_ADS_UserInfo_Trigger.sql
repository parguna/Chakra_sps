Create  TRIGGER [dbo].[DI_ADS_UserInfo_Trigger] ON [dbo].[DI_ADS_UserInfo]  AFTER INSERT, UPDATE
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @EndPointSystemId  int
	DECLARE @HistoryId Int
	DECLARE @action as varchar(20);
	SET @Action = (CASE WHEN EXISTS(SELECT * FROM INSERTED)
							AND EXISTS(SELECT * FROM DELETED)
						THEN 'Updated'  -- Set Action to Updated.
						WHEN EXISTS(SELECT * FROM INSERTED)
						THEN 'Inserted'  -- Set Action to Insert.
						ELSE NULL -- Skip. It may have been a "failed delete".   
						END)

	IF(@action = 'Updated')
	BEGIN
		DECLARE @RowsToProcess int
		DECLARE @CurrentRow int
		DECLARE @TrackingCoumns As Table(RowId int NOT NULL primary key identity(1,1), ScenarioID int ,ColumnName nvarchar(250))
		INSERT INTO @TrackingCoumns SELECT ScenarioID,ColumnName from DI_M_Trigger WHERE ScenarioID=2

		SET @RowsToProcess=@@ROWCOUNT
		SELECT * INTO #t FROM INSERTED;
		SELECT * INTO #d FROM DELETED;

		

		SET @CurrentRow=0
		WHILE @CurrentRow<@RowsToProcess
		BEGIN
			SET @CurrentRow=@CurrentRow+1
			DECLARE @COLUMNNAME nvarchar(250)
			DECLARE @NEWVALUETABLE table(value nvarchar(1000))
			DECLARE @OLDVALUETABLE table(value nvarchar(1000))

			SELECT @COLUMNNAME=ColumnName
			FROM @TrackingCoumns
			WHERE RowID=@CurrentRow

			INSERT INTO @NEWVALUETABLE EXEC('SELECT ' + @COLUMNNAME + ' FROM #t')
			INSERT INTO @OLDVALUETABLE EXEC('SELECT ' + @COLUMNNAME + ' FROM #d')

			DECLARE @NEWVALUE nvarchar(1000)
			SELECT @NEWVALUE = value from @NEWVALUETABLE
			DECLARE @OLDVALUE nvarchar(1000)
			SELECT @OLDVALUE = value from @OLDVALUETABLE

			IF(@NEWVALUE <> @OLDVALUE)
			BEGIN
				--Insert history
				SET @CurrentRow=@RowsToProcess
				SELECT  @EndPointSystemId =EndPointSystemId FROM INSERTED
				SET @HistoryId=0
				--SELECT @HistoryId =(SELECT  HistoryId from DI_M_DISCOVERYPROCESS where EndPointSystemId=@EndPointSystemId)
               
				INSERT INTO [dbo].[DI_ADS_UserInfoHistory]
					([UserId]
					,[HistoryId]
					,
					,[EndPointSystemId]
					,[cn]
					,[sn]
					,[st]
					,[title]
					,[description]
					,[postalAddress]
					,[postalCode]
					,[postOfficeBox]
					,[physicalDeliveryOfficeName]
					,[telephoneNumber]
					,[facsimileTelephoneNumber]
					,[givenName]
					,[initials]
					,[distinguishedName]
					,[instanceType]
					,[whenCreated]
					,[whenChanged]
					,[displayName]
					,[uSNCreated]
					,[info]
					,[uSNChanged]
					,[co]
					,[department]
					,[company]
					,[manager]
					,[streetAddress]
					,[wWWHomePage]
					,[name]
					,[objectGUID]
					,[userAccountControl]
					,[IsAccountDisabled]
					,[IsAccountLocked]
					,[IsPasswordRequired]
					,[PasswordExpirationDate]
					,[badPwdCount]
					,[codePage]
					,[countryCode]
					,[badPasswordTime]
					,[lastLogoff]
					,[lastLogon]
					,[pwdLastSet]
					,[primaryGroupID]
					,[objectSid]
					,[accountExpires]
					,[logonCount]
					,[sAMAccountName]
					,[sAMAccountType]
					,[managedObjects]
					,[ipPhone]
					,[objectCategory]
					,[mail]
					,[homePhone]
					,[mobile]
					,[pager]
					,[ExUserId]
					,[nTSecurityDescriptor]
					,[CreatedBy]
					,[CreatedDate]
					,[LastModifiedBy]
					,[LastModifiedDate]
					,[MappingType]
					,[DIUserId]
					,[Version]
					,[RecordStatus]
					,businessCategory
					,division
					,homeDrive
					,homeDirectory
					,IsUserAccount
					,IsPrimaryAccount
					,IsHomeFolderPermissionSetFailed
					,LockScenario)
				SELECT [UserId]
					,@HistoryId
					,
					,[EndPointSystemId]
					,[cn]
					,[sn]
					,[st]
					,[title]
					,[description]
					,[postalAddress]
					,[postalCode]
					,[postOfficeBox]
					,[physicalDeliveryOfficeName]
					,[telephoneNumber]
					,[facsimileTelephoneNumber]
					,[givenName]
					,[initials]
					,[distinguishedName]
					,[instanceType]
					,[whenCreated]
					,[whenChanged]
					,[displayName]
					,[uSNCreated]
					,[info]
					,[uSNChanged]
					,[co]
					,[department]
					,[company]
					,[manager]
					,[streetAddress]
					,[wWWHomePage]
					,[name]
					,[objectGUID]
					,[userAccountControl]
					,[IsAccountDisabled]
					,[IsLocked]
					,[IsPasswordRequired]
					,[PasswordExpirationDate]
					,[badPwdCount]
					,[codePage]
					,[countryCode]
					,[badPasswordTime]
					,[lastLogoff]
					,[lastLogon]
					,[pwdLastSet]
					,[primaryGroupID]
					,[objectSid]
					,[accountExpires]
					,[logonCount]
					,[sAMAccountName]
					,[sAMAccountType]
					,[managedObjects]
					,[ipPhone]
					,[objectCategory]
					,[mail]
					,[homePhone]
					,[mobile]
					,[pager]
					,[ExUserId]
					,[nTSecurityDescriptor]
					,[CreatedBy]
					,[CreatedDate]
					,[LastModifiedBy]
					,[LastModifiedDate]
					,[MappingType]
					,[DIUserId]
					,[Version]
					,@Action
					,businessCategory
					,division
					,homeDrive
					,homeDirectory
					,IsUserAccount
					,IsPrimaryAccount
					,IsHomeFolderPermissionSetFailed
					,LockScenario
					FROM Inserted

					return

				END
					
			END
		END
	ELSE
		BEGIN
			
						  

     
				SELECT  @EndPointSystemId =EndPointSystemId FROM INSERTED
				SET @HistoryId=0
				--SELECT @HistoryId =(SELECT  HistoryId from DI_M_DISCOVERYPROCESS where EndPointSystemId=@EndPointSystemId)
               
				INSERT INTO [dbo].[DI_ADS_UserInfoHistory]
					([UserId]
					,[HistoryId]
					,
					,[EndPointSystemId]
					,[cn]
					,[sn]
					,[ou]
					,[st]
					,[title]
					,[description]
					,[postalAddress]
					,[postalCode]
					,[postOfficeBox]
					,[physicalDeliveryOfficeName]
					,[telephoneNumber]
					,[facsimileTelephoneNumber]
					,[givenName]
					,[initials]
					,[distinguishedName]
					,[instanceType]
					,[whenCreated]
					,[whenChanged]
					,[displayName]
					,[uSNCreated]
					,[info]
					,[uSNChanged]
					,[co]
					,[department]
					,[company]
					,[manager]
					,[streetAddress]
					,[wWWHomePage]
					,[name]
					,[objectGUID]
					,[userAccountControl]
					,[IsAccountDisabled]
					,[IsAccountLocked]
					,[IsPasswordRequired]
					,[PasswordExpirationDate]
					,[badPwdCount]
					,[codePage]
					,[countryCode]
					,[badPasswordTime]
					,[lastLogoff]
					,[lastLogon]
					,[pwdLastSet]
					,[primaryGroupID]
					,[objectSid]
					,[accountExpires]
					,[logonCount]
					,[sAMAccountName]
					,[sAMAccountType]
					,[managedObjects]
					,[ipPhone]
					,[objectCategory]
					,[mail]
					,[homePhone]
					,[mobile]
					,[pager]
					,[nTSecurityDescriptor]
					,[ExUserId]
					,[CreatedBy]
					,[CreatedDate]
					,[LastModifiedBy]
					,[LastModifiedDate]
					,[MappingType]
					,[Version]
					,[RecordStatus])
				SELECT [UserId]
					,@HistoryId
					,
					,[EndPointSystemId]
					,[cn]
					,[sn]
					,[ou]
					,[st]
					,[title]
					,[description]
					,[postalAddress]
					,[postalCode]
					,[postOfficeBox]
					,[physicalDeliveryOfficeName]
					,[telephoneNumber]
					,[facsimileTelephoneNumber]
					,[givenName]
					,[initials]
					,[distinguishedName]
					,[instanceType]
					,[whenCreated]
					,[whenChanged]
					,[displayName]
					,[uSNCreated]
					,[info]
					,[uSNChanged]
					,[co]
					,[department]
					,[company]
					,[manager]
					,[streetAddress]
					,[wWWHomePage]
					,[name]
					,[objectGUID]
					,[userAccountControl]
					,[IsAccountDisabled]
					,[IsLocked]
					,[IsPasswordRequired]
					,[PasswordExpirationDate]
					,[badPwdCount]
					,[codePage]
					,[countryCode]
					,[badPasswordTime]
					,[lastLogoff]
					,[lastLogon]
					,[pwdLastSet]
					,[primaryGroupID]
					,[objectSid]
					,[accountExpires]
					,[logonCount]
					,[sAMAccountName]
					,[sAMAccountType]
					,[managedObjects]
					,[ipPhone]
					,[objectCategory]
					,[mail]
					,[homePhone]
					,[mobile]
					,[pager]
					,[nTSecurityDescriptor]
					,[ExUserId]
					,[CreatedBy]
					,[CreatedDate]
					,[LastModifiedBy]
					,[LastModifiedDate]
					,[MappingType]
					,[Version]
					,@Action
					FROM Inserted
		END
END
