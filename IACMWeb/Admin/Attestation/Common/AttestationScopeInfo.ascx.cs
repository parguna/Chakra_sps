using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Linq;
using Telerik.Web.UI;
using Application;

namespace DI.Chakra.Web.Attestation.Common
{
    public partial class AttestationScopeInfo : UIBaseUC
    {
        #region Members

        private const string V_ID = "ENTITY_ID";
        private const string V_ATTESTATIONSCOPE = "ATTESTATIONSCOPE";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        private const string V_ATTESTATIONSCOPEVALUE = "ATTESTATIONSCOPEVALUE";
        private const string ATTESTATIONSPLITVALUE = "ATTESTATIONSPLITVALUE";
        private const string V_SCOPELIST = "V_SCOPELIST";
        private List<Dto.AttestationScope> mAttestationScope = new List<Dto.AttestationScope>();
        private static Dto.Attestation mAttestation = new Dto.Attestation();
        private SessionStatusImpl sessionStatus = new SessionStatusImpl();
        private string SelectedScopeValueDesc = "";
        private string SelectedScopeName = "";

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

        public AttestationScopeEnum AttestationScope
        {
            get
            {
                if (null == ViewState[V_ATTESTATIONSCOPE])
                    return AttestationScopeEnum.User;
         
                return (AttestationScopeEnum)ViewState[V_ATTESTATIONSCOPE];
            }
            set
            {
                ViewState[V_ATTESTATIONSCOPE] = value;
            }
        }

        public string ScopeValue
        {
            get
            {
                return (string)ViewState[V_ATTESTATIONSCOPEVALUE];
            }
            set
            {
                ViewState[V_ATTESTATIONSCOPEVALUE] = value;
            }
        }

