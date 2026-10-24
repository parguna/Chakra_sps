using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Triggers.UC
{
    public partial class AddTriggerUC : UIBaseUC
    {
        #region Members

        SessionStatusImpl sessionStatus = new SessionStatusImpl();
        const string V_USERID = "USERID";

        #endregion

        #region Properties

        long UserId
        {
            get
            {
                if (ViewState[V_USERID] == null)
                    return 0;
                return (long)ViewState[V_USERID];
            }
            set
            {
                ViewState[V_USERID] = value;
            }
        }

        public DateTime SelectedEffectiveDate
        {
            get { return EffectiveDatePicker.SelectedDate ?? DateTime.Now; }
        }

        #endregion

        #region Page Events
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            HiddenValue.Value = false.ToString();

            if (!Page.IsPostBack)
            {
                ResetData();
                RadioButton_CheckedChanged(null, null);
            }
            
        }
        void ResetData()
        {
            EffectiveDatePicker.MinDate = DateTime.Now;
            TerminationDatePicker.MinDate = DateTime.Now;
            terminationDateDIV.Visible = false;
            HideShowControls(false);
            groupBox.Disabled = false;
            PropagationRadioButton.Checked = true;
            NUserDetailUC.IsFromTriggerPage = true;
            NUserDetailUC.User = null;
        }
        void HideShowControls(bool show)
        {
            divCompany.Visible = show;
            divDepartment.Visible = show;
            divDivision.Visible = show;
            divSection.Visible = show;
            divCostCenter.Visible = show;
            divCostDistribution.Visible = show;
            div1.Visible = show;
        }
        Dto.User GetUser()
        {
            return DomainApiClient<Dto.User>.GetById(Convert.ToInt64(UserNameAutoCompleteBox.Entries[0].Value));
        }
        void FillExistingData()
        {
            autoCompleteCompany.Clear();
            autoCompleteDepartment.Entries.Clear();
            autoCompleteDivision.Entries.Clear();
            autoCompleteCostCenter.Clear();
            autoCompleteSection.Entries.Clear();
            autoCompleteUnit.Entries.Clear();

            var user = GetUser();

            autoCompleteCompany.SetValue($"{user.CompanyName}({user.CompanyCode})", user.CompanyName.Trim());
            AutoCompleteBox_EntryAdded(autoCompleteCompany, null);


            SetValue(autoCompleteDepartment, $"{user.DepartmentName}({user.DepartmentCode})", user.DepartmentCode);
            AutoCompleteBox_EntryAdded(autoCompleteDepartment, null);

            if (!string.IsNullOrWhiteSpace(user.BranchCode))
            {
                SetValue(autoCompleteDivision, $"{user.OrgBranch}({user.BranchCode})", user.BranchCode);
                AutoCompleteBox_EntryAdded(autoCompleteDivision, null);
            }

            if (!string.IsNullOrWhiteSpace(user.SectionCode))
            {
                SetValue(autoCompleteSection, $"{user.SectionName}({user.SectionCode})", user.SectionCode);
                AutoCompleteBox_EntryAdded(autoCompleteSection, null);
            }

            if (!string.IsNullOrWhiteSpace(user.UnitCode))
            {
                SetValue(autoCompleteUnit, $"{user.OrgSection}({user.UnitCode})", user.UnitCode);
                AutoCompleteBox_EntryAdded(autoCompleteUnit, null);
            }

            autoCompleteCostCenter.SetValue(user.CostCenter + " - " + user.CostCenterText, user.CostCenter);
        }

        void SetValue(RadAutoCompleteBox autoCompleteBox, string Text, string Value)
        {
            autoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(Text, Value));
        }
        protected void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UserNameAutoCompleteBox.Entries.Clear();

            HideShowControls(false);

            if (this.TransferRadioButton.Checked || this.ReorgRadioButton.Checked)
            {
                HideShowControls(true);

                if (this.PropagationRadioButton.Checked)
                {
                    divDepartment.Attributes.Add("class", "form-group required");
                }

                if (this.TransferRadioButton.Checked)
                {
                    divCostCenter.Attributes.Add("class", "form-group required");
                    divDepartment.Attributes.Add("class", "form-group required");
                }
            }

            if (this.OffBoardRadioButton.Checked || this.PropagationRadioButton.Checked)
            {
                UserNameAutoCompleteBox.WebServiceSettings.Method = "GetUsers";
            }
            else
            {
                UserNameAutoCompleteBox.WebServiceSettings.Method = "GetFullTimeUsers";
            }
        }

        protected void UserNameAutoCompleteBox_EntryAdded(object sender, Telerik.Web.UI.AutoCompleteEntryEventArgs e)
        {
            if (UserNameAutoCompleteBox.Entries.Count > 0)
            {
                this.UserId = Convert.ToInt64(UserNameAutoCompleteBox.Entries[0].Value);
                User usr = DomainApiClient<User>.GetById((long)this.UserId, true);
                NUserDetailUC.IsFromTriggerPage = true;
                NUserDetailUC.User = usr;
                groupBox.Disabled = false;
                FillExistingData();
            }
        }

        protected void SuperVisorNameAutoCompleteBox_EntryAdded(object sender, Telerik.Web.UI.AutoCompleteEntryEventArgs e)
        {
            //if (SuperVisorAutoCompleteBox.Entries.Count > 0)
            //{
            //    this.UserId = Convert.ToInt64(UserNameAutoCompleteBox.Entries[0].Value);
            //    User usr = DomainApiClient<User>.GetById((long)this.UserId, true);
            //    NUserDetailUC.User = usr;
            //    groupBox.Disabled = false;
            //}
        }

        protected void UserNameAutoCompleteBox_EntryRemoved(object sender, Telerik.Web.UI.AutoCompleteEntryEventArgs e)
        {
            string redirectTarget = Request.Url.PathAndQuery;
            if (DI.Chakra.Infrastructure.UtilityMethods.IsLocalUrl(redirectTarget))
                Response.Redirect(redirectTarget);
            else
                Response.Redirect("~/Default.aspx");
        }

        protected void ReferenceUsersAutoCompleteBox_EntryAdded(object sender, Telerik.Web.UI.AutoCompleteEntryEventArgs e)
        {
            // SetCompanyDetails(ReferenceUsersAutoCompleteBox.Entries[0].Value);
        }

        private void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                this.SaveTrigger();
            }
        }

        #endregion

        #region Methods
        bool ValidData()
        {
            var errorMessage = string.Empty;
            if (this.UserNameAutoCompleteBox.Entries == null || this.UserNameAutoCompleteBox.Entries.Count <= 0)
            {
                errorMessage += "Please select User.";
            }
            if (string.IsNullOrEmpty(this.EffectiveDatePicker.SelectedDate.ToString()))
            {
                errorMessage += "Please select effective date before creating trigger.";
            }
            if (!string.IsNullOrEmpty(errorMessage.Trim()))
            {
                MasterPage.ShowAlert(AlertType.Error, errorMessage);
                return false;
            }
            else
                return true;
        }
        AutoCompleteBoxEntry GetValue(RadAutoCompleteBox autoCompleteBox)
        {
            if (autoCompleteBox.Entries.Count > 0)
                return autoCompleteBox.Entries[0];
            else
                return null;
        }
        private bool IsValidOU()
        {
            string message = string.Empty;

            if (string.IsNullOrWhiteSpace(Convert.ToString(autoCompleteCompany.GetValue())))
            {
                message += "Enter a valid Company.";
            }

            if (string.IsNullOrWhiteSpace(Convert.ToString(GetValue(autoCompleteDepartment))))
            {
                message += "Enter a valid Department.";
            }

            if (string.IsNullOrWhiteSpace(Convert.ToString(GetValue(autoCompleteDivision))))
            {
                message += "Enter a valid Branch.";
            }

            if (string.IsNullOrWhiteSpace(Convert.ToString(GetValue(autoCompleteSection))))
            {
                message += "Enter a valid OU.";
            }

            if (string.IsNullOrWhiteSpace(Convert.ToString(autoCompleteCostCenter.GetValue())))
            {
                message += "Enter a valid Cost Center.";
            }

            if (string.IsNullOrEmpty(message))
            {
                return true;
            }
            else
            {
                ShowAlertMessage(AlertType.Error, "Please enter valid OU information.");
                return false;
            }
        }
        void SaveTrigger()
        {
            try
            {
                if (ValidData())
                {
                    DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Add Trigger - Save");

                    if (this.OnBoardRadioButton.Checked || RejoinRadioButton.Checked)
                    {
                        CreateOnBoardTrigger();
                    }
                    else if (this.OffBoardRadioButton.Checked)
                    {
                        CreateOffBoardTrigger();
                    }
                    else if (this.PropagationRadioButton.Checked)
                    {
                        CreatePropagationTrigger();
                    }
                    else if (this.TransferRadioButton.Checked)
                    {
                        if (IsValidOU())
                        {
                            CreateTransferTrigger();
                        }
                        else
                        {
                            return;
                        }
                    }
                    else if (this.ReorgRadioButton.Checked)
                    {
                        if (IsValidOU())
                        {
                            CreateReOrgTrigger();
                        }
                        else
                        {
                            return;
                        }
                    }
                    this.UserNameAutoCompleteBox.Entries.RemoveAt(0);

                    ShowAlertMessage(AlertType.Success, "Trigger created successfully.");
                    Response.Redirect("~/Triggers/List.aspx");
                }
            }
            catch (Exception ex)
            {
                ShowAlertMessage(AlertType.Error, "Error while creating trigger - " + ex.Message);
            }
        }
        //private void ClearUserInfo()
        //{

        //    this.OnBoardRadioButton.Checked = false;
        //    this.OffBoardRadioButton.Checked = false;
        //    this.PropagationRadioButton.Checked = true;
        //    this.ReorgRadioButton.Checked = false;
        //    this.TransferRadioButton.Checked = false;
        //    this.RejoinRadioButton.Checked = false;

        //    NUserDetailUC.IsFromTriggerPage = true;
        //    NUserDetailUC.User = null;

        //    this.OnBoardRadioButton.Enabled = this.RejoinRadioButton.Enabled = this.OffBoardRadioButton.Enabled =
        //        this.PropagationRadioButton.Enabled = this.TransferRadioButton.Enabled = this.ReorgRadioButton.Enabled = false;
        //}
        void SaveAndWriteLog(Dto.EmployeeEventTrigger trigger, Dto.User user)
        {
            bool triggerCreated = DomainApiClient<EmployeeEventTrigger>.GetByAction<bool>(DomainApiAction.Provisioning.EmployeeEventTrigger.CreateEmployeeEventTrigger, null, trigger);
            ResetData();
            DILog.UILogger.WriteLog(sessionStatus.User?.Id, LogLevelType.Info, string.Format("{0} trigger for user {1}", UtilityMethods.GetEnumAttribute<EmployeeEventTrigger>(trigger.EventType), user.DisplayName));
        }
        Dto.EmployeeEventTrigger GetExistingTrigger(long userId, EmployeeEvents employeeEvent)
        {
            Dictionary<string, string> parameterEvent = new Dictionary<string, string>();
            parameterEvent.Add("userId", Convert.ToString(userId));
            parameterEvent.Add("evnt", employeeEvent.ToString());
            parameterEvent.Add("date", string.Empty);

            var tempTrigger = DomainApiClient<Dto.EmployeeEventTrigger>.GetByAction<Dto.EmployeeEventTrigger>
                (DomainApiAction.Provisioning.EmployeeEventTrigger.GetByUserAndEvent, parameterEvent);
            return tempTrigger;
        }

        protected void AutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            string group = string.Empty;
            var autoCompleteBox = sender as RadAutoCompleteBox;
            AutoCompleteBoxEntry NewItem;
            string id = string.Empty;
            if (autoCompleteBox == null)
            {
                var genericAutoCompleteBox = sender as DI.Chakra.Web.UsrControl.Common.GenericAutoCompleteBox;
                NewItem = genericAutoCompleteBox.GetValue();
                id = genericAutoCompleteBox.ID;
            }
            else
            {
                NewItem = autoCompleteBox.Entries[autoCompleteBox.Entries.Count - 1];
                id = autoCompleteBox.ID;
            }




            var name = string.Empty;
            switch (id)
            {
                case "autoCompleteCompany":

                    var dict = DomainApiClient<NOrganisation>.GetByAction<Dictionary<string, string>>(DomainApiAction.Org.Organisation.GetOrganisationsByName, new Dictionary<string, string> { { "name", NewItem.Value } });
                    if (dict != null && dict.Count > 0)
                    {
                        NewItem = new AutoCompleteBoxEntry() { Value = dict.Keys.First() };
                        name = "CompanyCode";
                    }

                    break;
                case "autoCompleteDepartment":
                    name = "DepartmentCode";
                    break;
                case "autoCompleteDivision":
                    name = "BranchCode";
                    break;
                case "autoCompleteSection":
                    name = "SectionCode";
                    break;
                case "autoCompleteUnit":
                    name = "UnitCode";
                    break;
            }
            if (!string.IsNullOrWhiteSpace(name) && NewItem != null)
            {
                Session[name] = NewItem.Value;
            }

        }

        protected void AutoCompleteBox_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {

            var autoCompleteBox = sender as RadAutoCompleteBox;
            if (autoCompleteBox.Entries != null)
            {
                switch (autoCompleteBox.ID)
                {
                    case "AutoCompleteBoxCompany":
                        autoCompleteCompany.Clear();
                        AutoCompleteBox_EntryRemoved(autoCompleteDepartment, null);
                        break;
                    case "autoCompleteDepartment":
                        autoCompleteDepartment.Entries.Clear();
                        AutoCompleteBox_EntryRemoved(autoCompleteDivision, null);
                        break;
                    case "autoCompleteDivision":
                        autoCompleteDivision.Entries.Clear();
                        AutoCompleteBox_EntryRemoved(autoCompleteSection, null);
                        break;
                    case "autoCompleteSection":
                        autoCompleteSection.Entries.Clear();
                        AutoCompleteBox_EntryRemoved(autoCompleteUnit, null);
                        break;
                    case "autoCompleteUnit":
                        autoCompleteUnit.Entries.Clear();
                        break;
                }
            }
        }
        private void GetUserOrgInfo(User user)
        {
            string routePrefix = "User";
            try
            {
                if (GetValue(autoCompleteDepartment) != null)
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("code", Convert.ToString(GetValue(autoCompleteDepartment)?.Value));
                    var department = DomainApiClient<Dto.NOrganisation>.GetByAction<Dto.NOrganisation>(routePrefix, DomainApiAction.User.GetOrganisationByCode, parameter, null);
                    if (department != null)
                    {
                        user.DepartmentCode = department.Code;
                        user.DepartmentName = department.Name;
                        user.DepartmentId = department.Id;
                        user.OrganisationCode = department.Code;
                    }
                }

                if (autoCompleteCompany.GetValue() != null)
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("code", Convert.ToString(autoCompleteCompany.GetValue()?.Value));
                    var company = DomainApiClient<Dto.User>.GetByAction<Dto.Company>(routePrefix, DomainApiAction.User.GetCompanyByCode, parameter, null);
                    if (company != null)
                    {
                        user.CompanyId = company.Id;
                        user.CompanyName = company.Text;
                        user.CompanyCode = company.Code;
                    }
                }

                if (GetValue(autoCompleteDivision) != null)
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("code", Convert.ToString(GetValue(autoCompleteDivision)?.Value));
                    var division = DomainApiClient<Dto.NOrganisation>.GetByAction<Dto.NOrganisation>(routePrefix, DomainApiAction.User.GetOrganisationByCode, parameter, null);
                    if (division != null)
                    {
                        user.DivisionCode = division.Code;
                        user.DivisionName = division.Name;
                        user.DivisionId = division.Id;
                        user.OrganisationCode = division.Code;
                    }
                }

                if (autoCompleteCostCenter.GetValue() != null)
                {
                    user.CostCenter = autoCompleteCostCenter.GetValue().Value;
                    var costCenterInfo = autoCompleteCostCenter.GetValue().Text.Split('-');
                    user.CostCenterText = costCenterInfo[1].Trim();
                }

                if (autoCompleteCostDistribution.GetValue() != null)
                {
                    user.CostDistribution = autoCompleteCostDistribution.GetValue().Value;
                }

                if (GetValue(autoCompleteSection) != null)
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("code", Convert.ToString(GetValue(autoCompleteSection).Value));
                    var section = DomainApiClient<Dto.NOrganisation>.GetByAction<Dto.NOrganisation>(routePrefix, DomainApiAction.User.GetOrganisationByCode, parameter, null);
                    if (section != null)
                    {
                        user.NOrganisationId = section.Id;
                        user.SectionCode = section.Code;
                        user.SectionName = section.Name;
                        user.OrganisationCode = section.Code;
                    }
                }

                if (GetValue(autoCompleteUnit) != null)
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("code", Convert.ToString(GetValue(autoCompleteUnit).Value));
                    var section = DomainApiClient<Dto.NOrganisation>.GetByAction<Dto.NOrganisation>(routePrefix, DomainApiAction.User.GetOrganisationByCode, parameter, null);
                    if (section != null)
                    {
                        user.NOrganisationId = section.Id;
                        user.UnitCode = section.Code;
                        user.OrgUnit = section.Name;
                        user.OrganisationCode = section.Code;
                    }
                }

                var ous = DomainApiClient<List<Dto.NOrganisation>>.GetByAction<List<Dto.NOrganisation>>(routePrefix, DomainApiAction.User.GetAllOrganisations, null, null);
                var ou = ous.FirstOrDefault(item => item.Code == user.SectionCode);
                while (ou != null && ou.OrganisationScope != null)
                {
                    if (ou.OrganisationScope.Scope.StartsWith("40"))
                    {
                        user.OrgSection = ou.Name;
                    }
                    else if (ou.OrganisationScope.Scope.StartsWith("30"))
                    {
                        user.OrgBranch = ou.Name;
                    }
                    else if (ou.OrganisationScope.Scope.StartsWith("20"))
                    {
                        user.OrgDivision = ou.Name;
                    }
                    else if (ou.OrganisationScope.Scope.StartsWith("10"))
                    {
                        user.OrgDepartment = ou.Name;
                    }
                    else if (ou.OrganisationScope.Scope.StartsWith("00"))
                    {
                        user.OrgCompany = ou.Name;
                    }
                    ou = ous.FirstOrDefault(item => item.Code == ou.ParentOrgCode);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void CreateOnBoardTrigger()
        {
            try
            {
                var user = GetUser();

                EmployeeEventTrigger trigger = new EmployeeEventTrigger();

                trigger.UserId = user.Id;
                trigger.EventType = EmployeeEvents.NewJoinee;
                trigger.CreatedBy = sessionStatus.User.UserName;
                trigger.CreatedDate = DateTime.Now;
                trigger.IsActive = true;
                trigger.EffectiveDate = SelectedEffectiveDate;

                if (RejoinRadioButton.Checked)
                {
                    trigger.EventType = EmployeeEvents.Rehire;
                    trigger.UserDetails = UtilityMethods.Serialize(user);
                }
                SaveAndWriteLog(trigger, user);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void CreateOffBoardTrigger()
        {
            try
            {
                if (UserNameAutoCompleteBox.Entries.Count > 0)
                {
                    var user = GetUser();
                    string oldUserDetails = UtilityMethods.Serialize(user);
                    var evnt = EmployeeEvents.Resignation;
                    EmployeeEventTrigger trigger = GetExistingTrigger(user.Id, evnt);

                    if (trigger != null)
                    {
                        trigger.EffectiveDate = SelectedEffectiveDate;
                        trigger.LastModifiedBy = trigger.CreatedBy;
                        trigger.LastModifiedDate = DateTime.Now;
                    }
                    else
                    {
                        trigger = new EmployeeEventTrigger()
                        {
                            CreatedBy = sessionStatus.User.UserName,
                            CreatedDate = DateTime.Now,
                            EffectiveDate = SelectedEffectiveDate,
                            EventType = evnt,
                            IsRequestCreated = false,
                            UserId = user.Id,
                            IsActive = true
                        };
                    }
                    trigger.OldUserDetails = oldUserDetails;
                    SaveAndWriteLog(trigger, user);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void CreatePropagationTrigger()
        {
            try
            {
                var user = GetUser();

                EmployeeEventTrigger trigger = new EmployeeEventTrigger()
                {
                    CreatedBy = sessionStatus.User.UserName,
                    CreatedDate = DateTime.Today,
                    EffectiveDate = SelectedEffectiveDate,
                    EventType = EmployeeEvents.Propagation,
                    IsRequestCreated = false,
                    UserId = user.Id,
                    IsActive = true
                };

                SaveAndWriteLog(trigger, user);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void CreateReOrgTrigger()
        {
            try
            {
                var user = GetUser();

                string oldUserDetails = UtilityMethods.Serialize(user);

                this.GetUserOrgInfo(user);

                EmployeeEventTrigger trigger = new EmployeeEventTrigger();

                trigger.UserId = user.Id;
                trigger.EventType = EmployeeEvents.ReOrganisation;
                trigger.EffectiveDate = SelectedEffectiveDate;
                trigger.IsActive = true;
                trigger.CreatedBy = sessionStatus.User.UserName;
                trigger.CreatedDate = DateTime.Now;

                trigger.OldUserDetails = oldUserDetails;

                trigger.UserDetails = UtilityMethods.Serialize(user);
                SaveAndWriteLog(trigger, user);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void CreateTransferTrigger()
        {
            try
            {
                var user = GetUser();

                string oldUserDetails = UtilityMethods.Serialize(user);

                this.GetUserOrgInfo(user);

                EmployeeEventTrigger trigger = new EmployeeEventTrigger();

                trigger.UserId = user.Id;
                trigger.EventType = EmployeeEvents.OUChange;
                trigger.EffectiveDate = SelectedEffectiveDate;
                trigger.IsActive = true;
                trigger.CreatedBy = sessionStatus.User.UserName;
                trigger.CreatedDate = DateTime.Now;

                trigger.OldUserDetails = oldUserDetails;

                trigger.UserDetails = UtilityMethods.Serialize(user);
                SaveAndWriteLog(trigger, user);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}