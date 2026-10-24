using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;


namespace DI.Chakra.Web.ProvRS.DefaultApplication.UC
{
    public partial class DetailUC : UIBaseUC, IAccessControl
    {
        #region Member
        private const string V_SELECTEDAPPLICATION = "V_SELECTEDAPPLICATION";
        private const string V_RULEID = "V_RULEID";
        private const string V_ENDPOINTID = "V_ENDPOINTID";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        private const string V_ENTITYID = "V_ENTITYID";
        #endregion

        #region Properties
        private List<Dto.PresentationNRulesetForApplication> SelectedApplications
        {
            get
            {
                if (ViewState[V_SELECTEDAPPLICATION] != null)
                {
                    return (List<Dto.PresentationNRulesetForApplication>)ViewState[V_SELECTEDAPPLICATION];
                }
                return new List<Dto.PresentationNRulesetForApplication>();
            }
            set
            {
                ViewState[V_SELECTEDAPPLICATION] = value;
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

        public long EndPointSystemId
        {
            get
            {
                if (ViewState[V_ENDPOINTID] == null)
                    return 0;
                return (long)ViewState[V_ENDPOINTID];
            }
            set
            {
                ViewState[V_ENDPOINTID] = value;

            }
        }


        public long RuleId
        {
            get
            {
                if (ViewState[V_RULEID] == null)
                    return 0;
                return (long)ViewState[V_RULEID];
            }
            set
            {
                ViewState[V_RULEID] = value;
            }
        }

        public long EntityId
        {
            get
            {
                if (ViewState[V_ENTITYID] == null)
                    return 0;
                return (long)ViewState[V_ENTITYID];
            }
            set
            {
                ViewState[V_ENTITYID] = value;
            }
        }

        #endregion

        #region Method

        private void FormGridEnable()
        {
            if ((UIMode == UIMode.Edit) || (UIMode == UIMode.Add))
            {
                if (ApplicationDefaultGrid.Items.Count == 0)
                    ApplicationDefaultGrid.Visible = false;
                else
                    ApplicationDefaultGrid.Visible = true;
            }
        }

        private void SetPage(UIMode UMode)
        {
            NRulesetForApplication rulesetforApp = DomainApiClient<Dto.NRulesetForApplication>.GetById(EntityId);
            if (rulesetforApp != null)
            {
                RuleId = rulesetforApp.RuleId;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("ruleId", rulesetforApp.RuleId.ToString());
                NRuleset ruleset = DomainApiClient<NRuleset>.GetByAction<NRuleset>(DomainApiAction.Ruleset.NRuleset.GetSingleRulesetByRuleId, parameter);
                if (ruleset != null)
                    RuleUC.SelectedRuleSet = ruleset;
            }
            RuleUC.DisableMode();
            BindGrid();
        }

        private void BindGrid()
        {
            if ((RuleUC.SelectedRuleSet != null) && (RuleUC.SelectedRuleSet.Id > 0))
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("rulesetId", RuleId.ToString());
                this.SelectedApplications = DomainApiClient<Dto.PresentationNRulesetForApplication>.GetByAction<List<Dto.PresentationNRulesetForApplication>>(DomainApiAction.Ruleset.PresentationNRulesetForApplication.GetByRuleset, parameters);
                if ((UIMode == UIMode.Add) && (this.SelectedApplications.Count > 0))
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Application Default already Exits for rule  " + this.SelectedApplications[0].RuleCode);
                    return;
                }
                ApplicationDefaultGrid.DataSource = this.SelectedApplications;
                ApplicationDefaultGrid.Rebind();
            }
        }

        public void Intialize()
        {
            EndpointUC.IncludeInhouse = true;
            EndpointUC.Initialize();
        }

        public void DataForm(NRulesetForApplication application)
        {

            PresentationNRulesetForApplication Lst = new PresentationNRulesetForApplication();
            Lst.RuleId = application.RuleId;
            Lst.EndpointSystemId = application.EndpointSystemId;
            Lst.AccountType = application.AccountType;
            Lst.CreatedDate = DateTime.Now;
            Lst.CreatedBy = new SessionStatusImpl().User.DisplayName;
            if (RuleUC.SelectedRuleSet.Code != null)
                Lst.RuleCode = RuleUC.SelectedRuleSet.Code;
            Lst.EndpointSystemName = EndpointUC.SelectedEndpoint.DisplayName;
            this.SelectedApplications.Add(Lst);
        }


