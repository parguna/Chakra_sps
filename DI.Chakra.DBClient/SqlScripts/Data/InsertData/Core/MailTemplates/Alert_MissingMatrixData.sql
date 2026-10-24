SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 

INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName]) VALUES (9, N'AlertNotification', N'AlertNotification', N'devteam@deepidentity.com', N'@@CONFIGURATIONMASTER_MAILTO_ALERT_REQ@@', NULL, NULL, N'Missing Provisioning Matrix Data - ##EVENT_REQ##', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html>
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
        <p>Dear Security Operations,</p>
        <br />
        <p>Provisioning mandatory information is missing for some of the default systems.  Please see the detailed information below.
        </p>
        <p>User : @@USERDISPLAYNAME_REQ@@  ( @@PERNRNO_REQ@@ )
		@@ORGCHARTLEVELS_USR@@
        <br />
        <br />
        </p>
        <p>Error Info : @@REQUEST_ERRORMESSAGE_DISC@@
        </p>
        <table style="margin-top:20px">
        <tr>
          <td><p>Best regards,<br/>
              IT ID Administration team</p>
            </td>
        </tr>
		<tr><td><p>*** This is a system-generated message. Please do not reply to this message.***</p></td></tr>
      </table></p>
    </body>
</html>', NULL, N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL, 2, 9)


SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] Off