using System;
using System.Collections.Generic;
using System.Text;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using System.Net.Mail;
namespace DI.Chakra.Services.EmailingProcessor
{
    public interface IEMailServerProvider 
    {
        IList<MailServerConfigInfo> GetAllMailServerInfo();

        MailServerConfigInfo GetAvailableMailServer();

        bool Send(EmailInfo mailInfo,System.Net.Mail.Attachment attachment=null);

        bool TestConnection(MailServerConfigInfo _config);

        void Insert(EmailInfo _mailInfo);

        void Insert(IList<EmailInfo> _lstMailInfo);

        void PostSend(EmailInfo _mailInfo);

        void PostSend(IList<EmailInfo> _lstMailInfo);

        IList<EmailInfo> GetMailsToDispatch(EmailActivityType emailActivityType);

        ////For Changing the status
        //void Update(EmailInfo _mailInfo);

        ////For Changing the status
        //void Update(IList<EmailInfo> _lstMailInfo);

        //void MoveSentItems(EmailInfo _mailInfo);

        //void MoveSentItems(IList<EmailInfo> _lstMailInfo);


    }
}
