using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Web.UI.WebControls;
using DI.Chakra.Dto;

namespace DI.Chakra.Web.Comp.Policy.Common
{
    public partial class ComplianceDetailsUC : UIBaseUC
    {
        #region Class Variables
        SessionStatusImpl session = new SessionStatusImpl();
        private const string V_ID = "ENTITY_ID";
        private const string V_CompliancePolicy = "ComplaincePolicy";
        private UIMode uiMode;
        #endregion

        #region Public Properties
        public UIMode UIMode
        {
            get { return uiMode; }
            set { uiMode = value; }
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

        public CompliancePolicy CompliancePolicy
        {
            get
            {
                if (ViewState[V_CompliancePolicy] == null)
                    return null;
                return (CompliancePolicy)ViewState[V_CompliancePolicy];
            }
            set
            {
                ViewState[V_CompliancePolicy] = value;
            }
        }
        #endregion

        #region Page and Control Events
        /// <summary>
        /// Method to load the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to set mode of display
        /// </summary>
        public void DisplayMode()
        {
            UtilityMethods.DisableControls(contentWrapper);
        }
        #endregion

        #region Initialise Section
        /// <summary>
        /// Method to initialise page
        /// </summary>
        public void InitilizePage()
        {
            BindComplianceType();
            rblActive.Checked = true;
            if (UIMode == UIMode.Edit || UIMode == UIMode.View)
            {
                txtComplianceCode.Text = CompliancePolicy.CompliancePolicyCode;
                txtConflictDescription.Text = CompliancePolicy.CompliancePolicyDescription;
                if (CompliancePolicy.IsActive.Value)
                {
                    rblActive.Checked = true;
                }
                else
                {
                    rblInActive.Checked = true;
                }
                ListItem selectedItem = ddlComplianceType.Items.FindByValue(CompliancePolicy.CompliancePolicyType.ToString());
                selectedItem.Selected = true;
                Session["CompliancType"] = selectedItem != null ? selectedItem.Text : "";
                UserSearch.SelectedUser = CompliancePolicy.Owner;
            }
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Method to assign user input to a compliance policy
        /// </summary>
        /// <returns></returns>
        public CompliancePolicy GetCompliancePolicy()
        {
            CompliancePolicy policy = new CompliancePolicy();
            policy.IsActive = rblActive.Checked;
            //policy.Owner = UserSearch.SelectedUser;
            if (UserSearch.SelectedUser != null)
                policy.OwnerId = UserSearch.SelectedUser.Id;
            policy.CompliancePolicyType = ddlComplianceType.SelectedValue;
            policy.CompliancePolicyCode = txtComplianceCode.Text;
            policy.CompliancePolicyDescription = txtConflictDescription.Text;
            return policy;
        }

        /// <summary>
        /// Method to bind compliance type dropdown
        /// </summary>
        [WebMethod(EnableSession = true)]
        private void BindComplianceType()
        {
            List<ComplianceType> compliType = DomainApiClient<ComplianceType>.GetAll();
            ddlComplianceType.DataSource = compliType;
            ddlComplianceType.DataTextField = "PolicyTypeName";
            ddlComplianceType.DataValueField = "PolicyTypeName";
            ddlComplianceType.DataBind();
            Session["CompliancType"] = compliType != null && compliType.Count > 0 ? compliType.FirstOrDefault().PolicyTypeName : "";
        }

        /// <summary>
        /// Method to handle selected index change in ddlComplianceType
        /// </summary>
        [WebMethod(EnableSession = true)]
        protected void ddlComplianceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["CompliancType"] = ((System.Web.UI.WebControls.ListControl)(sender)).SelectedValue;
        }
        #endregion
    }
}