        private void Save()
        {
            if (ApplicationDefaultGrid.Items.Count == 0)
            {
                MasterPage.ShowAlert(AlertType.Warning, "No RuleSet for Application has been added");
                return;
            }

            if (UIMode == UIMode.Add)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("ruleId", RuleId.ToString());
                DomainApiClient<Dto.NRulesetForApplication>.GetByAction<System.Object>(DomainApiAction.Ruleset.NRulesetforApplication.SaveApplicationDefault, parameters, this.SelectedApplications);
                MasterPage.ShowAlert(AlertType.Success, "Application Defaults saved successfully");
            }
            else if (UIMode == UIMode.Edit)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("ruleId", RuleId.ToString());
                DomainApiClient<Dto.NRulesetForApplication>.GetByAction<System.Object>(DomainApiAction.Ruleset.NRulesetforApplication.SaveApplicationDefault, parameters, this.SelectedApplications);
                MasterPage.ShowAlert(AlertType.Success, "Application Defaults updated successfully");
            }
        }

        private bool Validate()
        {
            bool flag;
            flag = true;
            if ((RuleUC.SelectedRuleSet == null) && (EndpointUC.SelectedEndpoint == null))
            {
                this.MasterPage.ShowAlert(AlertType.Error, string.Format("Please Select the Ruleset and EndPointSystem"));
                flag = false;
            }
            else if (RuleUC.SelectedRuleSet == null)
            {
                this.MasterPage.ShowAlert(AlertType.Error, string.Format("Please Select the Ruleset"));
                flag = false;
            }
            else if (EndpointUC.SelectedEndpoint == null)
            {
                this.MasterPage.ShowAlert(AlertType.Error, string.Format("Please Select the EndPointSystem"));
                flag = false;
            }
            return flag;
        }



        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                if (UIMode == UIMode.Edit)
                {
                    this.EntityId = this.ContextObjectId != null ? (int)this.ContextObjectId.Value : 0;
                    SetPage(UIMode);
                }

                BindGrid();
            }
        }


        protected void RuleUC_RuleChanged(NRuleset Rule)
        {

            if (Rule != null && Rule.RuleId > 0)
                this.RuleId = Rule.RuleId;
            else
                this.RuleId = 0;

            BindGrid();
            ApplicationDefaultGrid.Rebind();

        }

        protected void EndpointUC_EndpointChanged(EndPoint EndPoint)
        {

        }


        protected void RuleAppicationGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                GridDataItem item = e.Item as GridDataItem;
                long RuleId = (long)Convert.ChangeType(item.GetDataKeyValue("RuleId"), typeof(long));
                long EndpointSystemId = (long)Convert.ChangeType(item.GetDataKeyValue("EndpointSystemid"), typeof(long));
                var app = this.SelectedApplications.Where(m => m.RuleId == RuleId && m.EndpointSystemId == EndpointSystemId).FirstOrDefault();
                this.SelectedApplications.Remove(app);
                ApplicationDefaultGrid.DataSource = this.SelectedApplications;
                ApplicationDefaultGrid.Rebind();
                FormGridEnable();
            }
        }

        protected void Addbutton_Click(object sender, EventArgs e)
        {
            if (Validate())
            {
                if (RuleUC.SelectedRuleSet != null && EndpointUC.SelectedEndpoint != null)
                {
                    long EndpointId = EndpointUC.SelectedEndpoint.Id;
                    var selectedRuleset = RuleUC.SelectedRuleSet;

                    if (!SelectedApplications.Any(m => m.RuleId == selectedRuleset.RuleId && m.EndpointSystemId == EndpointId))
                    {
                        Dto.NRulesetForApplication application = new Dto.NRulesetForApplication();
                        application.RuleId = selectedRuleset.RuleId;
                        application.EndpointSystemId = EndpointUC.SelectedEndpoint.Id;
                        application.AccountType = AccountType.User.ToString();
                        application.CreatedDate = DateTime.Now;
                        application.CreatedBy = new SessionStatusImpl().User.UserName;
                        DataForm(application);
                        ApplicationDefaultGrid.DataSource = this.SelectedApplications;
                        ApplicationDefaultGrid.DataBind();
                        FormGridEnable();
                    }
                    else
                    {
                        MasterPage.ShowAlert(AlertType.Warning, "Application Ruleset Added Already");
                    }
                    EndpointUC.ClearApplication();
                }
            }
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                if (ApplicationDefaultGrid.Items.Count == 0)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "No RuleSet for Application has been added");
                    return;
                }
                else
                {
                    Save();
                    DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Application Defaults - Save");
                    Response.Redirect("~/ProvRS/DefaultApplication/List.aspx");
                }


            }
        }


        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/ProvRS/DefaultApplication/List.aspx");
        }

        #endregion
    }
}

