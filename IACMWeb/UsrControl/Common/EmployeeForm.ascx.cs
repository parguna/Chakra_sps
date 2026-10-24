using System;
using Application;
using System.Web.UI.WebControls;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System.Text;
using System.Collections.Generic;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class EmployeeForm : UIBaseUC
    {
        #region Members

        private const string V_ISEDIT = "ISEDIT";
        private const string V_USER = "USER";
        private const string V_ROLE = "ROLE";
        private const string V_FULLTIMEEMPLOYEE = "FULLTIMEEMPLOYEE";
        private const string V_FILENAME = "FILENAME";
        private const string V_PROFILEID = "PROFILEID";
        private const string V_PASS = "PASSWORD";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        private const string V_MANAGER = "MANAGER";
        private SessionStatusImpl SessionUser = new SessionStatusImpl();
        private long getUserId = 0;

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
        public bool IsFullTimeEmployee
        {
            get
            {
                if (ViewState[V_FULLTIMEEMPLOYEE] == null)
                    return false;
                return (bool)ViewState[V_FULLTIMEEMPLOYEE];
            }
            set
            {
                ViewState[V_FULLTIMEEMPLOYEE] = value;
            }
        }
        public Dto.User User
        {
            get
            {
                if (ViewState[V_USER] == null)
                    return new Dto.User();
                return (Dto.User)ViewState[V_USER];
            }
            set
            {
                ViewState[V_USER] = value;
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

        public List<Dto.Role> RoleList
        {
            get
            {
                if (ViewState[V_ROLE] == null)
                    return new List<Dto.Role>();
                return (List<Dto.Role>)ViewState[V_ROLE];
            }
            set
            {
                ViewState[V_ROLE] = value;
            }
        }
        public byte[] UploadedFileName
        {
            get
            {
                if (ViewState[V_FILENAME] == null)
                    return null;
                return (byte[])ViewState[V_FILENAME];
            }
            set
            {
                ViewState[V_FILENAME] = value;
            }
        }
        public long UserProfileId
        {
            get
            {
                if (ViewState[V_PROFILEID] == null)
                    return 0;
                return (long)ViewState[V_PROFILEID];
            }
            set
            {
                ViewState[V_PROFILEID] = value;
            }
        }

        public long UserManagerId
        {
            get
            {
                if (ViewState[V_MANAGER] == null)
                    return 0;
                return (long)ViewState[V_MANAGER];
            }
            set
            {
                ViewState[V_MANAGER] = value;
            }
        }

        public string PasswordValue
        {
            get
            {
                if (ViewState[V_PASS] == null)
                    return Password.Text;
                return (string)ViewState[V_PASS];
            }
            set
            {
                ViewState[V_PASS] = value;

            }
        }
        #endregion

        #region Methods
        public void Initialize()
        {
            BindEmployeeType();
            SetPage();
        }
        public void HideReferenceUser()
        {
            referenceUser.Visible = false;
        }
        
        public void HideAstricks()
        {
            IdentificationNo.Attributes.Remove("required");
            EmployeeNo.Attributes.Remove("required");
            DisplayName.Attributes.Remove("required");
            FirstName.Attributes.Remove("required");
            Password.Attributes.Remove("required");
           // txtMobileNo.Attributes.Remove("required");
        }

        public void DisableDateFields()
        {
            DatePickerValidFrom.Enabled = false;
            DatePickerValidTo.Enabled = false;
        }


        public void CheckSelfUpdateField()
        {
            Dictionary<string, string> paramNRIC = new Dictionary<string, string>();
            paramNRIC.Add("attributeName", "NRIC");
            IdentificationNo.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, paramNRIC);

            Dictionary<string, string> ParamDisplayName = new Dictionary<string, string>();
            ParamDisplayName.Add("attributeName", "DisplayName");
            DisplayName.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamDisplayName);

            Dictionary<string, string> ParamFirstName = new Dictionary<string, string>();
            ParamFirstName.Add("attributeName", "FirstName");
            FirstName.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamFirstName);

            Dictionary<string, string> ParamLastName = new Dictionary<string, string>();
            ParamLastName.Add("attributeName", "LastName");
            LastName.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamLastName);

            Dictionary<string, string> ParamUserName = new Dictionary<string, string>();
            ParamUserName.Add("attributeName", "ParamUserName");
            IdentificationNo.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamUserName);

            Dictionary<string, string> ParamCompanyName = new Dictionary<string, string>();
            ParamCompanyName.Add("attributeName", "ParamCompanyName");
            ReferenceUsersAutoCompleteBox.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamCompanyName);

            Dictionary<string, string> ParamCostCenter = new Dictionary<string, string>();
            ParamCostCenter.Add("attributeName", "CostCenter");
            ReferenceUsersAutoCompleteBox.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamCostCenter);

            Dictionary<string, string> ParamDepartmentName = new Dictionary<string, string>();
            ParamDepartmentName.Add("attributeName", "DepartmentName");
            ReferenceUsersAutoCompleteBox.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamDepartmentName);

            Dictionary<string, string> ParamDivisionName = new Dictionary<string, string>();
            ParamDivisionName.Add("attributeName", "DivisionName");
            IdentificationNo.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamDivisionName);

            Dictionary<string, string> ParamOrgCompany = new Dictionary<string, string>();
            ParamOrgCompany.Add("attributeName", "OrgCompany");
            IdentificationNo.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamOrgCompany);

            Dictionary<string, string> ParamJoinDate = new Dictionary<string, string>();
            ParamJoinDate.Add("attributeName", "JoinDate");
            IdentificationNo.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamJoinDate);

            Dictionary<string, string> ParamTerminationDate = new Dictionary<string, string>();
            ParamTerminationDate.Add("attributeName", "TerminationDate");
            IdentificationNo.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamTerminationDate);

            Dictionary<string, string> ParamEmployeeType = new Dictionary<string, string>();
            ParamEmployeeType.Add("attributeName", "EmployeeType");
            TypeofEmployee.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamEmployeeType);

            Dictionary<string, string> ParamTelephoneNumber = new Dictionary<string, string>();
            ParamTelephoneNumber.Add("attributeName", "TelephoneNumber");
            TelephoneNo.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamTelephoneNumber);

            Dictionary<string, string> ParamMobileNumber = new Dictionary<string, string>();
            ParamMobileNumber.Add("attributeName", "MobileNumber");
            txtMobileNo.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamMobileNumber);
        }

        private void BindEmployeeType()
        {
            TypeofEmployee.Items.Clear();
            foreach (EmployeeType item in Enum.GetValues(typeof(EmployeeType)))
            {
                var description = UtilityMethods.GetEnumAttribute<Description>(item);
                ListItem listItem = new ListItem(description, ((byte)item).ToString());
                if (item == EmployeeType.None || item == EmployeeType.Permanent)
                {
                    if (IsFullTimeEmployee)
                        TypeofEmployee.Items.Add(listItem);
                }
                else
                {
                    TypeofEmployee.Items.Add(listItem);
                }
            }
        }

        private string SetEmployeeNo()
        {
            string empID = null;
            string defaultEmployeeKey = MasterConfigHelper.EmployeeDefaultKey;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", defaultEmployeeKey);
            string EmpId = DomainApiClient<Dto.User>.GetByAction<string>(DomainApiAction.User.GetEmployeeId, parameter);

            if (EmpId == null)
            {
                EmployeeNo.Text = defaultEmployeeKey + "000000";
                empID = EmployeeNo.Text;
            }
            else
            {
                empID = EmpId.Replace(defaultEmployeeKey, "").Trim();
                EmployeeNo.Text = defaultEmployeeKey + (Convert.ToInt64(empID) + 1).ToString("000000");
                empID = EmployeeNo.Text;
            }
            return empID;
        }
        private void TrackEventTriggers(Dto.User user, bool InitiatePropagation)
        {
            if (InitiatePropagation && MasterConfigHelper.CanRunPropagationTrigger)
                this.PopulatePropagationTrigger(user, InitiatePropagation);
        }

        private void PopulatePropagationTrigger(Dto.User user, bool InitiatePropagatio)
        {
            var evnt = InitiatePropagatio ? EmployeeEvents.Propagation : EmployeeEvents.Resignation;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", Convert.ToString(user.Id));
            parameter.Add("evnt", evnt.ToString());

            var trigger = DomainApiClient<Dto.EmployeeEventTrigger>.GetByAction<List<Dto.EmployeeEventTrigger>>
                (DomainApiAction.Provisioning.EmployeeEventTrigger.GetTriggerToExecute, parameter);

            if (trigger != null && trigger.Count > 0)
            {
                Dictionary<string, string> parameterEvent = new Dictionary<string, string>();
                parameterEvent.Add("userId", Convert.ToString(user.Id));
                parameterEvent.Add("evnt", evnt.ToString());
                parameterEvent.Add("date", DateTime.Today.ToString());

                var tempTrigger = DomainApiClient<Dto.EmployeeEventTrigger>.GetByAction<Dto.EmployeeEventTrigger>
                    (DomainApiAction.Provisioning.EmployeeEventTrigger.GetByUserAndEvent, parameterEvent);

                if (tempTrigger != null && tempTrigger.IsRequestCreated)
                {
                    return;
                }
                trigger[0].EffectiveDate = user.TerminationDate != null ? user.TerminationDate.Value : DateTime.Now;
                trigger[0].LastModifiedBy = trigger[0].CreatedBy;
                trigger[0].LastModifiedDate = DateTime.Now;

                DomainApiClient<Dto.EmployeeEventTrigger>.Save(trigger[0]);
            }
            else
            {
                Dto.EmployeeEventTrigger newTrigger = new Dto.EmployeeEventTrigger()
                {
                    CreatedBy = user.CreatedBy,
                    CreatedDate = user.CreatedDate,
                    EffectiveDate = user.TerminationDate != null ? user.TerminationDate.Value : DateTime.Now,
                    EventType = evnt,
                    IsRequestCreated = false,
                    UserId = user.Id,
                    IsActive = true
                };
                DomainApiClient<Dto.EmployeeEventTrigger>.Save(newTrigger);
            }
        }
        private void SetCompanyDetails(string userid)
        {
            if (string.IsNullOrEmpty(userid.Trim()))
            {
                return;
            }

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", userid);

            Dto.User ouser = DomainApiClient<Dto.User>.GetByAction<Dto.User>(DomainApiAction.User.GetByIdReferenceUser, parameter);
            if (ouser.UserExtensionObj != null)
            {
                if (!(string.IsNullOrEmpty((ouser.UserExtensionObj.CompanyName))))
                {
                    CompanyNameAutoCompleteBox.Text = ouser.UserExtensionObj.CompanyName;
                    CompanyNameAutoCompleteBox.Attributes["CompanyValue"] = ouser.UserExtensionObj.CompanyId + "-" + ouser.UserExtensionObj.CompanyCode;
                }
                CostCenterAutoCompleteBox.Text = ouser.UserExtensionObj.CostCenter;
                if (!(string.IsNullOrEmpty((ouser.UserExtensionObj.DepartmentName))))
                {
                    DepartmentAutoCompleteBox.Text = ouser.UserExtensionObj.DepartmentName;
                    DepartmentAutoCompleteBox.Attributes["DepartmentValue"] = ouser.UserExtensionObj.DepartmentId + "-" + ouser.UserExtensionObj.DepartmentCode;
                }
                DivisionAutoCompleteBox.Text = ouser.UserExtensionObj.DivisionName;
                DivisionAutoCompleteBox.Attributes["DivisionValue"] = ouser.UserExtensionObj.DivisionId + "-" + ouser.UserExtensionObj.DivisionCode;
            }
            parameter.Clear();
            parameter.Add("orgID", Convert.ToString(ouser.OrganisationId));

            OrganisationAutoCompleteBox.Text = DomainApiClient<Dto.Organisation>.GetByAction<string>(DomainApiAction.Org.Organisation.GetOrgUnitPrefixById, parameter);
            OrganisationAutoCompleteBox.Attributes["OrganisationValue"] = ouser.OrganisationId + "-" +
                DomainApiClient<Dto.Organisation>.GetByAction<string>(DomainApiAction.Org.Organisation.GetOrgUnitPrefixById, parameter);
        }

        private string ConstructUserName()
        {
            string strUserName = string.Empty;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userName", DisplayName.Text);
            parameter.Add("identificationNo", IdentificationNo.Text);
            strUserName = DomainApiClient<Dto.User>.GetByAction<string>(DomainApiAction.User.ConstructUserName, parameter);
            return strUserName;
        }

        private string CheckUserNameExists(string userName)
        {
            int no = 1;
            if (userName.Length > 8)
                userName = userName.Substring(0, 8);

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("sAMAccountName", userName);
            parameter.Add("endpointsystemId", "1");
            bool isExist = DomainApiClient<Dto.ADSUserInfo>.GetByAction<bool>(DomainApiAction.Endpoint.ActiveDirectory.ADSUserInfo.DoesExists, parameter);

            while (isExist)
            {
                userName = userName.Substring(0, userName.Length - 1) + no;
                parameter.Clear();
                parameter.Add("sAMAccountName", userName);
                parameter.Add("endpointsystemId", "1");

                isExist = DomainApiClient<Dto.ADSUserInfo>.GetByAction<bool>(DomainApiAction.Endpoint.ActiveDirectory.ADSUserInfo.DoesExists, parameter);
                no++;
            }
            return userName;
        }


        private void SetPage()
        {
            if (!IsEdit)
            {
                Dto.User user = new Dto.User();
                if (IsFullTimeEmployee)
                {
                    if (TypeofEmployee.Items.FindByText(Convert.ToString(EmployeeType.Permanent)) != null)
                        TypeofEmployee.Items.FindByText(Convert.ToString(EmployeeType.Permanent)).Selected = true;
                    TypeofEmployee.Enabled = false;
                }
                if (DatePickerValidFrom.SelectedDate == null)
                {
                    DatePickerValidFrom.MinDate = DateTime.Now;
                    DatePickerValidFrom.SelectedDate = DateTime.Now;
                    DatePickerValidTo.MinDate = (DateTime)DatePickerValidFrom.SelectedDate;
                }
                return;
            }

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", Convert.ToString((long)ContextObjectId));
            Dto.UserImage UserProfile = DomainApiClient<Dto.UserImage>.GetByAction<Dto.UserImage>
                (DomainApiAction.UserImage.GetImageByUserId, parameter);

            if (UserProfile != null)
            {
                this.UserProfileId = UserProfile.Id;
                if (UserProfile.Image != null && UserProfile.Image.Length > 0)
                {
                    this.UploadedFileName = UserProfile.Image;
                    string data = Encoding.UTF8.GetString(UserProfile.Image);
                    ProfileImg.Src = "~/Profile/ImageUploadedFiles/" + data;
                }
                else
                {
                    ProfileImg.Attributes.Add("src", "../../images/profile-placeholder.png");

                }
            }

            EmployeeNo.Enabled = false;

            Dto.User ouser = DomainApiClient<Dto.User>.GetById((long)this.ContextObjectId);
            if (!String.IsNullOrEmpty(ouser.NRIC))
            {
                IdentificationNo.Text = Convert.ToString(ouser.NRIC);
            }
            if (!String.IsNullOrEmpty(ouser.EmployeeId))
            {
                EmployeeNo.Text = Convert.ToString(ouser.EmployeeId);
            }

            DisplayName.Text = Convert.ToString(ouser.DisplayName);
            UserName.Text = Convert.ToString(ouser.UserName);
            FirstName.Text = Convert.ToString(ouser.FirstName);
            LastName.Text = Convert.ToString(ouser.LastName);
            TelephoneNo.Text = Convert.ToString(ouser.TelephoneNumber);


            if (ouser.UserExtensionObj != null)
            {
                CompanyNameAutoCompleteBox.Text = ouser.UserExtensionObj.CompanyName;
                CompanyNameAutoCompleteBox.Attributes["CompanyValue"] = ouser.UserExtensionObj.CompanyId + "-" + ouser.UserExtensionObj.CompanyCode;
                CostCenterAutoCompleteBox.Text = ouser.UserExtensionObj.CostCenter;
                DepartmentAutoCompleteBox.Text = ouser.UserExtensionObj.DepartmentName;
                DepartmentAutoCompleteBox.Attributes["DepartmentValue"] = ouser.UserExtensionObj.DepartmentId + "-" + ouser.UserExtensionObj.DepartmentCode;
                DivisionAutoCompleteBox.Text = ouser.UserExtensionObj.DivisionName;
                DivisionAutoCompleteBox.Attributes["DivisionValue"] = ouser.UserExtensionObj.DivisionId + "-" + ouser.UserExtensionObj.DivisionCode;
            }
            parameter.Clear();
            parameter.Add("orgID", Convert.ToString(ouser.OrganisationId));

            OrganisationAutoCompleteBox.Text = DomainApiClient<Dto.Organisation>.GetByAction<string>(DomainApiAction.Org.Organisation.GetOrgUnitPrefixById, parameter);
            OrganisationAutoCompleteBox.Attributes["OrganisationValue"] = ouser.OrganisationId + "-" +
                DomainApiClient<Dto.Organisation>.GetByAction<string>(DomainApiAction.Org.Organisation.GetOrgUnitPrefixById, parameter);
            PasswordValue = RSAHelper.DecryptData(ouser.Password);
            Password.Attributes.Add("value", PasswordValue);

            if (ouser.JoinDate != null)
                DatePickerValidFrom.SelectedDate = Convert.ToDateTime(ouser.JoinDate);
            if (ouser.TerminationDate != null && ouser.TerminationDate >= DatePickerValidTo.MinDate && ouser.TerminationDate <= DatePickerValidTo.MaxDate )
                DatePickerValidTo.SelectedDate = Convert.ToDateTime(ouser.TerminationDate);
            if (DatePickerValidFrom.SelectedDate == null)
            {
                DatePickerValidFrom.MinDate = DateTime.Now;
                DatePickerValidFrom.SelectedDate = DateTime.Now;
                DatePickerValidTo.MinDate = (DateTime)DatePickerValidFrom.SelectedDate;
            }
            else
            {
                DatePickerValidFrom.MinDate = (DateTime)DatePickerValidFrom.SelectedDate;
                DatePickerValidTo.MinDate = (DateTime)DatePickerValidFrom.SelectedDate;
            }
            EmployeeType emp = ouser.EmployeeType;
            if (IsFullTimeEmployee)
            {
                if (TypeofEmployee.Items.FindByText(Convert.ToString(EmployeeType.Permanent)) != null)
                    TypeofEmployee.Items.FindByText(Convert.ToString(EmployeeType.Permanent)).Selected = true;
                TypeofEmployee.Enabled = false;
            }
            else
            {
                if (User.EmployeeType != EmployeeType.None)
                    if (TypeofEmployee.Items.FindByText(Convert.ToString(User.EmployeeType)) != null)
                        TypeofEmployee.Items.FindByText(Convert.ToString(User.EmployeeType)).Selected = true;
            }

            if (ouser.Roles.Count > 0)
            {
                foreach (Dto.Role role in ouser.Roles)
                {
                    RoleList.Add(role);
                }
            }
            TelephoneNo.Text = Convert.ToString(ouser.TelephoneNumber);
            txtMobileNo.Text = Convert.ToString(ouser.MobileNumber);

            Password.Enabled = false;
            UserName.Enabled = false;
        }


        public void HidePasswordField()
        {
            frmPassword.Visible = false;
        }
        public bool SaveUser()
        {
            bool isErrorOccured = false;
            if (!IsEdit)
            {
                if ((IsIdAlreadyExists(EmployeeNo.Text.Trim())))
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Employee Number Already Exists");
                    isErrorOccured = true;
                    return isErrorOccured;
                }
                else if ((IsNRICAlreadyExists(IdentificationNo.Text.Trim())))
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Identification Number Already Exists");
                    isErrorOccured = true;
                    return isErrorOccured;
                }
            }
            Dto.User usr = new Dto.User();
            if (IsEdit)
            {
                usr = DomainApiClient<Dto.User>.GetById((long)this.ContextObjectId);
                if (usr != null && usr.NRIC != IdentificationNo.Text)
                {
                    if ((IsNRICAlreadyExists(IdentificationNo.Text.Trim())))
                    {
                        MasterPage.ShowAlert(AlertType.Warning, "Identification Number Already Exists");
                        isErrorOccured = true;
                        return isErrorOccured;
                    }
                }
            }
            
            if (!string.IsNullOrEmpty(this.DatePickerValidFrom.SelectedDate.ToString()))
            {
                DateTime dt2 = Convert.ToDateTime(this.DatePickerValidFrom.SelectedDate);
                usr.JoinDate = dt2;
            }
            else
            {
                MasterPage.ShowAlert(AlertType.Warning, "Please enter Join Date");
                isErrorOccured = true;
                return isErrorOccured;
            }
            if (!string.IsNullOrEmpty(this.DatePickerValidTo.SelectedDate.ToString()))
            {
                DateTime dt1 = Convert.ToDateTime(this.DatePickerValidTo.SelectedDate);
                usr.TerminationDate = dt1;
            }
            if (usr.JoinDate != null && usr.TerminationDate != null)
            {
                if (usr.TerminationDate < usr.JoinDate)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Valid To Date should be greater then Join Date.");
                    isErrorOccured = true;
                    return isErrorOccured;
                }
            }
            UserExtension userExt = new UserExtension();
            usr.NRIC = IdentificationNo.Text;
            usr.DisplayName = DisplayName.Text;
            usr.FirstName = FirstName.Text;
            usr.LastName = LastName.Text;
            if(string.IsNullOrEmpty(UserName.Text))
            {
                MasterPage.ShowAlert(AlertType.Warning, "UserName Should Not Empty.");
                return true;
            }
            usr.UserName = UserName.Text;

            if (!IsEdit)
                usr.AuthenticationMode = AuthenticationMode.IACM.ToString();

            if ((ReferenceUsersAutoCompleteBox.Entries.Count > 0))
            {
                long ReferenceUserId = 0;
                if (ReferenceUsersAutoCompleteBox.Entries.Count > 0)
                {
                    ReferenceUserId = Convert.ToInt64(ReferenceUsersAutoCompleteBox.Entries[0].Value.Split('-')[0]);
                }

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", Convert.ToString(ReferenceUserId));


                var ReferenceUser = DomainApiClient<Dto.User>.GetByAction<Dto.User>
                    (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, parameter);

                if (ReferenceUser.UserExtensionObj != null)
                {
                    userExt.CompanyCode = ReferenceUser.UserExtensionObj.CompanyCode;
                    userExt.CompanyId = ReferenceUser.UserExtensionObj.CompanyId;
                    userExt.CompanyName = ReferenceUser.UserExtensionObj.CompanyName;
                    userExt.OrgCompany = ReferenceUser.UserExtensionObj.OrgCompany;

                    userExt.DepartmentCode = ReferenceUser.UserExtensionObj.DepartmentCode;
                    userExt.DepartmentId = ReferenceUser.UserExtensionObj.DepartmentId;
                    userExt.DepartmentName = ReferenceUser.UserExtensionObj.DepartmentName;
                    userExt.OrgDepartment = ReferenceUser.UserExtensionObj.OrgDepartment;

                    userExt.DivisionCode = ReferenceUser.UserExtensionObj.DivisionCode;
                    userExt.DivisionId = ReferenceUser.UserExtensionObj.DivisionId;
                    userExt.DivisionName = ReferenceUser.UserExtensionObj.DivisionName;
                    userExt.OrgDivision = ReferenceUser.UserExtensionObj.OrgDivision;

                    userExt.CostCenter = ReferenceUser.UserExtensionObj.CostCenter;
                    userExt.CostCenterText = ReferenceUser.UserExtensionObj.CostCenterText;


                    userExt.SectionCode = ReferenceUser.UserExtensionObj.SectionCode;
                    userExt.SectionName = ReferenceUser.UserExtensionObj.SectionName;
                    userExt.OrgSection = ReferenceUser.UserExtensionObj.OrgSection;

                    userExt.OrgBranch = ReferenceUser.UserExtensionObj.OrgBranch;
                    userExt.Supervisor = ReferenceUser.UserExtensionObj.Supervisor;
                }
                usr.OrganisationId = ReferenceUser.OrganisationId;
                userExt.EmployeeStatus = "1";
               
                usr.ManagerId = ReferenceUser.ManagerId;


            }
            if ((UIMode == UIMode.Edit || IsEdit) && ReferenceUsersAutoCompleteBox.Entries.Count == 0)
            {
                usr.UserExtension = usr.UserExtension;
                usr.OrganisationId = usr.OrganisationId;
            }
            else
            {
                usr.UserExtension = userExt.Serialize();
            }
            usr.EmployeeType = (EmployeeType)Convert.ToInt32(TypeofEmployee.SelectedValue);

            usr.Ethinicity = Convert.ToInt16(Ethinicity.Others).ToString();
            usr.FullTimeEmployee = usr.EmployeeType == EmployeeType.Permanent ? true : false;

            usr.IsActive = true;
            usr.KnownAs = DisplayName.Text;
            usr.LastLogonDate = DateTime.Now;
            usr.CreatedBy = SessionUser.User.UserName;
            usr.CreatedDate = DateTime.Now;

            if (IsEdit)
            {
                getUserId = usr.Id = Convert.ToInt64(ContextObjectId);
                usr.EmployeeId = EmployeeNo.Text;
                bool InitiatePropagation = usr.TelephoneNumber == TelephoneNo.Text ? false : true;
                usr.TelephoneNumber = TelephoneNo.Text;
                usr.MobileNumber = txtMobileNo.Text;
                usr.Password = RSAHelper.EncryptData(PasswordValue);
                usr.EmailId = DomainApiClient<Dto.User>.GetById((long)getUserId).EmailId;

                if (RoleList.Count > 0)
                {
                    List<Dto.Role> lstRoles = new List<Dto.Role>();
                    foreach (Dto.Role role in RoleList)
                    {
                        Dto.Role Extrole = DomainApiClient<Dto.Role>.GetById((long)usr.ManagerId);
                        lstRoles.Add(Extrole);
                    }
                    usr.Roles = lstRoles;
                }
                else
                {
                    if (RoleList.Count == 0)
                    {
                        Dto.Role role = DomainApiClient<Dto.Role>.GetById((long)MasterConfigHelper.RoleUser);
                        List<Dto.Role> lstRoles = new List<Dto.Role>();
                        lstRoles.Add(role);
                        usr.Roles = lstRoles;
                    }
                }
                DomainApiClient<Dto.User>.Save(usr);
                this.TrackEventTriggers(usr, InitiatePropagation);
            }
            else
            {
                usr.Password = RSAHelper.EncryptData(Password.Text);
                if (EmployeeNo.Text == string.Empty)
                    usr.EmployeeId = SetEmployeeNo();
                else
                    usr.EmployeeId = EmployeeNo.Text;
                usr.TelephoneNumber = TelephoneNo.Text;
                usr.MobileNumber = txtMobileNo.Text;
                long RoleId = MasterConfigHelper.RoleUser;
                Dto.Role role = DomainApiClient<Dto.Role>.GetById(RoleId);
                List<Dto.Role> lstRoles = new List<Dto.Role>();
                lstRoles.Add(role);
                usr.Roles = lstRoles;
                DomainApiClient<Dto.User>.Save(usr);

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("nric", IdentificationNo.Text);
                Dto.User existingUser = DomainApiClient<Dto.User>.GetByAction<Dto.User>(DomainApiAction.User.GetByNRIC, parameter);
                parameter.Clear();
                //parameter.Add("userID", Convert.ToString(existingUser.Id));
                // parameter.Add("roleID", Convert.ToString(RoleId));
                //DomainApiClient<Dto.User>.GetByAction<Dto.User>(DomainApiAction.User.InsertUserRole, parameter);
                this.TrackEventTriggers(usr, false);
                usr.Id = existingUser.Id;
                ViewState[V_USER] = usr;
            }
            if (usr.Id != 0)
            {
                Dto.UserImage userImage = new Dto.UserImage();
                userImage.Id = this.UserProfileId;
                userImage.UserId = usr.Id;
                userImage.Image = this.UploadedFileName;
                DomainApiClient<Dto.UserImage>.Save(userImage);
            }
            return isErrorOccured;
        }
        private bool IsIdAlreadyExists(string employeeno)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("username", employeeno);
            return DomainApiClient<Dto.User>.GetByAction<bool>(DomainApiAction.User.DoesEmployeeExists, parameter);
        }
        private bool IsNRICAlreadyExists(string identificationNumber)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("identificationNumber", identificationNumber);
            return DomainApiClient<Dto.User>.GetByAction<bool>(DomainApiAction.User.DoesIdentificationNumberExists, parameter);
        }
        protected void ConstructUserName(object sender, EventArgs e)
        {
            if (!IsEdit)
            {
                UserName.Text = ConstructUserName();
            }
        }

        #endregion

        #region Events

        protected void ReferenceUsersAutoCompleteBox_EntryAdded(object sender, Telerik.Web.UI.AutoCompleteEntryEventArgs e)
        {
            SetCompanyDetails(ReferenceUsersAutoCompleteBox.Entries[0].Value);
        }
        protected void fileUploadField_FileUploaded(object sender, Telerik.Web.UI.FileUploadedEventArgs e)
        {
            try
            {
                string path = Server.MapPath("~/Profile/ImageUploadedFiles/");
                string FileName = e.File.GetName();
                if (!(string.IsNullOrEmpty(FileName)))
                {
                    e.File.SaveAs(path + FileName);
                    byte[] data = Encoding.UTF8.GetBytes(FileName);
                    this.UploadedFileName = data;
                    ProfileImg.Src = "~/Profile/ImageUploadedFiles/" + FileName;
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        protected void imgDelete_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            this.UploadedFileName = null;
            ProfileImg.Src = "../../images/profile-placeholder.png";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if ((ViewState["PASSWORD"] == null) || (ViewState["PASSWORD"].ToString() == ""))
            {
                ViewState["PASSWORD"] = Password.Text.ToString();
                Password.Attributes.Add("value", PasswordValue);
            }
            else if (ViewState["PASSWORD"] != null)
            {
                Password.Attributes.Add("value", PasswordValue);
            }


        }
        #endregion

        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Employee/List.aspx");
        }
    }
}