
INSERT [dbo].[DI_M_MailTemplate] ( [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], 
[CreatedBy], [CreatedDate]) 
VALUES ( N'DelegateAttestationTask', N'This email is sent to the delegated user that is delegated by the approver for the attestation task.',
 N'devteam@deepidentity.com', N'@@MAILTO_ATTTSK@@', N'itidadmin@rwsentosa.com', NULL, 
N'Attestation Review Task (Delegated by - @@DELEGATED_USER@@) - @@ATTESTATION_NAME_ATTTSK@@', 
N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html>
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
        Dear ##APPROVER_ATTTSK##,
        <br />
        <br />
        The following task has been delegated to you by @@DELEGATED_USER@@ and is pending for your approval.
        <br />        
		Delegation Comments: @@DELEGATER_COMMENTS@@
		<br />
		Application(s) being reviewed:
		<br />
		##ATTTSK_ENDPOINTNAME_DISP##
		<br />
        Attestation Name: @@ATTESTATION_NAME_ATTTSK@@
        <br />
        Attestation Description: @@ATTESTATION_DESC_ATTTSK@@
        <br />
        Attestation Campaign Name: @@ATTTSK_CAMPAIGN_NAME@@
        <br />
        Last date for submission: ##REVIEWLASTDATE_ATTTSK##.
        <br />
        <br />
         Please log into <a href="@@CONFIGURATIONMASTER_IACMURL_ATTTSK@@">IDAS</a> to take action on the task.
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
</html>',  N'IACMAdmin', SYSDATETIME())

