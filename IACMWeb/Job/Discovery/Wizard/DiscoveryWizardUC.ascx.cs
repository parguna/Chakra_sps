using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Web.Job.Discovery.Wizard
{
    public partial class DiscoveryWizardUC : UIBaseUC
    {
        #region Class Member Variables
        private Dto.DiscoveryProcess _Process = null;
        private Dto.Scheduler _Schedule = null;
        private const string V_ISEDIT = "ISEDIT";
        private const string V_ISVIEW = "ISVIEW";
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
                DiscoveryInfoUC.DisplayMode();
                SchedulerUC.DisplayMode();
            }
            DiscoveryInfoUC.InitilizePage();
            SchedulerUC.InitilizePage();

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            // this.MasterPage.ActionClicked += ActionClicked;

            if (!IsPostBack)
            {

                //if (IsEdit || ViewOnly)
                //{
                //    this.EntityId = this.ContextObjectId.Value;
                //    SetPage();
                //}

                //if (ViewOnly)
                //    ViewOnlyMode();
            }
        }

        #endregion

        #region Public Properties
        public Dto.DiscoveryProcess Process
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


        #region Telerik Events
        protected void DiscoveryRadWizard_FinishButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {
            try
            {
                var usr = new SessionStatusImpl().User;
                if (!IsView)
                {
                    SessionStatusImpl session = new SessionStatusImpl();
                    Dto.Scheduler schedluer = SchedulerUC.getScheduler();
                    Dto.DiscoveryProcess disovery = DiscoveryInfoUC.getDiscoveryProcess();
                    if (disovery.Endpointsystemid == 0)
                    {
                        MasterPage.ShowAlert(AlertType.Error, "In Step 1: Please select Endpoint");
                        return;
                    }

                    if (schedluer == null)
                    {
                        MasterPage.ShowAlert(AlertType.Error, "In Step 2: Please select Scheduler");
                        return;
                    }

                    long discoveryProcessId = -1;
                    if (IsEdit)
                    {
                        discoveryProcessId = Convert.ToInt64(DiscoveryInfoUC.EntityId);
                        Dto.Process process = new Dto.Process();

                        if (discoveryProcessId > 0)
                        {
                            Dictionary<string, string> parameterprocess = new Dictionary<string, string>();
                            parameterprocess.Add("ProcessId", Convert.ToString(discoveryProcessId));
                            parameterprocess.Add("processType", Convert.ToString(ProcessType.Discovery.ToString()));

                            process = DomainApiClient<Dto.Process>.GetByAction<Dto.Process>
                                (DomainApiAction.Schedulers.Process.GetByProcessType, parameterprocess);

                            if (process != null)
                            {
                                schedluer.Id = process.Scheduler.Id;
                                if (process.Scheduler.StartDate != schedluer.StartDate || process.Scheduler.StartTime != schedluer.StartTime
                                    || process.Scheduler.Occurence.Value != schedluer.Occurence.Value)
                                {
                                    process.Status = "Pending";

                                    if (schedluer.StartDate >= DateTime.Now.Date)
                                    {
                                        process.ProcessStartAt = null;
                                        process.ProcessEndAt = null;
                                        if (!disovery.Endpoint.IsTrustedSource)
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
                                process.DiscoveryInstance = disovery.DiscoveryInstance;
                                process.Scheduler = schedluer;
                                process.ErrorMessage = "";
                            }
                        }

                        disovery.process = process;
                        disovery.Id = discoveryProcessId;

                        
                        DomainApiClient<Dto.DiscoveryProcess>.Save(disovery);
                        DI.Chakra.Logger.DILog.UILogger.WriteLog(usr.Id, usr.DisplayName, Logger.LogLevelType.Info, $"Discovery Job Edited:: Job Name:{disovery.ProcessName};Scheduler Occurence:{disovery.process.Scheduler.Occurence.ToString()}", null);
                        DI.Chakra.Logger.DILog.UILogger.WriteLog(usr.Id, usr.DisplayName, Logger.LogLevelType.Info, $"Discovery Job Edited:: Job Name:{disovery.ProcessName};Scheduler StartDate:{process.Scheduler.StartDate}", null);
                        DI.Chakra.Logger.DILog.UILogger.WriteLog(usr.Id, usr.DisplayName, Logger.LogLevelType.Info, $"Discovery Job Edited:: Job Name:{disovery.ProcessName};Scheduler StartTime:{process.Scheduler.StartTime}", null);

                        MasterPage.ShowAlert(AlertType.Success, "Discovery Job updated successfully");
                    }
                    else
                    {

                        Dictionary<string, string> parameter = new Dictionary<string, string>();
                        parameter.Add("processName", disovery.ProcessName);

                        bool isExists = DomainApiClient<Dto.DiscoveryProcess>.GetByAction<bool>
                            (DomainApiAction.Discovery.DiscoveryProcess.DoesProcessNameAlreadyExists, parameter);

                        if (!isExists)
                        {

                            Dto.Process process = new Dto.Process();
                            process.Scheduler = schedluer;
                            process.Status = "Pending";
                            process.ProcessType = ProcessType.Discovery;
                            process.DiscoveryInstance = disovery.DiscoveryInstance;
                            disovery.process = process;

                            DomainApiClient<Dto.DiscoveryProcess>.Save(disovery);
                            DI.Chakra.Logger.DILog.UILogger.WriteLog(usr.Id, usr.DisplayName, Logger.LogLevelType.Info, $"Discovery Job Added:: Job Name:{disovery.ProcessName};Scheduler Occurence:{disovery.process.Scheduler.Occurence.ToString()}", null);
                            DI.Chakra.Logger.DILog.UILogger.WriteLog(usr.Id, usr.DisplayName, Logger.LogLevelType.Info, $"Discovery Job Added:: Job Name:{disovery.ProcessName};Scheduler StartDate:{process.Scheduler.StartDate}", null);
                            DI.Chakra.Logger.DILog.UILogger.WriteLog(usr.Id, usr.DisplayName, Logger.LogLevelType.Info, $"Discovery Job Added:: Job Name:{disovery.ProcessName};Scheduler StartTime:{process.Scheduler.StartTime}", null);

                            MasterPage.ShowAlert(AlertType.Success, "Discovery Job saved successfully");
                        }
                        else
                        {
                            MasterPage.ShowAlert(AlertType.Error, "Discovery Name already exists");
                            return;
                        }
                    }
                }
                Response.Redirect("~//Job/Discovery//List.aspx");
            }
            catch (Exception ex)
            {
                // this.ShowAlertMessage(AlertType.Error, ex.Message);
                // Need to implement ShowAlert message here.
            }
        }
        #endregion
        private void SetPage(bool isEditMode, bool isViewMode)
        {
            long processId = Convert.ToInt64(this.ContextObjectId);
            DiscoveryInfoUC.IsEdit = isEditMode;
            DiscoveryInfoUC.IsView = isViewMode;
            DiscoveryInfoUC.DiscoveryProcessConfig = GetDiscoveryProcess(processId);
            DiscoveryInfoUC.EntityId = processId;
            SchedulerUC.IsEdit = isEditMode;
            SchedulerUC.SchedulerConfig = GetScheduler(processId);
        }
        private Dto.DiscoveryProcess GetDiscoveryProcess(long ProcessId)
        {
            try
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", Convert.ToString(ProcessId));
                return DomainApiClient<Dto.DiscoveryProcess>.GetByAction<Dto.DiscoveryProcess>
                (DomainApiAction.Discovery.DiscoveryProcess.GetByIdIncludeEndpoint, parameter);

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
                parameter.Add("discoveryProcessId", Convert.ToString(ProcessId));
                return DomainApiClient<Dto.Scheduler>.GetByAction<Dto.Scheduler>
                    (DomainApiAction.Schedulers.Scheduler.GetDiscoveryScheduler, parameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        protected void DiscoveryRadWizard_CancelButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {

            Response.Redirect("~/Job/Discovery/List.aspx");
        }
    }
}