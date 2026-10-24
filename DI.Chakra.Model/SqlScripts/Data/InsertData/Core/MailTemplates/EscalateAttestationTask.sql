
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'EscalateAttestationApproval', N'Escalate Attestation Approval', N'devteam@deepidentity.com', NULL, N'Attestation - Approval Level {0} Escalation', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
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
<body>
    <div>
        Dear @@RECEIVER@@,
      <br/>
      <br/>
        This is a escalation email to inform you that @@REVIEWER_NAME@@ didn&#39;t complete the attestation approval process.
      <br/>
        You are requested to take necessary steps to ensure the attestation process is complete.
      <br/>
		Please log in to IDMS 
      <br/>
        Attestation Name: @@ATTESTATION_NAME@@
      <br/>
        Attestation Description: @@ATTESTATION_DESC@@
      <br/>
        Required Action: Attestation - Approval Level @@APPROVAL_LEVEL@@
      <br/>
        Last date for submission: @@REVIEWLASTDATE@@.
      <br/>
        Regards
      <br/>
       IDMS Administrator  
	   <br/>
      <br/>
      *** This is a system-generated message. Please do not reply to this message.*** </p>  
</div>
</body>
</html>', N'System', GETDATE(), NULL, NULL)