
using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
using DI.Chakra.Services.EmailingProcessor;
using System.ServiceProcess;
using System.Timers;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Services.Emailing
{
    public partial class EmailingService : ServiceBase
    {
        Timer emailingTimer;
        bool isEmailingInProgress = false;
        int minuteMultiplier = 60000;

        public EmailingService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            int emailingTimerInterval = AppSettingsHelper.EmailingTimerIntervalInMinutes * minuteMultiplier;

            emailingTimer = new Timer(emailingTimerInterval);
            emailingTimer.Enabled = true;
            emailingTimer.Elapsed += new ElapsedEventHandler(emailingingTimer_Elapsed);

            DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Emailing agent started.");
        }

        protected override void OnStop()
        {
            DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "Emailing agent stopped.");
        }

        void emailingingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
           
            if (!isEmailingInProgress)
            {
                isEmailingInProgress = true;

                EmailAgent emailAgent = new EmailAgent();
                emailAgent.SendEmails(EmailActivityType.None);

                isEmailingInProgress = false;
            }
        }
    }
}
