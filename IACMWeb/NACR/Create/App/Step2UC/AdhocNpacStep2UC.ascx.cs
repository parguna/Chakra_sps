using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.NACR.Create.App.Step2UC
{
    public partial class AdhocNpacStep2UC : UIBaseStep2UC
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (LevelRadComboBox.SelectedValue == "-1" && LevelRadComboBox.Items.Count > 2)
            //{
            //    LevelRadComboBox.SelectedValue = "3";
            //}
        }
        public string Level99
        {
            get
            {
                if (ViewState["Level99"] == null)
                {
                    ViewState["Level99"] = MasterConfigHelper.NPACLevel99;
                }
                return Convert.ToString(ViewState["Level99"]);
            }
            set
            {
                ViewState["Level99"] = value;
            }
        }
        string SectionOfUser
        {
            get
            {
                return Convert.ToString(ViewState["SectionOfUser"]);
            }
            set
            {
                ViewState["SectionOfUser"] = value;
            }
        }
        public List<string> NPACSections
        {
            get
            {
                if (ViewState["NPACSections"] == null)
                {
                    ViewState["NPACSections"] = new List<string>();
                }
                return (List<string>)ViewState["NPACSections"];
            }
            set
            {
                ViewState["NPACSections"] = value;
            }
        }
        protected override void InitializeStep()
        {
            trSecurityQstns.Visible = false;
            this.Level99 = MasterConfigHelper.NPACLevel99;
            this.NPACSections = MasterConfigHelper.NPACSections;
            if (AdhocRequest != null)
            {
                SectionOfUser = null;
                BindLevels(AdhocRequest.RequesteeUserId);
                var user = DomainApiClient<Dto.User>.GetById(AdhocRequest.RequesteeUserId);
                BindSectionAndUserInfo(user);
                BindUnits();
                if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    trEndPointUser.Visible = true;
                    if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        OtherControls.Visible = true;

                    }
                    BindEndpointUsers(AdhocRequest.RequesteeUserId, AdhocRequest.EndpointSystemId.Value, AdhocRequest.AccountType);
                }
                if (AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    OtherControls.Visible = false;
                }
                if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    trEndPointUser.Visible = false;
                    OtherControls.Visible = true;
                    if (AdhocRequest.AccountType == AccountType.Service)
                    {
                        trSecurityQstns.Visible = true;
                    }
                }
                if (AdhocRequest.GetEntitlementDetail() != null || (Request.QueryString["AdhocRequestId"] != null && Convert.ToInt32(Request.QueryString["AdhocRequestId"]) > 0))
                {
                    BindAdhocRequest();
                }

                SetOtherInfo(user);

            }
        }

        void SetOtherInfo(Dto.User user)
        {


            LevelRadComboBox.Enabled = (user.FullTimeEmployee || user.EmployeeType == EmployeeType.TempStaff);

            if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                if (LevelRadComboBox.SelectedValue == "-1" && !(user.FullTimeEmployee || user.EmployeeType == EmployeeType.TempStaff))
                    LevelRadComboBox.SelectedValue = "3"; //Select Level 3 as requested.

                txtPhoneNumber.Text = !string.IsNullOrWhiteSpace(user.MobileNumber) ? user.MobileNumber : "";
                txtEmail.Text = user.EmailId;

                if (!string.IsNullOrWhiteSpace(user.OrganisationCode))
                {
                    var parameters = new Dictionary<string, string>();
                    parameters.Add("sectionCode", (!string.IsNullOrWhiteSpace(user.SectionCode) && !user.SectionCode.Equals("00000000"))? user.SectionCode : user.OrganisationCode);
                    var npacSection = DomainApiClient<Dto.NPACSectionInfo>.GetByAction<Dto.NPACSectionInfo>(DomainApiAction.Matrix.NPAC.GetNPACSectionByCode, parameters);
                    if (npacSection != null && !string.IsNullOrWhiteSpace(Convert.ToString(npacSection.NPACSection)))
                    {
                        SectionOfUser = npacSection.NPACSection;
                        ddlSections.SelectedValue = npacSection.NPACSection;
                    }
                }
            }
            else if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
            {
                var npacUser = DomainApiClient<Dto.NPACLevelInfo>.GetByAction<Dto.NPACUserInfo>(DomainApiAction.Endpoint.NPAC.GetPrimaryByDIUserId,
                 new Dictionary<string, string> { { "dIUserId", user.Id.ToString() }, { "endPointSystemId", AdhocRequest.EndpointSystemId.ToString() } });
                if (npacUser != null)
                {
                    txtPhoneNumber.Text = !string.IsNullOrWhiteSpace(npacUser.Pager) ? npacUser.Pager.Trim() : (!string.IsNullOrWhiteSpace(user.MobileNumber) ? user.MobileNumber : "");
                    txtEmail.Text = !string.IsNullOrWhiteSpace(npacUser.EmailId) ? npacUser.EmailId : user.EmailId;

                    if (!string.IsNullOrWhiteSpace(Convert.ToString(npacUser.Levels)))
                        LevelRadComboBox.SelectedValue = Convert.ToString(npacUser.Levels);
                    else if (!(user.FullTimeEmployee || user.EmployeeType == EmployeeType.TempStaff))
                        LevelRadComboBox.SelectedValue = "3";//Select Level 3 as requested.

                    if (!string.IsNullOrWhiteSpace(Convert.ToString(npacUser.Section)))
                    {
                        SectionOfUser = npacUser.Section;
                        ddlSections.SelectedValue = npacUser.Section;
                        if (ddlSections.SelectedIndex > 0 && NPACSections.Exists(i => i.Equals(ddlSections.SelectedValue, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            divUnit.Visible = true;
                        }
                    }
                    else
                        ddlSections.SelectedIndex = 0;
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(npacUser.Zone)))
                        ddlUnit.SelectedValue = npacUser.Zone;

                }
            }
            ddlSections.Enabled = !(user.FullTimeEmployee || user.EmployeeType == EmployeeType.TempStaff);
            
        }
        void BindAdhocRequest()
        {
            var Entitlement = (AdHocNPACRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null)
            {
                if (EndPointUserDropDownList.Items.FindByValue(Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId)) != null)
                    EndPointUserDropDownList.SelectedValue = Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId);

                if (LevelRadComboBox.Items.FindItemByValue(Entitlement.Entitlement == null ? "--Select Level--" : Convert.ToString(Entitlement.Entitlement)) != null)
                    LevelRadComboBox.SelectedValue = Entitlement.Entitlement == null ? "--Select Level--" : Convert.ToString(Entitlement.Entitlement);

                if (ddlSections.Items.FindItemByValue(string.IsNullOrWhiteSpace(Entitlement.Section) ? "--Select Section--" : Convert.ToString(Entitlement.Section)) != null)
                    ddlSections.SelectedValue = Entitlement.Section == null ? "--Select Section--" : Convert.ToString(Entitlement.Entitlement);

                if (ddlSections.Items.FindItemByValue(string.IsNullOrWhiteSpace(Entitlement.Section) ? "--Select Unit--" : Convert.ToString(Entitlement.Unit)) != null)
                    ddlSections.SelectedValue = Entitlement.Unit == null ? "--Select Unit--" : Convert.ToString(Entitlement.Unit);

                txtPhoneNumber.Text = !string.IsNullOrWhiteSpace(Entitlement.PhoneNumber) ? Entitlement.PhoneNumber.Trim() : "";
                txtEmail.Text = Entitlement.Email;
            }
        }

        public Dictionary<int, string> LevelData
        {
            get
            {
                if (ViewState["LevelData"] != null)
                {
                    return (Dictionary<int, string>)ViewState["LevelData"];
                }
                return null;
            }
            set
            {
                ViewState["LevelData"] = value;
            }
        }
        void BindLevels(long RequesteeID)
        {
            LevelData = DomainApiClient<Dto.NPACLevelInfo>.GetByAction<Dictionary<int, string>>(DomainApiAction.Endpoint.NPAC.GetNPACLevelInfoList,
                 new Dictionary<string, string> { { "RequesteeId", RequesteeID.ToString() } });

            LevelData.Add(-1, "--Select Level--");
            bindLevelControls(LevelData);
        }

        void BindSectionAndUserInfo(Dto.User user)
        {
            ddlSections.Items.Clear();
            var sections = new Dictionary<string, string>();
            sections.Add("SELECT", "--Select Section--");

            if (user.FullTimeEmployee || user.EmployeeType == EmployeeType.TempStaff)
            {
                //if (!string.IsNullOrWhiteSpace(user.OrgUnit))
                {
                    var parameters = new Dictionary<string, string>();
                    parameters.Add("isValidRecords", bool.TrueString);
                    parameters.Add("isAllRecords", bool.TrueString);

                    List<Dto.NPACSectionInfo> nPACSections = DomainApiClient<Dto.NPACSectionInfo>.GetByAction<List<Dto.NPACSectionInfo>>(DomainApiAction.Matrix.NPAC.GetAllNPACSections, parameters);
                    var tmpSections = nPACSections?.ToDictionary(k => k.NPACSection, v => $"{v.NPACSection} - {v.HRSectionName}");
                    foreach (var sec in tmpSections)
                    {
                        sections.Add(sec.Key, sec.Value);
                    }
                }
                ddlSections.Enabled = false;
            }
            else
            {
                var tmpSections = MasterConfigHelper.NPACNonEmployeeSection;
                if (tmpSections != null)
                {
                    foreach (var sec in tmpSections?.Split(','))
                    {
                        sections.Add(sec, sec);
                    }
                }
            }

            ddlSections.DataSource = sections;
            ddlSections.DataTextField = "value";
            ddlSections.DataValueField = "key";
            ddlSections.DataBind();
        }

        void BindUnits()
        {
            var tmpUnits = MasterConfigHelper.NPACUnits;
            var units = new Dictionary<string, string>();
            units.Add("SELECT", "--Select Unit/Zone--");
            if (tmpUnits != null)
            {
                foreach (var un in tmpUnits?.Split(','))
                {
                    units.Add(un, un);
                }
            }

            ddlUnit.DataSource = units;
            ddlUnit.DataTextField = "value";
            ddlUnit.DataValueField = "key";
            ddlUnit.DataBind();
            ddlUnit.ClearSelection();
            divUnit.Visible = false;
        }

        void bindLevelControls(Dictionary<int, string> dic)
        {
            LevelRadComboBox.Items.Clear();
            LevelRadComboBox.DataSource = dic.OrderBy(x => x.Key);
            LevelRadComboBox.DataTextField = "value";
            LevelRadComboBox.DataValueField = "key";
            LevelRadComboBox.DataBind();
        }


        void BindEndpointUsers(long userId, long endpointSystemId, AccountType accountType)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("userid", userId.ToString());
            dict.Add("endpointsystemid", endpointSystemId.ToString());
            dict.Add("accountType", Convert.ToString((int)accountType));
            AccountReconDto list = DomainApiClient<Dto.ADSUserInfo>.GetByAction<AccountReconDto>("GetADUsersByDIUserId", dict);
            Session["EndpointUserId"] = null;
            if (list != null && list.AccountReconDictionary != null)
            {
                EndPointUserDropDownList.Items.Clear();
                DropDownHelper.DropDownListBind(EndPointUserDropDownList, list.AccountReconDictionary, "Value", "Key", null, true, "-- Select EndPoint User --", false);
                if (EndPointUserDropDownList.SelectedItem != null && EndPointUserDropDownList.SelectedIndex > 0 && AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    Session["EndpointUserId"] = Convert.ToInt32(EndPointUserDropDownList.SelectedValue);
                }

                if (list.EntitlementReconcilations != null && AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    List<int> entitlment = list.EntitlementReconcilations.Where(e => e.EntitlementType == ((int)EntitlementType.Level)).Select(e => Convert.ToInt32(e.Entitlement)).ToList();
                    var levels = LevelData.Where(key => entitlment.Contains(key.Key)).ToDictionary(key => key.Key, key => key.Value);
                    bindLevelControls(levels);
                }
            }
        }

        protected void EndPointUserDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            afterSave();
            Session["EndpointUserId"] = null;
            Session["UserAccountName"] = null;
            if(AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd && EndPointUserDropDownList.SelectedIndex > 0)
            {
                var user = DomainApiClient<Dto.User>.GetById(AdhocRequest.RequesteeUserId);
                SetOtherInfo(user);
            }
            if (EndPointUserDropDownList.SelectedItem != null && EndPointUserDropDownList.SelectedIndex > 0 && AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                Session["EndpointUserId"] = Convert.ToInt32(EndPointUserDropDownList.SelectedValue);
                Session["UserAccountName"] = EndPointUserDropDownList.Text;

            }
        }

        public override bool Validate()
        {
            string Message = "Please Select the ";
            string ControlMessage = "";
            var user = DomainApiClient<Dto.User>.GetById(AdhocRequest.RequesteeUserId);

            if (AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
            {
                if ((LevelRadComboBox.SelectedValue == "0" || LevelRadComboBox.SelectedValue == "-1") || (trEndPointUser.Visible && EndPointUserDropDownList.SelectedIndex <= 0))
                {

                    if (trEndPointUser.Visible && EndPointUserDropDownList.SelectedIndex <= 0)
                    {
                        ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " EndPoint User" : ControlMessage + " and EndPoint User";
                    }
                    if ((LevelRadComboBox.SelectedValue == "0" || LevelRadComboBox.SelectedValue == "-1"))
                    {
                        ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Level" : ControlMessage + " and Level";
                    }
                }
                if (AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
                {
                    if (!(user.FullTimeEmployee || user.EmployeeType == EmployeeType.TempStaff) && LevelRadComboBox.SelectedValue != Level99 && ddlSections.SelectedIndex <= 0)
                        ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " Section" : ControlMessage + " and Section";
                    else if (string.IsNullOrWhiteSpace(txtEmail.Text))
                        ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " Email" : ControlMessage + " and Email";
                }
                if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && AdhocRequest.AccountType == AccountType.Service)
                {
                    if (string.IsNullOrEmpty(AdhocRequest.SecurityQuestions))
                    {
                        ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Service account compliance check" : ControlMessage + ", Service account compliance check";
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(ControlMessage))
            {
                Message = Message + ControlMessage + (ControlMessage.IndexOf(",") > 0 ? ";" : ".");
                ShowAlertMessage(Infrastructure.AlertType.Error, Message);
                return false;
            }
            else if (AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount && !UtilityMethods.IsValidEmailAddress(txtEmail.Text))
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, "Invalid email address.");
                return false;
            }
            else if (AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount && !string.IsNullOrWhiteSpace(txtPhoneNumber.Text) && !UtilityMethods.IsValidMobileNumber(txtPhoneNumber.Text))
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, "Please enter a valid Singapore mobile number (Length must be 8 characters only & Starting with either 8 or 9)");
                return false;
            }
                
            if ((user.FullTimeEmployee || user.EmployeeType == EmployeeType.TempStaff) && (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount))
            {
                if (ddlSections.SelectedIndex <= 0 && LevelRadComboBox.SelectedValue != Level99)
                {
                    ShowAlertMessage(Infrastructure.AlertType.Error, "No NMACS Section found, please contact ITD AMS Team");
                    return false;
                }
            }
            if (AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount && ddlSections.SelectedIndex > 0 && NPACSections.Exists(i => i.Equals(ddlSections.SelectedValue, StringComparison.InvariantCultureIgnoreCase)) && ddlUnit.SelectedIndex == 0)
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, "Unit/Zone is mandatory");
                return false;
            }
            return true;
        }

        protected override IEntitlementDetail GetEntitlementDetail()
        {
            AdHocNPACRequest request = new AdHocNPACRequest();
            if (this.AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
            {
                request.EndpointUserId = (EndPointUserDropDownList.SelectedIndex > 0) ? Convert.ToInt32(EndPointUserDropDownList.SelectedItem.Value) : 0;

                if (EndPointUserDropDownList.SelectedItem != null)
                {
                    request.DisplayAccountName = EndPointUserDropDownList.SelectedItem.Text;
                }
            }
            else
            {
                request.EndpointUserId = 0;
            }
            if (this.AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
            {
                request.Entitlement = LevelRadComboBox.SelectedValue == "0" ? null : LevelRadComboBox.SelectedValue;
                request.EntitlementType = EntitlementType.Level;
                request.Section = ddlSections.SelectedIndex > 0 ? ddlSections.SelectedValue.Trim() : string.Empty;
                request.PhoneNumber = txtPhoneNumber.Text?.Trim();
                request.Unit = ddlUnit.SelectedIndex > 0 ? ddlUnit.SelectedValue.Trim() : string.Empty;
                request.Email = txtEmail.Text?.Trim();
            }
            
            return request;
        }

        protected override void afterSave()
        {
            base.afterSave();
            LevelRadComboBox.SelectedIndex = 0;
            ddlSections.SelectedIndex = 0;
            ddlUnit.SelectedIndex = 0;
            txtEmail.Text = string.Empty;
            txtPhoneNumber.Text = string.Empty;
            if(AdhocRequest != null)
            {
                var user = DomainApiClient<Dto.User>.GetById(AdhocRequest.RequesteeUserId);
                if (LevelRadComboBox.SelectedValue == "-1" && !user.FullTimeEmployee)
                    LevelRadComboBox.SelectedValue = "3"; //Select Level 3 as requested.
            }
            
        }

        protected void ddlSections_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {

            if (ddlSections.SelectedIndex > 0 && NPACSections.Exists(i => i.Equals(ddlSections.SelectedValue, StringComparison.InvariantCultureIgnoreCase)))
            {
                //ddlUnit.ClearSelection();
                divUnit.Visible = true;
            }
            else
            {
                ddlUnit.ClearSelection();
                divUnit.Visible = false;
            }
        }

        protected void LevelRadComboBox_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            try
            {
                //ddlSections.Enabled = LevelRadComboBox.SelectedValue == Level99;
                if (!string.IsNullOrWhiteSpace(SectionOfUser))
                    ddlSections.SelectedValue = SectionOfUser;
                //if (LevelRadComboBox.SelectedValue != Level99)
                {
                    ddlSections_SelectedIndexChanged(null, null);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}