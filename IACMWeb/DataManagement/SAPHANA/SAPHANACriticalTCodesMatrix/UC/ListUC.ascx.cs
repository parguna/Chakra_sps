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

namespace DI.Chakra.Web.DataManagement.SAPHANACriticalTCodesMatrix.UC
{
    public partial class List : DI.Chakra.Web.UIBaseUC, IAccessControl
    {
        const string MatrixName = "EBS CriticalTCodesMatrix";
        static GridEditableItem selectedDataItem = null;
        private static IList<Dto.SAPHANAXLCriticalTCodeMatrix> inValidList;

        #region Members
        public static bool flag = false;
        #endregion

        #region Properties
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
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.MiddleCell.Text = "Provisioning SAP EBS Critical TCodes Matrix  ";
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.MiddleCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.LeftCell.Text = PDFFileLeftCell;
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.LeftCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(Session["ADXLMessage"])))
            {
                MasterPage.ShowAlert(AlertType.Success, Convert.ToString(Session["ADXLMessage"]));
                Session["ADXLMessage"] = string.Empty;
            }
            grdInvalidEntries.EnableLinqExpressions = true;
            XLUploadUC1.TemplateName = XLTemplates.SAPHANA_CriticalTCodesMatrix;

            this.MasterPage.ActionClicked += ActionClicked;

            if (!Page.IsPostBack)
            {
                inValidList = new List<Dto.SAPHANAXLCriticalTCodeMatrix>();
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
                var data = grdInvalidEntries.DataSource as List<Dto.SAPHANAXLCriticalTCodeMatrix>;
                if (data != null && data.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(grdInvalidEntries.MasterTableView.FilterExpression))
                        data = data.AsEnumerable().AsQueryable().Where(grdInvalidEntries.MasterTableView.FilterExpression).ToList();
                    if (grdInvalidEntries.MasterTableView.SortExpressions != null && grdInvalidEntries.MasterTableView.SortExpressions.Count > 0)
                    {
                        data = data.AsEnumerable().AsQueryable().OrderBy(GetSortExpression(grdInvalidEntries.MasterTableView.SortExpressions)).ToList();
                    }
                    GenericExport(data, e.ActionType, "SAPHANAXLCriticalTCodeMatrix");
                }
            }

        }
        protected override Dto.IDMSReportParam GetReportHeaderColumnMapping()
        {
            var param = new Dto.IDMSReportParam();
            param.ReportDataColumns = new List<Dto.IDMSReportDataColumn>();
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Application", "Application", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Transaction", "Transaction", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Transaction Description", "TransactionDescription", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Transaction Owner", "TransactionOwner", null));

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
                if (invalid != null) inValidList = invalid.Select(item => (Dto.SAPHANAXLCriticalTCodeMatrix)item).ToList();
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
                LogDeletedRecords<Dto.SAPHANAXLCriticalTCodeMatrix>(ContextObjectId.Value,MatrixName);
                DomainApiClient<Dto.SAPHANAXLCriticalTCodeMatrix>.DeleteById(ContextObjectId.Value);
                
                ShowAlertMessage(AlertType.Success, "Record deleted successfully.");
                FillValidEntriesGrid();
                grdInvalidEntries.DataBind();
            }
            catch (Exception)
            {
                MasterPage.ShowAlert(AlertType.Error, "SAPHANAXLCriticalTCodeMatrix cannot be deleted.");
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

            if (id > 0)
            {
                editMode = true;
                GridEditableItem item = grdInvalidEntries.EditItems[0] as GridEditableItem;
                xldata = PopulateData(item, id);
                successMessage = "Record updated successfully.";
                oldData = GetSerializeString<Dto.SAPHANAXLCriticalTCodeMatrix>(id);
            }
            else
            {
                editMode = false;
                editableItem = grdInvalidEntries.MasterTableView.GetInsertItem();
                xldata = PopulateData(editableItem, id);
                successMessage = "Record inserted successfully.";
            }
            var data = xldata as Dto.SAPHANAXLCriticalTCodeMatrix;
            if (data != null)
            {
                if (string.IsNullOrWhiteSpace(data.Application))
                {
                    MasterPage.ShowAlert(AlertType.Error, "Please enter Application");
                    return;
                }
                var param = new Dictionary<string, string>();
                param.Add("applicationCode", data.Application);
                var ep = DomainApiClient<Dto.EndPoint>.GetByAction<Dto.EndPoint>("GetByApplicationCode", param);
                if (ep == null || (ep.ApplicationType != ApplicationType.EBS && ep.ApplicationType != ApplicationType.MSSL))
                {
                    MasterPage.ShowAlert(AlertType.Error, "invalid Application");
                    return;
                }
                if (id == 0)
                {
                    data.EndpointSystemId = ep.Id;
                }
            }
            if (xldata != null)
            {
                var isExists = DomainApiClient<Dto.SAPHANAXLCriticalTCodeMatrix>.GetByAction<bool>("IsRecordExists", null, xldata);
                if (isExists)
                {
                    MasterPage.ShowAlert(AlertType.Error, "Record cannot be duplicated");
                    return;
                }
            }
            Dictionary<string, string> parameterProcess = new Dictionary<string, string>();
            parameterProcess.Add("templateName", XLTemplates.SAPHANA_CriticalTCodesMatrix.ToString());
            parameterProcess.Add("isEditMode", editMode.ToString());
            var datas = DomainApiClient<Dto.IXLData>.GetByAction<IList<DI.Chakra.Dto.IXLData>>(DomainApiAction.XLProcessor.XLData.ProcessXLData, parameterProcess, xldata);

            if (datas != null && datas.Count > 0)
            {
                string hasvalue = datas.First().ValidationMessage;
                if (string.IsNullOrWhiteSpace(hasvalue))
                {
                    UpdatedUILog<Dto.SAPHANAXLCriticalTCodeMatrix>(id, MatrixName, data: oldData);
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
        private Dto.SAPHANAXLCriticalTCodeMatrix PopulateData(GridEditableItem editableItem, long id)
        {
            var existingData = DomainApiClient<Dto.SAPHANAXLCriticalTCodeMatrix>.GetById(id);
            Hashtable values = new Hashtable();
            editableItem.ExtractValues(values);
            if (existingData != null)
            {
                existingData.Application = ((string)values["Application"])?.Trim();
                existingData.Transaction = ((string)values["Transaction"])?.Trim();
                existingData.TransactionDescription = ((string)values["TransactionDescription"])?.Trim();
                existingData.TransactionOwner = XlHelper.FormatLeadingZeros((((string)values["TransactionOwner"]).Trim()), 8);
                return existingData;
            }
            else
            {
                Dto.SAPHANAXLCriticalTCodeMatrix xldata = new Dto.SAPHANAXLCriticalTCodeMatrix();
                xldata.Application = ((string)values["Application"])?.Trim();
                xldata.Transaction = ((string)values["Transaction"])?.Trim();
                xldata.TransactionDescription = ((string)values["TransactionDescription"])?.Trim();
                xldata.TransactionOwner = XlHelper.FormatLeadingZeros((((string)values["TransactionOwner"])?.Trim()), 8);
                return xldata;
            }
        }
        #endregion
        protected void grdInvalidEntries_PreRender(object sender, EventArgs e)
        {
            if (!flag)
            {
                foreach (GridBoundColumn col in grdInvalidEntries.Columns)
                {
                    //col.AllowFiltering = rbValid.Checked;
                }
                grdInvalidEntries.Rebind();
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
                List<Dto.SAPHANAXLCriticalTCodeMatrix> validList= DomainApiClient<Dto.SAPHANAXLCriticalTCodeMatrix>.GetByAction<List<Dto.SAPHANAXLCriticalTCodeMatrix>>(DomainApiAction.Matrix.SAPHANA.GetAllSAPHANAXLCriticalTCodesMatrix, parameters);

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
    }
}