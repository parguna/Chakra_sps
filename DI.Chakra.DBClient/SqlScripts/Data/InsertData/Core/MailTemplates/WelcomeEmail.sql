SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 

INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName]) VALUES (25, N'	', N'LotusNotesWelcomeEmail', N'devteam@deepidentity.com', N'@@MAILTO_USR@@', NULL, NULL, N'Welcome to the Resort World Sentosa', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html>
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
    <body class="body marlef">
        <p>Dear New Email user,</p>
        <p>Welcome to the Resort World Sentosa.</p>
        <p>
        Please note that our Email facility is for official use and must not be
        misused. If you receive non-business related mails, especially those with
        large attachments or chain mails, please delete them immediately from your
        mailbox. You are NOT to disseminate non-business related mails as it is
        against our Corporate Email policy and can lead to disciplinary action. We
        would also like to remind all that every email is archived and can be
        retrieved for investigative purposes, where required. A copy of the policy
        is available from the Enterprise Portal at Resources-&gt;IT Needs-&gt;IT Policies
        &amp; Guidelines-&gt;Corporate Email Policy
        http://spportal.sap.local/irj/go/km/docs/documents/SP%20WCM/Singapore%20Power/Info%20Systems/Policies%20and%20Guidelines/documents/Corporate%20Email%20policy%20v1.0.pdf
        </p>
        <p>
        For assistance in using the Lotus Notes client, please visit the IT Needs
        section in the Enterprise portal. The IT Needs section is located at
        Resources-&gt;IT Needs-&gt;Tips&amp;Guides-&gt;Lotus Notes
        http://spportal.sap.local/irj/portal/anonymous/tipsguides . Please select
        the "Lotus Notes" tab. You are highly encouraged to find out more about the
        Enterprise Email Archival System and the Enterprise Fax System from the
        following documents in this section:
        </p>
        <p>- Email Archival Guide</p>
        <p>- FAQ for Email Archival</p>
        <p>- User Guide for Enterprise Fax</p>
        <p>- FAQ for Enterprise Fax</p>
        <p>
        Our anti-spam system, Mimesweeper, will help you filter potential spam
        emails from your mailbox. Please click the link below on the usage of the
        Mimesweeper system
        http://spportal.sap.local/irj/go/km/docs/documents/SP%20WCM/Singapore%20Power/Info%20Systems/New%20Employees/documents/Usage%20of%20MimeSweeper.pdf
        </p>
        <p>
        Please be reminded that regular housekeeping by deleting mails no longer
        required to ensure better system response time, easier mail searching and
        to reduce the chance of your mailbox database being corrupted due to large
        mail database size. The free space is immediately "returned" to you after
        you have emptied your "Trash" folder. Do note that deleted emails can be
        retrieved via the Email Archival system for a period of up to 7 years.
        </p>
        <p>
        The email system will prevent you from sending out mails if your mailbox
        size is larger than the quota allocated to you.
        </p>
        <p>
        Your email quota and current usage can be accessed by the mailbox indicator at
        the top-hand corner (slightly above the Inbox folder) of your Lotus Notes
        client. Hover your mouse on the indicator to see more details.
        </p>
        <p>For further assistance, please call our Helpdesk at 6378 8111.</p>
        <p>
        You can get more information about the company s businesses and policies
        from our Resort World Sentosa Enterprise Portal. These policies cover areas on
        Email, Financial, Purchasing, HR matters, etc.
        </p>
        <table style="margin-top:20px">
        <tr>
          <td><p>Best regards,<br/>
              IT ID Administration team</p>
            </td>
        </tr>
		<tr><td><p>*** This is a system-generated message. Please do not reply to this message.***</p></td></tr>
      </table>
    </body>
</html>', NULL, N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL, 2, 25)

SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] OFF