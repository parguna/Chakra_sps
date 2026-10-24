using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.DataManagement.InHousePermissions.UC
{
    public partial class List : DI.Chakra.Web.UIBaseUC, IAccessControl
    {
        const string MatrixName = "Inhouse Permission";
        static GridEditableItem selectedDataItem = null;
        private static IList<Dto.InhousePermission> inValidList;
        private XLDataConverterHelper xlHelper;
        public XLDataConverterHelper XlHelper
        {
            get
            {
                if (xlHelper == null)
                {
                    xlHelper = new XLDataConverterHelper();
                }
                return xlHelper;
            }
            set { xlHelper = value; }
        }
        public EndpointType EndpointSystemType
        {
            get
            {
                return (EndpointType)Convert.ToInt32(EndpointType.InhouseSystems);
            }
            set
            {
                ddlEndPointType.SelectedValue = ((byte)EndpointType.InhouseSystems).ToString();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.MiddleCell.Text = "Provisioning InHouse Permissions";
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.MiddleCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.LeftCell.Text = PDFFileLeftCell;
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.LeftCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(Session["ADXLMessage"])))
            {
                MasterPage.ShowAlert(AlertType.Success, Convert.ToString(Session["ADXLMessage"]));
                Session["ADXLMessage"] = string.Empty;
            }
            grdInvalidEntries.EnableLinqExpressions = true;
            XLUploadUC1.TemplateName = XLTemplates.Inhouse_Permissions;

            this.MasterPage.ActionClicked += ActionClicked;

            if (!Page.IsPostBack)
            {
                inValidList = new List<Dto.InhousePermission>();
                rbInValid.Checked = false;
                rbValid.Checked = true;
                ddlEndPointType.Items.Add(UtilityMethods.GetEnumAttribute<Description>(ApplicationType.InhouseSystems));
                ddlEndPointType.Enabled = false;
                this.EndpointSystemType = EndpointType.InhouseSystems;
                BindEndPointSystem();
                BindValidEntriesGrid();
                lblDetails.Text = XlHelper.GetWhoUpdatedData(XLUploadUC1.TemplateName);
            }
            else
            {
                if ((inValidList == null) || (inValidList != null && inValidList.Count == 0))
                {
                    FillValidEntriesGrid();
                }
            }

            XlHelper.InvisibleBetween(grdInvalidEntries.FilterMenu);
        }

        private void BindEndPointSystem()
        {
            try
            {
                int EndPointSystemTypeId = Convert.ToInt32(EndpointSystemType);
                ddlEndpoint.Items.Clear();
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", EndPointSystemTypeId.ToString());
                List<Dto.EndPoint> list = DomainApiClient<Dto.EndPoint>.GetByAction<List<Dto.EndPoint>>(DomainApiAction.Endpoint.EndPoint.GetByEndpointSystemTypeId, parameter).OrderBy(x => x.DisplayName).ToList();
                list.Insert(0, new Dto.EndPoint { ApplicationCode = "ALL", DisplayName = "ALL", Id = 0 });
                ddlEndpoint.DataTextField = "DisplayName";
                ddlEndpoint.DataValueField = "ApplicationCode";
                ddlEndpoint.DataSource = list;
                ddlEndpoint.DataBind();
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected void ddlEndpoint_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindValidEntriesGrid();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            modalPopup.VisibleOnPageLoad = false;
        }

        protected void grdInvalidEntries_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            grdInvalidEntries.DataSource = inValidList;
        }

        protected void grdInvalidEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem Grid = grdInvalidEntries.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)Grid.GetDataKeyValue("Id");
            if (this.ContextObjectId == 0)
            {
                this.ContextObjectId = null;
                Grid.Selected = false;
                //flag = false;
            }
            else
            {
                //flag = true;
            }
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Edit || e.ActionType == ACLAction.Detail || e.ActionType == ACLAction.Delete)
            {
                if (this.ContextObjectId == null || this.ContextObjectId == 0)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Invalid data cannot be processed");
                    return;
                }
            }
            if (e.ActionType == ACLAction.Delete)
            {
                delete();
            }
            else if (e.ActionType == ACLAction.Edit)
            {
                if (grdInvalidEntries.SelectedItems.Count > 0)
                {
                    selectedDataItem = grdInvalidEntries.SelectedItems[0] as GridEditableItem;
                    selectedDataItem.Edit = true;
                    grdInvalidEntries.Rebind();
                }
            }
            else if (e.ActionType == ACLAction.Add)
            {
                grdInvalidEntries.MasterTableView.InsertItem();
                grdInvalidEntries.Rebind();
                selectedDataItem = grdInvalidEntries.MasterTableView.GetInsertItem();
            }
            else if (e.ActionType == ACLAction.Save)
            {
                if (selectedDataItem != null)
                {
                    upsert();
                }
            }
            else if (e.ActionType == ACLAction.Upload)
            {
                modalPopup.VisibleOnPageLoad = true;
            }
            else if (e.ActionType == ACLAction.Download)
            {
                grdInvalidEntries.MasterTableView.Columns.FindByUniqueName("CreatedBy").Visible = true;
                grdInvalidEntries.MasterTableView.Columns.FindByUniqueName("CreatedDate").Visible = true;
                grdInvalidEntries.MasterTableView.GetColumn("CreatedBy").Display = true;
                grdInvalidEntries.MasterTableView.GetColumn("CreatedDate").Display = true;
                grdInvalidEntries.Rebind();
                grdInvalidEntries.DownloadTemplate(grdInvalidEntries, e.ActionType);
            }
            //else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            //{
            //    grdInvalidEntries.ExportSettings.FileName = "InhousePermission";
            //    ExportNew(grdInvalidEntries, e.ActionType);
            //}
            //else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            //{
            //    grdInvalidEntries.ExportSettings.FileName = "InhousePermission";
            //    ExportNew(grdInvalidEntries, e.ActionType);
            //}
            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                var data = grdInvalidEntries.DataSource as List<Dto.InhousePermission>;
                if (data != null && data.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(grdInvalidEntries.MasterTableView.FilterExpression))
                        data = data.AsEnumerable().AsQueryable().Where(grdInvalidEntries.MasterTableView.FilterExpression).ToList();
                    if (grdInvalidEntries.MasterTableView.SortExpressions != null && grdInvalidEntries.MasterTableView.SortExpressions.Count > 0)
                    {
                        data = data.AsEnumerable().AsQueryable().OrderBy(GetSortExpression(grdInvalidEntries.MasterTableView.SortExpressions)).ToList();
                    }
                    GenericExport(data, e.ActionType, "InhousePermission");
                }
            }
        }

        protected override Dto.IDMSReportParam GetReportHeaderColumnMapping()
        {
            var param = new Dto.IDMSReportParam();
            param.ReportDataColumns = new List<Dto.IDMSReportDataColumn>();
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("EntitlementName", "EntitlementName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("InhouseSystemCode", "InhouseSystemCode", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Description", "Description", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("ADGroups", "ADGroups", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("EPGroups", "EPGroups", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("EPRoles", "EPRoles", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("CEGroups", "CEGroups", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("CERoles", "CERoles", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Is Active", "IsActive", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Level1 Approver", "Level1Approver", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Level1 Approver Type", "Level1ApproverType", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Level2 Approver", "Level2Approver", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Level2 Approver Type", "Level2ApproverType", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Level3 Approver", "Level3Approver", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Level3 Approver Type", "Level3ApproverType", null));
            //param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Is Deleted", "IsDeleted", null));
            if (grdInvalidEntries.Columns.FindByDataField("ValidationMessage").Display)
                param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Validation Message", "ValidationMessage", null));
            return param;
        }

        protected void XLUploadUC1_UserControlButtonClicked(object sender, EventArgs e)
        {
            try
            {
                inValidList = null;
                IList<Dto.IXLData> invalid = XLUploadUC1.InvalidList;
                string errMsg = XLUploadUC1.ErrorMessage;
                if (invalid != null) inValidList = invalid.Select(item => (Dto.InhousePermission)item).ToList();
                if (!string.IsNullOrWhiteSpace(errMsg))
                {
                    rbInValid.Checked = true;
                    rbValid.Checked = false;
                    BindInValidGrid();
                    MasterPage.ShowAlert(AlertType.Error, "Below are the invalid records and those are not uploaded");
                    modalPopup.VisibleOnPageLoad = false;
                }
                else
                {
                    rbInValid.Checked = false;
                    rbValid.Checked = true;
                    BindValidEntriesGrid();
                    string mssg = "File uploaded sucessfully";
                    MasterPage.ShowAlert(AlertType.Success, mssg);
                    modalPopup.VisibleOnPageLoad = false;
                }
            }
            catch (Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, "Error while uploading Excel File");
            }
        }

        private void BindInValidGrid()
        {
            FillInvalidEntriesGrid();
        }

        public void delete()
        {
            if (!ContextObjectId.HasValue)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item.");
                return;
            }
            try
            {
                LogDeletedRecords<Dto.InhousePermission>(ContextObjectId.Value, MatrixName);
                SessionStatusImpl SessionUsr = new SessionStatusImpl();
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("permissionId", this.ContextObjectId.Value.ToString());
                parameter.Add("currentUser", SessionUsr.User.DisplayName);
                string message = DomainApiClient<Dto.InhousePermission>.GetByAction<string>("DeleteByPermissionId",parameter);
                if (message == "Inhouse Permission deleted Sucessfully.")
                {
                    MasterPage.ShowAlert(AlertType.Success, message);
                    Session["ADXLMessage"] = "Record deleted successfully.";
                    string redirectTarget = Page.Request.Url.PathAndQuery;
                    if (DI.Chakra.Infrastructure.UtilityMethods.IsLocalUrl(redirectTarget))
                        Page.Response.Redirect(redirectTarget, true);
                    else
                        Page.Response.Redirect("~/Default.aspx", true);
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Error, message);
                    //Session["ADXLMessage"] = message;
                    //Page.Response.Redirect(DI.Chakra.Infrastructure.UtilityMethods.GetSafeLocalRedirectUrl(Page.Request.Url.PathAndQuery), true);
                }
            }
            catch (Exception)
            {
                MasterPage.ShowAlert(AlertType.Error, "InhousePermission cannot be deleted.");
            }
        }

        private void upsert()
        {
            Dto.IXLData xldata;
            var editableItem = selectedDataItem;

            long id = 0;
            try
            {
                id = Convert.ToInt64(editableItem.GetDataKeyValue("Id"));
            }
            catch (Exception)
            {

            }

            Hashtable values = new Hashtable();
            string successMessage = string.Empty;
            bool editMode = false;
            string validationMessage = string.Empty;
            if (id > 0)
            {
                editMode = true;
                GridEditableItem item = grdInvalidEntries.EditItems[0] as GridEditableItem;
                xldata = PopulateData(item, id, out validationMessage);
                successMessage = "Record updated successfully.";
                oldData = GetSerializeString<Dto.InhousePermission>(id);
            }
            else
            {
                editMode = false;
                editableItem = grdInvalidEntries.MasterTableView.GetInsertItem();
                xldata = PopulateData(editableItem, id, out validationMessage);
                successMessage = "Record inserted successfully.";
            }

            if (!string.IsNullOrWhiteSpace(validationMessage))
            {
                MasterPage.ShowAlert(AlertType.Error, validationMessage);
                return;
            }

            Dictionary<string, string> parameterProcess = new Dictionary<string, string>();
            parameterProcess.Add("templateName", XLTemplates.Inhouse_Permissions.ToString());
            parameterProcess.Add("isEditMode", editMode.ToString());
            var datas = DomainApiClient<Dto.IXLData>.GetByAction<IList<DI.Chakra.Dto.IXLData>>(DomainApiAction.XLProcessor.XLData.ProcessXLData, parameterProcess, xldata);
            if (datas != null && datas.Count > 0)
            {
                string hasvalue = datas.First().ValidationMessage;
                if (string.IsNullOrWhiteSpace(hasvalue))
                {
                    UpdatedUILog<Dto.InhousePermission>(id,MatrixName,data:oldData);
                    MasterPage.ShowAlert(AlertType.Success, successMessage);
                    grdInvalidEntries.MasterTableView.IsItemInserted = false;
                    grdInvalidEntries.MasterTableView.ClearEditItems();
                    grdInvalidEntries.EnableRowOnDemandVS = true;
                    FillValidEntriesGrid();
                    grdInvalidEntries.DataBind();
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Error, hasvalue);
                    //e.Canceled = true;
                    return;
                }
            }
        }

        private Dto.InhousePermission PopulateData(GridEditableItem editableItem, long id, out string validationMsg)
        {
            var existingData = DomainApiClient<Dto.InhousePermission>.GetById(id);
            Hashtable values = new Hashtable();
            editableItem.ExtractValues(values);
            if (existingData != null)
            {
                TextBox Approvertype1 = editableItem.FindControl("TextBox1") as TextBox;
                TextBox Approvertype2 = editableItem.FindControl("TextBox2") as TextBox;
                TextBox Approvertype3 = editableItem.FindControl("TextBox3") as TextBox;
                if (IsValidApproverType(Approvertype1.Text))
                {
                    existingData.Level1ApproverType = GetApproverType(Approvertype1.Text);
                    validationMsg = "";
                }
                else
                {
                    validationMsg = InvalidApprovalTypeMatrixMessage;
                    return new Dto.InhousePermission();
                }
                if (IsValidApproverType(Approvertype2.Text))
                {
                    existingData.Level2ApproverType = GetApproverType(Approvertype2.Text);
                    validationMsg = "";
                }
                else
                {
                    validationMsg = InvalidApprovalTypeMatrixMessage;
                    return new Dto.InhousePermission();
                }
                if (IsValidApproverType(Approvertype3.Text))
                {
                    existingData.Level3ApproverType = GetApproverType(Approvertype3.Text);
                    validationMsg = "";
                }
                else
                {
                    validationMsg = InvalidApprovalTypeMatrixMessage;
                    return new Dto.InhousePermission();
                }

                existingData.Name = (string)values["EntitlementName"] == null ? string.Empty : (string)values["EntitlementName"];
                existingData.InhouseSystemCode = (string)values["InhouseSystemCode"] == null ? string.Empty : (string)values["InhouseSystemCode"];
                existingData.Description = (string)values["Description"] == null ? string.Empty : (string)values["Description"];

                existingData.Level1Approver = (string)values["Level1Approver"];
                existingData.Level2Approver = (string)values["Level2Approver"];
                existingData.Level3Approver = (string)values["Level3Approver"];

                existingData.ADGroups = (string)values["ADGroups"] == null ? string.Empty : (string)values["ADGroups"];
                existingData.EPGroups = (string)values["EPGroups"] == null ? string.Empty : (string)values["EPGroups"];
                existingData.EPRoles = (string)values["EPRoles"] == null ? string.Empty : (string)values["EPRoles"];
                existingData.CEGroups = (string)values["CEGroups"] == null ? string.Empty : (string)values["CEGroups"];
                existingData.CERoles = (string)values["CERoles"] == null ? string.Empty : (string)values["CERoles"];
                if(editableItem != null)
                {
                    var chkIsActive = editableItem.FindControl("chkIsActive") as CheckBox;
                    existingData.IsActive = chkIsActive.Checked;
                }
                
                return existingData;
            }
            else
            {
                Dto.InhousePermission xldata = new Dto.InhousePermission();
                TextBox Approvertype1 = editableItem.FindControl("TextBox1") as TextBox;
                TextBox Approvertype2 = editableItem.FindControl("TextBox2") as TextBox;
                TextBox Approvertype3 = editableItem.FindControl("TextBox3") as TextBox;
                if (IsValidApproverType(Approvertype1.Text))
                {
                    xldata.Level1ApproverType = GetApproverType(Approvertype1.Text);
                    validationMsg = "";
                }
                else
                {
                    validationMsg = InvalidApprovalTypeMatrixMessage;
                    return new Dto.InhousePermission();
                }
                if (IsValidApproverType(Approvertype2.Text))
                {
                    xldata.Level2ApproverType = GetApproverType(Approvertype2.Text);
                    validationMsg = "";
                }
                else
                {
                    validationMsg = InvalidApprovalTypeMatrixMessage;
                    return new Dto.InhousePermission();
                }
                if (IsValidApproverType(Approvertype3.Text))
                {
                    xldata.Level3ApproverType = GetApproverType(Approvertype3.Text);
                    validationMsg = "";
                }
                else
                {
                    validationMsg = InvalidApprovalTypeMatrixMessage;
                    return new Dto.InhousePermission();
                }
                xldata.Name = (string)values["EntitlementName"] == null ? string.Empty : (string)values["EntitlementName"];
                xldata.InhouseSystemCode = (string)values["InhouseSystemCode"] == null ? string.Empty : (string)values["InhouseSystemCode"];
                xldata.Description = (string)values["Description"] == null ? string.Empty : (string)values["Description"];

                xldata.Level1Approver = (string)values["Level1Approver"];
                xldata.Level2Approver = (string)values["Level2Approver"];
                xldata.Level3Approver = (string)values["Level3Approver"];

                xldata.ADGroups = (string)values["ADGroups"] == null ? string.Empty : (string)values["ADGroups"];
                xldata.EPGroups = (string)values["EPGroups"] == null ? string.Empty : (string)values["EPGroups"];
                xldata.EPRoles = (string)values["EPRoles"] == null ? string.Empty : (string)values["EPRoles"];
                xldata.CEGroups = (string)values["CEGroups"] == null ? string.Empty : (string)values["CEGroups"];
                xldata.CERoles = (string)values["CERoles"] == null ? string.Empty : (string)values["CERoles"];
                if (editableItem != null)
                {
                    var chkIsActive = editableItem.FindControl("chkIsActive") as CheckBox;
                    xldata.IsActive = chkIsActive.Checked;
                }
                return xldata;
            }
        }

        protected void grdInvalidEntries_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                ChangeEditButton(item, false);
            }
        }

        private void ChangeEditButton(GridDataItem item, bool visible)
        {
            if (item != null && item["Edit"].Controls.Count > 0)
            {
                LinkButton gridLinkButton = (LinkButton)item["Edit"].Controls[0];
                gridLinkButton.Visible = visible;
            }
        }

        protected void ValidRecordsSelectionChanged(object sender, EventArgs e)
        {
            BindValidEntriesGrid();
        }

        protected void BindValidEntriesGrid()
        {
            FillValidEntriesGrid();
            grdInvalidEntries.DataBind();
        }

        private void FillInvalidEntriesGrid()
        {
            grdHeader.InnerText = "InValid Records ";
            grdInvalidEntries.MasterTableView.Columns.FindByUniqueName("ValidationMessage").Display = true;
            grdInvalidEntries.MasterTableView.Columns.FindByUniqueName("Edit").Display = false;
            grdInvalidEntries.MasterTableView.CommandItemSettings.ShowAddNewRecordButton = false;
            grdInvalidEntries.MasterTableView.CommandItemSettings.ShowExportToCsvButton = false;
            grdInvalidEntries.MasterTableView.CommandItemSettings.ShowExportToExcelButton = false;
            grdInvalidEntries.DataSource = inValidList;
            grdInvalidEntries.DataBind();

        }

        private void FillValidEntriesGrid()
        {
            bool isValidRecords = rbValid.Checked;

            if (rbInValid.Checked == true)
                FillInvalidEntriesGrid();
            else
            {
                if (rbValid.Checked == true)
                    grdHeader.InnerText = "Valid Records ";
                grdInvalidEntries.MasterTableView.Columns.FindByUniqueName("ValidationMessage").Display = false;
                grdInvalidEntries.MasterTableView.Columns.FindByUniqueName("Edit").Display = true;
                grdInvalidEntries.MasterTableView.CommandItemSettings.ShowAddNewRecordButton = true;
                grdInvalidEntries.MasterTableView.CommandItemSettings.ShowExportToCsvButton = true;
                grdInvalidEntries.MasterTableView.CommandItemSettings.ShowExportToExcelButton = false;
                List<Dto.InhousePermission> validList = GetDatasource();
                grdInvalidEntries.DataSource = validList;
            }
        }

        private List<Dto.InhousePermission> GetDatasource()
        {
            var param = new Dictionary<string, string>();
            param.Add("InhouseSystemCode", Convert.ToString(ddlEndpoint.SelectedValue));
            List<Dto.InhousePermission> validList = DomainApiClient<Dto.InhousePermission>.GetByAction<List<Dto.InhousePermission>>(DomainApiAction.Endpoint.InhouseSystems.GetInhousePermissionGridData, param);
            ViewState[XlHelper.xldatasource] = validList;
            return validList;
        }

        
    }
}