using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using Telerik.Web.UI;

namespace DI.Chakra.Web 
{
    public partial class DepartmentChangeRequest : UIBasePage
    {
        private const string KEY_SessionStatus = @"APAC_KEY_SESSIONSTATUS";
        private const string KEY_CrntUserList = @"APAC_KEY_CrntUserList";
        private const int NegativeIntValue = -1;
        
        private SessionStatusImpl m_sessionStatus;
        private List<DeptChangedUserGridData> m_usrList;

        public SessionStatusImpl SessionStatus
        {
            get
            {
                if (m_sessionStatus == null)
                    m_sessionStatus = Session[KEY_SessionStatus] as SessionStatusImpl;
                return m_sessionStatus;
            }
            set
            {
                m_sessionStatus = value;
                Session[KEY_SessionStatus] = m_sessionStatus;
            }
        }

        public List<DeptChangedUserGridData> CrntUserList
        {
            get
            {
                if (m_usrList == null)
                    m_usrList = Session[KEY_CrntUserList] as List<DeptChangedUserGridData>;
                return m_usrList;
            }
            set
            {
                m_usrList = value;
                Session[KEY_CrntUserList] = m_usrList;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnAPACCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }
        void BindPageGridData()
        {

            if (ouChangeRB.Checked)
            {
                CrntUserList = DomainApiClient<Dto.User>.GetByAction<List<DeptChangedUserGridData>>(DomainApiAction.User.GetOUChangeUsersForAdminCheck, null);

            }
            else if (reOrgRB.Checked)
            {
                CrntUserList = DomainApiClient<Dto.User>.GetByAction<List<DeptChangedUserGridData>>(DomainApiAction.User.GetReOrgUsersForAdminCheck, null);
            }
            else if (onboardRB.Checked)
            {
                CrntUserList = DomainApiClient<Dto.User>.GetByAction<List<DeptChangedUserGridData>>(DomainApiAction.User.GetOnBoardUsersForAdminCheck, null);
            }
            else if (rehireRB.Checked)
            {
                CrntUserList = DomainApiClient<Dto.User>.GetByAction<List<DeptChangedUserGridData>>(DomainApiAction.User.GetReHireUsersForAdminCheck, null);
            }


            grdDeptChangedUsers.DataSource = CrntUserList; // DomainApiClient<EmployeeEventTrigger>.GetByAction< UsrFacade.GetDeptChangedUserGridData(CrntUserList, RequestSource);
        }

        protected void grdDeptChangedUsers_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BindPageGridData();
        }
        protected void grdDeptChangedUsers_ItemCommand(object sender, GridCommandEventArgs e)
        {
            int _userId = 0;
            User _oldUser = new User();

            if (e.CommandName == "Details")
            {
                GridDataItem item = e.Item as GridDataItem;

                _userId = (int)Convert.ChangeType(item.GetDataKeyValue("UserId"), typeof(int));
                DeptChangedUserGridData _newUser = CrntUserList.FirstOrDefault(usr => usr.UserId == _userId);
                _oldUser = DomainApiClient<Dto.User>.GetById(_userId);
                if (_newUser != null && _oldUser != null)
                {
                    txtUserName.Text = _oldUser.DisplayName;
                    if (_oldUser.DepartmentName != _newUser.Department)
                    {
                        trDepartment1.Visible = true;
                        trDepartment2.Visible = true;
                        txtDepartment.Text = _oldUser.DepartmentName;
                        txtNewDepartment.Text = _newUser.Department;
                    }
                    else
                    {
                        trDepartment1.Visible = false;
                        trDepartment2.Visible = false;
                    }
                    if (_oldUser.DivisionName != _newUser.Division)
                    {
                        trDivision1.Visible = true;
                        trDivision2.Visible = true;
                        txtDivision.Text = _oldUser.DivisionName;
                        txtNewDivision.Text = _newUser.Division;
                    }
                    else
                    {
                        trDivision1.Visible = false;
                        trDivision2.Visible = false;
                    }

                    if (_oldUser.CompanyName != _newUser.Company)
                    {
                        trCompany1.Visible = true;
                        trCompany2.Visible = true;
                        txtCompany.Text = _oldUser.CompanyName;
                        txtNewCompany.Text = _newUser.Company;
                    }
                    else
                    {
                        trCompany1.Visible = false;
                        trCompany2.Visible = false;
                    }

                    if (_oldUser.CostCenter != _newUser.CostCenter)
                    { 
                        trCostCenter1.Visible = true;
                        trCostCenter2.Visible = true;
                        txtCostCenter.Text = _oldUser.CostCenter;
                        txtNewCostCenter.Text = _newUser.CostCenter;
                    }
                    else
                    {
                        trCostCenter1.Visible = false;
                        trCostCenter2.Visible = false;
                    }

                    if (_oldUser.NOrganisationId != _newUser.Organisation)
                    {
                        trOrganisation1.Visible = true;
                        trOrganisation2.Visible = true;
                        NOrganisation org = null;


                        if (_oldUser.NOrganisationId.HasValue && _oldUser.NOrganisationId > 0)
                        {
                            org = DomainApiClient<Dto.NOrganisation>.GetById(_oldUser.NOrganisationId.Value);// new OrganisationFacade().GetOrganisationById(_oldUser.OrganisationId.Value);
                            if(org!=null)
                            txtOrganisation.Text = org.Name;
                        }

                        if (_newUser.Organisation.HasValue && _newUser.Organisation > 0)
                        {
                            org = DomainApiClient<Dto.NOrganisation>.GetById(_newUser.Organisation.Value);
                            if (org != null)
                                txtNewOrganisation.Text = org.Name;
                        }
                    }
                    else
                    {
                        trOrganisation1.Visible = false;
                        trOrganisation2.Visible = false;
                    }
                    UserOldNewDetailsModalPopupExtender.VisibleOnPageLoad = true;
                }
            }
        }

