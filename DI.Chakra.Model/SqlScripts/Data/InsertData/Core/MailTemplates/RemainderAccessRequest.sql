
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (N'RemindAdhocProvisioningApproval', N'Reminder Adhoc Provisioning Approval', N'devteam@deepidentity.com', NULL, N'Adhoc Provisioning Approval Request - {0}', 
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
   
        <p>Dear @@RECEIVER@@,<br/>
        <br/>
		This is a gentle reminder regarding the pending provisioning approval proces. You are requested to review the below request using IDMS
        <br/>
        <br/>
        Please log in to IDMS 
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
       IDMS Administrator    <p>
</div>
</body>
</html>', N'System', GETDATE(), NULL, NULL)