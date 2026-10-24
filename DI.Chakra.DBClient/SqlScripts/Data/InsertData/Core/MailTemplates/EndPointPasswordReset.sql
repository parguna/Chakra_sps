SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 

INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName]) VALUES (34, N'EndPointPasswordReset', N'EndPoint PasswordReset Notification', N'devteam@deepidentity.com', N'@@MAILTO_REQ@@', NULL, NULL, N'Password Reset -@@ACCOUNTNAME_REQ@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html>
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
        <p>Dear @@ACCOUNTNAME_REQ@@,
        <br />
        <br />
        This is to inform you that you have been transfered to a new organisation unit.
        <br />
        <br />
		Your access have been updated based on your new organisation unit.&nbsp;</p>
		<p>Application :&nbsp;@@APPLICATION_REQ@@</p>
		<p>UserName:&nbsp;@@ACCOUNTNAME_REQ@@</p>
		<p>Password:&nbsp;@@ACCOUNTPASSWORD_REQ@@</p>
		<p>&nbsp;</p>
		<p ><br />
        <table style="margin-top:20px">
        <tr>
          <td><p>Best regards,<br/>
              IT ID Administration team</p>
            </td>
        </tr>
		<tr><td><p>*** This is a system-generated message. Please do not reply to this message.***</p></td></tr>
      </table></p>
    </body>
</html>', NULL, N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL, 2, 34)

SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] OFF