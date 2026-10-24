using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;

namespace DI.Chakra.Services.EmailingProcessor
{
    public class EmailAgent
    {
        public void SendEmails(EmailActivityType emailActivityType)
        {
            EmailHandler.StartSendingMails(emailActivityType);
        }


    }
}
