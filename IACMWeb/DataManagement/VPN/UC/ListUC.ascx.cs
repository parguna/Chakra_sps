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

namespace DI.Chakra.Web.DataManagement.VPN.UC
{
    public partial class List : DI.Chakra.Web.UIBaseUC, IAccessControl
    {
        const string MatrixName = "VPN Matrix";
        static GridEditableItem selectedDataItem = null;
        private static IList<Dto.VPNMatrix> inValidList;
        static bool reloadDatasource = false;
        #region Members
        #endregion

        #region Properties
        public VPNMatrixType RequestType
        {
            get
            {
                if (ViewState["VPNMatrixType"] == null)
                    return VPNMatrixType.Application;
                return ((VPNMatrixType)ViewState["VPNMatrixType"]);
            }
            set
            {
                ViewState["VPNMatrixType"] = value;
            }
        }
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
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            rbAll.Visible = false;
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.MiddleCell.Text = "VPNMatrix";
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.MiddleCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.LeftCell.Text = PDFFileLeftCell;
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.LeftCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(Session["ADXLMessage"])))
            {
                var alertType = Session["ADXLMessage"].ToString().Contains("successfully", StringComparison.InvariantCultureIgnoreCase) ? AlertType.Success : AlertType.Error;
                MasterPage.ShowAlert(alertType, Convert.ToString(Session["ADXLMessage"]));
                Session["ADXLMessage"] = string.Empty;
            }
            XLUploadUC1.TemplateName = XLTemplates.VPNConfigurations;

            this.MasterPage.ActionClicked += ActionClicked;

            if (!Page.IsPostBack)
            {
                inValidList = new List<Dto.VPNMatrix>();
                tblValidRecordDetails.Visible = true;
                rbInValid.Checked = false;
                rbAll.Checked = true;
                rbValid.Checked = true;
                BindValidEntriesGrid();
                lblDetails.Text = XlHelper.GetWhoUpdatedData(XLUploadUC1.TemplateName);
                //var column = grdInvalidEntries.MasterTableView.GetColumn("VPNMatrixType");
                //column.CurrentFilterFunction = GridKnownFunction.EqualTo;
                //column.DataType = typeof(VPNType);
                //column.AutoPostBackOnFilter = true;
            }
            else
                FillValidEntriesGrid();

            XlHelper.InvisibleBetween(grdInvalidEntries.FilterMenu);
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
            RequestType = (VPNMatrixType)Grid.GetDataKeyValue("VPNMatrixType");
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
            if (rbInValid.Checked)
            {
                if (e.ActionType == ACLAction.Add || e.ActionType == ACLAction.Edit || e.ActionType == ACLAction.Delete || e.ActionType == ACLAction.Save)
                    return;
            }

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
            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                var data = grdInvalidEntries.DataSource as List<Dto.VPNMatrix>;
                if (data != null && data.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(grdInvalidEntries.MasterTableView.FilterExpression))
                        data = data.AsEnumerable().AsQueryable().Where(grdInvalidEntries.MasterTableView.FilterExpression).ToList();
                    if (grdInvalidEntries.MasterTableView.SortExpressions != null && grdInvalidEntries.MasterTableView.SortExpressions.Count > 0)
                    {
                        data = data.AsEnumerable().AsQueryable().OrderBy(GetSortExpression(grdInvalidEntries.MasterTableView.SortExpressions)).ToList();
                    }
                    GenericExport(data, e.ActionType, "VPNMatrix");
                }
            }
            //else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            //{
            //    grdInvalidEntries.ExportSettings.FileName = "VPNMatrix";
            //    //ExportNew(grdInvalidEntries, e.ActionType);
            //    ExportNew(grdInvalidEntries, e.ActionType, "VPNMatrix");
            //}
            //else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            //{
            //    grdInvalidEntries.ExportSettings.FileName = "VPNMatrix";
            //    //ExportNew(grdInvalidEntries, e.ActionType);
            //    ExportNew(grdInvalidEntries, e.ActionType, "VPNMatrix");
            //}
        }
        protected override Dto.IDMSReportParam GetReportHeaderColumnMapping()
        {
            var param = new Dto.IDMSReportParam();
            param.ReportDataColumns = new List<Dto.IDMSReportDataColumn>();
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Id", "Id", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("VPN Type", "Type", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Name", "Name", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("ProductionADgroups", "ProductionADgroups", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("QAADGroups", "QAADGroups", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("DevelpomentADGroups", "DevelpomentADGroups", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("ADGroups", "ADGroups", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Level1 Approver", "Level1Approver", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Level1 Approver Type", "Level1ApproverType", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Level2 Approver", "Level2Approver", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Level2 Approver Type", "Level2ApproverType", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Level3 Approver", "Level3Approver", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Level3 Approver Type", "Level3ApproverType", null));
            if (rbInValid.Checked)
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
                if (invalid != null) inValidList = invalid.Select(item => (Dto.VPNMatrix)item).ToList();
                if (!(string.IsNullOrEmpty(errMsg)))
                    MasterPage.ShowAlert(AlertType.Error, errMsg);
                else
                {
                    rbInValid.Checked = true;
                    rbAll.Checked = false;
                    rbValid.Checked = false;
                    BindInValidGrid();
                    tblValidRecordDetails.Visible = true;
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
            grdInvalidEntries.DataBind();
        }

        #endregion

        #region Method
        public void delete()
        {
            if (!ContextObjectId.HasValue)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item.");
                return;
            }
            try
            {
                LogDeletedRecords<Dto.VPNMatrix>(ContextObjectId.Value, MatrixName);
                var dic = new Dictionary<string, string>() { { "Id", ContextObjectId.Value.ToString() } };
                var strmsg = DomainApiClient<Dto.VPNMatrix>.GetByAction<string>("DeleteVPNMatrixById", dic);
                
                reloadDatasource = true;
                ShowAlertMessage(AlertType.Success, strmsg);
                FillValidEntriesGrid();
                grdInvalidEntries.DataBind();
            }
            catch (Exception)
            {
                MasterPage.ShowAlert(AlertType.Error, "VPNMatrix cannot be deleted.");
            }
        }

        void upsert()
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
                oldData = GetSerializeString<Dto.VPNMatrix>(id);
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
            parameterProcess.Add("templateName", XLTemplates.VPNConfigurations.ToString());
            parameterProcess.Add("isEditMode", editMode.ToString());
            var datas = DomainApiClient<Dto.IXLData>.GetByAction<IList<DI.Chakra.Dto.IXLData>>(DomainApiAction.XLProcessor.XLData.ProcessXLData, parameterProcess, xldata);
            reloadDatasource = true;
            if (datas != null && datas.Count > 0)
            {
                string hasvalue = datas.First().ValidationMessage;
                if (string.IsNullOrWhiteSpace(hasvalue))
                {
                    UpdatedUILog<Dto.VPNMatrix>(id,MatrixName,data:oldData);
                    //Session["ADXLMessage"] = successMessage;
                    MasterPage.ShowAlert(AlertType.Success, successMessage);
                    grdInvalidEntries.MasterTableView.IsItemInserted = false;
                    grdInvalidEntries.MasterTableView.ClearEditItems();
                    grdInvalidEntries.EnableRowOnDemandVS = true;
                    FillValidEntriesGrid();
                    grdInvalidEntries.DataBind();
                    return;
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Error, hasvalue);
                    //e.Canceled = true;
                    return;
                }
            }
        }
        private void LoadEntitlementType(RadComboBox RadRequestType)
        {
            RadRequestType.Items.Clear();
            RadRequestType.Items.Add(new RadComboBoxItem("---Select---", "0"));
            RadRequestType.Items.Add(new RadComboBoxItem(VPNType.SSLVPNOnly.ToString(), ((int)VPNMatrixType.Application).ToString()));
            RadRequestType.Items.Add(new RadComboBoxItem(VPNType.VDI.ToString(), ((int)VPNMatrixType.Project).ToString()));
            RadRequestType.SelectedValue = ((int)this.RequestType).ToString();
        }
        private Dto.VPNMatrix PopulateData(GridEditableItem editableItem, long id, out string validationMsg)
        {
            var existingData = DomainApiClient<Dto.VPNMatrix>.GetById(id);
            Hashtable values = new Hashtable();
            editableItem.ExtractValues(values);
            var requestTypeDDL = editableItem.FindControl("RadRequestType") as RadComboBox;
            if (requestTypeDDL.SelectedIndex == 0)
            {
                validationMsg = $"Type Should be {VPNType.SSLVPNOnly.ToString()},{VPNType.VDI.ToString()}";
                return new Dto.VPNMatrix();
            }
            
            if (existingData != null)
            {
                existingData.VPNMatrixType = (VPNMatrixType)Convert.ToInt64(requestTypeDDL.SelectedValue);
                existingData.Name = (string)values["Name"];
                TextBox txtProductionADgroups = editableItem.FindControl("txtProductionADgroups") as TextBox;
                TextBox txtQAADGroups = editableItem.FindControl("txtQAADGroups") as TextBox;
                TextBox txtDevelpomentADGroups = editableItem.FindControl("txtDevelpomentADGroups") as TextBox;
                TextBox txtADGroups = editableItem.FindControl("txtADGroups") as TextBox;

                existingData.ProductionADgroups = txtProductionADgroups.Text;
                existingData.QAADGroups = txtQAADGroups.Text;
                existingData.DevelpomentADGroups = txtDevelpomentADGroups.Text;
                existingData.ADGroups = txtADGroups.Text;

                existingData.Level1Approver = XlHelper.FormatLeadingZeros(((string)values["Level1Approver"]), 8);
                existingData.Level2Approver = XlHelper.FormatLeadingZeros(((string)values["Level2Approver"]), 8);
                existingData.Level3Approver = XlHelper.FormatLeadingZeros(((string)values["Level3Approver"]), 8);
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
                    return new Dto.VPNMatrix();
                }
                if (IsValidApproverType(Approvertype2.Text))
                {
                    existingData.Level2ApproverType = GetApproverType(Approvertype2.Text);
                    validationMsg = "";
                }
                else
                {
                    validationMsg = InvalidApprovalTypeMatrixMessage;
                    return new Dto.VPNMatrix();
                }
                if (IsValidApproverType(Approvertype3.Text))
                {
                    existingData.Level3ApproverType = GetApproverType(Approvertype3.Text);
                    validationMsg = "";
                }
                else
                {
                    validationMsg = InvalidApprovalTypeMatrixMessage;
                    return new Dto.VPNMatrix();
                }
                return existingData;
            }
            else
            {
                Dto.VPNMatrix xldata = new Dto.VPNMatrix();
                xldata.VPNMatrixType = (VPNMatrixType)Convert.ToInt32(requestTypeDDL.SelectedValue);
                xldata.Name = (string)values["Name"];
                TextBox txtProductionADgroups = editableItem.FindControl("txtProductionADgroups") as TextBox;
                TextBox txtQAADGroups = editableItem.FindControl("txtQAADGroups") as TextBox;
                TextBox txtDevelpomentADGroups = editableItem.FindControl("txtDevelpomentADGroups") as TextBox;
                TextBox txtADGroups = editableItem.FindControl("txtADGroups") as TextBox;

                xldata.ProductionADgroups = txtProductionADgroups.Text;
                xldata.QAADGroups = txtQAADGroups.Text;
                xldata.DevelpomentADGroups = txtDevelpomentADGroups.Text;
                xldata.ADGroups = txtADGroups.Text;

                xldata.Level1Approver = XlHelper.FormatLeadingZeros(((string)values["Level1Approver"]), 8);
                xldata.Level2Approver = XlHelper.FormatLeadingZeros(((string)values["Level2Approver"]), 8);
                xldata.Level3Approver = XlHelper.FormatLeadingZeros(((string)values["Level3Approver"]), 8);
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
                    return new Dto.VPNMatrix();
                }
                if (IsValidApproverType(Approvertype2.Text))
                {
                    xldata.Level2ApproverType = GetApproverType(Approvertype2.Text);
                    validationMsg = "";
                }
                else
                {
                    validationMsg = InvalidApprovalTypeMatrixMessage;
                    return new Dto.VPNMatrix();
                }
                if (IsValidApproverType(Approvertype3.Text))
                {
                    xldata.Level3ApproverType = GetApproverType(Approvertype3.Text);
                    validationMsg = "";
                }
                else
                {
                    validationMsg = InvalidApprovalTypeMatrixMessage;
                    return new Dto.VPNMatrix();
                }
                return xldata;
            }
        }
        #endregion
        protected void grdInvalidEntries_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                var radCombo = item.FindControl("RadRequestType") as RadComboBox;
                if (radCombo != null)
                {
                    radCombo.Attributes.Add("style", "width:100% !important");
                    LoadEntitlementType(radCombo);
                }
                ChangeEditButton(item, false);
            }
            if (e.Item is GridEditableItem && e.Item.IsInEditMode)
            {
                var dataItem = e.Item as GridEditableItem;
                var mData = dataItem.DataItem as Dto.VPNMatrix;
                var txtProductionADgroups = dataItem.FindControl("txtProductionADgroups") as TextBox;
                var txtQAADGroups = dataItem.FindControl("txtQAADGroups") as TextBox;
                var txtDevelpomentADGroups = dataItem.FindControl("txtDevelpomentADGroups") as TextBox;
                var txtADGroups = dataItem.FindControl("txtADGroups") as TextBox;
                var RadRequestType = dataItem.FindControl("RadRequestType") as RadComboBox;
                if (RadRequestType != null && RadRequestType.SelectedIndex > 0)
                {
                    var type = (VPNMatrixType)Convert.ToInt64(RadRequestType.SelectedValue);
                    if (type == VPNMatrixType.Application)
                    {
                        txtProductionADgroups.Visible = true;
                        txtQAADGroups.Visible = true;
                        txtDevelpomentADGroups.Visible = true;
                        txtADGroups.Visible = false;
                    }
                    else if (type == VPNMatrixType.Project)
                    {
                        txtProductionADgroups.Visible = false;
                        txtQAADGroups.Visible = false;
                        txtDevelpomentADGroups.Visible = false;
                        txtADGroups.Visible = true;
                    }
                    else
                    {
                        txtProductionADgroups.Visible = false;
                        txtQAADGroups.Visible = false;
                        txtDevelpomentADGroups.Visible = false;
                        txtADGroups.Visible = false;
                    }
                }
            }
            if (e.Item is GridFilteringItem)
            {
                GridFilteringItem Item = (GridFilteringItem)e.Item;
                RadComboBox combo = (RadComboBox)Item["MatrixType"].FindControl("MatrixType");
                string[] data = new string[3];
                data[0] = "All";
                //var data1 = Enum.GetValues(typeof(VPNType));
                data[1] = "SSLVPNOnly";
                data[2] = "VDI";
                combo.DataSource = data;
                combo.DataBind();
            }
        }
        
        void ChangeEditButton(GridDataItem item, bool visible)
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
            reloadDatasource = true;
            FillValidEntriesGrid();
            grdInvalidEntries.DataBind();
        }
        private void FillInvalidEntriesGrid()
        {
            grdHeader.InnerText = "InValid Records ";
            grdInvalidEntries.DataSource = inValidList;
            grdInvalidEntries.MasterTableView.Columns.FindByUniqueName("ValidationMessage").Display = true;
            grdInvalidEntries.MasterTableView.Columns.FindByUniqueName("Edit").Display = false;
            grdInvalidEntries.MasterTableView.CommandItemSettings.ShowAddNewRecordButton = false;
            grdInvalidEntries.MasterTableView.CommandItemSettings.ShowExportToCsvButton = false;
            grdInvalidEntries.MasterTableView.CommandItemSettings.ShowExportToExcelButton = false;
        }
        private void FillValidEntriesGrid()
        {
            bool isValidRecords = rbValid.Checked;
            bool isAllRecords = rbAll.Checked;

            if (rbInValid.Checked == true)
                FillInvalidEntriesGrid();
            else
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("isValidRecords", isValidRecords.ToString());
                parameters.Add("isAllRecords", isAllRecords.ToString());
                List<Dto.VPNMatrix> validList = GetDatasource(parameters);

                if (rbValid.Checked == true)
                    grdHeader.InnerText = "Valid Records ";
                else if (rbAll.Checked == true)
                    grdHeader.InnerText = "All Records ";
                grdInvalidEntries.DataSource = validList;
                grdInvalidEntries.MasterTableView.Columns.FindByUniqueName("ValidationMessage").Display = false;
                grdInvalidEntries.MasterTableView.Columns.FindByUniqueName("Edit").Display = true;
                grdInvalidEntries.MasterTableView.CommandItemSettings.ShowAddNewRecordButton = true;
                grdInvalidEntries.MasterTableView.CommandItemSettings.ShowExportToCsvButton = true;
                grdInvalidEntries.MasterTableView.CommandItemSettings.ShowExportToExcelButton = false;
            }
        }
        private List<Dto.VPNMatrix> GetDatasource(Dictionary<string, string> parameters)
        {
            if (ViewState[XlHelper.xldatasource] == null || reloadDatasource)
            {
                List<Dto.VPNMatrix> validList = DomainApiClient<Dto.VPNMatrix>.GetByAction<List<Dto.VPNMatrix>>(DomainApiAction.Matrix.VPNMatrix.GetAllVPNMatrix, parameters);
                ViewState[XlHelper.xldatasource] = validList;
                reloadDatasource = false;
            }
            return ViewState[XlHelper.xldatasource] as List<Dto.VPNMatrix>;
        }

        protected void RadRequestType_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var RadRequestType = sender as RadComboBox;
            GridDataItem dataItem = (GridDataItem)RadRequestType.NamingContainer;
            if(dataItem != null)
            {
                var txtProductionADgroups = dataItem.FindControl("txtProductionADgroups") as TextBox;
                var txtQAADGroups = dataItem.FindControl("txtQAADGroups") as TextBox;
                var txtDevelpomentADGroups = dataItem.FindControl("txtDevelpomentADGroups") as TextBox;
                var txtADGroups = dataItem.FindControl("txtADGroups") as TextBox;
                if(RadRequestType.SelectedIndex > 0)
                {
                    var type =(VPNMatrixType)Convert.ToInt64(RadRequestType.SelectedValue);
                    if(type == VPNMatrixType.Application)
                    {
                        txtProductionADgroups.Visible = true;
                        txtQAADGroups.Visible = true;
                        txtDevelpomentADGroups.Visible = true;
                        txtADGroups.Visible = false;
                    }
                    else if(type == VPNMatrixType.Project)
                    {
                        txtProductionADgroups.Visible = false;
                        txtQAADGroups.Visible = false;
                        txtDevelpomentADGroups.Visible = false;
                        txtADGroups.Visible = true;
                    }
                    else
                    {
                        txtProductionADgroups.Visible = false;
                        txtQAADGroups.Visible = false;
                        txtDevelpomentADGroups.Visible = false;
                        txtADGroups.Visible = false;
                    }
                }
            }
           
        }

        protected void grdInvalidEntries_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditableItem && e.Item.IsInEditMode)
            {
                
            }
        }
        protected void VpnTypeList_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            RadComboBox combo = sender as RadComboBox;
            ViewState[combo.ID] = combo.SelectedValue;
            string ColumnName = combo.ID;
            if (combo.SelectedValue.ToString() !="All")
            {
                //string ColumnName = "MatrixType";
                string filterExpression = String.Format(@"(iif({0} == null, """", {1}).ToString() = (""{2}""))", ColumnName, ColumnName, e.Text);
                //string filterExpression = "([" + "MatrixType" + "] = '" + e.Text + "')";
                grdInvalidEntries.MasterTableView.FilterExpression = filterExpression;
            }
            else
            {
                grdInvalidEntries.MasterTableView.FilterExpression = string.Empty;
            }
            //BindValidEntriesGrid();
            grdInvalidEntries.Rebind();
        }
        protected void VPNType_PreRender(object sender, EventArgs e)
        {
            {
                RadComboBox rcbFilter = sender as RadComboBox;
                if (ViewState[rcbFilter.ID] != null)
                {
                    rcbFilter.SelectedValue = ViewState[rcbFilter.ID].ToString();
                }
            }
        }
        //protected void OnLoadHandler(object sender, EventArgs e)
        //{
        //    if (!IsPostBack)
        //    {

        //        RadComboBox combo = (RadComboBox)sender;
        //        combo.Items.Clear();
        //        combo.Items.Add(new RadComboBoxItem("All", "0"));
        //        RadComboBoxItem item = new RadComboBoxItem();
        //        item.Value =((int)VPNType.SSLVPNOnly).ToString();
        //        item.Text = (VPNType.SSLVPNOnly).ToString();
        //        combo.Items.Add(item);
        //        RadComboBoxItem item2 = new RadComboBoxItem();
        //        item2.Value = ((int)VPNType.VDI).ToString();
        //        item2.Text = (VPNType.VDI).ToString();
        //        combo.Items.Add(item2);
        //    }
        //}

        
    }
}