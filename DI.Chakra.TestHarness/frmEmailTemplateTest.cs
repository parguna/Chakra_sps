using DI.Chakra.Connector;
using DI.Chakra.DomainModel;
using DI.Chakra.ProvisioningUtility;
using DI.Chakra.Services.ProvisioningProcessor;
using DI.Chakra.Workflow.Processor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace TestHarness
{
    public partial class frmEmailTemplateTest : Form
    {
        public static string requestId = string.Empty;
        public frmEmailTemplateTest()
        {
            InitializeComponent();
        }

        private void frmEmailTemplateTest_Load(object sender, EventArgs e)
        {
            cmbTemplates.Items.Clear();
            var emails = MailTemplate.GetAll();
            foreach (var email in emails)
            {
                cmbTemplates.Items.Add(email.TemplateName);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                requestId = txtRequestId.Text;
                var typeInfo = typeof(frmEmailTemplateTest).GetTypeInfo();
                var methodInfo = typeInfo.DeclaredMethods.Where(p => p.Name == cmbTemplates.Text).FirstOrDefault();
                if (methodInfo == null)
                {
                    MessageBox.Show("Method not implemented.");
                    return;
                }
                methodInfo.Invoke(null, null);
                MessageBox.Show("Email saved successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region methods
       
        static void CreateAccountNotificationCommonTemplate()
        {
           
            var accReq = AccessRequest.GetAll(true).FirstOrDefault(i => i.RequestSource == DI.Chakra.Infrastructure.Enums.AccessRequestSource.AdhocRequest);
            var user = User.GetById(accReq.UserId);
            var adUser = ADSUserInfo.GetAll().FirstOrDefault();
            ProvisioningEmailHelper.SendEmailForAccountProvisioning(accReq, adUser, user, DI.Chakra.Infrastructure.Enums.ApplicationType.ADSingaporePowerLocal);
        }
        static void OnboardingUserEmail()
        {
            var all = AccessRequest.GetAll(true);
            var accReq = all.FirstOrDefault(i=> i.Id == Convert.ToInt64(requestId));
            var user = User.GetById(accReq.UserId);
            var adUser = ADSUserInfo.GetAll().FirstOrDefault();
            ProvisioningEmailHelper.SendEmailForAccountProvisioning(accReq, adUser, user, DI.Chakra.Infrastructure.Enums.ApplicationType.ADSingaporePowerLocal);
        }
        static void NotifyEmployeeEventTask()
        {
            SendNotificationEmail obj = new SendNotificationEmail();
            var tasks = Task.GetAll(true).OrderByDescending(i=>i.CreatedDate).Where(i => i.EmployeeEventTriggerId.HasValue );
            var task1 = tasks.FirstOrDefault();
            var task = Task.GetById(task1.Id, true);
            obj.SendEmailForEmployeeEvent(task, task.WorkFlowConfiguration, task.EmployeeEventTrigger);

            EmailInfo.GetEmailInfoForSendEmailForEmployeeEventApproved(task, task.EmployeeEventTrigger);
        }
        static void TriggerDeletedNotification()
        {
            var tasks = Task.GetAll(true).OrderByDescending(i => i.LastModifiedDate).Where(i => i.EmployeeEventTriggerId.HasValue);
            var task1 = tasks.FirstOrDefault();
            var task = Task.GetById(task1.Id, true);
            var user = User.GetById(task.ActionUserId.Value);
            ProvisioningEmailHelper.NotifyDeletedTrigger(user, DI.Chakra.Infrastructure.Enums.EmployeeEvents.NewJoinee, task.EmployeeEventTrigger);
        }
        static void RemindEmployeeEventApproval()
        {
            SendReminderEmail email = new SendReminderEmail();
            var tasks = Task.GetAll(true).OrderByDescending(i => i.LastModifiedDate).Where(i => i.EmployeeEventTriggerId.HasValue);
            var task1 = tasks.FirstOrDefault();
            var task = Task.GetById(task1.Id, true);
            var workF = WorkflowConfiguration.GetById(task.WorkflowConfigurationId,true);
            email.SendEmailForEmployeeEvent(task, workF, task.EmployeeEventTrigger);
        }
        static void NotifyEmployeeEventTaskApproved()
        {
            GrantProvisioning grantProvisioning = new GrantProvisioning();
            var tasks = Task.GetAll(true).Where(i => i.EmployeeEventTriggerId.HasValue);
            var task1 = tasks.FirstOrDefault();
            var task = Task.GetById(task1.Id, true);
            grantProvisioning.SendEmailForEmployeeEventApproved(task.EmployeeEventTrigger,"Reason", task.ActionUserId);
        }
        static void RequestWithdrawn()
        {
            WithdrawRequest obj = new WithdrawRequest();
            var accReq = AccessRequest.GetAll().Where(i =>i.ProgressStatus == DI.Chakra.Infrastructure.Enums.AccessRequestProgress.Withdrawn).FirstOrDefault();
            var req = AccessRequest.GetById(accReq.Id, true);
            obj.NotifyApproverAboutRequestWithdrawn(accReq.RequesterId.Value, req);
        }
        static void NonEmployeeExpiryNotification()
        {
            RequestExecutor executor = new RequestExecutor();
            var user = User.GetActiveEmployees().OrderBy(i => i.LastLogonDate).FirstOrDefault(i=>i.TerminationDate.HasValue);
            executor.SendNonEmployeeExpiryNotification(user,12);
        }
        static void NotifyEmployeeEventTaskRejected()
        {
            var tasks = Task.GetAll(true).OrderByDescending(i=>i.LastModifiedDate).Where(i => i.EmployeeEventTriggerId.HasValue);
            var task1 = tasks.FirstOrDefault();
            var task = Task.GetById(task1.Id, true);
            var user = User.GetById(task.ActionUserId.Value);
            ProvisioningEmailHelper.SendEmailForAccessRequestRejected(task);
        }
        static void EmployeeEventWithdrawn()
        {
            WithdrawRequest obj = new WithdrawRequest();
            var tasks = Task.GetAll(true).Where(i => i.EmployeeEventTriggerId.HasValue);
            var task1 = tasks.FirstOrDefault();
            var task = Task.GetById(task1.Id, true);
            var user = User.GetById(task.ActionUserId.Value);
            obj.NotifyApproverAboutRequestWithdrawn(user.Id, task.EmployeeEventTrigger);
        }
        static void BulkRequestWithdrawn()
        {
            WithdrawRequest obj = new WithdrawRequest();
            var tasks = Task.GetAll(true).OrderByDescending(i => i.LastModifiedDate).Where(i => i.BulkRequestId.HasValue);
            var task = tasks.FirstOrDefault(i => i.BulkRequest.ProgressStatus == DI.Chakra.Infrastructure.Enums.AccessRequestProgress.Withdrawn);
            task = Task.GetById(task.Id, true);
            var bulkRequest = BulkRequest.GetById(task.BulkRequestId.Value, true);
            task.BulkRequest = bulkRequest;
            obj.NotifyApproverAboutRequestWithdrawn(task.ActionUserId.Value, task.BulkRequest);
        }
        static void RemindBulkRequestApproval()
        {
            SendReminderEmail obj = new SendReminderEmail();
            var tasks = Task.GetAll(true).OrderByDescending(i => i.LastModifiedDate).Where(i => i.BulkRequestId.HasValue);
            var task = tasks.FirstOrDefault(i => i.BulkRequest.ProgressStatus == DI.Chakra.Infrastructure.Enums.AccessRequestProgress.RequestRejected);
            task = Task.GetById(task.Id, true);
            var bulkRequest = BulkRequest.GetById(task.BulkRequestId.Value, true);
            task.BulkRequest = bulkRequest;
            obj.SendEmailForBulkRequest(task, task.WorkFlowConfiguration);
        }
        static void RequestRejectAlertToApprovers()
        {
            var tasks = Task.GetAll(true).Where(i => i.BulkRequestId.HasValue == false && i.AccessRequest != null && i.AccessRequest.ProgressStatus == DI.Chakra.Infrastructure.Enums.AccessRequestProgress.WaitingforApproval);
            var task1 = tasks.FirstOrDefault();
            var task = Task.GetById(task1.Id, true);
            var adR = AccessRequest.GetById(task.AccessRequest.Id, true);
            task.AccessRequest = adR;
            var user = User.GetById(task.AccessRequest.RequesterId.Value);
            ProvisioningEmailHelper.NotifyApproverAboutRequestRejectedAccessRequest(user, user.Id, task.AccessRequest);
        }
        static void NotifyBulkRequestApproval()
        {
            var tasks = Task.GetAll(true).OrderByDescending(i => i.LastModifiedDate).Where(i => i.BulkRequestId.HasValue);
            var task = tasks.FirstOrDefault(i => i.BulkRequest.ProgressStatus == DI.Chakra.Infrastructure.Enums.AccessRequestProgress.RequestRejected);
            task = Task.GetById(task.Id, true);
            var bulkRequest = BulkRequest.GetById(task.BulkRequestId.Value, true);
            task.BulkRequest = bulkRequest;
            SendNotificationEmail obj = new SendNotificationEmail();
            obj.SendEmailForBulkRequest(task);
        }
        static void RemindAdhocProvisioningApproval()
        {
            SendReminderEmail obj = new SendReminderEmail();
            var tasks = Task.GetAll(true).Where(i=> i.BulkRequestId.HasValue == false && i.AccessRequest != null && i.AccessRequest.ProgressStatus == DI.Chakra.Infrastructure.Enums.AccessRequestProgress.WaitingforApproval);
            var task1 = tasks.FirstOrDefault();
            var task = Task.GetById(task1.Id, true);
            var adR = AccessRequest.GetById(task.AccessRequest.Id, true);
            task.AccessRequest = adR;
            obj.SendEmailForAdhocProvisioning(task, task.WorkFlowConfiguration);
        }
        static void BulkRequestRejected()
        {
            var tasks = Task.GetAll(true).OrderByDescending(i=>i.LastModifiedDate).Where(i=>i.BulkRequestId.HasValue);
            var task = tasks.FirstOrDefault(i => i.BulkRequest.ProgressStatus == DI.Chakra.Infrastructure.Enums.AccessRequestProgress.RequestRejected);
            task = Task.GetById(task.Id, true);
            var bulkRequest = BulkRequest.GetById(task.BulkRequestId.Value, true);
            task.BulkRequest = bulkRequest;
            ProvisioningEmailHelper.SendEmailForAccessRequestRejected(task);

            var atasks = Task.GetAll(true).Where(i => i.AccessRequestId.HasValue && i.AccessRequest.ProgressStatus == DI.Chakra.Infrastructure.Enums.AccessRequestProgress.RequestRejected).OrderByDescending(i => i.LastModifiedDate);
            var atask = atasks.FirstOrDefault();
            atask = Task.GetById(atask.Id, true);
            atask.AccessRequest = AccessRequest.GetById(atask.AccessRequestId.Value,true);
            ProvisioningEmailHelper.SendEmailForAccessRequestRejected(atask);


        }
        static void BulkRequestRejectAlertToApprovers()
        {
            BulkRequest blk = BulkRequest.GetAll(true).FirstOrDefault(i=>i.RequestStatus == DI.Chakra.Infrastructure.Enums.AccessRequestStatus.Rejected && i.BulkRequestDetails.Count > 0);
            var user = User.GetById(blk.RequestedBy);
            ProvisioningEmailHelper.NotifyApproverAboutRequestRejectedBulkRequest(user.Id, user.Id, blk, "Rejection reason");
        }
        static void NotifyReferenceUserChange()
        {
            var accessRequest = AccessRequest.GetByProgressStatus(DI.Chakra.Infrastructure.Enums.AccessRequestProgress.Completed).FirstOrDefault();
            var user = User.GetById(accessRequest.UserId);
            List<string> referenceUsers = User.GetReferenceUsers(user.Id);
            ProvisioningEmailHelper.SendEmailToNotifyReferenceUserChange(user, referenceUsers, DI.Chakra.Infrastructure.Enums.EmployeeEvents.Resignation);
        }
        static void NotifyOffice365Failed()
        {
            var accessRequest = AccessRequest.GetByProgressStatus(DI.Chakra.Infrastructure.Enums.AccessRequestProgress.Completed).FirstOrDefault();
            ProvisioningEmailHelper.NotifyAdminOffice365Failed("UPN", accessRequest);
        }
        static void PassOfficeNotification()
        {
            var user = User.GetActiveEmployees().OrderByDescending(i => i.UserName).FirstOrDefault();
            ProvisioningEmailHelper.NotifyPassOfficeAboutOnBoard(user);
        }
        static void PassOfficeOffBoardNotification()
        {
            var user = User.GetActiveEmployees().OrderByDescending(i => i.UserName).FirstOrDefault();
            ProvisioningEmailHelper.NotifyPassOfficeAboutOffboard(user);
        }
        static void SRMSupportTeamNotification()
        {
            var accessRequest = AccessRequest.GetByProgressStatus(DI.Chakra.Infrastructure.Enums.AccessRequestProgress.Completed).FirstOrDefault();
            ProvisioningEmailHelper.NotifySRMSupportTeam(accessRequest, "test21", "abcdefgh", "Provisioning");
        }
        static void AdvanceDelegation()
        {
            var taskD = TaskDelegation.GetAll().Where(i=>i.IsActive).OrderByDescending(i=>i.LastModifiedDate).FirstOrDefault();
            DI.Chakra.Dto.TaskDelegation task = new DI.Chakra.Dto.TaskDelegation();
            task.Id = taskD.Id;
            task.ActionUser = taskD.ActionUser;
            task.DelegatedTo = taskD.DelegatedTo;
            task.DelegationComments = taskD.DelegationComments;
            task.CreatedBy = taskD.CreatedBy;
            task.LastModifiedBy = taskD.LastModifiedBy;
            task.StartDate = taskD.StartDate;
            task.EndDate = taskD.EndDate;
            var emailInfo = EmailInfo.GetEmailInfoForAdvanceDelegation(task);
            ProvisioningEmailHelper.SendEmailForAdvanceDelegation(emailInfo);
        }
        
        static void NotifyForNewResponseGroup()
        {
            try
            {
                var endpoint = DI.Chakra.DomainModel.EndPoint.GetEndPointByApplicationType(DI.Chakra.Infrastructure.Enums.ApplicationType.MicrosoftTeams);
                var SkypeForBusinessConnector = new SkypeForBusinessConnector(endpoint,null,null);
                SkypeForBusinessConnector.SendEmail("ResponseGroup1,ResponseGroup1");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        static void AdhocProvisioningAmendment()
        {
            AccessRequest accessRequest = null;
            if (!string.IsNullOrWhiteSpace(requestId))
            {
                accessRequest = AccessRequest.GetById(Convert.ToInt64(requestId), true);
            }
            else
            {
                accessRequest = AccessRequest.GetByProgressStatus(DI.Chakra.Infrastructure.Enums.AccessRequestProgress.Completed).FirstOrDefault();
            }
            
            var user = User.GetById(accessRequest.UserId);
            ProvisioningEmailHelper.SendEmailAfterAdhocAmendmentAdd(user, accessRequest, "New Entitlement", user.AccountName);
        }

        static void AdhocProvisioningUserRemoval()
        {
            AccessRequest accessRequest = null;
            if (!string.IsNullOrWhiteSpace(requestId))
            {
                accessRequest = AccessRequest.GetById(Convert.ToInt64(requestId), true);
            }
            else
            {
                accessRequest = AccessRequest.GetByProgressStatus(DI.Chakra.Infrastructure.Enums.AccessRequestProgress.Completed).FirstOrDefault();
            }
            var user = User.GetById(accessRequest.UserId);
            var endpoint = EndPoint.GetById(accessRequest.EndpointSystemId.Value);
            ProvisioningEmailHelper.SendEmailAfterAdhocUserRemoval(user, endpoint, accessRequest, user.AccountName);
        }

        static void DiscoveryJobFailed()
        {
            //var process = Process.GetAll().FirstOrDefault();
            //DiscoveryService service = new DiscoveryService(process);
            //service.m_endpoint = EndPoint.GetByApplicationType(DI.Chakra.Infrastructure.Enums.ApplicationType.ADSingaporePowerLocal);
            //service.m_endpoint.EndPointSystemType = EndPointSystemType.GetByEndpointSystemTypeName(DI.Chakra.Infrastructure.Enums.EndpointType.ActiveDirectory);
            //service.SendEmail(process.ProcessType.ToString(), new DiscoveryLogger(1, 1, DI.Chakra.Infrastructure.Enums.EndpointType.ActiveDirectory));
        }

        static void EscalateAdhocProvisioningApproval()
        {
            SendEscalationEmail sendEscalationEmail = new SendEscalationEmail();
            var task = DI.Chakra.DomainModel.Task.GetById(1, true);
            var wf = WorkflowConfiguration.GetById(task.WorkflowConfigurationId, true);
            sendEscalationEmail.SendEmailForAdhocProvisioning(task, wf);
        }

        static void NotificationForNewUser()
        {
            var user = User.GetAll().LastOrDefault();
            GrantProvisioning grantProvisioning = new GrantProvisioning();
            grantProvisioning.SendEmailForUser(user);
        }
        static void OUChangeRequestNotification()
        {
            var user = User.GetAll().LastOrDefault();
            ProvisioningEmailHelper.SendEmailAfterExecutingOUChangeRequest(user);
        }
        static void NotifyAdhocProvisioningApproval()
        {
            var currentTask = DI.Chakra.DomainModel.Task.GetById(1, true);
            currentTask.AccessRequest.AdHocAccessRequest = AdHocAccessRequest.GetById(currentTask.AccessRequest.AdHocAccessRequestId.Value, true);
            var emailInfo = EmailInfo.GetEmailInfoForSendingEmailForProvisioningRequest(currentTask);
            emailInfo.Save(MasterConfigHelper.ServiceUser);
        }

        static void AccessRequestRejected()
        {
            var currentTask = DI.Chakra.DomainModel.Task.GetById(1, true);
            currentTask.AccessRequest.AdHocAccessRequest = AdHocAccessRequest.GetById(currentTask.AccessRequest.AdHocAccessRequestId.Value, true);
            ProvisioningEmailHelper.SendEmailForAccessRequestRejected(currentTask);

            var bulkTask = DI.Chakra.DomainModel.Task.GetBulkRequestTask(117);
            ProvisioningEmailHelper.SendEmailForAccessRequestRejected(bulkTask);
        }
        static void OffBoardingUserNotification()
        {
            var user = User.GetAll().LastOrDefault();
            ProvisioningEmailHelper.SendEmailAfterOffBoardingTheUser(user);
        }

        static void DeleteNonConnectedSystemNotification()
        {
            var user = User.GetAll().LastOrDefault();
            var endpoints = new List<string> { "Active Directory,User1", "SAP,User1" };
            ProvisioningEmailHelper.SendEmailForDeletingNonConnectedAccount(user, endpoints, DI.Chakra.Infrastructure.Enums.EmployeeEvents.OUChange);
            ProvisioningEmailHelper.SendEmailForDeletingNonConnectedAccount(user, endpoints, DI.Chakra.Infrastructure.Enums.EmployeeEvents.Resignation);
        }

        static void AlertNotification()
        {
            var user = User.GetAll().LastOrDefault();
            ProvisioningValidator.AlertITAdmin(user, "Test message", DI.Chakra.Infrastructure.Enums.EmployeeEvents.Resignation);
        }

        static void AccessRequestFailureNotification()
        {
            var accessRequest = AccessRequest.GetByProgressStatus(DI.Chakra.Infrastructure.Enums.AccessRequestProgress.ExecutionFailed).FirstOrDefault();
            var user = User.GetById(accessRequest.UserId);
            ProvisioningEmailHelper.AccessRequestFailureNotification(accessRequest, user);
        }
        #endregion
    }
}
