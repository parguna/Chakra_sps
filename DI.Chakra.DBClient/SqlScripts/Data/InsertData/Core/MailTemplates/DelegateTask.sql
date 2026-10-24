SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 

INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName]) VALUES (27, N'DelegateAdhocProvisioningApproval', N'Delegate Adhoc Provisioning Approval', N'devteam@deepidentity.com', N'@@MAILTO_PRVTSK@@', NULL, NULL, 
   N'Pending Task (Delegated by - @@DELEGATED_USER@@) - @@APPLICATION_PRVTSK@@  - @@REQUESTTYPE_PRVTSK@@ - @@REQUESTNUMBER_PRVTSK@@ - @@USERDISPLAYNAME_PRVTSK@@ ( @@PERNRNO_PRVTSK@@ )', 
   N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html>
    <head>
        <title></title>
        <style type="text/css">
            .title {
            font-family: "lucida grande", tahoma, verdana, arial, sans-serif;
            font-size: 13px;
            color: #1c2a47;
            font-weight: bolder;
            }
            .body {
            font-family: "lucida grande", tahoma, verdana, arial, sans-serif;
            font-size: 12px;
            color: InfoText;
            font-weight: normal;
            margin-left: 50px;
            }
        </style>
    </head>
    <body class="body">
        <p>
        Dear @@APPROVER_PRVTSK@@,
        <br />
        <br />
        The following task has been delegated to you by @@DELEGATED_USER@@ and is pending for your approval.
        <br />        
		Delegation Comments: @@DELEGATER_COMMENTS@@
		<br />
		<br />
        Request ID: @@REQUESTNUMBER_PRVTSK@@
        <br />
        Request Name: @@APPLICATION_PRVTSK@@ - @@REQUESTTYPE_PRVTSK@@
        <br />
		Request Detail: @@ENTITLEMENT_PRV@@
		<br />
        Request For: @@USERDISPLAYNAME_PRVTSK@@
        <br />
        Employee ID: @@PERNRNO_PRVTSK@@
        <br />
        Account Type: @@ACCOUNTTYPE_PRVTSK@@
        <br />		
        Requester Comments: @@COMMENTS_PRVTSK@@
        <br />
        <br />
        Please click the below link to log in to IDMS portal to action on the task.
        <br />
        @@CONFIGURATIONMASTER_IACMURL_ATTTSK@@
        <br />
        <table style="margin-top:20px">
        <tr>
          <td><p>Best regards,<br/>
              IT ID Administration team</p>
            </td>
        </tr>
		<tr><td><p>*** This is a system-generated message. Please do not reply to this message.***</p></td></tr>
      </table>
        </p>
    </body>
</html>', NULL, N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL, 7, 27)

SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] OFF