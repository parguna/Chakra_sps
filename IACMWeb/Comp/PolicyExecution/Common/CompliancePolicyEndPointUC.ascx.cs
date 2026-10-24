using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Comp.PolicyExecution.Common
{
    public partial class CompliancePolicyEndPointUC : UIBaseUC
    {
        #region Class Variables
        private SessionStatusImpl session = new SessionStatusImpl();
        private bool IsObject = false;
        private const string V_ID = "ENTITY_ID";
        private List<string> gblAttributes = new List<string>();
        //private List<SODRunPolicySelection> EndpointSelections = new List<SODRunPolicySelection>();
        private List<SODEndPointPolicySelection> EndpointSelections = new List<SODEndPointPolicySelection>();
        private UIMode uiMode;
        private const string V_SODFunction = "SODPolicyExecutionDefinition";
        private const string V_UIMode = "UIMode";
        #endregion

        #region Public Properties
        public UIMode UIMode
        {
            get
            {
                if (ViewState[V_UIMode] == null)
                    return UIMode.View;
                return (UIMode)ViewState[V_UIMode];
            }
            set
            {
                ViewState[V_UIMode] = value;
            }
        }

        public SODPolicyExecutionDefinition PolicyDefinition
        {
            get
            {
                if (ViewState[V_SODFunction] == null)
                    return null;
                return (SODPolicyExecutionDefinition)ViewState[V_SODFunction];
            }
            set
            {
                ViewState[V_SODFunction] = value;
            }
        }
        public List<string> SelectedEndPointAttribute
        {
            get { return gblAttributes; }
        }
        public List<SODEndPointPolicySelection> GetEndpointRunlevelSelections()
        {
            SetSelectedEndPointAttributes();
            return EndpointSelections;
        }
        public bool IsObjectAttributeSelected
        {
            get { return IsObject; }
        }
        #endregion

        #region Page and Control Events
        /// <summary>
        /// Method to Load Page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
            }
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Method to bind data to radExecutionOptions
        /// </summary>
        private void BindEndPointConfigs()
        {
            List<SODExecutionConfig> Sodconfigs = DomainApiClient<SODExecutionConfig>.GetByAction<List<SODExecutionConfig>>
                  (DomainApiAction.SODPolicyExecutionDefinition.SODExecutionConfig.GetAllWithEndPointSystemType, null);
            radExecutionOptions.DataSource = Sodconfigs;
            radExecutionOptions.DataBind();
            SetConfigParametersVisibility(Sodconfigs);
        }

        /// <summary>
        /// Method to set configuration parameters visibility
        /// </summary>
        /// <param name="Sodconfigs"></param>
        private void SetConfigParametersVisibility(List<SODExecutionConfig> Sodconfigs)
        {
            int i = 0;
            radExecutionOptions.MasterTableView.GetColumn("EndpointSystemTypeName").HeaderText = T("App.Common.EndpointType");
            foreach (GridDataItem item in radExecutionOptions.Items)
            {
                SODExecutionConfig sConfig = Sodconfigs[i++];
                CheckBox chkLevel1 = (CheckBox)item.FindControl("chkLevel1");
                CheckBox chkLevel2 = (CheckBox)item.FindControl("chkLevel2");
                CheckBox chkLevel3 = (CheckBox)item.FindControl("chkLevel3");
                CheckBox chkLevel4 = (CheckBox)item.FindControl("chkLevel4"); ;
                if (sConfig.Level1 == null || (sConfig.Level1 != null && sConfig.Level1.Trim() == string.Empty))
                {
                    if (chkLevel1 != null)
                    {
                        chkLevel1.Visible = false;
                    }
                }
                else
                {
                    chkLevel1.Text = sConfig.Level1;
                    chkLevel1.Checked = true;
                }
                if (sConfig.Level2 == null || (sConfig.Level2 != null && sConfig.Level2.Trim() == string.Empty))
                {
                    if (chkLevel2 != null)
                    {
                        chkLevel2.Visible = false;
                    }
                }
                else
                {
                    chkLevel2.Text = sConfig.Level2;
                    chkLevel2.Checked = true;
                }

                if (sConfig.Level3 == null || (sConfig.Level3 != null && sConfig.Level3.Trim() == string.Empty))
                {
                    if (chkLevel3 != null)
                    {
                        chkLevel3.Visible = false;
                    }
                }
                else
                {
                    chkLevel3.Text = sConfig.Level3;
                    chkLevel3.Checked = true;
                }
                if (sConfig.Level4 == null || (sConfig.Level4 != null && sConfig.Level4.Trim() == string.Empty))
                {
                    if (chkLevel4 != null)
                    {
                        chkLevel4.Visible = false;
                    }
                }
                else
                {
                    chkLevel4.Text = sConfig.Level4;
                    chkLevel4.Checked = true;
                }
            }
            radExecutionOptions.Columns[1].Visible = false;
            radExecutionOptions.Columns[3].Visible = false;
            radExecutionOptions.Columns[5].Visible = false;
            radExecutionOptions.Columns[7].Visible = false;
        }

        /// <summary>
        /// Method to set attributes for seleceted endpoints
        /// </summary>
        public void SetSelectedEndPointAttributes()
        {
            gblAttributes.Clear();
            //chkObject.Checked = false;
            foreach (GridDataItem item in radExecutionOptions.MasterTableView.Items)
            {
                CheckBox chkLevel1 = (CheckBox)item.FindControl("chkLevel1");
                CheckBox chkLevel2 = (CheckBox)item.FindControl("chkLevel2");
                CheckBox chkLevel3 = (CheckBox)item.FindControl("chkLevel3");
                CheckBox chkLevel4 = (CheckBox)item.FindControl("chkLevel4");
                string endpointname = item["EndpointSystemTypeName"].Text;
                if (chkLevel1.Checked)
                {
                    AddToSelectedList(endpointname, chkLevel1.Text, true);
                    gblAttributes.Add(chkLevel1.Text);
                }
                if (chkLevel2.Checked)
                {
                    AddToSelectedList(endpointname, chkLevel2.Text, true);
                    gblAttributes.Add(chkLevel2.Text);
                }
                if (chkLevel3.Checked)
                {
                    AddToSelectedList(endpointname, chkLevel3.Text, true);
                    gblAttributes.Add(chkLevel3.Text);
                }
                if (chkLevel4.Checked)
                {
                    AddToSelectedList(endpointname, chkLevel4.Text, true);
                    IsObject = true;
                }
            }
        }

        /// <summary>
        /// Method to bind data for selected run level
        /// </summary>
        /// <param name="EndpointName"></param>
        /// <param name="LevelText"></param>
        /// <param name="Selected"></param>
        private void AddToSelectedList(string EndpointName, string LevelText, bool Selected)
        {
            SODEndPointPolicySelection RunLevelSelected = new SODEndPointPolicySelection();
            RunLevelSelected.EndpointTypeName = EndpointName;
            RunLevelSelected.LevelName = LevelText;
            RunLevelSelected.LevelValue = "1";
            RunLevelSelected.CreatedBy = session.User.DisplayName;
            RunLevelSelected.CreatedDate = DateTime.Now;
            if (UIMode == UIMode.Edit)
                RunLevelSelected.SODPolicyExceutionId = PolicyDefinition.Id;
            EndpointSelections.Add(RunLevelSelected);
        }

        /// <summary>
        /// Method to set configuration for endpoint
        /// </summary>
        /// <param name="selections"></param>
        private void SetEndPointSelectionConfigs(List<SODEndPointPolicySelection> selections)
        {
            if (selections.Count > 0)
            {
                foreach (GridDataItem item in radExecutionOptions.MasterTableView.Items)
                {
                    CheckBox chkLevel1 = (CheckBox)item.FindControl("chkLevel1");
                    CheckBox chkLevel2 = (CheckBox)item.FindControl("chkLevel2");
                    CheckBox chkLevel3 = (CheckBox)item.FindControl("chkLevel3");
                    CheckBox chkLevel4 = (CheckBox)item.FindControl("chkLevel4");
                    chkLevel1.Checked = false;
                    chkLevel2.Checked = false;
                    chkLevel3.Checked = false;
                    chkLevel4.Checked = false;
                    string endpointname = item["EndpointSystemTypeName"].Text;
                    string Level1 = item["Level1"].Text;
                    string Level2 = item["Level2"].Text;
                    string Level3 = item["Level3"].Text;
                    string Level4 = item["Level4"].Text;
                    SODEndPointPolicySelection endpoint = selections.Where
                        (m => m.EndpointTypeName == endpointname && m.LevelName == Level1).FirstOrDefault();
                    if (endpoint != null)
                    {
                        chkLevel1.Checked = true;
                    }
                    endpoint = selections.Where(m => m.EndpointTypeName == endpointname && m.LevelName == Level2).FirstOrDefault();
                    if (endpoint != null)
                    {
                        chkLevel2.Checked = true;
                    }
                    endpoint = selections.Where(m => m.EndpointTypeName == endpointname && m.LevelName == Level3).FirstOrDefault();
                    if (endpoint != null)
                    {
                        chkLevel3.Checked = true;
                    }
                    endpoint = selections.Where(m => m.EndpointTypeName == endpointname && m.LevelName == Level4).FirstOrDefault();
                    if (endpoint != null)
                    {
                        chkLevel4.Checked = true;
                    }
                }
            }
        }
        #endregion

        #region Initialise Section
        /// <summary>
        /// Method to initialise Page
        /// </summary>
        public void InitilizePage()
        {
            BindEndPointConfigs();
            if (UIMode == UIMode.Edit || UIMode == UIMode.View)
            {
                if (PolicyDefinition != null)
                {
                    SetEndPointSelectionConfigs(PolicyDefinition.EndpointSelection);
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to set display mode
        /// </summary>
        public void DisplayMode()
        {
            UtilityMethods.DisableControls(contentWrapper);
        }
        #endregion

        protected void radExecutionOptions_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GridHeaderItem headerItem = radExecutionOptions.MasterTableView.GetItems(GridItemType.Header)[0] as GridHeaderItem;

                headerItem["EndpointSystemTypeName"].Text = T("App.Common.EndpointType");
            }
        }
    }
}