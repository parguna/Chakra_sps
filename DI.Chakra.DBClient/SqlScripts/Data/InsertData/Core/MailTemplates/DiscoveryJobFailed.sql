SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName]) 
VALUES (6, N'DiscoveryJobFailed', N'This email is sent when a discovery job had failed.', N'devteam@deepidentity.com', N'ITIDAdministrationOperationTeam@RWSentosa.com,@@MAILTO_DISC@@', N'itidadmin@rwsentosa.com', NULL, 
N'IDAS - Discovery Job Failed - @@ENDPOINTNAME_DISC@@ ( @@PROCESSNAME_DISC@@ ) ',
 N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
    <head>
        <title>IDAS - Discovery Job Failed</title>
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

		<table>
			<tr>
			  <td ><p>Dear IT ID Administration,</p>
				<p><span class="title">Discovery Process Job Failed</span></p>
        <p><span class="title">Error Message:</span></p>
       
        <p>
        <span class="content">@@ERRORMESSAGE_DISC@@</span></p>
        <p>
		</td>
		</tr>

		<tr>
		<td >
        <span class="title">Endpoint system details:</span></p>
       
        <table border="0" cellpadding="5" cellspacing="0" width="300" bordercolor="#ffffff">
            <tbody>
                <tr>
                    <td class="style">
                    <strong>Application Type:</strong></td>
                    <td>@@ENDPOINTTYPE_DISC@@</td>
                </tr>
                <tr>
                    <td class="style">
                    <strong>Application:</strong></td>
                    <td>@@ENDPOINTNAME_DISC@@</td>
                </tr>
                <tr>
                    <td class="style">
                    <strong>Process Name: </strong></td>
                    <td>@@PROCESSNAME_DISC@@</td>
                </tr>
            </tbody>
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
</html>', NULL, N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL, 5, 6)
SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] OFF