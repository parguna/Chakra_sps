using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Workflow.ServiceHelpers;
using DI.Chakra.Infrastructure;
using System.Net.Http;
using System.ServiceProcess;
using System.Timers;
using DI.Chakra.Connector;
using DI.Chakra.ProvisioningUtility;

namespace DI.Chakra.Services.MiscProcessor
{
    public class WakeupServices
    {
        public static bool restartDiscoveryService = true;
        public static bool restartEmailingService = true;
        public static bool restartAdHocJobService = true;
        public static bool restartAttestationService = true;
        public static bool restartReportService = true;
        public static bool restartProvisioningService = true;
        
       
        public async void Start()
        {
            try
            {
                DILog.HouseKeepingLogger.WriteLog(LogLevelType.Info, "WakeupServices Started");
                ResetAllFlag();
               

                var serviceTiming = ConfigurationMaster.GetByConfigKey("WakeupServices", Product.IACM).ConfigValue.ToString();
                string[] services = serviceTiming.Split(',');
                foreach (var service in services)
                {
                    var occurence = service.Split('~');
                    if (occurence.Count() > 0 && occurence.Count() == 4)
                    {
                        if (!string.IsNullOrEmpty(occurence[1]) && occurence[1] == "Daily")
                        {
                            var time = TimeSpan.Parse(occurence[3]);
                            if (time < DateTime.Now.TimeOfDay)
                            {
                                WakeupCallForService(occurence[0]);
                            }
                        }
                        else if (!string.IsNullOrEmpty(occurence[1]) && occurence[1] == "Weekly")
                        {
                            if (Convert.ToInt32(occurence[2]) == Convert.ToInt32(DateTime.Now.DayOfWeek))
                            {
                                var time = TimeSpan.Parse(occurence[3]);
                                if (time < DateTime.Now.TimeOfDay)
                                {
                                    WakeupCallForService(occurence[0]);
                                }
                            }
                        }
                        else if (!string.IsNullOrEmpty(occurence[1]) && occurence[1] == "Monthly")
                        {
                            if (Convert.ToInt32(occurence[2]) == DateTime.Now.Day)
                            {
                                var time = TimeSpan.Parse(occurence[3]);
                                if (time < DateTime.Now.TimeOfDay)
                                {
                                    WakeupCallForService(occurence[0]);
                                }
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                DILog.HouseKeepingLogger.WriteLog(LogLevelType.Error, ex.Message, ex);
            }
        }

        private void WakeupCallForService(string serviceName)
        {
            try
            {
                if (string.IsNullOrEmpty(serviceName))
                    return;

                serviceName = serviceName.Trim();
                //DILog.HouseKeepingLogger.WriteLog(LogLevelType.Info, string.Format("Checking whether to restrat the serivce {0} now {1}", serviceName, DateTime.Now));
                if (serviceName == ServiceName.DIChakraDiscoveryService_CUST.ToString() && restartDiscoveryService)
                {
                    var process = Process.GetAll().Where(x => x.ProcessType == ProcessType.Discovery && x.Status == "InProgress").FirstOrDefault();
                    if (process == null)
                    {
                        RestartService(ServiceName.DIChakraDiscoveryService_CUST.ToString());
                    }
                }
                if (serviceName == ServiceName.DIChakraAttestationService_CUST.ToString() && restartAttestationService)
                {
                    var process = Process.GetAll().Where(x => x.ProcessType == ProcessType.Attestation && x.Status == "InProgress").FirstOrDefault();
                    if (process == null)
                    {
                        RestartService(ServiceName.DIChakraAttestationService_CUST.ToString());
                    }
                }
                if (serviceName == ServiceName.DIChakraAdHocJobService_CUST.ToString() && restartAdHocJobService)
                {
                    var process = Process.GetAll().Where(x => x.ProcessType == ProcessType.AdHocJob && x.Status == "InProgress").FirstOrDefault();
                    if (process == null)
                    {
                        RestartService(ServiceName.DIChakraAdHocJobService_CUST.ToString());
                    }
                }
                if (serviceName == ServiceName.DIChakraReportingService_CUST.ToString() && restartReportService)
                {
                    var process = Process.GetAll().Where(x => x.ProcessType == ProcessType.Reporting && x.Status == "InProgress").FirstOrDefault();
                    if (process == null)
                    {
                        RestartService(ServiceName.DIChakraReportingService_CUST.ToString());
                    }
                }
                if (serviceName == ServiceName.DIChakraProvisioningService_CUST.ToString() && restartProvisioningService)
                {
                    var isAnyAccessRequest = AccessRequest.GetApprovedAccessRequestBatch();

                    if (isAnyAccessRequest == null || isAnyAccessRequest.Count() <= 0)
                    {
                        RestartService(ServiceName.DIChakraProvisioningService_CUST.ToString());
                    }
                }
                if (serviceName == ServiceName.DIChakraEmailingService_CUST.ToString() && restartEmailingService)
                {
                    RestartService(ServiceName.DIChakraEmailingService_CUST.ToString());
                }
                if (serviceName == "DIChakraReportService" && restartReportService)
                {
                    var process = Process.GetAll().Where(x => x.ProcessType == ProcessType.Reporting && x.Status == "InProgress").FirstOrDefault();
                    if (process == null)
                    {
                        RestartService("DIChakraReportService");
                    }
                }
            }
            catch (Exception ex)
            {

                // throw;
            }
        }

        private void RestartService(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
                return;

            var service = serviceName.Replace("_CUST", "").Trim();
            ServiceController serviceController = new ServiceController(service);
            try
            {
                DILog.HouseKeepingLogger.WriteLog(LogLevelType.Info, string.Format("Attempting to restart the serivce {0}, at {1}", service, DateTime.Now));
                if ((serviceController.Status.Equals(ServiceControllerStatus.Running)) || (serviceController.Status.Equals(ServiceControllerStatus.StartPending)))
                {
                    serviceController.Stop();
                    serviceController.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 5, 0));
                }       
                         
                serviceController.Start();
                serviceController.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 10, 0));

                if (serviceName == ServiceName.DIChakraDiscoveryService_CUST.ToString())
                {
                    restartDiscoveryService = false;

                }
                else if (serviceName == ServiceName.DIChakraReportingService_CUST.ToString())
                {

                    restartReportService = false;
                }
                else if (serviceName == ServiceName.DIChakraAdHocJobService_CUST.ToString())
                {
                    restartAdHocJobService = false;
                }
                else if (serviceName == ServiceName.DIChakraAttestationService_CUST.ToString())
                {
                    restartAttestationService = false;
                }
                else if (serviceName == ServiceName.DIChakraProvisioningService_CUST.ToString())
                {
                    restartProvisioningService = false;
                }
                else if (serviceName == ServiceName.DIChakraEmailingService_CUST.ToString())
                {
                    restartEmailingService = false;
                }
            }
            catch (Exception ex)
            {
                //ShowMsg(AppTexts.Information, AppTexts.SystematicError, MessageBox.Icon.WARNING);
                DILog.HouseKeepingLogger.WriteLog(LogLevelType.Error, string.Format("error while restart the serivce {0}, at {1}", service, DateTime.Now), ex);
            }
        }

