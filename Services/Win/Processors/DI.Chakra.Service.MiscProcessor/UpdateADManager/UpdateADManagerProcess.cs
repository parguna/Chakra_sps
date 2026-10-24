using DI.Chakra.Connector;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Service.MiscProcessor
{
    public class UpdateADManagerProcess
    {
        readonly string _masterConfigHelperServiceUser;
        readonly SchedulerLogger _logger = null;
        bool _disposed = false;
        readonly ISchedulerFactory _schedulerFactory;
        readonly string _CronScheduleString;
        readonly string crReminderIdentity = "SPUpdateADManagerProcess";
        public UpdateADManagerProcess()
        {
            _masterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
            _logger = new SchedulerLogger();
            _schedulerFactory = new StdSchedulerFactory();
            _CronScheduleString = MasterConfigHelper.CRON_Expression_UpdateADManagerTimer;
        }

        public async void Start()
        {
            try
            {
                IScheduler scheduler = await _schedulerFactory.GetScheduler();
                _logger.WriteLog(LogLevelType.Info, $"UpdateADManagerProcess started");
                await scheduler.Start();

                ITrigger trigger = TriggerBuilder.Create()
                     .WithIdentity(crReminderIdentity)
                     .WithCronSchedule(_CronScheduleString)   // "0 59 23 L * ?")
                     .StartAt(DateTime.Now)
                     .WithPriority(1)
                     .Build();

                IJobDetail job = JobBuilder.Create<UpdateADManagherJob>().WithIdentity(crReminderIdentity).Build();
                await scheduler.ScheduleJob(job, trigger);

            }
            catch (Exception ex)
            {
                _logger.WriteLog(null, LogLevelType.Error, $"UpdateADManagerProcess start failed.{ex.Message}", ex);
            }
        }
        public async void Stop()
        {
            try
            {
                ISchedulerFactory schedFact = new StdSchedulerFactory();
                // get a scheduler
                IScheduler sched = await schedFact.GetScheduler();
                //sched.Standby();
                await sched.UnscheduleJob(new TriggerKey(crReminderIdentity));
                _logger.WriteLog(LogLevelType.Info, $"UpdateADManagerProcess stopped");
            }
            catch (Exception ex)
            {
                _logger.WriteLog(null, LogLevelType.Error, $"UpdateADManagerProcess Stop failed.{ex.Message}", ex);
            }
        }
        public void Dispose() => Dispose(true);
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
            }
            _disposed = true;
        }
    }
    public class UpdateADManagherJob : IJob
    {
        readonly string _masterConfigHelperServiceUser;
        public UpdateADManagherJob()
        {
            _masterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
        }
        public System.Threading.Tasks.Task Execute(IJobExecutionContext context)
        {
            Action action = () =>
            {
                DI.Chakra.Logger.SchedulerLogger _logger = new SchedulerLogger();
                try
                {
                    RunUpdateADManagher();
                }
                catch (Exception ex)
                {
                    _logger.WriteLog(null, LogLevelType.Error, $"UpdateADManagher Execute failed.{ex.Message}", ex);
                }
            };
            return System.Threading.Tasks.Task.Run(action);
        }
       
        public static void RunUpdateADManagher()
        {
            List<AccountReconcilation> adAccountReconUsers = new List<AccountReconcilation>();
            IdmsLogger idmsLogger = null;
            try
            {

                bool canrunADManagerJob = MasterConfigHelper.CanRunUpdateADManagerJob;

                if (canrunADManagerJob)
                {
                    var endpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.ADSingaporePowerLocal);
                    adAccountReconUsers = AccountReconcilation.GetAllActiveByEndpointIdByAccountType(endpoint.Id, AccountType.User);
                    idmsLogger = new ProvisioningLogger(null, endpoint.Id, endpoint.EndpointSystemTypeId, endpoint.DisplayName);
                    ConnectorBase _connector = ConnectorFactory.GetConnector(endpoint, null, idmsLogger);

                    idmsLogger.WriteLog(LogLevelType.DebugStep, $"Started updating AD Manager attribute. Total users count:{adAccountReconUsers?.Count}");

                    ADSUserInfo adsuser = null; SMSUserInfo smsUser = null;
                    User user = null;
                   
                    foreach (var item in adAccountReconUsers)
                    {
                        if (MasterConfigHelper.IsDeploymentInProgress)
                        {
                            idmsLogger?.WriteLog(LogLevelType.Info, $"UpdateADManagerAttribute - Deployment InProgress");
                            return;
                        }
                        try
                        {
                            adsuser = ADSUserInfo.GetById(item.EndpointUserId);
                            user = User.GetById(item.UserId.Value);
                            
                            if (user == null)
                                continue;

                            if (!string.IsNullOrEmpty(user.ROEmployeeId))
                            {
                                var ro = User.GetByEmployeeId(user.ROEmployeeId);

                                if (ro != null)
                                {
                                    var roAD = ADSUserInfo.GetActivePrimaryByDIUserId(ro.Id, endpoint.Id);

                                    if (roAD != null)
                                    {
                                        adsuser.manager = roAD.distinguishedName;
                                    }
                                    else
                                    {
                                        adsuser.manager = string.Empty;
                                    }
                                }
                                else
                                {
                                    adsuser.manager = string.Empty;
                                }
                            }
                            else
                            {
                                adsuser.manager = string.Empty;
                            }

                            if (user.FullTimeEmployee)
                            {
                                if (user.IsExecutiveEmployee)
                                {
                                    adsuser.otherFascimileTelephoneNumber = "Exec";
                                }
                                else
                                {
                                    adsuser.otherFascimileTelephoneNumber = "Non-Exec";
                                }
                                adsuser.extensionAttribute13 = ADSUserInfo.GetExtensionAttribute13(user);
                            }
                            else
                            {
                                adsuser.otherFascimileTelephoneNumber = "Non-Employees";
                            }

                            //update lenel badge id.                            
                            smsUser = SMSUserInfo.GetSMSUserByADID(adsuser.sAMAccountName);
                            //smsUser = allSMSUsers.Where(sms => sms.OPHONE.Equals(adsuser.sAMAccountName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                            if (smsUser != null)
                            {
                                // DILog.ProvisioningLogger.WriteLog(null, endpoint.Id, LogLevelType.Info, $"SMS User found for user - {adsuser.sAMAccountName}");
                                var smsuserBadges = SMSBadgeInfo.GetByBadgeUserId(smsUser.UserId);
                                //  var smsuserBadges = allSMSBadges.Where(badge => badge.UserId == smsUser.UserId && badge.STATUS == 1).FirstOrDefault();
                                if (smsuserBadges != null)
                                {
                                    adsuser.extensionAttribute7 = smsuserBadges.BadgeNumber.ToString();
                                }
                                else
                                {
                                    DILog.ProvisioningLogger.WriteLog(null, endpoint.Id, LogLevelType.Info, $"SMS User Badge  not found for user - {smsUser.OPHONE}");
                                }

                                //DILog.ProvisioningLogger.WriteLog(null, endpoint.Id, LogLevelType.Info, $"adsuser.extensionAttribute7 - {adsuser.extensionAttribute7}");
                            }
                            //else
                            //    DILog.ProvisioningLogger.WriteLog(null, endpoint.Id, LogLevelType.Info, $"SMS User not found for user - {adsuser.sAMAccountName}");

                            var org = NOrganisation.GetByOrgCode(user.SectionCode);

                            if (org != null)
                            {
                                var orgHead = NOrganisationHead.GetByOrganisationIdAndHead(org.Id, user.Id);
                                if (orgHead != 0)
                                {
                                    adsuser.extensionAttribute8 = NOrganisationScope.GetById(org.ScopeId.Value).Scope;
                                }
                            }

                            adsuser.extensionAttribute10 = user.OrgSection;
                            adsuser.extensionAttribute6 = user.CostCenter;
                            adsuser.extensionAttribute1 = user.OrgBranch;
                            adsuser.extensionAttribute2 = user.OrgUnit;

                            adsuser.otherPager = user.OrgUnit;
                            adsuser.businessCategory = user.OrgBranch;

                            adsuser.title = user.PositionName;
                            adsuser.department = user.OrgDepartment;
                            adsuser.division = user.OrgDivision;
                            adsuser.company = user.OrgCompany;
                            adsuser.businessCategory = user.OrgBranch;
                            adsuser.pager = user.OrgSection;


                            adsuser.postalAddress = user.PostalAddress;
                            adsuser.postalCode = user.PostalCode;
                            adsuser.countryCode = user.Country;

                            adsuser.telephoneNumber = user.TelephoneNumber;
                            adsuser.mobile = user.MobileNumber;
                            adsuser.facsimileTelephoneNumber = user.Fax;
                            adsuser.mail = user.EmailId;
                            adsuser.physicalDeliveryOfficeName = user.EmployeeId;

                            adsuser.LastModifiedBy = "System";
                            adsuser.LastModifiedDate = DateTime.Now;


                            if (_connector.UpdateADManagerAttribute(adsuser))
                            {
                                adsuser.Save("System");
                            }
                        }
                        catch (Exception ex)
                        {
                            idmsLogger?.WriteLog(LogLevelType.Exception, string.Format("Error while updating AD attributes for users - {0}", user.UserName), ex);
                        }

                    }

                    idmsLogger?.WriteLog(LogLevelType.DebugStep, $"Completed updating total {adAccountReconUsers.Count()} User's AD Manager attribute.", null);
                }
            }
            catch (Exception ex)
            {
                idmsLogger?.WriteLog(LogLevelType.Exception, "Error while updating manager for AD users.", ex);
            }
            finally
            {
                adAccountReconUsers?.Clear();
                adAccountReconUsers = null;
            }
        }

    }
}
