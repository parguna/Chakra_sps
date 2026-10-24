using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.DataManagement.SkypeForBusiness.UC
{
    public partial class ListUC : DI.Chakra.Web.UIBaseUC, IAccessControl
    {
        const string MatrixName = "ProvisioningMatrix";
        static GridEditableItem selectedDataItem = null;
        private static IList<Dto.SkypeForBusinessProvisioningMatrix> inValidList;
        static bool reloadDatasource = false;
        #region Members
        #endregion

        #region Properties
        private XLDataConverterHelper xlHelper;
        public EntitlementType RequestType
        {
            get
            {
                if (ViewState["RequestType"] == null)
                    return EntitlementType.None;
                return ((EntitlementType)ViewState["RequestType"]);
            }
            set
            {
                ViewState["RequestType"] = value;
            }
        }
        public long EndpointSystemId
        {
            get
            {
                if (ViewState["EndpointSystemId"] == null)
                    return 0;
                return Convert.ToInt64(ViewState["EndpointSystemId"].ToString());
            }
            set
            {
                ViewState["EndpointSystemId"] = value;
            }
        }
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
            
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.MiddleCell.Text = "SkypeForBusiness Provisioning Matrix";
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.MiddleCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.LeftCell.Text = PDFFileLeftCell;
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.LeftCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(Session["ADXLMessage"])))
            {
                MasterPage.ShowAlert(AlertType.Success, Convert.ToString(Session["ADXLMessage"]));
                Session["ADXLMessage"] = string.Empty;
            }
            grdInvalidEntries.EnableLinqExpressions = true;
            XLUploadUC1.TemplateName = XLTemplates.SkypeForBusinessConfigurations;

            this.MasterPage.ActionClicked += ActionClicked;

            if (!Page.IsPostBack)
            {
                rbAll.Visible = true;
                var param = new Dictionary<string, string>();
                param.Add("applicationType",ApplicationType.MicrosoftTeams.ToString());
                var endPoint = DomainApiClient<Dto.EndPoint>.GetByAction<Dto.EndPoint>(DomainApiAction.Endpoint.EndPoint.GetEndPointByApplicationType, param);
                this.EndpointSystemId = endPoint.Id;
                inValidList = new List<Dto.SkypeForBusinessProvisioningMatrix>();
                tblValidRecordDetails.Visible = true;
                rbInValid.Checked = false;
                rbAll.Checked = false;
                rbValid.Checked = true;
                BindValidEntriesGrid();
                lblDetails.Text = XlHelper.GetWhoUpdatedData(XLUploadUC1.TemplateName);
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
            RequestType = (EntitlementType)Grid.GetDataKeyValue("RequestType");

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
                    Update();
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
            //    grdInvalidEntries.ExportSettings.FileName = "SkypeForBusinessProvisioningMatrix";
            //    ExportNew(grdInvalidEntries, e.ActionType);
            //}
            //else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            //{
            //    grdInvalidEntries.ExportSettings.FileName = "SkypeForBusinessProvisioningMatrix";
            //    ExportNew(grdInvalidEntries, e.ActionType);
            //}
            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                var data = grdInvalidEntries.DataSource as List<Dto.SkypeForBusinessProvisioningMatrix>;
                if (data != null && data.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(grdInvalidEntries.MasterTableView.FilterExpression))
                        data = data.AsEnumerable().AsQueryable().Where(grdInvalidEntries.MasterTableView.FilterExpression).ToList();
                    if (grdInvalidEntries.MasterTableView.SortExpressions != null && grdInvalidEntries.MasterTableView.SortExpressions.Count > 0)
                    {
                        data = data.AsEnumerable().AsQueryable().OrderBy(GetSortExpression(grdInvalidEntries.MasterTableView.SortExpressions)).ToList();
                    }
                    GenericExport(data, e.ActionType, "SkypeForBusinessProvisioningMatrix");
                }
            }
        }
        protected override Dto.IDMSReportParam GetReportHeaderColumnMapping()
        {
            var param = new Dto.IDMSReportParam();
            param.ReportDataColumns = new List<Dto.IDMSReportDataColumn>();
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("RequestType", "RequestType", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Name", "Name", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Description", "Description", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("UPN(ADID)", "ADID", null));
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
        private void LoadEntitlementType(RadComboBox RadRequestType)
        {
            RadRequestType.Items.Clear();
            RadRequestType.Items.Add(new RadComboBoxItem("---Select---", "0"));
            RadRequestType.Items.Add(new RadComboBoxItem(EntitlementType.Default.ToString(), ((int)EntitlementType.Default).ToString()));
            RadRequestType.Items.Add(new RadComboBoxItem(EntitlementType.ResponseGroup.ToString(), ((int)EntitlementType.ResponseGroup).ToString()));
            RadRequestType.Items.Add(new RadComboBoxItem(EntitlementType.DCC.ToString(), ((int)EntitlementType.DCC).ToString()));
            RadRequestType.Items.Add(new RadComboBoxItem(EntitlementType.DCCS.ToString(), ((int)EntitlementType.DCCS).ToString()));
            RadRequestType.Items.Add(new RadComboBoxItem(EntitlementType.IDD.ToString(), ((int)EntitlementType.IDD).ToString()));
            RadRequestType.Items.Add(new RadComboBoxItem(EntitlementType.Move.ToString(), ((int)EntitlementType.Move).ToString()));
            if (this.RequestType != EntitlementType.None)
            {
                RadRequestType.SelectedValue = ((int)this.RequestType).ToString();
            }
        }
        protected void XLUploadUC1_UserControlButtonClicked(object sender, EventArgs e)
        {
            try
            {
                inValidList = null;
                IList<Dto.IXLData> invalid = XLUploadUC1.InvalidList;
                string errMsg = XLUploadUC1.ErrorMessage;
                if (invalid != null) inValidList = invalid.Select(item => (Dto.SkypeForBusinessProvisioningMatrix)item).ToList();
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
                LogDeletedRecords<Dto.SkypeForBusinessProvisioningMatrix>(ContextObjectId.Value,MatrixName);
                DomainApiClient<Dto.SkypeForBusinessProvisioningMatrix>.DeleteById(ContextObjectId.Value);
                
                ShowAlertMessage(AlertType.Success, "Record deleted successfully.");
                reloadDatasource = true;
                FillValidEntriesGrid();
                grdInvalidEntries.DataBind();
            }
            catch (Exception)
            {
                MasterPage.ShowAlert(AlertType.Error, "SkypeForBusinessProvisioningMatrix cannot be deleted.");
            }
        }

        void Update()
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
            parameterProcess.Add("templateName", XLTemplates.SkypeForBusinessConfigurations.ToString());
            parameterProcess.Add("isEditMode", editMode.ToString());
            var datas = DomainApiClient<Dto.IXLData>.GetByAction<IList<DI.Chakra.Dto.IXLData>>(DomainApiAction.XLProcessor.XLData.ProcessXLData, parameterProcess, xldata);
            reloadDatasource = true;
            if (datas != null && datas.Count > 0)
            {
                this.RequestType = EntitlementType.None;
                string hasvalue = datas.First().ValidationMessage;
                if (string.IsNullOrWhiteSpace(hasvalue))
                {
                    UpdatedUILog<Dto.SkypeForBusinessProvisioningMatrix>(id,MatrixName, data:oldData);
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
        private Dto.SkypeForBusinessProvisioningMatrix PopulateData(GridEditableItem editableItem, long id, out string validationMsg)
        {
            var existingData = DomainApiClient<Dto.SkypeForBusinessProvisioningMatrix>.GetById(id);
            Hashtable values = new Hashtable();
            editableItem.ExtractValues(values);
            
            if (existingData != null)
            {
                var requestTypeDDL = editableItem.FindControl("RadRequestType") as RadComboBox;
                if (requestTypeDDL.SelectedIndex == 0)
                {
                    validationMsg = $"RequestType Should be {EntitlementType.ResponseGroup.ToString()},{EntitlementType.Default.ToString()},{EntitlementType.DCC.ToString()},{EntitlementType.DCCS.ToString()},{EntitlementType.IDD.ToString()},{EntitlementType.Move.ToString()}";
                    return new Dto.SkypeForBusinessProvisioningMatrix();
                }
                else
                {
                    existingData.RequestType = (EntitlementType)Convert.ToInt64(requestTypeDDL.SelectedValue);
                }
                existingData.EndPointSystemId = this.EndpointSystemId;
                existingData.Name = (string)values["Name"];
                existingData.Description = (string)values["Description"];
                existingData.ADID = (string)values["ADID"];
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
                    return new Dto.SkypeForBusinessProvisioningMatrix();
                }
                if (IsValidApproverType(Approvertype2.Text))
                {
                    existingData.Level2ApproverType = GetApproverType(Approvertype2.Text);
                    validationMsg = "";
                }
                else
                {
                    validationMsg = InvalidApprovalTypeMatrixMessage;
                    return new Dto.SkypeForBusinessProvisioningMatrix();
                }
                if (IsValidApproverType(Approvertype3.Text))
                {
                    existingData.Level3ApproverType = GetApproverType(Approvertype3.Text);
                    validationMsg = "";
                }
                else
                {
                    validationMsg = InvalidApprovalTypeMatrixMessage;
                    return new Dto.SkypeForBusinessProvisioningMatrix();
                }
                return existingData;
            }
            else
            {
                Dto.SkypeForBusinessProvisioningMatrix xldata = new Dto.SkypeForBusinessProvisioningMatrix();
                var requestTypeDDL = editableItem.FindControl("RadRequestType") as RadComboBox;
                if (requestTypeDDL.SelectedIndex == 0)
                {
                    validationMsg = $"RequestType Should be {EntitlementType.ResponseGroup.ToString()},{EntitlementType.ResponseGroup.ToString()},{EntitlementType.Default.ToString()},{EntitlementType.DCC.ToString()},{EntitlementType.DCCS.ToString()},{EntitlementType.IDD.ToString()},{EntitlementType.Move.ToString()}";
                    return new Dto.SkypeForBusinessProvisioningMatrix();
                }
                else
                {
                    xldata.RequestType = (EntitlementType)Convert.ToInt64(requestTypeDDL.SelectedValue);
                }
                xldata.EndPointSystemId = this.EndpointSystemId;
                xldata.Name = (string)values["Name"];
                xldata.Description = (string)values["Description"];
                xldata.ADID = (string)values["ADID"];
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
                    return new Dto.SkypeForBusinessProvisioningMatrix();
                }
                if (IsValidApproverType(Approvertype2.Text))
                {
                    xldata.Level2ApproverType = GetApproverType(Approvertype2.Text);
                    validationMsg = "";
                }
                else
                {
                    validationMsg = InvalidApprovalTypeMatrixMessage;
                    return new Dto.SkypeForBusinessProvisioningMatrix();
                }
                if (IsValidApproverType(Approvertype3.Text))
                {
                    xldata.Level3ApproverType = GetApproverType(Approvertype3.Text);
                    validationMsg = "";
                }
                else
                {
                    validationMsg = InvalidApprovalTypeMatrixMessage;
                    return new Dto.SkypeForBusinessProvisioningMatrix();
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
                List<Dto.SkypeForBusinessProvisioningMatrix> validList = GetDatasource(parameters);

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
        private List<Dto.SkypeForBusinessProvisioningMatrix> GetDatasource(Dictionary<string, string> parameters)
        {
            if (ViewState[XlHelper.xldatasource] == null || reloadDatasource)
            {
                List<Dto.SkypeForBusinessProvisioningMatrix> validList = DomainApiClient<Dto.SkypeForBusinessProvisioningMatrix>.GetByAction<List<Dto.SkypeForBusinessProvisioningMatrix>>(DomainApiAction.Matrix.SkypeForBusinessMatrix.GetAllSkypeForBusinessMatrix, parameters);
                ViewState[XlHelper.xldatasource] = validList;
                reloadDatasource = false;
            }
            return ViewState[XlHelper.xldatasource] as List<Dto.SkypeForBusinessProvisioningMatrix>;
        }

        protected void RadRequestType_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            
        }
    }
}