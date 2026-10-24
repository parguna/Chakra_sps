using System;
using Application;
using DI.Chakra.Infrastructure;
using System.Collections.Generic;
using DI.Chakra.Infrastructure.Enums;
using System.Linq;
using DI.Chakra.Logger;

namespace DI.Chakra.Web.AdvanceDelegation.UC
{
    public partial class AdvanceDelegationFormUC : UIBaseUC
    {
        #region Members
        private const string V_ID = "ENTITY_ID";
        private const string V_DELEGATION = "DELEGATION";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        private SessionStatusImpl SessionUser = new SessionStatusImpl();
        #endregion

        #region Properties
        public long EntityId
        {
            get
            {
                if (ViewState[V_ID] == null)
                    return -1;
                return (long)ViewState[V_ID];
            }
            set
            {
                ViewState[V_ID] = value;
            }
        }

        public UIMode UIMode
        {
            get
            {
                if (ViewState[V_DISPLAYMODE] == null)
                    return UIMode.View;

                return (UIMode)ViewState[V_DISPLAYMODE];
            }
            set
            {
                ViewState[V_DISPLAYMODE] = value;
            }
        }

        public Dto.TaskDelegation Delegation
        {
            get
            {
                if (ViewState[V_DELEGATION] == null)
                    return new Dto.TaskDelegation();
                return (Dto.TaskDelegation)ViewState[V_DELEGATION];
            }
            set
            {
                ViewState[V_DELEGATION] = value;
            }
        }
        #endregion

        #region Methods

        public void Initialize()
        {
            if (SessionUser.isAdminGroup || SessionUser.isIACMAdminGroup)
                DelegateFrom.Disabled = false;
            else
            {
                DelegateFrom.SelectedUser = SessionUser.User;
                DelegateFrom.Disabled = true;
            }

            SetPage();
            //if (Provisioning.Checked)
            //{
            //    DelegateTo.DataSourceMethod = "GetUsersWithSameGradeOrAbove";
            //}
            //else
            //{
            DelegateTo.DataSourceMethod = "GetUsers";
            //}
        }

        private void SetPage()
        {

            Delegation = DomainApiClient<Dto.TaskDelegation>.GetById(EntityId);
            if (UIMode == UIMode.Add)
            {
                Delegation = new Dto.TaskDelegation();
                Delegation.DelegateAttestationTask = true;
                Delegation.DelegateProvisioningTask = true;
                Delegation.IsActive = IsActive.Checked;
            }
            Dictionary<String, string> parameter = new Dictionary<String, string>();

            if (UIMode == UIMode.Edit)
            {
                parameter.Add("id", Delegation.ActionUser.ToString());
                this.DelegateFrom.SelectedUser = DomainApiClient<Dto.User>.GetByAction<Dto.User>
                (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, parameter);
                parameter.Clear();
                SaveButton.Visible = Delegation.IsActive;
            }
            parameter.Add("id", Delegation.DelegatedTo.ToString());

            DelegateTo.SelectedUser = DomainApiClient<Dto.User>.GetByAction<Dto.User>
                (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, parameter);
            StartDate.SelectedDate = Delegation.StartDate;
            EndDate.SelectedDate = Delegation.EndDate;

            if (StartDate.SelectedDate == null)
            {
                StartDate.MinDate = DateTime.Now;
                StartDate.SelectedDate = DateTime.Now;
                EndDate.MinDate = (DateTime)StartDate.SelectedDate;
            }
            else
            {
                StartDate.MinDate = (DateTime)StartDate.SelectedDate;
                EndDate.MinDate = (DateTime)StartDate.SelectedDate;
            }
            Attestation.Checked = Delegation.DelegateAttestationTask;
            Provisioning.Checked = Delegation.DelegateProvisioningTask;
            Comment.Text = Delegation.DelegationComments;
            IsActive.Checked = Delegation.IsActive;
        }

