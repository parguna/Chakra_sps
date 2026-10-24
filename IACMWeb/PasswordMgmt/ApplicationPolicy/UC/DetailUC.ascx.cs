using System;
using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using DI.Chakra.Infrastructure.Enums;
using Telerik.Web.UI;
using DI.Chakra.Logger;

namespace DI.Chakra.Web.PasswordMgmt.ApplicationPolicy.UC
{
    public partial class DetailUC : UIBaseUC, IAccessControl
    {
        #region Variables     
        private const string V_EntityID = "V_EntityID";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        #endregion

        #region Properties
        public long EntityId
        {
            get
            {
                if (ViewState[V_EntityID] == null)
                    return 0;
                return (long)ViewState[V_EntityID];
            }
            set
            {
                ViewState[V_EntityID] = value;
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

        private void ViewOnlyMode()
        {
            ContentWrapper.Enabled = false;
        }

        private Dictionary<long, string> Endpointlist
        {
            get
            {
                if (ViewState["ENPOINTLIST"] != null)
                {
                    return (Dictionary<long, string>)ViewState["ENPOINTLIST"];
                }
                return new Dictionary<long, string>();
            }
            set
            {
                ViewState["ENPOINTLIST"] = value;
            }
        }

        private PasswordPolicyEndpoint CheckPolicyExists(long Key)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            PasswordPolicyEndpoint policyEndpoint = null;
            parameter.Add("endpointSystemId", Key.ToString());
            policyEndpoint = DomainApiClient<PasswordPolicyEndpoint>.GetByAction<PasswordPolicyEndpoint>
                (DomainApiAction.Configuration.PasswordPolicy.GetEndpointPasswordPolicy, parameter);
            return policyEndpoint;
        }
        #endregion

        #region Methods
        private void SetPage()
        {
            var objPasswordPolicy = DomainApiClient<PasswordPolicy>.GetById(EntityId);
            if (null == objPasswordPolicy)
            {
                return;
            }
            txtPolicyName.Text = objPasswordPolicy.Name;
            txtPasswordLength.Text = objPasswordPolicy.PasswordLength.ToString();
            txtMaximumLength.Text = objPasswordPolicy.PasswordMaximumLength.ToString();
            txtMinimumLength.Text = objPasswordPolicy.PasswordMinimumLength.ToString();
            chkNoOfSplCharNotAllowed.Checked = objPasswordPolicy.SpecialCharNotAllowed;
            if (chkNoOfSplCharNotAllowed.Checked != true)
            {
                txtNoofSpecialChar.Text = objPasswordPolicy.NoOfSpecialCharacters.ToString();
                txtNoOfSplCharNotAllowed.Text = objPasswordPolicy.NotAllowedCharacters;
            }
            else
            {
                txtNoofSpecialChar.Enabled = txtNoOfSplCharNotAllowed.Enabled = false;
            }
            chkNoOfAlphabets.Checked = objPasswordPolicy.AlphabetsNotAllowed;
            if (chkNoOfAlphabets.Checked == true)
            {
                txtNoOfUCAlphabets.Enabled = false;
                txtNoOfLCAlphabets.Enabled = false;
            }
            else
            {
                txtNoOfUCAlphabets.Text = objPasswordPolicy.NoOfUpperCaseAlphabets.ToString();
                txtNoOfLCAlphabets.Text = objPasswordPolicy.NoOfLowerCaseAlphabets.ToString();
            }
            chkNoOfNumeric.Checked = objPasswordPolicy.NumericCharacterNotAllowed;
            if (chkNoOfNumeric.Checked == true)
            {
                txtNoOfNumeric.Enabled = false;
            }
            else
            {
                txtNoOfNumeric.Text = objPasswordPolicy.NoOfNumericCharacters.ToString();
            }
            chkPwdUsername.Checked = objPasswordPolicy.UserNameNotAllowedInPassword;
            txtNoOfDaysPasswordChangeAllowed.Text = objPasswordPolicy.NoOfDaysPasswordChangeAllowed.ToString();
            txtNotAllowedKeywords.Text = objPasswordPolicy.NotAllowedKeywords;

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("policyId", objPasswordPolicy.Id.ToString());
            var EndpointPolicy = DomainApiClient<PasswordPolicyEndpoint>.GetByAction<List<PasswordPolicyEndpoint>>
                (DomainApiAction.Configuration.PasswordPolicyEndpoint.GetByPolicyId, parameter);

            EndpointPolicy.ForEach(lq =>
            {
                var forms = Endpointlist;
                if (!forms.ContainsKey(lq.EndpointSystemId))
                {
                    forms.Add(lq.EndpointSystemId, lq.Endpoint.DisplayName);
                }
                Endpointlist = forms;
            });

            if (EndpointPolicy.Count > 0)
                RebindFormsGrid();
        }

        private int ConvertToInt(string name)
        {
            if (string.IsNullOrEmpty(name))
                return 0;
            return Convert.ToInt32(name);
        }

        private long SavePolicy()
        {
            PasswordPolicy _PasswordPolicy = new PasswordPolicy();
            _PasswordPolicy.Id = EntityId;
            _PasswordPolicy.Name = txtPolicyName.Text;
            _PasswordPolicy.SpecialCharNotAllowed = chkNoOfSplCharNotAllowed.Checked;
            _PasswordPolicy.UserNameNotAllowedInPassword = chkPwdUsername.Checked;
            _PasswordPolicy.AlphabetsNotAllowed = chkNoOfAlphabets.Checked;
            _PasswordPolicy.NumericCharacterNotAllowed = chkNoOfNumeric.Checked;
            _PasswordPolicy.NotAllowedKeywords = txtNotAllowedKeywords.Text;
            
            if (txtMinimumLength.Text != "")
                _PasswordPolicy.PasswordMinimumLength = ConvertToInt(txtMinimumLength.Text);
            if (txtMaximumLength.Text != "")
                _PasswordPolicy.PasswordMaximumLength = ConvertToInt(txtMaximumLength.Text);
            if (txtNoOfDaysPasswordChangeAllowed.Text != "")
                _PasswordPolicy.NoOfDaysPasswordChangeAllowed = ConvertToInt(txtNoOfDaysPasswordChangeAllowed.Text);
            _PasswordPolicy.IsActive = true;
            if (chkNoOfSplCharNotAllowed.Checked == false)
            {
                if (txtNoofSpecialChar.Text != "")
                    _PasswordPolicy.NoOfSpecialCharacters = ConvertToInt(txtNoofSpecialChar.Text);
                _PasswordPolicy.NotAllowedCharacters = txtNoOfSplCharNotAllowed.Text;
            }
            if (chkNoOfAlphabets.Checked == false)
            {
                if (txtNoOfUCAlphabets.Text != "")
                    _PasswordPolicy.NoOfUpperCaseAlphabets = ConvertToInt(txtNoOfUCAlphabets.Text);
                if (txtNoOfLCAlphabets.Text != "")
                    _PasswordPolicy.NoOfLowerCaseAlphabets = ConvertToInt(txtNoOfLCAlphabets.Text);
            }
            if (chkNoOfNumeric.Checked == false)
            {
                if (txtNoOfNumeric.Text != "")
                    _PasswordPolicy.NoOfNumericCharacters = ConvertToInt(txtNoOfNumeric.Text);
            }
            _PasswordPolicy.PasswordLength = ConvertToInt(txtPasswordLength.Text);

            DomainApiClient<Dto.PasswordPolicy>.Save(_PasswordPolicy);
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("policyName", txtPolicyName.Text);
            long policyId = DomainApiClient<PasswordPolicy>.GetByAction<long>
                (DomainApiAction.Configuration.PasswordPolicy.GetPasswordPolicyIdByName, parameter);
            return policyId;
        }
        private void Save(PasswordPolicyEndpoint policyEndpoint, long Key, long policyId)
        {
            try
            {
                SessionStatusImpl session = new SessionStatusImpl();

                // long policyId = _PasswordPolicy.Id;
                //Dictionary<string, string> parameter = new Dictionary<string, string>();
                //parameter.Add("policyName", txtPolicyName.Text);
                //long policyId = DomainApiClient<PasswordPolicy>.GetByAction<long>(DomainApiAction.Configuration.PasswordPolicy.GetPasswordPolicyIdByName, parameter);
                if (policyId > 0 && policyEndpoint == null && Key != -1)
                {
                    List<PasswordPolicyEndpoint> policyEndpoints = new List<PasswordPolicyEndpoint>();
                    //foreach (var entry in Endpointlist)
                    //{
                    PasswordPolicyEndpoint policyEndPoint = new PasswordPolicyEndpoint();
                    policyEndPoint.EndpointSystemId = Key;
                    policyEndPoint.PolicyId = policyId;
                    policyEndPoint.IsActive = true;
                    policyEndpoints.Add(policyEndPoint);
                    //}
                    if (policyEndpoints.Count > 0)
                    {
                        DomainApiClient<PasswordPolicyEndpoint>.SaveList(policyEndpoints);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private List<PasswordPolicyEndpoint> RemovePolicyEndPoints(long policyId)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("policyId", policyId.ToString());
            List<PasswordPolicyEndpoint> endPoints = DomainApiClient<PasswordPolicyEndpoint>.GetByAction<List<PasswordPolicyEndpoint>>(DomainApiAction.Configuration.PasswordPolicyEndpoint.GetByPolicyId, parameter);
            foreach (PasswordPolicyEndpoint endpoint in endPoints)
            {
                if (!Endpointlist.ContainsKey(endpoint.EndpointSystemId))
                    DomainApiClient<PasswordPolicyEndpoint>.DeleteById(endpoint.Id);
            }
            return endPoints;
        }
        private void RebindFormsGrid()
        {
            ApplicationGrid.DataSource = Endpointlist;
            ApplicationGrid.Rebind();
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.ActionClicked += ActionClicked;
            if (!IsPostBack)
            {
                if (UIMode == UIMode.Edit || UIMode == UIMode.View)
                {
                    if (!ContextObjectId.HasValue)
                        return;
                    EntityId = ContextObjectId.Value;
                    SetPage();
                }
                if (UIMode == UIMode.View)
                    ViewOnlyMode();
            }
        }

        protected void CheckboxCheckChanged(object sender, EventArgs e)
        {
            var rad = (CheckBox)sender;
            string checkedCheckboxId = rad.ID.ToString();
            if (checkedCheckboxId == "chkNoOfSplCharNotAllowed")
            {
                txtNoofSpecialChar.Enabled = txtNoOfSplCharNotAllowed.Enabled = true;
                txtNoofSpecialChar.Text = String.Empty;
                if (rad.Checked == true)
                {
                    txtNoofSpecialChar.Enabled = txtNoOfSplCharNotAllowed.Enabled = false;
                }
            }
            else if (checkedCheckboxId == "chkNoOfAlphabets")
            {
                txtNoOfUCAlphabets.Enabled = true;
                txtNoOfLCAlphabets.Enabled = true;
                txtNoOfUCAlphabets.Text = String.Empty;
                txtNoOfLCAlphabets.Text = String.Empty;
                if (rad.Checked == true)
                {
                    txtNoOfUCAlphabets.Enabled = false;
                    txtNoOfLCAlphabets.Enabled = false;
                }
            }
            else
            {
                txtNoOfNumeric.Enabled = true;
                txtNoOfNumeric.Text = String.Empty;
                if (rad.Checked == true)
                {
                    txtNoOfNumeric.Enabled = false;
                }
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/PasswordMgmt/ApplicationPolicy/Add.aspx");
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                try
                {
                    if (UIMode != Infrastructure.Enums.UIMode.View)
                    {
                        
                        if (string.IsNullOrEmpty(txtPasswordLength.Text))
                        {
                            MasterPage.ShowAlert(AlertType.Warning, "Please Enter Password Length");
                            return;
                        }
                        if (string.IsNullOrEmpty(txtPolicyName.Text))
                        {
                            MasterPage.ShowAlert(AlertType.Warning, "Please Enter Policy Name");
                            return;
                        }
                        if (string.IsNullOrEmpty(txtMaximumLength.Text))
                        {
                            MasterPage.ShowAlert(AlertType.Warning, "Please Enter Maximum Length");
                            return;
                        }
                        if (string.IsNullOrEmpty(txtMinimumLength.Text))
                        {
                            MasterPage.ShowAlert(AlertType.Warning, "Please Enter Minimum Length");
                            return;
                        }
                        if (ConvertToInt(txtMinimumLength.Text) > ConvertToInt(txtMaximumLength.Text))
                        {
                            MasterPage.ShowAlert(AlertType.Warning, "Minimum length cannot exceed maximum length");
                            return;
                        }
                        if (chkNoOfSplCharNotAllowed.Checked == true && chkNoOfAlphabets.Checked == true && chkNoOfNumeric.Checked == true)
                        {
                            MasterPage.ShowAlert(AlertType.Warning,
                                "Invalid Password condition - All the three Not Allowed conditions cannot be set together");
                            return;
                        }
                        if (ConvertToInt(txtPasswordLength.Text) < ConvertToInt(txtMinimumLength.Text) || ConvertToInt(txtPasswordLength.Text) > ConvertToInt(txtMaximumLength.Text))
                        {
                            MasterPage.ShowAlert(AlertType.Warning, "Password Length should be between Minimum and Maximum Length");
                            return;
                        }
                        int totalMinimumCount = (ConvertToInt(txtNoofSpecialChar.Text) + (ConvertToInt(txtNoOfUCAlphabets.Text))
                            + (ConvertToInt(txtNoOfLCAlphabets.Text)) + (ConvertToInt(txtNoOfNumeric.Text)));
                        if (totalMinimumCount > ConvertToInt(txtMaximumLength.Text))
                        {
                            MasterPage.ShowAlert(AlertType.Warning,
                                "Invalid Password condition - Total of Minimum values of each type cannot exceed Maximum Password Length");
                            return;
                        }
                    }
                    if (Endpointlist.Count > 0)
                    {
                        if (UIMode == UIMode.Edit)
                        {
                            List<long> newlyAdded = new List<long>();
                            List<PasswordPolicyEndpoint> endPoints = RemovePolicyEndPoints(EntityId);
                            foreach (var entry in Endpointlist)
                            {
                                PasswordPolicyEndpoint endPoint = endPoints.Find(p => p.EndpointSystemId == entry.Key);
                                if (endPoint == null)
                                    newlyAdded.Add(entry.Key);
                            }
                            foreach (var key in newlyAdded)
                            {
                                PasswordPolicyEndpoint policyEndPoint = CheckPolicyExists(key);
                                if (policyEndPoint != null)
                                {
                                    MasterPage.ShowAlert(AlertType.Warning, "Policy Already Exists For this EndPoint");
                                    return;
                                }
                            }
                        }
                        int i = 0;
                        long policyId = 0;
                        foreach (var entry in Endpointlist)
                        {
                            PasswordPolicyEndpoint policyEndPoint = CheckPolicyExists(entry.Key);

                            if (policyEndPoint != null && UIMode == UIMode.Add)
                            {
                                MasterPage.ShowAlert(AlertType.Warning, "Policy Already Exists For this EndPoint");
                                return;
                            }
                            if (i == 0)
                            {
                                policyId = SavePolicy();
                            }

                            Save(policyEndPoint, entry.Key, policyId);
                            DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Application Policy - Save");
                            i++;
                        }
                    }
                    else
                    {
                        if (UIMode == UIMode.Edit)
                        {
                            RemovePolicyEndPoints(EntityId);
                        }
                        long policyId = SavePolicy();
                        Save(null, -1, policyId);
                        DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Application Policy - Save");
                    }
                    Response.Redirect("~/PasswordMgmt/ApplicationPolicy/List.aspx");
                }
                catch (Exception)
                {
                    MasterPage.ShowAlert(AlertType.Error, "Error while saving Policy.");
                    throw;
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/PasswordMgmt/ApplicationPolicy/List.aspx");
        }

        protected void AddApplicationButton_Click(object sender, EventArgs e)
        {
            if (ApplicationAutoCompleteBox.Entries.Count != 0)
            {
                long EndpointID = Convert.ToInt64(ApplicationAutoCompleteBox.Entries[0].Value);
                string form = ApplicationAutoCompleteBox.Entries[0].Text;
                var forms = Endpointlist;
                if (!forms.ContainsKey(EndpointID))
                {
                    forms.Add(EndpointID, form);
                }
                Endpointlist = forms;
                RebindFormsGrid();
                ApplicationAutoCompleteBox.Entries.Clear();
            }
        }

        protected void ApplicationGrid_DeleteCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            GridDataItem dataItem = e.Item as GridDataItem;
            long key = Convert.ToInt64(dataItem.GetDataKeyValue("Key"));

            Endpointlist.Remove(key);
            ApplicationGrid.DataSource = this.Endpointlist;
            ApplicationGrid.Rebind();
        }
        #endregion
    }
}

