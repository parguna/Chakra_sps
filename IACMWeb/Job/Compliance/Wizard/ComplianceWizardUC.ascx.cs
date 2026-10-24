using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Web.Job.Compliance.Wizard
{
    public partial class ComplianceWizardUC : UIBaseUC
    {
        #region Class Member Variables
        private Dto.ComplianceProcess _Process = null;
        private Dto.Scheduler _Schedule = null;
        private const string V_ISEDIT = "ISEDIT";
        private const string V_ISVIEW = "ISVIEW";
        #endregion

        #region Public Properties
        public Dto.ComplianceProcess Process
        {
            set { _Process = value; }
            get { return _Process; }
        }
        public Dto.Scheduler Schedule
        {
            set { _Schedule = value; }
            get { return _Schedule; }
        }
        public bool IsEdit
        {
            get
            {
                if (ViewState[V_ISEDIT] == null)
                    return false;
                return (bool)ViewState[V_ISEDIT];
            }
            set
            {
                ViewState[V_ISEDIT] = value;
            }
        }
        public bool IsView
        {
            get
            {
                if (ViewState[V_ISVIEW] == null)
                    return false;
                return (bool)ViewState[V_ISVIEW];
            }
            set
            {
                ViewState[V_ISVIEW] = value;
            }
        }
        #endregion
        #region Public Method
        public void InitilizePage(bool isEditMode, bool isViewMode)
        {
            if (isEditMode)
            {
                SetPage(isEditMode, isViewMode);
                IsEdit = isEditMode;
            }
            else if (isViewMode)
            {
                IsView = true;
                SetPage(isEditMode, isViewMode);
                ComplianceinfoUC.DisplayMode();
                SchedulerUC.DisplayMode();
            }
            ComplianceinfoUC.InitilizePage();
            SchedulerUC.InitilizePage();
        }
        #endregion
        private void SetPage(bool isEditMode, bool isViewMode)
        {
            long processId = Convert.ToInt64(this.ContextObjectId);
            ComplianceinfoUC.IsEdit = isEditMode;
            ComplianceinfoUC.IsView = isViewMode;
            ComplianceinfoUC.ComplianceProcessConfig = GetComplianceProcess(processId);
            SchedulerUC.IsEdit = isEditMode;
            SchedulerUC.SchedulerConfig = GetScheduler(processId);
        }
        private Dto.ComplianceProcess GetComplianceProcess(long ProcessId)
        {
            try
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", Convert.ToString(ProcessId));
                return DomainApiClient<Dto.ComplianceProcess>.GetByAction<Dto.ComplianceProcess>
                    (DomainApiAction.Complaince.ComplianceProcess.GetByIdIncludePolicyExecutionDefinition, parameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Dto.Scheduler GetScheduler(long ProcessId)
        {
            try
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("complianceProcessId", Convert.ToString(ProcessId));
                return DomainApiClient<Dto.Scheduler>.GetByAction<Dto.Scheduler>
                    (DomainApiAction.Schedulers.Scheduler.GetComplianceScheduler, parameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void ComplianceRadWizard_FinishButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {
            try
            {
                if (!IsView)
                {
                    SessionStatusImpl session = new SessionStatusImpl();
                    Dto.Scheduler schedluer = SchedulerUC.getScheduler();
                    Dto.ComplianceProcess compliance = ComplianceinfoUC.getComplianceProcess();
                    if(compliance.PolicyExecutionDefinitionId == -1)
                    {
                        MasterPage.ShowAlert(AlertType.Warning, "Please Select Policy");
                        return;
                    }
                    if(SchedulerUC.SelectedDate == null)
                    {
                        MasterPage.ShowAlert(AlertType.Warning, "Please Select Start date");
                        return;
                    }
                    long complianceProcessId = -1;
                    if (IsEdit)
                    {
                        complianceProcessId = Convert.ToInt64(this.ContextObjectId.Value);
                        Dto.Process process = new Dto.Process();

                        if (complianceProcessId > 0)
                        {
                            Dictionary<string, string> parameterprocess = new Dictionary<string, string>();
                            parameterprocess.Add("ProcessId", Convert.ToString(complianceProcessId));
                            parameterprocess.Add("processType", Convert.ToString(ProcessType.Compliance.ToString()));                     
                            process = DomainApiClient<Dto.Process>.GetByAction<Dto.Process>
                            (DomainApiAction.Schedulers.Process.GetByProcessType, parameterprocess);

                            if (process != null)
                            {
                                schedluer.Id = process.Scheduler.Id;
                                if (process.Scheduler.StartDate != schedluer.StartDate ||
                                process.Scheduler.StartTime != schedluer.StartTime
                                    || process.Scheduler.Occurence.Value != schedluer.Occurence.Value)
                                {
                                    process.Status = "Pending";

                                    if (schedluer.StartDate >= DateTime.Now.Date)
                                    {
                                        process.ProcessStartAt = null;
                                        process.ProcessEndAt = null;
                                        process.LastRunOn = null;
                                    }

                                    if ((schedluer.Occurence == Occurence.RunNow || schedluer.Occurence == Occurence.OneTime) &&
                                        process.Scheduler.Occurence.Value != schedluer.Occurence.Value)
                                    {
                                        process.ProcessStartAt = null;
                                        process.ProcessEndAt = null;
                                        process.LastRunOn = null;
                                        process.Status = "Pending";
                                    }
                                }
                                process.Scheduler = schedluer;
                                process.ErrorMessage = "";
                            }
                        }
                        compliance.process = process;
                        compliance.Id = complianceProcessId;
                        DomainApiClient<Dto.ComplianceProcess>.Save(compliance);
                        MasterPage.ShowAlert(AlertType.Success, "Compliance Job updated successfully");
                    }
                    else
                    {
                        Dictionary<string, string> parameter = new Dictionary<string, string>();
                        parameter.Add("processName", compliance.ProcessName);
                        bool isExists=DomainApiClient<Dto.ComplianceProcess>.GetByAction<bool>
                            (DomainApiAction.Complaince.ComplianceProcess.DoesExistsByProcessName, parameter);

                        if (!isExists)
                        {

                            Dto.Process process = new Dto.Process();
                            process.Scheduler = schedluer;
                            process.Status = "Pending";
                            process.ProcessType = ProcessType.Compliance;
                            compliance.process = process;
                            DomainApiClient<Dto.ComplianceProcess>.Save(compliance);
                            MasterPage.ShowAlert(AlertType.Success, "Compliance Job saved successfully");
                        }
                        else
                        {
                            MasterPage.ShowAlert(AlertType.Error, "Process Name already exists");
                            return;
                        }
                    }                 
                }
                Response.Redirect("~//Job/Compliance//List.aspx");
            }
            catch (Exception ex)
            {
                // this.ShowAlertMessage(AlertType.Error, ex.Message);
                // Need to implement ShowAlert message here.
            }
        }
        protected void ComplianceRadWizard_CancelButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {

            Response.Redirect("~/Job/Compliance/List.aspx");
        }
    }
}