        protected void btnOk_Close(object sender, EventArgs e)
        {
            UserOldNewDetailsModalPopupExtender.VisibleOnPageLoad = false; 
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            long UserId;
            DeptChangedUserGridData newUsr;
            bool isIntValue;
            List<long> newUsrIds = new List<long>();
            StringBuilder sbUserNames = new StringBuilder();
            List<string> invalidMatrix = new List<string>();
            int userId = 0;
            int i = 0;
            foreach (GridDataItem grow in grdDeptChangedUsers.Items)
            {
                CheckBox checkbox = (CheckBox)grow.FindControl("chkUser");
                Label LabelStatus = (Label)grow.FindControl("LabelStatus");
                //grdDeptChangedUsers.Columns.FindByUniqueNameSafe("LabelStatus").Display = true;
                userId = (int)Convert.ChangeType(grow.GetDataKeyValue("UserId"), typeof(int));

                if (checkbox.Checked)
                {
                    i++;
                    HiddenField hdUsedId = (HiddenField)grow.FindControl("hdnUserid");
                    isIntValue = long.TryParse(hdUsedId.Value, out UserId);
                    if (isIntValue && UserId > 0)
                    {
                        newUsr = CrntUserList.FirstOrDefault(item => item.UserId == UserId);
                       
                        if (newUsr != null)
                        {
                            Dto.User usr = null;
                            if (!string.IsNullOrWhiteSpace(newUsr.UserDetail))
                                usr = UtilityMethods.Deserialize<User>(newUsr.UserDetail);
                            else
                                usr = DomainApiClient<Dto.User>.GetById(UserId);
                            string message = string.Empty;
                            EmployeeEvents events = EmployeeEvents.None;
                            if (ouChangeRB.Checked)
                            {
                                events = EmployeeEvents.OUChange;
                            }
                            else if (reOrgRB.Checked)
                            {
                                events = EmployeeEvents.ReOrganisation;
                            }
                            else if (onboardRB.Checked)
                            {
                                events = EmployeeEvents.NewJoinee;
                            }
                            else if(rehireRB.Checked)
                            {
                                events = EmployeeEvents.Rehire;
                            }
                            message = DomainApiClient<Dto.User>.GetByAction<string>(DomainApiAction.User.IsProvisioningDefaultExists,
                                new Dictionary<string, string> { { "events", Convert.ToString((int)events) }  }, usr);

                            if (string.IsNullOrEmpty(message))
                            {
                                var trigger = DomainApiClient<Dto.EmployeeEventTrigger>.GetByAction<EmployeeEventTrigger>(
                                     DomainApiAction.Provisioning.EmployeeEventTrigger.GetByUserAndEvent,
                                     new Dictionary<string, string>{

                                                { "userId", newUsr.UserId.ToString() },
                                            {"evnt", Convert.ToString((int)events)},
                                            { "date", null}
                                     });
                                if (trigger != null)
                                {
                                    trigger.IsAdminCheckRequired = false;
                                    trigger.LastModifiedBy = new SessionStatusImpl().User.UserName;
                                    trigger.LastModifiedDate = DateTime.Now;

                                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                                    parameters.Add("currentUser", new SessionStatusImpl().User.DisplayName);
                                    DomainApiClient<Dto.EmployeeEventTrigger>.GetByAction<bool>("UdpateEventTrigger", parameters, trigger);

                                }

                                if (!string.IsNullOrEmpty(message) && LabelStatus != null)
                                {
                                    LabelStatus.Text = "Success";
                                }
                                newUsrIds.Add(newUsr.UserId);
                                sbUserNames.Append(", " + newUsr.DisplayName);

                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(message) && LabelStatus != null)
                                {
                                    LabelStatus.Text = "Failed";
                                    LabelStatus.ToolTip = message;
                                }
                                invalidMatrix.Add(newUsr.DisplayName);
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    if (LabelStatus != null)
                    {
                        LabelStatus.Text = "Not Selected";
                    }
                }
            }
            if (i > 0)
            {
                string errorMessage = string.Empty;
                if (invalidMatrix.Count != 0)
                {
                    errorMessage += string.Format("Provisioning default info is missing for the following user(s) {0}. ", String.Join(",", invalidMatrix));
                    MasterPage.ShowAlert(AlertType.Error, errorMessage);
                    return;
                }

                BindPageGridData();
                grdDeptChangedUsers.DataBind();
                MasterPage.ShowAlert(AlertType.Success, "Data Saved Successfully!"); 
            }
            else
            {
                MasterPage.ShowAlert(AlertType.Warning, "Please select at least one item"); 
            }

            string tmpUsrNamesStr = sbUserNames.Length > 0 ? sbUserNames.ToString().Substring(1, sbUserNames.Length - 1) : string.Empty;
          //  UIBasePage.SaveLogInfo(DeepIdentity.Logger.UIAction.Add, string.Format(Constants.UIAuditLog_Provisioning_DepartmentChange, tmpUsrNamesStr, SessionStatus.User.UserName));
        }
        protected void ouChangeRB_CheckedChanged(object sender, EventArgs e)
        { 
            BindPageGridData();
            grdDeptChangedUsers.Visible = true;
            grdDeptChangedUsers.DataBind();
        }
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            ouChangeRB_CheckedChanged(null,null);
        }
    }
}