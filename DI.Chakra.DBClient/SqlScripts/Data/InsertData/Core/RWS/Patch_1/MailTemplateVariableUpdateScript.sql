--0
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Endpoint Notification Email Id' WHERE VariableScope = 0 AND VariableValue = '@@CONFIGURATION_ENDPOINT_NOTIFICATION@@'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'ITCC Team Text' WHERE VariableScope = 0 AND VariableValue = '##ITCC_TEAM##'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'IDAS Admin Text' WHERE VariableScope = 0 AND VariableValue = '##IDAS_ADMIN##'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'IT Admin Text' WHERE VariableScope = 0 AND VariableValue = '##IT_ADMIN##'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'ITCC Email Address' WHERE VariableScope = 0 AND VariableValue = '##ITCCEmail##'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'IDAS Email Address' WHERE VariableScope = 0 AND VariableValue = '##IDASEmail##'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'IT Admin Email Address' WHERE VariableScope = 0 AND VariableValue = '##ITAdmin##'

--2
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Requestee Display Name' WHERE VariableScope = 2 AND VariableValue = '@@USERDISPLAYNAME_REQ@@'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Requestee First Name' WHERE VariableScope = 2 AND VariableValue = '@@USERFIRSTNAME_REQ@@'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Requestee Last Name' WHERE VariableScope = 2 AND VariableValue = '@@USERLASTNAME_REQ@@'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Requestee Supervisor Name' WHERE VariableScope = 2 AND VariableValue = '##SUPERVISOR_NAME##'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Requestee Supervisor Email' WHERE VariableScope = 2 AND VariableValue = '##MAILTO_SUPERVISOR##'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Requestee Employee Number' WHERE VariableScope = 2 AND VariableValue = '@@PERNRNO_REQ@@'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Request Date' WHERE VariableScope = 2 AND VariableValue = '##DATE_REQ##'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Request Subsystem Name' WHERE VariableScope = 2 AND VariableValue = '@@APPLICATION_REQ@@'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Requestee Entitlement' WHERE VariableScope = 2 AND VariableValue = '@@ENTITLEMENT_REQ@@'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Requestee Email Id' WHERE VariableScope = 2 AND VariableValue = '@@MAILTO_REQ@@'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Failure Notification Email Id' WHERE VariableScope = 2 AND VariableValue = '@@CONFIGURATIONMASTER_MAILTO_REQ@@'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Alert Notification Email Id' WHERE VariableScope = 2 AND VariableValue = '@@CONFIGURATIONMASTER_MAILTO_ALERT_REQ@@'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Entitlement Requested Info Header' WHERE VariableScope = 2 AND VariableValue = '##ENTITLEMENT_REQ_INFO##'

