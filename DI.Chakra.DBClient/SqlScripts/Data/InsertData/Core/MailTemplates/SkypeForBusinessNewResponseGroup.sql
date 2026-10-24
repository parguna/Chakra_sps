INSERT INTO DI_M_MailTemplate(TemplateName,TemplateDescription,MailFrom,MailTo,MailBcc,MailCC,MailSubject,MailContent,CreatedBy,CreatedDate)
VALUES('NotifyForNewResponseGroup','Skype for business: Send email on new responsegroup','securityadmin@singaporepower.com.sg','emailteam@spgroup.com.sg',null,null,'Discovery - @@ENDPOINTNAME@@ New ResponseGroup','<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html>
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
        <p>Dear IT ID Administration, </p>
        <br />
        <p>The following ResponseGroup(s) were discovered as part of Skype For Business discovery and added to the Provisioning Matrix. Kindly update the ADID and Level1/2/3Approver and Approver type .</p>
        <p><h4>Response Group(s):</h4></p>
        <p>@@RESPONSEGROUPNAMES@@<br />
        <h4>Application Type:</h4>@@ENDPOINTTYPE@@<br />
        <h4>Application:</h4> @@ENDPOINTNAME@@<br />
        <br />
        <br />
        <table style="margin-top:20px">
        <tr>
          <td><p>Best regards,<br/>
              IT ID Administration</p>
            </td>
        </tr>
		<tr><td><p>*** This is a system-generated message. Please do not reply to this message.***</p></td></tr>
      </table>
    </body>
</html>','System',GETDATE())
