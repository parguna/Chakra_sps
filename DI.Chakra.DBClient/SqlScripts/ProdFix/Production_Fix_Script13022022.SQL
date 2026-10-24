USE [V5_IDMS_SP]
GO

IF EXISTS(SELECT 1 FROM SYS.objects WHERE NAME='USP_SODCONFLICTING_REPORT')
BEGIN
	DROP PROCEDURE DBO.USP_SODCONFLICTING_REPORT
END
GO


/*
NAME		:	USP_SODCONFLICTING_REPORT
DESC		:	GENERATE THE CONFLICTING ROLES REPORT BY ACCOUNTNAME;	
EXEC USP_SODCONFLICTING_REPORT 10,'SPECIAL_SEC'

*/

CREATE PROC [dbo].[USP_SODCONFLICTING_REPORT]
(
@ENDPOINTID BIGINT,
@USERNAME NVARCHAR(512)='',
@DEBUG BIT = 0
)
AS
BEGIN

	SET NOCOUNT ON;
	DECLARE @AccountOwner BIGINT
	DECLARE @ACCOUNTRECONID BIGINT
	DECLARE @ENDPOINT_USERID BIGINT
	DECLARE @DISPLAYNAME nVARCHAR(MAX)=''
	DECLARE @ACCOUNTNAME nVARCHAR(512)=''
	DECLARE @TECHNICALROLE nVARCHAR(MAX)=''
	DECLARE @DESCRIPTION nVARCHAR(MAX)=''
	DECLARE @POSITION nVARCHAR(MAX)=''
	DECLARE @ACCOUNTTYPE TINYINT
	DECLARE @ENDPOINT_NAME nVARCHAR(1024)=''
	DECLARE @USER_NAME nVARCHAR(MAX)=''
	DECLARE @USERDISPLAYNAME	nVARCHAR(MAX)=''
	DECLARE @MODULE VARCHAR(52)=''
	DECLARE @APPTYPE TINYINT

	SELECT @APPTYPE= ApplicationType,@ENDPOINT_NAME = DisplayName FROM DI_M_EndpointSystem WHERE ID = @ENDPOINTID 
	
	IF @APPTYPE = 10
	BEGIN
			SET @MODULE = 'CRP (EBS-CRM)'
	END
	ELSE IF @APPTYPE = 9
	BEGIN
			SET @MODULE = 'ISP (EBS - ISU)'
	END
	ELSE IF @APPTYPE = 8
	BEGIN
			SET @MODULE = 'MCP (MSSL - CRM)'
	END
	ELSE IF @APPTYPE = 7
	BEGIN
			SET @MODULE = 'MSP (MSSL - ISU)'
	END

	IF (LEN(ISNULL(@MODULE,''))<= 0)
	BEGIN
			RETURN;
	END
	
	DECLARE @TOTALACCOUNT BIGINT=0

	CREATE TABLE #TEMPACCOUNTS(ID BIGINT IDENTITY(1,1),DISPLAYNAME NVARCHAR(MAX) NULL,ACCOUNTNAME NVARCHAR(512) NOT NULL,ACCOUNTRECONID BIGINT NOT NULL,ENDPOINTUSERID BIGINT,ACCOUNTTYPE TINYINT, [UserName] [nvarchar](max) null,
	[UserDisplayName] [nvarchar](max) null,UserId BIGINT NULL)

	IF LEN(ISNULL(@USERNAME,''))> 0
	BEGIN
			DELETE FROM [dbo].[DI_SAP_ConflictingRoleReport] WHERE EndPointSystemId = @ENDPOINTID AND AccountName=@USERNAME

			INSERT INTO #TEMPACCOUNTS(ACCOUNTRECONID,DISPLAYNAME,ACCOUNTNAME,ENDPOINTUSERID,ACCOUNTTYPE,[UserName],[UserDisplayName],UserId)
			SELECT DISTINCT A.ID,FULLNAME ,A.ACCOUNTNAME,A.EndpointUserId,AccountType,B.USERNAME,B.DisplayName,A.UserId  FROM DI_M_AccountReconcilation A INNER JOIN DI_M_EntitlementReconcilation E ON A.ID = E.AccountReconcilationId
			LEFT JOIN  DI_M_User B on A.UserId = B.ID
			WHERE A.EndpointSystemId = @ENDPOINTID  AND E.EndpointSystemId = @ENDPOINTID AND A.ISDELETED = 0 AND A.ISLOCKED = 0 AND A.AccountName=@USERNAME 
			AND E.IsDeleted = 0 AND E.EntitlementType IN(1,5)  AND A.EndpointUserId IN(SELECT ID FROM DI_SAP_UserInfo WHERE EndPointSystemId  = @ENDPOINTID AND UserLogonName=@USERNAME)
			AND A.AccountName=@USERNAME
			SET @TOTALACCOUNT = @@ROWCOUNT
	END
	ELSE
	BEGIN
			

			DELETE FROM [dbo].[DI_SAP_ConflictingRoleReport] WHERE EndPointSystemId = @ENDPOINTID

			INSERT INTO #TEMPACCOUNTS(ACCOUNTRECONID,DISPLAYNAME,ACCOUNTNAME,ENDPOINTUSERID,ACCOUNTTYPE,[UserName],[UserDisplayName],UserId)
			SELECT DISTINCT A.ID,FULLNAME ,A.ACCOUNTNAME,A.EndpointUserId,AccountType,B.USERNAME,B.DisplayName,A.UserId   FROM DI_M_AccountReconcilation A INNER JOIN DI_M_EntitlementReconcilation E ON A.ID = E.AccountReconcilationId
			INNER JOIN DI_SAP_UserInfo SAPU ON A.EndpointUserId = SAPU.ID
			LEFT JOIN  DI_M_User B on A.UserId = B.ID
			WHERE A.EndpointSystemId = @ENDPOINTID  AND E.EndpointSystemId = @ENDPOINTID AND A.ISDELETED = 0 AND A.ISLOCKED = 0 AND E.IsDeleted = 0
			AND E.EntitlementType IN(1,5) AND  A.AccountName = SAPU.UserLogonName AND SAPU.EndPointSystemId = @ENDPOINTID
			ORDER BY A.FULLNAME
			SET @TOTALACCOUNT = @@ROWCOUNT
			
	END

	IF @TOTALACCOUNT > 0
	BEGIN
			
			DECLARE @INCR BIGINT=1
			WHILE @TOTALACCOUNT >= @INCR
			BEGIN
				
					 SET @AccountOwner = 0
					 SET @ENDPOINT_USERID = 0
					 SET @ACCOUNTRECONID = 0
					 SET @DISPLAYNAME =''
					 SET @TECHNICALROLE =''
					 SET @DESCRIPTION =''
					 SET @POSITION =null
					 SET @ACCOUNTTYPE =0
					 SET @ACCOUNTNAME = ''	 
					 SET @USER_NAME		= ''
					 SET @USERDISPLAYNAME = ''

					 SELECT @ACCOUNTRECONID		= ACCOUNTRECONID,
							@DISPLAYNAME		= DISPLAYNAME,
							@ACCOUNTNAME		= ACCOUNTNAME,
							@ENDPOINT_USERID	= ENDPOINTUSERID,
							@ACCOUNTTYPE		= ACCOUNTTYPE,
							@USERDISPLAYNAME	= USERDISPLAYNAME,
							@USER_NAME			= USERNAME,
							@AccountOwner		= USERID
					 FROM	#TEMPACCOUNTS 
					 WHERE	ID = @INCR
					
					 --PRINT @ACCOUNTNAME
					DECLARE @ROLES TABLE(ACCOUNTNAME NVARCHAR(MAX), POSITION NVARCHAR(1024) NULL,ROLE NVARCHAR(MAX) NULL,DESCR NVARCHAR(MAX) NULL)
					
					INSERT INTO @ROLES(ACCOUNTNAME,POSITION,ROLE,DESCR)
					SELECT	@ACCOUNTNAME,NULL, E.Entitlement,E.EntitlementDescription
					FROM DI_M_EntitlementReconcilation E
					WHERE AccountReconcilationId = @ACCOUNTRECONID AND EndpointSystemId = @ENDPOINTID AND IsDeleted = 0 AND ENTITLEMENTTYPE = 1
					--FROM POSITION---
					DECLARE @POSCOUNT INT = 0
					IF OBJECT_ID('tempdb..#TBLPOST') IS NOT NULL
					BEGIN
							DROP TABLE #TBLPOST
					END
					SET @POSCOUNT = 0
					SELECT IDENTITY(INT,1,1) AS ID, Entitlement  INTO #TBLPOST FROM DI_M_EntitlementReconcilation WHERE AccountReconcilationId = @ACCOUNTRECONID AND IsDeleted = 0 AND EntitlementType = 5
					SET @POSCOUNT = @@ROWCOUNT

					DECLARE @POS NVARCHAR(100)=''
					WHILE @POSCOUNT > 0
					BEGIN
							SET @POSITION = NULL
							SELECT @POS = Entitlement FROM #TBLPOST WHERE ID = @POSCOUNT
							IF(@APPTYPE = 7 or @APPTYPE = 8)--MSSL--
							BEGIN
								IF(@APPTYPE=7)
								BEGIN
										SELECT @POSITION = PositionText FROM DI_SAP_MSSL_XL_PositionsMatrix WHERE Position_MSP = @POS
								END
								ELSE IF(@APPTYPE=8)
								BEGIN
										SELECT @POSITION = PositionText FROM DI_SAP_MSSL_XL_PositionsMatrix WHERE Position_MCP = @POS
								END
								
							END
							ELSE IF(@APPTYPE = 9 or @APPTYPE = 10)--EBS--
							BEGIN
								IF(@APPTYPE=9)
								BEGIN
										SELECT @POSITION = PositionText FROM DI_SAP_EBS_XL_PositionsMatrix WHERE Position_ISP = @POS
								END
								ELSE IF(@APPTYPE=10)
								BEGIN
										SELECT @POSITION = PositionText FROM DI_SAP_EBS_XL_PositionsMatrix WHERE Position_CRP = @POS
								END
							END
							IF (LEN(ISNULL(@POSITION,'')) = 0)
							BEGIN
									SET @POSITION = 'POSITION:'+@POS+'NOT FOUND IN MATRIX'
							END
							---getrolesby position
							INSERT INTO @ROLES(ACCOUNTNAME,POSITION,ROLE,DESCR)
							SELECT @ACCOUNTNAME, ISNULL(@POSITION,@POS),RelatedObjectCode,ROLE_DESC FROM (SELECT RelatedObjectCode FROM DI_SAP_SAPHRP1001 WHERE EndpointSystemId = @ENDPOINTID AND ActualObjectCode = @POS AND RelatedObjectType='AG' AND ActualObjectType='S')AS A
							INNER JOIN DI_SAP_ROLEINFO R ON A.RelatedObjectCode = R.ROLE_CODE WHERE EndPointSystemId = @ENDPOINTID
							--GetOrgByPosition--
							DECLARE @ORGUNIT NVARCHAR(512)=''
							SET @ORGUNIT  = (SELECT  TOP 1 RelatedObjectCode FROM DI_SAP_SAPHRP1001 WHERE EndpointSystemId=@ENDPOINTID AND ActualObjectCode = @POS AND RelatedObjectType='O' AND ActualObjectType='S')

							IF LEN(ISNULL(@ORGUNIT,'')) > 0
							BEGIN
							
									--getrolesbyorg--
									INSERT INTO @ROLES(ACCOUNTNAME,POSITION,ROLE,DESCR)
									SELECT @ACCOUNTNAME,ISNULL(@POSITION,@POS),RelatedObjectCode,ROLE_DESC FROM
									(SELECT RelatedObjectCode FROM DI_SAP_SAPHRP1001 WHERE EndpointSystemId=@ENDPOINTID AND RelatedObjectType='AG' AND ActualObjectType='O' AND ActualObjectCode = @ORGUNIT
									) AS A INNER JOIN DI_SAP_ROLEINFO R ON A.RelatedObjectCode = R.ROLE_CODE WHERE EndPointSystemId = @ENDPOINTID
									--GETPARENTOTGBYORG
									SET @ORGUNIT  = (SELECT  TOP 1 ActualObjectCode FROM DI_SAP_SAPHRP1001 WHERE EndpointSystemId=@ENDPOINTID AND ActualObjectCode = @ORGUNIT AND RelatedObjectType='O' AND ActualObjectType='O')

							END

							DECLARE @INCRE INT = 0
							DECLARE @ISPARENTEXISTS BIT = 1
							WHILE(@ISPARENTEXISTS = 1 AND LEN(ISNULL(@ORGUNIT,'')) > 0)
							BEGIN
									SET @INCRE = @INCRE + 1
									IF(@INCRE <= 5 AND LEN(ISNULL(@ORGUNIT,'')) > 0)
									BEGIN
											INSERT INTO @ROLES(ACCOUNTNAME,POSITION,ROLE,DESCR)
											SELECT @ACCOUNTNAME,@POSITION,RelatedObjectCode,ROLE_DESC FROM
											(SELECT RelatedObjectCode FROM DI_SAP_SAPHRP1001 WHERE EndpointSystemId=@ENDPOINTID AND RelatedObjectType='AG' AND ActualObjectType='O' AND ActualObjectCode = @ORGUNIT
											) AS A INNER JOIN DI_SAP_ROLEINFO R ON A.RelatedObjectCode = R.ROLE_CODE WHERE EndPointSystemId = @ENDPOINTID

											SET @ORGUNIT  = (SELECT  TOP 1 ActualObjectCode FROM DI_SAP_SAPHRP1001 WHERE EndpointSystemId=@ENDPOINTID AND ActualObjectCode = @ORGUNIT AND RelatedObjectType='O' AND ActualObjectType='O')

									END
									ELSE 
									BEGIN
											SET @ISPARENTEXISTS = 0
									END

							END
							
							SET @POSCOUNT = @POSCOUNT - 1
					END


					DECLARE @CURRENTACCESS TABLE(
					[BusinessRole] [nvarchar](max) NULL,
					[TechnicalRole] [nvarchar](max) NULL,
					[REQ_Position] [nvarchar](max) NULL,
					[ConflictingRole] [nvarchar](max) NULL,
					[ConflictingTechnicalRole] [nvarchar](max) NULL,
					[SoDRemarks] [nvarchar](max) NULL,
					[SODOwner] [nvarchar](max) null,
					ISCURRENTPOSITION BIT DEFAULT(0)
					)


					INSERT INTO @CURRENTACCESS(BusinessRole,TechnicalRole,REQ_Position,ConflictingRole,ConflictingTechnicalRole,SoDRemarks,ISCURRENTPOSITION,[SODOwner])
					SELECT 
					M.ConflictingBusinessRole,M.ConflictingTechnicalRole,B.POSITION,M.BusinessRole,M.TechnicalRole,M.ConflictingRemarks,CASE WHEN B.ROLE = M.TechnicalRole AND B.POSITION IS NOT NULL THEN 1 ELSE 0 END,SODOwner
					FROM DI_SAP_SODConflictingRole M INNER JOIN @ROLES B ON M.TechnicalRole = B.ROLE where M.EndpointSystemName=@MODULE

					INSERT INTO @CURRENTACCESS(BusinessRole,TechnicalRole,REQ_Position,ConflictingRole,ConflictingTechnicalRole,SoDRemarks,ISCURRENTPOSITION,[SODOwner])
					SELECT 
					M.BusinessRole,M.TechnicalRole,B.POSITION,M.ConflictingBusinessRole,M.ConflictingTechnicalRole,M.ConflictingRemarks,CASE WHEN B.ROLE = M.ConflictingTechnicalRole AND B.POSITION IS NOT NULL THEN 1 ELSE 0 END,SODOwner
					FROM DI_SAP_SODConflictingRole M INNER JOIN @ROLES B ON M.ConflictingTechnicalRole = B.ROLE where M.EndpointSystemName=@MODULE

					IF OBJECT_ID('tempdb..#TBLRESULT') IS NOT NULL
					BEGIN
							DROP TABLE #TBLRESULT
					END
					SELECT A.ISCURRENTPOSITION,A.REQ_Position,A.ConflictingRole,A.ConflictingTechnicalRole,A.BusinessRole,A.TechnicalRole,R.POSITION,A.SoDRemarks,A.SODOwner INTO #TBLRESULT
					 FROM @CURRENTACCESS A INNER JOIN @ROLES R ON A.TechnicalRole = R.ROLE

					 INSERT INTO [dbo].DI_SAP_ConflictingRoleReport(EndPointSystemId,EndpointSystemName,AccountName,Displayname,AccountType,BusinessRole,TechnicalRole,
						ConflictingRole,ConflictingTechnicalRole,From_Position,SoDRemarks,CreatedBy,CreatedDate,ConflictingPosition,UserName,UserDisplayName,[SODOwner],[AccountOwnerId])

					SELECT distinct A.EndPointSystemId,A.EndpointSystemName,A.AccountName,A.Displayname,A.AccountType,A.BusinessRole,A.TechnicalRole, A.ConflictingRole,A.ConflictingTechnicalRole,A.Position,
					A.SoDRemarks,A.CREATEDBY,A.CREATEDDATE,A.ConflictingPosition,A.UserName,A.UserDisplayName,A.SODOwner,A.AccountOwner 
					FROM
					(
						SELECT @ENDPOINTID as EndPointSystemId,@ENDPOINT_NAME as EndpointSystemName,@ACCOUNTNAME as AccountName,
						@DISPLAYNAME as Displayname,@ACCOUNTTYPE as AccountType,A.BusinessRole,A.TechnicalRole, A.ConflictingRole,A.ConflictingTechnicalRole,A.Position,A.SoDRemarks,'SYSTEM' AS CREATEDBY,
						GETDATE() AS CREATEDDATE,A.REQ_Position as ConflictingPosition, @USER_NAME as UserName,@USERDISPLAYNAME as UserDisplayName,A.SODOwner,@AccountOwner as AccountOwner
						FROM #TBLRESULT A WHERE A.ConflictingTechnicalRole > A.TechnicalRole OR NOT EXISTS(SELECT 1 FROM #TBLRESULT B WHERE B.ConflictingTechnicalRole = A.TechnicalRole AND B.TechnicalRole = A.ConflictingTechnicalRole )
					)AS A

					IF @DEBUG = 1
					BEGIN
						SELECT * FROM @CURRENTACCESS
						SELECT * FROM @ROLES
						SELECT * FROM #TEMPACCOUNTS
						SELECT * FROM #TBLRESULT

					END
					DELETE FROM @CURRENTACCESS
					DELETE FROM @ROLES

					SET @INCR = @INCR + 1
			END
		
	END
	IF OBJECT_ID('tempdb..#TBLRESULT') IS NOT NULL
	BEGIN
			DROP TABLE #TBLRESULT
	END
	IF OBJECT_ID('tempdb..#TBLPOST') IS NOT NULL
	BEGIN
			DROP TABLE #TBLPOST
	END
	IF OBJECT_ID('tempdb..#TEMPACCOUNTS') IS NOT NULL
	BEGIN
			DROP TABLE #TEMPACCOUNTS
	END

