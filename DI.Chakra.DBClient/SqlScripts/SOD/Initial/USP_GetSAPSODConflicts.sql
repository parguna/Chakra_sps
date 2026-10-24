USE [V5_IDMS_SP]
GO
/****** Object:  StoredProcedure [dbo].[USP_GetSAPSODConflicts]    Script Date: 31/12/2021 10:51:16 am ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [dbo].[USP_GetSAPSODConflicts]
(
@userId BIGINT,
@currentRequestId BIGINT,
@isCurrentBulkRequest BIT=0,
@applicationType NVARCHAR(MAX),
@inGrid bit= 1,
@debug bit=0
)
AS
BEGIN
	
	BEGIN TRY 
	SET NOCOUNT ON;
	--declare @flinsApplicationType int 
	DECLARE @AdhocActionType INT
	DECLARE @progressStatus BIGINT
	DECLARE @ACCOUNTTYPE TINYINT

	DECLARE @tblCurrentRequestTabContent TABLE(id INT IDENTITY,    xmlcontent XML)
	DECLARE @tblExistingXmlContent TABLE(id INT IDENTITY,RequestId INT,[source] NVARCHAR(500), xmlcontent XML NULL,ENDPOINTNAME NVARCHAR(MAX) null,ENDPOINTID NVARCHAR(MAX) null,ApplicationType int null)
	DECLARE @tblExistingRequestEntitlements TABLE(id INT IDENTITY,[SOURCE] NVARCHAR(500),RequestId INT,Entitlement NVARCHAR(1000),TECHNICALROLE NVARCHAR(MAX) NULL,POSITION NVARCHAR(MAX) NULL,ENDPOINTNAME NVARCHAR(MAX) null,ENDPOINTID NVARCHAR(MAX) null,ApplicationType int null)
	DECLARE @tblCurrentRequestEntitlements TABLE(id INT IDENTITY,Entitlement  NVARCHAR(MAX))
	DECLARE @tblCurrentAccess TABLE(id INT IDENTITY,[SOURCE] NVARCHAR(500),RequestId INT,Entitlement NVARCHAR(1000),TECHNICALROLE NVARCHAR(MAX) NULL,POSITION NVARCHAR(MAX) NULL,ENDPOINTNAME NVARCHAR(MAX) null,ENDPOINTID NVARCHAR(MAX) null,ApplicationType int null)

	DECLARE @tblCRequestRole TABLE(id INT IDENTITY,POSITION  NVARCHAR(MAX),TechnicalRole NVARCHAR(MAX), BUSINESSROLE NVARCHAR(MAX) NULL, ENDPOINTNAME NVARCHAR(MAX),EP_CODE NVARCHAR(32) NULL)
	
	--DECLARE @tbldependent_CRequestRole TABLE(id INT IDENTITY,POSITION  NVARCHAR(MAX),TechnicalRole NVARCHAR(MAX), BUSINESSROLE NVARCHAR(MAX) NULL, ENDPOINTNAME NVARCHAR(MAX))

	DECLARE @tblConflictBRoles TABLE(id INT IDENTITY, BRole  NVARCHAR(MAX)NULL,TRole  NVARCHAR(MAX)NULL,CBRole  NVARCHAR(MAX) NULL,CTRole  NVARCHAR(MAX) NULL,ConflictingRemarks NVARCHAR(MAX) NULL,Position NVARCHAR(MAX) NULL, ENDPOINTNAME NVARCHAR(MAX),EP_APPLNCODE NVARCHAR(32) NULL, FROMCurrentRequestPosition bit default(0))
  
	DECLARE @RESULT TABLE(ID INT IDENTITY(1,1), EndpointName nvarchar(512) null,RequestedRole nvarchar(max) null,ConflictedRole nvarchar(max) null,ConflictRemarks nvarchar(max) null,[Source] nvarchar(512) null,[ReqRoleForSorting] nvarchar(512) null)
	
	DECLARE @endpointUserId INT = 0
	DECLARE @i INT =  0
	DECLARE @splitter nvarchar(1) = ','
	DECLARE @XMLCONTENT XML 
	DECLARE @count INT
	DECLARE @applicationTypeXML NVARCHAR(MAX) 
	DECLARE @positionText NVARCHAR(200) 
	DECLARE @applicationName NVARCHAR(100)
	DECLARE @endpointSystemId INT =0 
	SET @AdhocActionType = 1  
	SET @progressStatus  = 1  
    DECLARE @endpointsystem NVARCHAR(MAX) = ''
	DECLARE @ISCOMPLETEDREQUEST BIT = 0
	SET @i=0  
	SET @endpointUserId   = 0 
	DECLARE @POSITIONS_TBL TABLE(POSITION VARCHAR(MAX));
	DECLARE @TEMP_POS_CODE NVARCHAR(64)=''	

	IF @isCurrentBulkRequest = 0
	BEGIN
		SELECT @endpointSystemId =  EndpointSystemId,@ACCOUNTTYPE=AccountType FROM DI_M_AdHocAccessRequest WHERE id = @currentRequestId
		SELECT @ISCOMPLETEDREQUEST = CASE WHEN (ProgressStatus = 5 OR ProgressStatus=10 OR ProgressStatus=8  OR ProgressStatus=2) THEN 1 ELSE 0 END FROM DI_M_AccessRequest WHERE AdHocAccessRequestId = @currentRequestId
	END
	ELSE
	BEGIN
			---BULK REQUEST FOR USER ACCOUNT ONLY
			SELECT @endpointSystemId =  EndpointSystemId,@ISCOMPLETEDREQUEST = CASE WHEN (ProgressStatus = 5 OR ProgressStatus=10 OR ProgressStatus=8 OR ProgressStatus=2) THEN 1 ELSE 0 END FROM DI_M_BulkRequest WHERE id = @currentRequestId
			SET @ACCOUNTTYPE = 1;
	END
	
	DECLARE @REALLOCATE NVARCHAR(10)='false';
	declare @applnType tinyint=0
	declare @DEPENDENT_APPLNType tinyint=0
	SELECT @applnType =  ApplicationType,@endpointsystem = DisplayName FROM DI_M_EndpointSystem where Id = @endpointSystemId
	
	DECLARE @DEPENDENT_SYSTEMID BIGINT
	DECLARE @DEPENDENT_SYSTEM_NAME NVARCHAR(MAX)=''
	DECLARE @ACCOUNTNAME NVARCHAR(512)=''
	DECLARE @DEPENDENT_ENDPOINTUSERID INT = 0
	-----GET ENTITLEMENT DETAILS FROM CURRENT REQUEST -----
	IF @isCurrentBulkRequest = 0
	BEGIN

		SELECT @AdhocActionType = adhoc.AdhocActionType	FROM DI_M_AccessRequest AS accrrst join DI_M_AdHocAccessRequest AS adhoc ON accrrst.AdHocAccessRequestId = adhoc.Id 
		WHERE  adhoc.Id = @currentRequestId    
		INSERT INTO @tblCurrentRequestTabContent (xmlcontent) 
		( 
			SELECT  cast( adhoc.EntitlementDetail as XML)  FROM DI_M_AccessRequest AS accrrst join DI_M_AdHocAccessRequest AS adhoc ON accrrst.AdHocAccessRequestId = adhoc.Id 
			WHERE  adhoc.Id = @currentRequestId    
		)
	END
	ELSE
	BEGIN
		SELECT @AdhocActionType = AdhocActionType,@endpointUserId = BLKD.EndpointUserId 
		FROM DI_M_BulkRequest AS bulkReq  INNER JOIN DI_M_BulkRequestDetail BLKD ON bulkReq.ID = BLKD.BulkRequestId 
		WHERE  bulkReq.Id = @currentRequestId AND BLKD.UserId = @userId    
		 
		INSERT INTO @tblCurrentRequestTabContent (xmlcontent) 
		(	 
			SELECT  cast(bulkReq.EntitlementDetail as XML) FROM DI_M_BulkRequest AS bulkReq
			WHERE  Id =  @currentRequestId 
		)
	END 
	
	DECLARE @EBSCRM NVARCHAR(500)	='CRP (EBS-CRM)'
	DECLARE @EBSISU NVARCHAR(500)	='ISP (EBS - ISU)'
	DECLARE @MSSLCRM NVARCHAR(500)	='MCP (MSSL - CRM)'
	DECLARE @MSSLISU NVARCHAR(500)	='MSP (MSSL - ISU)'
	
	DECLARE @DEPENDENT_APPln NVARCHAR(500)
	DECLARE @CURRENT_APPln NVARCHAR(500)


	DECLARE @MODULE NVARCHAR(500)=''
	
	IF @applnType = 10
	BEGIN
			SET @applicationTypeXML = 'AdHocEBSRequest'
			SET @CURRENT_APPln = @EBSCRM
			SET @DEPENDENT_APPln = @EBSISU
			SELECT @DEPENDENT_SYSTEMID = ID,@DEPENDENT_SYSTEM_NAME = DisplayName,@DEPENDENT_APPLNType = ApplicationType FROM DI_M_EndpointSystem where ApplicationType = 9
			SET @applicationName = @EBSCRM
	END
	ELSE IF @applnType = 9
	BEGIN
			SET @applicationTypeXML = 'AdHocEBSRequest'
			SET @CURRENT_APPln = @EBSISU
			SET @DEPENDENT_APPln = @EBSCRM
			SELECT @DEPENDENT_SYSTEMID = ID,@DEPENDENT_SYSTEM_NAME = DisplayName,@DEPENDENT_APPLNType = ApplicationType FROM DI_M_EndpointSystem where ApplicationType = 10
			SET @applicationName = @EBSISU
	END
	ELSE IF @applnType = 8
	BEGIN
			SET @applicationTypeXML = 'AdHocMSSLRequest'
			SET @CURRENT_APPln = @MSSLCRM
			SET @DEPENDENT_APPln =  @MSSLISU
			SELECT @DEPENDENT_SYSTEMID = ID,@DEPENDENT_SYSTEM_NAME = DisplayName,@DEPENDENT_APPLNType = ApplicationType FROM DI_M_EndpointSystem where ApplicationType = 7
			SET @applicationName = @MSSLCRM
			
	END
	ELSE IF @applnType = 7
	BEGIN
			SET @applicationTypeXML = 'AdHocMSSLRequest'
			SET @CURRENT_APPln = @MSSLISU
			SET @DEPENDENT_APPln = @MSSLCRM
			SELECT @DEPENDENT_SYSTEMID = ID,@DEPENDENT_SYSTEM_NAME = DisplayName,@DEPENDENT_APPLNType = ApplicationType FROM DI_M_EndpointSystem where ApplicationType = 8
			SET @applicationName = @MSSLISU
	END
	

	SELECT  @XMLCONTENT =  xmlcontent FROM @tblCurrentRequestTabContent; 
 
	---FETCH CURRENT REQUEST ROLES
	INSERT INTO @tblCurrentRequestEntitlements  
	SELECT DISTINCT   REPLACE(B.Item,CHAR(9),'') FROM 
	(SELECT Field.N.value('(Roles/text())[1]', 'nvarchar(500)') As Roles FROM 
	@XMLCONTENT.nodes('/*[local-name()=sql:variable("@applicationTypeXML")]') as Field (N) ) A CROSS APPLY dbo.split(A.Roles,@splitter) B
	

	IF @isCurrentBulkRequest <> 1
	BEGIN
			SELECT @endpointUserId =  E.EndpointUserId FROM 
			(
				SELECT Field.N.value('(text())[1]', 'nvarchar(500)') AS EndpointUserId FROM 
				@XMLCONTENT.nodes('/*[local-name()=sql:variable("@applicationTypeXML")]/EndpointUserId') as Field (N) 
			)AS  E
	END
	
	DECLARE @POSPREFIX NVARCHAR(512)=''
	SET @POSPREFIX = ''
	DECLARE @CURRENT_APPLN_TYPENAME NVARCHAR(32)
	SET @CURRENT_APPLN_TYPENAME = CASE WHEN @applnType = 7 THEN @MSSLISU WHEN @applnType = 8 THEN @MSSLCRM WHEN @applnType = 9 THEN @EBSISU WHEN @applnType = 10 THEN @EBSCRM ELSE 'NOTHING' END
			
	---GET CURRENT REQUEST ROLE AND INDIRECT ROLE FROM POSITION---
	IF(@applnType = 7 or @applnType = 8)
	BEGIN

			
			SELECT @ACCOUNTNAME =  E.AccName FROM 
			(
			SELECT Field.N.value('(text())[1]', 'nvarchar(500)') AS AccName FROM 
			@XMLCONTENT.nodes('/AdHocMSSLRequest/AccName') AS Field (N) ) E

			SELECT @POSPREFIX =  E.PositionPrefix FROM 
			(
			SELECT Field.N.value('(text())[1]', 'nvarchar(500)') AS PositionPrefix FROM 
			@XMLCONTENT.nodes('/AdHocMSSLRequest/PositionPrefix') AS Field (N) ) E

			SELECT @positionText =  E.PositionText FROM 
			(
			SELECT Field.N.value('(text())[1]', 'nvarchar(500)') AS PositionText FROM 
			@XMLCONTENT.nodes('/AdHocMSSLRequest/Position') AS Field (N) ) E

			SELECT @REALLOCATE =  E.IsReallocate FROM 
			(
			SELECT Field.N.value('(text())[1]', 'nvarchar(500)') AS IsReallocate FROM 
			@XMLCONTENT.nodes('/AdHocMSSLRequest/IsReallocate') AS Field (N) ) E
			


			---GET REQUESTED DETAILS FROM CURRENT REQUEST--
			INSERT INTO @tblCRequestRole(BUSINESSROLE,TechnicalRole,ENDPOINTNAME,EP_CODE)
			SELECT A.RoleName,item,@endpointsystem,@CURRENT_APPLN_TYPENAME FROM
			(
				SELECT M.RoleName,M.TechnicalRole from DI_SAP_MSSL_XL_RolesMatrix M inner join @tblCurrentRequestEntitlements B on M.RoleName = B.Entitlement
				where m.Application = 	@CURRENT_APPLN_TYPENAME
			) as A CROSS APPLY [dbo].[Split](A.TechnicalRole,',')

	END
	ELSE IF(@applnType = 9 or @applnType = 10)
	BEGIN
			SELECT @ACCOUNTNAME =  E.AccName FROM 
			(
			SELECT Field.N.value('(text())[1]', 'nvarchar(500)') AS AccName FROM 
			@XMLCONTENT.nodes('/AdHocEBSRequest/AccName') AS Field (N) ) E

			

			SELECT @REALLOCATE =  E.IsReallocate FROM 
			(
			SELECT Field.N.value('(text())[1]', 'nvarchar(500)') AS IsReallocate FROM 
			@XMLCONTENT.nodes('/AdHocEBSRequest/IsReallocate') AS Field (N) ) E

			SELECT @POSPREFIX =  E.PositionPrefix FROM 
			(
			SELECT Field.N.value('(text())[1]', 'nvarchar(500)') AS PositionPrefix FROM 
			@XMLCONTENT.nodes('/AdHocEBSRequest/PositionPrefix') AS Field (N) ) E

			SELECT @positionText =  E.PositionText FROM 
			(
			SELECT Field.N.value('(text())[1]', 'nvarchar(500)') AS PositionText FROM 
			@XMLCONTENT.nodes('/AdHocEBSRequest/Position') AS Field (N) ) E
			
			
			
			INSERT INTO @tblCRequestRole(BUSINESSROLE,TechnicalRole,ENDPOINTNAME,EP_CODE)
			SELECT A.RoleName,item,@endpointsystem,@CURRENT_APPLN_TYPENAME FROM
			(
				SELECT M.RoleName,M.TechnicalRole from DI_SAP_EBS_XL_RolesMatrix M inner join @tblCurrentRequestEntitlements B on M.RoleName = B.Entitlement
				where m.Application = 	@CURRENT_APPLN_TYPENAME
			) as A CROSS APPLY [dbo].[Split](A.TechnicalRole,',')
			
	END

	
	----FIND THE INDIRECT ROLE FROM POSITION -------------------
	IF LEN(ISNULL(@positionText,''))>0
	BEGIN
		IF @applicationType = 'MSSL'
		BEGIN
				
				IF(@applicationName = @MSSLCRM)
				BEGIN
						SET @TEMP_POS_CODE = ''
						SET @TEMP_POS_CODE = (SELECT TOP 1 Position_MCP FROM DI_SAP_MSSL_XL_PositionsMatrix WHERE PositionText=@positionText)

						INSERT INTO @tblCRequestRole(POSITION,TechnicalRole,BUSINESSROLE,ENDPOINTNAME,EP_CODE)
						SELECT @positionText,[ROLE],ISNULL(M.ROLENAME,DESCR),@endpointsystem,@CURRENT_APPln from dbo.getRoleFromPosition(@endpointSystemId,@TEMP_POS_CODE) AS A
						LEFT JOIN 
						(
						SELECT A.RoleName,REPLACE(ITEM,CHAR(9),'') AS TECROLE FROM DI_SAP_MSSL_XL_RolesMatrix A CROSS APPLY DBO.Split(A.TechnicalRole,',')  
						WHERE [Application] = @CURRENT_APPln
						)AS M ON A.ROLE = M.TECROLE


						

						IF EXISTS(SELECT 1 FROM DI_SAP_MSSL_XL_PositionsMatrix WHERE PositionText=@positionText AND LEN(LTRIM(RTRIM(ISNULL(Position_MSP,''))))>0)
						BEGIN
								SET @TEMP_POS_CODE = ''
								SET @TEMP_POS_CODE = (SELECT TOP 1 Position_MSP FROM DI_SAP_MSSL_XL_PositionsMatrix WHERE PositionText=@positionText)

								INSERT INTO @tblCRequestRole(POSITION,TechnicalRole,BUSINESSROLE,ENDPOINTNAME,EP_CODE)
								SELECT @positionText,[ROLE],ISNULL(M.ROLENAME,DESCR),@DEPENDENT_SYSTEM_NAME,@DEPENDENT_APPln FROM 
								dbo.getRoleFromPosition(@DEPENDENT_SYSTEMID,@TEMP_POS_CODE) AS A
								LEFT JOIN 
								(
								SELECT A.RoleName,REPLACE(ITEM,CHAR(9),'') AS TECROLE FROM DI_SAP_MSSL_XL_RolesMatrix A CROSS APPLY DBO.Split(A.TechnicalRole,',')  
								WHERE [Application] = @DEPENDENT_APPln
								)AS M ON A.ROLE = M.TECROLE

						END
				END
				ELSE
				BEGIN
						SET @TEMP_POS_CODE = ''
						SET @TEMP_POS_CODE = (SELECT TOP 1  Position_MSP FROM DI_SAP_MSSL_XL_PositionsMatrix WHERE PositionText=@positionText)
						
						INSERT INTO @tblCRequestRole(POSITION,TechnicalRole,BUSINESSROLE,ENDPOINTNAME,EP_CODE)
						SELECT @positionText,[ROLE],ISNULL(M.ROLENAME,DESCR),@endpointsystem,@CURRENT_APPln from dbo.getRoleFromPosition(@endpointSystemId,@TEMP_POS_CODE) AS A
						LEFT JOIN 
						(
						SELECT A.RoleName,REPLACE(ITEM,CHAR(9),'') AS TECROLE FROM DI_SAP_MSSL_XL_RolesMatrix A CROSS APPLY DBO.Split(A.TechnicalRole,',')  
						WHERE [Application] = @CURRENT_APPln
						)AS M ON A.ROLE = M.TECROLE
						

						
						IF EXISTS(SELECT 1 FROM DI_SAP_MSSL_XL_PositionsMatrix WHERE PositionText=@positionText AND LEN(LTRIM(RTRIM(ISNULL(Position_MCP,''))))>0)
						BEGIN
								SET @TEMP_POS_CODE = ''
								SET @TEMP_POS_CODE = (SELECT TOP 1 Position_MCP FROM DI_SAP_MSSL_XL_PositionsMatrix WHERE PositionText=@positionText)
								INSERT INTO @tblCRequestRole(POSITION,TechnicalRole,BUSINESSROLE,ENDPOINTNAME,EP_CODE)
								SELECT @positionText,[ROLE],ISNULL(M.ROLENAME,DESCR),@DEPENDENT_SYSTEM_NAME,@DEPENDENT_APPln from dbo.getRoleFromPosition(@DEPENDENT_SYSTEMID,@TEMP_POS_CODE) AS A
								LEFT JOIN 
								(
								SELECT A.RoleName,REPLACE(ITEM,CHAR(9),'') AS TECROLE FROM DI_SAP_MSSL_XL_RolesMatrix A CROSS APPLY DBO.Split(A.TechnicalRole,',')  
								WHERE [Application] = @DEPENDENT_APPln
								)AS M ON A.ROLE = M.TECROLE
								
						END
			
				END
		END
		ELSE
		BEGIN
				IF(@applicationName = @EBSCRM)
				BEGIN
						SET @TEMP_POS_CODE = ''
						SET @TEMP_POS_CODE = (SELECT TOP 1 Position_CRP FROM DI_SAP_EBS_XL_PositionsMatrix WHERE PositionText=@positionText)

						INSERT INTO @tblCRequestRole(POSITION,TechnicalRole,BUSINESSROLE,ENDPOINTNAME,EP_CODE)
						SELECT @positionText,[ROLE],ISNULL(M.ROLENAME,DESCR),@endpointsystem,@CURRENT_APPln from dbo.getRoleFromPosition(@endpointSystemId,@TEMP_POS_CODE) AS A
						LEFT JOIN 
						(
							SELECT A.RoleName,REPLACE(ITEM,CHAR(9),'') AS TECROLE FROM DI_SAP_EBS_XL_RolesMatrix A CROSS APPLY DBO.Split(A.TechnicalRole,',')  
							WHERE [Application] = @CURRENT_APPln
						)AS M ON A.ROLE = M.TECROLE

						

						IF EXISTS(SELECT 1 FROM DI_SAP_EBS_XL_PositionsMatrix WHERE PositionText=@positionText AND LEN(LTRIM(RTRIM(ISNULL(Position_ISP,''))))>0)
						BEGIN
							SET @TEMP_POS_CODE = ''
							SET @TEMP_POS_CODE = (SELECT  TOP 1 Position_ISP FROM DI_SAP_EBS_XL_PositionsMatrix WHERE PositionText=@positionText)

							INSERT INTO @tblCRequestRole(POSITION,TechnicalRole,BUSINESSROLE,ENDPOINTNAME,EP_CODE)
							SELECT @positionText,[ROLE],ISNULL(M.ROLENAME,DESCR),@DEPENDENT_SYSTEM_NAME,@DEPENDENT_APPln from dbo.getRoleFromPosition(@DEPENDENT_SYSTEMID,@TEMP_POS_CODE) AS A
							LEFT JOIN 
							(
							SELECT A.RoleName,ITEM AS TECROLE FROM DI_SAP_EBS_XL_RolesMatrix A CROSS APPLY DBO.Split(A.TechnicalRole,',')  
							WHERE [Application] = @DEPENDENT_APPln
							)AS M ON A.ROLE = M.TECROLE

						END
			
				END
				ELSE
				BEGIN
						SET @TEMP_POS_CODE = ''
						SET @TEMP_POS_CODE = (SELECT TOP 1 Position_ISP FROM DI_SAP_EBS_XL_PositionsMatrix WHERE PositionText=@positionText)

						INSERT INTO @tblCRequestRole(POSITION,TechnicalRole,BUSINESSROLE,ENDPOINTNAME,EP_CODE)
						SELECT @positionText,[ROLE],ISNULL(M.ROLENAME,DESCR),@endpointsystem,@CURRENT_APPln from dbo.getRoleFromPosition(@endpointSystemId,@TEMP_POS_CODE) AS A
						LEFT JOIN 
						(
						SELECT A.RoleName,REPLACE(ITEM,CHAR(9),'') AS TECROLE FROM DI_SAP_EBS_XL_RolesMatrix A CROSS APPLY DBO.Split(A.TechnicalRole,',')  
						WHERE [Application] = @CURRENT_APPln
						)AS M ON A.ROLE = M.TECROLE



						IF EXISTS(SELECT 1 FROM DI_SAP_EBS_XL_PositionsMatrix WHERE PositionText=@positionText AND LEN(LTRIM(RTRIM(ISNULL(Position_CRP,''))))>0)
						BEGIN

							SET @TEMP_POS_CODE = ''
							SET @TEMP_POS_CODE = (SELECT TOP 1 Position_CRP FROM DI_SAP_EBS_XL_PositionsMatrix WHERE PositionText=@positionText)

							INSERT INTO @tblCRequestRole(POSITION,TechnicalRole,BUSINESSROLE,ENDPOINTNAME,EP_CODE)
							SELECT @positionText,[ROLE],ISNULL(M.ROLENAME,DESCR),@DEPENDENT_SYSTEM_NAME,@DEPENDENT_APPln from dbo.getRoleFromPosition(@DEPENDENT_SYSTEMID,@TEMP_POS_CODE) AS A
							LEFT JOIN 
							(
							SELECT A.RoleName,REPLACE(ITEM,CHAR(9),'') AS TECROLE FROM DI_SAP_EBS_XL_RolesMatrix A CROSS APPLY DBO.Split(A.TechnicalRole,',')  
							WHERE [Application] = @DEPENDENT_APPln
							)AS M ON A.ROLE = M.TECROLE


						END

			
				END
		END
	END
			---CURRENT REQUEST NEW ACCOUNT AND AMENTMENTADD-REALLOCATE START---
	--BEGIN

			---CURRENT SYSTEM REQUEST CONFLICT ----
			INSERT INTO @tblConflictBRoles (BRole,TRole,CBRole,CTRole,ConflictingRemarks,ENDPOINTNAME,EP_APPLNCODE,FROMCurrentRequestPosition,Position)
			SELECT distinct
			ConflictingBusinessRole,ConflictingTechnicalRole,COALESCE(B.BUSINESSROLE, m.BusinessRole), m.TechnicalRole,m.ConflictingRemarks,CASE WHEN B.EP_CODE = @CURRENT_APPLN_TYPENAME THEN @endpointsystem ELSE @DEPENDENT_SYSTEM_NAME END,B.EP_CODE,
			CASE WHEN (B.TechnicalRole = M.TechnicalRole AND B.POSITION IS NOT NULL) THEN 1 ELSE 0 END,B.POSITION
			FROM DI_SAP_SODConflictingRole M INNER JOIN @tblCRequestRole B ON M.TechnicalRole = B.TechnicalRole where M.EndpointSystemName=B.EP_CODE
			

			INSERT INTO @tblConflictBRoles (BRole,TRole,CBRole,CTRole,ConflictingRemarks,ENDPOINTNAME,EP_APPLNCODE,FROMCurrentRequestPosition,Position)
			SELECT DISTINCT
			m.BusinessRole, m.TechnicalRole,COALESCE(B.BUSINESSROLE,ConflictingBusinessRole),ConflictingTechnicalRole,m.ConflictingRemarks,CASE WHEN B.EP_CODE = @CURRENT_APPLN_TYPENAME THEN @endpointsystem ELSE @DEPENDENT_SYSTEM_NAME END,B.EP_CODE,
			CASE WHEN (B.TechnicalRole = M.ConflictingTechnicalRole AND B.POSITION IS NOT NULL) THEN 1 ELSE 0 END,B.POSITION
			FROM DI_SAP_SODConflictingRole M INNER JOIN @tblCRequestRole B ON M.ConflictingTechnicalRole = B.TechnicalRole
			WHERE M.EndpointSystemName=B.EP_CODE
			
			IF @debug = 1
			BEGIN	
					SELECT * FROM @tblConflictBRoles
			

					SELECT 'REQUESTED ROLE'
					SELECT * FROM @tblCRequestRole

					SELECT 'REQUESTED DEPENDENT SYSTEM ROLE'
					--SELECT *FROM @tbldependent_CRequestRole
					SELECT 'Conflicting roles'
					SELECT * FROM @tblConflictBRoles
			END
			/*---FOR NEW ACCOUNT AND REALLOCATE------
				FOR REALLOCATION WE DO CLEARING OF EXISITING POSITION AND ROLES.HENCE NO NEED TO VALIDATE IN THE EXISITING REQUEST AND CURRENT ACCESS
			*/
			
			IF (@AdhocActionType = 1 or (@AdhocActionType = 2 AND @REALLOCATE <>'false' ) OR (@ACCOUNTTYPE = 1 AND @AdhocActionType = 2 AND LEN(LTRIM(RTRIM(ISNULL(@POSPREFIX,'')))) > 0 AND LEN(LTRIM(RTRIM(ISNULL(@ACCOUNTNAME,'')))) > 0 AND @ACCOUNTNAME NOT LIKE (@POSPREFIX+'%')) )
			BEGIN
					SELECT A.ENDPOINTNAME,A.FROMCurrentRequestPosition,A.Position,A.CBRole,A.CTRole,R.POSITION AS REQPOSITION,R.BUSINESSROLE,R.TechnicalRole,A.ConflictingRemarks
					INTO #TBLCURRENTREQ
					FROM @tblConflictBRoles A inner join @tblCRequestRole r ON A.TRole = R.TechnicalRole 
					WHERE A.ENDPOINTNAME = r.ENDPOINTNAME AND A.EP_APPLNCODE = R.EP_CODE AND EXISTS(SELECT 1 FROM DI_SAP_SODConflictingRole B  WHERE 
					B.TechnicalRole = A.TRole AND  A.EP_APPLNCODE = B.EndpointSystemName)


					INSERT INTO @RESULT(EndpointName,RequestedRole,ConflictedRole,ConflictRemarks,[Source],[ReqRoleForSorting])
					SELECT A.ENDPOINTNAME,CONCAT(CASE WHEN A.FROMCurrentRequestPosition =1 THEN ' POSITION: '+A.Position+'(Technical Role)' ELSE ' ' END,A.CBRole,'</b> [',A.CTRole,']'),
					CONCAT(CASE WHEN LEN(LTRIM(RTRIM(ISNULL(A.REQPOSITION,'')))) > 0 THEN 'POSITION: '+A.REQPOSITION+' ' ELSE '' END,A.BUSINESSROLE,'</b>[',A.TechnicalRole,']'),A.ConflictingRemarks,'Current Request',
					A.CTRole
					FROM (
					SELECT A.ENDPOINTNAME,A.FROMCurrentRequestPosition,A.Position,A.CBRole,A.CTRole,A.REQPOSITION AS REQPOSITION,A.BUSINESSROLE,A.TechnicalRole,A.ConflictingRemarks FROM #TBLCURRENTREQ A WHERE A.CTRole > A.TechnicalRole OR NOT EXISTS(SELECT 1 FROM #TBLCURRENTREQ B WHERE B.CTRole = A.TechnicalRole AND B.TechnicalRole = A.CTRole AND B.ENDPOINTNAME = A.ENDPOINTNAME )
					)AS A

					SELECT EndpointName,RequestedRole,ConflictedRole,ConflictRemarks,[Source] INTO #TBLFINALRESILTTBL FROM @RESULT ORDER BY EndpointName DESC,[ReqRoleForSorting],Id

					SELECT DISTINCT EndpointName,RequestedRole,ConflictedRole,ConflictRemarks,[Source] FROM #TBLFINALRESILTTBL

					IF OBJECT_ID('tempdb..#TBLFINALRESILTTBL') IS NOT NULL
					BEGIN
							DROP TABLE #TBLFINALRESILTTBL
					END		

					IF OBJECT_ID('tempdb..#TBLCURRENTREQ') IS NOT NULL
					BEGIN
							DROP TABLE #TBLCURRENTREQ
					END
					RETURN;
			END
	
	--END
	
	---CURRENT REQUEST NEW ACCOUNT AND AMENTMENTADD-REALLOCATE END---
	IF (@isCurrentBulkRequest = 1 AND @AdhocActionType = 2  )
	BEGIN

		SELECT @endpointUserId =  E.EndpointUserId FROM DI_M_BulkRequestDetail E INNER JOIN DI_M_BulkRequest B on E.BulkRequestId = B.Id WHERE B.Id = @currentRequestId AND E.UserId = @userId

	END

	DECLARE @USR_CURRENT_POS TABLE(ID INT IDENTITY(1,1), ENDPOINTID BIGINT,ENDPOINTNAME NVARCHAR(MAX),POSITION NVARCHAR(32),ApplicationType TINYINT,POS_DESC  NVARCHAR(MAX))
	
	IF (@AdhocActionType = 2 AND LEN(ISNULL(@endpointUserId,'')) > 0)
	BEGIN

			
			SELECT @ACCOUNTNAME = UserLogonName FROM DI_SAP_UserInfo WHERE ID = @endpointUserId
			
			declare @tblroles Table(rolecode nvarchar(max), roledesc nvarchar(max))
			
			
			IF(@applnType = 9 or @applnType = 10)--EBS
			BEGIN
					INSERT INTO @tblroles(rolecode,roledesc)
					SELECT item,A.RoleName FROM
					(
						SELECT M.RoleName,M.TechnicalRole from DI_SAP_EBS_XL_RolesMatrix m
						where m.Application = 	@CURRENT_APPLN_TYPENAME
					) as A CROSS APPLY [dbo].[Split](A.TechnicalRole,',')

			END
			ELSE IF (@APPLNTYPE = 7 OR @APPLNTYPE = 8)
			BEGIN
					INSERT INTO @tblroles(rolecode,roledesc)
					SELECT item,A.RoleName FROM
					(
						SELECT M.RoleName,M.TechnicalRole from DI_SAP_MSSL_XL_RolesMatrix m
						where m.Application = 	@CURRENT_APPLN_TYPENAME
					) as A CROSS APPLY [dbo].[Split](A.TechnicalRole,',')

			END

			---CURRENT USER ACCESS(ROLE) FOR ENDPOINT----
			INSERT INTO @tblCurrentAccess(ENDPOINTID,ENDPOINTNAME,Entitlement,TECHNICALROLE,SOURCE,RequestId,POSITION,ApplicationType)
			SELECT @endpointSystemId,@endpointsystem,r.roledesc,E.Entitlement,'CurrentAccess',null,null,@applnType FROM DI_M_EntitlementReconcilation E INNER JOIN DI_M_AccountReconcilation A ON E.AccountReconcilationId = A.ID
			LEFT JOIN (SELECT rolecode,roledesc FROM @tblroles)as R ON E.Entitlement = R.rolecode
			WHERE A.EndpointSystemId = @endpointSystemId AND E.EndpointSystemId = @endpointSystemId AND A.IsDeleted = 0 AND A.IsLocked = 0 AND E.IsDeleted = 0 AND A.EndpointUserId IN(
			SELECT ID FROM DI_SAP_UserInfo WHERE EndpointSystemId = @endpointSystemId AND UserLogonName = @ACCOUNTNAME  AND ID = @endpointUserId
			) AND E.EntitlementType = 1

			
			
			INSERT INTO @USR_CURRENT_POS(ENDPOINTID,ENDPOINTNAME,POSITION,ApplicationType,POS_DESC)
			SELECT @endpointSystemId,@endpointsystem,E.Entitlement,@applnType,EntitlementDescription FROM DI_M_EntitlementReconcilation E INNER JOIN DI_M_AccountReconcilation A ON E.AccountReconcilationId = A.ID
			WHERE A.EndpointSystemId = @endpointSystemId AND E.EndpointSystemId = @endpointSystemId AND A.IsDeleted = 0 AND A.IsLocked = 0 AND E.IsDeleted = 0 AND A.EndpointUserId IN(
			SELECT ID FROM DI_SAP_UserInfo WHERE EndpointSystemId = @endpointSystemId AND UserLogonName = @ACCOUNTNAME AND ID = @endpointUserId
			) AND E.EntitlementType = 5

			IF(@ACCOUNTTYPE = 1 OR @ACCOUNTTYPE = 4)
			BEGIN
					---EXISTING PENDING REQUEST FOR SAME ENDPOINT---
					INSERT INTO @tblExistingXmlContent([source], [RequestId] ,xmlcontent,ENDPOINTID,ENDPOINTNAME,ApplicationType) 
					SELECT [Source],RequestId, CAST(EntitlementDetail As XML)  EntitlementDetails,ENDPOINTSYSTEMID,ENDPOINTNAME,@applnType  FROM 
					(
 
						SELECT 'Adhoc Request'  as [Source], accrrst.Id  AS RequestId, adhoc.EntitlementDetail,E.ID  AS ENDPOINTSYSTEMID,E.DisplayName AS ENDPOINTNAME
						FROM DI_M_AccessRequest AS accrrst 
						JOIN DI_M_AdHocAccessRequest AS adhoc ON accrrst.AdHocAccessRequestId = adhoc.Id 
						INNER JOIN DI_M_EndpointSystem E on adhoc.EndpointSystemId = e.Id
						WHERE   adhoc.RequesteeUserId = @userId AND  adhoc.EntitlementDetail like '%<AccName>'+@ACCOUNTNAME+'</AccName>%' AND
						adhoc.EndpointSystemId = @endpointSystemId AND  
						adhoc.AdhocActionType in (2) AND adhoc.AccountType = @ACCOUNTTYPE AND
						(accrrst.ProgressStatus = @progressStatus OR accrrst.ProgressStatus = 3 OR accrrst.ProgressStatus = 6 OR accrrst.ProgressStatus = 9 OR accrrst.ProgressStatus = 4) AND  adhoc.Id <> @currentRequestId

						UNION							  
						SELECT 'Bulk Request' as [Source], bulkReq.Id  AS RequestId,  bulkReq.EntitlementDetail,E.ID  AS ENDPOINTSYSTEMID,E.DisplayName
						FROM DI_M_BulkRequest AS bulkReq
						JOIN DI_M_BulkRequestDetail bulkdetail on bulkReq.Id = bulkdetail.BulkRequestId
						INNER JOIN DI_M_EndpointSystem E on bulkReq.EndpointSystemId = e.Id
						WHERE 
						bulkdetail.UserId = @userId AND
						bulkReq.EndpointSystemId = @endpointSystemId AND  
						bulkReq.AdhocActionType IN (2) AND
						(bulkReq.ProgressStatus = @progressStatus OR bulkReq.ProgressStatus = 3 OR bulkReq.ProgressStatus = 6 OR bulkReq.ProgressStatus = 9 OR bulkReq.ProgressStatus = 4) AND  bulkReq.Id <> @currentRequestId AND (@ACCOUNTTYPE = 1)
					)AS C
			END
			ELSE ---SHARED ACCOUNT AND SERVICE ACCOUNT
			BEGIN
					---EXISTING PENDING REQUEST FOR SAME ENDPOINT---
					INSERT INTO @tblExistingXmlContent([source], [RequestId] ,xmlcontent,ENDPOINTID,ENDPOINTNAME,ApplicationType) 
					SELECT [Source],RequestId, CAST(EntitlementDetail As XML)  EntitlementDetails,ENDPOINTSYSTEMID,ENDPOINTNAME,@applnType  FROM 
					(
 
						SELECT 'Adhoc Request'  as [Source], accrrst.Id  AS RequestId, adhoc.EntitlementDetail,E.ID  AS ENDPOINTSYSTEMID,E.DisplayName AS ENDPOINTNAME
						FROM DI_M_AccessRequest AS accrrst 
						JOIN DI_M_AdHocAccessRequest AS adhoc ON accrrst.AdHocAccessRequestId = adhoc.Id 
						INNER JOIN DI_M_EndpointSystem E on adhoc.EndpointSystemId = e.Id
						WHERE adhoc.EntitlementDetail like '%<AccName>'+@ACCOUNTNAME+'</AccName>%' AND
						adhoc.EndpointSystemId = @endpointSystemId AND  
						adhoc.AdhocActionType in (2) AND adhoc.AccountType = @ACCOUNTTYPE AND
						(accrrst.ProgressStatus = @progressStatus OR accrrst.ProgressStatus = 3 OR accrrst.ProgressStatus = 6 OR accrrst.ProgressStatus = 9 OR accrrst.ProgressStatus = 4)  AND  adhoc.Id <> @currentRequestId
					)AS C

			END
			

			---CURRENT USER ACCESS(ROLE) FOR DEPENDENT ENDPOINT----

			IF(@applnType = 9 or @applnType = 10)--EBS
			BEGIN
					INSERT INTO @tblroles(rolecode,roledesc)
					SELECT item,A.RoleName FROM
					(
						SELECT M.RoleName,M.TechnicalRole from DI_SAP_EBS_XL_RolesMatrix m
						WHERE m.Application = 	@DEPENDENT_APPln
					) as A CROSS APPLY [dbo].[Split](A.TechnicalRole,',')

			END
			ELSE IF (@APPLNTYPE = 7 OR @APPLNTYPE = 8)
			BEGIN
					INSERT INTO @tblroles(rolecode,roledesc)
					SELECT item,A.RoleName FROM
					(
						SELECT M.RoleName,M.TechnicalRole from DI_SAP_MSSL_XL_RolesMatrix m
						WHERE m.Application = 	@DEPENDENT_APPln
					) as A CROSS APPLY [dbo].[Split](A.TechnicalRole,',')

			END


			INSERT INTO @tblCurrentAccess(ENDPOINTID,ENDPOINTNAME,Entitlement,TECHNICALROLE,SOURCE,RequestId,POSITION,ApplicationType)
			SELECT @DEPENDENT_SYSTEMID,@DEPENDENT_SYSTEM_NAME,ISNULL(E.EntitlementDescription,r.roledesc),E.Entitlement,'CurrentAccess',null,null,@DEPENDENT_APPLNType FROM DI_M_EntitlementReconcilation E INNER JOIN DI_M_AccountReconcilation A ON E.AccountReconcilationId = A.ID
			LEFT JOIN (SELECT rolecode,roledesc FROM @tblroles)as R ON E.Entitlement = R.rolecode
			WHERE A.EndpointSystemId = @DEPENDENT_SYSTEMID AND E.EndpointSystemId = @DEPENDENT_SYSTEMID AND A.IsDeleted = 0 AND A.IsLocked = 0 AND E.IsDeleted = 0 AND A.EndpointUserId IN(
			SELECT ID FROM DI_SAP_UserInfo WHERE EndpointSystemId = @DEPENDENT_SYSTEMID AND UserLogonName = @ACCOUNTNAME
			) AND E.EntitlementType = 1

			INSERT INTO @USR_CURRENT_POS(ENDPOINTID,ENDPOINTNAME,POSITION,ApplicationType,POS_DESC)
			SELECT @DEPENDENT_SYSTEMID,@DEPENDENT_SYSTEM_NAME,E.Entitlement,@DEPENDENT_APPLNType,EntitlementDescription FROM DI_M_EntitlementReconcilation E INNER JOIN DI_M_AccountReconcilation A ON E.AccountReconcilationId = A.ID
			WHERE A.EndpointSystemId = @DEPENDENT_SYSTEMID AND E.EndpointSystemId = @DEPENDENT_SYSTEMID AND A.IsDeleted = 0 AND A.IsLocked = 0 AND E.IsDeleted = 0 AND A.EndpointUserId IN(
			SELECT ID FROM DI_SAP_UserInfo WHERE EndpointSystemId = @DEPENDENT_SYSTEMID AND UserLogonName = @ACCOUNTNAME
			) AND  E.EntitlementType = 5 

			IF (@ACCOUNTTYPE = 1 OR @ACCOUNTTYPE = 4)
			BEGIN

				---EXISTING PENDING REQUEST DEPENDENT SYSTEM---
				INSERT INTO @tblExistingXmlContent([source], [RequestId] ,xmlcontent,ENDPOINTID,ENDPOINTNAME,ApplicationType) 
				SELECT [Source],RequestId, CAST(EntitlementDetail As XML)  EntitlementDetails,ENDPOINTSYSTEMID,ENDPOINTNAME,@DEPENDENT_APPLNType as ApplicationType  FROM 
				(
 
					SELECT 'Adhoc Request'  as [Source], accrrst.Id  AS RequestId, adhoc.EntitlementDetail,E.ID  AS ENDPOINTSYSTEMID,E.DisplayName AS ENDPOINTNAME
					FROM DI_M_AccessRequest AS accrrst 
					JOIN DI_M_AdHocAccessRequest AS adhoc ON accrrst.AdHocAccessRequestId = adhoc.Id 
					INNER JOIN DI_M_EndpointSystem E on adhoc.EndpointSystemId = e.Id
					WHERE   adhoc.RequesteeUserId = @userId AND  
					adhoc.EndpointSystemId = @DEPENDENT_SYSTEMID AND  
					adhoc.AdhocActionType in (2) AND  adhoc.EntitlementDetail like '%<AccName>'+@ACCOUNTNAME+'</AccName>%' AND 
				    (accrrst.ProgressStatus = @progressStatus OR accrrst.ProgressStatus = 3 OR accrrst.ProgressStatus = 6 OR accrrst.ProgressStatus = 9 OR accrrst.ProgressStatus = 4) AND adhoc.AccountType = @ACCOUNTTYPE

					UNION							  
					SELECT 'Bulk Request' as [Source], bulkReq.Id  AS RequestId,  bulkReq.EntitlementDetail,E.ID  AS ENDPOINTSYSTEMID  ,E.DisplayName
					FROM DI_M_BulkRequest AS bulkReq
					JOIN DI_M_BulkRequestDetail bulkdetail on bulkReq.Id = bulkdetail.BulkRequestId
					INNER JOIN DI_M_EndpointSystem E on bulkReq.EndpointSystemId = e.Id
					WHERE bulkReq.EndpointSystemId = @DEPENDENT_SYSTEMID AND  
					bulkdetail.UserId = @userId AND
					bulkReq.EndpointSystemId = @DEPENDENT_SYSTEMID AND  
					bulkReq.AdhocActionType IN (1,2) AND 
					(bulkReq.ProgressStatus = @progressStatus OR bulkReq.ProgressStatus = 3 OR bulkReq.ProgressStatus = 6 OR bulkReq.ProgressStatus = 9 OR bulkReq.ProgressStatus = 4)
					AND (@ACCOUNTTYPE = 1)		     
				)AS C
			END
			ELSE
			BEGIN
					INSERT INTO @tblExistingXmlContent([source], [RequestId] ,xmlcontent,ENDPOINTID,ENDPOINTNAME,ApplicationType) 
					SELECT [Source],RequestId, CAST(EntitlementDetail As XML)  EntitlementDetails,ENDPOINTSYSTEMID,ENDPOINTNAME,@DEPENDENT_APPLNType as ApplicationType  FROM 
					(
 
						SELECT 'Adhoc Request'  as [Source], accrrst.Id  AS RequestId, adhoc.EntitlementDetail,E.ID  AS ENDPOINTSYSTEMID,E.DisplayName AS ENDPOINTNAME
						FROM DI_M_AccessRequest AS accrrst 
						JOIN DI_M_AdHocAccessRequest AS adhoc ON accrrst.AdHocAccessRequestId = adhoc.Id 
						INNER JOIN DI_M_EndpointSystem E on adhoc.EndpointSystemId = e.Id
						WHERE  
						adhoc.EndpointSystemId = @DEPENDENT_SYSTEMID AND  
						adhoc.AdhocActionType in (2) AND  adhoc.EntitlementDetail like '%<AccName>'+@ACCOUNTNAME+'</AccName>%' AND 
						(accrrst.ProgressStatus = @progressStatus OR accrrst.ProgressStatus = 3 OR accrrst.ProgressStatus = 6 OR accrrst.ProgressStatus = 9 OR accrrst.ProgressStatus = 4)	AND adhoc.AccountType = @ACCOUNTTYPE
					)AS C
			END


			
	END
	
	/**************************************************************** GET THE CURRENT ACCESS POSITION ROLE START********************************************************/
	
	

	DECLARE @CURCOUNT INT
	SELECT @CURCOUNT = COUNT(1) FROM @USR_CURRENT_POS
	DECLARE @INCR INT = 0
	DECLARE @CUR_APPTYPE TINYINT
	DECLARE @EPID INT
	DECLARE @EPNAME  NVARCHAR(MAX)
	DECLARE @POS_DESC  NVARCHAR(MAX)
	DECLARE @CUR_POS NVARCHAR(512)
	WHILE @INCR < @CURCOUNT 
	BEGIN
			SET @POS_DESC = ''
			SET @CUR_APPTYPE = 0
			SET @CUR_POS = ''
			SET @EPNAME = ''
			SET @EPID = NULL

			SELECT  @POS_DESC =POS_DESC, @CUR_APPTYPE = ApplicationType, @CUR_POS =POSITION,@EPNAME= ENDPOINTNAME,@EPID = ENDPOINTID FROM @USR_CURRENT_POS WHERE ID = @INCR + 1
			INSERT INTO @tblCurrentAccess(ENDPOINTID,ENDPOINTNAME,Entitlement,TECHNICALROLE,SOURCE,RequestId,POSITION,ApplicationType)
			SELECT @EPID,@EPNAME,DESCR,ROLE,'CurrentAccess',NULL,@POS_DESC,@CUR_APPTYPE FROM getRoleFromPosition(@EPID,@CUR_POS)
			WHERE NOT EXISTS(select * from @tblCurrentAccess u2 where u2.ENDPOINTID = @EPID and u2.TECHNICALROLE = ROLE)
			
			
			SET @INCR = @INCR +	1	
	END
	
	
	/**************************************************************** GET THE CURRENT ACCESS POSITION ROLE END**********************************************************/
	DECLARE @RequestId BIGINT
	DECLARE @source NVARCHAR(200)
	DECLARE @exisitingEndpointName NVARCHAR(512) =''
	DECLARE @exisitingEndpointID BIGINT
	DECLARE @exisitingApptype tinyint
	DECLARE @exisiting_EndpointUserId BIGINT
	DECLARE @temp_applcationName nvarchar(32)
	SET @i=0 
	SELECT @count = count(1) FROM @tblExistingXmlContent
	
	WHILE(@i < @count)
	BEGIN
			
			SET @XMLCONTENT = NULL
			SET @RequestId = 0
			SET @source = ''
			SET @exisitingEndpointID = 0
			SET @exisitingEndpointName = ''
			SET @exisitingApptype = 0
			SET @positionText = ''
			SET @REALLOCATE = ''
			SET @exisiting_EndpointUserId = 0


			SELECT    @XMLCONTENT = xmlcontent,@RequestId = RequestId, @source = source,@exisitingEndpointID=ENDPOINTID,@exisitingEndpointName= ENDPOINTNAME,@exisitingApptype = ApplicationType  FROM @tblExistingXmlContent WHERE id = @i+1
			

			set @temp_applcationName = CASE WHEN @exisitingApptype = 7 THEN @MSSLISU WHEN @exisitingApptype = 8 THEN @MSSLCRM WHEN @exisitingApptype = 9 THEN @EBSISU WHEN @exisitingApptype = 10 THEN @EBSCRM ELSE 'NOTHING' END
			
			---GET EXISTING REQUESTED ROLE---
			IF(@exisitingApptype = 7 or @exisitingApptype = 8)
			BEGIN
					

					SELECT @positionText =  E.PositionText FROM 
					(
					SELECT Field.N.value('(text())[1]', 'nvarchar(500)') AS PositionText FROM 
					@XMLCONTENT.nodes('/AdHocMSSLRequest/Position') AS Field (N) ) E

					
					SELECT @REALLOCATE =  E.IsReallocate FROM 
					(
					SELECT Field.N.value('(text())[1]', 'nvarchar(500)') AS IsReallocate FROM 
					@XMLCONTENT.nodes('/AdHocMSSLRequest/IsReallocate') AS Field (N) ) E
			
					SELECT @exisiting_EndpointUserId =  E.EndpointUserId FROM 
					(
					SELECT Field.N.value('(text())[1]', 'bigint') AS EndpointUserId FROM 
					@XMLCONTENT.nodes('/AdHocMSSLRequest/EndpointUserId') AS Field (N) ) E
					

					IF(@REALLOCATE ='false' )
					BEGIN
						----TAKE ONLY ROLE----
						INSERT INTO @tblExistingRequestEntitlements(Entitlement,TECHNICALROLE,RequestId,ApplicationType,ENDPOINTID,ENDPOINTNAME,SOURCE)
						SELECT A.RoleName,item,@RequestId,@exisitingApptype,@exisitingEndpointID,@exisitingEndpointName,@source FROM
						(
							SELECT M.RoleName,M.TechnicalRole from DI_SAP_MSSL_XL_RolesMatrix M inner join 
							(
								SELECT DISTINCT   REPLACE(B.ITEM,CHAR(9),'') as Entitlement FROM 
								(SELECT Field.N.value('(Roles/text())[1]', 'nvarchar(500)') As Roles FROM 
								@XMLCONTENT.nodes('/*[local-name()=sql:variable("@applicationTypeXML")]') as Field (N) 
								) A CROSS APPLY dbo.split(A.Roles,@splitter) B
								WHERE A.Roles IS NOT NULL
							) AS B on M.RoleName = B.Entitlement
							where m.Application = 	@temp_applcationName AND  M.TechnicalRole IS NOT NULL
						) as A CROSS APPLY [dbo].[Split](A.TechnicalRole,',')
						WHERE item IS NOT NULL
					END

			END
			ELSE IF(@exisitingApptype = 9 or @exisitingApptype = 10)
			BEGIN

					SELECT @exisiting_EndpointUserId =  E.EndpointUserId FROM 
					(
					SELECT Field.N.value('(text())[1]', 'bigint') AS EndpointUserId FROM 
					@XMLCONTENT.nodes('/AdHocEBSRequest/EndpointUserId') AS Field (N) ) E

					SELECT @REALLOCATE =  E.IsReallocate FROM 
					(
					SELECT Field.N.value('(text())[1]', 'nvarchar(500)') AS IsReallocate FROM 
					@XMLCONTENT.nodes('/AdHocEBSRequest/IsReallocate') AS Field (N) ) E

					SELECT @positionText =  E.PositionText FROM 
					(
					SELECT Field.N.value('(text())[1]', 'nvarchar(500)') AS PositionText FROM 
					@XMLCONTENT.nodes('/AdHocEBSRequest/Position') AS Field (N) ) E

					
					IF(@REALLOCATE ='false' )
					BEGIN
						----TAKE ONLY ROLE----
						INSERT INTO @tblExistingRequestEntitlements(Entitlement,TECHNICALROLE,RequestId,ApplicationType,ENDPOINTID,ENDPOINTNAME,SOURCE)
						SELECT A.RoleName,item,@RequestId,@exisitingApptype,@exisitingEndpointID,@exisitingEndpointName,@source FROM
						(
							SELECT M.RoleName,M.TechnicalRole from DI_SAP_EBS_XL_RolesMatrix M inner join 
							(
								SELECT DISTINCT   REPLACE(B.ITEM,CHAR(9),'') as Entitlement FROM 
								(SELECT Field.N.value('(Roles/text())[1]', 'nvarchar(500)') As Roles FROM 
								@XMLCONTENT.nodes('/*[local-name()=sql:variable("@applicationTypeXML")]') as Field (N) 
								) A CROSS APPLY dbo.split(A.Roles,@splitter) B
								WHERE A.Roles IS NOT NULL
							) AS B on M.RoleName = B.Entitlement
							where m.Application = 	@temp_applcationName AND  M.TechnicalRole IS NOT NULL

						) as A CROSS APPLY [dbo].[Split](A.TechnicalRole,',')
						WHERE Item IS NOT NULL
					END
			END

			
			IF(@REALLOCATE ='false' AND LEN(ISNULL(@positionText,''))>0)
			BEGIN
					
					---FINDING INDIRECT ROLE FROM  EXISITING REQUST POSITION
					IF(@exisitingApptype = 8) ---MSSL CRM
					BEGIN
							SET @TEMP_POS_CODE = ''
							SET @TEMP_POS_CODE = (SELECT TOP 1 Position_MCP FROM DI_SAP_MSSL_XL_PositionsMatrix WHERE PositionText=@positionText)

							INSERT INTO @tblExistingRequestEntitlements(POSITION, Entitlement,TECHNICALROLE,[SOURCE],RequestId,ENDPOINTID,ENDPOINTNAME,ApplicationType)
							SELECT DISTINCT  @positionText,ISNULL(M.ROLENAME,A.DESCR),A.ROLE,@source, @RequestId,@exisitingEndpointID,@exisitingEndpointName,@exisitingApptype
							FROM dbo.getRoleFromPosition(@exisitingEndpointID,@TEMP_POS_CODE) AS A
							LEFT JOIN 
							(
							SELECT A.RoleName,REPLACE(ITEM,CHAR(9),'') AS TECROLE FROM DI_SAP_MSSL_XL_RolesMatrix A CROSS APPLY DBO.Split(A.TechnicalRole,',')  
							WHERE [Application] = @temp_applcationName AND Item IS NOT NULL
							)AS M ON A.ROLE = M.TECROLE
							

					END
					ELSE IF(@exisitingApptype = 7) --MSSL ISU
					BEGIN
							SET @TEMP_POS_CODE = ''
							SET @TEMP_POS_CODE = (SELECT TOP 1 Position_MSP FROM DI_SAP_MSSL_XL_PositionsMatrix WHERE PositionText=@positionText)

							INSERT INTO @tblExistingRequestEntitlements(POSITION, Entitlement,TECHNICALROLE,[SOURCE],RequestId,ENDPOINTID,ENDPOINTNAME,ApplicationType)
							SELECT DISTINCT  @positionText,ISNULL(M.ROLENAME,A.DESCR),A.ROLE,@source, @RequestId,@exisitingEndpointID,@exisitingEndpointName,@exisitingApptype
							FROM dbo.getRoleFromPosition(@exisitingEndpointID,@TEMP_POS_CODE) AS A
							LEFT JOIN 
							(
							SELECT A.RoleName,REPLACE(ITEM,CHAR(9),'') AS TECROLE FROM DI_SAP_MSSL_XL_RolesMatrix A CROSS APPLY DBO.Split(A.TechnicalRole,',')  
							WHERE [Application] = @temp_applcationName  AND Item IS NOT NULL
							)AS M ON A.ROLE = M.TECROLE
							
							
							
					END
					ELSE IF(@exisitingApptype  = 10)---EBS CRM
					BEGIN
							SET @TEMP_POS_CODE = ''
							SET @TEMP_POS_CODE = (SELECT TOP 1 Position_CRP FROM DI_SAP_EBS_XL_PositionsMatrix WHERE PositionText=@positionText)

							INSERT INTO @tblExistingRequestEntitlements(POSITION, Entitlement,TECHNICALROLE,[SOURCE],RequestId,ENDPOINTID,ENDPOINTNAME,ApplicationType)
							SELECT DISTINCT  @positionText,ISNULL(M.ROLENAME,A.DESCR),A.ROLE,@source, @RequestId,@exisitingEndpointID,@exisitingEndpointName,@exisitingApptype
							FROM dbo.getRoleFromPosition(@exisitingEndpointID,@TEMP_POS_CODE) AS A
							LEFT JOIN 
							(
							SELECT A.RoleName,REPLACE(ITEM,CHAR(9),'') AS TECROLE FROM DI_SAP_EBS_XL_RolesMatrix A CROSS APPLY DBO.Split(A.TechnicalRole,',')  
							WHERE [Application] = @temp_applcationName AND Item IS NOT NULL
							)AS M ON A.ROLE = M.TECROLE
							
							
					END
					ELSE IF(@exisitingApptype  = 9) --EBS ISU
					BEGIN
							SET @TEMP_POS_CODE = ''
							SET @TEMP_POS_CODE = (SELECT TOP 1 Position_ISP FROM DI_SAP_EBS_XL_PositionsMatrix WHERE PositionText=@positionText)

							INSERT INTO @tblExistingRequestEntitlements(POSITION, Entitlement,TECHNICALROLE,[SOURCE],RequestId,ENDPOINTID,ENDPOINTNAME,ApplicationType)
							SELECT DISTINCT  @positionText,ISNULL(M.ROLENAME,A.DESCR),A.ROLE,@source, @RequestId,@exisitingEndpointID,@exisitingEndpointName,@exisitingApptype
							FROM dbo.getRoleFromPosition(@exisitingEndpointID,@TEMP_POS_CODE) AS A
							LEFT JOIN 
							(
							SELECT A.RoleName,REPLACE(ITEM,CHAR(9),'') AS TECROLE FROM DI_SAP_EBS_XL_RolesMatrix A CROSS APPLY DBO.Split(A.TechnicalRole,',')  
							WHERE [Application] = @temp_applcationName AND Item IS NOT NULL
							)AS M ON A.ROLE = M.TECROLE
							
							
					END
			END
			SET @i = @i + 1
	END

	IF @debug =1
	BEGIN
			SELECT 'CURRENT ACCESS DETAILS'
			SELECT * FROM @tblCurrentAccess
			SELECT 'CURRENT ACCESS PSOTION'
			SELECT * FROM @USR_CURRENT_POS
			SELECT 'PENDIGN REQUEST'
			SELECT * FROM @tblExistingRequestEntitlements
	END
	
	
	----CURRENT REQUEST----
	SELECT A.ENDPOINTNAME,A.FROMCurrentRequestPosition,A.Position,A.CBRole,A.CTRole,R.POSITION AS REQPOSITION,R.BUSINESSROLE,R.TechnicalRole,A.ConflictingRemarks
	INTO #TBLCURRENTREQAMENDMENT
	FROM @tblConflictBRoles A inner join @tblCRequestRole r ON A.TRole = R.TechnicalRole 
	WHERE A.ENDPOINTNAME = r.ENDPOINTNAME AND A.EP_APPLNCODE = R.EP_CODE AND EXISTS(SELECT 1 FROM DI_SAP_SODConflictingRole B  WHERE 
	B.TechnicalRole = A.TRole AND  A.EP_APPLNCODE = B.EndpointSystemName)

	INSERT INTO @RESULT(EndpointName,RequestedRole,ConflictedRole,ConflictRemarks,[Source],[ReqRoleForSorting])
	SELECT A.ENDPOINTNAME,CONCAT(CASE WHEN A.FROMCurrentRequestPosition =1 THEN ' POSITION: '+A.Position+'(Technical Role)' ELSE ' ' END,A.CBRole,'</b> [',A.CTRole,']'),
	CONCAT(CASE WHEN LEN(LTRIM(RTRIM(ISNULL(A.REQPOSITION,'')))) > 0 THEN 'POSITION: '+A.REQPOSITION+' ' ELSE '' END,A.BUSINESSROLE,'</b>[',A.TechnicalRole,']'),A.ConflictingRemarks,'Current Request',A.CTRole
	FROM (
	SELECT A.ENDPOINTNAME,A.FROMCurrentRequestPosition,A.Position,A.CBRole,A.CTRole,A.REQPOSITION ,A.BUSINESSROLE,A.TechnicalRole,A.ConflictingRemarks FROM #TBLCURRENTREQAMENDMENT
	A WHERE A.CTRole > A.TechnicalRole OR NOT EXISTS(SELECT 1 FROM #TBLCURRENTREQAMENDMENT B WHERE B.CTRole = A.TechnicalRole AND B.TechnicalRole = A.CTRole AND B.ENDPOINTNAME = A.ENDPOINTNAME )
	)AS A ORDER BY A.ENDPOINTNAME,A.TechnicalRole

	IF OBJECT_ID('tempdb..#TBLCURRENTREQAMENDMENT') IS NOT NULL
	BEGIN
			DROP TABLE #TBLCURRENTREQAMENDMENT
	END
			

	----EXISTING REQUEST----
	SELECT A.ENDPOINTNAME,A.FROMCurrentRequestPosition,A.Position,A.CBRole,A.CTRole,R.POSITION AS REQPOSITION,r.Entitlement,R.TechnicalRole,A.ConflictingRemarks,R.RequestId
	INTO #TBLEXISTSINGREQ
	FROM @tblConflictBRoles A inner join @tblExistingRequestEntitlements r ON A.TRole = R.TECHNICALROLE 
	where A.ENDPOINTNAME = r.ENDPOINTNAME 

	INSERT INTO @RESULT(EndpointName,RequestedRole,ConflictedRole,ConflictRemarks,[Source],[ReqRoleForSorting])
	SELECT A.ENDPOINTNAME,CONCAT(CASE WHEN A.FROMCurrentRequestPosition =1 THEN ' POSITION: '+A.Position+'(Technical Role)' ELSE ' ' END,A.CBRole,'</b> [',A.CTRole,']'),
	CONCAT(CASE WHEN LEN(LTRIM(RTRIM(ISNULL(A.REQPOSITION,'')))) > 0 THEN 'POSITION: '+A.REQPOSITION+' ' ELSE '' END,A.Entitlement,' [',A.TechnicalRole,']'),A.ConflictingRemarks,('Existing Request -Id:'+CAST(A.RequestId as varchar)+')'),A.CTRole
	FROM
	(
		SELECT A.* FROM #TBLEXISTSINGREQ A WHERE A.CTRole >A.TECHNICALROLE OR NOT EXISTS(SELECT 1 FROM #TBLEXISTSINGREQ B WHERE B.CTRole = A.TECHNICALROLE AND B.TECHNICALROLE = A.CTRole AND B.ENDPOINTNAME = A.ENDPOINTNAME)
	)AS A ORDER BY A.ENDPOINTNAME,A.TechnicalRole

			
	IF OBJECT_ID('tempdb..#TBLEXISTSINGREQ') IS NOT NULL
	BEGIN
			DROP TABLE #TBLEXISTSINGREQ
	END
	----CURRENT ACCESS----
	SELECT A.ENDPOINTNAME,A.FROMCurrentRequestPosition,A.Position,A.CBRole,A.CTRole,R.POSITION AS REQPOSITION,R.Entitlement,R.TECHNICALROLE,A.ConflictingRemarks
	INTO #TBLCURRENTACCESSREQ
	FROM @tblConflictBRoles A inner join @tblCurrentAccess r ON A.TRole = R.TECHNICALROLE 
	where A.ENDPOINTNAME = r.ENDPOINTNAME 
			
	INSERT INTO @RESULT(EndpointName,RequestedRole,ConflictedRole,ConflictRemarks,[Source],[ReqRoleForSorting])
	SELECT A.ENDPOINTNAME,CONCAT(CASE WHEN A.FROMCurrentRequestPosition =1 THEN ' POSITION: '+A.Position+'(Technical Role)' ELSE ' ' END,A.CBRole,'</b> [',A.CTRole,']'),
	CONCAT(CASE WHEN LEN(LTRIM(RTRIM(ISNULL(A.REQPOSITION,'')))) > 0 THEN 'POSITION: '+A.REQPOSITION+' ' ELSE '' END,A.Entitlement,' [',A.TechnicalRole,']'),A.ConflictingRemarks,'Current Access',A.CTRole
	FROM
	(
		SELECT A.ENDPOINTNAME,A.FROMCurrentRequestPosition,A.Position,A.CBRole,A.CTRole,A.REQPOSITION,A.Entitlement,A.TECHNICALROLE,A.ConflictingRemarks FROM #TBLCURRENTACCESSREQ A WHERE A.CTRole >A.TECHNICALROLE OR NOT EXISTS(SELECT 1 FROM #TBLCURRENTACCESSREQ B WHERE B.CTRole = A.TECHNICALROLE AND B.TECHNICALROLE = A.CTRole AND B.ENDPOINTNAME = A.ENDPOINTNAME)
	)AS A ORDER BY A.ENDPOINTNAME,A.TechnicalRole
			
	IF OBJECT_ID('tempdb..#TBLCURRENTACCESSREQ') IS NOT NULL
	BEGIN
			DROP TABLE #TBLCURRENTACCESSREQ
	END

	SELECT EndpointName,RequestedRole,ConflictedRole,ConflictRemarks,[Source] INTO #TBLFINALRESILTTBLEXT FROM @RESULT ORDER BY EndpointName DESC,[ReqRoleForSorting],Id
	
	
	SELECT DISTINCT EndpointName,RequestedRole,ConflictedRole,ConflictRemarks,[Source] FROM #TBLFINALRESILTTBLEXT

	IF OBJECT_ID('tempdb..#TBLFINALRESILTTBLEXT') IS NOT NULL
	BEGIN
			DROP TABLE #TBLFINALRESILTTBLEXT
	END		

	END TRY
	BEGIN CATCH
		
		INSERT INTO DI_Log_Provisioning([LogLevel], [Message], [ExceptionMessage], [StackTrace], [ExceptionSource], [InnerExceptionMessage], [LogDate], [CreatedBy], [AccessRequestId], [EndpointSystemTypeId], [EndpointSystemTypeName], [EndpointSystemId])
		VALUES(1,'ERROR OCCURED USP_GetSAPSODConflicts',@@ERROR, ERROR_MESSAGE(),ERROR_PROCEDURE(),ERROR_NUMBER(),GETDATE(),'System',null,null,null,null)
		
	END CATCH

END






GO