CREATE NONCLUSTERED INDEX [NINXindex_DI_M_AdHocAccessRequest] ON [dbo].[DI_M_AdHocAccessRequest]
(
	[EndpointSystemId] ASC,
	[Id] ASC,
	[RequesteeUserId] ASC,
	[RequesterUserId] ASC
)
INCLUDE ( 	[AdhocActionType],
	[AccountType],
	[Comments]) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX [NINX_WFConfigID] ON [dbo].[DI_M_WorkflowConfigurationDetail]
(
	[WorkflowConfigurationId] ASC
)
INCLUDE ( 	[SequenceNumber]) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [NINX_ACCESREQUESTWITHADHOC] ON [dbo].[DI_M_AccessRequest]
(
	[AdHocAccessRequestId] ASC,
	[EndpointSystemId] ASC,
	[UserId] ASC,
	[Id] ASC,
	[RequestStatus] ASC
)
INCLUDE ( 	[RequestSource])  ON [PRIMARY]
 
 GO
 CREATE NONCLUSTERED INDEX [Nindex_DI_M_AdHocAccessRequest] ON [dbo].[DI_M_AdHocAccessRequest]
(
	[EndpointSystemId] ASC,
	[Id] ASC,
	[RequesteeUserId] ASC,
	[RequesterUserId] ASC
)
INCLUDE ( 	[AdhocActionType],
	[AccountType],
	[Comments])  ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [NINX_Campaign]
ON [dbo].[DI_M_AttestationSnapshot] ([AttestationCampaignId])

GO


CREATE NONCLUSTERED INDEX [NINX_AttestationSnapshotActionInterim_AttestationSnapshotActionId]
ON [dbo].[DI_M_AttestationSnapshotActionInterim] ([AttestationSnapshotActionId])



CREATE NONCLUSTERED INDEX [NINX_AttestationSnapshot_AttestationCampaignId]
ON [dbo].[DI_M_AttestationSnapshot] ([AttestationCampaignId])

GO
CREATE NONCLUSTERED INDEX [NINX_SnapshotAction]
ON [dbo].[DI_M_AttestationSnapshotAction] ([AttestationSnapshotId])
INCLUDE ([TaskId],[ReviewerId])