        private void Save()
        {
            
            var existingDelegationData = new Dto.TaskDelegation();
            if (Delegation.Id != 0 && UIMode == UIMode.Edit)
            {
                existingDelegationData = DomainApiClient<Dto.TaskDelegation>.GetById(Delegation.Id);
            }

            Delegation.ActionUser = DelegateFrom.SelectedUser.Id;
            Delegation.DelegatedTo = DelegateTo.SelectedUser.Id;
            Delegation.StartDate = StartDate.SelectedDate.Value;
            Delegation.EndDate = new DateTime(EndDate.SelectedDate.Value.Year, EndDate.SelectedDate.Value.Month, EndDate.SelectedDate.Value.Day, 23, 59, 59);
            Delegation.DelegateAttestationTask = Attestation.Checked;
            Delegation.DelegateProvisioningTask = Provisioning.Checked;
            Delegation.DelegationComments = Comment.Text;
            Delegation.IsActive = IsActive.Checked;
            Delegation.LastModifiedDate = DateTime.Now;
            Delegation.LastModifiedBy = new SessionStatusImpl().User.UserName;
            DomainApiClient<Dto.TaskDelegation>.Save(Delegation);
            if(Delegation.Id==0)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, $"Added Newrecord - AdvanceTaskDelegation:Delegation From:{DelegateFrom.SelectedUser.DisplayName},Delegation To:{DelegateTo.SelectedUser.DisplayName}, StartDate:{Delegation.StartDate} EndDate:{Delegation.EndDate} IsActive:{Delegation.IsActive} Comments:{Delegation.DelegationComments} Attestation:{Delegation.DelegateAttestationTask} Provisioning:{Delegation.DelegateProvisioningTask}");

            }
            if (UIMode == UIMode.Edit)
            {
                var strChange =string.Empty;
                var exisitingFromUser = DomainApiClient<Dto.User>.GetById(existingDelegationData.ActionUser);
                var exisitingToUser = DomainApiClient<Dto.User>.GetById(existingDelegationData.DelegatedTo);
               
                string deltaData = UtilityMethods.PublicInstancePropertiesEqual(existingDelegationData, Delegation, new string[] { "LastModifiedDate", "LastModifiedBy", "CreatedDate", "CreatedBy","ActionUser","DelegatedTo"});

                if (Delegation != null && existingDelegationData != null)
                {
                    if (Delegation.ActionUser != existingDelegationData.ActionUser)
                    {
                        strChange += $" DelegatedFrom: OldValue: {exisitingFromUser?.DisplayName}  NewValue: {DelegateFrom?.SelectedUser?.DisplayName} ";
                    }
                   
                    if (Delegation.DelegatedTo != existingDelegationData.DelegatedTo)
                    {
                        strChange += $" DelegatedTo: OldValue: {exisitingToUser?.DisplayName}         NewValue: {DelegateTo?.SelectedUser?.DisplayName} ";
                    }
                    if (Delegation.IsActive != existingDelegationData.IsActive)
                    {
                        strChange += $" IsActive: OldValue: {exisitingToUser?.IsActive}         NewValue: {Delegation?.IsActive} ";
                    }
                    if (string.IsNullOrWhiteSpace(deltaData) && !string.IsNullOrWhiteSpace(strChange))
                    {
                        deltaData =$"{strChange} StartDate:{Delegation.StartDate} EndDate:{Delegation.EndDate}";
                    }
                    else if (!string.IsNullOrWhiteSpace(deltaData) && string.IsNullOrWhiteSpace(strChange))
                    {
                        strChange += $" DelegatedFrom: {exisitingFromUser?.DisplayName} ";
                        strChange += $" DelegatedTo:{exisitingToUser?.DisplayName} ";
                        deltaData = $"{strChange} {deltaData}";
                    }
                    else if(!string.IsNullOrWhiteSpace(deltaData) && !string.IsNullOrWhiteSpace(strChange))
                    {
                        deltaData = $"{strChange} {deltaData}";
                    }
                }

                if (!string.IsNullOrWhiteSpace(deltaData))
                    DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, $"AdvanceTaskDelegation: Delegation ID:{Delegation.Id}   {deltaData}");
            }

            //send pending task email to new approver
            Dto.TaskDelegationWrapper wrapper = new Dto.TaskDelegationWrapper();
            wrapper.Delegation = Delegation;
            wrapper.SelectedtaskList = new List<long>() { Delegation.Id };
            DomainApiClient<Dto.TaskDelegation>.GetByAction<Dto.TaskDelegation>
                (DomainApiAction.Tasks.TaskDelegation.CreateAdvanceTasksDelegationEmail, null, wrapper);
        }

        private bool IsValid()
        {
            string Message = "Please select the";
            string ControlMessage = "";
            if (DelegateFrom.SelectedUser == null)
            {
                ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Delegate From" : ControlMessage + " , Delegate From";
            }
            if (DelegateTo.SelectedUser == null)
            {
                ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Delegate To" : ControlMessage + " , Delegate To";
            }
            if (StartDate.SelectedDate == null)
            {
                ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Start Date" : ControlMessage + " , Start Date";
            }
            if (EndDate.SelectedDate == null)
            {
                ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " End Date" : ControlMessage + " , End Date";
            }

            if (ControlMessage != "")
            {
                int LastIndex = ControlMessage.LastIndexOf(",");
                Message += LastIndex > 0 ? ControlMessage.Substring(0, LastIndex) + " and" + ControlMessage.Substring(LastIndex + 1) : ControlMessage;
                Message += ".";
                ShowAlertMessage(AlertType.Error, Message);
                return false;
            }
            if (StartDate.SelectedDate != null && EndDate.SelectedDate != null)
            {
                if (EndDate.SelectedDate < StartDate.SelectedDate)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "End date should not be less than Start date");
                    return false;
                }
            }
            if (DelegateFrom.SelectedUser.Id == DelegateTo.SelectedUser.Id)
            {
                MasterPage.ShowAlert(AlertType.Error, "Self Delegation Not Allowed");
                return false;
            }
            if (Attestation.Checked == false && Provisioning.Checked == false)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select at least Delegate Attestation Tasks or Delegate Provisioning Tasks");
                return false;
            }
            if (string.IsNullOrWhiteSpace(Comment.Text))
            {
                MasterPage.ShowAlert(AlertType.Error, "Please enter Comments");
                return false;
            }
            var endDate = new DateTime(EndDate.SelectedDate.Value.Year, EndDate.SelectedDate.Value.Month, EndDate.SelectedDate.Value.Day, 23, 59, 59);
            var param = new Dictionary<string, string>();
            param.Add("requestType", RequestType.Provisioning.ToString());
            param.Add("actionUser", DelegateTo.SelectedUser.Id.ToString());
            param.Add("startDate", StartDate.SelectedDate.Value.ToString("dd-MM-yyyy 00:00:00.000"));
            param.Add("endDate", EndDate.SelectedDate.Value.ToString("dd-MM-yyyy 23:59:59.999"));
            var taskDelegation = DomainApiClient<Dto.TaskDelegation>.GetByAction<Dto.TaskDelegation>(DomainApiAction.Tasks.TaskDelegation.GetAdvanceDelegationByActionUser, param);
            if (taskDelegation != null && taskDelegation.DelegatedToUser != null)
            {
                ShowAlertMessage(AlertType.Error, $"{DelegateTo.SelectedUser.DisplayName}({DelegateTo.SelectedUser.UserName}) has advance delegation enabled(Delegated To:{taskDelegation.DelegatedToUser.DisplayName}). Please assign to other(s)");
                return false;
            }
            if (IsActive.Checked)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("actionUser", DelegateFrom.SelectedUser.Id.ToString());
                List<Dto.TaskDelegation> listDelegation = DomainApiClient<Dto.TaskDelegation>.GetByAction<List<Dto.TaskDelegation>>
                    (DomainApiAction.Tasks.TaskDelegation.GetByActionUser, parameter);
                #region UserDelegation
                if (listDelegation != null && listDelegation.Count > 0)
                {
                    //starting date in between
                    var inBetweenStart = listDelegation.Where(del => del.IsActive && del.StartDate.Value <= StartDate.SelectedDate.Value && del.EndDate.Value >= StartDate.SelectedDate.Value).ToList();

                    if (inBetweenStart != null && inBetweenStart.Count > 0)
                    {

                        var provDelegations = inBetweenStart.Where(a => a.DelegateProvisioningTask == Provisioning.Checked || a.DelegateAttestationTask == Attestation.Checked);
                        if (Delegation.Id > 0)
                        {

                            var temp = listDelegation.Where(del => del.IsActive && (del.DelegateProvisioningTask == Provisioning.Checked || del.DelegateAttestationTask == Attestation.Checked) && Delegation.Id != del.Id).ToList();
                            if (TimeBlockIsOverLapping(StartDate.SelectedDate.Value, endDate, temp))
                            {
                                MasterPage.ShowAlert(AlertType.Warning, string.Format("You already have an Active Delegation for the given period."));
                                return false;
                            }
                        }
                        else
                        {
                            if (provDelegations != null && provDelegations.Count() > 0)
                            {
                                MasterPage.ShowAlert(AlertType.Warning, string.Format("You already have an Active Delegation for the given period."));
                                return false;
                            }
                        }

                    }

                    //end date in between
                    var inBetweenEnd = listDelegation.Where(del => del.IsActive && del.StartDate.Value <= endDate && del.EndDate.Value >= endDate).ToList();

                    if (inBetweenEnd != null && inBetweenEnd.Count > 0)
                    {
                        var provDelegations = inBetweenEnd.Where(a => a.DelegateProvisioningTask == Provisioning.Checked || a.DelegateAttestationTask == Attestation.Checked);
                        if (Delegation.Id > 0)
                        {
                            var temp = listDelegation.Where(del => del.IsActive && (del.DelegateProvisioningTask == Provisioning.Checked || del.DelegateAttestationTask == Attestation.Checked) && Delegation.Id != del.Id).ToList();
                            if (TimeBlockIsOverLapping(StartDate.SelectedDate.Value, endDate, temp))
                            {
                                MasterPage.ShowAlert(AlertType.Warning, string.Format("You already have an Active Delegation for the given period."));
                                return false;
                            }
                        }
                        else
                        {
                            if (provDelegations != null && provDelegations.Count() > 0)
                            {
                                MasterPage.ShowAlert(AlertType.Warning, string.Format("You already have an Active Delegation for the given period."));
                                return false;
                            }
                        }

                    }
                }
                #endregion
                #region DelegatedUser Delegation
                parameter = new Dictionary<string, string>();
                parameter.Add("actionUser", DelegateTo.SelectedUser.Id.ToString());
                listDelegation = DomainApiClient<Dto.TaskDelegation>.GetByAction<List<Dto.TaskDelegation>>
                    (DomainApiAction.Tasks.TaskDelegation.GetByActionUser, parameter);

                listDelegation = listDelegation.Where(i => i.DelegatedTo == DelegateFrom.SelectedUser.Id).ToList();
                if (listDelegation != null && listDelegation.Count > 0)
                {
                    //starting date in between

                    var inBetweenStart = listDelegation.Where(del => (StartDate.SelectedDate.Value >= del.StartDate.Value &&
                    StartDate.SelectedDate.Value <= del.EndDate.Value) || (del.EndDate.Value >= StartDate.SelectedDate.Value && del.EndDate.Value <= endDate)
                    ).ToList();
                    if (inBetweenStart != null && inBetweenStart.Count > 0)
                    {
                        var provDelegations = inBetweenStart.Where(a => a.DelegateProvisioningTask == Provisioning.Checked || a.DelegateAttestationTask == Attestation.Checked);
                        if (Delegation.Id > 0)
                        {
                            provDelegations = provDelegations.Where(a => Delegation.Id != a.Id);
                        }
                        if (provDelegations != null && provDelegations.Count() > 0)
                        {
                            MasterPage.ShowAlert(AlertType.Warning, $"User:{DelegateTo.SelectedUser.DisplayName} has delegated for the selected period.Hence please select other user.");
                            return false;
                        }
                    }
                }
            }
            #endregion
            return true;
        }
        #endregion
        private static bool TimeBlockIsOverLapping(DateTime newTimeBlockStart, DateTime newTimeBlockEnd, List<Dto.TaskDelegation> existingTimeBlocks)
        {
            foreach (var block in existingTimeBlocks)
            {
                // This is only if existingTimeBlocks are ordered. It is only
                // a speedup
                //if (newTimeBlockStart > block.EndDate)
                //{
                //    break;
                //}

                // This is the real check. The ordering of the existingTimeBlocks
                // is irrelevant
                if (newTimeBlockStart <= block.EndDate && newTimeBlockEnd >= block.StartDate)
                {
                    return true;
                }
            }

            return false;
        }
        #region Events

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (IsValid())
            {
                Save();
                Response.Redirect("List.aspx");
            }
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("List.aspx");
        }
        #endregion

        protected void Provisioning_CheckedChanged(object sender, EventArgs e)
        {
            if (Provisioning.Checked)
            {
                DelegateTo.Clear();
                DelegateTo.DataSourceMethod = "GetUsersWithSameGradeOrAbove";
            }
            else
            {
                DelegateTo.DataSourceMethod = "GetUsers";
            }
        }
    }
}