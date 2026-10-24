IF NOT EXISTS(SELECT 1 FROM [dbo].[DI_M_MailTemplate] WHERE [TemplateName]='NotifyInactiveEmployeeEvent')
BEGIN
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'NotifyInactiveEmployeeEvent', N'Notify the ITSecurity Team for inactive trigger', N'securityadmin@spgroup.com.sg', N'', NULL, NULL, N'IDMS: Warning: EmployeeEvent - @@EVENTNAME@@ exist as Inactive for Employee(@@EMPLOYEEID@@) EffectiveDate: @@EFFECTIVEDATE@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
    <head>
        <title>NotifyInactiveEmployeeEvent</title>
        <style type="text/css">
            .body {              font-family: Courier New;              font-size: 13px;              font-weight: normal;              margin-left: 5px;              }              .marlef {              margin-left: 40px;              }              .marlef1 {              margin-left: 50px;              }              .marlef2 {              margin-left: 50px;              }
        </style>
    </head>
    <body class="body" jquery191031130699542479323="28" jquery19105777307022835423="28" _events="[object Object]">
        <p style="margin-left: 40px;"><span style="text-align: left; text-transform: none; text-indent: 0px; letter-spacing: normal; font-style: normal; font-weight: 400; word-spacing: 0px; float: none; white-space: pre-wrap; orphans: 2; widows: 2; background-color: #ffffff; text-decoration-color: initial; font-variant-ligatures: normal; font-variant-caps: normal; -webkit-text-stroke-width: 0px; text-decoration-style: initial; display: inline !important; font-family: ''Courier New''; font-size: 13px; color: #000000;">Dear IAM Team,</span></p>
        <p style="margin-left: 40px;"><span style="text-align: left; text-transform: none; text-indent: 0px; letter-spacing: normal; font-style: normal; font-weight: 400; word-spacing: 0px; float: none; white-space: pre-wrap; orphans: 2; widows: 2; background-color: #ffffff; text-decoration-color: initial; font-variant-ligatures: normal; font-variant-caps: normal; -webkit-text-stroke-width: 0px; text-decoration-style: initial; display: inline !important; font-family: ''Courier New''; font-size: 13px; color: #000000;">The Event @@EVENTNAME@@ already exist as Inactive for Employee(@@EMPLOYEEID@@) - Existing EmployeeEventTrigger Id: @@EXISTING_EVENTID@@ .</span></p>
<p style="margin-left: 40px;">
    <b>EmployeeId</b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;: @@EMPLOYEEID@@ <br>
    <b>Event Name</b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;: @@EVENTNAME@@ <br>
    <b>Existing EmployeeEvent Trigger Id&nbsp;</b>:&nbsp;@@EXISTING_EVENTID@@ <br>
	<b>EffectiveDate&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</b>:&nbsp;@@EFFECTIVEDATE@@ <br>
 </p>    
     
        
        
        
        <p style="font-family: arial; margin-left: 40px;">&nbsp;<span style="font-size: small; white-space: pre-wrap; background-color: #ffffff; font-family: ''Courier New''; color: infotext;"></span></p>
        
        <p style="margin-left: 40px;">Regards,</p>
        <p style="margin-left: 40px;">@@REGARDS@@</p>
        <p style="margin-left: 40px;"><span style="font-family: ''Courier New''; font-size: 13px; color: infotext;">&nbsp;</span></p>
        <p style="margin-left: 40px;"><span style="font-family: ''Courier New''; font-size: 13px; color: infotext;">*** This is a system-generated message. Please do not reply to this message.***</span></p>
    
</body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
END
