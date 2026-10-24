using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using Telerik.Web.UI;

namespace DI.Chakra.Web.WorkFlow
{
    public partial class List : DI.Chakra.Web.UIBasePage, IContextObjectEnabled
    {
        private static List<Dto.Presentation.PresentationWorkflow> wflist;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
        }

        protected void WorkflowConfigurationGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            try
            {
                FillDataSource();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void FillDataSource()
        {
            wflist = DomainApiClient<Dto.Presentation.PresentationWorkflow>.GetByAction<List<Dto.Presentation.PresentationWorkflow>>
                   (DomainApiAction.Workflow.Presentation.PresentationWorkflow.GetWorkflowList, null);
            WorkflowConfigurationGrid.DataSource = wflist;
        }

        protected void WorkflowConfigurationGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = WorkflowConfigurationGrid.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("WorkflowConfigurationId");
        }

        private void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Workflow - Delete");
                DeleteWorkflow();
            }
            else if (e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.Excel)
            {
                WorkflowConfigurationGrid.ExportSettings.FileName = "Workflow";
                Export(WorkflowConfigurationGrid, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                WorkflowConfigurationGrid.Export<List<Dto.Presentation.PresentationWorkflow>>(wflist, e.ActionType, "Workflow");
            }
        }

        private void DeleteWorkflow()
        {
            if (!ContextObjectId.HasValue)
                return;

            Dictionary<String, string> aparameter = new Dictionary<String, string>();
            aparameter.Add("workflowid", this.ContextObjectId.Value.ToString());
            bool isExists = DomainApiClient<Dto.Attestation>.GetByAction<bool>
                (DomainApiAction.Attestation.IsLinkedWithAtestation, aparameter);
            if (isExists)
            {
                MasterPage.ShowAlert(AlertType.Warning, "Workflow is linked with Attestion, Cann't make inactive. ");
                return;
            }

            bool isExistsPendingTask = DomainApiClient<Dto.Task>.GetByAction<bool>
              (DomainApiAction.Tasks.Task.DoesExistsByWorkflowConfigurationId, aparameter);
            if (isExistsPendingTask)
            {
                MasterPage.ShowAlert(AlertType.Warning, "Workflow is linked with Task, Cann't delete. Make as inactive.");
                return;
            }

            Dto.WorkflowConfiguration wfDetails = DomainApiClient<Dto.WorkflowConfiguration>.GetById(this.ContextObjectId.Value,true);
            if (wfDetails.WorkflowConfigurationDetails != null)
            {
                foreach (Dto.WorkflowConfigurationDetail item in wfDetails.WorkflowConfigurationDetails)
                { 
                     DomainApiClient<Dto.WorkflowConfigurationDetail>.DeleteById(item.Id);                  
                }
            }

            DomainApiClient<Dto.WorkflowConfiguration>.DeleteById(this.ContextObjectId.Value);
            MasterPage.ShowAlert(AlertType.Success, "Workflow deleted successfully");
            WorkflowConfigurationGrid.Rebind();
            this.ContextObjectId = null;
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }

        protected void IsActive_PreRender(object sender, EventArgs e)
        {
            RadComboBox rcbFilter = sender as RadComboBox;
            if (ViewState[rcbFilter.ID] != null)
            {
                rcbFilter.SelectedValue = ViewState[rcbFilter.ID].ToString();
            }
        }
        protected void IsActive_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            RadComboBox combo = sender as RadComboBox;
            ViewState[combo.ID] = combo.SelectedValue;
            if (combo.SelectedValue != "0")
            {
                string ColumnName = combo.ID;
                string filterExpression = "([" + ColumnName + "] = '" + combo.SelectedValue + "')";
                WorkflowConfigurationGrid.MasterTableView.FilterExpression = filterExpression;
            }
            else
            {
                WorkflowConfigurationGrid.MasterTableView.FilterExpression = string.Empty;
            }
            FillDataSource();
            WorkflowConfigurationGrid.DataBind();


        }


    }
}