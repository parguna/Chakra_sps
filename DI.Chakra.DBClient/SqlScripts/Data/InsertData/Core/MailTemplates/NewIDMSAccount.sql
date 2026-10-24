SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName]) VALUES (11, N'NotificationForNewUser', N'Notification for New User', N'devteam@deepidentity.com', N'@@CONFIGURATIONMASTER_MAILTO_ACCOUNTCREATED_USR@@', NULL, NULL, N'IDMS User Profile @@USERDISPLAYNAME_USR@@ (@@PERNRNO_USR@@) created under your Organisation Unit', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html>
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
        Dear ##SUPERVISOR_NAME##
        <br />
        <br />
        IDMS User Profile has been successfully created for the following user.
        <br />
        <br />
        User&nbsp;:&nbsp;@@USERDISPLAYNAME_USR@@&nbsp;(@@PERNRNO_USR@@)
        @@ORGCHARTLEVELS_USR@@
        <br />
        <br />
        Please note that non-employees are not allowed to raise request in IDMS. Only designated co-ordincators can apply access on behalf of non-employees.
        <br />
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
</html>', NULL, N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL, 0, 11)

SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] Off