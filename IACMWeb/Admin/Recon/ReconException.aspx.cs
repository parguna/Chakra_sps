using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Application;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure;
using DI.Chakra.Dto.Presentation;
using System.Reflection;

namespace DI.Chakra.Web.Admin.Recon
{
    public partial class ReconException : UIBasePage
    {
        #region variables
        private SessionStatusImpl sessionStatus = new SessionStatusImpl();
        private CheckBox chk = null;
        #endregion


        private void BindDatasource()
        {
            if (EndpointUC.SelectedEndpoint != null)
            {
                long EndpointSystemId = Convert.ToInt64(EndpointUC.SelectedEndpoint.Id);
                //if (chkAccountReconcilation.Checked)
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("endPointSystemId", EndpointSystemId.ToString());
                    parameter.Add("exceptionType", ExceptionTypesField.SelectedValue.ToString());
                    if (chkAccount.Checked)
                        parameter.Add("reconcilationType", ReconcilationType.Account.ToString());
                    else if (chkEntitlement.Checked)
                        parameter.Add("reconcilationType", ReconcilationType.Entitlement.ToString());
                    else
                        parameter.Add("reconcilationType", ReconcilationType.All.ToString());

                    parameter.Add("isResolved", IsResolvedField.Checked.ToString());
                    parameter.Add("isDeleted", IsDeletedField.Checked.ToString());


                    var gridList = DomainApiClient<PresentationAccountReconcilationReport>.GetByAction<List<PresentationAccountReconcilationReport>>(DomainApiAction.Provisioning.Presentation.PresentationAccountReconcilationReport.GetExceptions, parameter);
                    this.grdEntitlement.DataSource = gridList;
                    //if (chkAccount.Checked)
                    //this.GridViewControl1.DataSource = gridList;
                    //else
                    //    this.grdEntitlement.DataSource = gridList;

                    if (gridList != null && gridList.Count() > 0)
                    {
                        btnResolveManullay.Visible = IsResolvedField.Checked ? false : true;
                        //  grdpanel.Visible = true;
                        //  grdPanelEntitlement.Visible = false;
                        comments.Text = string.Empty;
                        Tr1.Visible = IsResolvedField.Checked ? false : true;
                    }
                    //else
                    //{
                    //    this.grdEntitlement.DataSource = new List<PresentationAccountReconcilationReport>();
                    //    //grdpanel.Visible = true;
                    //    Tr1.Visible = false;
                    //    btnResolveManullay.Visible = false;
                    //}
                    ExceptionTypes exceptionType = (ExceptionTypes)Enum.Parse(typeof(ExceptionTypes), ExceptionTypesField.SelectedItem.Text, true);
                    if (exceptionType == ExceptionTypes.Attribute)
                    {
                        //grdEntitlement.MasterTableView.GetColumn("TemplateInfoColumn").Visible = true;
                    }
                }
                //else
                //{
                //    Dictionary<string, string> parameter = new Dictionary<string, string>();
                //    parameter.Add("endPointSystemId", EndpointSystemId.ToString());
                //    parameter.Add("exceptionType", ExceptionTypesField.SelectedValue.ToString());
                //    parameter.Add("isResolved", ExceptionTypesField.SelectedValue.ToString());
                //    parameter.Add("isDeleted", ExceptionTypesField.SelectedValue.ToString());

                //    var gridList = DomainApiClient<PresentationEntitlementReconcilationReport>.GetByAction<List<PresentationEntitlementReconcilationReport>>(DomainApiAction.Provisioning.Presentation.PresentationEntitlementReconcilationReport.GetExceptions, parameter);
                //    this.GridViewControl1.DataSource = null;
                //    this.grdEntitlement.DataSource = gridList; buttonsTR.Visible = true;
                //    if (gridList != null)
                //    {
                //        buttonsTR.Visible = true;
                //        grdpanel.Visible = chkAccountReconcilation.Checked;
                //        grdPanelEntitlement.Visible = !chkAccountReconcilation.Checked;
                //        comments.Text = string.Empty;
                //        Tr1.Visible = true;
                //    }
                //}

            }
            //else
            //{
            //    grdpanel.Visible = btnResolveManullay.Visible = Tr1.Visible = grdPanelEntitlement.Visible = false;
            //}
        }
        private void BindException()
        {
            ExceptionTypesField.Items.Clear();
            foreach (ExceptionTypes item in Enum.GetValues(typeof(ExceptionTypes)))
            {
                var description = UtilityMethods.GetEnumAttribute<Description>(item);
                ListItem listItem = new ListItem(description, ((byte)item).ToString());
                if (item != ExceptionTypes.None && item != ExceptionTypes.Attribute)
                    ExceptionTypesField.Items.Add(listItem);
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                EndpointUC.Initialize();
                BindException();
            }
        }

