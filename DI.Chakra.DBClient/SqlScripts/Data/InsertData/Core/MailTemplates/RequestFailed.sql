SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 

INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName]) 
VALUES (21, N'AccessRequestFailureNotification', N'This email is sent to the IDADMIN to notify OLR execution failure and the same information will be updated in the alert center.',
 N'devteam@deepidentity.com',
  N'ITIDAdministrationOperationTeam@RWSentosa.com',  N'itidadmin@rwsentosa.com', NULL, N'Execution Failed - @@APPLICATION_REQ@@  - @@REQUESTTYPE_REQ@@ - @@REQUESTNUMBER_REQ@@',
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
        <p>Dear IT ID Administration, </p>
        <br />
        <p>The following request has failed to execute. Please view the error details from the Log viewer.</p>
        <p>Request Details:<br />
        -------------------------------------------------------------------</p>
        <p> Request ID: @@REQUESTNUMBER_REQ@@<br />
        Request Name: @@APPLICATION_REQ@@ - @@REQUESTTYPE_REQ@@<br />
        Request For: @@USERDISPLAYNAME_REQ@@<br />
        Employee ID: @@PERNRNO_REQ@@<br />
        Execution Failure Date: ##DATE_REQ##<br />
        <br />
        <br />
        <table style="margin-top:20px">
        <tr>
          <td><p>Best regards,<br/>
              IT ID Administration</p>
            </td>
        </tr>
		<tr><td><p>*** This is a system-generated message. Please do not reply to this message.***</p></td></tr>
      </table>
    </body>
</html>', NULL, N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL, 2, 21)

SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] OFF