        public AttestationScopeEnum SplitValue
        {
            get
            {
                if (null == ViewState[ATTESTATIONSPLITVALUE])
                    return AttestationScopeEnum.None;
                return (AttestationScopeEnum)ViewState[ATTESTATIONSPLITVALUE];
            }
            set
            {
                ViewState[ATTESTATIONSPLITVALUE] = value;
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

        public List<Dto.AttestationScope> attestationScope
        {
            get
            {
                mAttestationScope = GetControl();                
                return mAttestationScope;
            }
            set
            {
                mAttestationScope = value;
                SetControl();
            }
        }

        public Dto.Attestation attestation
        {
            get
            {
                return mAttestation;
            }
            set
            {
                mAttestation = value;
                setAttestationTypeBasedSettings();                
            }
        }

        public List<AttestationScope> SelectedScopeList 
        {
            get
            {
                if (ViewState[V_SCOPELIST] == null)
                    return new List<AttestationScope>();
                return (List<AttestationScope>)ViewState[V_SCOPELIST];
            }
            set
            {
                ViewState[V_SCOPELIST] = value;
            }
        }
        #endregion

        #region Methods
        private List<Dto.AttestationScope> GetControl()
        {
            if ((UIMode == UIMode.View) | (UIMode == UIMode.Edit))
            {
                EntityId = Convert.ToInt64(this.ContextObjectId);
                mAttestation.Id = EntityId;
            }
            else if (UIMode == UIMode.Add)
            {
                mAttestation.Id = -1;
            }
            List<AttestationScope> attestationScopes = new List<AttestationScope>();
            long TempAttestationScopeId = 0;
            foreach (var attscp in SelectedScopeList) 
            {
                Dto.AttestationScope attScope = new Dto.AttestationScope();
                attScope.AttestationId = mAttestation.Id;
                attScope.AttestationScopeName = attscp.AttestationScopeName;

                if ((attscp.OrganisationScopeId == 0) || (attscp.OrganisationScopeId == null))
                    attScope.OrganisationScopeId = null;
                else
                    attScope.OrganisationScopeId = attscp.OrganisationScopeId;         

                if ((attscp.AttestationScopeValueDesc == "ALL") && (attscp.AttestationScopeValue == 0))
                    attScope.AttestationScopeValue = null;
                else
                    attScope.AttestationScopeValue = attscp.AttestationScopeValue;

                attScope.Id = attscp.Id;
                attestationScopes.Add(attScope);
                TempAttestationScopeId = TempAttestationScopeId + 1;
            }

            mAttestation.SplitCampaignUsing = (AttestationScopeEnum)(Convert.ToInt32(ddlSplitValue.SelectedValue));
            mAttestation.SplitCampaignUsing = this.SplitValue;

            return attestationScopes;
        }

        private void SetControl()
        {
            SetAttestationScope();
        }

        void setAttestationTypeBasedSettings()
        {
            BindCampaignSplitScope();
            ddlSplitValue.SelectedValue = ((int)this.SplitValue).ToString();


            if (attestation.AttestationType != AttestationType.User)
            {
                ddlScopeValue.Enabled = false;
                ScopeValueAutoCompleteBox.Enabled = false;
                btnAddScopeType.Enabled = false;

                this.SelectedScopeList.Clear();
            }
            else
            {
                ddlScopeValue.Enabled = true;
                ScopeValueAutoCompleteBox.Enabled = true;
                btnAddScopeType.Enabled = true;
            }
        }

        private void SetAttestationScope()
        {
            EntityId = Convert.ToInt64(this.ContextObjectId);
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("attestationId", EntityId.ToString());
            List<Dto.Presentation.PresentationAttestationScope> PresentationAttestationScope = DomainApiClient<Dto.Presentation.PresentationAttestationScope>.
                GetByAction<List<Dto.Presentation.PresentationAttestationScope>>(DomainApiAction.Attestation.PresentationAttestationScope.GetAttestationScopeByAttestationId, parameter);

            var scopes = this.SelectedScopeList;
            foreach (Dto.Presentation.PresentationAttestationScope attScope in PresentationAttestationScope)
            {
                if (attScope != null)
                {
                    Dto.AttestationScope attestationScope = new Dto.AttestationScope();
                    attestationScope.Id = attScope.Id;
                    attestationScope.AttestationId = attScope.AttestationId;
                    attestationScope.OrganisationScopeId = attScope.OrganisationScopeId;
                    attestationScope.AttestationScopeName = attScope.AttestationScopeName;
                    attestationScope.AttestationScopeValue = attScope.AttestationScopeValue;
                    attestationScope.AttestationScopeValueDesc = attScope.AttestationScopeValueDesc;
                    scopes.Add(attestationScope);
                    this.SelectedScopeList = scopes;
                }
            }
            RebindScopeGrid();
            ScopeGridEnable();
            ScopeValuesGrid.Enabled = true;
            ddlSplitValue.SelectedValue = ((int)mAttestation.SplitCampaignUsing).ToString();
            this.SplitValue = mAttestation.SplitCampaignUsing;
        }

        public void InitializeControl(UIMode UIMode)
        {            
            BindOrganisationScope();
            BindCampaignSplitScope();
            ViewState["V_DISPLAYMODE"] = UIMode;
        }

        public void BindOrganisationScope() 
        {
            ddlScopeValue.DataSource = DomainApiClient<Dto.NOrganisationScope>.GetByAction<List<NOrganisationScope>>(DomainApiAction.Org.NOrganisationScope.GetAllOrganisationScope, null); ;
            ddlScopeValue.DataBind();
            ddlScopeValue.Items.Insert(0, new ListItem("--Select--", ""));
        }

        public void BindCampaignSplitScope()
        {
            ddlSplitValue.Items.Clear();
            ddlSplitValue.DataTextField = "Value";
            ddlSplitValue.DataValueField = "Key";
            Dictionary<long, string> dataSource = new Dictionary<long, string>();

            foreach (AttestationScopeEnum item in Enum.GetValues(typeof(AttestationScopeEnum)))
            {                
                var displayName = UtilityMethods.GetEnumAttribute<VariantType>(item);
                if(string.IsNullOrEmpty(displayName) || displayName == "None" || displayName == attestation.AttestationType.ToString())
                {
                    dataSource.Add((long)item, item.ToString());
                }        
            }

            ddlSplitValue.DataSource = dataSource;
            ddlSplitValue.DataBind();
            ddlSplitValue.SelectedIndex = 0;
        }

        private void RebindScopeGrid()   
        {
            this.ScopeValuesGrid.DataSource = this.SelectedScopeList;
            this.ScopeValuesGrid.Rebind();
        }

        private void ScopeGridEnable() 
        {
            if ((UIMode == UIMode.Edit) || (UIMode == UIMode.Add))
            {
                if (ScopeValuesGrid.Items.Count == 0)
                    ScopeValuesGrid.Visible = false;
                else
                    ScopeValuesGrid.Visible = true;
            }
            if (UIMode == UIMode.View)
            {
                ScopeValuesGrid.Enabled = false;
            }
        }

        private void delete()
        {
            if ((!string.IsNullOrEmpty(this.SelectedScopeValueDesc)) && (!string.IsNullOrEmpty(this.SelectedScopeName)))
            {     
                SelectedScopeList.RemoveAt(SelectedScopeList.FindIndex(scope => scope.AttestationScopeName == SelectedScopeName && scope.AttestationScopeValueDesc == SelectedScopeValueDesc));
                MasterPage.ShowAlert(AlertType.Success, "Scope value deleted successfully");
            }
            RebindScopeGrid();
            ScopeGridEnable();
        }
        #endregion

        #region Events
        protected void ScopeValue_OnSelectedIndexChanged(object sender, EventArgs e) 
        {
            ScopeValueAutoCompleteBox.Entries.Clear();
            if (((DropDownList)sender).ID == ddlScopeValue.ID)
            {
                this.ScopeValue = ddlScopeValue.SelectedItem.Value.ToString();
            }
            if ((ddlScopeValue.SelectedItem.Text == AttestationScopeEnum.IACMGroup.ToString()) || (ddlScopeValue.SelectedItem.Text == AttestationScopeEnum.User.ToString())
                || (ddlScopeValue.SelectedItem.Text == AttestationScopeEnum.RO.ToString())
                //||(ddlScopeValue.SelectedItem.Text == AttestationScopeEnum.OrganisationUnit.ToString())
                )
                Session["OrganisationScopeId"] = ddlScopeValue.SelectedItem.Text;
            else
                    Session["OrganisationScopeId"] = ddlScopeValue.SelectedItem.Value;
        }

        protected void btnAddScopeType_Click(object sender, EventArgs e) 
        {
            if (this.ScopeValueAutoCompleteBox.Entries.Count != 0)
            {
                long organisationScopeId = 0;
                if (this.ScopeValueAutoCompleteBox.Entries[0].Text != "ALL")
                    organisationScopeId = Convert.ToInt64(this.ScopeValueAutoCompleteBox.Entries[0].Value); 
                string scope = this.ScopeValueAutoCompleteBox.Entries[0].Text;
                var scopes = this.SelectedScopeList;
                string attestationScopeName = ddlScopeValue.SelectedItem.Text;

                var match = SelectedScopeList.Where(item => item.AttestationScopeValueDesc.Contains(scope) 
                            && item.AttestationScopeName == attestationScopeName).FirstOrDefault();
                var matchALL = SelectedScopeList.Where(item => item.AttestationScopeName == attestationScopeName
                           && item.AttestationScopeValueDesc == "ALL").FirstOrDefault();

                if ((match == null) && (matchALL == null))
                {
                    AttestationScope attScope = new AttestationScope();
                    attScope.Id = 0;
                    if ((ddlScopeValue.SelectedItem.Text != AttestationScopeEnum.IACMGroup.ToString()) &&
                        (ddlScopeValue.SelectedItem.Text != AttestationScopeEnum.User.ToString()) &&
                        (ddlScopeValue.SelectedItem.Text != AttestationScopeEnum.RO.ToString()))
                    {
                        attScope.OrganisationScopeId = organisationScopeId;
                        attScope.AttestationScopeValue = Convert.ToUInt32(ddlScopeValue.SelectedItem.Value);
                    }
                    if ((ddlScopeValue.SelectedItem.Text == AttestationScopeEnum.IACMGroup.ToString()) ||
                        (ddlScopeValue.SelectedItem.Text == AttestationScopeEnum.User.ToString()) ||
                        (ddlScopeValue.SelectedItem.Text == AttestationScopeEnum.RO.ToString()))
                    {
                        attScope.AttestationScopeValue = organisationScopeId;
                        if(organisationScopeId == 0)
                        {
                            attScope.OrganisationScopeId = attScope.AttestationScopeValue = null;
                        }
                    }
                    attScope.AttestationScopeName = ddlScopeValue.SelectedItem.Text;
                    attScope.AttestationScopeValueDesc = scope;                   
                    scopes.Add(attScope);
                    this.SelectedScopeList = scopes;
                    this.RebindScopeGrid();
                    ScopeGridEnable();
                }
                else
                {
                    if (match != null)
                        MasterPage.ShowAlert(AlertType.Warning, "Scope value already added");
                     if (matchALL != null)
                            MasterPage.ShowAlert(AlertType.Warning, "Scope ALL value already added");                    
                }
                this.ScopeValueAutoCompleteBox.Entries.Clear();
            }
        }

        protected void ScopeValuesGrid_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                GridDataItem item = (GridDataItem)e.Item;
                this.SelectedScopeName = item["AttestationScopeName"].Text.Trim();
                this.SelectedScopeValueDesc = item["AttestationScopeValueDesc"].Text.Trim();
                delete();
            }
        }

        #endregion

        protected void ddlSplitValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SplitValue = (AttestationScopeEnum)Convert.ToInt64(ddlSplitValue.SelectedValue);
        }
    }
}