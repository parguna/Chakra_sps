CREATE   TRIGGER [dbo].[DI_M_Organisation_DELETE_Trigger] ON [dbo].[DI_M_Organisation]  AFTER DELETE
AS
BEGIN
SET NOCOUNT ON;

      DECLARE @EndPointSystemId  int
	  DECLARE @EndPointSystemTypeId  int
      DECLARE @HistoryId Int
     
      --SELECT  @EndPointSystemId =EndPointSystemId FROM DELETED
	  SELECT @EndPointSystemTypeId =(SELECT EndpointSystemTypeId FROM DI_M_EndpointSystemType where EndpointSystemTypeName = 'SAP')
	  SELECT @EndPointSystemId =(SELECT EndPointSystemId FROM DI_M_EndpointSystem where EndpointSystemTypeId = @EndPointSystemTypeId and IsTrustedSource = 1)
      SET @HistoryId=0
               
      INSERT INTO [dbo].[DI_M_OrganisationHistory]
					([OrganisationID]
					,
					,[HistoryId]
					,[OrgUnitPrefix]
					,[OrganisationName]
					,[ParentOrganisation]
					,[HOD]
					,[OrganisationHeadUserId]
					,[DivisionRepresentative]
					,[DivisionRepresentativeUserId]
					,[CreatedBy]
					,[CreatedDate]
					,[LastModifiedBy]
					,[LastModifiedDate]
					,[RecordStatus])
     SELECT [OrganisationID]
					,
					,@HistoryId
					,[OrgUnitPrefix]
					,[OrganisationName]
					,[ParentOrganisation]
					,[HOD]
					,[OrganisationHeadUserId]
					,[DivisionRepresentative]
					,[DivisionRepresentativeUserId]
					,[CreatedBy]
					,[CreatedDate]
					,[LastModifiedBy]
					,[LastModifiedDate]
					,'Deleted'
        FROM deleted

END
