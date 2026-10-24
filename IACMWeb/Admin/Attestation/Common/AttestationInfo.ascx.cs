using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Linq;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Web.Attestation.Common
{
    public partial class AttestationInfo : UIBaseUC
    {
        #region Members
        private const string V_ID = "ENTITY_ID";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        private const string V_SELECTEDENDPOINTS = "SELECTEDENDPOINTS";
        private const string V_SELECTEDENDPOINTTYPES = "SELECTEDENDPOINTTYPES";
        private const string V_SELECTEDATTESTATIONTYPES = "SELECTEDATTESTATIONTYPES";
        private static Dto.Attestation mAttestation = new Dto.Attestation();
        private SessionStatusImpl sessionStatus = new SessionStatusImpl();
        #endregion

        #region Properties
        public long EntityId
        {
            get
            {
                if (ViewState[V_ID] == null)
                    return 0;
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

        public Dictionary<long, string> SelectedEndpoints
        {
            get
            {
                if (ViewState[V_SELECTEDENDPOINTS] != null)
                {
                    return (Dictionary<long, string>)ViewState[V_SELECTEDENDPOINTS];
                }
                return new Dictionary<long, string>();
            }
            set
            {
                ViewState[V_SELECTEDENDPOINTS] = value;
            }
        }

        public Dictionary<long, string> SelectedEndpointTypes
        {
            get
            {
                if (ViewState[V_SELECTEDENDPOINTTYPES] != null)
                {
                    return (Dictionary<long, string>)ViewState[V_SELECTEDENDPOINTTYPES];
                }
                return new Dictionary<long, string>();
            }
            set
            {
                ViewState[V_SELECTEDENDPOINTTYPES] = value;
            }
        }

        private int SelectedAttestationType
        {
            get
            {
                if (ViewState[V_SELECTEDATTESTATIONTYPES] != null)
                {
                    return (int)ViewState[V_SELECTEDATTESTATIONTYPES];
                }
                return 0;
            }
            set
            {
                ViewState[V_SELECTEDATTESTATIONTYPES] = value;
            }
        }

        public Dto.Attestation attestation
        {
            get
            {
                mAttestation = GetControl();
                return mAttestation;
            }

            set
            {
                mAttestation = value;
                SetControl();
            }
        }
        #endregion

        #region Events
        private void AddEndPoint(long epId, string hostName)
        {
            var eps = this.SelectedEndpoints;
            if (!eps.ContainsKey(epId))
            {
                eps.Add(epId, hostName);
               
            }
            this.SelectedEndpoints = eps;
        }

        private void LoadFromEndpointHierarchy(string EndpointId)
        {
            var defaultReviewStatus = MasterConfigHelper.AttestationDefaultReviewStatus;
            Dictionary<string, string> parameterList = new Dictionary<string, string>();
            parameterList.Add("EndpointSystemId", EndpointId);
            parameterList.Add("attestationType", ((AttestationType)SelectedAttestationType).ToString());
            List<Dto.EndpointHierarchy> endpointHierarchy = DomainApiClient<Dto.EndpointHierarchy>.
                                                       GetByAction<List<Dto.EndpointHierarchy>>(DomainApiAction.Endpoint.EndPoint.GetEndpointHierarchyByEndpoint, parameterList);

            long epId = Convert.ToInt64(EndpointId);
            endpointHierarchy.ForEach(s => s.EndpointId = epId);

            List<AttestationHierarchy> attestationHierarchyfromEndpoint = (from hierarchy in endpointHierarchy
                                                                           select new AttestationHierarchy
                                                                           {
                                                                               EndpointHierarchyId = hierarchy.Id,
                                                                               IsActionAllowed = hierarchy.IsActionable,
                                                                               IsActive = hierarchy.IsActive,
                                                                               IsVisible = true,
                                                                               IsActionExtendable = false,
                                                                               EndpointHierarchy = hierarchy,
                                                                               EndpointId = epId,
                                                                               DefaultAttestationAction = (AttestationDefaultReviewStatus)defaultReviewStatus,

                                                                           }).ToList();
                        
            var sessionFlag = string.Format("AttestationHierarchy_EP_{0}", epId);            
            Session[sessionFlag] = attestationHierarchyfromEndpoint;
        }

        protected void txtAttestationName_TextChanged(object sender, EventArgs e)
        {
            if (txtAttestationName.Text.Length == 1)
            {
                if (txtAttestationName.Text == string.Empty)
                {
                    sender = true;
                }
                else
                {
                    sender = false;
                }
            }
            else
            {
                sender = false;
            }
        }

        protected void EndPointSystem_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = e.Item as GridDataItem;
                if ((UIMode == UIMode.View))
                {
                    (item["DeleteColumn"].Controls[0] as ImageButton).Enabled = false;
                }
                else
                {
                    (item["DeleteColumn"].Controls[0] as ImageButton).Enabled = true;
                }

                //if(SelectedAttestationType == (int)AttestationType.Privileged)
                //{
                //    (item["Hierarchy"].Controls[0] as LinkButton).Enabled = false;
                //}
                //else
                {
                    (item["Hierarchy"].Controls[0] as LinkButton).Enabled = true;
                }
            }
        }

        protected void EndpointTypeUC_EndpointTypeChanged(EndPointSystemType EndPointSystemType)
        {
            if (EndPointSystemType != null)
                AddEndPointType(Convert.ToInt64(EndPointSystemType.EndpointSystemTypeName),
                    Convert.ToString(EndPointSystemType.EndpointSystemTypeName));
            else
            {
                this.EndPointAutoCompleteBox.Entries.Clear();
                Session.Remove("AttestationEndpointTypeIds");
            }

            lblFirstPart.Text = System.Web.HttpUtility.HtmlEncode(txtAttestationName.Text);
        }

        private void AddEndPointType(long epTypeId, string hostName)
        {
            this.SelectedEndpointTypes.Clear();
            var epsType = this.SelectedEndpointTypes;
            if (!epsType.ContainsKey(epTypeId))
            {
                epsType.Add(epTypeId, hostName);
            }
            if (Session["AttestationEndpointTypeIds"] != null)
            {
                Session.Remove("AttestationEndpointTypeIds");
                this.EndPointAutoCompleteBox.Entries.Clear();
            }
            sessionStatus.AttestationEndPointTypeIds = new List<long>();
            sessionStatus.AttestationEndPointTypeIds.Add(epTypeId);
            this.SelectedEndpointTypes = epsType;
        }

        public void InitializeControl(UIMode UIMode)
        {
            ViewState["V_DISPLAYMODE"] = UIMode;
            EndpointUC.Initialize();
            BindAttestationType();
            if ((UIMode == UIMode.View) | (UIMode == UIMode.Edit))
            {
                EntityId = Convert.ToInt64(this.ContextObjectId);
                mAttestation.Id = EntityId;
                sessionStatus.AttestationEndPointTypeIds = new List<long>();
                RebindEndPointTypeGrid();
                RebindEndPointGrid();
                SetControl();
                if (UIMode == UIMode.View)
                    EnableControls();
            }
            else if (UIMode == UIMode.Add)
            {
                GetControl();
            }
            lblFirstPart.Text = System.Web.HttpUtility.HtmlEncode(txtAttestationName.Text);
        }

        private void BindAttestationType()
        {
            try
            {
                AttestationTypeField.Items.Clear();
                AttestationTypeField.DataTextField = "Value";
                AttestationTypeField.DataValueField = "Key";
                AttestationTypeField.DataSource = DI.Chakra.Infrastructure.UtilityMethods.GetAll<AttestationType>();
                AttestationTypeField.DataBind();
                AttestationTypeField.SelectedIndex = 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void EnableControls()
        {
            AddEndPointButton.Enabled = false;
            EndPointAutoCompleteBox.Enabled = false;
        }

        protected void EndPointSystem_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                GridDataItem item = e.Item as GridDataItem;
                long epId = (long)Convert.ChangeType(item.GetDataKeyValue("Key"), typeof(long));
                var eps = this.SelectedEndpoints;
                eps.Remove(epId);
                this.SelectedEndpoints = eps;
                this.RebindEndPointGrid();

                if (this.SelectedEndpoints.Count == 0 && Convert.ToInt32(AttestationTypeField.SelectedValue) != (int)AttestationType.User)
                {
                    AddEndPointButton.Enabled = true;
                }
                else
                {
                    AddEndPointButton.Enabled = true;
                }
            }
            else if (e.CommandName == "Hierarchy")
            {
                //if (SelectedAttestationType == (int)AttestationType.Privileged) return;
                GridDataItem item = e.Item as GridDataItem;
                long epId = (long)Convert.ChangeType(item.GetDataKeyValue("Key"), typeof(long));
                Session["EndpointID"] = epId;
                Dictionary<string, string> parameterList = new Dictionary<string, string>();
                parameterList.Add("EndpointSystemId", epId.ToString());

                bool LoadHierarchyFromEndPoint = true;
                List<Dto.EndpointHierarchy> endpointHierarchy = null;
                List<AttestationHierarchy> attestationHierarchy = null;
                var sessionFlag = string.Format("AttestationHierarchy_EP_{0}", epId);
                if (Session[sessionFlag] != null)
                {
                    attestationHierarchy = (List<AttestationHierarchy>)Session[sessionFlag];

                    List<AttestationHierarchy> savedHierarchy = attestationHierarchy.Where(hierarchy => hierarchy.EndpointId == epId).ToList();
                    if (savedHierarchy.Count > 0)
                    {
                        LoadHierarchyFromEndPoint = false;
                        attestationHierarchy = savedHierarchy;
                    }

                }

                if (LoadHierarchyFromEndPoint)
                {
                    LoadFromEndpointHierarchy(epId.ToString());

                }
                Session["SelectedAttestationType"] = SelectedAttestationType;
                AttestationObject.BindRepeater();
                //modalPopup.VisibleOnPageLoad = true;
                modalAttestationPopup.Visible = true;
                modalAttestationPopup.VisibleOnPageLoad = true;
                //AttestationObjectModalPopupExtender.Show();
            }
        }

        protected void btnAttSubmit_Click(object sender, EventArgs e)
        {
            //modalPopup.VisibleOnPageLoad = false;
            List<Dto.AttestationHierarchy> SavedHierarchy  = (List<Dto.AttestationHierarchy>)Session["AttestHierarchy"];
            List<AttestationHierarchy> lstHierarchy = (List<AttestationHierarchy>)Session["AttestationHierarchy"];

            long EndpointId = (long)Session["EndpointID"];

            foreach (AttestationHierarchy attestHierarchy in lstHierarchy)
            {
                if (attestHierarchy.EndpointId == EndpointId)
                {
                    AttestationHierarchy configured = SavedHierarchy.FirstOrDefault(hier => hier.EndpointHierarchy.EntitlementType == attestHierarchy.EndpointHierarchy.EntitlementType);
                    attestHierarchy.IsVisible = configured.IsVisible;
                    attestHierarchy.IsActionExtendable = configured.IsActionExtendable;
                    attestHierarchy.IsActionAllowed = configured.IsActionAllowed;
                }
            }
            Session["AttestationHierarchy"] = lstHierarchy;
        }

        protected void btnAttCancel_Click(object sender, EventArgs e)
        {
           // modalPopup.VisibleOnPageLoad = false;
        }     

        protected void DeleteEndPointSystemTypeButton_Click(object sender, EventArgs e)
        {
            long epTypeId = Convert.ToInt64(((LinkButton)sender).CommandArgument);
            var epTypes = this.SelectedEndpointTypes;
            epTypes.Remove(epTypeId);
            this.SelectedEndpointTypes = epTypes;
            this.RebindEndPointTypeGrid();
            var epTypeIdList = sessionStatus.AttestationEndPointTypeIds;
            epTypeIdList.Remove(epTypeId);
            sessionStatus.AttestationEndPointTypeIds = epTypeIdList;
        }

        protected void DeleteEndPointButton_Click(object sender, EventArgs e)
        {
            long epId = Convert.ToInt64(((LinkButton)sender).CommandArgument);
            var eps = this.SelectedEndpoints;
            eps.Remove(epId);
            this.SelectedEndpoints = eps;
            this.RebindEndPointGrid();
        }

        protected void AddEndPointButton_Click(object sender, EventArgs e)
        {
            foreach (AutoCompleteBoxEntry item in EndPointAutoCompleteBox.Entries)
            {
                bool loadEpHierrchy = false;
                var epId = Convert.ToInt64(item.Value);
                var eps = this.SelectedEndpoints;
                if (!eps.ContainsKey(epId))
                {
                    loadEpHierrchy = true;
                }

                AddEndPoint(epId, item.Text);

                if (loadEpHierrchy)
                {
                    var sessionFlag = string.Format("AttestationHierarchy_EP_{0}", item.Value);
                    Session[sessionFlag] = null;
                    LoadFromEndpointHierarchy(item.Value);
                }
            }
            this.RebindEndPointGrid();

            if(this.SelectedEndpoints.Count > 0 && Convert.ToInt32(AttestationTypeField.SelectedValue) != (int)AttestationType.User)
            {
                AddEndPointButton.Enabled = false;
            }
            else
            {
                AddEndPointButton.Enabled = true;
            }
        }

        protected void EndPointSystem_FormGrid_PreRender(object sender, EventArgs e)
        {
            foreach (GridColumn col in EndPointSystem_FormGrid.Columns)
            {
                if (col.UniqueName == "EndPointSystemName")
                    col.HeaderText = T("App.Common.Endpoint");
            }
            this.EndPointSystem_FormGrid.DataSource = this.SelectedEndpoints;
            EndPointSystem_FormGrid.Rebind();
        }
        #endregion         

        #region Methods
        private Dto.Attestation GetControl()
        {
            if ((UIMode == UIMode.View) | (UIMode == UIMode.Edit))
            {
                EntityId = Convert.ToInt64(this.ContextObjectId);
                mAttestation.Id = EntityId;
            }
            else if (UIMode == UIMode.Add)
            {
                mAttestation.Id = -1;
                mAttestation.AttestationProcesses = null;
                if (!IsPostBack)
                {
                    mAttestation.WorkflowConfigurationId = 0;
                    mAttestation.WorkflowConfiguration = null;
                }
            }
            mAttestation.AttestationName = txtAttestationName.Text;
            mAttestation.CampaignNameFormat = txtCampaignName.Text;
            mAttestation.AttestationDescription = txtAttestationDesc.Text;
            mAttestation.AttestationType = ((AttestationType)Convert.ToInt32(string.IsNullOrEmpty(AttestationTypeField.SelectedValue) ? "0" : AttestationTypeField.SelectedValue));
            mAttestation.RequiresAttestationRemediation = chkRemediate.Checked ? true : false;
            mAttestation.IncludeUnmappedAccount = chkUnmappedAccount.Checked ? true : false;
            mAttestation.IncludeDormantAccount = chkDormantAccount.Checked ? true : false;
            if (mAttestation.IncludeDormantAccount)
                mAttestation.DormantAccountDays = string.IsNullOrEmpty(txtDormantDays.Text.Trim()) ? 0 : Convert.ToInt64(txtDormantDays.Text.Trim());
            else
                mAttestation.DormantAccountDays = 0;
            mAttestation.AttestationEndpoint = GetAttestationEndpoints(mAttestation);
            return mAttestation;
        }

        private void SetControl()
        {
            txtAttestationName.Text = mAttestation.AttestationName;
            lblFirstPart.Text = System.Web.HttpUtility.HtmlEncode(txtAttestationName.Text);
            txtCampaignName.Text = mAttestation.CampaignNameFormat;
            txtAttestationDesc.Text = mAttestation.AttestationDescription;
            if (mAttestation.AttestationHierarchy != null && mAttestation.AttestationHierarchy.Count > 0)
            {
                var distinctEps = mAttestation.AttestationHierarchy.Select(a => a.EndpointId).Distinct().ToList();
                foreach (var distinctEp in distinctEps)
                {
                    var sessionFlag = string.Format("AttestationHierarchy_EP_{0}", distinctEp);
                    Session[sessionFlag] = null;
                    Session[sessionFlag] = mAttestation.AttestationHierarchy.Where(a => a.EndpointId == distinctEp).ToList();
                }
            }
            chkRemediate.Checked = (mAttestation.RequiresAttestationRemediation) ? true : false;
            chkUnmappedAccount.Checked = (mAttestation.IncludeUnmappedAccount) ? true : false;
            chkDormantAccount.Checked = (mAttestation.IncludeDormantAccount) ? true : false;

            if(mAttestation.DormantAccountDays == 0)
                txtDormantDays.Text = string.Empty;
            else
                txtDormantDays.Text = mAttestation.DormantAccountDays.ToString();

            foreach (Dto.AttestationEndpoint attestationEndpoint in mAttestation.AttestationEndpoint)
            {
                if (attestationEndpoint.EndPoint != null)
                {
                    AddEndPoint(attestationEndpoint.EndPoint.Id, attestationEndpoint.EndPoint.DisplayName);
                    AddEndPointType((long)attestationEndpoint.EndpointSystemType, attestationEndpoint.EndpointSystemType.ToString());
                }
                EndpointUC.SelectedApplication = attestationEndpoint.EndPoint.EndPointSystemType;
            }
            RebindEndPointGrid();
            EntityId = mAttestation.Id;
            EndPointSystem_FormGrid.Enabled = true;
            EndpointUC.DisabledApplicationType = false;

            if (EntityId > 0)
            {
                var param = new Dictionary<string, string>();
                param.Add("attestationId", EntityId.ToString());
                var dontAllow = DomainApiClient<Dto.AttestationProcess>.GetByAction<bool>("IsCampaignRunningForAttestation", param);
                EndPointSystem_FormGrid.Enabled = !dontAllow;
                EndpointUC.DisabledApplicationType = dontAllow;
            }

            AttestationTypeField.SelectedValue = ((int)mAttestation.AttestationType).ToString();
            Session["SelectedAttestationType"] = (int)mAttestation.AttestationType;
            SelectedAttestationType = (int)mAttestation.AttestationType;
            setUserAttestationConfig(((int)mAttestation.AttestationType).ToString());
        }

        private List<Dto.AttestationEndpoint> GetAttestationEndpoints(Dto.Attestation _attestation)
        {
            List<Dto.AttestationEndpoint> attestationEndpoints = new List<Dto.AttestationEndpoint>();
            long TempAttestationEndPointId = 0;
            foreach (var eps in SelectedEndpoints)
            {
                Dto.AttestationEndpoint attEndpoint = new Dto.AttestationEndpoint();
                attEndpoint.Attestation = _attestation;
                attEndpoint.AttestationId = _attestation.Id;
                Dictionary<String, string> parameter = new Dictionary<String, string>();
                parameter.Add("id", eps.Key.ToString());
                Dto.EndPoint _endpoint = DomainApiClient<Dto.EndPoint>.GetByAction<Dto.EndPoint>
                    (DomainApiAction.Endpoint.EndPoint.GetByIdIncludingOwner, parameter);
                attEndpoint.EndpointSystemType = _endpoint.EndPointSystemType.EndpointSystemTypeName;
                attEndpoint.EndpointSystemId = eps.Key;
                attEndpoint.Id = 0;
                attestationEndpoints.Add(attEndpoint);
                TempAttestationEndPointId = TempAttestationEndPointId + 1;
            }
            return attestationEndpoints;
        }

        private void RebindEndPointGrid()
        {
            this.EndPointSystem_FormGrid.DataSource = this.SelectedEndpoints;
            this.EndPointSystem_FormGrid.Rebind();
            this.EndPointAutoCompleteBox.Entries.Clear();
            EndPointSystem_FormGrid.Enabled = true;
            if (EndPointSystem_FormGrid.Items.Count > 0)
            {
                EndPointSystem_FormGrid.Visible = true;
            }
            else
            {
                EndPointSystem_FormGrid.Visible = false;
            }
        }

        private void RebindEndPointTypeGrid()
        {
        }
        #endregion       

        protected void AttestationObject_Add_CancelButtonClicked(object sender, EventArgs e)
        {
            modalAttestationPopup.ShowContentDuringLoad = false;
            modalAttestationPopup.Visible = false;

        }

        protected void AttestationObject_Add_SubmitButtonClicked(object sender, EventArgs e)
        {
            modalAttestationPopup.ShowContentDuringLoad = false;
            modalAttestationPopup.Visible = false;
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {

        }

        protected void chkDormantAccount_CheckedChanged(object sender, EventArgs e)
        {
            txtDormantDays.Text = string.Empty;
        }

        protected void AttestationTypeField_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedAttestationType = Convert.ToInt32(AttestationTypeField.SelectedValue);
            Session["SelectedAttestationType"] = SelectedAttestationType;
            setUserAttestationConfig(AttestationTypeField.SelectedValue);   
        }

        void setUserAttestationConfig(string selectedValue)
        {
            if (selectedValue == ((int)AttestationType.User).ToString())
            {
                chkRemediate.Enabled = true;
                chkUnmappedAccount.Enabled = true;
                chkDormantAccount.Enabled = true;
                txtDormantDays.Enabled = true;
            }
            else
            {
                chkRemediate.Checked = false;
                chkRemediate.Enabled = false;
                chkUnmappedAccount.Checked = false;
                chkUnmappedAccount.Enabled = false;
                chkDormantAccount.Checked = false;
                chkDormantAccount.Enabled = false;
                txtDormantDays.Text = string.Empty;
                txtDormantDays.Enabled = false;
            }
        }
    }
}