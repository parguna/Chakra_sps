using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using Telerik.Web.UI;
using System.Data;
using System.Text;
using DI.Chakra.Logger;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Services.EmailingProcessor;


namespace DI.Chakra.Web.Delegation.UC
{
    public partial class ListUC : UIBaseUC
    {
        SessionStatusImpl sessionStatus = new SessionStatusImpl();
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;

            if (!IsPostBack)
            {
                Dto.User DelegateFrom = DomainApiClient<Dto.User>.GetById(this.ContextObjectId.Value);
                DelegateFrom.DisplayName = DelegateFrom.DisplayName + " [" + DelegateFrom.UserName + "]";
                delegateFrom.SelectedUser = DelegateFrom;

                if (sessionStatus.isAdminGroup || sessionStatus.isIACMAdminGroup)
                {
                    delegateFrom.DisableMyAutocompleteBox = true;
                }
                else
                {
                    delegateFrom.DisableMyAutocompleteBox = false;
                }
                Session["RedirectFrom"] = "CreateDelegation";
                grdTask.DataBind();
            }
        }

        public string Title
        {
            get
            {
                return gridTitle.InnerText;
            }
            set
            {
                gridTitle.InnerText = value;
            }
        }

        protected void grdTask_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            int userId = Convert.ToInt32(sessionStatus.UserId);

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("actionUserId", userId.ToString());
            parameter.Add("taskStatus", StatusType.Pending.ToString());
            var taskDetailsList =DomainApiClient<Dto.Presentation.PresentationTask>.
                GetByAction<IList<Dto.Presentation.PresentationTask>>(
                DomainApiAction.Tasks.Presentation.PresentationTask.GetTaskDetailsByTaskStatus, parameter);

