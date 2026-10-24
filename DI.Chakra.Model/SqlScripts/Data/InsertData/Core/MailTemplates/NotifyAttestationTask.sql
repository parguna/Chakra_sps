
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'NotifyAttestationApproval', N'Notify Attestation Approval', N'devteam@deepidentity.com', NULL, N'Attestation - Approval Level {0} Request', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
  <head>
    <title></title>
    <style type="text/css">
      .body {
      font-family: Courier New;
      font-size: 13px;
      font-weight: normal;
      margin-left: 5px;
      }

      .marlef {
      margin-left: 40px;
      }
      .marlef1 {
      margin-left: 50px;
      }
      .marlef2 {
      margin-left: 50px;
      }
    </style>
  </head>
  <body class="body">


    <p>   Dear @@RECEIVER@@,
	   <br/>
	   <br/>
       Please click the below link to log in to IDMS portal to action on the task.
      <br/>
	  @@IACMURL@@
      <br/>
	   <br/>
        Attestation Name: @@ATTESTATION_NAME@@
	   <br/>
        Attestation Description: @@ATTESTATION_DESC@@
	   <br/>
        Required Action: Attestation - Approval Level @@APPROVAL_LEVEL@@
	   <br/>
        Last date for submission: @@REVIEWLASTDATE@@.
	   <br/>
	   <br/>
      Regards
	   <br/>
      IDMS Administrator
	   <br/>
      <br/>
      *** This is a system-generated message. Please do not reply to this message.*** </p>
</body>
</html>', N'System', GETDATE(), NULL, NULL)