        private void ResetAllFlag()
        {
            if (DateTime.Now.Hour == 0)
            {
                DILog.HouseKeepingLogger.WriteLog(LogLevelType.Info, "WakeupServices  ResetAllFlag Started");
                restartDiscoveryService = true;
                restartEmailingService = true;
                restartAdHocJobService = true;
                restartAttestationService = true;
                restartReportService = true;
                restartProvisioningService = true;
            }
        }

        public async void StartHealthCheck()
        {
            CheckHealthService(ServiceName.DIChakraDiscoveryService_CUST.ToString());
            CheckHealthService(ServiceName.DIChakraProvisioningService_CUST.ToString());
            CheckHealthService(ServiceName.DIChakraAttestationService_CUST.ToString());
            CheckHealthService(ServiceName.DIChakraAdHocJobService_CUST.ToString());
            CheckHealthService(ServiceName.DIChakraEmailingService_CUST.ToString());
            CheckHealthService(ServiceName.DIChakraReportingService_CUST.ToString());
            CheckHealthService("DIChakraReportService");
        }

        public void CheckHealthService(string serviceName)
        {
           
            try
            {
                var service = serviceName.Replace("_CUST", "");
                ServiceController serviceController = new ServiceController(service);
                if (!serviceController.Status.Equals(ServiceControllerStatus.Running))
                {
                    //string remarks = service + " is not running.";
                    //var isAlreadyNotificationLog = AlertCenterNotification.GetAlertCenterNotificationsByType(AlertCenterNotificationType.ServiceNotRunning).Where(x => x.Remarks.Contains(remarks) && x.CreatedDate.Date == DateTime.Now.Date && x.AlertCenterNotificationStatus == AlertCenterNotificationStatus.Pending).Any();
                    //if (!isAlreadyNotificationLog)
                    //{
                    //    DILog.AlertNotificationLogger.WriteLog(null, null, remarks, AlertCenterNotificationType.ServiceNotRunning);
                    //}
                }
                // Auto close notification for service running again.
                //else if (serviceController.Status.Equals(ServiceControllerStatus.Running))
                //{
                //    var notification = AlertCenterNotification.GetAlertCenterNotificationsByType(AlertCenterNotificationType.ServiceNotRunning).Where(x => x.Remarks.Contains(service) && x.CreatedDate.Date == DateTime.Now.Date && x.AlertCenterNotificationStatus == AlertCenterNotificationStatus.Pending).FirstOrDefault();
                //    if(notification != null)
                //    {
                //        notification.AlertCenterNotificationStatus = AlertCenterNotificationStatus.Closed;
                //        notification.LastModifiedBy = "System";
                //        notification.LastModifiedDate = DateTime.Now;
                //        notification.Save("System");
                //    }
                //}
            }
            catch (Exception ex)
            {

                // throw;
            }
        }
        public void GenerateTCode()
        {
            try
            {
                if (MasterConfigHelper.RunRolesToTCodeGeneration)
                {
                    try
                    {
                        SAPRoleToTCodes.RunRolesToTCodeGenertion();
                    }
                    catch (Exception ex)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, $"Error While Executing RolesToTCodeGeneration Method. stack Trace:{ex.StackTrace}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error While Executing GenerateTCode Method.", ex);
            }
        }
        /// <summary>
        /// Method is used to update the HRMS details in HRMS. Also it generates the propagation trigger accordingly
        /// Should update the reference user information accordingly
        /// </summary>
        public void UpdateHRMSUserDataInIDMS()
        {
            List<string> users = null;
            try
            {
                if (MasterConfigHelper.TriggerHRMSUserDataUpdate)
                {
                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "UpdateHRMSUserDataInIDMS trigger started");

                    var endpoint = EndPoint.GetTrustedSourceEndPoint();
                    var connector = ConnectorFactory.GetConnector(endpoint, null, new DiscoveryLogger(0, endpoint.Id, EndpointType.SAP, endpoint.DisplayName));
                    users = User.GetActiveEmployeesPerno();
                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"UpdateHRMSUserDataInIDMS - Get All active Employee list Count:{users?.Count}");
                    if (users != null && users.Count > 0)
                    {
                        connector.UpdateAllUserFromHRMS(users);
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Info,$"UpdateHRMSUserDataInIDMS completed");
                    }
                    ConfigurationMasterIM config =  ConfigurationMaster.GetByConfigKey(ConfigConstants.TriggerHRMSUserDataUpdate, Product.IM) as ConfigurationMasterIM ;
                    if (config != null)
                    {
                        config.ConfigValue = string.Empty;
                        config.Save("System");
                    }
                }
                
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"Error While Executing UpdateAllUserFromHR Method. stack Trace:{ex.StackTrace}", ex);
            }
            finally
            {
                users?.Clear();
                users = null;
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "UpdateHRMSUserDataInIDMS trigger Completed");
            }
        }
        //public void CreateADPropagation()
        //{
        //    try
        //    {
        //        if (MasterConfigHelper.CreateADPropagation)
        //        {
        //            DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "CreateADPropagation started.");
        //            var ep = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
        //            if(ep != null && ep.IsActive)
        //            {
        //                var usrs = User.GetActiveEmployees().Where(i => i.FullTimeEmployee).ToList();
        //                var adAccounts = AccountReconcilation.GetAllActiveByEndpointIdByAccountType(ep.Id, AccountType.User);
        //                if(adAccounts != null && adAccounts.Any() && usrs != null && usrs.Any())
        //                {
        //                    var users = adAccounts.Where(i=> usrs.Exists(j=> i.UserId.Value == j.Id)).Select(i=> i.UserId.Value).Distinct().ToList();
        //                    foreach (var userId in users)
        //                    {
        //                        try
        //                        {
        //                            var evntId = EmployeeEventTrigger.CreateDummyPropagationTrigger(userId, "System", "For EEGroup propagation", "Bulk Propagation from IAM Team");
        //                            if (evntId > 0)
        //                            {
        //                                var newRequest = new AccessRequest();
        //                                newRequest.UserId = userId;
        //                                newRequest.AccountType = AccountType.User;
        //                                newRequest.ApplicationType = ep.ApplicationType;
        //                                newRequest.EndpointSystemId = ep.Id;
        //                                newRequest.RequestStatus = AccessRequestStatus.Granted;
        //                                newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
        //                                newRequest.RequestSource = AccessRequestSource.Propagation;
        //                                newRequest.RequesterId = null;
        //                                newRequest.ExecutionTime = DateTime.Now;
        //                                newRequest.Description = "This request is created automatically for Propagation";
        //                                newRequest.Remarks = string.Empty;
        //                                newRequest.TransactionId = Guid.NewGuid();
        //                                newRequest.EmployeeEventTriggerId = evntId;
        //                                newRequest.CreatedBy = "Sysetm";
        //                                newRequest.CreatedDate = DateTime.Now;
        //                                newRequest.Save("System");
        //                            }
        //                            else
        //                            {
        //                                DILog.SchedulerLogger.WriteLog(LogLevelType.Info, $"CreateADPropagation: Employeevent did not create.");
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            DILog.SchedulerLogger.WriteLog(LogLevelType.Info, $"CreateADPropagation failed to create trigger.");
        //                        }
        //                    }
        //                    users.Clear();
        //                    users = null;
        //                    usrs.Clear();
        //                    usrs = null;
        //                    adAccounts.Clear();
        //                    adAccounts = null;
        //                }
                        
        //                DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "ADPropagation created successfully.");
        //            }
        //            else
        //            {
        //                DILog.SchedulerLogger.WriteLog(LogLevelType.Info, "ADPropagation: Endpoint inactive. Cannot create propagation trigger.");
        //            }
        //            ADSUserInfo.ExecuteQuery("UPDATE DI_M_IMConfigurationMaster SET ConfigValue='0' WHERE ConfigKey='CreateADPropagation'");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.SchedulerLogger.WriteLog(LogLevelType.Exception, "CreateADPropagation trigger Completed",ex);
        //    }
        //}
    }
}