IF NOT EXISTS(SELECT * FROM DI_M_MailTemplateVariable WHERE VariableValue = '@@REQ_BY_DEPT@@' AND VariableScope = 2 AND DisplayName='Requester Department')
BEGIN
INSERT [dbo].[DI_M_MailTemplateVariable] ([DisplayName], [VariableValue], [VariableValuePath1], [VariableValuePath2], [VariableScope], [IsActive],[CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'Requester Department', N'@@REQ_BY_DEPT@@', N'Requester.NOrganisation.Name', N'', 2, 1,N'System', GETDATE(), N'System', GETDATE())
END

IF NOT EXISTS(SELECT * FROM DI_M_MailTemplateVariable WHERE VariableValue = '@@REQ_FOR_DEPT@@' AND VariableScope = 2 AND DisplayName='Requestee Department')
BEGIN
INSERT [dbo].[DI_M_MailTemplateVariable] ([DisplayName], [VariableValue], [VariableValuePath1], [VariableValuePath2], [VariableScope], [IsActive],[CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'Requestee Department', N'@@REQ_FOR_DEPT@@', N'User.NOrganisation.Name', N'', 2, 1,N'System', GETDATE(), N'System', GETDATE())
END

--3
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'User Employee Number' WHERE VariableScope = 3 AND VariableValue = '@@PERNRNO_USR@@'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'User Email Id' WHERE VariableScope = 3 AND VariableValue = '@@MAILTO_USR@@'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Alert Notification Email Id' WHERE VariableScope = 3 AND VariableValue = '@@CONFIGURATIONMASTER_MAILTO_ALERT_USR@@'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Account Creation Notification Email Id' WHERE VariableScope = 3 AND VariableValue = '@@CONFIGURATIONMASTER_MAILTO_ACCOUNTCREATED_USR@@'

--5
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Endpoint Owner Email Id' WHERE VariableScope = 5 AND VariableValue = '@@MAILTO_DISC@@'

--6
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Endpoint Owner Email Id' WHERE VariableScope = 6 AND VariableValue = '##MAILTO_ATTE##'

--7
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Requestee Display Name' WHERE VariableScope = 7 AND VariableValue = '@@USERDISPLAYNAME_PRVTSK@@'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Requestee Employee Number' WHERE VariableScope = 7 AND VariableValue = '@@PERNRNO_PRVTSK@@'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Approver Email Id' WHERE VariableScope = 7 AND VariableValue = '@@MAILTO_PRVTSK@@'

IF NOT EXISTS(SELECT * FROM DI_M_MailTemplateVariable WHERE VariableValue = '@@TSK_REQ_BY_DEPT@@' AND VariableScope = 7 AND DisplayName='Requester Department')
BEGIN
INSERT [dbo].[DI_M_MailTemplateVariable] ([DisplayName], [VariableValue], [VariableValuePath1], [VariableValuePath2], [VariableScope], [IsActive],[CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'Requester Department', N'@@TSK_REQ_BY_DEPT@@', N'AccessRequest.Requester.NOrganisation.Name', N'', 7, 1,N'System', GETDATE(), N'System', GETDATE())
END

IF NOT EXISTS(SELECT * FROM DI_M_MailTemplateVariable WHERE VariableValue = '@@TSK_REQ_FOR_DEPT@@' AND VariableScope = 7 AND DisplayName='Requestee Department')
BEGIN
INSERT [dbo].[DI_M_MailTemplateVariable] ([DisplayName], [VariableValue], [VariableValuePath1], [VariableValuePath2], [VariableScope], [IsActive],[CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'Requestee Department', N'@@TSK_REQ_FOR_DEPT@@', N'AccessRequest.User.NOrganisation.Name', N'', 7, 1,N'System', GETDATE(), N'System', GETDATE())
END

IF NOT EXISTS(SELECT * FROM DI_M_MailTemplateVariable WHERE VariableValue = '##SUPERVISOR_NAME_TSK##' AND VariableScope = 7 AND DisplayName='Requestee Supervisor Name')
BEGIN
INSERT [dbo].[DI_M_MailTemplateVariable] ([DisplayName], [VariableValue], [VariableValuePath1], [VariableValuePath2], [VariableScope], [IsActive],[CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'Requestee Supervisor Name', N'##SUPERVISOR_NAME_TSK##', N'', N'', 7, 1,N'System', GETDATE(), N'System', GETDATE())
END

IF NOT EXISTS(SELECT * FROM DI_M_MailTemplateVariable WHERE VariableValue = '##MAILTO_REQUESTEE_SUPERVISOR_TSK##' AND VariableScope = 7 AND DisplayName='Requestee Supervisor Email')
BEGIN
INSERT [dbo].[DI_M_MailTemplateVariable] ([DisplayName], [VariableValue], [VariableValuePath1], [VariableValuePath2], [VariableScope], [IsActive],[CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'Requestee Supervisor Email', N'##MAILTO_REQUESTEE_SUPERVISOR_TSK##', N'', N'', 7, 1,N'System', GETDATE(), N'System', GETDATE())
END

--8
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Requestee Display Name' WHERE VariableScope = 8 AND VariableValue = '@@USERDISPLAYNAME_ATTTSK@@'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Requestee Employee Number' WHERE VariableScope = 8 AND VariableValue = '@@PERNRNO_ATTTSK@@'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Requester Employee Number' WHERE VariableScope = 8 AND VariableValue = '@@REQUESTERID_ATTTSK@@'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Approver Email Id' WHERE VariableScope = 8 AND VariableValue = '@@MAILTO_ATTTSK@@'
UPDATE DI_M_MailTemplateVariable SET DisplayName = 'Approver Email Id' WHERE VariableScope = 8 AND VariableValue = '##MAILTO_ATTTSK##'