END

GO
IF NOT EXISTS(SELECT 1 FROM [dbo].[DI_M_MailTemplate] WHERE [TemplateName]='NotifyInactiveEmployeeEvent')
BEGIN
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'NotifyInactiveEmployeeEvent', N'Notify the ITSecurity Team for inactive trigger', N'securityadmin@spgroup.com.sg', N'', NULL, NULL, N'IDMS: Warning: EmployeeEvent - @@EVENTNAME@@ exist as Inactive for Employee(@@EMPLOYEEID@@) EffectiveDate: @@EFFECTIVEDATE@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
    <head>
        <title>NotifyInactiveEmployeeEvent</title>
        <style type="text/css">
            .body {              font-family: Courier New;              font-size: 13px;              font-weight: normal;              margin-left: 5px;              }              .marlef {              margin-left: 40px;              }              .marlef1 {              margin-left: 50px;              }              .marlef2 {              margin-left: 50px;              }
        </style>
    </head>
    <body class="body" jquery191031130699542479323="28" jquery19105777307022835423="28" _events="[object Object]">
        <p style="margin-left: 40px;"><span style="text-align: left; text-transform: none; text-indent: 0px; letter-spacing: normal; font-style: normal; font-weight: 400; word-spacing: 0px; float: none; white-space: pre-wrap; orphans: 2; widows: 2; background-color: #ffffff; text-decoration-color: initial; font-variant-ligatures: normal; font-variant-caps: normal; -webkit-text-stroke-width: 0px; text-decoration-style: initial; display: inline !important; font-family: ''Courier New''; font-size: 13px; color: #000000;">Dear IAM Team,</span></p>
        <p style="margin-left: 40px;"><span style="text-align: left; text-transform: none; text-indent: 0px; letter-spacing: normal; font-style: normal; font-weight: 400; word-spacing: 0px; float: none; white-space: pre-wrap; orphans: 2; widows: 2; background-color: #ffffff; text-decoration-color: initial; font-variant-ligatures: normal; font-variant-caps: normal; -webkit-text-stroke-width: 0px; text-decoration-style: initial; display: inline !important; font-family: ''Courier New''; font-size: 13px; color: #000000;">The Event @@EVENTNAME@@ already exist as Inactive for Employee(@@EMPLOYEEID@@) - Existing EmployeeEventTrigger Id: @@EXISTING_EVENTID@@ .</span></p>
<p style="margin-left: 40px;">
    <b>EmployeeId</b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;: @@EMPLOYEEID@@ <br>
    <b>Event Name</b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;: @@EVENTNAME@@ <br>
    <b>Existing EmployeeEvent Trigger Id&nbsp;</b>:&nbsp;@@EXISTING_EVENTID@@ <br>
	<b>EffectiveDate&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</b>:&nbsp;@@EFFECTIVEDATE@@ <br>
 </p>    
     
        
        
        
        <p style="font-family: arial; margin-left: 40px;">&nbsp;<span style="font-size: small; white-space: pre-wrap; background-color: #ffffff; font-family: ''Courier New''; color: infotext;"></span></p>
        
        <p style="margin-left: 40px;">Regards,</p>
        <p style="margin-left: 40px;">@@REGARDS@@</p>
        <p style="margin-left: 40px;"><span style="font-family: ''Courier New''; font-size: 13px; color: infotext;">&nbsp;</span></p>
        <p style="margin-left: 40px;"><span style="font-family: ''Courier New''; font-size: 13px; color: infotext;">*** This is a system-generated message. Please do not reply to this message.***</span></p>
    
</body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
END
