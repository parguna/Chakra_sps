SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 

INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName]) VALUES (18, N'PasswordChangeNotification', N'Password Change Notification', N'devteam@deepidentity.com', N'##SELF_SERVICE_USR_EMAIL##', NULL, NULL, N'Password reset in @@ENDPOINT_ACCREC@@ - for user @@USERDISPLAYNAME_ACCREC@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html>
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
        <p>Dear @@USERDISPLAYNAME_ACCREC@@ ,
		
        <br />
        <br />
        This is to inform you that your password Change is successful for  @@USR_ACCOUNT_NAME_ACCREC@@ in @@ENDPOINT_ACCREC@@ - on ##DATE_REQ##.
        <br />
        <br />
        <table style="margin-top:20px">
        <tr>
          <td><p>Best regards,<br/>
              IT ID Administration team</p>
            </td>
        </tr>
		<tr><td><p>*** This is a system-generated message. Please do not reply to this message.***</p></td></tr>
      </table></p>
    </body>
</html>', NULL, N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL, 0, 18)

SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] OFF