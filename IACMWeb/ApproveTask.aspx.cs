using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Web
{
    public partial class ApproveTask : FrontUIPage
    {
        Dto.User usr;
        long taskId = 0;
        string comments = "";
        string conflictRemarks = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string userName = Request.Form.Get("txtUserName").ToString();

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("employeeId", userName);

                usr = DomainApiClient<Dto.User>.GetByAction<User>(DomainApiAction.User.GetByEmployeeId, parameter);                
                taskId = Convert.ToInt64(Request.Form.Get("txtTaskId").ToString());
                comments = Request.Form.Get("txtComments").ToString();

                if (usr != null && taskId > 0)
                    UpdateAccessRequest();
            }
            catch (Exception ex)
            {
                processing.Visible = false;
                taskStatus.Attributes["class"] = "fail";
                taskStatus.InnerText = "Task Approval Failed. Please login to IDMS system and approve the task.";
            }
        }
        void UpdateAccessRequest()
        {
            try
            {
                //Dictionary<string, string> parameter = new Dictionary<string, string>();
                //parameter.Add("accessType", AccessType.Approve.ToString());
                //parameter.Add("UserName", usr.UserName);
                //parameter.Add("TaskId", taskId.ToString());
                //parameter.Add("Comments", comments.ToString());
                //parameter.Add("AccountCreatedNotificationTO", MasterConfigHelper.AccountCreatedNotificationTO.ToString());
                //parameter.Add("conflictRemarks", conflictRemarks);

                PresentationWorkFlowCriteria wfCriteriaParam = new PresentationWorkFlowCriteria();
                wfCriteriaParam.AccessType = AccessType.Approve;
                wfCriteriaParam.TaskId = taskId;
                wfCriteriaParam.UserName = usr.UserName;
                wfCriteriaParam.Comments = comments;
                wfCriteriaParam.AccountCreatedNotificationTO = MasterConfigHelper.AccountCreatedNotificationTO.ToString();
                wfCriteriaParam.ConflictRemarks = conflictRemarks;

                var criteria = DomainApiClient<Dto.Presentation.PresentationWorkflowCriteriaBase>.GetByAction<Dto.Presentation.PresentationWorkflowCriteriaBase>(DomainApiAction.Workflow.Presentation.PresentationWorkflowCriteriaBase.GetWorkFlowCriteria, null, wfCriteriaParam);

                bool status = DomainApiClient<Object>.GetByAction<bool>("WorkflowController", DomainApiAction.WorkflowProcessor.SubmitWorkflowCriteria, null, criteria);

                if (status)
                {
                    processing.Visible = false;
                    taskStatus.Attributes["class"] = "success";
                    taskStatus.InnerText = "Task Approval Successful.";
                }
                else
                {
                    processing.Visible = false;
                    taskStatus.Attributes["class"] = "fail";
                    taskStatus.InnerText = "Task Approval Failed. Please login to IDMS system and approve the task.";
                }
            }
            catch (Exception ex)
            {
                processing.Visible = false;
                taskStatus.Attributes["class"] = "fail";
                taskStatus.InnerText = "Task Approval Failed. Please login to IDMS system and approve the task.";
            }
        }
    }
}