            IList<Dto.Presentation.PresentationTask> updatedDatasource = new List<Dto.Presentation.PresentationTask>();
            foreach (Dto.Presentation.PresentationTask items in taskDetailsList)
            {
                if ((items.IsGroup && items.AssignedTo == userId) || (items.IsGroup == false && items.AssignedTo == null))
                {
                    updatedDatasource.Add(items);
                }
            }
            if (updatedDatasource != null && updatedDatasource.Count > 0)
            {
                grdTask.DataSource = updatedDatasource.OrderByDescending(c => c.CreatedDate);
            }
            else
            {
                grdTask.DataSource = updatedDatasource;
            }
        }

        protected void grdTask_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName.Equals("1") || e.CommandName.Equals("2"))
            {

                if (Convert.ToByte(e.CommandName) == (byte)RequestType.Attestation)
                {
                    Response.Redirect("~/Attestation/AttestationReviewPage.aspx?Id=" + e.CommandArgument);
                }
                else if (Convert.ToByte(e.CommandName) == (byte)RequestType.Provisioning)
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("id", Convert.ToString(e.CommandArgument));

                    Dto.Task SelectedTask = DomainApiClient<Dto.Task>.GetByAction<Dto.Task>
                        (DomainApiAction.Tasks.Task.GetProvisioningTask,parameter);


                    if (SelectedTask.AccessRequestId.HasValue)
                    {
                        Response.Redirect("~/AccessRequest/TaskDetail.aspx?Id=" + e.CommandArgument);
                    }
                }
            }
        }
        protected void ToggleSelectedState(object sender, EventArgs e)
        {
            CheckBox headerCheckBox = (sender as CheckBox);
            foreach (GridDataItem dataItem in grdTask.MasterTableView.Items)
            {
                (dataItem.FindControl("chbxSelect") as CheckBox).Checked = headerCheckBox.Checked;
                dataItem.Selected = headerCheckBox.Checked;
            }
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (Page.IsValid)
            {
                Dto.TaskDelegation delegation = this.GetDelegationInfoFromUI();
                CheckBox checkbox;
                HiddenField hdnTaskId;
                long selectedTaskId = -1;
                List<long> selectedTaskIdList = new List<long>();
   
                foreach (GridDataItem row in grdTask.Items)
                {
                    checkbox = (CheckBox)row.FindControl("chbxSelect"); ;
                    if (checkbox != null && checkbox.Checked)
                    {
                        hdnTaskId = (HiddenField)row.FindControl("hdnTaskId");
                        if (hdnTaskId != null)
                        {
                            long.TryParse(hdnTaskId.Value, out selectedTaskId);
                            if (selectedTaskId > 0)
                                selectedTaskIdList.Add(selectedTaskId);
                        }
                    }
                }

                Dto.TaskDelegationWrapper wrapper = new Dto.TaskDelegationWrapper();
                wrapper.Delegation = delegation;
                wrapper.SelectedtaskList = selectedTaskIdList;
                DomainApiClient<Dto.TaskDelegation>.GetByAction<Dto.TaskDelegation>
                    (DomainApiAction.Tasks.TaskDelegation.CreateSelectedTasksDelegation, null, wrapper);

                foreach (int delegatedTaskId in selectedTaskIdList)
                {
                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    parameters.Add("id", Convert.ToString(delegatedTaskId));

                    Dto.Task delegatedTask = DomainApiClient<Dto.Task>.GetByAction<Dto.Task>
                        (DomainApiAction.Tasks.Task.GetProvisioningTask, parameters);

                    if (delegatedTask.AccessRequestId.HasValue)
                    {
                        Dictionary<string, string> parameteraccessRequest = new Dictionary<string, string>();
                        parameteraccessRequest.Add("id",Convert.ToString(delegatedTask.AccessRequestId.Value));

                        Dto.AccessRequest accessRequest = DomainApiClient<Dto.AccessRequest>.
                            GetByAction<Dto.AccessRequest>
                            (DomainApiAction.Provisioning.AccessRequest.GetByIdIncludeAdHocAccessRequest, parameteraccessRequest);

                      // SendEmailForAdhocProvisioning(delegatedTask, delegatedTask.AccessRequest.AdHocAccessRequest, accessRequest);
                        //SendEmailForAdhocProvisioning((int)EmailTemplate.NotifyAdhocProvisioningApproval, delegatedTask.Id,delegatedTask,delegatedTask.AccessRequest.AdHocAccessRequest,accessRequest);
                       // new EmailComposer().Compose(EmailTemplate.NotifyAdhocProvisioningApproval, delegatedTask.Id);

                        Dictionary<string, string> param = new Dictionary<string, string>();
                        param.Add("delegateTaskId", delegatedTask.Id.ToString());
                        DomainApiClient<Dto.AccessRequest>.GetByAction<bool>(DomainApiAction.Provisioning.AccessRequest.EmailCompose, param);

                    }
                    else if (delegatedTask.AttestationId.HasValue)
                    {
                        Dictionary<string, string> parameterAttestation = new Dictionary<string, string>();
                        parameterAttestation.Add("id",Convert.ToString(delegatedTask.AttestationId.Value));

                        Dto.Attestation attestation = DomainApiClient<Dto.Attestation>.GetByAction<Dto.Attestation>
                        (DomainApiAction.Attestation.GetByIdIncludingAttestationProcesses, parameterAttestation);

                        SendEmailForAttestation(delegatedTask, attestation.WorkflowConfiguration, attestation);
                    }
                }
                Response.Redirect("~/Delegation/List.aspx");
            }
        }

        private bool SendEmailForAttestation(Dto.Task currentTask, Dto.WorkflowConfiguration currentWorkflowConfiguration, 
            Dto.Attestation attestation)
        {
            try
            {

                Dto.EmailInfo emailInfo = new Dto.EmailInfo();

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("configKey", "NotifyAdhocProvisioningApproval");

                Dto.MailTemplate template = DomainApiClient<Dto.MailTemplate>.GetByAction<Dto.MailTemplate>
                (DomainApiAction.Email.MailTemplate.GetByTemplateName, parameter);

                var user = DomainApiClient<Dto.User>.GetById((long)currentTask.ActionUserId);

                if (string.IsNullOrEmpty(user.EmailId))
                {
                    DILog.AttestationLogger.WriteLog(attestation.Id, LogLevelType.Info, 
                        string.Format("Notification email is not sent to {0} since he has no valid email id", user.UserName));
                    return false;
                }

                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@RECEIVER@@", user.DisplayName);
                body.Replace("@@ATTESTATION_NAME@@", attestation.AttestationName);
                body.Replace("@@ATTESTATION_DESC@@", attestation.AttestationDescription);
                body.Replace("@@APPROVAL_LEVEL@@", currentTask.WorkflowStateId.ToString());
                body.Replace("@@REVIEWLASTDATE@@", currentTask.CreatedDate.AddDays
                    (currentWorkflowConfiguration.WorkflowConfigurationDetails[0].ResponseTimeInDays).ToShortDateString());
                body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);

                emailInfo.Body = body.ToString();
                emailInfo.Subject = string.Format(template.MailSubject, currentTask.WorkflowStateId.ToString());
                emailInfo.To = user.EmailId;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Attestation.ToString();
                DomainApiClient<Dto.EmailInfo>.Save(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(null,LogLevelType.Exception, ex.Message, ex);
                return false;
            }
        }

        private Dto.TaskDelegation GetDelegationInfoFromUI()
        {
            Dto.TaskDelegation delegation = new Dto.TaskDelegation();

            delegation.ActionUser = Convert.ToInt64(this.delegateFrom.SelectedUser.Id);
            delegation.DelegatedTo = Convert.ToInt64(this.delegateTo.SelectedUser.Id);
            delegation.DelegationComments = this.Comment.Text.Trim();
            delegation.IsActive = this.IsActiveCheckBox.Checked;
            return delegation;
        }



        private bool SendEmailForAdhocProvisioning(int templateId,object objectId, Dto.Task currentTask, Dto.AdHocAccessRequest adHocAccessRequest,
           Dto.AccessRequest accessRequest)
        {
            try
            {
                Dto.EmailInfo emailInfo = new Dto.EmailInfo();
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("templateId", templateId.ToString());
                parameter.Add("objectId", objectId.ToString());

                StringBuilder[] subjectContent = DomainApiClient<Dto.MailTemplate>.GetByAction<StringBuilder[]>
        (DomainApiAction.Email.MailTemplate.GetReplacedTokensByTemplateId, parameter);

                var user = DomainApiClient<Dto.User>.GetById((long)currentTask.ActionUserId);        

                if (string.IsNullOrEmpty(user.EmailId))
                {
                    DILog.ProvisioningLogger.WriteLog(null, null,  LogLevelType.Info,
                        string.Format("Notification email is not sent to {0} since he has no valid email id", user.UserName));
                    return false;
                }   
               
                emailInfo.Body = subjectContent[1].ToString();
                emailInfo.Subject = subjectContent[0].ToString(); //string.Format(template.MailSubject, appName);
                emailInfo.To = user.EmailId;

                Dto.MailTemplate template = DomainApiClient<Dto.MailTemplate>.GetByAction<Dto.MailTemplate>
                  (DomainApiAction.Email.MailTemplate.GetByTemplateId, parameter);

                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();

                DomainApiClient<Dto.EmailInfo>.Save(emailInfo);
                return true;


               
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(null, LogLevelType.Exception, ex.Message, ex);

                return false;
            }
        }

        private bool SendEmailForAdhocProvisioning(Dto.Task currentTask, Dto.AdHocAccessRequest adHocAccessRequest, 
            Dto.AccessRequest accessRequest)
        {
            try
            {
                Dto.EmailInfo emailInfo = new Dto.EmailInfo();
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("configKey", "NotifyAdhocProvisioningApproval");

                Dto.MailTemplate template = DomainApiClient<Dto.MailTemplate>.GetByAction<Dto.MailTemplate> 
                (DomainApiAction.Email.MailTemplate.GetByTemplateName, parameter);
       
                var user = DomainApiClient<Dto.User>.GetById((long)currentTask.ActionUserId);
                var requester = DomainApiClient<Dto.User>.GetById((long)accessRequest.RequesterId);
                var RequesteeUser = DomainApiClient<Dto.User>.GetById((long)adHocAccessRequest.RequesteeUserId);
                string accounttype = string.Format("{0}", UtilityMethods.GetEnumAttribute<Description>(adHocAccessRequest.AccountType));

                if (string.IsNullOrEmpty(user.EmailId))
                {
                    DILog.ProvisioningLogger.WriteLog(null, null,  LogLevelType.Info, 
                        string.Format("Notification email is not sent to {0} since he has no valid email id", user.UserName));
                    return false;
                }

                StringBuilder subject = new StringBuilder(template.MailSubject);
                subject.Replace("@@applicationName@@", adHocAccessRequest.EndpointSystemType.ToString());
                subject.Replace("@@requestType@@", UtilityMethods.GetEnumAttribute<Description>(adHocAccessRequest.AdhocActionType));
                subject.Replace("@@requestNum@@", accessRequest.Id.ToString());

                subject.Replace("@@userDisplayName@@", RequesteeUser.DisplayName);
                subject.Replace("@@pernrNum@@", RequesteeUser.EmployeeId);

                subject.Replace("@@RECEIVERID@@", requester.EmployeeId.ToString());

                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@approverName@@", user.DisplayName);
                body.Replace("@@requestNum@@", accessRequest.Id.ToString());
                body.Replace("@@applicationName@@", adHocAccessRequest.EndpointSystemType.ToString());
                body.Replace("@@requestType@@", UtilityMethods.GetEnumAttribute<Description>(adHocAccessRequest.AdhocActionType));
                body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);

                body.Replace("@@userDisplayName@@", RequesteeUser.DisplayName);
                body.Replace("@@pernrNum@@", RequesteeUser.EmployeeId);

                body.Replace("@@AccountType@@", accounttype);

                if (string.IsNullOrEmpty(adHocAccessRequest.Comments))
                    body.Replace("@@COMMENTS@@", string.Empty);
                else
                    body.Replace("@@COMMENTS@@", adHocAccessRequest.Comments?.Replace("\n", "<br>"));

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString(); //string.Format(template.MailSubject, appName);
                emailInfo.To = user.EmailId;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();

                DomainApiClient<Dto.EmailInfo>.Save(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(null,LogLevelType.Exception, ex.Message, ex);

                return false;
            }
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                grdTask.ExportSettings.FileName = "MyDelegations";
                Export(grdTask,e.ActionType);
            }
        }
    }
}