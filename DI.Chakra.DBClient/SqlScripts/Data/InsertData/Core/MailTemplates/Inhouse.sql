SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 

INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName], [ActivityType]) 
VALUES (26, N'InHouseAccountCreation', N'InHouse Account Creation', N'devteam@deepidentity.com', N'##ITCCEmail##', NULL, NULL, N'InHouse Account Creation For @@INHOUSE_ENDPOINTNAME@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html>
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
        Dear ##IDAS_ADMIN##<br />
        <br />
        INHOUSE ACCOUNT&nbsp; SUBMITTED BY @@INHOUSE_ACCOUNT_CREATEDBY@@</span></p>
        <p >
        Request Details:<br />
        --------------------------------------------------------------------<br />
        Account Name :@@ACCOUNTNAME_INHOUSE@@</p>
        <p>Account Password :@@ACCOUNTPASSWORD_INHOUSE@@</p>
        <p>Account Type&nbsp; :@@ACCOUNTTYPE_INHOUSE@@</p>
        <p>Mapped To User Name :##USERDISPLAYNAME_INHOUSE##<br />
		<p>Mapped To User Email :##INHOUSE_OWNER_EMAIL##<br />
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
</html>', NULL, N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL, 9, 26, 6)

SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] off