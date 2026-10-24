
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES (N'NotifyAdhocProvisioningApproval', N'Notify Adhoc Provisioning Approval', N'devteam@deepidentity.com', NULL, N'Pending Task - @@applicationName@@  - @@requestType@@ - @@requestNum@@ - @@userDisplayName@@ (@@pernrNum@@)',
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

    <p>
      Dear @@approverName@@,
      <br/>
      <br/>
      The following task is pending your approval.
      <br/>
      <br/>
      Request ID: @@requestNum@@
      <br/>
      Request Name: @@applicationName@@ - @@requestType@@
      <br/>
      Request For: @@userDisplayName@@
      <br/>
      Employee ID: @@pernrNum@@
      <br/>
      Account Type: @@AccountType@@
      <br/>
	  Requester Comments: @@COMMENTS@@
      <br/>
      <br/>
      Please click the below link to log in to IDMS portal to action on the task.
      <br/>
	  @@IACMURL@@
      <br/>
      Regards,
      <br/>
      IDMS Administrator.
      <br/>
      <br/>
      *** This is a system-generated message. Please do not reply to this message.***
    </p>


  </body>
</html>', N'System', GETDATE(), NULL, NULL)


GO