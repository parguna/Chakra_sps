SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 

INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName]) 
VALUES (28, N'DelegateBulkProvisioningApproval', N'This email is sent every time a task is assigned to the delegated user.',
 N'devteam@deepidentity.com', N'@@MAILTO_PRVTSK@@', N'itidadmin@rwsentosa.com', NULL, N'Pending Task - Delegated by - @@DELEGATED_USER@@', 
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
        Request ID: @@REQUESTNUMBERS@@
        <br />
      
       <p> Please log into <a href="@@CONFIGURATIONMASTER_IACMURL_ATTTSK@@">IDAS</a> to take action on the task. </p>
        <br />
        <table style="margin-top:20px">
        <tr>
          <td><p>Best regards,<br/>
              IT ID Administration </p>
            </td>
        </tr>
		<tr><td><p>*** This is a system-generated message. Please do not reply to this message.***</p></td></tr>
      </table>
        </p>
    </body>
</html>', NULL, N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL, 7, 28)

SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] OFF