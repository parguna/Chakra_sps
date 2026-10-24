using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.EndPoints.EndpointSystem.Common
{
    public partial class EndpointInfoUC : DI.Chakra.Web.UIBaseUC
    {
        #region Members

        private const string V_ID = "ENTITY_ID";
        private const string V_ENDPOINTTYPEID = "ENDPOINTTYPE_ID";
        private EndPoint mEndpoint;
        private SessionStatusImpl sessionstatus = new SessionStatusImpl();
        private bool isEdit = false;

        #endregion

        #region Properties

        public long EndpointTypeId
        {
            get
            {
                if (ViewState[V_ENDPOINTTYPEID] == null)
                    return 1;
                return (long)ViewState[V_ENDPOINTTYPEID];
            }
            set
            {
                ViewState[V_ENDPOINTTYPEID] = value;
            }
        }

        public bool IsTrustedSourceSelected
        {
            get { return chkIsTrustedSource.Checked; }
        }

        public bool IsAttestationSelected
        {
            get { return chkIncludeInAttestation.Checked; }
        }

        public bool MakeTSourceVisible
        {
            set
            {
                IsTrustedSourceDiv.Visible = value;
            }
        }

        public bool IsEdit
        {
            set { isEdit = value; }
            get { return isEdit; }
        }

        public bool AllowPaswordReset
        {
            get { return resetform.Visible = false; }
            set { resetform.Visible = value; }
        }
        public EndPoint EndpointSystem
        {
            get
            {
                mEndpoint = new EndPoint();
                mEndpoint = GetEndpointSystem();
                return mEndpoint;
            }
            set
            {
                mEndpoint = value;
                this.EntityId = (long)mEndpoint.Id;
                SetEndpointSystem();
            }

        }
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

        #endregion

        #region Events

        protected void EndpointTypeUC_EndpointTypeChanged(EndPointSystemType EndPointSystemType)
        {
            if (EndPointSystemType != null)
                Response.Redirect("~/Admin/Endpoint/Add.aspx?Type=" + EndPointSystemType.EndpointSystemTypeName.ToString());
        }

        #endregion

        #region Methods

        public void DisplayViewMode()
        {
            ControlStatus(contentwrapper, true);
        }
        private void ControlStatus(Control control, bool isDisable)
        {
            foreach (Control c in control.Controls)
                try
                {
                    if (c.HasControls())
                        ControlStatus(c, isDisable);
                    else
                    {
                        if (isDisable)
                        {
                            if (c is WebControl)
                                ((WebControl)c).Enabled = false;
                        }
                        else
                        {
                            if (c is WebControl)
                                ((WebControl)c).Enabled = true;
                        }
                    }
                }
                catch (Exception)
                {
                }
        }

        void ForNonConnectedSystems(long endpointType)
        {
            if (endpointType == Convert.ToInt64(EndpointType.InhouseSystems))
            {
                divFunctionLevel.Visible = true;
                divCanAutomate.Visible = true;
                resetform.Visible = false;
                SecondaryAccountDiv.Visible = true;
            }
            else
            {
                divFunctionLevel.Visible = false;
                divCanAutomate.Visible = false;
                chkFunctionLevel.Checked = false;
                resetform.Visible = true;
                SecondaryAccountDiv.Visible = true;
            }
        }

        public void InitializePage()
        {
            EndpointUC.Initialize();
            if (!IsPostBack)
            {
                if (EndpointTypeId > 0)
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("id", EndpointTypeId.ToString());
                    EndpointUC.SelectedApplication = DomainApiClient<Dto.EndPointSystemType>.GetByAction<EndPointSystemType>(DomainApiAction.Endpoint.EndPointSystemType.GetByIdIncludingEndpoints, parameter);
                    //EndpointUC.SelectedEndpoint = DomainApiClient<Dto.EndPoint>.GetById(EntityId);
                    this.EndpointTypeId = (long)EndpointUC.SelectedApplication.Id;

                    ForNonConnectedSystems(EndpointTypeId);
                }
                else if (!(Request.QueryString.HasKeys()))
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("id", ((long)EndpointType.ActiveDirectory).ToString());
                    EndpointUC.SelectedApplication = DomainApiClient<Dto.EndPointSystemType>.GetByAction<EndPointSystemType>(DomainApiAction.Endpoint.EndPointSystemType.GetByIdIncludingEndpoints, parameter);
                    this.EndpointTypeId = EndpointUC.SelectedApplication == null ? 0 : (long)EndpointUC.SelectedApplication.Id;
                    //EndpointUC.SelectedEndpoint = DomainApiClient<Dto.EndPoint>.GetById(EntityId);
                }



                //var endpoints = Enum.GetValues(typeof(ApplicationType));
                //var res = new Dictionary<int, string>();
                //foreach (ApplicationType val in endpoints)
                //{
                //    var tempEndpointTypeId = UtilityMethods.GetCustomEnumAttributeValueIfExists<EndpointTypeAttribute>(val);

                //    if (tempEndpointTypeId == EndpointTypeId.ToString())
                //    {
                //        var name = Enum.GetName(typeof(ApplicationType), val);
                //        res.Add((int)val, name);
                //    }
                //}

                //ddlApplicationTypes.DataSource = res;
                ddlApplicationTypes.DataSource = EnumExt.GetAll<ApplicationType>();
                ddlApplicationTypes.DataTextField = "Value";
                ddlApplicationTypes.DataValueField = "Key";
                ddlApplicationTypes.DataBind();
            }
        }

        private void SetEndpointSystem()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", ((long)mEndpoint.EndpointSystemTypeId).ToString());
            EndpointType epTypename = DomainApiClient<Dto.EndPointSystemType>.GetByAction<EndpointType>(DomainApiAction.Endpoint.EndPointSystemType.GetEndPointTypeName, parameter);
            EndpointUC.SelectedApplication = mEndpoint.EndPointSystemType;
            var ep = DomainApiClient<Dto.EndPoint>.GetById(Convert.ToInt64(mEndpoint?.Id));
            EndpointUC.SelectedEndpoint = ep;
            if (mEndpoint.OwnerUserId.HasValue)
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("id", ((long)mEndpoint.OwnerUserId.Value).ToString());
                UserSearchUC.SelectedUser = DomainApiClient<Dto.User>.GetByAction<User>(DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, param);
            }
            if(mEndpoint != null && mEndpoint.Id > 0)
            {
                divApplication.Visible = true;
            }
            txtName.Text = mEndpoint.DisplayName;
            txtApplicationCode.Text = mEndpoint.ApplicationCode;
            txtEndPointDesc.Text = mEndpoint.Description;
            chkIsTrustedSource.Checked = mEndpoint.IsTrustedSource;
            chkIncludeInAttestation.Checked = (bool)mEndpoint.IncludeInAttestation;
            chkAllowPaswordReset.Checked = mEndpoint.IsPasswordResetAllowed;
            chkunlockAccount.Checked = mEndpoint.IsUnlockAllowed;
            chkIsSODApplicable.Checked = mEndpoint.IsSodApplicable;
            chkSupportSecondaryAccount.Checked = mEndpoint.SupportSecondaryAccount;
            chkIsActive.Checked = mEndpoint.IsActive;
            chkCanAutomate.Checked = mEndpoint.CanAutomate;
            chkFunctionLevel.Checked = mEndpoint.IsFunctionLevel;

            ddlApplicationTypes.SelectedValue = Convert.ToString((byte)ep?.ApplicationType);

            EndpointUC.DisabledApplicationType = true;

            if (mEndpoint.IsTrustedSource)
                resetform.Visible = attestationDiv.Visible = SODApplicableDiv.Visible = SecondaryAccountDiv.Visible = false;
            else
            {
                resetform.Visible = attestationDiv.Visible = SODApplicableDiv.Visible = SecondaryAccountDiv.Visible = true;
            }
        }

        private EndPoint GetEndpointSystem()
        {
            return new EndPoint()
            {
                Id = this.EntityId,
                EndpointSystemTypeId = this.EndpointTypeId,
                OwnerUserId = UserSearchUC.SelectedUser == null ? 0 : UserSearchUC.SelectedUser.Id,
                DisplayName = txtName.Text.Trim(),
                ApplicationCode = txtApplicationCode.Text.Trim(),
                Description = txtEndPointDesc.Text.Trim(),
                IsTrustedSource = chkIsTrustedSource.Checked,
                IncludeInAttestation = chkIncludeInAttestation.Checked,
                IsPasswordResetAllowed = chkAllowPaswordReset.Checked,
                IsUnlockAllowed = chkunlockAccount.Checked,
                IsActive = chkIsActive.Checked,
                ApplicationType = (ApplicationType)Convert.ToInt32(string.IsNullOrEmpty(ddlApplicationTypes.SelectedValue) ? "0" : ddlApplicationTypes.SelectedValue),
                IsSodApplicable = chkIsSODApplicable.Checked,
                SupportSecondaryAccount = chkSupportSecondaryAccount.Checked,
                CanAutomate = chkCanAutomate.Checked,
                IsFunctionLevel = chkFunctionLevel.Checked,
                OwnerName = UserSearchUC.SelectedUser == null ? "" : $"{UserSearchUC.SelectedUser.DisplayName}({UserSearchUC.SelectedUser.UserName})",
            };
        }

        public bool isEpNameExists(UIMode uiMode)
        {
            if (uiMode == UIMode.Edit)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", ((long)mEndpoint.Id).ToString());
                parameter.Add("name", txtName.Text);
                parameter.Add("endpointSystemTypeId", ((long)mEndpoint.EndpointSystemTypeId).ToString());
                return DomainApiClient<Dto.EndPoint>.GetByAction<bool>(DomainApiAction.Endpoint.EndPoint.DoesExists, parameter);
            }
            else
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", ((long)0).ToString());
                parameter.Add("name", txtName.Text);
                parameter.Add("endpointSystemTypeId", ((long)mEndpoint.EndpointSystemTypeId).ToString());
                return DomainApiClient<Dto.EndPoint>.GetByAction<bool>(DomainApiAction.Endpoint.EndPoint.DoesExists, parameter);
            }
        }

        public int isExistApplicationCode()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("applicationCode", txtApplicationCode.Text.Trim());
            parameter.Add("id", EntityId.ToString());
            return DomainApiClient<Dto.EndPoint>.GetByAction<int>(DomainApiAction.Endpoint.EndPoint.DoesExistsbyApplicationCode, parameter);
        }
        #endregion

        protected void chkIsTrustedSource_CheckedChanged(object sender, EventArgs e)
        {
            if (chkIsTrustedSource.Checked)
                resetform.Visible = attestationDiv.Visible = SODApplicableDiv.Visible = SecondaryAccountDiv.Visible = false;
            else
            {
                resetform.Visible = attestationDiv.Visible = SODApplicableDiv.Visible = SecondaryAccountDiv.Visible = true;
            }
        }

        protected void chkCanAutomate_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkCanAutomate.Checked)
                chkFunctionLevel.Enabled = true;
            else
            {
                chkFunctionLevel.Enabled = false;
                chkFunctionLevel.Checked = false;
            }
        }
    }
}