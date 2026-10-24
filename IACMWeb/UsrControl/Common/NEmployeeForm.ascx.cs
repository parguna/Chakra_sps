using Application;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class NEmployeeForm : UIBaseUC
    {
        #region Members
        private const long V_COMPANY = 1;
        private const long V_COSTCENTER = 5;
        private const string V_ISEDIT = "ISEDIT";
        private const string V_OldEffectiveDate = "V_OldEffectiveDate";
        private const string V_RequestStatus = "V_RequestStatus";
        private const string V_ISAPPROVEPAGE = "ISAPPROVEPAGE";
        private const string V_USER = "USER";
        private const string V_ROLE = "ROLE";
        private const string V_FULLTIMEEMPLOYEE = "FULLTIMEEMPLOYEE";
        private const string V_ERROROCURRING = "ERROROCURRING";
        private const string V_ERRORMESSAGE = "ERRORMESSAGE";
        private const string V_FILENAME = "FILENAME";
        private const string V_PROFILEID = "PROFILEID";
        private const string V_EMPLOYEEUSERID = "EMPLOYEEUSERID";
        private const string V_PASS = "PASSWORD";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        private const string V_MANAGER = "MANAGER";

        private SessionStatusImpl SessionUser = new SessionStatusImpl();
        private long getUserId = 0;
        PasswordPolicyValidator passwordPolicyValidator = new PasswordPolicyValidator();
        private User savedUser;
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
        public DateTime VSOldEffectiveDate
        {
            get
            {
                if (ViewState[V_OldEffectiveDate] == null)
                    return DateTime.MinValue;
                return (DateTime)ViewState[V_OldEffectiveDate];
            }
            set
            {
                ViewState[V_OldEffectiveDate] = value;
            }
        }
        
        public string RequestStatus
        {
            get
            {
                return Convert.ToString(ViewState[V_RequestStatus]);
            }
            set
            {
                ViewState[V_RequestStatus] = value;
            }
        }

        public bool IsApprovePage
        {
            get
            {
                if (ViewState[V_ISAPPROVEPAGE] == null)
                    return false;
                return (bool)ViewState[V_ISAPPROVEPAGE];
            }
            set
            {
                ViewState[V_ISAPPROVEPAGE] = value;
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

        public long EmployeeUserId
        {
            get
            {
                if (ViewState[V_EMPLOYEEUSERID] == null)
                    return 0;
                return (long)ViewState[V_EMPLOYEEUSERID];
            }
            set
            {
                ViewState[V_EMPLOYEEUSERID] = value;
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
        public string UserDisplayName
        {
            get
            {
                return txtDisplayName.Text.Trim();    
            }
        }
        public string UserEmailId
        {
            get
            {
                return txtEmail.Text.Trim();
            }
        }
        public string UserProjectDescription
        {
            get
            {
                return txtProjectDescription.Text.Trim();
            }
        }
        public string UserNonEmployeeCompany
        {
            get
            {
                return txtCompanyOfNonEmployee.Text.Trim();
            }
        }
        //public string PasswordValue
        //{
        //    get
        //    {
        //        if (ViewState[V_PASS] == null)
        //            return Password.Text;
        //        return (string)ViewState[V_PASS];
        //    }
        //    set
        //    {
        //        ViewState[V_PASS] = value;

        //    }
        //}

        public bool IsErrorOccuring
        {
            get
            {
                if (ViewState[V_ERROROCURRING] == null)
                    return false;
                return (bool)ViewState[V_ERROROCURRING];
            }
            set
            {
                ViewState[V_ERROROCURRING] = value;
            }
        }

        public string ErrorMessage
        {
            get
            {
                if (ViewState[V_ERRORMESSAGE] == null)
                    return "";
                return (string)ViewState[V_ERRORMESSAGE];
            }
            set
            {
                ViewState[V_ERRORMESSAGE] = value;
            }
        }
        //public string NRIC
        //{
        //    get
        //    {
        //        return this.txtIdentificationNo.Text.Trim();
        //    }
        //    set
        //    {
        //        this.txtIdentificationNo.Text = value;
        //    }
        //}
        #endregion

        #region Methods
        public void Initialize()
        {
            BindEmployeeType();
            SetPage();
        }

        public void HideReferenceUser()
        {
            SupervisorUser.Visible = false;
        }

        public void HideAstricks()
        {
            //txtIdentificationNo.Attributes.Remove("required");
            //txtEmployeeNo.Attributes.Remove("required");
            txtDisplayName.Attributes.Remove("required");
            //FirstName.Attributes.Remove("required");
            //Password.Attributes.Remove("required");
        }

        public void DisableDateFields()
        {
            DatePickerValidFrom.Enabled = false;
            DatePickerValidTo.Enabled = false;
        }

        //public void CheckSelfUpdateField()
        //{
        //    Dictionary<string, string> paramNRIC = new Dictionary<string, string>();
        //    paramNRIC.Add("attributeName", "NRIC");
        //    //txtIdentificationNo.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.
        //    //    GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, paramNRIC);

        //    Dictionary<string, string> ParamDisplayName = new Dictionary<string, string>();
        //    ParamDisplayName.Add("attributeName", "DisplayName");
        //    txtDisplayName.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.
        //        GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamDisplayName);

        //    Dictionary<string, string> ParamFirstName = new Dictionary<string, string>();
        //    ParamFirstName.Add("attributeName", "FirstName");
        //    //FirstName.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.
        //    //    GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamFirstName);

        //    Dictionary<string, string> ParamLastName = new Dictionary<string, string>();
        //    ParamLastName.Add("attributeName", "LastName");
        //    //LastName.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.
        //    //    GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamLastName);

        //    Dictionary<string, string> ParamUserName = new Dictionary<string, string>();
        //    ParamUserName.Add("attributeName", "ParamUserName");
        //    txtIdentificationNo.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.
        //        GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamUserName);

        //    Dictionary<string, string> ParamCompanyName = new Dictionary<string, string>();
        //    ParamCompanyName.Add("attributeName", "ParamCompanyName");
        //    ReferenceUsersAutoCompleteBox.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.
        //        GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamCompanyName);

        //    Dictionary<string, string> ParamCostCenter = new Dictionary<string, string>();
        //    ParamCostCenter.Add("attributeName", "CostCenter");
        //    ReferenceUsersAutoCompleteBox.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.
        //        GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamCostCenter);

        //    Dictionary<string, string> ParamDepartmentName = new Dictionary<string, string>();
        //    ParamDepartmentName.Add("attributeName", "DepartmentName");
        //    ReferenceUsersAutoCompleteBox.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.
        //        GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamDepartmentName);

        //    Dictionary<string, string> ParamDivisionName = new Dictionary<string, string>();
        //    ParamDivisionName.Add("attributeName", "DivisionName");
        //    txtIdentificationNo.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.
        //        GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamDivisionName);

        //    Dictionary<string, string> ParamOrgCompany = new Dictionary<string, string>();
        //    ParamOrgCompany.Add("attributeName", "OrgCompany");
        //    txtIdentificationNo.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.
        //        GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamOrgCompany);
        //    //UserName.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamUserName);

        //    Dictionary<string, string> ParamJoinDate = new Dictionary<string, string>();
        //    ParamJoinDate.Add("attributeName", "JoinDate");
        //    txtIdentificationNo.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.
        //        GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamJoinDate);
        //    DatePickerValidFrom.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamJoinDate);

        //    Dictionary<string, string> ParamTerminationDate = new Dictionary<string, string>();
        //    ParamTerminationDate.Add("attributeName", "TerminationDate");
        //    txtIdentificationNo.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.
        //        GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamTerminationDate);
        //    DatePickerValidTo.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamTerminationDate);

        //    Dictionary<string, string> ParamEmployeeType = new Dictionary<string, string>();
        //    ParamEmployeeType.Add("attributeName", "EmployeeType");
        //    TypeofEmployee.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.
        //        GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamEmployeeType);

        //    Dictionary<string, string> ParamTelephoneNumber = new Dictionary<string, string>();
        //    ParamTelephoneNumber.Add("attributeName", "TelephoneNumber");
        //    txtTelephoneNo.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.
        //        GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamTelephoneNumber);

        //    //Dictionary<string, string> ParamMobileNumber = new Dictionary<string, string>();
        //    //ParamMobileNumber.Add("attributeName", "MobileNumber");
        //    //txtMobileNo.Enabled = DomainApiClient<Dto.ConfigGeneralTrustedSource>.
        //    //    GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.IsSelfUpdated, ParamMobileNumber);

        //    //OrgSearchUC.Disabled = !ReferenceUsersAutoCompleteBox.Enabled;
        //    // DesignationSearchUC.Disabled = !ReferenceUsersAutoCompleteBox.Enabled;
        //    //DepartmentAutoCompleteBox.Disabled = !ReferenceUsersAutoCompleteBox.Enabled;
        //}

        private void BindEmployeeType()
        {
            TypeofEmployee.Items.Clear();
            TypeofEmployee.Items.Add(new ListItem("--Select--", "0"));
            foreach (EmployeeType item in Enum.GetValues(typeof(EmployeeType)))
            {
                var description = UtilityMethods.GetEnumAttribute<Description>(item);
                ListItem listItem = new ListItem(description, ((byte)item).ToString());
                if (item == EmployeeType.Permanent)
                    continue;
                TypeofEmployee.Items.Add(listItem);
            }
        }
        //private string SettxtEmployeeNo()
        //{
        //    string empID = null;
        //    string defaultEmployeeKey = MasterConfigHelper.EmployeeDefaultKey;
        //    Dictionary<string, string> parameter = new Dictionary<string, string>();
        //    parameter.Add("searchString", defaultEmployeeKey);
        //    string EmpId = DomainApiClient<Dto.User>.GetByAction<string>(DomainApiAction.User.GetEmployeeId, parameter);

        //    if (EmpId == null)
        //    {
        //        txtEmployeeNo.Text = defaultEmployeeKey + "000000";
        //        empID = txtEmployeeNo.Text;
        //    }
        //    else
        //    {
        //        empID = EmpId.Replace(defaultEmployeeKey, "").Trim();
        //        txtEmployeeNo.Text = defaultEmployeeKey + (Convert.ToInt64(empID) + 1).ToString("000000");
        //        empID = txtEmployeeNo.Text;
        //    }
        //    return empID;
        //}
        private void CreateNonEmployeeCreationTrigger(User user, bool includeInactive = false,User inActiveUser= null)
        {
            try
            {
                var trigger = GetExistingTrigger(user.Id, EmployeeEvents.NonEmployeeCreation);

                if (trigger == null && includeInactive)
                    trigger = GetExistingTriggerIncludingInactive(user.Id, EmployeeEvents.NonEmployeeCreation);

                if (trigger == null)
                    trigger = new EmployeeEventTrigger();

                trigger.UserId = user.Id;
                trigger.EventType = EmployeeEvents.NonEmployeeCreation;
                trigger.CreatedBy = SessionUser.User.UserName;
                trigger.CreatedDate = DateTime.Now;
                trigger.IsActive = false;
                trigger.EffectiveDate = user.JoinDate ?? DateTime.Now;
                trigger.IsAdminCheckRequired = false;
                trigger.RequestStatus = AccessRequestStatus.Submitted;
                trigger.IsRequestCreated = false;
                if (inActiveUser != null)
                {
                    trigger.OldUserDetails = UtilityMethods.Serialize(inActiveUser);
                }
                else
                {
                    trigger.OldUserDetails = UtilityMethods.Serialize(inActiveUser);
                }
                trigger.UserDetails = UtilityMethods.Serialize(user);
                //trigger.UserDetails = UtilityMethods.Serialize(user);
                SaveAndWriteLog(trigger, user);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void CreateNonEmployeeCreationTrigger(User user,User activeUser)
        {
            try
            {
                EmployeeEventTrigger trigger = new EmployeeEventTrigger();
                trigger.UserId = user.Id;
                trigger.EventType = EmployeeEvents.NonEmployeeCreation;
                trigger.CreatedBy = SessionUser.User.UserName;
                trigger.CreatedDate = DateTime.Now;
                trigger.IsActive = false;
                trigger.EffectiveDate = user.JoinDate ?? DateTime.Now;
                trigger.IsAdminCheckRequired = false;
                trigger.RequestStatus = AccessRequestStatus.Submitted;
                trigger.IsRequestCreated = false;
                trigger.OldUserDetails = UtilityMethods.Serialize(activeUser);
                trigger.UserDetails = UtilityMethods.Serialize(user);
                SaveAndWriteLog(trigger, user);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SaveAndWriteLog(EmployeeEventTrigger trigger, User user)
        {
            bool triggerCreated = DomainApiClient<EmployeeEventTrigger>.GetByAction<bool>(DomainApiAction.Provisioning.EmployeeEventTrigger.CreateEmployeeEventTrigger, null, trigger);
            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, string.Format("{0} trigger for user {1} is created by {2}", UtilityMethods.GetEnumAttribute<EmployeeEventTrigger>(trigger.EventType), user.DisplayName, SessionUser.User.UserName));
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
        Dto.EmployeeEventTrigger GetExistingTriggerIncludingInactive(long userId, EmployeeEvents employeeEvent)
        {
            Dictionary<string, string> parameterEvent = new Dictionary<string, string>();
            parameterEvent.Add("userId", Convert.ToString(userId));
            parameterEvent.Add("evnt", employeeEvent.ToString());

            var tempTrigger = DomainApiClient<Dto.EmployeeEventTrigger>.GetByAction<Dto.EmployeeEventTrigger>
                (DomainApiAction.Provisioning.EmployeeEventTrigger.GetByUserAndEventIncludingInactive, parameterEvent);
            return tempTrigger;
        }
        //private void CreateOffBoardTrigger(User user)
        //{
        //    try
        //    {
        //        var evnt = EmployeeEvents.Resignation;
        //        EmployeeEventTrigger trigger = GetExistingTrigger(user.Id, evnt);

        //        if (trigger != null)
        //        {
        //            trigger.EffectiveDate = user.TerminationDate ?? DateTime.Now;
        //            trigger.LastModifiedBy = trigger.CreatedBy;
        //            trigger.LastModifiedDate = DateTime.Now;
        //        }
        //        else
        //        {
        //            trigger = new EmployeeEventTrigger()
        //            {
        //                CreatedBy = SessionUser.User.UserName,
        //                CreatedDate = DateTime.Now,
        //                EffectiveDate = user.TerminationDate ?? DateTime.Now,
        //                EventType = evnt,
        //                IsRequestCreated = false,
        //                UserId = user.Id,
        //                IsActive = false,
        //                RequestStatus = AccessRequestStatus.Submitted
        //            };
        //        }

        //        SaveAndWriteLog(trigger, user);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        private void CreateExtensionTrigger(User user, User existingUser, bool includeInactive = false, bool isTerminationDateChanged = false, bool isDisplayNameChanged = false)
        {
            try
            {
                var oldTrigger = new EmployeeEventTrigger()
                {
                    CreatedBy = SessionUser.User.UserName,
                    CreatedDate = DateTime.Now,
                    EffectiveDate = DateTime.Now,
                    //EffectiveDate = DatePickerValidTo.SelectedDate.Value,
                    OldEffectiveDate = VSOldEffectiveDate,
                    EventType = EmployeeEvents.NonEmployeeExtension,
                    IsActive = false,
                    IsAdminCheckRequired = false,
                    RequestStatus = AccessRequestStatus.Submitted,
                    IsRequestCreated = false,
                    UserId = user.Id,
                    UserDetails = UtilityMethods.Serialize(user),
                    OldUserDetails = UtilityMethods.Serialize(existingUser),
                };
                if (isTerminationDateChanged)
                {
                    oldTrigger.OldEffectiveDate = VSOldEffectiveDate;
                }
                SaveAndWriteLog(oldTrigger, user);
            }
            catch
            {
                throw;
            }
        }
        private string removeSpecialCharacters(string inPutStr, bool isRemoveSpace)
        {
            StringBuilder outPutSB = new StringBuilder();
            char[] spChars = new char[] { '~', '`', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '+', '=', '{', '}', '[', ']', '|', '\\', '<', '>', '?', ',', '.', '/', '\'' };

            inPutStr = inPutStr != null ? inPutStr.Trim() : string.Empty;
            if (inPutStr.Length > 4 && inPutStr.StartsWith("Bin ", StringComparison.CurrentCultureIgnoreCase))
            {
                inPutStr = inPutStr.Substring(4, inPutStr.Length - 4);
            }
            if (inPutStr.Length > 4 && inPutStr.StartsWith("bte ", StringComparison.CurrentCultureIgnoreCase))
            {
                inPutStr = inPutStr.Substring(4, inPutStr.Length - 4);
            }
            if (inPutStr.Length > 6 && inPutStr.StartsWith("binte ", StringComparison.CurrentCultureIgnoreCase))
            {
                inPutStr = inPutStr.Substring(6, inPutStr.Length - 6);
            }

            outPutSB.Append(inPutStr);

            outPutSB.Replace("S/O", string.Empty);
            outPutSB.Replace("S/o", string.Empty);
            outPutSB.Replace("s/O", string.Empty);
            outPutSB.Replace("s/o", string.Empty);
            outPutSB.Replace("s o", string.Empty);
            outPutSB.Replace("S O", string.Empty);

            outPutSB.Replace("D/O", string.Empty);
            outPutSB.Replace("D/o", string.Empty);
            outPutSB.Replace("d/O", string.Empty);
            outPutSB.Replace("d/o", string.Empty);
            outPutSB.Replace("d o", string.Empty);
            outPutSB.Replace("D O", string.Empty);

            foreach (char spChar in spChars)
            {
                outPutSB.Replace(spChar, ' ');
            }

            if (isRemoveSpace)
            {
                outPutSB.Replace(" ", string.Empty);
            }
            else
            {
                outPutSB.Replace("   ", string.Empty);
                outPutSB.Replace("  ", string.Empty);
            }

            return outPutSB.ToString();
        }
        private string ConstructUserName()
        {
            string userName = string.Empty;
            StringBuilder userNameSB = new StringBuilder();

            try
            {
                string DisplayName = this.UserDisplayName.Trim();
                userName = removeSpecialCharacters(DisplayName, false);
                userName.TrimStart();

                var firstLetter = Enum.GetName(typeof(EmployeeType), empType(TypeofEmployee.SelectedItem.Value.Trim()))[0];
                userNameSB.Append(firstLetter);

                if (!string.IsNullOrEmpty(userName)) // && userName.Length >= 3)
                {
                    string[] nameAry = userName.Split(' ');
                    if (nameAry.Length == 1)
                    {
                        if (nameAry[0].Length >= 3)
                            userNameSB.Append(nameAry[0].Substring(0, 3));
                        else
                            userNameSB.Append(nameAry[0]);
                    }
                    else if (nameAry.Length == 2)
                    {
                        if (nameAry[0].Length >= 2)
                        {
                            if (nameAry[0] != "")
                            {
                                userNameSB.Append(nameAry[0].Substring(0, 2));
                            }
                            userNameSB.Append(nameAry[1].Substring(0, 1));
                        }
                        else if (nameAry[1].Length >= 2)
                        {
                            if (nameAry[0] != "")
                            {
                                userNameSB.Append(nameAry[0].Substring(0, 1));
                            }
                            userNameSB.Append(nameAry[1].Substring(0, 2));
                        }
                        else
                        {
                            userNameSB.Append(nameAry[0]);
                            userNameSB.Append(nameAry[1]);
                        }
                    }
                    else if (nameAry.Length >= 3)
                    {
                        if (nameAry[0] != "")
                        {
                            userNameSB.Append(nameAry[0].Substring(0, 1));
                        }
                        userNameSB.Append(nameAry[1].Substring(0, 1));
                        userNameSB.Append(nameAry[2].Substring(0, 1));
                    }
                }

                userNameSB.Append(GetLastFourChars());

                userName = userNameSB.ToString().ToUpper();

                userName = this.CheckUserNameExists(userName);
            }
            catch
            {
                throw;
            }

            return userName;
        }

        string GetLastFourChars()
        {
            var lastStoredUsername = MasterConfigHelper.LAST_ASSIGNED_USERNAME_SEQUENCE;
            var nextUsernameSequence = "001A";
            if (lastStoredUsername.Key == 0)
                return nextUsernameSequence;

            var number = Convert.ToInt32(lastStoredUsername.Value.Substring(0, 3)) + 1;
            var lastChar = lastStoredUsername.Value[lastStoredUsername.Value.Length - 1];
            if (number < 1000)
            {
                nextUsernameSequence = number.ToString("D3") + lastChar;
            }
            else
            {
                lastChar++;
                nextUsernameSequence = "001" + lastChar++;
            }
            return nextUsernameSequence;
        }

        private string CheckUserNameExists(string userName)
        {
            if (userName.Length > 8)
                userName = userName.Substring(0, 8);

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("sAMAccountName", userName);
            parameter.Add("endpointsystemId", "1");
            bool isExist = DomainApiClient<Dto.ADSUserInfo>.GetByAction<bool>
                                        (DomainApiAction.Endpoint.ActiveDirectory.ADSUserInfo.DoesExists, parameter);

            var isExistInUser = UserNameExists(userName);
            while (isExist || isExistInUser)
            {
                SaveUsernameSequence(userName);

                userName = ConstructUserName();
                parameter.Clear();
                parameter.Add("sAMAccountName", userName);
                parameter.Add("endpointsystemId", "1");

                isExist = DomainApiClient<Dto.ADSUserInfo>.GetByAction<bool>
                                        (DomainApiAction.Endpoint.ActiveDirectory.ADSUserInfo.DoesExists, parameter);

                isExistInUser = UserNameExists(userName);
            }

            SaveUsernameSequence(userName);
            return userName;
        }

        void SaveUsernameSequence(string username)
        {
            var value = username.Substring(username.Length - 4);
            var existing = MasterConfigHelper.LAST_ASSIGNED_USERNAME_SEQUENCE;
            var config = new ConfigurationMaster();
            config.Id = existing.Key;
            config.ConfigKey = ConfigConstants.LAST_ASSIGNED_USERNAME_SEQUENCE;
            config.ConfigValue = value;
            config.IsUiDisplay = false;
            config.CreatedBy = MasterConfigHelper.ServiceUser;
            config.CreatedDate = DateTime.Now;
            config.Description = "Last Assigned Username Sequence";

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("product", Convert.ToString(Product.IACM));
            DomainApiClient<Dto.ConfigurationMaster>.GetByAction<object>
            (DomainApiAction.Configuration.ConfigurationMaster.SaveConfiguration, parameter, config);
        }

        private bool doExists(string userName)
        {
            bool isExists = true;
            try
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("userName", userName);
                bool isExist = DomainApiClient<Dto.User>.GetByAction<bool>
                                            (DomainApiAction.User.DoesADIDExists, parameter);
                return isExist;
            }
            catch (Exception ex)
            {

            }
            return isExists;
        }
        private string GetempType(EmployeeType emptype)
        {
            try
            {
                switch (emptype)
                {
                    //case EmployeeType.None: return "None";
                    case EmployeeType.Contractor: return "Third Party Contractor";
                    //case EmployeeType.Intern: return "Intern";
                    case EmployeeType.Vendor: return "Vendor";
                    //case EmployeeType.SDCUser: return "SDC User";
                    //case EmployeeType.SPClub: return "SP Club";
                    case EmployeeType.TempStaff: return "Temp or Contact Staff";
                    default: break;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return string.Empty;
        }
        private void SetPage()
        {
            clearAll();
            var msg = MasterConfigHelper.PDPAConsentStatement;
            if (!string.IsNullOrWhiteSpace(msg))
            {
                lblInformationMessage.Text = msg;
            }
            if (!IsEdit)
            {
                Dto.User user = new Dto.User();
                if (DatePickerValidFrom.SelectedDate == null)
                {
                    //DatePickerValidFrom.MinDate = DateTime.Now;
                    DatePickerValidFrom.SelectedDate = DateTime.Now;
                    DatePickerValidTo.MinDate = (DateTime)DatePickerValidFrom.SelectedDate > DateTime.Now ? (DateTime)DatePickerValidFrom.SelectedDate : DateTime.Now;
                }
                
                //PasswordValue = DomainApiClient<Dto.PasswordPolicy>.GetByAction<string>
                //            (DomainApiAction.Configuration.PasswordPolicy.ConstructGeneralUserPassword, null);
                //Password.Attributes.Add("value", PasswordValue);
                divExtension.Visible = false;
                return;
            }
            EmployeeUserId = (long)ContextObjectId;

            Dto.User usr = DomainApiClient<Dto.User>.GetById(EmployeeUserId);
            if (usr == null)
                return;

            if(usr.RequestStatus == AccessRequestStatus.Requested)                 
            {
                ShowAlertMessage(AlertType.Warning, "Non-Employee details cannot be updated as it is still pending for approval.");
                Response.Redirect("~/Login.aspx");
            }
            this.User = usr;
            //if (!String.IsNullOrEmpty(usr.IdentificationNo))
            //{
            //    txtIdentificationNo.Text = Convert.ToString(usr.IdentificationNo);
            //}

            //if (!String.IsNullOrEmpty(usr.EmployeeId))
            //{
            //    txtEmployeeNo.Text = Convert.ToString(usr.EmployeeId);
            //}
            //txtFirstName.Text = Convert.ToString(usr.FirstName);
            //txtLastName.Text = Convert.ToString(usr.LastName);
            txtDisplayName.Text = Convert.ToString(usr.DisplayName);
            //UserName.Text = Convert.ToString(ouser.UserName);
            txtTelephoneNo.Text = usr.TelephoneNumber;
            txtMobileNumber.Text = usr.MobileNumber;

            int Id = (int)Convert.ChangeType(usr.Id, typeof(int));
            ViewState["TempId"] = Id.ToString();

            //NRIC = usr.IdentificationNo?.ToString();
            //txtIdentificationNo.Enabled = false;
            //txtEmployeeNo.Text = usr.EmployeeId?.ToString();
            txtDisplayName.Text = usr.DisplayName.ToString();
            //txtUsername.Text = usr.UserName;

            this.txtEmail.Text = usr.EmailId;
            this.txtProjectDescription.Text = usr.ProjectDescription;
            var refuser = DomainApiClient<Dto.User>.GetById(usr.ReferenceUserId);
            if (refuser != null)
            {
                ReferenceUsersAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(refuser.DisplayName, refuser.Id.ToString()));
                //ReferenceUsersAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(refuser.DisplayName + " [" + refuser.UserName + "]", refuser.Id + "-" + refuser.Supervisor));
            }
            txtCompany.Text = usr.OrgCompany;
            txtCompany.Attributes["CompanyValue"] = usr.CompanyId + "-" + usr.CompanyCode;
            txtCostCenter.Text = usr.CostCenter;
            txtDepartment.Text = usr.OrgDepartment;
            txtDepartment.Attributes["DepartmentValue"] = usr.DepartmentId + "-" + usr.DepartmentCode;
            txtDivision.Text = usr.OrgBranch;
            txtDivision.Attributes["DivisionValue"] =   usr.BranchCode;
            txtUnit.Text = usr.OrgUnit;
            txtSection.Text = usr.OrgSection;
            txtOrgUnit.Text = getOrganisationCodeName(usr);
            txtOrgUnit.Attributes["OrganisationValue"] = usr.OrganisationCode;
            DatePickerValidTo.SelectedDate = usr.TerminationDate > DatePickerValidTo.MaxDate ? DatePickerValidTo.MaxDate : usr.TerminationDate;
            DatePickerValidTo.Enabled = false;
            var emptype = GetempType(usr.EmployeeType);
            TypeofEmployee.SelectedIndex = TypeofEmployee.Items.IndexOf(TypeofEmployee.Items.FindByText(emptype));
            txtCompanyOfNonEmployee.Text = usr.NCompany;
            DatePickerValidFrom.Enabled = false;
            DatePickerValidFrom.SelectedDate = usr.JoinDate;
            if (this.User != null && this.User.Id > 0)
            {
                TypeofEmployee.Enabled = false;
            }
            foreach (Dto.Role role in usr.Roles)
            {
                RoleList.Add(role);
            }
            //txtMobileNo.Text = Convert.ToString(ouser.MobileNumber);
            //ReferenceUsersAutoCompleteBox.Enabled = false;

            if (!IsApprovePage)
            {
                //txtDisplayName.Enabled = false;
                //txtEmployeeNo.Enabled = false;
                ////Password.Enabled = false;
                ////UserName.Enabled = false;
                //txtIdentificationNo.Enabled = false;
                //txtEmployeeNo.Enabled = false;
                ////FirstName.Enabled = false;
                ////LastName.Enabled = false;
                //TypeofEmployee.Enabled = false;
                ////DepartmentAutoCompleteBox.EnabledAutoCompleteBox = false;
                ////DesignationAutoCompleteBox.EnabledAutoCompleteBox = false;
                //DatePickerValidFrom.Enabled = false;
                //DatePickerValidTo.Enabled = false;
                //CompanyAutoCompleteBox.EnabledAutoCompleteBox = false;
                //UserName.Enabled = false;
                //btnCheck.Enabled = false;
            }
            else
            {
                //PhotoContainer.Visible = false;
                btn_Cancel.Visible = false;
                //photoDiv.Visible = false;
            }

            if (IsEdit)
            {
                //txtIdentificationNo.Enabled = false;
                txtCompany.Enabled = false;
                txtDepartment.Enabled = false;
                //txtEmployeeNo.Enabled = false;
                //txtUsername.Enabled = false;
                txtOrgUnit.Enabled = false;
                txtCostCenter.Enabled = false;
                txtDivision.Enabled = false;
                txtSection.Enabled = false;
                txtUnit.Enabled = false;
            }
        }

          string getOrganisationCodeName(User user)
        {
            var emptyCode = "00000000";
            if (!string.IsNullOrWhiteSpace(user.UnitCode) && user.UnitCode!= emptyCode)  return user.OrgUnit;
            else if (!string.IsNullOrWhiteSpace(user.SectionCode) && user.SectionCode != emptyCode) return user.OrgSection;
            else if (!string.IsNullOrWhiteSpace(user.BranchCode) && user.BranchCode != emptyCode) return user.OrgBranch;
            else if (!string.IsNullOrWhiteSpace(user.DepartmentCode) && user.DepartmentCode != emptyCode) return user.OrgDepartment;
            else return user.OrgCompany; 
        }
         
        public void HidePasswordField()
        {
            //frmPassword.Visible = false;
        }

        public void HideCompanyCode()
        {
            frmCompany.Visible = false;
        }

        public void HideOrgChartLink()
        {
            //OrgChartLink.Visible = false;
        }

        public void ShowPasswordPolicyButton(bool state)
        {
            //DisplayPolicies.Visible = state;
        }

        public void ShowOrgChartLink()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", SessionUser.User.Id.ToString());
            bool head = DomainApiClient<Dto.NOrganisationHead>.GetByAction<bool>
                            (DomainApiAction.Org.NOrganisationHead.IsOrgHead, parameter);
            //if (head)
            //    OrgChartLink.Visible = true;
            //else
            //    OrgChartLink.Visible = false;
        }
        bool UserNameExists(string username)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("name", username);

            var tmpUsr = DomainApiClient<Dto.User>.GetByAction<Dto.User>
                (DomainApiAction.User.GetByUserName, parameter);
            return tmpUsr != null;
        }
        private void clearAll()
        {
            //NRIC = string.Empty;
            //txtEmployeeNo.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtProjectDescription.Text = string.Empty;
            txtDisplayName.Text = string.Empty;
            //txtEmployeeNo.Text = string.Empty;
            txtTelephoneNo.Text = string.Empty;
            //txtUsername.Text = string.Empty;
            ReferenceUsersAutoCompleteBox.Entries.Clear();
            txtCompany.Text = string.Empty;
            txtCostCenter.Text = string.Empty;
            txtDepartment.Text = string.Empty;
            txtDivision.Text = string.Empty;
            txtOrgUnit.Text = string.Empty;
            txtUnit.Text = string.Empty;
            txtSection.Text = string.Empty;

            DatePickerValidFrom.Clear();
            DatePickerValidTo.Clear();
            TypeofEmployee.SelectedIndex = 0;
        }
        private bool Validate()
        {
            DateTime? fromDate = this.DatePickerValidFrom.SelectedDate;
            DateTime? toDate = this.DatePickerValidTo.SelectedDate;
            //if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            //{
            //    MasterPage.ShowAlert(AlertType.Warning, "Please enter the first name.");
            //    return false;
            //}
            //else if(string.IsNullOrWhiteSpace(txtLastName.Text))
            //{
            //    MasterPage.ShowAlert(AlertType.Warning, "Please enter the last name.");
            //    return false;
            //}
            if (string.IsNullOrWhiteSpace(this.UserDisplayName))
            {
                MasterPage.ShowAlert(AlertType.Warning, "Please enter the Name.");
                return false;
            }
            if (!Regex.IsMatch(this.UserDisplayName, @"^[a-zA-Z][0-9a-zA-Z @\/_'()-]*$"))
            {
                MasterPage.ShowAlert(AlertType.Warning, "Please enter the valid Name.");
                return false;
            }
            long userId = 0;
            if(ViewState["TempId"] != null)
            {
                userId = Convert.ToInt64(ViewState["TempId"]);
            }
            #region Check for Duplicate
            try
            {
                Dictionary<string, string> paramRole = new Dictionary<string, string>();
                paramRole.Add("userId", userId.ToString());
                paramRole.Add("displayName", this.UserDisplayName);
                var user = DomainApiClient<Dto.User>.GetByAction<Dto.User>(DomainApiAction.User.CheckDisplayNameDuplicate, paramRole);
                if(user != null)
                {
                    if (user.IsActive)
                    {
                        MasterPage.ShowAlert(AlertType.Error, $"Same Name already exists for Employee({user.EmployeeId})");
                        return false;
                    }
                    else
                    {
                        MasterPage.ShowAlert(AlertType.Error, $"Same Name already exists for Employee({user.EmployeeId}) which is InActive. Please update the exisiting details.");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info,$"Error while checking duplicate name for Non-Employee", ex);
                MasterPage.ShowAlert(AlertType.Warning, "Error occured while checking duplicate name");
                return false;
            }
            #endregion
            #region email check
            if (!string.IsNullOrWhiteSpace(this.UserEmailId))
            {
                try
                {
                    Dictionary<string, string> paramRole = new Dictionary<string, string>();
                    paramRole.Add("userId", userId.ToString());
                    paramRole.Add("emailId", this.UserEmailId);
                    var user = DomainApiClient<Dto.User>.GetByAction<Dto.User>(DomainApiAction.User.IsEmailIdAlreadyExist, paramRole);
                    if (user != null)
                    {
                        MasterPage.ShowAlert(AlertType.Error, $"EmailId already used by Employee({user.EmployeeId})");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, $"Error while checking duplicate EmailId for Non-Employee", ex);
                    MasterPage.ShowAlert(AlertType.Warning, "Error occured while checking duplicate EmailId");
                    return false;
                }
            }
            #endregion
            if (string.IsNullOrWhiteSpace(this.UserProjectDescription))
            {

                MasterPage.ShowAlert(AlertType.Warning, "Please enter the project/description.");
                return false;
            }
            else if (this.ReferenceUsersAutoCompleteBox.Entries.Count == 0)
            {
                MasterPage.ShowAlert(AlertType.Warning, "Please select the reference user.");
                return false;
            }
            
            else if (!fromDate.HasValue)
            {
                MasterPage.ShowAlert(AlertType.Warning, "Please select the onboard date.");
                return false;
            }
            else if (string.IsNullOrWhiteSpace(this.UserNonEmployeeCompany))
            {
                MasterPage.ShowAlert(AlertType.Warning, "Please enter the company of non employee.");
                return false;
            }
            else if (TypeofEmployee.SelectedValue == "0")
            {
                MasterPage.ShowAlert(AlertType.Warning, "Please select the employee type.");
                return false;
            }
            else if (!toDate.HasValue)
            {
                MasterPage.ShowAlert(AlertType.Warning, "Please select the Last Service Date.");
                return false;
            }
            //else if (!string.IsNullOrWhiteSpace(txtTelephoneNo.Text) && !UtilityMethods.IsValidTelephoneNumber(txtTelephoneNo.Text))
            //{
            //    MasterPage.ShowAlert(AlertType.Warning, "Please enter a valid telephone number.");
            //    return false;
            //}
            else if (string.IsNullOrWhiteSpace(txtMobileNumber.Text) || !UtilityMethods.IsValidMobileNumber(txtMobileNumber.Text))
            {
                MasterPage.ShowAlert(AlertType.Warning, "Please enter a valid Singapore mobile number (Length must be 8 characters only & Starting with either 8 or 9)");
                return false;
            }
            //else if (fromDate.Value.Date < DateTime.Today)
            //{
            //    MasterPage.ShowAlert(AlertType.Warning, "Join Date can not be earlier than current date.");
            //    return false;
            //}
            else if ((userId == 0 && toDate.Value.Date <= fromDate.Value.Date) || (userId > 0 && toDate.Value.Date < fromDate.Value.Date))
            {
                MasterPage.ShowAlert(AlertType.Warning, "Last Service Date cannot be earlier than or equal to Join Date.");
                return false;
            }

             


            return true;
        }

        protected void ExtensionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ExtensionCheckBox.Checked)
            {
                if (this.User.IsActive)
                    this.DatePickerValidFrom.Enabled = false;
                else
                    this.DatePickerValidFrom.Enabled = true;

                this.DatePickerValidTo.Enabled = true;
            }
            else
            {
                this.DatePickerValidFrom.Enabled = false;
                this.DatePickerValidTo.Enabled = false;
            }
        }
        private EmployeeType empType(string emptype)
        {
            try
            {
                switch (emptype)
                {
                    //case "None": return EmployeeType.None;
                    case "Contractor": return EmployeeType.Contractor;
                    //case "Intern": return EmployeeType.Intern;
                    case "Vendor": return EmployeeType.Vendor;
                    //case "SDC User": return EmployeeType.SDCUser;
                    //case "SP Club": return EmployeeType.SPClub;
                    case "Temp Staff": return EmployeeType.TempStaff;
                    case "3": return EmployeeType.Contractor;
                    //case "Intern": return EmployeeType.Intern;
                    case "2": return EmployeeType.Vendor;
                    //case "SDC User": return EmployeeType.SDCUser;
                    //case "SP Club": return EmployeeType.SPClub;
                    case "4": return EmployeeType.TempStaff;
                    default: return EmployeeType.TempStaff;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        void SaveUserRole(long userId)
        {
            try
            {
                Dictionary<string, string> paramRole = new Dictionary<string, string>();
                paramRole.Add("userID", userId.ToString());
                paramRole.Add("roleID", MasterConfigHelper.RoleUser.ToString());
                DomainApiClient<Dto.User>.GetByAction<Dto.User>(DomainApiAction.User.InsertUserRole, paramRole);
            }
            catch (Exception ex)
            {

            }
        }
        void SaveUserEntry()
        {
            try
            {
                if (!Validate())
                {
                    return;
                }

                User usr = null;
                bool isOnBoardDateChanged = false;
                bool isEndDateChanged = false;
                bool IsDisplayNameChanged = false;
                //Used for scenario were Request was rejected but while resubmit the request without any changes system should allow.
                //This is for Employee created with Inactive with Rejected.
                bool isResubmitForRejectedRequest = false;
                var VSOldUserName = string.Empty;
                string OldProjectDesc = string.Empty, oldCompanyDec = string.Empty, oldRefUsr = string.Empty, oldEmail = string.Empty, oldMobile = string.Empty;

                string newReferenceUser = string.Empty;
                var userId = 0;
                if (ViewState["TempId"] != null)
                {
                    userId = Convert.ToInt32(ViewState["TempId"]);
                    usr = DomainApiClient<Dto.User>.GetById(userId);
                    OldProjectDesc = usr.ProjectDescription;
                    oldCompanyDec = usr.NCompany;
                    oldRefUsr = DomainApiClient<Dto.User>.GetById(usr.ReferenceUserId)?.DisplayName;
                    oldMobile = usr.MobileNumber;
                    oldEmail = usr.EmailId;
                    VSOldUserName = usr.DisplayName;
                    usr.LastModifiedBy = SessionUser.User.UserName;
                    usr.LastModifiedDate = DateTime.Now;
                    VSOldEffectiveDate = usr.TerminationDate.HasValue ? usr.TerminationDate.Value : DateTime.MinValue;
                }
                else
                {
                    usr = new User();

                    usr.CreatedBy = SessionUser.User.UserName;
                    usr.CreatedDate = DateTime.Now;
                }

                //usr.IdentificationNo = NRIC;
                //usr.FirstName = txtFirstName.Text.Trim();
                usr.DisplayName = this.UserDisplayName;
                usr.LastName = usr.DisplayName;

                //new fields
                usr.ProjectDescription = this.UserProjectDescription;
                usr.EmailId = this.UserEmailId;
                usr.NCompany = this.UserNonEmployeeCompany;
                usr.AuthenticationMode = MasterConfigHelper.AuthenticationMode;
                
                if ((ReferenceUsersAutoCompleteBox.Entries.Count > 0))
                {
                    long referenceUserId = 0;
                    if (ReferenceUsersAutoCompleteBox.Entries.Count > 0)
                    {
                        newReferenceUser = ReferenceUsersAutoCompleteBox.Entries[0].Text;
                        referenceUserId = Convert.ToInt64(ReferenceUsersAutoCompleteBox.Entries[0].Value.Split('-')[0]);
                    }
                    usr.ReferenceUserId = referenceUserId;
                    var referenceUser = DomainApiClient<Dto.User>.GetById(referenceUserId);

                    if (referenceUser == null || referenceUser.IsActive == false)
                    {
                        MasterPage.ShowAlert(AlertType.Warning, "Selected Reference User is inactive");
                        return ; 
                    }
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("employeeId", referenceUser.ROEmployeeId);
                    var managerUser = DomainApiClient<Dto.User>.GetByAction<User>(DomainApiAction.User.GetByEmployeeId, parameter);

                    if (managerUser == null)
                    {
                        MasterPage.ShowAlert(AlertType.Warning, "Could not find RO for the reference user.");
                        return;
                    }

                    if (managerUser != null && managerUser.IsActive == false)
                    {
                        MasterPage.ShowAlert(AlertType.Warning, "Selected Reference User's RO is InActive.");
                        return;
                    }

                    usr.ManagerId = referenceUser.ManagerId;
                    
                    //Reference User
                    usr.Reserved5 = referenceUser.EmployeeId.ToString();
                    //RO
                    usr.Reserved6 = referenceUser.Reserved6;
                    usr.ROEmployeeId = referenceUser.ROEmployeeId;
                    //CO
                    usr.Reserved7 = referenceUser.Reserved7;
                    usr.COEmployeeId = referenceUser.COEmployeeId;

                    usr.CompanyCode = referenceUser.CompanyCode;
                    usr.CompanyId = referenceUser.CompanyId;
                    usr.CompanyName = referenceUser.CompanyName;
                    usr.OrgCompany = referenceUser.OrgCompany;

                    usr.DepartmentCode = referenceUser.DepartmentCode;
                    usr.DepartmentId = referenceUser.DepartmentId;
                    usr.DepartmentName = referenceUser.DepartmentName;
                    usr.OrgDepartment = referenceUser.OrgDepartment;

                    usr.DivisionCode = referenceUser.DivisionCode;
                    usr.DivisionId = referenceUser.DivisionId;
                    usr.DivisionName = referenceUser.DivisionName;
                    usr.OrgDivision = referenceUser.OrgDivision;

                    usr.CostCenter = referenceUser.CostCenter;
                    usr.CostCenterText = referenceUser.CostCenterText;

                    usr.NOrganisationId = referenceUser.NOrganisationId;

                    usr.SectionCode = referenceUser.SectionCode;
                    usr.SectionName = referenceUser.SectionName;
                    usr.OrgSection = referenceUser.OrgSection;

                    usr.OrgBranch = referenceUser.OrgBranch;
                    usr.BranchCode = referenceUser.BranchCode;

                    usr.Supervisor = referenceUser.Supervisor;
                    usr.OrgUnit = referenceUser.OrgUnit;
                    usr.NBusinessRoleId = referenceUser.NBusinessRoleId;

                    usr.OrganisationCode = referenceUser.OrganisationCode;
                    usr.UnitCode = referenceUser.UnitCode;
                }
                
                if (usr.Id != 0)
                {
                    
                    if (usr.IsActive)
                    {
                        isEndDateChanged = this.ExtensionCheckBox.Checked && usr.TerminationDate.Value.Date != DatePickerValidTo.SelectedDate.Value.Date;
                        IsDisplayNameChanged = !VSOldUserName.Equals(this.UserDisplayName, StringComparison.InvariantCultureIgnoreCase);
                    }
                    else
                    {
                        isOnBoardDateChanged = usr.JoinDate.Value != DatePickerValidFrom.SelectedDate.Value.Date;
                        isEndDateChanged = usr.TerminationDate.Value.Date != DatePickerValidTo.SelectedDate.Value.Date;
                        IsDisplayNameChanged = !VSOldUserName.Equals(this.UserDisplayName, StringComparison.InvariantCultureIgnoreCase);
                        if((usr.RequestStatus == AccessRequestStatus.Rejected || usr.RequestStatus == AccessRequestStatus.Withdrawn) && isEndDateChanged == false)
                        {
                            isResubmitForRejectedRequest = true;
                        }
                    }
                }

                usr.JoinDate = DatePickerValidFrom.SelectedDate;
                usr.TerminationDate = DatePickerValidTo.SelectedDate.Value.Date.AddHours(Constants.MaxHour);
                usr.EmployeeType = empType(TypeofEmployee.SelectedItem.Value.Trim());
                usr.TelephoneNumber = txtTelephoneNo.Text;
                usr.MobileNumber = txtMobileNumber.Text;
                usr.Ethinicity = Convert.ToInt16(Ethinicity.Others).ToString();
                usr.FullTimeEmployee = false;
                usr.EmployeeStatus = "1";
                usr.KnownAs = this.UserDisplayName;
                StringBuilder sb = new StringBuilder();
                if (usr.Id != 0)
                {
                    sb.AppendLine($"Updating User Data: {usr.KnownAs}");
                    if (!string.IsNullOrWhiteSpace(usr.DisplayName) && !usr.DisplayName.Equals(VSOldUserName))
                    {
                        sb.AppendLine($"Full Name: oldValue: {VSOldUserName} NewValue: {usr.DisplayName}");
                    }
                    if (!string.IsNullOrWhiteSpace(usr.MobileNumber) && !usr.MobileNumber.Equals(oldMobile))
                    {
                        sb.AppendLine($"Mobile: oldValue: {oldMobile} NewValue: {usr.MobileNumber}");
                    }
                    if (!string.IsNullOrWhiteSpace(usr.EmailId) || !usr.EmailId.Equals(oldEmail))
                    {
                        sb.AppendLine($"Email: oldValue: {oldEmail} NewValue: {usr.EmailId}");
                    }
                    if (!string.IsNullOrWhiteSpace(usr.NCompany) && !usr.NCompany.Equals(oldCompanyDec))
                    {
                        sb.AppendLine($"Company: oldValue: {oldCompanyDec} NewValue: {usr.NCompany}");
                    }
                    if (!string.IsNullOrWhiteSpace(usr.ProjectDescription) && !usr.ProjectDescription.Equals(OldProjectDesc))
                    {
                        sb.AppendLine($"ProjectDescription: oldValue: {OldProjectDesc} NewValue: {usr.ProjectDescription}");
                    }
                    if (!string.IsNullOrWhiteSpace(newReferenceUser) && !newReferenceUser.Equals(oldRefUsr))
                    {
                        sb.AppendLine($"ReferenceUser: oldValue: {oldRefUsr} NewValue: {newReferenceUser}");
                    }
                }

                if (usr.Id == 0)
                {
                    usr.Password = RSAHelper.EncryptData(MasterConfigHelper.DefaultPassword);
                    usr.UserName = ConstructUserName();
                    usr.EmployeeId = "1000" + usr.UserName.Substring(4, 4);
                    usr.IdentificationNo = usr.EmployeeId;
                    usr.IsActive = false;
                    usr.RequestStatus = AccessRequestStatus.Requested;
                    var savedUser = SaveAndGetUser(usr);
                    CreateNonEmployeeCreationTrigger(savedUser);
                    //CreateOffBoardTrigger(savedUser);
                    if(usr.EmployeeType == EmployeeType.TempStaff)
                    SaveUserRole(savedUser.Id);

                    MasterPage.ShowAlert(AlertType.Success, "User detail added successfully. User creation is waiting for supervisor's approval.");
                    string message = $"Non employee {usr.UserName} Created by {SessionUser.User.UserName}";
                    DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, message);
                }
                else if (usr.Id > 0)
                {
                    if (usr.IsActive && isEndDateChanged)
                    {
                        usr.TerminationDate = VSOldEffectiveDate;
                        if (IsDisplayNameChanged)
                        {
                            usr.DisplayName = VSOldUserName;
                            usr.KnownAs = VSOldUserName;
                            usr.LastName = usr.DisplayName;
                        }
                        usr.RequestStatus = AccessRequestStatus.Requested;
                        var savedUser = SaveAndGetUser(usr);
                        savedUser.TerminationDate = DatePickerValidTo.SelectedDate.Value.Date.AddHours(Constants.MaxHour);
                        if (IsDisplayNameChanged)
                        {
                            savedUser.DisplayName = this.UserDisplayName;
                            savedUser.KnownAs = this.UserDisplayName;
                            savedUser.LastName = savedUser.DisplayName;
                        }
                        CreateExtensionTrigger(savedUser, usr, true, isEndDateChanged, IsDisplayNameChanged);
                        MasterPage.ShowAlert(AlertType.Success, "User detail updated successfully. User extension is waiting for supervisor's approval.");
                        string message = $"Non employee {usr.UserName } information has been modified(Last Service Date From {VSOldEffectiveDate} to {savedUser.TerminationDate}) by {SessionUser.User.UserName}. Details:{sb.ToString()}";
                        DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, message);
                    }
                    else if (!usr.IsActive && (isEndDateChanged || isOnBoardDateChanged || isResubmitForRejectedRequest))
                    {
                        usr.IsActive = false;
                        usr.RequestStatus = AccessRequestStatus.Requested;
                        var exisitingDetails = DomainApiClient<Dto.User>.GetById(usr.Id);
                        var savedUser = SaveAndGetUser(usr);
                        if (isEndDateChanged)
                        {
                            //CreateNonEmployeeCreationTrigger(savedUser, true, exisitingDetails);
                            CreateNonEmployeeCreationTrigger(savedUser, exisitingDetails);
                        }
                        else if (isOnBoardDateChanged)
                        {
                            CreateNonEmployeeCreationTrigger(savedUser, exisitingDetails);
                        }
                        else if (isResubmitForRejectedRequest)
                        {
                            CreateNonEmployeeCreationTrigger(savedUser, exisitingDetails);
                        }
                        MasterPage.ShowAlert(AlertType.Success, "User detail added successfully. User creation is waiting for supervisor's approval.");
                        string message = $"Non employee{usr.UserName } Created by {SessionUser.User.UserName}";
                        DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, message);
                    }
                    else
                    {
                        var propagationFields = MasterConfigHelper.PropagationFields;

                        //Commented below code: Only Displayname change / Termination date change only
                        //usr.RequestStatus = AccessRequestStatus.Requested;
                        var savedUser = SaveAndGetUser(usr);
                        //Commented below code: Only Displayname change / Termination date change only
                        //CreateNonEmployeeCreationTrigger(savedUser, true);
                        //MasterPage.ShowAlert(AlertType.Success, "User detail updated successfully.");
                        string message = $"Non employee {usr.UserName } Edited by {SessionUser.User.UserName} {Environment.NewLine} {sb.ToString()}";
                        DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, message);
                    }
                }

                if (userId > 0) // If update mode, create propagation trigger;
                {
                    var updatedUsr = DomainApiClient<Dto.User>.GetById(userId);
                    if (updatedUsr.IsActive)
                    {
                        CreatePropagationTrigger(userId);
                    }
                        
                }
                Response.Redirect("~/NonEmployee/List.aspx");
                ViewState[V_USER] = usr;
                savedUser = usr;
                clearAll();
            }
            catch
            {
                throw;
            }
        }

        void CreatePropagationTrigger(long userId)
        {
            try
            {
                var parameter = new Dictionary<string, string>();
                parameter.Add("userId", userId.ToString());
                parameter.Add("createdBy", SessionUser.User.DisplayName);
                DomainApiClient<Dto.EmployeeEventTrigger>.GetByAction<bool>(DomainApiAction.Provisioning.EmployeeEventTrigger.CreatePropagationTrigger, parameter);
            }
            catch (Exception ex)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Exception, "Error in CreatePropagationTrigger", ex);
            }
        }

        Dto.User SaveAndGetUser(Dto.User usr)
        {
            DomainApiClient<Dto.User>.Save(usr);
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("name", usr.UserName);
            Dto.User user = DomainApiClient<Dto.User>.GetByAction<Dto.User>(DomainApiAction.User.GetByUserName, parameter);
            user.Password = RSAHelper.EncryptData(user.Password);
            DomainApiClient<Dto.User>.GetByAction<Dto.User>(DomainApiAction.User.DoAfterSaveUser, null, user);
            return user;
        }

        public bool SaveUser(bool fromHdnBtn = false)
        {
            SaveUserEntry();
            return true;
            #region old code
            //bool isErrorOccured = false;
            //foreach (Control c in this.Controls)
            //{
            //    if (c is TextBox)
            //    {
            //        TextBox textBox = c as TextBox;
            //        if ((textBox.Text.Trim() == string.Empty) && (textBox.CausesValidation))
            //        {
            //            ErrorMessage = "Mandatory fields should be required";
            //            MasterPage.ShowAlert(AlertType.Warning, ErrorMessage);
            //            IsErrorOccuring = true;
            //            isErrorOccured = true;
            //            return isErrorOccured;
            //        }
            //    }
            //}

            ////if (string.IsNullOrEmpty(FirstName.Text) || string.IsNullOrEmpty(LastName.Text))
            ////{
            ////    ErrorMessage = "Mandatory fields should be required";
            ////    MasterPage.ShowAlert(AlertType.Warning, ErrorMessage);
            ////    IsErrorOccuring = true;
            ////    isErrorOccured = true;
            ////    return isErrorOccured;
            ////}

            //if (!IsEdit)
            //{
            //    if ((IsNRICAlreadyExists(txtIdentificationNo.Text.Trim(), Convert.ToInt32(TypeofEmployee.SelectedValue))))
            //    {
            //        ErrorMessage = "Identification Number Already Exists";
            //        MasterPage.ShowAlert(AlertType.Warning, ErrorMessage);
            //        IsErrorOccuring = true;
            //        isErrorOccured = true;
            //        return isErrorOccured;
            //    }
            //}
            //if (UIMode == UIMode.Add || IsApprovePage)
            //{
            //    //if (Password.Text == string.Empty)
            //    //{
            //    //    IsErrorOccuring = true;
            //    //    isErrorOccured = true;
            //    //    ErrorMessage = "Please Enter Password";
            //    //    MasterPage.ShowAlert(AlertType.Warning, ErrorMessage);
            //    //    return true;
            //    //}
            //    //string message = string.Empty;
            //    //passwordPolicyValidator.EndPointId = 0;
            //    //bool bRet = passwordPolicyValidator.IsPasswordComplaint(UserName.Text.ToString().Trim(), Password.Text, out message);
            //    //if (message != string.Empty)
            //    //{
            //    //    ErrorMessage = message;
            //    //    MasterPage.ShowAlert(AlertType.Warning, message);
            //    //    return true;
            //    //}

            //    if (TypeofEmployee.SelectedItem.Text == "Vendor")
            //    {
            //        //var userName = UserName.Text.Trim();
            //        //if (string.IsNullOrEmpty(userName))
            //        //{
            //        //    ErrorMessage = "Please Enter UserName";
            //        //    MasterPage.ShowAlert(AlertType.Warning, ErrorMessage);
            //        //    return true;
            //        //}
            //        //else if (!string.IsNullOrEmpty(userName) && !userName.ToLower().EndsWith(".rven"))
            //        //{
            //        //    ErrorMessage = "Contractor username must ends with .rven";
            //        //    MasterPage.ShowAlert(AlertType.Warning, ErrorMessage);
            //        //    return true;
            //        //}
            //    }
            //}

            //if ((UIMode == UIMode.Add) && (ReferenceUsersAutoCompleteBox.Text == string.Empty) && (ReferenceUsersAutoCompleteBox.Visible))
            //{
            //    IsErrorOccuring = true;
            //    isErrorOccured = true;
            //    ErrorMessage = "Please Enter Supervisor";
            //    MasterPage.ShowAlert(AlertType.Warning, ErrorMessage);
            //    return true;
            //}
            ////if (CompanyAutoCompleteBox.GetValue() == null || CompanyAutoCompleteBox.GetValue().Text == string.Empty)
            ////{
            ////    ErrorMessage = "Please Enter Company Code";
            ////    MasterPage.ShowAlert(AlertType.Warning, ErrorMessage);
            ////    IsErrorOccuring = true;
            ////    isErrorOccured = true;
            ////    return isErrorOccured;
            ////}
            ////if (DesignationAutoCompleteBox.GetValue() == null || DesignationAutoCompleteBox.GetValue().Text == string.Empty)
            ////{
            ////    ErrorMessage = "Please Enter Designation Code";
            ////    MasterPage.ShowAlert(AlertType.Warning, ErrorMessage);
            ////    IsErrorOccuring = true;
            ////    isErrorOccured = true;
            ////    return isErrorOccured;
            ////}

            //Dto.User usr = new Dto.User();
            //if (IsEdit)
            //{
            //    usr = DomainApiClient<Dto.User>.GetById(EmployeeUserId);
            //    if (usr != null && usr.IdentificationNo != txtIdentificationNo.Text)
            //    {
            //        if ((IsNRICAlreadyExists(txtIdentificationNo.Text.Trim(), (int)usr.EmployeeType)))
            //        {
            //            ErrorMessage = "Identification Number Already Exists";
            //            MasterPage.ShowAlert(AlertType.Warning, ErrorMessage);
            //            IsErrorOccuring = true;
            //            isErrorOccured = true;
            //            return isErrorOccured;
            //        }
            //    }
            //}

            //if (!string.IsNullOrEmpty(this.DatePickerValidFrom.SelectedDate.ToString()))
            //{
            //    DateTime dt2 = Convert.ToDateTime(this.DatePickerValidFrom.SelectedDate);
            //    usr.JoinDate = dt2;
            //}
            //else
            //{
            //    ErrorMessage = "Please enter Join Date";
            //    MasterPage.ShowAlert(AlertType.Warning, ErrorMessage);
            //    IsErrorOccuring = true;
            //    isErrorOccured = true;
            //    return isErrorOccured;
            //}
            //if (!string.IsNullOrEmpty(this.DatePickerValidTo.SelectedDate.ToString()))
            //{
            //    DateTime dt1 = Convert.ToDateTime(this.DatePickerValidTo.SelectedDate);
            //    usr.TerminationDate = dt1;
            //}
            //else
            //{
            //    ErrorMessage = "Please enter Off-boarding Date";
            //    MasterPage.ShowAlert(AlertType.Warning, ErrorMessage);
            //    IsErrorOccuring = true;
            //    isErrorOccured = true;
            //    return isErrorOccured;
            //}

            //if (usr.JoinDate != null && usr.TerminationDate != null)
            //{
            //    if (usr.TerminationDate < usr.JoinDate)
            //    {
            //        ErrorMessage = "Valid To Date should be greater then Join Date.";
            //        MasterPage.ShowAlert(AlertType.Warning, ErrorMessage);
            //        isErrorOccured = true;
            //        IsErrorOccuring = true;
            //        return isErrorOccured;
            //    }
            //}

            //usr.IdentificationNo = txtIdentificationNo.Text;
            //usr.DisplayName = txtDisplayName.Text;
            //TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            ////usr.FirstName = textInfo.ToTitleCase(FirstName.Text);
            ////usr.LastName = LastName.Text.ToUpper();

            ////if ((!IsEdit || IsApprovePage) && doExists(txtUsername.Text.Trim()))
            ////{
            ////    Dictionary<string, string> parameter = new Dictionary<string, string>();
            ////    parameter.Add("name", Convert.ToString(txtUsername.Text.Trim()));

            ////    var tmpUsr = DomainApiClient<Dto.User>.GetByAction<Dto.User>
            ////        (DomainApiAction.User.GetByUserName, parameter);

            ////    if (tmpUsr != null && tmpUsr.EmployeeId != txtEmployeeNo.Text.Trim()
            ////        && (tmpUsr.IdentificationNo != usr.IdentificationNo))
            ////    {
            ////        ErrorMessage = "UserName already Exists.";
            ////        MasterPage.ShowAlert(AlertType.Warning, ErrorMessage);
            ////        IsErrorOccuring = true;
            ////        return true;
            ////    }
            ////}
            //if (UIMode == UIMode.Edit || IsApprovePage)
            //{
            //    //usr.UserName = UserName.Text;
            //    //  usr.Reserved5 = DisplayName.Text;

            //    if (!IsEdit)
            //        usr.AuthenticationMode = AuthenticationMode.IACM.ToString();

            //    if ((ReferenceUsersAutoCompleteBox.Entries.Count > 0))
            //    {
            //        long ReferenceUserId = 0;
            //        if (ReferenceUsersAutoCompleteBox.Entries.Count > 0)
            //        {
            //            ReferenceUserId = Convert.ToInt64(ReferenceUsersAutoCompleteBox.Entries[0].Value.Split('-')[0]);
            //        }
            //        Dictionary<string, string> parameter = new Dictionary<string, string>();
            //        parameter.Add("id", Convert.ToString(ReferenceUserId));

            //        var ReferenceUser = DomainApiClient<Dto.User>.GetByAction<Dto.User>
            //            (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, parameter);

            //        usr.NOrganisationId = ReferenceUser.NOrganisationId;
            //        usr.ManagerId = ReferenceUserId;
            //    }
            //    //AutoCompleteBoxEntry selectedDepartMent = DepartmentAutoCompleteBox.GetValue();
            //    //if (selectedDepartMent != null)
            //    //{
            //    //    usr.NOrganisationId = Convert.ToInt64(selectedDepartMent.Value);
            //    //}

            //    //AutoCompleteBoxEntry selectedDesignation = DesignationAutoCompleteBox.GetValue();
            //    //if (selectedDesignation != null)
            //    //{
            //    //    usr.NBusinessRoleId = Convert.ToInt64(selectedDesignation.Value);
            //    //}

            //    //AutoCompleteBoxEntry selectedCompany = CompanyAutoCompleteBox.GetValue();
            //    //if (selectedCompany != null)
            //    //{
            //    //    usr.CompanyId = Convert.ToInt64(selectedCompany.Value);
            //    //}


            //    //if ((UIMode == UIMode.Edit || IsEdit) && ReferenceUsersAutoCompleteBox.Entries.Count == 0)
            //    //{
            //    //    usr.NOrganisationId = usr.NOrganisationId;
            //    //    usr.NBusinessRoleId = usr.NBusinessRoleId;
            //    //}

            //    else if ((EmployeeType)Convert.ToInt32(TypeofEmployee.SelectedValue) == EmployeeType.Permanent)
            //    {
            //        usr.EmployeeType = EmployeeType.Permanent;
            //        usr.FullTimeEmployee = true;
            //    }
            //    else if ((EmployeeType)Convert.ToInt32(TypeofEmployee.SelectedValue) == EmployeeType.Contractor)
            //    {
            //        usr.EmployeeType = EmployeeType.Contractor;
            //        usr.FullTimeEmployee = true;
            //    }
            //    else
            //    {
            //        usr.EmployeeType = EmployeeType.Vendor;
            //        usr.FullTimeEmployee = false;
            //    }

            //    usr.Ethinicity = Convert.ToInt16(Ethinicity.Others).ToString();
            //    //usr.FullTimeEmployee = true;
            //    usr.IsActive = false;
            //    usr.LastLogonDate = DateTime.Now;
            //    //if (selectedCompany != null)
            //    //{
            //    //    NOrganisation Company = DomainApiClient<NOrganisation>.GetById(Convert.ToInt64(selectedCompany.Value));
            //    //    if (usr.EmployeeType == EmployeeType.Vendor)
            //    //        usr.EmailId = usr.Company + ".rven@" + Company.Domain + ".com";
            //    //    else
            //    //        usr.EmailId = usr.UserName + "@" + Company.Domain + ".com";
            //    //}
            //    Dictionary<string, string> parameters = new Dictionary<string, string>();
            //    parameters.Add("endpointType", EndpointType.IDMS.ToString());
            //    parameters.Add("hostName", "");
            //    List<EndPoint> idasEndPoints = DomainApiClient<EndPoint>.GetByAction<List<EndPoint>>("GetByEndpointType", parameters);
            //    if (idasEndPoints.Count > 0)
            //        usr.EndPointSystemId = idasEndPoints[0].Id;

            //    if (IsEdit)
            //    {
            //        getUserId = usr.Id = Convert.ToInt64(EmployeeUserId);
            //        usr.EmployeeId = txtEmployeeNo.Text;
            //        bool InitiatePropagation = usr.TelephoneNumber == txtTelephoneNo.Text ? false : true;
            //        usr.TelephoneNumber = txtTelephoneNo.Text;
            //        //usr.MobileNumber = txtMobileNo.Text;
            //        //if (IsApprovePage)
            //        //    usr.Password = RSAHelper.EncryptData(Password.Text);
            //        //else
            //        //    usr.Password = RSAHelper.EncryptData(PasswordValue);
            //        usr.EmailId = DomainApiClient<Dto.User>.GetById((long)getUserId).EmailId;

            //        if (RoleList.Count > 0)
            //        {
            //            List<Dto.Role> lstRoles = new List<Dto.Role>();
            //            foreach (Dto.Role role in RoleList)
            //            {
            //                Dto.Role Extrole = DomainApiClient<Dto.Role>.GetById((long)usr.ManagerId);
            //                lstRoles.Add(Extrole);
            //            }
            //            usr.Roles = lstRoles;
            //        }
            //        else
            //        {
            //            if (RoleList.Count == 0)
            //            {
            //                Dto.Role role = DomainApiClient<Dto.Role>.GetById((long)MasterConfigHelper.RoleUser);
            //                List<Dto.Role> lstRoles = new List<Dto.Role>();
            //                lstRoles.Add(role);
            //                usr.Roles = lstRoles;
            //            }
            //        }
            //        DomainApiClient<Dto.User>.Save(usr);
            //        this.TrackEventTriggers(usr, InitiatePropagation);
            //    }
            //    else
            //    {
            //        //usr.Password = RSAHelper.EncryptData(Password.Text);
            //        if (txtEmployeeNo.Text == string.Empty)
            //            usr.EmployeeId = SettxtEmployeeNo();
            //        else
            //            usr.EmployeeId = txtEmployeeNo.Text;
            //        usr.TelephoneNumber = txtTelephoneNo.Text;
            //        //usr.MobileNumber = txtMobileNo.Text;

            //        //Harihara : Commented out below code here, as RWS consider IDAS is an endpoint and provisioning of roles should happen through workflow.
            //        //long RoleId = MasterConfigHelper.RoleUser;
            //        //Dto.Role role = DomainApiClient<Dto.Role>.GetById(RoleId);
            //        //List<Dto.Role> lstRoles = new List<Dto.Role>();
            //        //lstRoles.Add(role);
            //        //usr.Roles = lstRoles;


            //        DomainApiClient<Dto.User>.Save(usr);
            //        Dictionary<string, string> parameter = new Dictionary<string, string>();
            //        parameter.Add("name", usr.UserName);
            //        Dto.User userobj = DomainApiClient<Dto.User>.GetByAction<Dto.User>(DomainApiAction.User.GetByUserName, parameter);
            //        var createdAccessRequestId = CreateAccessRequest(userobj);
            //        parameter.Clear();
            //        parameter.Add("nric", txtIdentificationNo.Text);
            //        parameter.Add("employeeType", TypeofEmployee.SelectedValue);
            //        Dto.User existingUser = DomainApiClient<Dto.User>.GetByAction<Dto.User>(DomainApiAction.User.GetByNRIC, parameter);
            //        parameter.Clear();
            //        this.TrackEventTriggers(usr, false);

            //        usr.Id = existingUser.Id;

            //        //save the access request id for reference
            //        if (createdAccessRequestId > 0 && usr.Id != 0)
            //        {
            //            usr.Reserved9 = createdAccessRequestId.ToString();
            //            DomainApiClient<Dto.User>.Save(usr);
            //        }

            //        //parameter.Clear();
            //        //parameter.Add("userId", usr.Id.ToString());
            //        //DomainApiClient<Dto.User>.GetByAction<bool>(DomainApiAction.User.ComposeNewUserCredentialMail, parameter);

            //        ViewState[V_USER] = usr;
            //    }
            //    DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "IDAS User - Save");
            //    if (usr.Id != 0)
            //    {
            //        Dto.UserImage userImage = new Dto.UserImage();
            //        userImage.Id = this.UserProfileId;
            //        userImage.UserId = usr.Id;
            //        userImage.Image = this.UploadedFileName;
            //        DomainApiClient<Dto.UserImage>.Save(userImage);
            //    }
            //}
            //savedUser = usr;
            //return isErrorOccured;
            #endregion
        }

        private bool IsIdAlreadyExists(string txtEmployeeNo)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("username", txtEmployeeNo);
            return DomainApiClient<Dto.User>.GetByAction<bool>(DomainApiAction.User.DoesEmployeeExists, parameter);
        }

        private bool IsNRICAlreadyExists(string identificationNumber, int employeeType)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("identificationNumber", identificationNumber);
            parameter.Add("employeeType", employeeType.ToString());
            return DomainApiClient<Dto.User>.GetByAction<bool>(DomainApiAction.User.DoesIdentificationNumberExists, parameter);
        }
        //protected void ConstructUserName(object sender, EventArgs e)
        //{
        //    if (!IsEdit)
        //    {
        //        txtUsername.Text = ConstructUserName();
        //    }
        //    txtProjectDescription.Focus();
        //    //if (!IsEdit || IsApprovePage)
        //    //{
        //    //    UserName.Text = ConstructUserName();
        //    //    if (!string.IsNullOrEmpty(FirstName.Text) && !string.IsNullOrEmpty(LastName.Text) &&
        //    //        string.IsNullOrEmpty(UserName.Text))
        //    //    {
        //    //        if (TypeofEmployee.SelectedItem.Text == "Vendor")
        //    //        {
        //    //            ErrorMessage = "Unable to construct User Name or User Name exceeded more than 10 characters.";
        //    //        }
        //    //        else
        //    //        {
        //    //            ErrorMessage = "Unable to construct User Name or User Name exceeded more than 15 characters.";
        //    //        }
        //    //        MasterPage.ShowAlert(AlertType.Warning, ErrorMessage);
        //    //        return;
        //    //    }

        //    //    TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        //    //    FirstName.Text = textInfo.ToTitleCase(FirstName.Text);
        //    //    LastName.Text = LastName.Text.ToUpper();

        //    //    DisplayName.Text = FirstName.Text.Trim() + " " + LastName.Text.Trim().ToUpper();

        //    //}
        //}
        #endregion

        #region Events
        protected void ReferenceUsersAutoCompleteBox_EntryAdded(object sender, Telerik.Web.UI.AutoCompleteEntryEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Entry.Text))
            {
                if (this.ReferenceUsersAutoCompleteBox.Entries.Count > 0)
                {
                    if (ReferenceUsersAutoCompleteBox.Entries.Count == 2)
                    {
                        ReferenceUsersAutoCompleteBox.Entries.Remove(ReferenceUsersAutoCompleteBox.Entries[1]);
                    }
                    else
                    {
                        Session["ReferenceUserId"] = this.ReferenceUsersAutoCompleteBox.Entries[0].Value;
                        txtCompany.Text = string.Empty;
                        txtCostCenter.Text = string.Empty;
                        txtDepartment.Text = string.Empty;
                        txtDivision.Text = string.Empty;
                        txtOrgUnit.Text = string.Empty;
                        txtSection.Text = string.Empty;
                        txtUnit.Text = string.Empty;

                        if (ReferenceUsersAutoCompleteBox.Entries.Count > 0)
                        {
                            string[] strReference = ReferenceUsersAutoCompleteBox.Entries[0].Value.Split('-');
                            setCompanyDetails(strReference[0]);
                            //SetCompanyDetails(ReferenceUsersAutoCompleteBox.Entries[0].Value);

                        }
                    }
                }
            }
        }
        private void setCompanyDetails(string prefix)
        {
            try
            {
                if (string.IsNullOrEmpty(prefix.Trim()))
                {
                    return;
                }
                User olist = GetUserCompanyDetails(prefix); 
                txtCompany.Text = olist.OrgCompany;
                txtCostCenter.Text = olist.CostCenter;
                txtDepartment.Text = olist.OrgDepartment;
                txtDivision.Text = olist.OrgBranch;
                txtOrgUnit.Text = this.getOrganisationCodeName(olist);
                txtSection.Text = olist.OrgSection;
                txtUnit.Text = olist.OrgUnit; 
            }
            catch (Exception)
            {
                throw;
            }
        }


        private Dto.User GetUserCompanyDetails(string prefix)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("prefix", prefix);
            return DomainApiClient<Dto.User>.GetByAction<Dto.User>(DomainApiAction.User.GetCompanyDetails, parameter);
        }
        protected void fileUploadField_FileUploaded(object sender, Telerik.Web.UI.FileUploadedEventArgs e)
        {
            //try
            //{
            //    string path = Server.MapPath("~/Profile/ImageUploadedFiles/");
            //    string FileName = e.File.GetName();
            //    if (!(string.IsNullOrEmpty(FileName)))
            //    {
            //        e.File.SaveAs(path + FileName);
            //        byte[] data = Encoding.UTF8.GetBytes(FileName);
            //        this.UploadedFileName = data;
            //        ProfileImg.Src = "~/Profile/ImageUploadedFiles/" + FileName;
            //    }
            //}
            //catch (Exception)
            //{
            //    return;
            //}
        }

        protected void imgDelete_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            this.UploadedFileName = null;
            //ProfileImg.Src = "../../images/profile-placeholder.png";
        }

        void DisableControls()
        {
            txtCompany.Enabled = false;
            txtDepartment.Enabled = false;
            //txtEmployeeNo.Enabled = false;
            //txtUsername.Enabled = false;
            txtOrgUnit.Enabled = false;
            txtCostCenter.Enabled = false;
            txtDivision.Enabled = false;
            txtUnit.Enabled = false;
            txtSection.Enabled = false;
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                //if (!string.IsNullOrEmpty(Password.Text))
                //{
                //    ViewState["PASSWORD"] = Password.Text.ToString();
                //    Password.Attributes.Add("value", PasswordValue);
                //}

                //if (ViewState["PASSWORD"] != null)
                //{
                //    Password.Text = Convert.ToString(ViewState["PASSWORD"]);
                //    Password.Attributes.Add("value", Convert.ToString(ViewState["PASSWORD"]));
                //}
            }
            else
            {
                DisableControls();
            }
            setCompanyService();
        }
        #endregion
        protected void setCompanyService()
        {
            //if (IsFullTimeEmployee)
            //{
            //    CompanyAutoCompleteBox.ServiceMethodName = "GetInternalCompanies";
            //    //AddNewCompany.Visible = false;
            //}
            //else
            //{
            //    CompanyAutoCompleteBox.ServiceMethodName = "GetExternalCompanies";
            //    //AddNewCompany.Visible = true;
            //}
        }
        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Employee/List.aspx");
        }

        protected void ok_Click(object sender, EventArgs e)
        {
            DisplayPolicy.VisibleOnPageLoad = false;
        }

        protected void DisplayPolicies_Click(object sender, EventArgs e)
        {
            EnterprisePasswordPolicy1.Title = "IACM Password Policy";
            EnterprisePasswordPolicy1.EndPointSystemID = 0;
            EnterprisePasswordPolicy1.BindPolicy();
            DisplayPolicy.VisibleOnPageLoad = true;
        }
        private long CreateAccessRequest(Dto.User userobj)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("name", EndpointType.IDMS.ToString());
            Dto.EndPointSystemType endPointSystemType = DomainApiClient<Dto.EndPointSystemType>.GetByAction<Dto.EndPointSystemType>(DomainApiAction.Endpoint.EndPointSystemType.GetByName, parameters);
            parameters.Clear();
            parameters.Add("endpointSystemTypeId", endPointSystemType.Id.ToString());
            List<Dto.EndPoint> endPoints = DomainApiClient<Dto.EndPoint>.GetByAction<List<Dto.EndPoint>>(DomainApiAction.Endpoint.EndPointSystemType.GetByEndpointSystemTypeIdIncludingEndPointSystemType, parameters);
            long? epId = endPoints.First().Id;
            Guid transactionId = Guid.NewGuid();
            Dto.AccessRequest newRequest = new Dto.AccessRequest();
            newRequest.UserId = userobj.Id;
            newRequest.EndpointSystemId = epId;
            newRequest.RequestStatus = AccessRequestStatus.Submitted;
            newRequest.ProgressStatus = AccessRequestProgress.WaitingforApproval;
            newRequest.RequesterId = SessionUser.User.Id;
            newRequest.ActivationDate = (userobj.JoinDate.HasValue && userobj.JoinDate.Value > DateTime.Now) ? userobj.JoinDate.Value : DateTime.Now;
            newRequest.Description = "This request is created to add new employee in IDAS.";
            newRequest.DeactivationDate = userobj.TerminationDate;
            newRequest.TransactionId = transactionId;
            newRequest.ExecutionTime = DateTime.Now;
            newRequest.Remarks = string.Empty;
            newRequest.AccountType = AccountType.User;
            newRequest.RequestSource = AccessRequestSource.EmployeeCreation;

            AdHocAccessRequest adhocrequst = new AdHocAccessRequest();
            adhocrequst.RequesteeUserId = userobj.Id;
            adhocrequst.RequesterUserId = SessionUser.User.Id;
            adhocrequst.EndpointSystemType = EndpointType.IDMS;
            adhocrequst.EndpointSystemId = epId;
            adhocrequst.AdhocActionType = AdHocActionType.NewAccount;
            adhocrequst.AccountType = AccountType.User;
            adhocrequst.ValidFrom = (userobj.JoinDate.HasValue && userobj.JoinDate.Value > DateTime.Now) ? userobj.JoinDate.Value : DateTime.Now;
            adhocrequst.ValidTo = userobj.TerminationDate;
            adhocrequst.Comments = "Employee Creation Request";
            DomainApiClient<Dto.AdHocAccessRequest>.Save(adhocrequst);

            parameters.Clear();
            parameters.Add("requesteeUserId", adhocrequst.RequesteeUserId.ToString());
            parameters.Add("comments", adhocrequst.Comments);
            parameters.Add("endpointSystemId", epId.ToString());
            var adhocObj = DomainApiClient<AdHocAccessRequest>.GetByAction<AdHocAccessRequest>(DomainApiAction.Provisioning.AdhocAccessRequest.GetByRequesteeUserId, parameters);

            if (adhocObj != null && adhocObj.Id > 0)
                newRequest.AdHocAccessRequestId = adhocObj.Id;

            DomainApiClient<Dto.AccessRequest>.Save(newRequest);
            Dto.AccessRequest accessRequestObj = null;
            parameters.Clear();
            if (adhocObj != null && adhocObj.Id > 0)
            {
                parameters.Add("adhocAccessRequestId", adhocObj.Id.ToString());
                accessRequestObj = DomainApiClient<Dto.AccessRequest>.GetByAction<Dto.AccessRequest>(DomainApiAction.Provisioning.AccessRequest.GetByAdHocAccessRequestId, parameters);
            }
            else
            {
                parameters.Add("userId", adhocrequst.RequesteeUserId.ToString());
                accessRequestObj = DomainApiClient<Dto.AccessRequest>.GetByAction<Dto.AccessRequest>(DomainApiAction.Provisioning.AccessRequest.GetByUserId, parameters);
            }

            if (accessRequestObj != null && accessRequestObj.Id > 0)
                return accessRequestObj.Id;
            else
                return 0;
        }

        private Dto.NOrganisation GetNOrganizationByIdandScope(long Id, string scope)
        {
            Dto.NOrganisation nOrg = null;

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Scope", Convert.ToString(scope));
            var LocationScope = DomainApiClient<Dto.NOrganisationScope>.GetByAction<Dto.NOrganisationScope>("GetOrganisationScope", parameters);

            parameters.Clear();
            parameters.Add("Id", Convert.ToString(Id));
            parameters.Add("scopeId", LocationScope.Id.ToString());
            nOrg = DomainApiClient<Dto.NOrganisation>.GetByAction<Dto.NOrganisation>("GetNOrganisationIdByScopeId", parameters);


            return nOrg;
        }

        protected void btnCheck_Click(object sender, EventArgs e)
        {
            //if (!string.IsNullOrEmpty(UserName.Text.Trim()))
            //{
            //    if (doExists(UserName.Text.Trim()))
            //    {
            //        Dictionary<string, string> parameter = new Dictionary<string, string>();
            //        parameter.Add("name", Convert.ToString(UserName.Text.Trim()));

            //        var tmpUsr = DomainApiClient<Dto.User>.GetByAction<Dto.User>
            //            (DomainApiAction.User.GetByUserName, parameter);

            //        if (tmpUsr != null && tmpUsr.EmployeeId != txtEmployeeNo.Text.Trim()
            //            && (tmpUsr.txtIdentificationNo != txtIdentificationNo.Text))
            //        {
            //            MasterPage.ShowAlert(AlertType.Error, "user name already available in Active Directory, please try with other options.");
            //        }
            //    }
            //    else
            //    {
            //        MasterPage.ShowAlert(AlertType.Success, "user name available.");
            //    }
            //}
            //else
            //{
            //    MasterPage.ShowAlert(AlertType.Error, "please entry user name and check");
            //}
        }

        protected void ReferenceUsersAutoCompleteBox_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {

        }

        //protected void txtFirstName_TextChanged(object sender, EventArgs e)
        //{
        //    txtDisplayName.Text = string.IsNullOrWhiteSpace(txtFirstName.Text) ? txtLastName.Text : $"{txtFirstName.Text} {txtLastName.Text}";
        //}
    }
}