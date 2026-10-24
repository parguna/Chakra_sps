SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], 
[LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName], [ActivityType]) 
VALUES (30, N'AttestationJobFailed', N'This is an email notification to IDADMIN about attestation job failed with the error message.', N'devteam@deepidentity.com', N'ITIDAdministrationOperationTeam@RWSentosa.com', N'##MAILTO_ATTE##,itidadmin@rwsentosa.com', NULL, 
N'Attestation Job Failed - ##ATT_ENDPOINTNAME_DISP## ( @@PROCESSNAME_ATTE@@ ) ', 
N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
    <head>
        <title>IDAS - Attestation Job Failed</title>
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
	       
       
        <span>Dear Dear IT ID Administration,</span></p>
       
        <span >Attestation Process Failed</span></p>
        <p>
        <span >Error Message:</span></p>
      
        <p>
        <span class="content">@@ERRORMESSAGE_ATTE@@</span></p>
        <p>
        <span class="title">Endpoint system details:</span></p>
      
       
        <table  style="width: 100%;">
            <tbody>
                <tr>
                    <td>
                    Application Type:</td>
                    <td>##ATT_ENDPOINTTYPE_DISP##</td>
                </tr>
                <tr>
                    <td>
                   Application:</td>
                    <td>##ATT_ENDPOINTNAME_DISP##</td>
                </tr>
                <tr>
                    <td >
                   Process Name: </td>
                    <td>@@PROCESSNAME_ATTE@@</td>
                </tr>
            </tbody>
        </table>
		<br />
        <br />
        <table style="margin-top:20px">
        <tr>
          <td><p>Best regards,<br/>
              IT ID Administration</p>
            </td>
        </tr>
		<tr><td><p>*** This is a system-generated message. Please do not reply to this message.***</p></td></tr>
      </table></p>
    </body>
</html>', NULL, N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL, 6, 30, 2)
SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] OFF