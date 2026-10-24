SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName]) VALUES (33, N'IACMNewUserNotification', N'Notification containing login credentials for new IACM user', N'devteam@deepidentity.com', N'@@EMAILID_USR@@', NULL, NULL, N'New User @@USERDISPLAYNAME_USR@@ - Credentials', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html>
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
        <p>PRIVATE &amp; CONFIDENTIAL<br />
        -------------------------------------------------------------------<br />
        To:&nbsp;&nbsp;@@USERDISPLAYNAME_USR@@</p>
        <div class="marlef">
        IACM - UserName and Password Created<br /></div>
        <p>&nbsp;</p>
        <p>1. The following User ID has been created :-
        </p>
        <div class="marlef">
        User Name		  : @@USERNAME_USR@@<br />
        Password	  : @@PASSWORD_USR@@<br />
		</div>
        <p>&nbsp;</p>
        <p>2. Please perform the following steps:
        </p>
        <div class="marlef">
        (i) Sign on to the application using the login ID and password given above and immediately change the password to one of your choice before using the system.<br />
        (ii) Your password must have the following characteristics:<br />
        <div class="marlef1">
        ##PASSWORD_POLICY_USER##</div>
        </div>
        </div>
        <p>&nbsp;</p>
        <p>
        3. Please ensure that your password is kept confidential and that you do not grant permission to any person to use your User ID or password.
        Please also ensure that your terminal is not left unattended after you have signed on to the system
        </p>
        <p>4. Should you have any query, please do not hesitate to contact Helpdesk at 6378 8111.</p>

		<table style="margin-top:20px">
        <tr>
          <td><p>Best regards,<br/>
              IT ID Administration team</p>
            </td>
        </tr>
		<tr><td><p>*** This is a system-generated message. Please do not reply to this message.***</p></td></tr>
      </table>
    </body>
</html>', NULL, N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL, 3, 33)


SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] Off