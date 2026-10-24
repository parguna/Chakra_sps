Create  TRIGGER [dbo].[DI_LNS_Users_Trigger] ON [dbo].[DI_LNS_Users]  AFTER INSERT, UPDATE
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
		INSERT INTO @TrackingCoumns SELECT ScenarioID,ColumnName from DI_M_Trigger WHERE ScenarioID=13
		
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
               
				INSERT INTO [dbo].[DI_LNS_UsersHistory]
					(UserId,HistoryId,TenantId,EndPointSystemId,Name,LastName,ListNameFullName,FullName,ListName,Department,CompanyName,Owner,JobTitle,PasswordChangedDate,Members,MailServer,MailFile,MailDomain,MailAddress,ShortName,Location,EncryptIncomingMail,CalendarDomain,NetUserName,HTTPPassword,CertThumbprint,InternetAddress,NewMailServer,NewMailFile,AltFullName,MessageStorage,ServerName,AltFullNameLanguage,QuickPlace,SametimeServer,Type,MailSystem,AdminpOldWebName,HTTPPasswordChangeDate,HTTPPasswordForceChange,Policy,LTPA_UsrNm,DB2UserName,OfficePhoneNumber,OfficeFAXPhoneNumber,AvailableForDirSync,MailExternalAccess,MailChargeable,Comment,IDMPolicy,NRIC,PwdResetCnt,CryptoCap,DIUserId,IsServiceAccount,IsSharedAccount,IfUnmappedMapTo,MappingType,IACMLastPasswordChange,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate,Version,IsUserAccount,LockScenario,IsLocked
					,[RecordStatus])
				SELECT UserId,@HistoryId,TenantId,EndPointSystemId,Name,LastName,ListNameFullName,FullName,ListName,Department,CompanyName,Owner,JobTitle,PasswordChangedDate,Members,MailServer,MailFile,MailDomain,MailAddress,ShortName,Location,EncryptIncomingMail,CalendarDomain,NetUserName,HTTPPassword,CertThumbprint,InternetAddress,NewMailServer,NewMailFile,AltFullName,MessageStorage,ServerName,AltFullNameLanguage,QuickPlace,SametimeServer,Type,MailSystem,AdminpOldWebName,HTTPPasswordChangeDate,HTTPPasswordForceChange,Policy,LTPA_UsrNm,DB2UserName,OfficePhoneNumber,OfficeFAXPhoneNumber,AvailableForDirSync,MailExternalAccess,MailChargeable,Comment,IDMPolicy,NRIC,PwdResetCnt,CryptoCap,DIUserId,IsServiceAccount,IsSharedAccount,IfUnmappedMapTo,MappingType,IACMLastPasswordChange,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate,Version,IsUserAccount,LockScenario,IsLocked
					,@Action
					FROM Inserted

					return

				END
					
			END
		END
	ELSE
		BEGIN
			
						  

     
			SELECT  @EndPointSystemId =EndPointSystemId FROM INSERTED
			SET @HistoryId=0
               
			INSERT INTO [dbo].[DI_LNS_UsersHistory]
				(UserId,HistoryId,TenantId,EndPointSystemId,Name,LastName,ListNameFullName,FullName,ListName,Department,CompanyName,Owner,JobTitle,PasswordChangedDate,Members,MailServer,MailFile,MailDomain,MailAddress,ShortName,Location,EncryptIncomingMail,CalendarDomain,NetUserName,HTTPPassword,CertThumbprint,InternetAddress,NewMailServer,NewMailFile,AltFullName,MessageStorage,ServerName,AltFullNameLanguage,QuickPlace,SametimeServer,Type,MailSystem,AdminpOldWebName,HTTPPasswordChangeDate,HTTPPasswordForceChange,Policy,LTPA_UsrNm,DB2UserName,OfficePhoneNumber,OfficeFAXPhoneNumber,AvailableForDirSync,MailExternalAccess,MailChargeable,Comment,IDMPolicy,NRIC,PwdResetCnt,CryptoCap,DIUserId,IsServiceAccount,IsSharedAccount,IfUnmappedMapTo,MappingType,IACMLastPasswordChange,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate,Version,IsUserAccount,LockScenario,IsLocked
				,[RecordStatus])
			SELECT UserId,@HistoryId,TenantId,EndPointSystemId,Name,LastName,ListNameFullName,FullName,ListName,Department,CompanyName,Owner,JobTitle,PasswordChangedDate,Members,MailServer,MailFile,MailDomain,MailAddress,ShortName,Location,EncryptIncomingMail,CalendarDomain,NetUserName,HTTPPassword,CertThumbprint,InternetAddress,NewMailServer,NewMailFile,AltFullName,MessageStorage,ServerName,AltFullNameLanguage,QuickPlace,SametimeServer,Type,MailSystem,AdminpOldWebName,HTTPPasswordChangeDate,HTTPPasswordForceChange,Policy,LTPA_UsrNm,DB2UserName,OfficePhoneNumber,OfficeFAXPhoneNumber,AvailableForDirSync,MailExternalAccess,MailChargeable,Comment,IDMPolicy,NRIC,PwdResetCnt,CryptoCap,DIUserId,IsServiceAccount,IsSharedAccount,IfUnmappedMapTo,MappingType,IACMLastPasswordChange,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate,Version,IsUserAccount,LockScenario,IsLocked		
				,@Action
				FROM Inserted
		END
END