        protected void EndpointUC_EndpointChanged(EndPoint endpoint)
        {
        }
        protected void GridViewControl1_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            BindDatasource();
        }

        protected void btnResolveManullay_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(comments.Text))
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Please add comment");
                    return;
                }
                if (!(AsyncUploadAttach.UploadedFiles.Count > 0))
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Please add attachment");
                    return;
                }
                long EndpointSystemId = (long)EndpointUC.SelectedEndpoint.Id;
                // long ReconcilationId = 0;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                foreach (GridDataItem item in grdEntitlement.Items)
                {
                    parameter.Clear();
                    if (item.Selected)
                    {
                        long ReconciliationId = (long)item.GetDataKeyValue("ReconciliationId");
                        //long endpointSystemId = (long)item.GetDataKeyValue("EndpointSystemId");
                        //long endpointUserId = (long)item.GetDataKeyValue("EndpointUserId");
                        //int EntitlementType = (int)item.GetDataKeyValue("EntitlementType");
                        //string Entitlement = (string)item.GetDataKeyValue("Entitlement");
                        ReconcilationType ReconcilationType = (ReconcilationType)item.GetDataKeyValue("ReconcilationType");
                        // ExceptionTypes exceptionType = (ExceptionTypes)item.GetDataKeyValue("ExceptionType");

                        //parameter.Add("endPointSystemId", endpointSystemId.ToString());
                        //parameter.Add("endpointUserId", endpointUserId.ToString());


                        if (ReconcilationType == ReconcilationType.Account)
                        {
                            //parameter.Add("exceptionType", exceptionType.ToString());
                            AccountReconcilation accountReconcile = DomainApiClient<Dto.AccountReconcilation>.GetById(ReconciliationId);
                            //accountReconcile = DomainApiClient<AccountReconcilation>.GetByAction<AccountReconcilation>
                            //    (DomainApiAction.Provisioning.AccountReconcilation.GetUserByException, parameter);
                            accountReconcile.IsResolved = true;
                            accountReconcile.ResolvedDate = DateTime.Now;
                            accountReconcile.ResolvedStatus = ResolvedStatus.Manual;
                            accountReconcile.ResolvedBy = sessionStatus.User.UserName;
                            accountReconcile.Comments = comments.Text;
                            DomainApiClient<Dto.AccountReconcilation>.Save(accountReconcile);
                            //ReconcilationId = accountReconcile.Id;
                        }
                        else if (ReconcilationType == ReconcilationType.Entitlement)
                        {
                            EntitlementReconcilation entitlementReconcile = DomainApiClient<Dto.EntitlementReconcilation>.GetById(ReconciliationId);
                            //parameter.Add("entitlement", Entitlement);
                            //parameter.Add("entitlementType", EntitlementType.ToString());
                            //entitlementReconcile = DomainApiClient<EntitlementReconcilation>.GetByAction<EntitlementReconcilation>
                            //    (DomainApiAction.Provisioning.EntitlementReconcilation.GetByUser, parameter);
                            entitlementReconcile.ResolvedStatus = ResolvedStatus.Manual;
                            entitlementReconcile.IsResolved = true;
                            entitlementReconcile.ResolvedDate = DateTime.Now;
                            entitlementReconcile.ResolvedBy = sessionStatus.User.UserName;
                            entitlementReconcile.Comments = comments.Text;
                            DomainApiClient<Dto.EntitlementReconcilation>.Save(entitlementReconcile);
                            //ReconcilationId = entitlementReconcile.Id;
                        }
                    }
                }
                grdEntitlement.Rebind();
                comments.Text = string.Empty;

                MasterPage.ShowAlert(AlertType.Success, "Reconciliation Exception has been resolved");
            }
            catch (SystemException ex)
            {
                MasterPage.ShowAlert(AlertType.Error, "Error occured on resolving Reconciliation Exception");
            }

        }
        protected void search_Click(object sender, EventArgs e)
        {
            if (EndpointUC.SelectedEndpoint == null)
            {
                MasterPage.ShowAlert(AlertType.Warning, "Endpoint field must be required");
                return;
            }
            grdEntitlement.Rebind();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            //ViewAttributes.VisibleOnPageLoad = false;
        }

        protected void grdEntitlement_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BindDatasource();
        }

        protected void chkEntitlement_CheckedChanged(object sender, EventArgs e)
        {
            var x = sender as CheckBox;
            if (x != null)
            {
                bool checkStatus = x.Checked;
                foreach (GridDataItem item in grdEntitlement.MasterTableView.Items)
                {
                    CheckBox chkbx = (CheckBox)item["CheckTemp"].FindControl("ClientSelectEntitlementColumn");
                    chkbx.Checked = checkStatus;
                    item.Selected = chkbx.Checked;
                }
            }
        }

        protected void ClientSelectEntitlementColumn_CheckedChanged(object sender, EventArgs e)
        {
            int gridcount = grdEntitlement.MasterTableView.Items.Count;
            int selectedcount = 0;
            foreach (GridDataItem item in grdEntitlement.MasterTableView.Items)
            {
                CheckBox chkbx = (CheckBox)item["CheckTemp"].FindControl("ClientSelectEntitlementColumn");
                if (chkbx.Checked)
                    selectedcount++;
                item.Selected = chkbx.Checked;
            }
            foreach (GridItem item in grdEntitlement.MasterTableView.GetItems(GridItemType.Header))
            {
                CheckBox chkbx = (CheckBox)item.FindControl("chkEntitlement");
                if (selectedcount == gridcount)
                {
                    chkbx.Checked = true;
                }
                else
                {
                    chkbx.Checked = false;
                }
            }
        }

        protected void grdEntitlement_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridHeaderItem)
            {
                GridHeaderItem headerItem = (GridHeaderItem)e.Item;
                chk = (CheckBox)headerItem.FindControl("chkEntitlement");

                if (!IsResolvedField.Checked)
                {
                    GridColumn attachmentColumn = grdEntitlement.MasterTableView.GetColumn("Attachment");
                    attachmentColumn.Display = false;
                }
                else
                {
                    GridColumn attachmentColumn = grdEntitlement.MasterTableView.GetColumn("Attachment");
                    attachmentColumn.Display = true;
                }
            }

            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                CheckBox chkbx = (CheckBox)item.FindControl("ClientSelectEntitlementColumn");
                if (IsResolvedField.Checked)
                {
                    chkbx.Visible = false;
                    chk.Visible = false;
                    GridColumn attachmentColumn = grdEntitlement.MasterTableView.GetColumn("Attachment");
                    attachmentColumn.Display = true;
                }
                else
                {
                    chkbx.Visible = true;
                    chk.Visible = true;
                    GridColumn attachmentColumn = grdEntitlement.MasterTableView.GetColumn("Attachment");
                    attachmentColumn.Display = false;
                }
            }
        }

        protected void grdEntitlement_SelectedIndexChanged(object sender, EventArgs e)
        {
            //GridDataItem item = grdEntitlement.SelectedItems[0] as GridDataItem;
            //ReconcilationType ReconcilationType = (ReconcilationType)item.GetDataKeyValue("ReconcilationType");
            //long ReconciliationId = (long)item.GetDataKeyValue("ReconciliationId");
            //Dictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters.Add("id", ReconciliationId.ToString());
            //parameters.Add("reconType", ReconcilationType.ToString());
            //ReconcilationAttachment attachment = DomainApiClient<Dto.ReconcilationAttachment>.GetByAction<Dto.ReconcilationAttachment>("GetReconcilationAttachment", parameters);
            //if(attachment != null)
            //{
            //    ShowDocument(attachment.FileName, attachment.AttachmentFile);
            //}
        }

        protected void grdEntitlement_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = e.Item as GridDataItem;
                ReconcilationType ReconcilationType = (ReconcilationType)item.GetDataKeyValue("ReconcilationType");
                long ReconciliationId = (long)item.GetDataKeyValue("ReconciliationId");
            }
        }

        protected void UpdatePanelDownload_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
         .Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });

        }

        protected void ExceptionTypesField_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ExceptionTypesField.SelectedItem.Text == "Removed")
            {
                IsDeletedField.Checked = true;
                Label3.Visible = false;
                IsDeletedField.Visible = false;
            }
            else
            {
                IsDeletedField.Checked = false;
                Label3.Visible = false;
                IsDeletedField.Visible = false;
            }
        }
    }
}
