using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Telerik.Web.UI;


namespace DI.Chakra.Web.Job.CustomReport.UC
{
    public partial class DetailUC : UIBaseUC, IAccessControl
    {
        private const string V_EntityID = "V_EntityID";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
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
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!IsPostBack)
            {
                if (UIMode == UIMode.Edit || UIMode == UIMode.View)
                {
                    this.EntityId = this.ContextObjectId.Value;
                    SetPage(UIMode);
                }
                if (UIMode == UIMode.View)
                    ViewOnlyMode();

                ddlColumns.Enabled = false;
            }
        }

        private void ViewOnlyMode()
        {
            txtReportName.Enabled = false;
            txtDescription.Enabled = false;
            txtQuery.Enabled = false;
            btnValidate.Enabled = false;
            btnGenerateParameters.Enabled = false;
            chkLinkToMenu.Enabled = false;
            GenericAutoCompleteBox.ReadOnly = true;
        }
        private void SetPage(UIMode mode)
        {
            var reportDefinition = DomainApiClient<Dto.CustomReport>.GetById(this.EntityId, true);
            if (reportDefinition != null)
            {
                txtReportName.Text = reportDefinition.Name;
                txtReportName.Enabled = false;
                txtDescription.Text = reportDefinition.Description;
                txtQuery.Text = reportDefinition.ReportQuery;
                chkLinkToMenu.Checked = reportDefinition.IsLinkedToMenu;
                chkGroupBy.Checked = reportDefinition.IsGroupByEnabled;
                ddlColumns.Items.Add(reportDefinition.GroupByColumn);
                ddlColumns.Text = reportDefinition.GroupByColumn;
                chkGroupBy.Enabled = false;
                CustomReportParameterList.DataSource = reportDefinition.CustomReportParameters.Where(p => !p.IsDeleted).Select(p => p).ToList(); ;
            }

            if (reportDefinition != null && reportDefinition.IsLinkedToMenu && reportDefinition.LinkedToMenu.HasValue)
            {
                var menu = DomainApiClient<Dto.TabMenu>.GetById(reportDefinition.LinkedToMenu.Value);
                if (menu != null)
                    this.GenericAutoCompleteBox.SetValue(menu.UniqueName + "(" + menu.MenuName + ")", menu.Id.ToString());

                this.chkLinkToMenu.Enabled = false;
                this.GenericAutoCompleteBox.ReadOnly = true;
            }
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                Save();
            }
        }
        private void Save()
        {
            if (string.IsNullOrEmpty(txtReportName.Text) || string.IsNullOrEmpty(txtDescription.Text) || string.IsNullOrEmpty(txtQuery.Text))
            {
                MasterPage.ShowAlert(AlertType.Error, "Please enter required fields.");
                return;
            }

            if (chkLinkToMenu.Checked && (GenericAutoCompleteBox.GetValue() == null || GenericAutoCompleteBox.GetValue().Text.Trim().Length == 0))
            {
                MasterPage.ShowAlert(AlertType.Warning, "Please select the menu to link");
                GenericAutoCompleteBox.Focus();
                return;
            }

            var query = txtQuery.Text;
            bool isMatch = IsValidQuery(query);
            if (isMatch)
            {
                MasterPage.ShowAlert(AlertType.Error, "Entered query is not valid for report, please enter SELECT query only.");
                return;
            }

            var children = new List<Dto.CustomReportParameter>();

            CustomReportParameterList.Items.ForEach(item =>
            {
                var CustomReportParameterItem = (CustomReportParameterItemUC)item.FindControl("CustomReportParameterItemUC");
                children.Add(CustomReportParameterItem.CustomReportParameterItem);
            });

            if (children.Count > 0 && children.Any(a => a.ParameterVariant == CustomReportParameterVariant.Value))
            {
                var valueVariant = children.Where(a => a.ParameterVariant == CustomReportParameterVariant.Value).ToList();
                if (valueVariant.Any(a => string.IsNullOrEmpty(a.DefaultValue)))
                {
                    var emptyValueVariant = valueVariant.Where(a => string.IsNullOrEmpty(a.DefaultValue)).Select(a => a.ParameterName).ToList();
                    MasterPage.ShowAlert(AlertType.Error, string.Format("Please entered default value for parameter(s) {0}.", string.Join(",", emptyValueVariant)));
                    return;
                }
            }

            if (UIMode == UIMode.Add)
            {

                Dto.CustomReport definition = new Dto.CustomReport();
                definition.Name = txtReportName.Text;
                definition.Description = txtDescription.Text;
                definition.ReportQuery = txtQuery.Text;

                if (chkLinkToMenu.Checked && !string.IsNullOrEmpty(GenericAutoCompleteBox.GetValue().Value))
                {
                    definition.IsLinkedToMenu = true;
                    definition.LinkedToMenu = Convert.ToInt64(GenericAutoCompleteBox.GetValue().Value);
                }
                if (definition.CustomReportParameters == null)
                    definition.CustomReportParameters = new List<Dto.CustomReportParameter>();
                children.ForEach(child =>
                {
                    child.CustomReport = definition;
                    definition.CustomReportParameters.Add(child);
                });

                if (chkGroupBy.Checked)
                {
                    definition.IsGroupByEnabled = true;
                    definition.GroupByColumn = ddlColumns.Text;
                }
                DomainApiClient<Dto.CustomReport>.Save(definition);
                //var refreshCache = DomainApiClient<Dto.CustomReport>.GetByAction<bool>("RefreshWebUrlCache", null);
                //try
                //{
                //    if (refreshCache)
                //    {
                //        MasterPage.RefreshCache<Dto.PageUrl>();
                //        MasterPage.RefreshCache<Dto.WebPage>();
                //        MasterPage.RefreshCache<Dto.PagePermission>();
                //        MasterPage.RefreshCache<Dto.TabMenu>();
                //    }
                //}
                //catch (Exception ex)
                //{

                //}
                //DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, string.Format("Custom Report Added for {0}", definition.Name), "Report");
            }
            else
            {
                Dto.CustomReport reportDef = DomainApiClient<Dto.CustomReport>.GetById(this.EntityId, true);
                if (reportDef != null)
                {
                    reportDef.Name = txtReportName.Text;
                    reportDef.Description = txtDescription.Text;
                    reportDef.ReportQuery = txtQuery.Text;

                    if (chkLinkToMenu.Checked && !string.IsNullOrEmpty(GenericAutoCompleteBox.GetValue().Value))
                    {
                        try
                        {
                            reportDef.IsLinkedToMenu = true;
                            reportDef.LinkedToMenu = Convert.ToInt64(GenericAutoCompleteBox.GetValue().Value);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    if (reportDef.CustomReportParameters == null)
                    {
                        reportDef.CustomReportParameters = new List<Dto.CustomReportParameter>();
                    }

                    children.ForEach(childParameter =>
                    {
                        var parameter = reportDef.CustomReportParameters.FirstOrDefault(cparam => cparam.ParameterName == childParameter.ParameterName);
                        if (parameter == null)
                        {
                            childParameter.CustomReport = reportDef;
                            childParameter.CustomReportId = this.EntityId;
                            reportDef.CustomReportParameters.Add(childParameter);
                        }
                        else
                        {
                            parameter.ParameterType = childParameter.ParameterType;
                            parameter.DefaultValue = childParameter.DefaultValue;
                            parameter.Description = childParameter.Description;
                            parameter.DataFormatString = childParameter.DataFormatString;
                            parameter.ParameterVariant = childParameter.ParameterVariant;
                            parameter.IsDeleted = false;
                        }
                    });

                    reportDef.CustomReportParameters.ForEach(childParameter =>
                    {
                        var parameter = children.FirstOrDefault(cparam => cparam.ParameterName == childParameter.ParameterName && !childParameter.IsDeleted);
                        if (parameter == null)
                        {
                            childParameter.IsDeleted = true;
                        }
                    });

                    DomainApiClient<Dto.CustomReport>.Save(reportDef);
                    // DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, string.Format("Custom Report Edited for {0}", reportDef.Name), "Report");
                }
            }
            Response.Redirect("~//Job/CustomReport//List.aspx");
        }

        private bool IsValidQuery(string query)
        {
            string matchWords = @"\b(delete |insert |update |create |with |drop |alter |--|cursor |exec |execute |commit|waitfor |kill )\b";
            Regex reg = new Regex(matchWords, RegexOptions.IgnoreCase);
            bool isMatch = reg.IsMatch(query);
            if (isMatch || !query.ToLower().StartsWith("select"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        protected void btnValidate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtQuery.Text))
            {
                MasterPage.ShowAlert(AlertType.Error, "Please enter the select query for report.");
                return;
            }
            else
            {
                var query = txtQuery.Text;
                bool isMatch = IsValidQuery(query);
                if (isMatch)
                {
                    MasterPage.ShowAlert(AlertType.Error, "Entered query is not valid for report, please enter SELECT query only.");
                    return;
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Success, "Query is valid proceed to save.");
                    return;
                }
            }
        }

        protected void btnGenerateParameters_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtQuery.Text))
            {
                MasterPage.ShowAlert(AlertType.Success, "Please enter query.");
                return;
            }
            var results = new Dictionary<string, CustomReportParameterVariant>();
            var list = new List<Dto.CustomReportParameter>();
            var query = txtQuery.Text;
            Regex regexObj = new Regex(@":\w*\.?\w*");
            Match matchResults = regexObj.Match(query);
            while (matchResults.Success)
            {
                results.Add(matchResults.ToString().Replace(":", ""), CustomReportParameterVariant.Column);
                matchResults = matchResults.NextMatch();
            }

            regexObj = new Regex(@"@\w*");
            matchResults = regexObj.Match(query);
            while (matchResults.Success)
            {
                results.Add(matchResults.ToString().Replace("@", ""), CustomReportParameterVariant.Value);
                matchResults = matchResults.NextMatch();
            }

            int count = 0;
            foreach (var str in results)
            {
                var parameter = new Dto.CustomReportParameter();
                parameter.Sequence = count = count + 1;
                parameter.ParameterName = str.Key;
                parameter.Description = str.Key;
                parameter.ParameterVariant = str.Value;
                parameter.ParameterType = CustomReportParameterTypes.varchar;
                list.Add(parameter);
            }
            CustomReportParameterList.DataSource = list;
        }
        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~//Job/CustomReport//List.aspx");
        }

        protected void chkLinkToMenu_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void chkGroupBy_CheckedChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtQuery.Text))
                return;

            if (chkGroupBy.Checked)
            {
                BindColumnNames();
            }
            else
            {
                ddlColumns.Items.Clear();
                ddlColumns.Enabled = false;
            }
        }

        private void BindColumnNames()
        {
            ddlColumns.Enabled = true;
            var parameter = new Dictionary<string, string>();
            parameter.Add("query", txtQuery.Text);
            var columns = DomainApiClient<Dto.CustomReport>.GetByAction<List<string>>
                (DomainApiAction.CustomReport.GetAllColumnsFromQuery, parameter);
            ddlColumns.DataSource = columns;
            ddlColumns.DataBind();
        }

        protected void ddlMenu_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void CustomReportParameterList_NeedDataSource(object sender, Telerik.Web.UI.RadListViewNeedDataSourceEventArgs e)
        {
        }

        protected void CustomReportParameterList_ItemDataBound(object sender, Telerik.Web.UI.RadListViewItemEventArgs e)
        {

            Telerik.Web.UI.RadListViewDataItem dataItem = (Telerik.Web.UI.RadListViewDataItem)e.Item;

            if (e.Item.ItemType == RadListViewItemType.DataItem || e.Item.ItemType == RadListViewItemType.AlternatingItem)
            {
                Dto.CustomReportParameter yourDataSource = (Dto.CustomReportParameter)dataItem.DataItem;

                CustomReportParameterItemUC CustomReportParameterItemUC = (CustomReportParameterItemUC)e.Item.FindControl("CustomReportParameterItemUC");
                CustomReportParameterItemUC.CustomReportParameterItem = yourDataSource;
                CustomReportParameterItemUC.Initialize();
            }
        }

        protected void CustomReportParameterItemUC_ItemClicked(object sender, EventArgs e)
        {
            //PresentationAccessRequestItem item = (PresentationAccessRequestItem)sender;
            //string message = string.Empty;

            //ProcessRequestedItem(item);
            //if (!string.IsNullOrEmpty(message))
            //{
            //    MasterPage.ShowAlert(AlertType.Warning, "Item Added Successfully-" + message);
            //}
            //else
            //{
            //    MasterPage.ShowAlert(AlertType.Success, "Item Added Successfully");
            //}

            //PIMRequestList.Rebind();

            //if (ItemClicked != null)
            //    ItemClicked(this, e);
        }


    }
}