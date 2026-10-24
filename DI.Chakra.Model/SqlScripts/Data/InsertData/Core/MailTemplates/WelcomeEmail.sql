
INSERT INTO [dbo].[DI_M_MailTemplate]
           ([TemplateName],[TemplateDescription],[MailFrom],[MailTo],[MailSubject],[MailContent],[CreatedBy],[CreatedDate],[LastModifiedBy],[LastModifiedDate])
     VALUES
           ('LotusNotesWelcomeEmail','LotusNotesWelcomeEmail','devteam@deepidentity.com',NULL,'Welcome to the Singapore Power Corporate Email system',
'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head>
    <title></title>
    <style type="text/css">
        .body {
            font-family: Courier New;
            font-size: 17px;
            font-weight: normal;
            margin-left: 20px;
        }

        .marlef {
            margin-left: 40px;
        }
    </style>
</head>
<body class="body marlef">
    <p>Dear New Email user,</p>


    <p>Welcome to the Singapore Power Corporate Email system.</p>

    <p>
        Please note that our Email facility is for official use and must not be
misused. If you receive non-business related mails, especially those with
large attachments or chain mails, please delete them immediately from your
mailbox. You are NOT to disseminate non-business related mails as it is
against our Corporate Email policy and can lead to disciplinary action. We
would also like to remind all that every email is archived and can be
retrieved for investigative purposes, where required. A copy of the policy
is available from the Enterprise Portal at Resources->IT Needs->IT Policies
& Guidelines->Corporate Email Policy
http://spportal.sap.local/irj/go/km/docs/documents/SP%20WCM/Singapore%20Power/Info%20Systems/Policies%20and%20Guidelines/documents/Corporate%20Email%20policy%20v1.0.pdf
    </p>

    <p>
        For assistance in using the Lotus Notes client, please visit the IT Needs
section in the Enterprise portal. The IT Needs section is located at
Resources->IT Needs->Tips&Guides->Lotus Notes
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
        You can get more information about the company''s businesses and policies
from our Singapore Power Enterprise Portal. These policies cover areas on
Email, Financial, Purchasing, HR matters, etc.
    </p>

    <p>Regards,</p>

    <p>Singapore Power Limited : 199406577N </p>

    <p>________________________________________________________________________________________________________________________________________________________________________________________________________</p>

    <p>This email and any files transmitted with it are confidential and intended for the named recipient only. The contents herein are privileged to the sender and the use thereof is restricted to the intended purpose. Please notify the sender immediately by e-mail if you have received this e-mail by mistake and delete this e-mail from your system. If you are not the intended recipient please do not disclose, copy, distribute or take any action in reliance on the contents of this e-mail. Thank you.</p>

    <p>________________________________________________________________________________________________________________________________________________________________________________________________________</p>

</body>
</html>
','IACMAdmin',GETDATE(),'IACMAdmin',NULL)

GO
