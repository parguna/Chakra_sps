
SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName]) 
VALUES (46, N'InstallationNotification', N'This email is sent when a client installation is required for a Sub-System', 
N'devteam@deepidentity.com', 
N'ITIDAdministrationOperationTeam@RWSentosa.com', 
N'itidadmin@rwsentosa.com', NULL, 
N'IDAS - Installation Notification for @@USERLASTNAME_REQ@@ @@USERFIRSTNAME_REQ@@', 
N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html>
    <head>
        <title>IDAS - Installation Notification for @@USERLASTNAME_REQ@@ @@USERFIRSTNAME_REQ@@</title>
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

		<table style="font-family:arial;">
			<tr>
			  <td ><p>Dear IT Service Desk,</p>
				<p> This email is to notify you to install the required software needed to run the subsystem <b>@@APPLICATION_REQ@@</b>.</p>
				<p>For Employee <b>@@USERLASTNAME_REQ@@ @@USERFIRSTNAME_REQ@@</b></p>
<PRE>
Employee Id: @@PERNRNO_REQ@@
Display Name: @@USERDISPLAYNAME_REQ@@
AD ID: @@REQUESTEE_ADID@@
Email Id: @@MAILTO_REQ@@
</PRE>
				</td>
			</tr>
			
		</table>        
        
	</td>
	</tr>
    
	<tr>
    <td><!-- Message Footer -->
      
      <table style="margin-top:20px">
        <tr>
          <td><p>Best regards,<br/>
              IT ID Administration team</p>
            </td>
        </tr>
		<tr><td><p>*** This is a system-generated message. Please do not reply to this message.***</p></td></tr>
      </table></td>
  </tr>
</table>
</body>
</html>', N'Notification', N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL, 2, 46)

SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] Off