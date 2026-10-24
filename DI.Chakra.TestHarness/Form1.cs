using DI.Chakra.Connector;
using DI.Chakra.DomainModel;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Service.MiscProcessor;
using DI.Chakra.Services.AttestationProcessor;
using DI.Chakra.Services.DiscoveryProcessor;
using DI.Chakra.Services.EmailingProcessor;
using DI.Chakra.Services.MiscProcessor;
using DI.Chakra.Services.ProvisioningProcessor;
using DI.Chakra.Web;
using DI.Chakra.Workflow.Processor;
using DI.Chakra.Workflow.Processor.Common;
using DI.Chakra.Workflow.ServiceHelpers;
using Newtonsoft.Json;
//using iTextSharp.text;
//using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;


namespace TestHarness
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private static Dictionary<int, string> subordinateList = new Dictionary<int, string>();
        private bool MapAll;

        private void Form1_Load(object sender, EventArgs e)
        {
            //Task task = _taskFacade.GetAttestationTask(4670, true);
            //task.TaskReviews = null;
            //var lightTask = task;

            //var x = _taskFacade.GetDelegatedUserForAttestationByUserId(2);

            //TestDA _testDA = new TestDA();
            //this.textBox1.Text = _testDA.GetLatestAttestationProcessId().ToString();
            //this.textBox2.Text = _testDA.GetLatestTaskId().ToString();

            string today = DateTime.Today.ToString("yyyyMMdd");

            //new TaskFacade().IsUserReceivedEmailToday(1, 2);

            string dn = "CN=ne-aiwah,OU=Users,OU=Revenue Collection,OU=Finance & Accounts,OU=HQ,OU=SPS,DC=singaporepower,DC=local";
            string ou = dn.Substring(dn.IndexOf("OU="), dn.Length - dn.IndexOf("OU="));

            string ou1 = "Users/Revenue Collection/Finance & Accounts/HQ/SPS";
            string[] ous = ou1.Split('/');
            string finalOU = string.Empty;

            foreach (string item in ous)
            {
                finalOU += "OU=" + item + ",";
            }

            var x = finalOU;

            var desc = UtilityMethods.GetEnumAttribute<Description>(EndpointType.ActiveDirectory);
            var tableName = UtilityMethods.GetEnumAttribute<TableName>(EndpointType.ActiveDirectory);

            //var group = new GroupFacade().GetGroupByName("IACMAdminGroup");

            //var tasks = _taskFacade.GetTaskDetailsByActionUserId(750);

            //var sapRoles = new SAPFacade().GetSAPRolesByUserId(8);

            //var triggers = new UserFacade().GetEmployeeEventTrigger(10328, EmployeeEvents.NewJoinee, DateTime.Today);

            //var re = MasterConfigHelper.CanRunOnBoardTrigger;

            //var wfConfig = new WorkFlowConfigurationFacade().GetWorkFlowConfigurationById(1, true);

            var xx = DateTime.Today.AddDays(-3);


            System.Version version = AssemblyName.GetAssemblyName("TestHarness.exe").Version;
            this.Text = "Version : " + version.ToString();
            //Assembly.GetEntryAssembly().
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var x = Attestation.GetByIdIncludingAttestationProcesses(6);
            var y = Attestation.GetAll();
        }


        private void button11_Click(object sender, EventArgs e)
        {
            int processId = int.Parse(this.textBox1.Text.Trim());
            //Attestation.ResetAttestationProcess(processId);
            MessageBox.Show("TBD");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int processId = int.Parse(this.textBox1.Text.Trim());
                var process = Process.GetByProcessType(processId, ProcessType.Attestation);
                AttestationService _service = new AttestationService();

                _service.StartWorkFlow(process);
                MessageBox.Show("Tasks created.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}" + Environment.NewLine + "{1}" + Environment.NewLine + "{2}" + Environment.NewLine + "{3}", ex.Message, ex.StackTrace, ex.InnerException, ex.Source));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            long taskId = long.Parse(this.textBox2.Text.Trim());

            var tasks = Task.GetAll();

            var pendingTasks = tasks.Where(item => !item.IsCompleted && item.IsActive);


            foreach (Task tsk in tasks)
            {
                Task task = Task.GetAttestationTask(tsk.Id, true);
                task.LastModifiedBy = "Sandeep";
                task.LastModifiedDate = DateTime.Now;

                var actions = AttestationSnapshotAction.GetByTask((long)tsk.AttestationCampaignId, task.Id);

                foreach (var item in actions)
                {
                    item.Comment = "Approval Level " + task.WorkflowStateId.ToString();
                    item.Approve = true;
                    item.Revoke = true;
                    item.LastModifiedBy = "Sandeep";
                    item.LastModifiedDate = DateTime.Now;
                }

                PresentationAttestationSnapshotActionsForTask storeTask = new PresentationAttestationSnapshotActionsForTask()
                {
                    AttestationSnapshotActions = actions,
                    TaskID = task.Id
                };

                this.AddTaskToInterimStore(storeTask);

                DI.Chakra.DomainModel.Presentation.PresentationWorkflowCriteriaBase _criteria = new DI.Chakra.DomainModel.Presentation.ApprovalActionCriteria()
                {
                    RequestType = (RequestType)task.RequestTypeId,
                    TaskId = task.Id,
                    WorkflowInstanceId = task.WorkflowInstanceId
                };

                ServiceAccessStrategy _serviceAccessStrategy = new ServiceAccessStrategy(_criteria);
                _serviceAccessStrategy.AccessWorkflowService();
            }

            MessageBox.Show("Task completed.");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            TerminateTask tskterm = new TerminateTask();
            var tsk = Task.GetById(Convert.ToInt64(textBox3.Text));
            tskterm.Execute(tsk.WorkflowInstanceId);
            //WorkFlowConfigurationFacade _wfConfigFacade = new WorkFlowConfigurationFacade();

            //var x = _wfConfigFacade.GetWorkFlowConfigurationById(3, true);
            //var y = _wfConfigFacade.GetWorkFlowConfigurationsByRequestType(RequestType.Attestation);
            //var z = _wfConfigFacade.GetWorkFlowConfigurationsByRequestType(RequestType.Provisioning);

            //var x1 = _wfConfigFacade.GetWorkFlowConfigurationByAttestationId(3, true);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            long taskId = long.Parse(this.textBox2.Text.Trim());

            Task task = Task.GetAttestationTask(taskId, true);
            task.LastModifiedBy = "Rajesh";
            task.LastModifiedDate = DateTime.Now;

            var actions = AttestationSnapshotAction.GetByTask((long)task.AttestationCampaignId, taskId);

            //var store = InterimStore.GetByTaskId(taskId, true, InterimStoreItemType.Task);

            foreach (var action in actions)
            {
                action.LastModifiedBy = "System";
                action.LastModifiedDate = DateTime.Now;
                action.Approve = true;
                action.Revoke = false;
            }

            PresentationAttestationSnapshotActionsForTask storeTask = new PresentationAttestationSnapshotActionsForTask()
            {
                AttestationSnapshotActions = actions,
                TaskID = taskId
            };

            //foreach (TaskReview taskReview in task.TaskReviews)
            //{
            //    taskReview.AttestationReview.LastModifiedBy = "Rajesh";
            //    taskReview.AttestationReview.LastModifiedDate = DateTime.Now;

            //    foreach (ReviewEndpointSystem reviewEndpointSystem in taskReview.AttestationReview.ReviewEndpointSystems)
            //    {
            //        reviewEndpointSystem.AttestationReviewEndpointSystem.Comments = "Approval Level " + task.WorkflowStateId.ToString();
            //        reviewEndpointSystem.AttestationReviewEndpointSystem.Approve = true;
            //        reviewEndpointSystem.AttestationReviewEndpointSystem.LastModifiedBy = "Rajesh";
            //        reviewEndpointSystem.AttestationReviewEndpointSystem.LastModifiedDate = DateTime.Now;

            //        foreach (AttestationReviewEndpointSystemL2Detail l2Detail in reviewEndpointSystem.AttestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details)
            //        {
            //            l2Detail.Comments = "Approval Level " + task.WorkflowStateId.ToString();
            //            l2Detail.Approve = true;
            //            l2Detail.LastModifiedBy = "Rajesh";
            //            l2Detail.LastModifiedDate = DateTime.Now;

            //            foreach (AttestationReviewEndpointSystemL3Detail l3Detail in l2Detail.AttestationReviewEndpointSystemL3Details)
            //            {
            //                l3Detail.Comments = "Approval Level " + task.WorkflowStateId.ToString();
            //                l3Detail.Approve = true;
            //                l3Detail.LastModifiedBy = "Rajesh";
            //                l3Detail.LastModifiedDate = DateTime.Now;
            //            }
            //        }
            //    }
            //}

            this.AddTaskToInterimStore(task);
        }

        private void AddTaskToInterimStore(Task task)
        {
            var xmlData = UtilityMethods.Serialize<Task>(task);

            InterimStore interimStore = new InterimStore()
            {
                InterimStoreItemType = InterimStoreItemType.Task,
                IsActive = true,
                TaskId = task.Id,
                XmlData = xmlData,
                IsSaved = true

            };
            interimStore.Save(MasterConfigHelper.ServiceUser);
        }

        private void AddTaskToInterimStore(PresentationAttestationSnapshotActionsForTask task)
        {
            var xmlData = UtilityMethods.Serialize<PresentationAttestationSnapshotActionsForTask>(task);

            InterimStore interimStore = new InterimStore()
            {
                InterimStoreItemType = InterimStoreItemType.Task,
                IsActive = true,
                TaskId = task.TaskID,
                XmlData = xmlData,
                IsSaved = true

            };
            interimStore.Save(MasterConfigHelper.ServiceUser);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            long taskId = long.Parse(this.textBox2.Text.Trim());
            var interimStore = InterimStore.GetByTaskId(taskId, true, InterimStoreItemType.Task);
            var task = UtilityMethods.Deserialize<Task>(interimStore.XmlData);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            long taskId = long.Parse(this.textBox2.Text.Trim());
            var interimStore = InterimStore.GetByTaskId(taskId, true, InterimStoreItemType.Task);

            PresentationAttestationSnapshotActionsForTask action = null;

            if (interimStore != null)
            {
                action = UtilityMethods.Deserialize<PresentationAttestationSnapshotActionsForTask>(interimStore.XmlData);
                Task task = Task.GetById(taskId);
                foreach (var sa in action.AttestationSnapshotActions)
                {
                    sa.Approve = true;
                    sa.Revoke = false;
                    sa.Comment = "Approval Level " + task.WorkflowStateId.ToString();
                    sa.LastModifiedBy = "Sandeep";
                    sa.LastModifiedDate = DateTime.Now;
                }

                var xmlData = UtilityMethods.Serialize<PresentationAttestationSnapshotActionsForTask>(action);
                interimStore.XmlData = xmlData;
                interimStore.Save(MasterConfigHelper.ServiceUser);
            }
            // 

            //var task = UtilityMethods.Deserialize<Task>(interimStore.XmlData);

            //task.LastModifiedBy = "Rajesh";
            //task.LastModifiedDate = DateTime.Now;

            //foreach (TaskReview taskReview in task.TaskReviews)
            //{
            //    taskReview.AttestationReview.LastModifiedBy = "Rajesh";
            //    taskReview.AttestationReview.LastModifiedDate = DateTime.Now;

            //    foreach (ReviewEndpointSystem reviewEndpointSystem in taskReview.AttestationReview.ReviewEndpointSystems)
            //    {
            //        reviewEndpointSystem.AttestationReviewEndpointSystem.Comments = "Approval Level " + task.WorkflowStateId.ToString();
            //        reviewEndpointSystem.AttestationReviewEndpointSystem.Approve = true;
            //        reviewEndpointSystem.AttestationReviewEndpointSystem.LastModifiedBy = "Rajesh";
            //        reviewEndpointSystem.AttestationReviewEndpointSystem.LastModifiedDate = DateTime.Now;

            //        foreach (AttestationReviewEndpointSystemL2Detail l2Detail in reviewEndpointSystem.AttestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details)
            //        {
            //            l2Detail.Comments = "Approval Level " + task.WorkflowStateId.ToString();
            //            l2Detail.Approve = true;
            //            l2Detail.LastModifiedBy = "Rajesh";
            //            l2Detail.LastModifiedDate = DateTime.Now;

            //            foreach (AttestationReviewEndpointSystemL3Detail l3Detail in l2Detail.AttestationReviewEndpointSystemL3Details)
            //            {
            //                l3Detail.Comments = "Approval Level " + task.WorkflowStateId.ToString();
            //                l3Detail.Approve = true;
            //                l3Detail.LastModifiedBy = "Rajesh";
            //                l3Detail.LastModifiedDate = DateTime.Now;
            //            }
            //        }
            //    }
            //}

            //var xmlData = UtilityMethods.Serialize<Task>(task);
            //interimStore.XmlData = xmlData;
            //interimStore.Save(MasterConfigHelper.ServiceUser);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            long taskId = long.Parse(this.textBox2.Text.Trim());
            InterimStore.DeleteInterimStoreByTaskId(taskId);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            long taskId = long.Parse(this.textBox3.Text.Trim());

            Task task = Task.GetProvisioningTask(taskId);
            task.LastModifiedBy = "Rajesh" + task.WorkflowStateId.ToString();
            task.LastModifiedDate = DateTime.Now;

            task.AccessRequest.Comments = "Test Approve.";
            task.AccessRequest.RequestStatus = AccessRequestStatus.Approved;
            task.AccessRequest.LastModifiedBy = "Rajesh" + task.WorkflowStateId.ToString();
            task.AccessRequest.LastModifiedDate = DateTime.Now;

            this.AddTaskToInterimStore(task);

            DI.Chakra.DomainModel.Presentation.PresentationWorkflowCriteriaBase _criteria = new DI.Chakra.DomainModel.Presentation.ApprovalActionCriteria()
            {
                RequestType = (RequestType)task.RequestTypeId,
                TaskId = taskId,
                WorkflowInstanceId = task.WorkflowInstanceId,
            };

            ServiceAccessStrategy _serviceAccessStrategy = new ServiceAccessStrategy(_criteria);
            _serviceAccessStrategy.AccessWorkflowService();

            MessageBox.Show("Task completed.");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //ComplianceProcess pro = new ComplianceProcess();
            //pro.CreateTasks(7);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            try
            {
                //var usr = NHRMSUser.GetById(42050);
                //PeopleSoftHRMSUserInfo.ProcessBackdatedUser(usr, EmployeeEvents.NewJoinee, null, 16, null, DateTime.Today);

                int processId = int.Parse(this.textBox4.Text.Trim());
                var process = Process.GetByProcessType(processId, ProcessType.Discovery);
                DiscoveryService _discoveryService = new DiscoveryService(process);
                _discoveryService.Start();
                MessageBox.Show("Discovery Completed.");
                //var mails = EmailInfo.GetById(1);
                //MailServerConfigInfo server = new MailServerConfigInfo();
                //EmailSender sender1 = new EmailSender(server);
                //sender1.Send(mails);

                // bool isMailSent = new EmailComposer().Compose(EmailTemplate.RequestWithdrawn, 551);
                //isMailSent = new EmailComposer().Compose(EmailTemplate.RemindAttestationApproval, 118);

                //OffboardMailNotification job = new OffboardMailNotification();
                //job.BuildPreConditionResultSet();
                //job.Execute();

                //PromotionMailNotification job1 = new PromotionMailNotification();
                //job1.BuildPreConditionResultSet();
                //job1.Execute();

                //TransferMailNotification job3 = new TransferMailNotification();
                //job3.BuildPreConditionResultSet();
                //job3.Execute();
                //new EmailComposer().Compose(EmailTemplate.EntitlementExtension, 691);
                //new EmailComposer().Compose(EmailTemplate.InstallationNotification, 10753);
                //new EmailComposer().Compose(EmailTemplate.AccessRequestFailureNotification, 10753);
                //new EmailComposer().Compose(EmailTemplate.AccessRequestRejected, 10753);
                //bool isMailSent = new EmailComposer().Compose(EmailTemplate.EscalateAttestationApproval, 501);
                //isMailSent = new EmailComposer().Compose(EmailTemplate.EscalateAdhocProvisioningApproval, 551);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}" + Environment.NewLine + "{1}" + Environment.NewLine + "{2}" + Environment.NewLine + "{3}", ex.Message, ex.StackTrace, ex.InnerException, ex.Source));
            }
        }

        private void AdhochJob_Click(object sender, EventArgs e)
        {
            try
            {


            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}" + Environment.NewLine + "{1}" + Environment.NewLine + "{2}" + Environment.NewLine + "{3}", ex.Message, ex.StackTrace, ex.InnerException, ex.Source));
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            List<string> content = new List<string>();
            using (StreamReader sr = new StreamReader("C:\\PasswordFilter\\User掤఩⣆言hp.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        content.Add(line);
                    }
                }
            }
            string fileName = "user.txt";
            string userName = fileName.Split('.')[0];
        }

        private void button14_Click(object sender, EventArgs e)
        {

            ProvisioningEngine peManager = new ProvisioningEngine();

            peManager.StartProvisioning();
            // peManager.StartTrustedSourcePropagation();
            //provisioningTaskHelper.CreateBulkProvisioningTasks();
            // var test = DI.Chakra.DomainModel.AccessRequest.GetByAccountType(1, AccountType.User, 5).ToList();

            MessageBox.Show("Done.");
        }

        private void button15_Click(object sender, EventArgs e)
        {
            //var x = configFacade.GetPasswordSyncConfigDetails();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            decimal health = Task.GetAttestationHealth(30);
            MessageBox.Show(health.ToString());
        }

        private void button17_Click(object sender, EventArgs e)
        {
            Task.CreateTriggers();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            //ProvisioningFlowHelper.CreateOnBoardingTasksIfAdminCheckNotRequired();
            //MessageBox.Show("Done.");
        }

        private void button19_Click(object sender, EventArgs e)
        {
            //ProvisioningEngine proveng = new ProvisioningEngine();
            //proveng.StartTrustedSourcePropagation();
            //MessageBox.Show("Done.");

            ProvisioningEngine provEng = new ProvisioningEngine();
            provEng.StartTrustedSourcePropagation();
            MessageBox.Show("Done.");
        }

        //private string GetRolesFromAdhocRequest(User usr, string existingRoles, AdHocSAPRequest sapDetail)
        //{
        //string ItRoles = String.Empty;
        //List<string> TechnicalRoles = new ProvisioningConfigurationFacade().GetHrFlinsTechnicalRolesByAdhocRequestObject(sapDetail);

        //foreach (string technicalRole in TechnicalRoles)
        //{
        //    if (string.IsNullOrEmpty(ItRoles))
        //        ItRoles = technicalRole;
        //    else
        //        ItRoles = ItRoles + "," + technicalRole;
        //}

        //if (string.IsNullOrEmpty(existingRoles))
        //{
        //    return ItRoles;
        //}
        //else
        //{
        //    return existingRoles + "," + ItRoles;
        //}

        //return string.Empty;
        //}

        //public void UpdateUserBusinessRoleDetails(int UserId, AdHocSAPRequest sapDetail, string Roles)
        //{
        //ProvisioningRoleFacade ProvisioningFacade = new ProvisioningRoleFacade();
        //ProvisioningFacade.UpdateUserBusinessRoleDetails(UserId, sapDetail, Roles);
        //}

        private void button20_Click(object sender, EventArgs e)
        {
            //EndPoint _endpoint = new EndPoint();
            //_endpoint.EndpointSystemTypeId = (int) EndpointType.SAP;
            //_endpoint.EndPointSystemType = new EndPointSystemType();
            //_endpoint.EndpointType.EndpointSystemTypeName = "SAP";
            //DILog.ProvisioningLogger.WriteLog(null, null, _endpoint, LogLevelType.Info, "Testing", null, ProvisioningEventType.OnBoarding, "TestHarness");

            //AdHocSAPHRFLINSRequest sapHRAdhocRequest = new AdHocSAPHRFLINSRequest();
            //sapHRAdhocRequest.AdHocSAPHRFLINSRequestDetails = new List<AdHocSAPHRFLINSRequestDetail>();
            //AdHocSAPHRFLINSRequestDetail sapDetail = new AdHocSAPHRFLINSRequestDetail();

            ////UpdateCoReportDetails(1, "SP", "CCVN", "Y_FR_SPAST_FIN_CCTR-CATU,Y_SAP_FLINSUSER_GENERAL");



            ////sapDetail.Company = "SP";
            ////sapDetail.Entitlement = "GL Clerk";

            ////sapDetail.Company = "SPPA";
            //////sapDetail.CostCenter = "AADM, ACCT, ADML";
            //////sapDetail.PurchaseGroup = "AA1, AA2, AA3";
            ////sapDetail.Plant = "7DA,7DB,7DD";
            ////sapDetail.Entitlement = "Disp & Rpt Goods Rec (App)";

            //sapHRAdhocRequest.AdHocSAPHRFLINSRequestDetails.Add(sapDetail);

            //AdHocPLRELearningRequest sapElearningAdhoc = new AdHocPLRELearningRequest();
            //sapElearningAdhoc.Module = "ITD";
            //sapElearningAdhoc.SubModule = "ITD";
            //sapElearningAdhoc.Entitlement = "Y_HR_SPHCO_ISD_OPERATIONS";

            //AdHocPERERecruitmentRequest sapRecruitmentRequest = new AdHocPERERecruitmentRequest();
            //sapRecruitmentRequest.Module = "HR";
            //sapRecruitmentRequest.SubModule = "E-Recruitment";
            //sapRecruitmentRequest.Entitlement = "E-Recruitment - Administrator";

            //AdHocSAPPIRequest sapPiRequest = new AdHocSAPPIRequest();
            //sapPiRequest.Module = "IT";
            //sapPiRequest.Entitlement = "Security Audit Configuration";

            //AdHocSAPGRCRequest sapGRCRequest = new AdHocSAPGRCRequest();
            //sapGRCRequest.Module = "GRC";
            //sapGRCRequest.SubModule = "EAM";
            //sapGRCRequest.Entitlement = "EAM - Controller Role";

            //List<string> Elearningroles = new RoleMatrixDA().GetTechnicalRolesForSapELearning(sapElearningAdhoc);
            //List<string> ERecruitmentroles = new RoleMatrixDA().GetTechnicalRolesForSapERecruitment(sapRecruitmentRequest);
            //List<string> piRoles = new RoleMatrixDA().GetTechnicalRolesForSapPi(sapPiRequest);
            //List<string> grcRoles = new RoleMatrixDA().GetTechnicalRolesForSapGrc(sapGRCRequest);

            User usr = new User();

            //string lstTechnicalRoles = GetRolesFromAdhocRequest(usr, "aaaa,bbb,cccc", sapElearningAdhoc);

            //List<string> lstTechnicalRoles = new ProvisioningConfigurationDA().GetHrFlinsTechnicalRolesByAdhocRequestObject(sapHRAdhocRequest);


            //ProvisioningConfigurationDA provDa = new ProvisioningConfigurationDA();

            //List<string> Columns = new List<string>() { "CompanyCode", "DepartmentCode", "DivisionCode", "CostCenterCode" };

            //List<string> values = new List<string>() { "SP", "SP18", "*", "*" };
            //List<string> PurchasingGroups = new List<string>() { "AA1", "AA2", "AA3", "AA4" };

            //string role = provDa.FindMatchedConfigurationMatrix("DI_M_ProvisioningConfigMatrixSAPORG", Columns, values, "DefaultHRMSGeneralRoles", 1);
            //string role = provDa.FindMatchedConfigurationMatrix("DI_M_ProvisioningConfigMatrixSAPORG", Columns, PurchasingGroups, "DefaultHRMSGeneralRoles", 1);
            //string group = provDa.FindMatchedAsterixWord("DI_M_ProvisioningConfigMatrixSAPORG", Columns, values, "DefaultEPGroup");


            //List<string> Columns2 = new List<string>() { "DepartmentCode", "PayRollArea", "EmployeeGroup" };

            //List<string> values2 = new List<string>() { "SP18", "P1", "Exec" };

            //string role2 = provDa.FindMatchedConfigurationMatrix("DI_M_ProvisioningConfigMatrixSAPEEGroup", Columns2, values2, "DefaultRole", 1);

        }

        private void button21_Click(object sender, EventArgs e)
        {
            List<AdHocAccessRequest> requests = new List<AdHocAccessRequest>();

            AdHocAccessRequest request1 = new AdHocAccessRequest()
            {
                AccountType = AccountType.User,
                Id = 1,
                AdhocActionType = AdHocActionType.AmendmentAdd,
                Comments = "sdfasdf",
                CreatedBy = "System",
                CreatedDate = DateTime.Now,
                EndpointSystemId = 1,
                EndpointSystemType = EndpointType.ActiveDirectory,
                IsSelfRequest = true,
            };

            AdHocAccessRequest request2 = new AdHocAccessRequest()
            {
                AccountType = AccountType.User,
                Id = 2,
                AdhocActionType = AdHocActionType.AmendmentRemove,
                Comments = "sdfasdf",
                CreatedBy = "System",
                CreatedDate = DateTime.Now,
                //EndpointSystemId = 1,
                //EndpointSystemType = EndpointType.LotusNotes,
                IsSelfRequest = true,
            };

            requests.Add(request1);
            requests.Add(request2);

            var x = UtilityMethods.SerializeCollection<AdHocAccessRequest>(requests);
            var y = UtilityMethods.DeserializeCollection<AdHocAccessRequest>(x);

            var x1 = UtilityMethods.Serialize<AdHocAccessRequest>(request1);
            var y1 = UtilityMethods.Deserialize<AdHocAccessRequest>(x1);
        }

        private void button22_Click(object sender, EventArgs e)
        {
            //ProvisioningFlowHelper.CreateOffBoardingTasksIfAdminCheckNotRequired();
            //MessageBox.Show("Done.");
        }

        private void button23_Click(object sender, EventArgs e)
        {
            //ProvisioningFlowHelper.CreateOUChangeTasksIfAdminCheckNotRequired();
            //MessageBox.Show("Done.");


        }

        private void button24_Click(object sender, EventArgs e)
        {
            var user = User.GetById(7104);

            DI.Chakra.DomainModel.Presentation.PresentationOrgChangeTrack orgTrack = new DI.Chakra.DomainModel.Presentation.PresentationOrgChangeTrack();
            orgTrack.Event = EmployeeEvents.OUChange;
            orgTrack.EffectiveDate = DateTime.Today;

            orgTrack.CompanyCode = "SP";
            orgTrack.DivisionCode = "0022";
            orgTrack.DepartmentCode = "SP17";
            orgTrack.SectionCode = "10002522";
            orgTrack.CostCenter = "MSSY";
            orgTrack.CompanyId = 28;
            orgTrack.DepartmentId = 147;
            orgTrack.DivisionId = 649;
            orgTrack.OrganisationId = 1069;

            EmployeeEventTrigger trigger = new EmployeeEventTrigger()
            {

                UserId = user.Id,
                EventType = EmployeeEvents.OUChange,
                EffectiveDate = DateTime.Now,
                IsRequestCreated = false,
                IsAdminCheckRequired = false,
                IsActive = true,
                CreatedBy = "TestHarness",
                CreatedDate = DateTime.Now,
                LastModifiedBy = "TestHarness",
                LastModifiedDate = DateTime.Now,

            };

            trigger.UserDetails = UtilityMethods.Serialize<User>(user);
            trigger.Save(MasterConfigHelper.ServiceUser);

        }

        private void button25_Click(object sender, EventArgs e)
        {
            //int Days = Convert.ToInt32(textBox5.Text);
            // HouseKeepingHelper housekeepingHelper = new HouseKeepingHelper();
            //  housekeepingHelper.StartProcess();
            PurgeAgent housekeepingprocess = new PurgeAgent();
            housekeepingprocess.StartProcess();
            MessageBox.Show("Done");
        }

        private void button26_Click(object sender, EventArgs e)
        {

            new ResolveExceptions().Show();
        }

        private void button27_Click(object sender, EventArgs e)
        {

            ProvisioningEngine peManager = new ProvisioningEngine();
            //MessageBox.Show("Started");
            peManager.StartProvisioningTest(Convert.ToInt32(txtAcReq.Text.ToString()));
            MessageBox.Show("Done");
        }

        private void button28_Click(object sender, EventArgs e)
        {
            //  new RequestExecutor().CreateHomeFolderForADUsers();

            //try
            //{
            //   var ep = DI.Chakra.DomainModel.EndPoint.GetById(2);
            //    var ADConnector = ConnectorFactory.GetConnector(ep, null);
            //    ADConnector.CreateAccount(item.sAMAccountName);


            //    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"AssignRightstoHomeFolder for User {item.sAMAccountName} Ended.");
            //}
            //catch (Exception ex)
            //{
            //    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, $"Error while assigning home folder for AD user {item.sAMAccountName}.", ex);
            //}
        }

        private bool SendTestEmail(DI.Chakra.DomainModel.MailServerConfiguration serverConfig, string toAddress)
        {
            bool isError = false;
            try
            {
                MailTemplate template = MailTemplate.GetByTemplateName("test");

                SmtpClient testSMPT = new SmtpClient(serverConfig.HostName);
                string sub = "Test email from  IACM Mail server Configuration.";
                string body = template.MailContent;
                string fromAddress = serverConfig.EmailID;
                MailMessage msg = new MailMessage(fromAddress, toAddress, sub, body);
                testSMPT.EnableSsl = serverConfig.EnableSSL;
                testSMPT.Port = int.Parse(serverConfig.PortNumber);
                if (!string.IsNullOrEmpty(serverConfig.UserName) && !string.IsNullOrEmpty(serverConfig.Password))
                    testSMPT.Credentials = new System.Net.NetworkCredential(serverConfig.UserName, serverConfig.Password);
                else
                    testSMPT.UseDefaultCredentials = false;

                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                testSMPT.Send(msg);
                MessageBox.Show(string.Format("Successfully Sent to {0}", toAddress));
                isError = true;
            }
            catch (Exception)
            {
                throw;
            }
            return isError;
        }

        private void button29_Click(object sender, EventArgs e)
        {
            //User user = new UserFacade().GetUser(5707);
            //IList<string> approverConfiguration = new XLDataFacade().GetAllConfigurationOfApprover(user.EmployeeId);

            //if (approverConfiguration != null && approverConfiguration.Count > 0)
            //    ProvisioningFlowHelper.SendEmailAfterOffBoardingTheApprover(user);

            AccountReconcilation acctRecon = AccountReconcilation.GetByUser(2, 28);

            if (acctRecon == null)
                return;

            acctRecon.UserId = 1;
            acctRecon.OffboardingAccountAction = OffboardingAccountActionType.Supervisor;
            acctRecon.AccountType = AccountType.Secondary;
            acctRecon.Save(MasterConfigHelper.ServiceUser);

        }

        private void button30_Click(object sender, EventArgs e)
        {

           

            DI.Chakra.DomainModel.EndPoint m_endpoint = DI.Chakra.DomainModel.EndPoint.GetByApplicationType(ApplicationType.EAM);

            IdmsLogger logger = new DiscoveryLogger(61, 0, EndpointType.IDMS, m_endpoint.DisplayName);

            ConnectorBase connector = ConnectorFactory.GetConnector(m_endpoint, null, logger);
            connector.DiscoveryProcess = DiscoveryProcess.GetById(61);
            connector.Process = Process.GetByProcessTypeKeyId(61, ProcessType.Discovery);
            connector.GetData();
        }

        private void button31_Click(object sender, EventArgs e)
        {
            GetSubordinate(2990);
        }

        private void GetSubordinate(int UserID)
        {
            Dictionary<int, string> data;
            data = new Dictionary<int, string>();
            //data = _userFacade.GetReportingofSubordinate(UserID, "");
            if (data.Count == 0) { return; }
            else
            {
                foreach (var key in data.Keys)
                {
                    if (!subordinateList.Keys.Contains(key))
                    {
                        subordinateList.Add(key, data[key]);
                    }
                }
                foreach (int userid in data.Keys)
                {
                    GetSubordinate(userid);
                }
            }
        }

        private void button32_Click(object sender, EventArgs e)
        {
            //AdHocSAPHRFLINSRequestDetail requestDetail = new AdHocSAPHRFLINSRequestDetail()
            //{
            //    Company = "SPPA",
            //    CostCenter = null,
            //    Entitlement = "WO Approving Officer",
            //    EntitlementType = EntitlementType.Role,
            //    RetainCompanyRoles = true,
            //    Module = null,
            //    MRPController = null,
            //    Plant = null,
            //    PurchaseGroup = "AA1,AA2",
            //    PurchaseOrg = null,
            //    StorageLocation = null,
            //    SubModule = null
            //};

            //var x = new ProvisioningConfigurationFacade().GetFLINSMatrixByRequest(requestDetail);

        }

        private void button33_Click(object sender, EventArgs e)
        {
            // new RequestExecutor().AssignEPGroup();
        }

        private void button34_Click(object sender, EventArgs e)
        {
            //new RequestExecutor().AddFLINSLoginIdInHR();
        }

        private void button35_Click(object sender, EventArgs e)
        {
            //new RequestExecutor().PrepareTelephoneDirectorySyncData();
            MessageBox.Show("Done.");
        }

        private void button36_Click(object sender, EventArgs e)
        {
            //List<Scheduler> dailyJobs = new SchedulerDA().GetCurrentJobs(ScheduleJobType.Discovery);
            //List<DiscoveryProcess> timeOutJobs = new SchedulerDA().GetTimeOutDiscoveryJobs();
            //foreach (DiscoveryProcess discoveryProcess in timeOutJobs)
            //{
            //    new DiscoveryService().ResetDiscoveryProcess(discoveryProcess);
            //}

            //DiscoveryEngine DiscoveryEngine = new DiscoveryEngine();
            //DiscoveryEngine.ResetStatusForAllInProcessJobs();
        }


        private void button37_Click(object sender, EventArgs e)
        {
            try
            {
                var x = ApprovalUserTypes.GetByIsActive();

                foreach (ApprovalUserTypes item in x)
                {
                    MessageBox.Show(item.Id + " - " + item.ApprovalUserTypeDesc);
                }
            }
            catch
            {
                throw;
            }

        }

        private void button38_Click(object sender, EventArgs e)
        {
            var x = RSAHelper.DecryptData(this.textBox6.Text.Trim());
            MessageBox.Show(x);
        }

        private void button39_Click(object sender, EventArgs e)
        {
            var x = RSAHelper.EncryptData(textBox6.Text.Trim());
            textBox6.Text = x;
        }

        private void button40_Click(object sender, EventArgs e)
        {
            var nonConnectedSystems = DI.Chakra.DomainModel.EndPoint.GetNonConnectedAccount(5333, LockScenario.OffBoard);
            var user = User.GetById(5333);
            // ProvisioningFlowHelper.SendEmailForDeletingNonConnectedAccount((int)EmailTemplate.DeleteNonConnectedSystemNotificationResignation,user.Id,user, nonConnectedSystems, EmployeeEvents.Resignation);
            //new EmailComposer().Compose(EmailTemplate.DeleteNonConnectedSystemNotificationResignation, user.Id);
            //ProvisioningFlowHelper.SendEmailAfterOffBoardingTheUser((int)EmailTemplate.OffBoardingUserNotification,user.Id, user);
            //new EmailComposer().Compose(EmailTemplate.OffBoardingUserNotification, user.Id);
        }

        private void button41_Click(object sender, EventArgs e)
        {
            var user = User.GetById(3306);
            //var x = new ProvisioningConfigurationFacade().GetSAPEPGroupBasedOnUserOrg(user);
        }

        private void button42_Click(object sender, EventArgs e)
        {
            //var usr = new UserFacade().GetUser(5431);

            //ProvisioningConfigurationFacade ProvConfigFacade = new ProvisioningConfigurationFacade();

            //string RolesBasedOnEEGroup = string.Empty;
            //string RolesBasedOnPosition = string.Empty;

            //string RolesBasedOnOrg = ProvConfigFacade.GetSAPRolesBasedOnUserOrg(usr);

            //if (!string.IsNullOrEmpty(usr.EEGroup))
            //    RolesBasedOnEEGroup = ProvConfigFacade.GetSAPRolesBasedOnEEGroup(usr);
            //if (!string.IsNullOrEmpty(usr.PositionName))
            //    RolesBasedOnPosition = ProvConfigFacade.GetSAPRolesBasedOnUserPosition(usr);
            //string RolesFromMatrix = RolesBasedOnOrg + "," + RolesBasedOnEEGroup + "," + RolesBasedOnPosition;
            //RolesFromMatrix = RolesFromMatrix.Replace("\n", "");

            //var x = RolesFromMatrix;

            //string x1 = "sp18";
            //string x2 = "*".Replace("*", string.Empty);
            //var x3 = x1.Contains(x2);

            //var y = ProvConfigFacade.GetSAPRolesBasedOnEEGroup(usr);
            //var y1 = ProvConfigFacade.GetSAPRolesBasedOnUserOrg(usr);
            //var y2 = ProvConfigFacade.GetSAPRolesBasedOnUserPosition(usr);

            //var z1 = ProvConfigFacade.GetSAPEPGroupBasedOnUserOrg(usr);

            //var y3 = y + "," + y1 + "," + y2;
        }

        private void button43_Click(object sender, EventArgs e)
        {
            //GEMSFacade gemsFacade = new GEMSFacade();
            //var adsUser = new ActiveDirectoryFacade().GetADUserByUserId(5875);
            ////var x = gemsFacade.GetGEMSGroupsFromADGroups(adsUser);
        }

        private void button44_Click(object sender, EventArgs e)
        {
            var usr = User.GetById(5422);
            // ProvisioningFlowHelper.SendEmailForDeletingNonConnectedAccount((int)EmailTemplate.DeleteNonConnectedSystemNotificationResignation,usr.Id,usr, new List<string>(), EmployeeEvents.Resignation);

            // ProvisioningFlowHelper.SendEmailForDeletingNonConnectedAccount((int)EmailTemplate.DeleteNonConnectedSystemNotificationOUChange,usr.Id, usr, new List<string>(), EmployeeEvents.OUChange);
        }

        private void button45_Click(object sender, EventArgs e)
        {
            int groupId = 0;
            string exceptionRemarks = string.Empty;
            List<SubTask> subTasks = new List<SubTask>();
            List<long> reviewerIds = new List<long>();

            ////var x = DI.Chakra.DomainModel.SAPRoleToAuthMap.GetByRoleCode("Z_TEST", 14);

            ////var criticalTCodes = SAPCriticalTCodeMatrix.GetAll(true, false);

            ////var matrix = SAPRoleCatalog.GetByBusinessRole("Test Critical");
            ////var roleTCodes = DI.Chakra.DomainModel.SAPRoleToAuthMap.GetByRoleCode(matrix.TechnicalRole, 14);
            //////List<string> roleTCodes = new List<string>();
            //////roleTCodes.Add("E40B");
            //////roleTCodes.Add("EL01");
            //////roleTCodes.Add("EL37");
            //////roleTCodes.Add("ZFIRF_REFUND_VIEW");
            //////roleTCodes.Add("ZFIRF_REFVIEW");
            //////roleTCodes.Add("ZCATV");

            ////Dictionary<string, string> tcodeOwners = new Dictionary<string, string>();

            ////foreach (string tcode in roleTCodes)
            ////{
            ////    var criticalTCode = criticalTCodes.FirstOrDefault(item => item.Transaction.Equals(tcode, StringComparison.CurrentCultureIgnoreCase));
            ////    if (criticalTCode != null)
            ////    {
            ////        if (!string.IsNullOrEmpty(criticalTCode.TransactionOwner))
            ////        {
            ////            if (!tcodeOwners.Keys.Contains(criticalTCode.TransactionOwner))
            ////            {
            ////                tcodeOwners.Add(criticalTCode.TransactionOwner, criticalTCode.Transaction);
            ////            }
            ////            else
            ////            {
            ////                tcodeOwners[criticalTCode.TransactionOwner] += ", " + criticalTCode.Transaction;
            ////            }
            ////        }
            ////    }
            ////}

            ////foreach (KeyValuePair<string, string> tcodeOwner in tcodeOwners)
            ////{

            ////    long owner = this.GetUserIdByEmployeeId(tcodeOwner.Key);

            ////    if (owner != 0)
            ////    {
            ////        subTasks.Add(this.CreateSubTaskShell(matrix.TechnicalRole, tcodeOwner.Value, (Int32)owner));
            ////        reviewerIds.Add(owner);
            ////    }
            ////    else
            ////    {
            ////        groupId = MasterConfigHelper.ITSecurityGroupId;
            ////        reviewerIds = this.GetUsersByGroupId(groupId);
            ////        foreach (int reviewerId in reviewerIds)
            ////        {
            ////            subTasks.Add(this.CreateSubTaskShell(matrix.TechnicalRole, tcodeOwner.Value, reviewerId));
            ////        }
            ////        exceptionRemarks = string.Format("The request {0} is routed to IT security team since the critical t-code(s) {1} for the role {2} has no owner.", 1, tcodeOwner.Value, matrix.TechnicalRole);
            ////        DILog.ProvisioningLogger.WriteLog(null, null,  LogLevelType.Warning, exceptionRemarks);
            ////    }
            ////}

            MessageBox.Show("Done.");
        }



        private long GetUserIdByEmployeeId(string employeeId)
        {
            var user = User.GetByEmployeeId(employeeId);
            if (user != null)
            {
                return user.Id;
            }
            return 0;
        }

        private List<long> GetUsersByGroupId(int groupId)
        {
            return Group.GetByIdIncludingUser(groupId).Users.Select(item => item.Id).ToList();
        }

        private SubTask CreateSubTaskShell(string roleName, string tcode, int owner)
        {
            return new SubTask()
            {
                RoleName = roleName,
                TCode = tcode,
                OwnerId = owner,
                CreatedBy = "Admin",
                CreatedDate = DateTime.Now,
                IsActive = true,
                Remarks = string.Format("This request requires your approval since the requested role - {0} has the critical tcode(s) - {1}.", roleName, tcode),
            };
        }

        private void button46_Click(object sender, EventArgs e)
        {
            //var endpoint = new EndPointFacade().GetEndPointByApplicationType(ApplicationType.NPAC);
            //this.MapWithMasterUser(endpoint);
        }

        private DataTable MapWithMasterUser(DI.Chakra.DomainModel.EndPoint EndPoint)
        {
            //NPACFacade _npacFacade = new NPACFacade();

            //string strQry = string.Format("Update DI_NPAC_UserInfo Set DIUserId=NULL, MappingType=NULL where EndpointSystemId={0} AND MappingType='System'", EndPoint.EndpointSystemId.ToString());

            //_npacFacade.ExecuteQry(strQry);

            //StringBuilder bldR = new StringBuilder();

            //string strJoin = GetJoinQry(EndPoint);

            //if (strJoin == string.Empty)
            //    return null;

            //bldR.Append("Update DI_NPAC_UserInfo set DI_NPAC_UserInfo.DIUserId = DI_M_User.UserId, DI_NPAC_UserInfo.MappingType='System',DI_NPAC_UserInfo.IsUserAccount=1 FROM DI_NPAC_UserInfo JOIN DI_M_User on");
            //bldR.Append(" ");
            //bldR.Append(strJoin);
            //bldR.Append(" ");
            //bldR.AppendFormat("WHERE LEN(RTRIM(DI_NPAC_UserInfo.Emp_Id)) > = 8 AND DI_NPAC_UserInfo.EndpointSystemId = {0} AND DI_M_User.EmployeeId <> '' AND DI_M_User.UserId <> 1 AND ISNULL(MappingType,'') <> 'Manual'", EndPoint.EndpointSystemId.ToString());

            //_npacFacade.ExecuteQry(bldR.ToString());

            return null;
        }

        private string GetJoinQry(DI.Chakra.DomainModel.EndPoint EndPoint)
        {
            string clauseType = "AND";

            if (!MapAll)
            {
                clauseType = "OR";
            }

            StringBuilder bldr = new StringBuilder();

            IList<AttributeMappingColumns> listAttributes = AttributeMappingColumns.GetAll();

            //on usr.NRIC like '%' + RTRIM(npac.Emp_Id) +  '%'

            foreach (AttributeMappingColumns attDto in listAttributes)
            {
                bldr.AppendFormat(string.Format("(DI_M_User.{1} LIKE '%' + RTRIM(DI_NPAC_UserInfo.{0}) + '%' and DI_NPAC_UserInfo.{2} != '' and DI_NPAC_UserInfo.{3} is not null and DI_M_User.{4} != '' and DI_M_User.{5} is not null)  {6} ", attDto.Tgcolumn, attDto.DiColumn, attDto.Tgcolumn, attDto.Tgcolumn, attDto.DiColumn, attDto.DiColumn, clauseType));
            }

            if (bldr.ToString() != string.Empty)
                return bldr.ToString().Remove(bldr.ToString().Length - (clauseType.Length + 1));

            return string.Empty;
        }

        private void button47_Click(object sender, EventArgs e)
        {
            var x = EmployeeEventTrigger.GetTriggerToExecute(5424, EmployeeEvents.Resignation);
        }

        private void button49_Click(object sender, EventArgs e)
        {
            //SAPEPDA SapEPDA = new SAPEPDA();
            //SAPEPFacade sapEPFacade = new SAPEPFacade();
            //ProvisioningConfigurationFacade provConfigFacade = new ProvisioningConfigurationFacade();
            //EndPointFacade endpointFacade = new EndPointFacade();
            //var user = new UserFacade().GetUser(5373);

            ////var endpoint = endpointFacade.GetEndPointByApplicationType(ApplicationType.SAP_EP);
            //var sapepUsrs = sapEPFacade.GetPrimarySAPEPUserByDIUserId(user.UserId, endpoint.EndpointSystemId);
            //ConnectorBase _connector = ConnectorFactory.GetConnector(endpoint, null);
            ////var flinsEndpoint = endpointFacade.GetEndPointByApplicationType(ApplicationType.SAP);

            //foreach (SAPEPUserInfo sapepUsr in sapepUsrs)
            //{
            //    sapepUsr.Department = user.OrgDepartment;
            //    sapepUsr.JobTitle = user.NRIC;

            //    sapepUsr.Roles = sapEPFacade.GetSAPEPRolesByUserId(sapepUsr.UserId).Select(item => item.RoleId).ToList();
            //    sapepUsr.Groups = sapEPFacade.GetSAPEPGroupsByUserId(sapepUsr.UserId).Select(item => item.GroupName).ToList();

            //    _connector.RemoveAllEntitlements(sapepUsr);

            //    _connector.ChangeContainer(sapepUsr);

            //    SapEPDA.UpdateSAPEPUser(sapepUsr);
            //}

            MessageBox.Show("Done.");
        }

        private void button50_Click(object sender, EventArgs e)
        {
            var user = User.GetById(5373);
            //ProvisioningFlowHelper.PopulateOnBoardAccessRequests(user);
            MessageBox.Show("Done.");
        }

        private void button51_Click(object sender, EventArgs e)
        {
            var user = User.GetById(5333);
            //  ProvisioningFlowHelper.AlertITAdmin(user, "ABCD", EmployeeEvents.ReOrganisation);
            //   ProvisioningFlowHelper.AlertITAdmin(user, "ABCD", EmployeeEvents.NewJoinee);
            //  ProvisioningFlowHelper.AlertITAdmin(user, "ABCD", EmployeeEvents.OUChange);
        }

        private void button52_Click(object sender, EventArgs e)
        {
            //var endpoint = new EndPointFacade().GetEndPointByApplicationType(ApplicationType.CE);
            //ConnectorBase _connector = ConnectorFactory.GetConnector(endpoint, null);


            //var sapepUsr = sapEPFacade.GetSAPEPUserByUserName("TDAT567T", endpoint.EndpointSystemId);
            //sapepUsr.Roles = sapEPFacade.GetSAPEPRolesByUserId(sapepUsr.UserId).Select(item => item.UniqueRoleName).ToList();
            //sapepUsr.Groups = sapEPFacade.GetSAPEPGroupsByUserId(sapepUsr.UserId).Select(item => item.GroupUniqueName).ToList();

            //bool isSuccess = _connector.RemoveEntitlement(sapepUsr, EntitlementType.Role);
            //string roleListStr = string.Join(",", sapepUsr.Roles.Select(item => item));

            //if (isSuccess)
            //{
            //    new AccessRequestFacade().DeleteSAPEPUserRoleMap(sapepUsr);
            //    DILog.ProvisioningLogger.WriteLog(null, null,  endpoint, LogLevelType.Info, string.Format("Roles - {0} removed successfully for the user {1}.", roleListStr, sapepUsr.UserName), null, ProvisioningEventType.OUChange, "Provisioning Engine");
            //}
            //else
            //{
            //    DILog.ProvisioningLogger.WriteLog(null, null,  endpoint, LogLevelType.Error, string.Format("Error while removing roles - {0} for the user {1}.", roleListStr, sapepUsr.UserName), null, ProvisioningEventType.OUChange, "Provisioning Engine");
            //}

            //isSuccess = _connector.RemoveEntitlement(sapepUsr, EntitlementType.Group);
            //roleListStr = string.Join(",", sapepUsr.Groups.Select(item => item));

            //if (isSuccess)
            //{
            //    new AccessRequestFacade().DeleteSAPEPUserGroupMap(sapepUsr);
            //    DILog.ProvisioningLogger.WriteLog(null, null,  endpoint, LogLevelType.Info, string.Format("Groups - {0} removed successfully for the user {1}.", roleListStr, sapepUsr.UserName), null, ProvisioningEventType.OUChange, "Provisioning Engine");
            //}
            //else
            //{
            //    DILog.ProvisioningLogger.WriteLog(null, null,  endpoint, LogLevelType.Error, string.Format("Error while removing groups - {0} for the user {1}.", roleListStr, sapepUsr.UserName), null, ProvisioningEventType.OUChange, "Provisioning Engine");
            //}

            MessageBox.Show("Done.");
        }

        private void button53_Click(object sender, EventArgs e)
        {
            List<InhouseUserInfo> inhUsers = InhouseUserInfo.GetByDIUserId(5461, 20);

            foreach (var item in inhUsers)
            {
                this.CreateDeleteTasks(item, LockScenario.OffBoard);
            }

            MessageBox.Show("Done.");
        }

        private void CreateDeleteTasks(InhouseUserInfo inhUser, LockScenario lockScenario)
        {

        }

        private void button54_Click(object sender, EventArgs e)
        {
            ProvisioningEngine peManager = new ProvisioningEngine();
            peManager.StartMiscProcess();

            MessageBox.Show("Done.");
        }



        private void button56_Click(object sender, EventArgs e)
        {
            string pernr = this.textBox7.Text.Trim();

            var endpoint = DI.Chakra.DomainModel.EndPoint.GetTrustedSource();
            ConnectorBase _connector = ConnectorFactory.GetConnector(endpoint, null);

            //_connector.DownloadSpecificUserFromHR(pernr);
        }

        private void button57_Click(object sender, EventArgs e)
        {
            //var endpoint = new EndPointFacade().GetEndPointByApplicationType(ApplicationType.SAP);
            //StringBuilder bldR = new StringBuilder();

            //string strJoin = GetJoinQry(endpoint.EndpointSystemId);

            //bldR.Append("Update DI_SAP_UserInfo set DI_SAP_UserInfo.DIUserId = DI_M_User.UserId,MappingType='System',DI_SAP_UserInfo.IsUserAccount=1  FROM DI_SAP_UserInfo JOIN DI_M_User on");
            //bldR.Append(" ");
            //bldR.Append(strJoin);
            //bldR.Append(" ");
            //bldR.AppendFormat("WHERE DI_SAP_UserInfo.EndpointSystemId = {0}  AND DI_M_User.UserId <> 1 AND ISNULL(MappingType,'') <> 'Manual'", endpoint.EndpointSystemId.ToString());

            //var x = bldR.ToString();
        }

        public string GetJoinQry(int _systemId)
        {
            string clauseType = "AND";
            if (!MapAll)
            {
                clauseType = "OR";
            }

            StringBuilder bldr = new StringBuilder();
            IList<AttributeMappingColumns> listAttributes = AttributeMappingColumns.GetByEndPoindId(_systemId);

            foreach (AttributeMappingColumns attDto in listAttributes)
            {
                bldr.AppendFormat(string.Format("(DI_SAP_UserInfo.{0}=DI_M_User.{1} and DI_SAP_UserInfo.{2} != '' and DI_SAP_UserInfo.{3} is not null and DI_M_User.{4} != '' and DI_M_User.{5} is not null)  {6} ", attDto.Tgcolumn, attDto.DiColumn, attDto.Tgcolumn, attDto.Tgcolumn, attDto.DiColumn, attDto.DiColumn, clauseType));

            }
            if (bldr.ToString() != string.Empty)
                return bldr.ToString().Remove(bldr.ToString().Length - (clauseType.Length + 1));
            return string.Empty;
        }

        private void button58_Click(object sender, EventArgs e)
        {
            List<string> source = "a,b,c,d,e,f,g".Split(',').ToList();
            List<string> dest = "c,f,g".Split(',').ToList();

            List<string> result = source.Except(dest).ToList();
        }

        private void button59_Click(object sender, EventArgs e)
        {
            //var lnUsrs = LNFacade.GetPrimaryLNSUserByDIUserId(user.UserId, this.Endpoint.EndpointSystemId);

            //ConnectorBase _connector = ConnectorFactory.GetConnector(this.Endpoint, this.CurrentRequest);

            //foreach (LotusUserInfo lnUserValue in lnUsrs)
            //{
            //    //set new ou values to LN user object
            //    lnUserValue.CompanyName = user.OrgCompany;
            //    lnUserValue.Department = user.OrgDepartment;
            //    lnUserValue.JobTitle = user.PositionName;
            //    lnUserValue.OfficePhoneNumber = user.TelephoneNumber;
            //    lnUserValue.LastModifiedBy = "System";
            //    lnUserValue.LastModifiedDate = DateTime.Now;

            //    var configLNS = ProvConfigFacade.GetLNSCertifierFileByUser(user, this.Endpoint.EndpointSystemId);
            //    if (null != configLNS)
            //    {
            //        lnUserValue.MailServer = configLNS.MailServer;
            //        lnUserValue.TargetCertifier = configLNS.CertifierOU;
            //        lnUserValue.CertifierIDFile = configLNS.CertifierIdFile;
            //    }

            //    this.TakeOUSnapshot(lnUserValue);

            //    //TODO: Temporarily commented - confirm this and uncomment the code.

            //    var existingGroups = LNFacade.GetLNSGroupsByUserId(lnUserValue.UserId, lnUserValue.EndPointSystemId).Select(item => item.Group).ToList();

            //    //retain groups created using prov form
            //    var createForm = FormFacade.GetCreateProvisioningFormByEndpoint(this.Endpoint.EndpointSystemId, AccountType.User);
            //    var form = FormFacade.GetProvisioningFormByID(createForm.ProvisioningFormId, true);
            //    var lnsForm = form.FormDetails as ProvisioningFormLNS;

            //    if (!string.IsNullOrEmpty(lnsForm.Groups))
            //    {
            //        foreach (string group in existingGroups)
            //        {
            //            if (!lnsForm.Groups.Contains(group))
            //            {
            //                lnUserValue.Groups.Add(group);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        lnUserValue.Groups = existingGroups;
            //    }

            //    //retain exception groups (hardcode for phase 1)
            //    List<string> groupstoRetain = "BlackBerry10 Users Group,BlackBerry5 Users Group,ZZ Lotus Traveler Users,ZZ Lotus Traveler Users 1,ZZ Lotus Traveler VIP Users,SP RedisafeUsers Group,MM Group,SP Broadcast Group,SPPG Board Group,SPPG Broadcast Group,UPAGE EXCO Group,SP Club Members 1 Group,SP Club Members 2 Group,SP Club Members 3 Group,SP Club Members 4 Group,SP Club Members Group".Split(',').ToList();

            //    lnUserValue.Groups = lnUserValue.Groups.Except(groupstoRetain).ToList();

            //    _connector.RemoveEntitlement(lnUserValue, EntitlementType.Group);

            //    this.deleteLotusNotesUserGroupMap(lnUserValue);

            //    _connector.ChangeContainer(lnUserValue);

            //    //add default groups for the new org.

            //    if (user.FullTimeEmployee.HasValue && user.FullTimeEmployee.Value)
            //    {
            //        lnUserValue.Groups = this.GetAdditionalGroupsByUser(user, lnUserValue.EndPointSystemId);

            //        bool isAdded = _connector.AddEntitlement(lnUserValue, EntitlementType.Group);
            //        if (isAdded)
            //        {
            //            this.addUserIntoLNGroupInfo(lnUserValue);
            //        }
            //    }
            //    new LotusNotesDA().UpdateLNUserInfoFromProvisioning(lnUserValue);
            //}

            //return true;
        }

        private void button60_Click(object sender, EventArgs e)
        {
            var x = this.setEmployeeNo();
        }

        private string setEmployeeNo()
        {
            string empID = null;
            string defaultEmployeeKey = MasterConfigHelper.EmployeeDefaultKey;
            string EmpId = User.GetEmployeeId(defaultEmployeeKey);
            if (EmpId == null)
            {
                return defaultEmployeeKey + "000000";

            }
            else
            {
                empID = EmpId.Replace(defaultEmployeeKey, "").Trim();
                return defaultEmployeeKey + (Convert.ToInt32(empID) + 1).ToString("000000");
            }
        }

        private void button61_Click(object sender, EventArgs e)
        {
            IList<Group> userGroups = Group.GetByUser(2408);
        }

        private void button62_Click(object sender, EventArgs e)
        {
            bool IsConfilctedByBusinessRole = false;
            //var x = new AdHocRequestFacade().GetConfilictConfiguration("GL Accountant", out IsConfilctedByBusinessRole);

            var y = this.ConflictBusinessRoles(new List<InterimStore>(), "GL Accountant", 3357, 5694);
        }

        private string ConflictBusinessRoles(IList<InterimStore> AllRequest, string CurrentEntitlement, int CurrentEndPointUserID, int CurrentRequesteeUserId)
        {
            string Remark = "";
            string ConfilctRolesInterimStore = "";
            string ConfilctRolesRequested = "";
            string ConfilctProvisioning = "";


            //AdHocRequestFacade adhocfacad = new AdHocRequestFacade();

            //#region SourceGenerator
            //// Memory Source
            //Dictionary<string, string> ListofEntilement = new Dictionary<string, string>();
            //foreach (InterimStore item in AllRequest)
            //{
            //    AdHocAccessRequest adhocaccessrequest = UtilityMethods.Deserialize<AdHocAccessRequest>(item.XmlData);
            //    if (adhocaccessrequest.EntitlementDetailType == ApplicationType.SAP)
            //    {
            //        AdHocSAPHRFLINSRequest flinsRequest = (AdHocSAPHRFLINSRequest)adhocaccessrequest.GetEntitlementDetail();
            //        if (flinsRequest != null)
            //        {
            //            if (!ListofEntilement.ContainsKey(flinsRequest.AdHocSAPHRFLINSRequestDetails[0].Entitlement))
            //            {
            //                ListofEntilement.Add(flinsRequest.AdHocSAPHRFLINSRequestDetails[0].Entitlement, "InterimStore");

            //            }

            //        }
            //    }
            //}
            //// End

            //// Requested Source
            //Dictionary<string, string> ListofEntilementRequest = new Dictionary<string, string>();
            ////ListofEntilementRequest = adhocfacad.GetListofEntitlementByRequesteeUserId(CurrentRequesteeUserId);
            //// End

            //// OnSubmiting Source
            //List<SAPRoleInfo> listofsaprolesInfo = new SAPFacade().GetSAPRolesByUserId(CurrentEndPointUserID);
            //List<string> listofRolesString = listofsaprolesInfo.Select(lq => lq.ROLE_CODE).ToList();
            //// End

            //// Main Source
            //bool IsConfilctedByBusinessRole;
            ////IList<SAPHRFLINSConflictingRole> sAPHRFLINSConflictingRole = adhocfacad.GetConfilictConfiguration(CurrentEntitlement, out IsConfilctedByBusinessRole);
            //// End
            //#endregion


            //if (sAPHRFLINSConflictingRole != null)
            //{
            //    #region FirstScenario and Third Scenario
            //    foreach (SAPHRFLINSConflictingRole items in sAPHRFLINSConflictingRole)
            //    {
            //        if (ListofEntilement.ContainsKey(IsConfilctedByBusinessRole ? items.ConflictingBusinessRole : items.BusinessRole) && ListofEntilement.ContainsValue("InterimStore"))
            //        {
            //            ConfilctRolesInterimStore = string.IsNullOrEmpty(ConfilctRolesInterimStore) ? (IsConfilctedByBusinessRole ? items.ConflictingBusinessRole : items.BusinessRole) : ConfilctRolesInterimStore + ", " + (IsConfilctedByBusinessRole ? items.ConflictingBusinessRole : items.BusinessRole);
            //        }
            //        if (ListofEntilementRequest.ContainsKey(IsConfilctedByBusinessRole ? items.ConflictingBusinessRole : items.BusinessRole) && ListofEntilementRequest.ContainsValue("Requested"))
            //        {
            //            ConfilctRolesRequested = string.IsNullOrEmpty(ConfilctRolesRequested) ? (IsConfilctedByBusinessRole ? items.ConflictingBusinessRole : items.BusinessRole) : ConfilctRolesRequested + ", " + (IsConfilctedByBusinessRole ? items.ConflictingBusinessRole : items.BusinessRole);
            //        }
            //    }
            //    if (!string.IsNullOrEmpty(ConfilctRolesInterimStore))
            //    {
            //        int lastIndex = ConfilctRolesInterimStore.LastIndexOf(",");
            //        ConfilctRolesInterimStore = "The Requested Business Role " + CurrentEntitlement + " Conflicts with " + (lastIndex > 0 ? ConfilctRolesInterimStore.Substring(0, lastIndex) + " and" + ConfilctRolesInterimStore.Substring(lastIndex + 1) : ConfilctRolesInterimStore) + " (Requested)";
            //    }
            //    if (!string.IsNullOrEmpty(ConfilctRolesRequested))
            //    {
            //        int lastIndex = ConfilctRolesRequested.LastIndexOf(",");
            //        ConfilctRolesRequested = "The Requested Business Role " + CurrentEntitlement + " Conflicts with " + (lastIndex > 0 ? ConfilctRolesRequested.Substring(0, lastIndex) + " and" + ConfilctRolesRequested.Substring(lastIndex + 1) : ConfilctRolesRequested) + " (Requested)";
            //    }
            //    #endregion
            //    #region SecondScenario
            //    foreach (SAPHRFLINSConflictingRole items in sAPHRFLINSConflictingRole)
            //    {
            //        int countofAsterix = IsConfilctedByBusinessRole ? items.ConflictingTechnicalRole.Split('*').Count() - 1 : items.TechnicalRole.Split('*').Count() - 1;
            //        string toCompare;
            //        if (countofAsterix == 0)
            //        {
            //            if (CurrentEntitlement.Equals(items.BusinessRole, StringComparison.CurrentCultureIgnoreCase))
            //            {
            //                if (listofRolesString.Equals(items.ConflictingTechnicalRole))
            //                {
            //                    ConfilctProvisioning = string.IsNullOrEmpty(ConfilctProvisioning) ? (IsConfilctedByBusinessRole ? items.ConflictingBusinessRole : items.BusinessRole) : ConfilctProvisioning + ", " + (IsConfilctedByBusinessRole ? items.ConflictingBusinessRole : items.BusinessRole);
            //                }
            //            }
            //            else
            //            {
            //                if (listofRolesString.Equals(items.TechnicalRole))
            //                {
            //                    ConfilctProvisioning = string.IsNullOrEmpty(ConfilctProvisioning) ? (IsConfilctedByBusinessRole ? items.ConflictingBusinessRole : items.BusinessRole) : ConfilctProvisioning + ", " + (IsConfilctedByBusinessRole ? items.ConflictingBusinessRole : items.BusinessRole);
            //                }
            //            }
            //        }
            //        else if (countofAsterix == 1)
            //        {
            //            string coloumnName;
            //            if (CurrentEntitlement.Equals(items.BusinessRole, StringComparison.CurrentCultureIgnoreCase))
            //            {
            //                coloumnName = items.ConflictingTechnicalRole;
            //                toCompare = coloumnName.Substring(coloumnName.LastIndexOf("*") + 2);
            //                var count = (from c in listofRolesString where c.EndsWith(toCompare) select c).Count();
            //                if (count > 0)
            //                {
            //                    ConfilctProvisioning = string.IsNullOrEmpty(ConfilctProvisioning) ? (IsConfilctedByBusinessRole ? items.ConflictingBusinessRole : items.BusinessRole) : ConfilctProvisioning + ", " + (IsConfilctedByBusinessRole ? items.ConflictingBusinessRole : items.BusinessRole);
            //                }
            //            }
            //            else
            //            {
            //                coloumnName = items.TechnicalRole;
            //                toCompare = coloumnName.Substring(coloumnName.LastIndexOf("*") + 2);
            //                var count2 = (from c in listofRolesString where c.EndsWith(toCompare) select c).Count();
            //                if (count2 > 0)
            //                {
            //                    ConfilctProvisioning = string.IsNullOrEmpty(ConfilctProvisioning) ? (IsConfilctedByBusinessRole ? items.ConflictingBusinessRole : items.BusinessRole) : ConfilctProvisioning + ", " + (IsConfilctedByBusinessRole ? items.ConflictingBusinessRole : items.BusinessRole);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            string coloumnName;
            //            if (CurrentEntitlement.Equals(items.BusinessRole, StringComparison.CurrentCultureIgnoreCase))
            //            {
            //                coloumnName = items.ConflictingTechnicalRole;
            //                toCompare = coloumnName.Substring(0, coloumnName.LastIndexOf("*"));
            //                toCompare = toCompare.Substring(toCompare.LastIndexOf('*') + 2);
            //                var count = (from c in listofRolesString where c.Contains(toCompare) select c).Count();
            //                if (count > 0)
            //                {
            //                    ConfilctProvisioning = string.IsNullOrEmpty(ConfilctProvisioning) ? (IsConfilctedByBusinessRole ? items.ConflictingBusinessRole : items.BusinessRole) : ConfilctProvisioning + ", " + (IsConfilctedByBusinessRole ? items.ConflictingBusinessRole : items.BusinessRole);
            //                }
            //            }
            //            else
            //            {
            //                coloumnName = items.TechnicalRole;
            //                toCompare = coloumnName.Substring(0, coloumnName.LastIndexOf("*"));
            //                toCompare = toCompare.Substring(toCompare.LastIndexOf('*') + 2);
            //                var count2 = (from c in listofRolesString where c.Contains(toCompare) select c).Count();
            //                if (count2 > 0)
            //                {
            //                    ConfilctProvisioning = string.IsNullOrEmpty(ConfilctProvisioning) ? (IsConfilctedByBusinessRole ? items.ConflictingBusinessRole : items.BusinessRole) : ConfilctProvisioning + ", " + (IsConfilctedByBusinessRole ? items.ConflictingBusinessRole : items.BusinessRole);
            //                }
            //            }
            //        }
            //    }
            //    if (!string.IsNullOrEmpty(ConfilctProvisioning))
            //    {
            //        int lastIndex = ConfilctProvisioning.LastIndexOf(",");
            //        ConfilctProvisioning = "The Requested Business Role " + CurrentEntitlement + " Conflicts with " + (lastIndex > 0 ? ConfilctProvisioning.Substring(0, lastIndex) + " and" + ConfilctProvisioning.Substring(lastIndex + 1) : ConfilctProvisioning) + " (Existing)";
            //    }
            //    #endregion
            //}

            //Remark = ConfilctRolesInterimStore + "," + ConfilctProvisioning + "," + ConfilctRolesRequested;
            //string[] Scenario = Remark.Split(',');
            //Remark = "";
            //foreach (string remark in Scenario)
            //{
            //    if (!string.IsNullOrEmpty(remark))
            //    {
            //        Remark = string.IsNullOrEmpty(Remark) ? remark : Remark + "<br>" + remark;
            //    }
            //}
            return Remark;
        }

        private void button63_Click(object sender, EventArgs e)
        {
            var wfConfig = WorkflowConfiguration.GetByEndpointSystemId(Convert.ToInt64(RequestType.Provisioning), 2, "1", "1", "1", DateTime.Now.AddDays(-4000));
        }

        private void button64_Click(object sender, EventArgs e)
        {
            var user = User.GetById(3691);
            var userXml = UtilityMethods.SerializeAsAttributes<User>(user);

            var formattedString = this.RemoveXmlHeader(userXml);
        }

        private string RemoveXmlHeader(string xmlString)
        {
            xmlString = xmlString.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n", string.Empty);
            xmlString = xmlString.Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", string.Empty);
            xmlString = xmlString.Replace("<User ", string.Empty);
            xmlString = xmlString.Substring(0, xmlString.IndexOf(">"));
            xmlString = xmlString.Remove(xmlString.IndexOf("Password"), xmlString.IndexOf("Password") + 26);
            xmlString = xmlString.Replace("=\"", " = ");
            xmlString = xmlString.Replace("\" ", "\r\n");

            return xmlString;
        }
        public static string ConstructUserName(string userName)
        {
            var acName = System.Text.RegularExpressions.Regex.Replace(userName, "[^0-9A-Za-z]+", "");

            if (acName.Length > 8)
                return acName.Substring(0, 8);

            return acName;
        }
        private static bool DoesExists(string value)
        {
            return false;

        }
        public string ConstructUserName(string userName, string identificationNo)
        {
            var acNameTmp = System.Text.RegularExpressions.Regex.Replace(userName, "[^0-9A-Za-z]+", "");

            string retValue = string.Empty;
            int counter = 1;

            if (acNameTmp.Length > 8)
                acNameTmp = acNameTmp.Substring(0, 8);


            retValue = acNameTmp;

            while (DoesExists(retValue))
            {
                retValue = acNameTmp + counter.ToString();

                counter++;
            }

            return retValue;
        }
        private void Test1()
        {
            var t = DI.Chakra.DomainModel.SAPUserinfo.GetByIdWithNavigation(4318);
        }
        private void btnDiscoveryEngine_Click(object sender, EventArgs e)
        {
            this.Test1();

            string s = ConstructUserName("Test Engg", null);

            DiscoveryEngine discoveryEngine = new DiscoveryEngine();
            DI.Chakra.Infrastructure.Enums.DiscoveryServiceInstance serviceInstance = (DiscoveryServiceInstance)AppSettingsHelper.DiscoveryInstance;
            discoveryEngine.StartProcess(serviceInstance);
        }

        private void button65_Click(object sender, EventArgs e)
        {
            EmailAgent emailAgent = new EmailAgent();
            emailAgent.SendEmails(EmailActivityType.None);
            MailMessage mMailMessage = null;
            SmtpClient mSmtpClient = null;
            try
            {
                mMailMessage = new MailMessage();
                mMailMessage.From = new MailAddress("itidadmin@rwsentosa.com");
                string To = "dgaddam@deepidentity.com";//"oscar.villegas@rwsentosa.com,shaikf.rven@sgpconsult.com";
                string[] toAddlist = To.Split(',');
                foreach (string tmpToAddress in toAddlist)
                {
                    if (!string.IsNullOrEmpty(tmpToAddress.Trim()))
                    {
                        mMailMessage.To.Add(new MailAddress(tmpToAddress));
                    }
                }

                //if ((_info.Bcc != null) && (_info.Bcc != string.Empty))
                //{
                mMailMessage.Bcc.Add(new MailAddress("dgaddam@deepidentity.com"));
                //}

                //if ((_info.Cc != null) && (_info.Cc != string.Empty))
                //{
                mMailMessage.CC.Add(new MailAddress("amjath@deepidentity.com"));
                //}

                //if (!string.IsNullOrEmpty(_info.Attachment))
                //{
                //    //  Attachment attach = new Attachment(new MemoryStream(Encoding.ASCII.GetBytes(_info.Attachment)),string.Empty);
                //    Attachment attach = new Attachment(_info.Attachment);
                //    mMailMessage.Attachments.Add(attach);

                //}
                //if (attachment != null)
                //{
                //    Attachment attach = attachment;
                //    mMailMessage.Attachments.Add(attach);
                //}
                mMailMessage.Subject = "Test for cc bcc";
                mMailMessage.Body = "Test this mail for cc and bcc";
                mMailMessage.IsBodyHtml = true;
                mMailMessage.Priority = MailPriority.Normal;

                mSmtpClient = new SmtpClient();

                mSmtpClient.Host = "smtp.rwsentosa.com";
                mSmtpClient.Port = int.Parse("25");
                mSmtpClient.EnableSsl = true;

                if (!string.IsNullOrEmpty("itidadmin@rwsentosa.com") && !string.IsNullOrEmpty(""))
                    mSmtpClient.Credentials = new System.Net.NetworkCredential("vijay.more@deepidentity.com", "D33pidentity123");
                else
                    mSmtpClient.UseDefaultCredentials = false;

                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                mSmtpClient.Send(mMailMessage);

                MessageBox.Show("sent");
                //DILog.EmailLogger.WriteLog((int)_info.Id,LogLevelType.Info, "Email Sent Successfully");
                // return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("error");
                //DILog.EmailLogger.WriteLog((int)_info.Id, LogLevelType.Exception, string.Empty, ex);
                throw;
            }
            finally
            {
                mMailMessage = null;
                mSmtpClient = null;
            }


        }

        private void button66_Click(object sender, EventArgs e)
        {
            var approvalChain = new ApprovalChainFinder().Execute(50803, false);
        }

        private void button67_Click(object sender, EventArgs e)
        {

        }

        private void button68_Click(object sender, EventArgs e)
        {

        }

        private void button69_Click(object sender, EventArgs e)
        {

        }

        private void button70_Click(object sender, EventArgs e)
        {
            //DI.Chakra.DomainModel.ProvisioningRole provisioningRole =
            //    new DI.Chakra.DomainModel.ProvisioningRole
            //    {
            //        Name = "PrvRoleName-500",
            //        Description = "ProvRoleDesc-500",
            //        CreatedBy = "Marimuthu Thirumalai",
            //        CreatedDate = DateTime.Now,
            //        ProvisioningRoleDetails = new List<DI.Chakra.DomainModel.ProvisioningRoleDetail>()
            //        {
            //          new DI.Chakra.DomainModel.ProvisioningRoleDetail(){ EndpointSystemId=500,Entitlements="Entitle-500"},
            //          new DI.Chakra.DomainModel.ProvisioningRoleDetail(){ EndpointSystemId=501,Entitlements="Entitle-501"},
            //          new DI.Chakra.DomainModel.ProvisioningRoleDetail(){ EndpointSystemId=502,Entitlements="Entitle-502"},
            //          new DI.Chakra.DomainModel.ProvisioningRoleDetail(){ EndpointSystemId=503,Entitlements="Entitle-503"}
            //        }

            //    };
            //provisioningRole.Save(MasterConfigHelper.ServiceUser);
            // _prvrolefacade.AddProvisioningRole_Test();
        }

        private void button71_Click(object sender, EventArgs e)
        {
            //DI.Chakra.DomainModel.ProvisioningRole provisioningRole = new DI.Chakra.DomainModel.ProvisioningRole { Id = 18, Name = "PrvRoleName-Updated", Description = "ProvRoleDesc-Updated" };
            //List<DI.Chakra.DomainModel.ProvisioningRoleDetail> provRoleDetail = new List<DI.Chakra.DomainModel.ProvisioningRoleDetail>()
            // {
            //     new DI.Chakra.DomainModel.ProvisioningRoleDetail { EndpointSystemId = 500, Entitlements = "Entitle-New-500-1"},
            //     new DI.Chakra.DomainModel.ProvisioningRoleDetail { EndpointSystemId = 500, Entitlements = "Entitle-New-500-2"},
            //     new DI.Chakra.DomainModel.ProvisioningRoleDetail { EndpointSystemId = 500, Entitlements = "Entitle-New-500-3"}
            // };

            //provisioningRole.ProvisioningRoleDetails = provRoleDetail;
            //provisioningRole.Save(MasterConfigHelper.ServiceUser);

            //_provisioningFacade.UpdateProvisioningRole(provRole);
            // _provisioningFacade.UpdateProvisioningRole(provRole,provRoleDetail);
        }

        private void button72_Click(object sender, EventArgs e)
        {
            var currentJobs = DI.Chakra.DomainModel.Process.GetCurrentJobs(ProcessType.Discovery);
            if (currentJobs.Count > 0)
            {
                string processId = currentJobs[0].Id.ToString();
            }
            foreach (DI.Chakra.DomainModel.Process job in currentJobs)
            {
                DiscoveryService service = new DiscoveryService(job);
                service.Start();
            }
        }

        private void button73_Click(object sender, EventArgs e)
        {
            //    TokenProcessor processor = new TokenProcessor(1, "local");

            //    string value = processor.Execute();

            //    MessageBox.Show(value);

            //    EmailComposer ec = new EmailComposer();
            //    ec.Compose(EmailTemplate.AdhocProvisioningAmendment, 82);
        }

        private void btnCompliance_Click(object sender, EventArgs e)
        {
            //ComplianceEngine engine = new ComplianceEngine();
            //engine.StartProcess();
        }

        private string ConstructShortName(string UName)
        {
            string retString = string.Empty;
            string spaceRemovedString = RemoveSplCharsWithEmptyString(UName);
            int len = spaceRemovedString.Length;
            if (len <= 8)
            {
                retString = spaceRemovedString.ToUpper();
            }
            else
            {
                retString = spaceRemovedString.Substring(0, 8).ToUpper();
            }
            return retString;
        }

        private static string RemoveSplCharsWithEmptyString(string UserName)
        {
            UserName = System.Text.RegularExpressions.Regex.Replace(UserName, "[^0-9A-Za-z]+", "");
            return UserName;
        }

        private void button74_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this.ConstructShortName("Luiz Sanz Garcia"));
        }

        private void button75_Click(object sender, EventArgs e)
        {
            var x = ADSUserInfo.GetByIdIncludeADSGroups(776);
        }
        private void button55_Click(object sender, EventArgs e)
        {
            var accessRequest = AccessRequest.GetByIdIncludeAdHocAccessRequest(10);
            var user = User.GetById(accessRequest.UserId);

            // ProvisioningFlowHelper.AccessRequestFailureNotification(accessRequest, user);
            // ProvisioningFlowHelper.AccessRequestFailureNotification((int)EmailTemplate.AccessRequestFailureNotification,accessRequest.Id);
            //new EmailComposer().Compose(EmailTemplate.AccessRequestFailureNotification, accessRequest.Id);
        }
        private void btnMailTemplateTest_Click(object sender, EventArgs e)
        {
            var task = Task.GetById(Convert.ToInt64(txtTemplateId.Text));
            //UpdateAttestationDateForDelta tt = new UpdateAttestationDateForDelta();
            //tt.Execute(task, 705);

            SaveAttestationSnapshot sv = new SaveAttestationSnapshot();
            sv.Execute(task, Convert.ToInt64(txtObjectId.Text));

            bool isError = false;
            //try
            //{
            //    SmtpClient testSMPT = new SmtpClient("smtp.rwsentosa.com");
            //    string sub = "Test email from  IDAS V5 Mail server Configuration.";
            //    string body = "This is a test email sent when configure Mail server from IDAS V5 .";
            //    string fromAddress = "";
            //    MailMessage msg = new MailMessage("itidadmin@rwsentosa.com", "shaikf.rven@sgpconsult.com", sub, body);
            //    testSMPT.EnableSsl = false;
            //    testSMPT.Port = int.Parse("25");
            //    //serverConfig.Password = "";
            //    //if (!string.IsNullOrEmpty(serverConfig.UserName) && !string.IsNullOrEmpty(serverConfig.Password))
            //    //    testSMPT.Credentials = new System.Net.NetworkCredential(serverConfig.UserName, serverConfig.Password);
            //    //else
            //        testSMPT.UseDefaultCredentials = false;

            //    ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            //    testSMPT.Send(msg);
            //    isError = true;
            //}
            //catch (Exception ex)
            //{
            //    isError = false;
            //}

            ////var testForSumma = RSAHelper.DecryptData("");
            ////var user = new UserProcessor(1, new Process());
            ////user.ProcessHRMSUsers();

            ////ADAccountCreationForOnBoarding b1 = new ADAccountCreationForOnBoarding();
            ////b1.BuildPreConditionResultSet();
            ////b1.Execute();

            ////var request = AccessRequest.GetNewAccountByEndpointIdIncludeUser(4);
            ////var summa1 = AccountReconcilation.GetNonSeedAccounts(1, 4);
            ////var summa = NRuleset.GetMatchingRulesets(18);



            ////AttestationMailNotificationJob v1 = new AttestationMailNotificationJob();
            ////v1.BuildPreConditionResultSet();
            ////v1.Execute();

            ////PromotionMailNotification v = new PromotionMailNotification();
            ////v.BuildPreConditionResultSet();
            ////v.Execute();

            ////var eventHRMS = NHRMSUserEvent.GetById(13);
            ////var nHRMSUser = NHRMSUser.GetById(eventHRMS.UserId);
            ////var usr = User.GetByEmployeeId(nHRMSUser.EmployeeId);
            ////EmployeeEventTrigger trigger = new EmployeeEventTrigger()
            ////{

            ////    UserId = usr.Id,
            ////    EventType = eventHRMS.EmployeeEvent,
            ////    EffectiveDate = eventHRMS.EffectiveDate.Value,
            ////    IsRequestCreated = false,
            ////    IsAdminCheckRequired = false,
            ////    IsActive = true,
            ////    CreatedBy = "TestHarness",
            ////    CreatedDate = DateTime.Now,
            ////    LastModifiedBy = "TestHarness",
            ////    LastModifiedDate = DateTime.Now,

            ////};

            ////trigger.Save(MasterConfigHelper.ServiceUser);


            ////int processId = 1;
            ////var process = Process.GetByProcessType(processId, ProcessType.Reporting);
            ////ReportingService _discoveryService = new ReportingService();
            ////_discoveryService.Start(process);
            ////MessageBox.Show("Discovery Completed.");
            ////return;
            ////ADAccountCreationForOnBoarding t1 = new ADAccountCreationForOnBoarding();
            ////t1.BuildPreConditionResultSet();
            ////t1.Execute();
            ////var t = AdHocAccessRequest.GetPendingByRequesteeUserId(3117);
            ////NRulesetForTechnicalRoleDetail.GetResolvedExpressionByRule(new List<long> { 3 }, 29);
            ////GrantProvisioning gp = new GrantProvisioning();
            ////var task1 = Task.GetById(22);
            ////gp.Execute(task1);
            //emailInfo.ActivityType = EmailActivityType.None.ToString();

            //if (string.IsNullOrEmpty(emailInfo.To))
            //    emailInfo.To = DI.Chakra.DomainModel.ConfigurationMaster.GetByConfigKey("AccountCreatedNotificationTo", Product.IACM).ConfigValue.ToString();

            //EmailHandler.Send(emailInfo);

            //var epHierarchey = EndpointHierarchy.GetEndpointHierarchy(3);
            //var attEpHierarchey = AttestationHierarchy.GetAttestationHierarchy(2);
            //MessageBox.Show(DateTime.Today.AddMonths(7).ToString("yyyy.MM.dd"));
            // Dictionary<string,string> placeholders =ProvisioningFlowHelper.CollectPlaceHolders(Convert.ToInt16(txtTemplateId.Text));       
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button76_Click(object sender, EventArgs e)
        {
            // Dictionary<string, string> placeholders = ProvisioningFlowHelper.CollectPlaceHolders(Convert.ToInt16(txtTemplateId.Text));
            //  ProvisioningFlowHelper.ReplacePlaceHolders(placeholders, Convert.ToInt16(txtTemplateId.Text),Convert.ToInt16(txtObjectId.Text));
            //new EmailComposer().Compose(EmailTemplate.DiscoveryJobFailed, 1);
        }

        private void button77_Click(object sender, EventArgs e)
        {
            //  ProvisioningFlowHelper.AccessRequestFailureNotification(Convert.ToInt16(txtTemplateId.Text), Convert.ToInt16(txtObjectId.Text));
            // WithdrawRequest.NotifyApproverAboutRequestWithdrawn(Convert.ToInt16(txtTemplateId.Text), Convert.ToInt16(txtObjectId.Text),12);
            //ProvisioningFlowHelper.SendEmailAfterExecutingOUChangeRequest(Convert.ToInt16(txtTemplateId.Text), Convert.ToInt16(txtObjectId.Text));


        }

        private void button78_Click(object sender, EventArgs e)
        {
            var user = User.GetById(5758);
            //bool considerDefault = MasterConfigHelper.CanConsiderDefaultMatrix;
            //bool canConsiderBestMatrixChooser = MasterConfigHelper.CanConsiderBestMatrixChooser;
        }

        private void button79_Click(object sender, EventArgs e)
        {
            long Id = Convert.ToInt64(textBox8.Text);
            var Deepload = SAPUserinfo.GetByIdWithNavigation(Id);
            if (Deepload != null && Deepload.SAPRoleInfo.Count > 0)
            {
                MessageBox.Show("Deep Load works fine");
            }
            else
            {
                MessageBox.Show("Deep Load not works fine");
            }
        }

        private void button80_Click(object sender, EventArgs e)
        {
            //ComplianceEngine complianceEngine = new ComplianceEngine();
            //complianceEngine.StartProcess();
        }

        private void button81_Click(object sender, EventArgs e)
        {
            var sfbUser = SkypeForBusinessUserInfo.GetById(268);
            var entitlements = string.Empty;
            var sfbUserEntitlements = EntitlementReconcilation.GetActiveEntitlementByEndpointUserId(sfbUser.Id, sfbUser.EndPointSystemId);
            if (sfbUserEntitlements.Count > 0)
            {
                entitlements = JsonConvert.SerializeObject(sfbUserEntitlements);
            }
        }
        private void CreateTransferTriggers(long userId, long newOrgId, DateTime effectiveDate, long oldOrgId)
        {
            var evnt = EmployeeEvents.OUChange;

            var oldUser = User.GetById(userId);
            if (oldUser == null) return;
            var oldOrg = NOrganisation.GetById(oldOrgId);
            oldUser.NOrganisation = oldOrg;

            var oldUserDetail = UtilityMethods.Serialize<User>(oldUser);

            var newUser = User.GetById(userId);
            var newOrg = NOrganisation.GetById(newOrgId);
            newUser.NOrganisation = newOrg;
            var newUserDetail = UtilityMethods.Serialize<User>(newUser);

            if (newUser == null) return;

            var trigger = EmployeeEventTrigger.GetByUserAndEvent(newUser.Id, evnt, null);

            if (trigger != null)
            {
                var tempTrigger = EmployeeEventTrigger.GetByUserAndEvent(newUser.Id, EmployeeEvents.OUChange, DateTime.Now.AddMinutes(-10));
                if (tempTrigger != null && trigger.IsRequestCreated)
                {
                    return;
                }

                tempTrigger.EffectiveDate = effectiveDate;
                tempTrigger.LastModifiedBy = trigger.CreatedBy;
                tempTrigger.LastModifiedDate = DateTime.Now;
                tempTrigger.OldUserDetails = oldUserDetail;
                tempTrigger.UserDetails = newUserDetail;
                tempTrigger.Save(MasterConfigHelper.ServiceUser);
            }
            else
            {

                EmployeeEventTrigger newTrigger = new EmployeeEventTrigger()
                {
                    CreatedBy = Constants.CreatedBy,
                    CreatedDate = DateTime.Now,
                    EventType = evnt,
                    IsRequestCreated = false,
                    UserId = newUser.Id,
                    IsActive = true,
                    EffectiveDate = effectiveDate,
                    LastModifiedBy = Constants.CreatedBy,
                    LastModifiedDate = DateTime.Now,
                    UserDetails = newUserDetail,
                    OldUserDetails = oldUserDetail

                };


                newTrigger.Save(MasterConfigHelper.ServiceUser);

                //newUser.Save();
            }
        }
        private void button82_Click(object sender, EventArgs e)
        {
            var user = User.GetById(Convert.ToInt32(txtTfrUser.Text));

            var org = NOrganisation.GetById(Convert.ToInt64(txtTfrOrg.Text));

            CreateTransferTriggers(Convert.ToInt64(txtTfrUser.Text), Convert.ToInt64(txtTfrOrg.Text), DateTime.Now, user.NOrganisationId.Value);
        }

        private void button82_Click_1(object sender, EventArgs e)
        {
            try
            {
                var attestation = Attestation.GetById(1);

                if (attestation.RequiresAttestationRemediation)
                {
                    var deepLoadedTask = Task.GetAttestationTask(1, true);
                    if (deepLoadedTask != null)
                    {
                        var actions = AttestationSnapshotAction.GetByTask((long)deepLoadedTask.AttestationCampaignId, deepLoadedTask.Id).Where(a => a.Revoke);

                        foreach (var action in actions)
                        {
                            // set remediation status as Pending
                            action.RemediationStatus = AttestationRemediationStatus.Pending;
                            action.Save(MasterConfigHelper.ServiceUser);
                        }
                    }
                }

                //var attestation = Attestation.GetById(1);

                //if (attestation.RequiresAttestationRemediation)
                //{
                //    var tasks = Task.GetByWorkflowInstanceIdAndState(task.WorkflowInstanceId, task.WorkflowStateId);

                //    foreach (Task tsk in tasks)
                //    {
                //        var deepLoadedTask = Task.GetAttestationTask(tsk.Id, true);
                //        foreach (TaskReview taskReview in deepLoadedTask.TaskReviews)
                //        {
                //            taskReview.AttestationReview.RemediationStatus = AttestationRemediationStatus.Pending;
                //            (taskReview.AttestationReview).Save();
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while executing the RemediateAttestation activity", ex);
            }
        }

        private void button83_Click(object sender, EventArgs e)
        {
            //ReportingService service = new ReportingService();

            //Process process = Process.GetById(Convert.ToInt64(this.reportId.Text));

            //if (process != null)
            //{
            //    service.Start(process);
            //}

            //MessageBox.Show("Done");
        }


        // PDF creation test using iText dll
        private void button84_Click(object sender, EventArgs e)
        {
            try
            {
                //var doc1 = new Document();
                //PdfWriter.GetInstance(doc1, new FileStream(@"D:/tEST/Doc1.pdf", FileMode.Create));
                //doc1.Open();
                //doc1.Add(new Paragraph("My first PDF"));


                //PdfPTable table = new PdfPTable(4);
                //table.TotalWidth = 400f;
                //table.LockedWidth = true;
                //PdfPCell header = new PdfPCell(new Phrase("Header"));
                //header.Colspan = 4;
                //table.AddCell(header);
                //table.AddCell("Cell 1");
                //table.AddCell("Cell 2");
                //table.AddCell("Cell 3");
                //table.AddCell("Cell 4");
                //PdfPTable nested = new PdfPTable(1);
                //nested.AddCell("Nested Row 1");
                //nested.AddCell("Nested Row 2");
                //nested.AddCell("Nested Row 3");
                //PdfPCell nesthousing = new PdfPCell(nested);
                //nesthousing.Padding = 0f;
                //table.AddCell(nesthousing);
                //PdfPCell bottom = new PdfPCell(new Phrase("bottom"));
                //bottom.Colspan = 3;
                //table.AddCell(bottom);
                //doc1.Add(table);


                //doc1.Close();
                //ExportExcel(133,String.Format("TestABC-{0}",Guid.NewGuid().ToString()));
                //ExportPDF(161, Guid.NewGuid().ToString());
                //MessageBox.Show("Pdf created");

                //ExportPDF_sameLevel(11231, String.Format("TestABC-{0}", Guid.NewGuid().ToString()));
                //ExportExcel_sameLevel_recursive(11231, String.Format("TestABC-{0}", Guid.NewGuid().ToString()));

                //var test = AttestationSnapshotAction.GetByTask(10044, 10202);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not create pdf. Eexception: " + ex.Message);
            }
        }

        private void ExportExcel(long taskID, string campaignName)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            var attestationUserDetails = PresentationAttestationEntitlementReviewDetail.GetUsersForAttestationTaskReview(taskID, "iacmadmin", 1);

            OfficeOpenXml.ExcelPackage excel = new OfficeOpenXml.ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Attestation Review");

            //workSheet.Column(2).OutlineLevel = 1;
            //workSheet.Column(2).Collapsed = true;
            //workSheet.Column(3).OutlineLevel = 1;
            //workSheet.Column(3).Collapsed = true;
            //workSheet.OutLineSummaryRight = true;

            //workSheet.Row(5).OutlineLevel = 4;
            //workSheet.Row(5).Collapsed = true;
            //workSheet.Row(6).OutlineLevel = 4;
            //workSheet.Row(6).Collapsed = true;
            //workSheet.OutLineSummaryRight = true;

            int printLevel = 1;
            int currentRow = 1;
            int colStartPosition = 1;
            int colNum = colStartPosition;
            //int topRow = 0;
            int[] topRows = new int[] { 0, 0, 0, 0, 0 };
            int level = 0;

            // loop top level users
            // add workcell entry for top level
            // TOP User header
            workSheet.Cells[currentRow, colNum].Value = "Name";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "DisplayName";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "Type";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "ReviewCategory";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            currentRow++;
            foreach (PresentationAttestationEntitlementReviewDetail diUser in attestationUserDetails)
            {
                topRows[level] = currentRow;// level 2, cr = 6, tr[0]=2,tr[1]=4,tr[2]=6
                colStartPosition = 1;
                colNum = colStartPosition;
                // add workcell entry for top level
                workSheet.Cells[currentRow, colNum].Value = diUser.Name;
                workSheet.Cells[currentRow, ++colNum].Value = diUser.DisplayName;
                workSheet.Cells[currentRow, ++colNum].Value = diUser.Type;
                workSheet.Cells[currentRow, ++colNum].Value = diUser.ReviewCategory;
                currentRow++;
                level++;
                long keyId = diUser.KeyId;
                string category = diUser.ReviewCategory;
                //if (category == "User")
                //  ENDPOINT USERS (ACCOUNT) HEADER
                {
                    List<PresentationAttestationEntitlementReviewDetail> gridNextLevelDetails =
                        PresentationAttestationEntitlementReviewDetail.GetEndpointsForAttestationTaskReview(taskID, "iacmAdmin", 1);
                    // set outline
                    workSheet.Row(currentRow).OutlineLevel = topRows[level - 1];
                    workSheet.Row(currentRow).Collapsed = true;
                    // set column start position (incremented to create indent
                    colStartPosition = 2;
                    colNum = colStartPosition;
                    topRows[level] = currentRow;// level 2, cr = 6, tr[0]=2,tr[1]=4,tr[2]=6
                    workSheet.Cells[currentRow, colNum].Value = "Name";
                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                    workSheet.Cells[currentRow, ++colNum].Value = "DisplayName";
                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                    //workSheet.Cells[currentRow, ++colNum].Value = "EndpointType";
                    //workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                    //workSheet.Cells[currentRow, ++colNum].Value = "EndpointName";
                    //workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                    workSheet.Cells[currentRow, ++colNum].Value = "Type";
                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                    workSheet.Cells[currentRow, ++colNum].Value = "ReviewCategory";
                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                    workSheet.Cells[currentRow, ++colNum].Value = "Approved";
                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                    workSheet.Cells[currentRow, ++colNum].Value = "Revoked";
                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                    currentRow++;
                    level++; // level 2, cr = 5, tr[0]=2,tr[1]=4
                    // ENDPOINT USERS (ACCOUNT) DATA
                    foreach (PresentationAttestationEntitlementReviewDetail epUser in gridNextLevelDetails)
                    {
                        workSheet.Row(currentRow).OutlineLevel = topRows[level - 1];
                        workSheet.Row(currentRow).Collapsed = true;
                        colNum = colStartPosition;
                        // add workcell entry for top level
                        workSheet.Cells[currentRow, colNum].Value = epUser.Name;
                        workSheet.Cells[currentRow, ++colNum].Value = epUser.DisplayName;
                        //workSheet.Cells[currentRow, ++colNum].Value = epUser.EndpointType.ToString();
                        //workSheet.Cells[currentRow, ++colNum].Value = epUser.EndpointSystemName;
                        workSheet.Cells[currentRow, ++colNum].Value = epUser.Type;
                        workSheet.Cells[currentRow, ++colNum].Value = epUser.ReviewCategory;
                        workSheet.Cells[currentRow, ++colNum].Value = epUser.Approved.ToString();
                        workSheet.Cells[currentRow, ++colNum].Value = epUser.Rejected.ToString();
                        currentRow++;
                        // ROLE level HEADER
                        List<PresentationAttestationEntitlementReviewDetail> gridNextLevelDetails2 =
                            PresentationAttestationEntitlementReviewDetail.GetUsersForAttestationTaskReview(taskID, "iacmadmin", 1);
                        workSheet.Row(currentRow).OutlineLevel = topRows[level - 1];
                        workSheet.Row(currentRow).Collapsed = true;
                        colStartPosition = 3;
                        colNum = colStartPosition;
                        topRows[level] = currentRow;// level 2, cr = 6, tr[0]=2,tr[1]=4,tr[2]=6
                        workSheet.Cells[currentRow, colNum].Value = "Name";
                        workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                        workSheet.Cells[currentRow, ++colNum].Value = "DisplayName";
                        workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                        //workSheet.Cells[currentRow, ++colNum].Value = "EndpointType";
                        //workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                        //workSheet.Cells[currentRow, ++colNum].Value = "EndpointName";
                        //workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                        workSheet.Cells[currentRow, ++colNum].Value = "Type";
                        workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                        workSheet.Cells[currentRow, ++colNum].Value = "ReviewCategory";
                        workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                        workSheet.Cells[currentRow, ++colNum].Value = "Approved";
                        workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                        workSheet.Cells[currentRow, ++colNum].Value = "Revoked";
                        workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                        currentRow++;
                        level++;// level 3, cr = 6, tr[0]=2,tr[1]=4,tr[2]=6
                        // ROLE LEVEL DATA
                        foreach (PresentationAttestationEntitlementReviewDetail epAcnt in gridNextLevelDetails2)
                        {

                            workSheet.Row(currentRow).OutlineLevel = topRows[level - 1];
                            workSheet.Row(currentRow).Collapsed = true;
                            colNum = colStartPosition;
                            // add workcell entry for top level
                            workSheet.Cells[currentRow, colNum].Value = epAcnt.Name;
                            workSheet.Cells[currentRow, ++colNum].Value = epAcnt.DisplayName;
                            //workSheet.Cells[currentRow, ++colNum].Value = epAcnt.EndpointType.ToString();
                            //workSheet.Cells[currentRow, ++colNum].Value = epAcnt.EndpointSystemName;
                            workSheet.Cells[currentRow, ++colNum].Value = epAcnt.Type;
                            workSheet.Cells[currentRow, ++colNum].Value = epAcnt.ReviewCategory;
                            workSheet.Cells[currentRow, ++colNum].Value = epAcnt.Approved;
                            workSheet.Cells[currentRow, ++colNum].Value = epAcnt.Rejected;
                            currentRow++;

                            //// entitlement level
                            //List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail> gridNextLevelDetails3 = GetEntitlementDetailForAttestationTaskReview(keyId);
                            //colStartPosition = 4;
                            //workSheet.Cells[currentRow, colNum].Value = "Name";
                            //workSheet.Cells[currentRow, ++colNum].Value = "DisplayName";
                            //workSheet.Cells[currentRow, ++colNum].Value = "EndpointType";
                            //workSheet.Cells[currentRow, ++colNum].Value = "EndpointName";
                            //workSheet.Cells[currentRow, ++colNum].Value = "Type";
                            //workSheet.Cells[currentRow, ++colNum].Value = "ReviewCategory";
                            //currentRow++;
                            //foreach (Dto.Presentation.PresentationAttestationEntitlementReviewDetail epEnt in gridNextLevelDetails3)
                            //{
                            //    colNum = colStartPosition;
                            //    // add workcell entry for top level
                            //    workSheet.Cells[currentRow, colNum].Value = epEnt.Name;
                            //    workSheet.Cells[currentRow, ++colNum].Value = epEnt.DisplayName;
                            //    workSheet.Cells[currentRow, ++colNum].Value = epEnt.EndpointType.ToString();
                            //    workSheet.Cells[currentRow, ++colNum].Value = epEnt.EndpointSystemName;
                            //    workSheet.Cells[currentRow, ++colNum].Value = epEnt.Type;
                            //    workSheet.Cells[currentRow, ++colNum].Value = epEnt.ReviewCategory;
                            //    currentRow++;
                            //}
                        }
                        level--;
                    }
                    level--;
                }
                level--;
            }

            excel.SaveAs(new FileInfo(String.Format(@"D:/tEST/{0}.xlsx", campaignName)));
            //using (var memoryStream = new System.IO.MemoryStream())
            //{
            //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //    Response.AddHeader("content-disposition", "attachment;  filename=products.xlsx");
            //    excel.SaveAs(memoryStream);
            //    memoryStream.WriteTo(Response.OutputStream);
            //    Response.Flush();
            //    Response.End();
            //}
        }

        private void ExportExcel_sameLevel(long taskID, string campaignName)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            var attestationUserDetails = PresentationAttestationEntitlementReviewDetail.GetUsersForAttestationTaskReview(taskID, MasterConfigHelper.ServiceUser, 0, true);

            OfficeOpenXml.ExcelPackage excel = new OfficeOpenXml.ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Attestation Review");

            int printLevel = 1;
            int currentRow = 1;
            int colStartPosition = 1;
            int colNum = colStartPosition;

            // loop top level users
            // add workcell entry for top level
            // TOP User header
            workSheet.Cells[currentRow, colNum].Value = "Name";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "DisplayName";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "Type";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "LastLogon";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "ReviewCategory";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "Approved";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "Revoked";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            currentRow++;
            foreach (PresentationAttestationEntitlementReviewDetail diUser in attestationUserDetails)
            {
                colStartPosition = 1;
                colNum = colStartPosition;
                // add workcell entry for top level
                workSheet.Cells[currentRow, colNum].Value = diUser.Name;
                workSheet.Cells[currentRow, ++colNum].Value = diUser.DisplayName;
                workSheet.Cells[currentRow, ++colNum].Value = diUser.Type;
                workSheet.Cells[currentRow, ++colNum].Value = diUser.LastLogon;
                workSheet.Cells[currentRow, ++colNum].Value = diUser.ReviewCategory;
                currentRow++;
                long keyId = diUser.KeyId;
                string category = diUser.ReviewCategory;
                //if (category == "User")
                //  ENDPOINT USERS (ACCOUNT) HEADER
                {
                    List<PresentationAttestationEntitlementReviewDetail> gridNextLevelDetails =
                        PresentationAttestationEntitlementReviewDetail.GetDetailsForAttestationTaskReviewByUser(taskID, MasterConfigHelper.ServiceUser,
                        diUser.Level, diUser.BindingLevel, diUser.Entity, diUser.EndpointSystemId, "");
                    // set column start position (incremented to create indent
                    colStartPosition = 1;
                    colNum = colStartPosition;
                    //workSheet.Cells[currentRow, colNum].Value = "Name";
                    //workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                    //workSheet.Cells[currentRow, ++colNum].Value = "DisplayName";
                    //workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                    ////workSheet.Cells[currentRow, ++colNum].Value = "EndpointType";
                    ////workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                    ////workSheet.Cells[currentRow, ++colNum].Value = "EndpointName";
                    ////workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                    //workSheet.Cells[currentRow, ++colNum].Value = "Type";
                    //workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                    //workSheet.Cells[currentRow, ++colNum].Value = "ReviewCategory";
                    //workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                    //workSheet.Cells[currentRow, ++colNum].Value = "Approved";
                    //workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                    //workSheet.Cells[currentRow, ++colNum].Value = "Revoked";
                    //workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                    //currentRow++;
                    // ENDPOINT USERS (ACCOUNT) DATA
                    foreach (PresentationAttestationEntitlementReviewDetail epUser in gridNextLevelDetails)
                    {
                        colNum = colStartPosition;
                        // add workcell entry for top level
                        workSheet.Cells[currentRow, colNum].Value = epUser.Name;
                        workSheet.Cells[currentRow, ++colNum].Value = epUser.DisplayName;
                        //workSheet.Cells[currentRow, ++colNum].Value = epUser.EndpointType.ToString();
                        //workSheet.Cells[currentRow, ++colNum].Value = epUser.EndpointSystemName;
                        workSheet.Cells[currentRow, ++colNum].Value = epUser.Type;
                        workSheet.Cells[currentRow, ++colNum].Value = epUser.LastLogon;
                        workSheet.Cells[currentRow, ++colNum].Value = epUser.ReviewCategory;
                        workSheet.Cells[currentRow, ++colNum].Value = epUser.Approved.ToString();
                        workSheet.Cells[currentRow, ++colNum].Value = epUser.Rejected.ToString();
                        currentRow++;
                        // ROLE level HEADER
                        List<PresentationAttestationEntitlementReviewDetail> gridNextLevelDetails2 =
                            PresentationAttestationEntitlementReviewDetail.GetDetailsForAttestationTaskReviewByUser(taskID, MasterConfigHelper.ServiceUser,
                        epUser.Level, epUser.BindingLevel, epUser.Entity, epUser.EndpointSystemId, "");
                        //colStartPosition = 3;
                        //colNum = colStartPosition;
                        //workSheet.Cells[currentRow, colNum].Value = "Name";
                        //workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                        //workSheet.Cells[currentRow, ++colNum].Value = "DisplayName";
                        //workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                        ////workSheet.Cells[currentRow, ++colNum].Value = "EndpointType";
                        ////workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                        ////workSheet.Cells[currentRow, ++colNum].Value = "EndpointName";
                        ////workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                        //workSheet.Cells[currentRow, ++colNum].Value = "Type";
                        //workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                        //workSheet.Cells[currentRow, ++colNum].Value = "ReviewCategory";
                        //workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                        //workSheet.Cells[currentRow, ++colNum].Value = "Approved";
                        //workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                        //workSheet.Cells[currentRow, ++colNum].Value = "Revoked";
                        //workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                        //currentRow++;
                        // ROLE LEVEL DATA
                        foreach (PresentationAttestationEntitlementReviewDetail epAcnt in gridNextLevelDetails2)
                        {
                            colNum = colStartPosition;
                            // add workcell entry for top level
                            workSheet.Cells[currentRow, colNum].Value = epAcnt.Name;
                            workSheet.Cells[currentRow, ++colNum].Value = epAcnt.DisplayName;
                            //workSheet.Cells[currentRow, ++colNum].Value = epAcnt.EndpointType.ToString();
                            //workSheet.Cells[currentRow, ++colNum].Value = epAcnt.EndpointSystemName;
                            workSheet.Cells[currentRow, ++colNum].Value = epAcnt.Type;
                            workSheet.Cells[currentRow, ++colNum].Value = epAcnt.LastLogon;
                            workSheet.Cells[currentRow, ++colNum].Value = epAcnt.ReviewCategory;
                            workSheet.Cells[currentRow, ++colNum].Value = epAcnt.Approved;
                            workSheet.Cells[currentRow, ++colNum].Value = epAcnt.Rejected;
                            currentRow++;

                            //// entitlement level
                            //List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail> gridNextLevelDetails3 = GetEntitlementDetailForAttestationTaskReview(keyId);
                            //colStartPosition = 4;
                            //workSheet.Cells[currentRow, colNum].Value = "Name";
                            //workSheet.Cells[currentRow, ++colNum].Value = "DisplayName";
                            //workSheet.Cells[currentRow, ++colNum].Value = "EndpointType";
                            //workSheet.Cells[currentRow, ++colNum].Value = "EndpointName";
                            //workSheet.Cells[currentRow, ++colNum].Value = "Type";
                            //workSheet.Cells[currentRow, ++colNum].Value = "ReviewCategory";
                            //currentRow++;
                            //foreach (Dto.Presentation.PresentationAttestationEntitlementReviewDetail epEnt in gridNextLevelDetails3)
                            //{
                            //    colNum = colStartPosition;
                            //    // add workcell entry for top level
                            //    workSheet.Cells[currentRow, colNum].Value = epEnt.Name;
                            //    workSheet.Cells[currentRow, ++colNum].Value = epEnt.DisplayName;
                            //    workSheet.Cells[currentRow, ++colNum].Value = epEnt.EndpointType.ToString();
                            //    workSheet.Cells[currentRow, ++colNum].Value = epEnt.EndpointSystemName;
                            //    workSheet.Cells[currentRow, ++colNum].Value = epEnt.Type;
                            //    workSheet.Cells[currentRow, ++colNum].Value = epEnt.ReviewCategory;
                            //    currentRow++;
                            //}
                        }
                    }
                }
            }


            // ToDO: Save to ftp location or Database
            excel.SaveAs(new FileInfo(String.Format("{0}{1}.xlsx", MasterConfigHelper.AttestationSnapshotFilePath, campaignName)));
            //using (var memoryStream = new System.IO.MemoryStream())
            //{
            //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //    Response.AddHeader("content-disposition", "attachment;  filename=products.xlsx");
            //    excel.SaveAs(memoryStream);
            //    memoryStream.WriteTo(Response.OutputStream);
            //    Response.Flush();
            //    Response.End();
            //}
        }

        private int GetAttestationDetailsList(long taskID, PresentationAttestationEntitlementReviewDetail parent,
            OfficeOpenXml.ExcelWorksheet workSheet, int currentRow)
        {
            var grid = PresentationAttestationEntitlementReviewDetail.GetDetailsForAttestationTaskReviewByUser(taskID, MasterConfigHelper.ServiceUser,
            parent.Level, parent.BindingLevel, parent.Entity, parent.EndpointSystemId, "");

            int colNum = 1;

            if (grid != null && grid.Count > 0)
            {
                foreach (var data in grid)
                {
                    colNum = 1;
                    // add workcell entry for top level
                    workSheet.Cells[currentRow, colNum].Value = data.EndpointSystemName;
                    workSheet.Cells[currentRow, colNum].Value = parent.Name;
                    workSheet.Cells[currentRow, ++colNum].Value = parent.DisplayName;
                    //workSheet.Cells[currentRow, ++colNum].Value = epUser.EndpointType.ToString();
                    //workSheet.Cells[currentRow, ++colNum].Value = epUser.EndpointSystemName;
                    workSheet.Cells[currentRow, ++colNum].Value = data.Name;
                    workSheet.Cells[currentRow, ++colNum].Value = data.LastLogon;
                    workSheet.Cells[currentRow, ++colNum].Value = data.ReviewCategory;
                    workSheet.Cells[currentRow, ++colNum].Value = data.Approved.ToString();
                    workSheet.Cells[currentRow, ++colNum].Value = data.Rejected.ToString();
                    currentRow++;

                    currentRow = GetAttestationDetailsList(taskID, data, workSheet, currentRow);
                }
            }

            return currentRow;
        }

        private void ExportExcel_sameLevel_recursive(long taskID, string campaignName)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            var attestationUserDetails = PresentationAttestationEntitlementReviewDetail.GetUsersForAttestationTaskReview(taskID, MasterConfigHelper.ServiceUser, 0, true);

            OfficeOpenXml.ExcelPackage excel = new OfficeOpenXml.ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Attestation Review");

            int currentRow = 1;
            int colStartPosition = 1;
            int colNum = colStartPosition;

            // loop top level users
            // add workcell entry for top level
            // TOP User header
            workSheet.Cells[currentRow, colNum].Value = "Application Name";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, colNum].Value = "Name";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "DisplayName";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "Entitlement";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "LastLogon";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "ReviewCategory";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "Approved";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "Revoked";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            currentRow++;
            foreach (PresentationAttestationEntitlementReviewDetail diUser in attestationUserDetails)
            {
                colStartPosition = 1;
                colNum = colStartPosition;
                // add workcell entry for top level
                workSheet.Cells[currentRow, colNum].Value = String.Empty;
                workSheet.Cells[currentRow, ++colNum].Value = diUser.Name;
                workSheet.Cells[currentRow, ++colNum].Value = diUser.DisplayName;
                workSheet.Cells[currentRow, ++colNum].Value = String.Empty;
                workSheet.Cells[currentRow, ++colNum].Value = String.Empty;
                workSheet.Cells[currentRow, ++colNum].Value = diUser.Type;
                workSheet.Cells[currentRow, ++colNum].Value = String.Empty;
                workSheet.Cells[currentRow, ++colNum].Value = String.Empty;
                currentRow++;

                currentRow = GetAttestationDetailsList(taskID, diUser, workSheet, currentRow);
            }

            workSheet.Column(1).AutoFit(0, 50);
            workSheet.Column(2).AutoFit(0, 50);
            workSheet.Column(3).AutoFit(0, 50);
            workSheet.Column(4).AutoFit(0, 50);
            workSheet.Column(5).AutoFit(0, 50);
            workSheet.Column(6).AutoFit(0, 50);
            workSheet.Column(7).AutoFit(0, 50);
            workSheet.Column(8).AutoFit(0, 50);

            // ToDO: Save to ftp location or Database
            excel.SaveAs(new FileInfo(String.Format("{0}{1}.xlsx", MasterConfigHelper.AttestationSnapshotFilePath, campaignName)));
        }
        //private PdfPCell GetPDFCell(string cellText, bool isheader)
        //{
        //    PdfPCell cell = new PdfPCell(new Phrase(cellText));
        //    if (isheader)
        //    {
        //       // cell.BackgroundColor = Color.GRAY;
        //    }
        //    return cell;
        //}
        //private void ExportPDF(long taskId, string campaignName)
        //{
        //    Dictionary<string, string> parameter = new Dictionary<string, string>();
        //    //int taskID = 1;
        //    var attestationUserDetails = PresentationAttestationEntitlementReviewDetail.GetUsersForAttestationTaskReview(taskId, MasterConfigHelper.ServiceUser, 1);
        //    var doc1 = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 10f);
        //    PdfWriter.GetInstance(doc1, new FileStream(String.Format(@"D:/tEST/{0}.pdf", campaignName), FileMode.Create));
        //    doc1.Open();


        //    PdfPTable table = new PdfPTable(8);
        //    //table.SetWidths(new float[] { 0.3f, 1f });
        //    //table.TotalWidth = 800f;
        //    //table.LockedWidth = true;
        //    table.WidthPercentage = 95;


        //    int printLevel = 1;
        //    int currentRow = 1;
        //    int colStartPosition = 1;
        //    int colNum = colStartPosition;

        //    // loop top level users
        //    // add workcell entry for top level
        //    table.AddCell(GetPDFCell("Name", true));
        //    table.AddCell(GetPDFCell("DisplayName", true));
        //    table.AddCell(GetPDFCell("Type", true));
        //    table.AddCell(GetPDFCell("ReviewCategory", true));
        //    table.AddCell(GetPDFCell("", true));
        //    table.AddCell(GetPDFCell("", true));
        //    table.AddCell(GetPDFCell("", true));
        //    table.AddCell(GetPDFCell("", true));
        //    currentRow++;
        //    foreach (PresentationAttestationEntitlementReviewDetail diUser in attestationUserDetails)
        //    {
        //        colStartPosition = 1;
        //        colNum = colStartPosition;
        //        // add workcell entry for top level
        //        table.AddCell(GetPDFCell(diUser.Name, false));
        //        table.AddCell(GetPDFCell(diUser.DisplayName, false));
        //        table.AddCell(GetPDFCell(diUser.Type, false));
        //        table.AddCell(GetPDFCell(diUser.ReviewCategory, false));
        //        table.AddCell(GetPDFCell("", false));
        //        table.AddCell(GetPDFCell("", false));
        //        table.AddCell(GetPDFCell("", false));
        //        table.AddCell(GetPDFCell("", false));
        //        currentRow++;

        //        long keyId = diUser.KeyId;
        //        string category = diUser.ReviewCategory;
        //        //if (category == "User")
        //        {
        //            List<PresentationAttestationEntitlementReviewDetail> gridNextLevelDetails =
        //                PresentationAttestationEntitlementReviewDetail.GetEndpointsForAttestationTaskReview(taskId, MasterConfigHelper.ServiceUser, 1);
        //            colStartPosition = 2;
        //            colNum = colStartPosition;
        //            table.AddCell(GetPDFCell("", true));
        //            table.AddCell(GetPDFCell("Name", true));
        //            table.AddCell(GetPDFCell("DisplayName", true));
        //            table.AddCell(GetPDFCell("Type", true));
        //            table.AddCell(GetPDFCell("ReviewCategory", true));
        //            table.AddCell(GetPDFCell("Approved", true));
        //            table.AddCell(GetPDFCell("Revoked", true));
        //            table.AddCell(GetPDFCell("", true));
        //            currentRow++;
        //            foreach (PresentationAttestationEntitlementReviewDetail epUser in gridNextLevelDetails)
        //            {
        //                colNum = colStartPosition;
        //                table.AddCell(GetPDFCell("", false));
        //                table.AddCell(GetPDFCell(epUser.Name, false));
        //                table.AddCell(GetPDFCell(epUser.DisplayName, false));
        //                table.AddCell(GetPDFCell(epUser.Type, false));
        //                table.AddCell(GetPDFCell(epUser.ReviewCategory, false));

        //                table.AddCell(new PdfPCell(new Phrase("\u221A", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD))));
        //                //table.AddCell(GetPDFCell(epUser.Approved.ToString(), false));
        //                table.AddCell(GetPDFCell(epUser.Rejected.ToString(), false));
        //                table.AddCell(GetPDFCell("", false));
        //                currentRow++;

        //                // Account level
        //                List<PresentationAttestationEntitlementReviewDetail> gridNextLevelDetails2 =
        //                    PresentationAttestationEntitlementReviewDetail.GetUsersForAttestationTaskReview(taskId, MasterConfigHelper.ServiceUser, 1);
        //                colStartPosition = 3;
        //                colNum = colStartPosition;
        //                table.AddCell(GetPDFCell("", true));
        //                table.AddCell(GetPDFCell("", true));
        //                table.AddCell(GetPDFCell("Name", true));
        //                table.AddCell(GetPDFCell("DisplayName", true));
        //                table.AddCell(GetPDFCell("Type", true));
        //                table.AddCell(GetPDFCell("ReviewCategory", true));
        //                table.AddCell(GetPDFCell("Approved", true));
        //                table.AddCell(GetPDFCell("Revoked", true));
        //                currentRow++;
        //                foreach (PresentationAttestationEntitlementReviewDetail epAcnt in gridNextLevelDetails2)
        //                {
        //                    colNum = colStartPosition;
        //                    table.AddCell(GetPDFCell("", false));
        //                    table.AddCell(GetPDFCell("", false));
        //                    table.AddCell(GetPDFCell(epAcnt.Name, false));
        //                    table.AddCell(GetPDFCell(epAcnt.DisplayName, false));
        //                    table.AddCell(GetPDFCell(epAcnt.Type, false));
        //                    table.AddCell(GetPDFCell(epAcnt.ReviewCategory, false));
        //                    table.AddCell(GetPDFCell(epUser.Approved.ToString(), false));
        //                    table.AddCell(GetPDFCell(epUser.Rejected.ToString(), false));
        //                    currentRow++;

        //                    //// entitlement level
        //                    //List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail> gridNextLevelDetails3 = GetEntitlementDetailForAttestationTaskReview(keyId);
        //                    //colStartPosition = 4;
        //                    //workSheet.Cells[currentRow, colNum].Value = "Name";
        //                    //workSheet.Cells[currentRow, ++colNum].Value = "DisplayName";
        //                    //workSheet.Cells[currentRow, ++colNum].Value = "EndpointType";
        //                    //workSheet.Cells[currentRow, ++colNum].Value = "EndpointName";
        //                    //workSheet.Cells[currentRow, ++colNum].Value = "Type";
        //                    //workSheet.Cells[currentRow, ++colNum].Value = "ReviewCategory";
        //                    //currentRow++;
        //                    //foreach (Dto.Presentation.PresentationAttestationEntitlementReviewDetail epEnt in gridNextLevelDetails3)
        //                    //{
        //                    //    colNum = colStartPosition;
        //                    //    // add workcell entry for top level
        //                    //    workSheet.Cells[currentRow, colNum].Value = epEnt.Name;
        //                    //    workSheet.Cells[currentRow, ++colNum].Value = epEnt.DisplayName;
        //                    //    workSheet.Cells[currentRow, ++colNum].Value = epEnt.EndpointType.ToString();
        //                    //    workSheet.Cells[currentRow, ++colNum].Value = epEnt.EndpointSystemName;
        //                    //    workSheet.Cells[currentRow, ++colNum].Value = epEnt.Type;
        //                    //    workSheet.Cells[currentRow, ++colNum].Value = epEnt.ReviewCategory;
        //                    //    currentRow++;
        //                    //}
        //                }
        //            }
        //        }
        //    }
        //    doc1.Add(table);
        //    doc1.Close();
        //}

        //private void ExportPDF_sameLevel(long taskId, string campaignName)
        //{
        //    Dictionary<string, string> parameter = new Dictionary<string, string>();
        //    //int taskID = 1;
        //    var attestationUserDetails = PresentationAttestationEntitlementReviewDetail.GetUsersForAttestationTaskReview(taskId, MasterConfigHelper.ServiceUser, 0, true);
        //    var doc1 = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 10f);
        //    PdfWriter.GetInstance(doc1, new FileStream(String.Format(@"D:/tEST/{0}.pdf", campaignName), FileMode.Create));
        //    doc1.Open();


        //    PdfPTable table = new PdfPTable(8);
        //    //table.SetWidths(new float[] { 0.3f, 1f });
        //    //table.TotalWidth = 800f;
        //    //table.LockedWidth = true;
        //    table.WidthPercentage = 95;


        //    int printLevel = 1;
        //    int currentRow = 1;
        //    int colStartPosition = 1;
        //    int colNum = colStartPosition;

        //    // loop top level users
        //    // add workcell entry for top level
        //    table.AddCell(GetPDFCell("Name", true));
        //    table.AddCell(GetPDFCell("Display Name", true));
        //    table.AddCell(GetPDFCell("Type", true));
        //    table.AddCell(GetPDFCell("Last Logon", true));
        //    table.AddCell(GetPDFCell("Review Category", true));
        //    table.AddCell(GetPDFCell("Approved", true));
        //    table.AddCell(GetPDFCell("Revoked", true));
        //    table.AddCell(GetPDFCell("", true));
        //    currentRow++;
        //    foreach (PresentationAttestationEntitlementReviewDetail diUser in attestationUserDetails)
        //    {
        //        colStartPosition = 1;
        //        colNum = colStartPosition;
        //        // add workcell entry for top level
        //        table.AddCell(GetPDFCell(diUser.Name, false));
        //        table.AddCell(GetPDFCell(diUser.DisplayName, false));
        //        table.AddCell(GetPDFCell(diUser.Type, false));
        //        table.AddCell(GetPDFCell("", false));
        //        table.AddCell(GetPDFCell(diUser.ReviewCategory, false));
        //        table.AddCell(GetPDFCell("", false));
        //        table.AddCell(GetPDFCell("", false));
        //        table.AddCell(GetPDFCell("", false));
        //        currentRow++;

        //        long keyId = diUser.KeyId;
        //        string category = diUser.ReviewCategory;
        //        //if (category == "User")
        //        //{
        //        List<PresentationAttestationEntitlementReviewDetail> gridNextLevelDetails =
        //            PresentationAttestationEntitlementReviewDetail.GetDetailsForAttestationTaskReviewByUser(taskId, MasterConfigHelper.ServiceUser, diUser.Level, diUser.BindingLevel, diUser.Entity, diUser.EndpointSystemId, "");
        //        colStartPosition = 2;
        //        colNum = colStartPosition;
        //        //table.AddCell(GetPDFCell("", true));
        //        //table.AddCell(GetPDFCell("Name", true));
        //        //table.AddCell(GetPDFCell("DisplayName", true));
        //        //table.AddCell(GetPDFCell("Type", true));
        //        //table.AddCell(GetPDFCell("ReviewCategory", true));
        //        //table.AddCell(GetPDFCell("Approved", true));
        //        //table.AddCell(GetPDFCell("Revoked", true));
        //        //table.AddCell(GetPDFCell("", true));
        //        //currentRow++;
        //        foreach (PresentationAttestationEntitlementReviewDetail epUser in gridNextLevelDetails)
        //        {
        //            colNum = colStartPosition;
        //            table.AddCell(GetPDFCell(epUser.Name, false));
        //            table.AddCell(GetPDFCell(epUser.DisplayName, false));
        //            table.AddCell(GetPDFCell(epUser.Type, false));
        //            table.AddCell(GetPDFCell(epUser.LastLogon, false));
        //            table.AddCell(GetPDFCell(epUser.ReviewCategory, false));
        //            //table.AddCell(new PdfPCell(new Phrase("\u221A", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, Color.GREEN))));
        //            table.AddCell(GetPDFCell(epUser.Approved.ToString(), false));
        //            table.AddCell(GetPDFCell(epUser.Rejected.ToString(), false));
        //            table.AddCell(GetPDFCell("", false));
        //            currentRow++;

        //            // Account level
        //            List<PresentationAttestationEntitlementReviewDetail> gridNextLevelDetails2 =
        //                PresentationAttestationEntitlementReviewDetail.GetDetailsForAttestationTaskReviewByUser(taskId, MasterConfigHelper.ServiceUser,
        //                epUser.Level, epUser.BindingLevel, epUser.Entity, epUser.EndpointSystemId, "");

        //            colStartPosition = 3;
        //            colNum = colStartPosition;
        //            //table.AddCell(GetPDFCell("", true));
        //            //table.AddCell(GetPDFCell("", true));
        //            //table.AddCell(GetPDFCell("Name", true));
        //            //table.AddCell(GetPDFCell("DisplayName", true));
        //            //table.AddCell(GetPDFCell("Type", true));
        //            //table.AddCell(GetPDFCell("ReviewCategory", true));
        //            //table.AddCell(GetPDFCell("Approved", true));
        //            //table.AddCell(GetPDFCell("Revoked", true));
        //            //currentRow++;
        //            foreach (PresentationAttestationEntitlementReviewDetail epAcnt in gridNextLevelDetails2)
        //            {
        //                colNum = colStartPosition;
        //                table.AddCell(GetPDFCell(epAcnt.Name, false));
        //                table.AddCell(GetPDFCell(epAcnt.DisplayName, false));
        //                table.AddCell(GetPDFCell(epAcnt.Type, false));
        //                table.AddCell(GetPDFCell(epAcnt.LastLogon, false));
        //                table.AddCell(GetPDFCell(epAcnt.ReviewCategory, false));
        //                table.AddCell(GetPDFCell(epUser.Approved.ToString(), false));
        //                table.AddCell(GetPDFCell(epUser.Rejected.ToString(), false));
        //                table.AddCell(GetPDFCell("", false));
        //                currentRow++;

        //                //// entitlement level
        //                //List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail> gridNextLevelDetails3 = GetEntitlementDetailForAttestationTaskReview(keyId);
        //                //colStartPosition = 4;
        //                //workSheet.Cells[currentRow, colNum].Value = "Name";
        //                //workSheet.Cells[currentRow, ++colNum].Value = "DisplayName";
        //                //workSheet.Cells[currentRow, ++colNum].Value = "EndpointType";
        //                //workSheet.Cells[currentRow, ++colNum].Value = "EndpointName";
        //                //workSheet.Cells[currentRow, ++colNum].Value = "Type";
        //                //workSheet.Cells[currentRow, ++colNum].Value = "ReviewCategory";
        //                //currentRow++;
        //                //foreach (Dto.Presentation.PresentationAttestationEntitlementReviewDetail epEnt in gridNextLevelDetails3)
        //                //{
        //                //    colNum = colStartPosition;
        //                //    // add workcell entry for top level
        //                //    workSheet.Cells[currentRow, colNum].Value = epEnt.Name;
        //                //    workSheet.Cells[currentRow, ++colNum].Value = epEnt.DisplayName;
        //                //    workSheet.Cells[currentRow, ++colNum].Value = epEnt.EndpointType.ToString();
        //                //    workSheet.Cells[currentRow, ++colNum].Value = epEnt.EndpointSystemName;
        //                //    workSheet.Cells[currentRow, ++colNum].Value = epEnt.Type;
        //                //    workSheet.Cells[currentRow, ++colNum].Value = epEnt.ReviewCategory;
        //                //    currentRow++;
        //                //}
        //            }
        //        }
        //        //}
        //    }
        //    doc1.Add(table);
        //    doc1.Close();
        //}

        //public PdfPCell getCell(String text, int alignment)
        //{
        //    PdfPCell cell = new PdfPCell(new Phrase(text));
        //    cell.Padding = 0;
        //    cell.HorizontalAlignment = alignment;
        //    cell.MinimumHeight = 20f;
        //    cell.Border = PdfPCell.NO_BORDER;
        //    return cell;
        //}

        //public PdfPCell getNestedTableCell(PdfPTable table, int alignment)
        //{
        //    PdfPCell cell = new PdfPCell(table);
        //    cell.Padding = 0;
        //    cell.HorizontalAlignment = alignment;
        //    cell.MinimumHeight = 20f;
        //    cell.Border = PdfPCell.NO_BORDER;
        //    return cell;
        //}

        public string getEmployeeName()
        {
            return "John DOE";
        }

        public string getEmployeeID()
        {
            return "012345";
        }

        public string getEmployeeDepartment()
        {
            return "MICE Event Services";
        }

        public string getUserID() { return "john.doe"; }

        public string getUserPassword()
        {
            return "96&AJTsG#";
        }

        public string getUserEmail() { return "john.doe@rwsentosa.com"; }

        public string getNotesAccountDetails() { return " The following new LAN ID and Internet email account is created for you. You may use it with immediate effect. Your ID / Password should not be disclosed to anyone or used for testing, as you will be held accountable for all accesses done through your ID.If you have any problem with your account, please call the IT Service Desk at 655 41188 or * 99 when using our IP Phone."; }

        private void button85_Click(object sender, EventArgs e)
        {
            try
            {

                // var nruleset = NRuleset.GetMatchingRulesets(63);

                //PasswordLetterJob t1 = new PasswordLetterJob();
                //t1.BuildPreConditionResultSet();
                //t1.Execute();

                //InterimStoreCleanup t1 = new InterimStoreCleanup();
                //t1.BuildPreConditionResultSet();
                //t1.Execute();



                //ReportingEngine reportingEngine = new ReportingEngine();
                //reportingEngine.StartProcess();

                //PasswordLetterProcessor PLProcessor = new PasswordLetterProcessor();
                //PLProcessor.CreatePDF();


                //var doc1 = new Document();
                //PdfWriter.GetInstance(doc1, new FileStream(@"C:/tEST/passwordLetter.pdf", FileMode.Create));
                //doc1.Open();

                //for (int i = 0; i < 5; i++)
                //{
                //    Paragraph mainContainer = new Paragraph();
                //    mainContainer.IndentationLeft = 20f;

                //    Font boldFont = FontFactory.GetFont(FontFactory.COURIER_BOLD);
                //    Font boldUnderline = new Font();
                //    boldUnderline.SetStyle("underline");
                //    boldUnderline.SetStyle("bold");

                //    Paragraph p1 = new Paragraph();
                //    p1.IndentationLeft = 10f;

                //    Chunk chk1 = new Chunk("Network Login & Email Account", boldFont);
                //    p1.Add(chk1);

                //    PdfPTable container = new PdfPTable(2);

                //    PdfPTable personalInfo = new PdfPTable(2);
                //    personalInfo.TotalWidth = 295f;
                //    personalInfo.LockedWidth = true;
                //    personalInfo.DefaultCell.Border = Rectangle.NO_BORDER;
                //    personalInfo.AddCell(getCell("Name", PdfPCell.ALIGN_LEFT));
                //    personalInfo.AddCell(getCell(": " + getEmployeeName(), PdfPCell.ALIGN_LEFT));
                //    personalInfo.AddCell(getCell("Employee ID", PdfPCell.ALIGN_LEFT));
                //    personalInfo.AddCell(getCell(": " + getEmployeeID(), PdfPCell.ALIGN_LEFT));
                //    personalInfo.AddCell(getCell("Department", PdfPCell.ALIGN_LEFT));
                //    personalInfo.AddCell(getCell(": " + getEmployeeDepartment(), PdfPCell.ALIGN_LEFT));

                //    container.AddCell(getNestedTableCell(personalInfo, 0));
                //    container.AddCell(getCell("", 0));
                //    container.AddCell(getCell("", 0));
                //    p1.Add(container);

                //    p1.Add(Chunk.NEWLINE);
                //    p1.Add(Chunk.NEWLINE);
                //    p1.Add(Chunk.NEWLINE);

                //    container = new PdfPTable(2);
                //    PdfPTable credInfo = new PdfPTable(2);
                //    credInfo.TotalWidth = 295f;
                //    credInfo.LockedWidth = true;
                //    credInfo.DefaultCell.Border = Rectangle.NO_BORDER;
                //    credInfo.AddCell(getCell("User ID ", PdfPCell.ALIGN_LEFT));
                //    credInfo.AddCell(getCell(": " + getUserID(), PdfPCell.ALIGN_LEFT));
                //    credInfo.AddCell(getCell("Password", PdfPCell.ALIGN_LEFT));
                //    credInfo.AddCell(getCell(": " + getUserPassword(), PdfPCell.ALIGN_LEFT));
                //    credInfo.AddCell(getCell("Internet Email Address", PdfPCell.ALIGN_LEFT));
                //    credInfo.AddCell(getCell(": " + getUserEmail(), PdfPCell.ALIGN_LEFT));

                //    container.AddCell(getNestedTableCell(credInfo, 0));
                //    container.AddCell(getCell("", 0));
                //    p1.Add(container);

                //    Paragraph p2 = new Paragraph();

                //    p2.Add(Chunk.NEWLINE);
                //    List li = new List(List.UNORDERED);
                //    li.SetListSymbol("\u2022");
                //    foreach (var item in getNotesAccountDetails().Split('.'))
                //    {
                //        li.Add(item);
                //    }
                //    //li.Add(" The following new LAN ID and Internet email account is created for you.");
                //    //li.Add(" You may use it with immediate effect.");
                //    //li.Add(" Your ID / Password should not be disclosed to anyone or used for testing, as you will be held accountable for all accesses done through your ID.");
                //    //li.Add(" If you have any problem with your account, please call the IT Service Desk at 655 41188 or *99 when using our IP Phone.");
                //    ////Chunk chk2 = new Chunk();
                //    p2.Add(li);

                //    p2.Add(Chunk.NEWLINE);
                //    p2.Add(Chunk.NEWLINE);

                //    Paragraph p3 = new Paragraph();
                //    p3.IndentationLeft = 10f;
                //    p3.Add(new Chunk("Things to take note of:", boldFont));

                //    Paragraph p4 = new Paragraph();
                //    p4.IndentationLeft = 10f;

                //    p4.Add(new Chunk("Account credentials :", boldUnderline));
                //    List credInfoText = new List(List.ORDERED, 20f);
                //    credInfoText.SetListSymbol("\u2022");
                //    credInfoText.Add(" The account and password issued to you should not be shared nor disclosed to any other person.");
                //    credInfoText.Add(" You would be held accountable for all access done through your ID.");
                //    credInfoText.Add(" Please change the password once you have access to the network.");
                //    credInfoText.Add(" Password should be a minimum of 8 characters with alphanumeric characters.");
                //    credInfoText.Add(" Simple and easy to guess passwords must be avoided. (For example, password123)");
                //    //Chunk chk2 = new Chunk();
                //    p4.Add(credInfoText);
                //    p4.Add(Chunk.NEWLINE);

                //    Paragraph p5 = new Paragraph();
                //    p5.IndentationLeft = 10f;
                //    p5.Add(new Chunk("Use of RWS IT systems :", boldUnderline));
                //    List sysInfoText = new List(List.ORDERED, 20f);
                //    sysInfoText.SetListSymbol("\u2022");
                //    sysInfoText.Add(" The account and password issued to you should not be shared nor disclosed to any other person.");
                //    sysInfoText.Add(" You would be held accountable for all access done through your ID.");
                //    sysInfoText.Add(" Please change the password once you have access to the network.");
                //    sysInfoText.Add(" Password should be a minimum of 8 characters with alphanumeric characters.");
                //    sysInfoText.Add(" Simple and easy to guess passwords must be avoided. (For example, password123)");
                //    //Chunk chk2 = new Chunk();
                //    p5.Add(sysInfoText);

                //    p3.Add(p4);
                //    p3.Add(p5);

                //    mainContainer.Add(p1);
                //    mainContainer.Add(p2);
                //    mainContainer.Add(p3);

                //    doc1.Add(mainContainer);
                //    doc1.NewPage();
                //}

                //doc1.Close();
                MessageBox.Show("Pdf created");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not create pdf. Eexception: " + ex.Message);
            }
        }

        private void button86_Click(object sender, EventArgs e)
        {
            //ReportingEngine reportingEngine = new ReportingEngine();
            //reportingEngine.StartProcess();
        }

        private void button87_Click(object sender, EventArgs e)
        {

            //Process process = null;
            //var reportprocessId = Convert.ToInt64(textBox9.Text);
            //var runDate = Convert.ToDateTime(textBox10.Text);
            //process = Process.GetByProcessTypeKeyId(reportprocessId, ProcessType.JobReport);
            //ReportingService service = new ReportingService();
            //service.StartAdHocReport(process, runDate);
        }

        private void btnTestReturnId_Click(object sender, EventArgs e)
        {
            try
            {
                DI.Chakra.Dto.AdHocAccessRequest adhocrequst = new DI.Chakra.Dto.AdHocAccessRequest();
                adhocrequst.RequesteeUserId = 82914;
                adhocrequst.RequesterUserId = 1;
                adhocrequst.EndpointSystemType = EndpointType.ActiveDirectory;
                adhocrequst.EndpointSystemId = 2;
                adhocrequst.AdhocActionType = AdHocActionType.NewAccount;
                adhocrequst.AccountType = AccountType.User;
                adhocrequst.ValidFrom = DateTime.Now;
                adhocrequst.Comments = "Employee Creation Request";
                var Id = DomainApiClient<DI.Chakra.Dto.AdHocAccessRequest>.SaveAndReturnId(adhocrequst);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button88_Click(object sender, EventArgs e)
        {
            try
            {
                EmailAgent emailAgent = new EmailAgent();
                emailAgent.SendEmails(EmailActivityType.None);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button89_Click(object sender, EventArgs e)
        {
            try
            {
                int endpointSystemId = int.Parse(this.textBox4.Text.Trim());
                var endpoint = DI.Chakra.DomainModel.EndPoint.GetByIdIncludingOwner(endpointSystemId);
                var connector = ConnectorFactory.GetConnector(endpoint, null, null);
                if (connector.Connect())
                {
                    MessageBox.Show("Connected Successfully");
                }
                else
                {
                    MessageBox.Show("Failed to connect");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnTestSAPConnection_Click(object sender, EventArgs e)
        {
            try
            {
                var endpoint = DI.Chakra.DomainModel.EndPoint.GetEndPointByApplicationType(ApplicationType.EBS_ISU);
                ConnectorBase _connector = ConnectorFactory.GetConnector(endpoint, null);
                _connector.TestSAPConnection();
                MessageBox.Show("Connection successful");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
        }

        private void btnSAPUtiltity_Click(object sender, EventArgs e)
        {
            //var res = DI.Chakra.DomainModel.User.GetEmailIdsByGroupId(1);
            SAPUtilityForm utilityForm = new SAPUtilityForm();
            utilityForm.Show();
        }

        private void button89_Click_1(object sender, EventArgs e)
        {
            var sapHRMSEndPoint = DI.Chakra.DomainModel.EndPoint.GetTrustedSourceEndPoint();
            User user = User.GetById(Convert.ToInt64(textBox4.Text));
            if (sapHRMSEndPoint != null)
            {
                ConnectorBase sapConnector = ConnectorFactory.GetConnector(sapHRMSEndPoint, null);
                if (sapConnector.AddTelephoneInHR(user.EmployeeId, user.TelephoneNumber, user.TelephoneNumber, user.JoinDate.Value))
                {
                    MessageBox.Show($"SAPHRMSAttributes successfully updated");
                }
                else
                {
                    MessageBox.Show($"SAPHRMSAttributes failed to updated");
                }
            }
        }

        private void btnTestSRMDiscovery_Click(object sender, EventArgs e)
        {

            //var ee = EmployeeEventTrigger.GetById(73098);

            ////var org = new OrgChangeTrack(EmployeeEvents.Propagation, "40", "5", DateTime.Now);
            ////var str = org.GetComment();
            //var s= EmployeeEventTriggerComment.GetEmployeeEventTriggerComment(ee.Comments);




            string pernr = "20000350"; 

            var endpoint = DI.Chakra.DomainModel.EndPoint.GetTrustedSourceEndPoint();

            long endpointType = endpoint.EndpointSystemTypeId.HasValue ? endpoint.EndpointSystemTypeId.Value : 3;

            var logger = new DiscoveryLogger(0, endpoint.Id, (EndpointType)endpointType, endpoint.DisplayName);

            logger.WriteLog(LogLevelType.Info, $"EndPointServiceController: Started DiscoverSpecificUserFromHR-[{pernr}]");

            var connector = ConnectorFactory.GetConnector(endpoint, null, logger);
            connector.Process = Process.GetByProcessTypeKeyId(MasterConfigHelper.GetDailyHRMSProcessId, ProcessType.Discovery);
            connector.DiscoveryProcess = DiscoveryProcess.GetById(connector.Process.ProcessTypeKeyId);
            connector.DiscoverSpecificUserFromHR(pernr);


            //var ep = DI.Chakra.DomainModel.EndPoint.GetByApplicationType(ApplicationType.SuccessFactors,true);
            //SuccessFactorConnector sf = new SuccessFactorConnector(ep, new DiscoveryLogger(1, 1,EndpointType.SuccessFactor,ep.DisplayName));// ConnectorFactory.GetConnector(ep, null);
            //sf.Test();
        }

        private void button90_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox11.Text))
            {
                var adsuser = ADSUserInfo.GetBysAMAccountName(textBox11.Text.Trim());
                var user = User.GetByUserName(textBox11.Text.Trim());

                var endpoint = DI.Chakra.DomainModel.EndPoint.GetEndPointByApplicationType(ApplicationType.ADSingaporePowerLocal);
              

                ConnectorBase _connector = ConnectorFactory.GetConnector(endpoint, null);

                try
                {                   

                    if (!string.IsNullOrEmpty(user.ROEmployeeId))
                    {
                        var ro = User.GetByEmployeeId(user.ROEmployeeId);

                        if (ro != null)
                        {
                            var roAD = ADSUserInfo.GetPrimaryByDIUserId(ro.Id, endpoint.Id);

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

                        if (user.EEGroup == "0")
                        {
                            adsuser.extensionAttribute13 = "VP&Above";
                        }
                        else
                        {
                            adsuser.extensionAttribute13 = "Director&Below";
                        }
                    }
                    else
                    {
                        adsuser.otherFascimileTelephoneNumber = "Non-Employees";
                    }

                    

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
                    DILog.ProvisioningLogger.WriteLog(null, endpoint.Id, LogLevelType.Exception, string.Format("Error while updating AD attributes for users - {0}", user.UserName), ex);
                }

            }
        }

        private void button91_Click(object sender, EventArgs e)
        {
            NovaExcelUploadForm f = new NovaExcelUploadForm();
            f.ShowDialog();

            //ExcelConnect connect = new ExcelConnect();
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Multiselect = false;
            //openFileDialog.DefaultExt = "xls*";
            //var d = openFileDialog.ShowDialog();
            //string s = openFileDialog.FileName;
            //var ds = connect.ConvertExcelToDataSet(s);
        }

        private void btnEmailTemplateTest_Click(object sender, EventArgs e)
        {
            var datetime = getDateTime("20070904");
            frmEmailTemplateTest frm = new frmEmailTemplateTest();
            frm.Show();
        }

        public DateTime? getDateTime(string strDate)
        {

            try
            {
                return DateTime.ParseExact(strDate, "yyyyMMdd", CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }

        }

        private void button92_Click(object sender, EventArgs e)
        {
            SAP_AccountCreation utilityForm = new SAP_AccountCreation();
            utilityForm.Show();
        }

        private void button93_Click(object sender, EventArgs e)
        {
            HRMS_Data hRMS_Data = new HRMS_Data();
            hRMS_Data.ShowDialog();

        }

        private void button94_Click(object sender, EventArgs e)
        {
            OtherSystemDiscoveryProcessJob.DiscoverOtherSystem();
            //AttestationReportJob attestationReportJob = new AttestationReportJob();
            //attestationReportJob.GenerateAttestationReport();
            //attestationReportJob.GenerateAttestationParentCampaignReport();
            //QueryStringEncyptorModule.Decrypt(null);
            //PresentationSystemDecommission.StartDecommisson();
            //var ep = DI.Chakra.DomainModel.EndPoint.GetByApplicationType (ApplicationType.MicrosoftTeams);
            //var connect1 = ConnectorFactory.GetConnector(ep, null, null) as SkypeForBusinessConnector;
            //if (connect1.Connect())
            //{
            //        connect1.IsUserPropertyExistsInOnPrem("", "", "");

            //        var data = connect1.GetSkypeForBusinessObjectByType();
            //    if (connect1.GetCSUser("KANCHANA@spgroup.com.sg", true, true))
            //    {

            //    }
            //}
            //var currentDate = DateTime.Now;
            //var Id = $"{currentDate.Month}{currentDate.Day.ToString().PadLeft(2, '0')}{currentDate.Year.ToString().GetLastDigit(2)}{currentDate.Hour.ToString().PadLeft(2, '0')}{currentDate.Minute.ToString().PadLeft(2, '0')}";
            //Int32 s = Int32.Parse(Id);
            //currentDate = DateTime.Now;
            //Id = $"{currentDate.Month}{currentDate.Day.ToString().PadLeft(2, '0')}{currentDate.Year.ToString().GetLastDigit(2)}{currentDate.Hour.ToString().PadLeft(2, '0')}{currentDate.Minute.ToString().PadLeft(2, '0')}";
            //s = Int32.Parse(Id);
            //currentDate = DateTime.Now;

            //Id = $"{currentDate.Month}{currentDate.Day.ToString().PadLeft(2, '0')}{currentDate.Year.ToString().GetLastDigit(2)}{currentDate.Hour.ToString().PadLeft(2, '0')}{currentDate.Minute.ToString().PadLeft(2, '0')}";
            //s = Int32.Parse(Id);
            //currentDate = DateTime.Now;
            //Id = $"{currentDate.Month}{currentDate.Day.ToString().PadLeft(2, '0')}{currentDate.Year.ToString().GetLastDigit(2)}{currentDate.Hour.ToString().PadLeft(2, '0')}{currentDate.Minute.ToString().PadLeft(2, '0')}";
            //s = Int32.Parse("1231222359");
            //s = Int32.Parse("1201222359");
            //EntitlementReconcilation.GetActiveAccountsForSODConflictingReport(10);
            //EntitlementReconcilation.GetActiveAccountsForSODConflictingReport(11);
            //EntitlementReconcilation.GetActiveAccountsForSODConflictingReport(9);
            //EntitlementReconcilation.GetActiveAccountsForSODConflictingReport(8);
            ////var usr = SkypeForBusinessUserInfo.GetValidActiveUser(25);
            ////var ep = DI.Chakra.DomainModel.EndPoint.GetByApplicationType(ApplicationType.MicrosoftTeams);
            ////var connect = ConnectorFactory.GetConnector(ep, null, null) as SkypeForBusinessConnector;
            ////connect.UpdateHRMSandUserProfileForTelephoneNumber();

            //var ep = DI.Chakra.DomainModel.EndPoint.GetTrustedSource();// (ApplicationType.SuccessFactors);
            //var connect1 = ConnectorFactory.GetConnector(ep, null, null) as SuccessFactorConnector;
            //connect1.DiscoverSpecificUserFromHR("10006497");
            //RequestExecutor s = new RequestExecutor();
            //s.AddEmailIdAndTelephoneNumberInHR();
            //var t = PresentationTask.GetPagedResult(null, null, 0, 50, null, null);

            //SAPTCodeInfo.GenerateRoleTCodeForReport(5);
            //var endpointPolicy = PasswordPolicyEndpoint.GetEndpointPasswordPolicy(16);

            //var policy = endpointPolicy.PasswordPolicy;

            //string password = new PasswordResolver().ConstructGeneralUserPassword
            //    (
            //        policy.PasswordLength.HasValue ? policy.PasswordLength.Value : 8,
            //        policy.NoOfSpecialCharacters.HasValue ? policy.NoOfSpecialCharacters.Value : 0,
            //        policy.NoOfLowerCaseAlphabets.HasValue ? policy.NoOfLowerCaseAlphabets.Value : 0,
            //        policy.NoOfUpperCaseAlphabets.HasValue ? policy.NoOfUpperCaseAlphabets.Value : 0,
            //        policy.NoOfNumericCharacters.HasValue ? policy.NoOfNumericCharacters.Value : 0
            //    );

            //var us = User.GetReportingOfficerByUser(0);
            //new AttestationReportJob().GenerateAttestationParentCampaignReport();
            //PresentationAttestationSnapshotActionsForTask.GetAttestationSnapshotByTaskIdFromHistory(73053, 83539, false);
            //PresentationExportAttestationEntitlementReviewDetail.GetAttestationTaskReviewForExportFlat(73053, true, "admin");
            //PresentationAttestationSnapshotActionsForTask.GetAttestationSnapshotByTaskId(74092, 84754);
            //DI.Chakra.Workflow.Processor.SaveAttestationSnapshot v = new SaveAttestationSnapshot();
            //var tsk = Task.GetById(64684, true);
            //var cam = AttestationCampaign.GetById(66101);
            //var dataArray = v.ExportPDFAsByteArray(tsk.Id, cam);

            //using (FileStream fileStream = new FileStream($@"C:\\temp\{DateTime.Now.ToString("ddmmyyyyhhmmss")}.pdf", FileMode.Create))
            //{
            //    // Write the data to the file, byte by byte.
            //    for (int i = 0; i < dataArray.Length; i++)
            //    {
            //        fileStream.WriteByte(dataArray[i]);
            //    }

            //    // Set the stream position to the beginning of the file.
            //    fileStream.Seek(0, SeekOrigin.Begin);

            //    // Read and verify the data.
            //    for (int i = 0; i < fileStream.Length; i++)
            //    {
            //        if (dataArray[i] != fileStream.ReadByte())
            //        {
            //            Console.WriteLine("Error writing data.");
            //            return;
            //        }
            //    }
            //    Console.WriteLine("The data was written to {0} " +
            //        "and verified.", fileStream.Name);
            //}
            //DI.Chakra.Service.MiscProcessor.ReminderJob srv = new DI.Chakra.Service.MiscProcessor.ReminderJob();
            //srv.SendTaskReminder();
            //var emails = new []{ "TANYANHENG@SPGROUP.COM.SG", "San.Emirza@chinasofti.com", "rofinowkw1@spgroup.com.sg", "Rupali.Rathore@capgemini.com" };
            //var domain = MasterConfigHelper.ADUPNDomainName;
            //foreach (var email in emails)
            //{
            //    if (!email.EndsWith(domain, StringComparison.InvariantCultureIgnoreCase))
            //    {
            //        MessageBox.Show($"Not Matched {email}");
            //    }
            //}

            //new VPNTokenUpdate().ShowDialog();
        }

        private void button95_Click(object sender, EventArgs e)
        {
            new Office365Form().Show();
        }

        private void btnSFBOnline_Click(object sender, EventArgs e)
        {
            bool isAccountCreated = false;
            try
            {
                var userInfo = new SkypeForBusinessUserInfo();
                userInfo.ExternalAccessPolicy = "FederationOnly";
                userInfo.ConferencingPolicy = "BPosSAllModalityNoFT";
                var ep = DI.Chakra.DomainModel.EndPoint.GetByApplicationType(ApplicationType.MicrosoftTeams);
                var sfbConnector = ConnectorFactory.GetConnector(ep,null,null) as SkypeForBusinessConnector;
                var userIdentity = txtIdentity.Text;

                var setUser = $"Set-CsUser -Identity \"{userIdentity}\"  -EnterpriseVoiceEnabled $true -HostedVoiceMail $true";
                var GrantCsExternalAccessPolicy = $"Grant-CsExternalAccessPolicy -Identity {userIdentity} -PolicyName \"{userInfo.ExternalAccessPolicy}\"";
                var GrantCsConferencingPolicy = $"Grant-CsConferencingPolicy -identity {userIdentity} -policyname \"{userInfo.ConferencingPolicy}\"";
                
                var onLineExecutionscripts = new List<SkypeForBusinessScriptBlock>();
                onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = setUser });
                onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = GrantCsExternalAccessPolicy });
                onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = GrantCsConferencingPolicy });
                MessageBox.Show(string.Join(Environment.NewLine, onLineExecutionscripts.Select(i=>i.Script)));
                isAccountCreated = sfbConnector.ExecuteOnLineLineScript(onLineExecutionscripts);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message} isAccountCreated:{isAccountCreated}");
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button96_Click(object sender, EventArgs e)
        {
            try
            {
                ReminderJob workflowReminder = new ReminderJob();
                workflowReminder.SendTaskReminder();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button97_Click(object sender, EventArgs e)
        {
            try
            {
                OtherSystemDiscoveryProcessJob processJob = new OtherSystemDiscoveryProcessJob();
                OtherSystemDiscoveryProcessJob.DiscoverOtherSystem();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
