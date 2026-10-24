SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 

INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName], [ActivityType]) 
VALUES (45, N'ExtractionRemainderNotice', N'This email is sent every 3 days (configurable), after the extraction notice is sent, to the respective application services team until the required Access Rights Listing is uploaded or completed.', N'devteam@deepidentity.com', N'@@EP_ITLEAD_MAIL_ID@@,@@EP_IT_DIST_MAIL_ID@@', N'##ITCCEmail##,itidadmin@rwsentosa.com', NULL, N'IDAS - REMINDER: Extract and Upload Access Rights Listing for ##ATTESTATION_ENDPOINTNAME_DISC##', 
N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html>
    <head>
        <title>IDAS - REMINDER: Extract and Upload Access Rights Listing for ##ATTESTATION_ENDPOINTNAME_DISC##</title>
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
	<table style="font-family:arial;">
	<tr>
    <td ><!-- Message start -->
        <p>
        Dear Application Services Team,
        <br />
		<br />
		Please be reminded to extract and upload the Access Rights Listing for ##ATTESTATION_ENDPOINTNAME_DISC##. The following is a recap of our earlier email.
		<br />
<PRE>	
1.	As part of the requirement of <u>User Access Management & Control Policy</u>, we will soon be conducting a periodic ##ATTESTATION_TYPE## Review for ##ATTESTATION_ENDPOINTNAME_DISC##.
2.	Please assist with the information extraction, and follow the steps below to upload the extracted Access Rights Listing:
	a.	Logon to <a href="@@CONFIGURATIONMASTER_IACMURL_ATTTSK@@">IDAS</a>.
	b.	1.Go to Administrator -> Configuration -> In-house Systems menu . 
		2.Under In-house Systems menu Account, Permission, Function and Privilege menus are present please go to corresponding menus and upload the needed files.
	c.	<b>Complete the uploading by ##ATTESTATION_STARTDATE##</b>.
</PRE>
		You may contact us should you require any clarification.
        <br />
		</p>
	</td>
	</tr>
    <tr>
    <td><!-- Message Footer -->
      
      <table style="margin-top:20px">
        <tr>
          <td><p>Best regards,<br/>
              IT Account Review</p>
            </td>
        </tr>
		<tr><td><p>*** This is a system-generated message. Please do not reply to this message.***</p></td></tr>
      </table></td>
  </tr>
</table>
</body>
</html>', NULL, N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL, 4, 45, 2)

SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] OFF