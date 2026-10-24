
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (N'EscalateAdhocProvisioningApproval', N'Escalate Adhoc Provisioning Approval', N'devteam@deepidentity.com', NULL, N'Adhoc Provisioning Approval Request - {0} - Escalation', 
N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
       <p> Dear @@RECEIVER@@,
        <br/>
        This is a escalation email to inform you that @@REVIEWER_NAME@@ didn&#39;t complete the provisioning approval process.
        <br/>
        You are requested to take necessary steps to ensure the provisioning process is complete.
        <br/>
        <br/>
		Please log in to IDMS 
        <br/>
        <br/>
		Application: @@APPLICATION_NAME@@
        <br/>
        Requester Name: @@REQUESTER@@
        <br/>
        Date of Request: @@REQUESTEDDATE@@
        <br/>
        Request: @@REQUEST_DETAIL@@
        <br/>
        Last date for submission: @@REVIEWLASTDATE@@.
        <br/>
        <br/>
       Regards
        <br/>
       IDMS Administrator
    </div>
</body>
</html>', N'System', GETDATE(), NULL, NULL)

