using System;
using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;

using DI.Chakra.Web.UsrControl.Scheduler;
using Telerik.Web.UI;
using System.Collections.Generic;

namespace DI.Chakra.Web.Job.Attestation.Wizard
{
    public partial class SchedulerWizardUC : UIBaseUC
    {
        #region Variable declaration

        readonly SessionStatusImpl _sessionStatus = new SessionStatusImpl();
        private const string V_ISEDIT = "ISEDIT";
        private const string V_ISVIEW = "ISVIEW";

        #endregion

        #region Properties
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


        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void AttSchRadWizard_OnFinishButtonClick(object sender, WizardEventArgs e)
        {
            SaveAttestationProcess();
        }

        #endregion

        #region Methods

        public void InitializePage(bool isEditMode, bool isViewMode)
        {
            if (isEditMode)
            {
                IsEdit = isEditMode;
                SetPage(isEditMode, isViewMode);
            }
            else if (isViewMode)
            {
                IsView = true;
                JobInfo.DisplayMode();
                SchedulerUC.DisplayMode();
                SetPage(isEditMode, isViewMode);
            }
            JobInfo.InitializePage();
            SchedulerUC.InitilizePage();

        }

        private void SetPage(bool isEditMode, bool isViewMode)
        {
            int attestationProcessId = Convert.ToInt32(this.ContextObjectId);
            JobInfo.IsEdit = isEditMode;
            JobInfo.IsView = isViewMode;
            SchedulerUC.IsEdit = isEditMode;
            JobInfo.AttestationProcessConfig = GetAttestationProcess(attestationProcessId);
            SchedulerUC.SchedulerConfig = GetScheduler(attestationProcessId);
            JobInfo.EntityId = attestationProcessId;

        }

        private Dto.AttestationProcess GetAttestationProcess(int id)
        {
            try
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", Convert.ToString(id));

                return DomainApiClient<Dto.AttestationProcess>.GetByAction<Dto.AttestationProcess>(DomainApiAction.Attestation.AttestationProcess.GetByIdIncludeAttestation, parameter);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Dto.Scheduler GetScheduler(int attestaionProcessId)
        {
            try
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("attestationProcessId", Convert.ToString(attestaionProcessId));

                return DomainApiClient<Dto.Scheduler>.GetByAction<Dto.Scheduler>(DomainApiAction.Schedulers.Scheduler.GetAttestationScheduler, parameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void SchedulerRadWizard_CancelButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {
            Response.Redirect("~/Job/Attestation/List.aspx");
        }

        private void SaveAttestationProcess()
        {
            try
            {
                if (!IsView)
                {
                    SessionStatusImpl session = new SessionStatusImpl();
                    Dto.Scheduler schedluer = SchedulerUC.getScheduler();
                    Dto.AttestationProcess attestationProcess = JobInfo.getAttestationProcess();

                    if (attestationProcess.AttestationId == 0)
                    {
                        MasterPage.ShowAlert(AlertType.Error, "In Step 1: Please select AttestationName");
                        return;
                    }

                    if (schedluer == null)
                    {
                        MasterPage.ShowAlert(AlertType.Error, "In Step 2: Please select Scheduler");
                        return;
                    }

                    long attestationProcessId = -1;
                    if (IsEdit)
                    {
                        if (!JobInfo.IsAttestationInProgress(attestationProcess.ProcessName))
                        {
                            attestationProcessId = Convert.ToInt64(JobInfo.EntityId);
                            Dto.Process process = new Dto.Process();

                            if (attestationProcessId > 0)
                            {
                                Dictionary<string, string> parameterprocess = new Dictionary<string, string>();
                                parameterprocess.Add("ProcessId", Convert.ToString(attestationProcessId));
                                parameterprocess.Add("processType", Convert.ToString(ProcessType.Attestation.ToString()));

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

                            attestationProcess.process = process;
                            attestationProcess.Id = attestationProcessId;

                            DomainApiClient<Dto.AttestationProcess>.Save(attestationProcess);
                            MasterPage.ShowAlert(AlertType.Success, "Attestation Job updated successfully");
                        }
                        else
                        {
                            MasterPage.ShowAlert(AlertType.Warning, "Attestation already In-Progress and cannot be changed!");
                        }
                    }
                    else
                    {

                        Dictionary<string, string> parameter = new Dictionary<string, string>();
                        parameter.Add("processName", attestationProcess.ProcessName);

                        bool isExists = DomainApiClient<Dto.AttestationProcess>.GetByAction<bool>(DomainApiAction.Attestation.AttestationProcess.DoesExists, parameter);

                        if (!isExists)
                        {

                            Dto.Process process = new Dto.Process();
                            process.Scheduler = schedluer;
                            process.Status = "Pending";
                            process.ProcessType = ProcessType.Attestation;

                            attestationProcess.process = process;

                            DomainApiClient<Dto.AttestationProcess>.Save(attestationProcess);
                            MasterPage.ShowAlert(AlertType.Success, "Attestation Job saved successfully");
                        }
                        else
                        {
                            MasterPage.ShowAlert(AlertType.Error, "Attestation Name already exists");
                            return;
                        }
                    }
                }
                Response.Redirect("~//Job/Attestation//List.aspx");
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

    }
}