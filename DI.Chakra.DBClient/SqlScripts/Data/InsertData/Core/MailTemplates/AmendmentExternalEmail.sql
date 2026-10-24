SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName]) VALUES (2, N'AdhocProvisioningAmendmentExternalMail', N'AdhocProvisioningAmendmentExternalEmail', N'devteam@deepidentity.com', N'@@MAILTO_REQ@@', NULL, NULL, N'Request Completed - External Email  - @@REQUESTTYPE_REQ@@ - @@REQUESTNUMBER_REQ@@ - @@USERDISPLAYNAME_REQ@@  ( @@PERNRNO_REQ@@ ) - ( @@ACCOUNTNAME_REQ@@ )', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html>
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
        <p> Dear @@USERDISPLAYNAME_REQ@@,
        <br />
        <br />
        The following request has been executed.
        <br />
        <br />
        Request Details:<br />
        --------------------------------------------------------------------
        <br />
        <br />
        Request ID: @@REQUESTNUMBER_REQ@@
        <br />
        Request Name: External Email - @@REQUESTTYPE_REQ@@
        <br />
        Request For: @@USERDISPLAYNAME_REQ@@
        <br />
        Employee ID: @@PERNRNO_REQ@@
        <br />
        Login ID: @@ACCOUNTNAME_REQ@@
        <br />
        <br />
        Access Name : External Email
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
</html>', NULL, N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL, 2, 2)
SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] Off