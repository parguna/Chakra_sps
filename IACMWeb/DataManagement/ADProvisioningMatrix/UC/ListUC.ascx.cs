using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.DataManagement.ADProvisioningMatrix.UC
{
    public partial class List : DI.Chakra.Web.UIBaseUC, IAccessControl
    {
        string MatrixName = "ADProvisioningMatrix";
        static WebClient client = null;
        static GridEditableItem selectedDataItem = null;
        private static IList<Dto.ADSXLProvisioningMatrix> inValidList;

        #region Members

        public static bool flag = false;
        private const string V_FILTEREXPRESSION = "V_FILTEREXPRESSION";

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

        public string XLTemplateName { get; set; }
        
        #endregion

        #region Properties
        public string FilterExpression
        {
            get
            {
                if (ViewState[V_FILTEREXPRESSION] == null)
                    return string.Empty;
                return (string)ViewState[V_FILTEREXPRESSION];
            }
            set
            {
                ViewState[V_FILTEREXPRESSION] = value;
            }
        }

        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            rbAll.Visible = false;
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.MiddleCell.Text = "Provisioning AD Configuration Management   ";
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.MiddleCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.LeftCell.Text = PDFFileLeftCell;
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.LeftCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(Session["ADXLMessage"])))
            {
                MasterPage.ShowAlert(AlertType.Success, Convert.ToString(Session["ADXLMessage"]));
                Session["ADXLMessage"] = string.Empty;
            }
            grdInvalidEntries.EnableLinqExpressions = true;
            XLUploadUC1.TemplateName = XLTemplates.ADS_PROVISIONINGMATRIX;

            //string strURL = Request.Url.GetLeftPart(UriPartial.Authority) + "/Files/CSV/DGM/WinAccessMatrix.csv";
            //this.btnExportToExcel.NavigateUrl = strURL;
            this.MasterPage.ActionClicked += ActionClicked;

            //lblDownloaded.Visible = false;
            if (!Page.IsPostBack)
            {
                inValidList = new List<Dto.ADSXLProvisioningMatrix>();
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
                    //selectedDataItem["Edit"].Controls[0].Visible = true;
                    //ChangeEditButton(grdInvalidEntries.SelectedItems[0] as GridDataItem, true);
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
                var data = grdInvalidEntries.DataSource as List<Dto.ADSXLProvisioningMatrix>;
                if (!string.IsNullOrWhiteSpace(grdInvalidEntries.MasterTableView.FilterExpression))
                    data = data.AsEnumerable().AsQueryable().Where(grdInvalidEntries.MasterTableView.FilterExpression).ToList();
                if (grdInvalidEntries.MasterTableView.SortExpressions != null && grdInvalidEntries.MasterTableView.SortExpressions.Count > 0)
                {
                    data = data.AsEnumerable().AsQueryable().OrderBy(GetSortExpression(grdInvalidEntries.MasterTableView.SortExpressions)).ToList();
                }
                GenericExport(data, e.ActionType, "ADProvisioningMatrix");
            }
            //else if ()
            //{
            //    grdInvalidEntries.ExportSettings.FileName = "ADProvisioningMatrix";
            //    //ExportNew(grdInvalidEntries, e.ActionType);
            //    ExportNew(grdInvalidEntries, e.ActionType, "ADProvisioningMatrix");
            //}
        }
        protected override IDMSReportParam GetReportHeaderColumnMapping()
        {
            var param = new Dto.IDMSReportParam();
            param.ReportDataColumns = new List<Dto.IDMSReportDataColumn>();
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Company Code", "CompanyCode", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Department Code", "DepartmentCode", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Branch Code", "DivisionCode", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Organisation Code", "OrganisationId", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Organisation Name", "OrganisationName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("OU", "OU", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Default Groups", "DefaultGroups", null));
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
                if (invalid != null) inValidList = invalid.Select(item => (Dto.ADSXLProvisioningMatrix)item).ToList();
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

        private void FillDataSource()
        {
            if (!string.IsNullOrEmpty(FilterExpression))
                grdInvalidEntries.MasterTableView.FilterExpression = FilterExpression;
            inValidList = DomainApiClient<Dto.ADSXLProvisioningMatrix>.GetAll();
            if (inValidList == null)
                grdInvalidEntries.DataSource = new List<Dto.ADSXLProvisioningMatrix>();
            else
                grdInvalidEntries.DataSource = inValidList;
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
                LogDeletedRecords<Dto.ADSXLProvisioningMatrix>(ContextObjectId.Value, MatrixName);
                DomainApiClient<Dto.ADSXLProvisioningMatrix>.DeleteById(ContextObjectId.Value);
                MasterPage.ShowAlert(AlertType.Success, "Record deleted successfully.");
                FillValidEntriesGrid();
                grdInvalidEntries.DataBind();
                //Page.Response.Redirect(Page.Request.Url.ToString(), true);
            }
            catch (Exception)
            {
                MasterPage.ShowAlert(AlertType.Error, "ADProvisioningMatrix cannot be Deleted");
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
                item.ExtractValues(values);
                xldata = PopulateData(values, id);
                successMessage = "Record updated successfully.";
                oldData = GetSerializeString<Dto.ADSXLProvisioningMatrix>(id);
            }
            else
            {
                editMode = false;
                editableItem = grdInvalidEntries.MasterTableView.GetInsertItem();
                editableItem.ExtractValues(values);
                xldata = PopulateData(values, id);
                successMessage = "Record inserted successfully.";
            }

            Dictionary<string, string> parameterProcess = new Dictionary<string, string>();
            parameterProcess.Add("templateName", XLTemplates.ADS_PROVISIONINGMATRIX.ToString());
            parameterProcess.Add("isEditMode", editMode.ToString());
            var datas = DomainApiClient<Dto.IXLData>.GetByAction<IList<DI.Chakra.Dto.IXLData>>(DomainApiAction.XLProcessor.XLData.ProcessXLData, parameterProcess, xldata);

            if (datas != null && datas.Count > 0)
            {
                string hasvalue = datas.First().ValidationMessage;
                if (string.IsNullOrWhiteSpace(hasvalue))
                {
                    UpdatedUILog<Dto.ADSXLProvisioningMatrix>(id,this.MatrixName,data: oldData);
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
        Dto.IXLData PopulateData(Hashtable values, long id)
        {
            var existingData = DomainApiClient<Dto.ADSXLProvisioningMatrix>.GetById(id);
            if (existingData != null && existingData.Id > 0)
            {
                existingData.CompanyCode = (string)values["CompanyCode"];
                existingData.CostCenterCode = (string)values["CostCenterCode"];
                existingData.CostDistributionCode = (string)values["CostDistributionCode"];
                existingData.DefaultGroups = (string)values["DefaultGroups"];
                existingData.DepartmentCode = (string)values["DepartmentCode"];
                existingData.DivisionCode = XlHelper.FormatLeadingZeros(((string)values["DivisionCode"]), 4);
                existingData.OrganisationId = XlHelper.FormatLeadingZeros(((string)values["OrganisationId"]), 8);
                existingData.OrganisationName = (string)values["OrganisationName"];
                existingData.OU = (string)values["OU"];
                return existingData;
            }
            else
            {
                Dto.ADSXLProvisioningMatrix xldata = new Dto.ADSXLProvisioningMatrix();
                xldata.CompanyCode = (string)values["CompanyCode"];
                xldata.CostCenterCode = (string)values["CostCenterCode"];
                xldata.CostDistributionCode = (string)values["CostDistributionCode"];
                xldata.DefaultGroups = (string)values["DefaultGroups"];
                xldata.DepartmentCode = (string)values["DepartmentCode"];
                xldata.DivisionCode = XlHelper.FormatLeadingZeros(((string)values["DivisionCode"]), 4);
                xldata.OrganisationId = XlHelper.FormatLeadingZeros(((string)values["OrganisationId"]), 8);
                xldata.OrganisationName = (string)values["OrganisationName"];
                xldata.OU = (string)values["OU"];
                return xldata;
            }
        }
        protected void DownloadFile(object sender, EventArgs e)
        {

            string strURL = Request.Url.GetLeftPart(UriPartial.Authority) + "/Files/CSV/Endpoint/INHgrdInvalidEntries.csv";

        }

        public static void DownloadFile(string urlAddress)
        {
            client = new WebClient();
            string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string pathDownload = Path.Combine(pathUser, "Downloads") + "test.csv";
            client.DownloadFile(urlAddress, @"" + pathDownload);

        }

        protected void btnExportToExcel_Click(object sender, EventArgs e)
        {

            string strFileName = "INHgrdInvalidEntries.csv";
            string path = Server.MapPath("~/Files//CSV//Endpoint//" + strFileName);
            if (File.Exists(path))

            {
                byte[] bts = System.IO.File.ReadAllBytes(path);
                MemoryStream mstream = new MemoryStream(bts);
                byte[] byteArray = mstream.ToArray();
                mstream.Flush();
                mstream.Close();
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=foo.csv");
                Response.AddHeader("Content-Length", byteArray.Length.ToString());
                Response.ContentType = "application/octet-stream";
                Response.BinaryWrite(byteArray);
                Response.End();
            }
        }

        #endregion

        protected void IsActive_PreRender(object sender, EventArgs e)
        {
            RadComboBox rcbFilter = sender as RadComboBox;
            if (ViewState[rcbFilter.ID] != null)
            {
                rcbFilter.SelectedValue = ViewState[rcbFilter.ID].ToString();
            }
        }

        protected void IsActive_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            RadComboBox combo = sender as RadComboBox;
            ViewState[combo.ID] = combo.SelectedValue;
            if (combo.SelectedValue != "0")
            {
                string ColumnName = combo.ID;
                string filterExpression = "([" + ColumnName + "] = '" + combo.SelectedValue + "')";
                grdInvalidEntries.MasterTableView.FilterExpression = filterExpression;
            }
            else
            {
                grdInvalidEntries.MasterTableView.FilterExpression = string.Empty;
            }
            FillDataSource();
            grdInvalidEntries.DataBind();
        }

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
        public void SetValueForNull(DataTable dt)
        {
            int firstIndex;
            int secondIndex;
            for (firstIndex = 0; firstIndex < dt.Columns.Count; firstIndex++)
            {
                for (secondIndex = 0; secondIndex < dt.Rows.Count; secondIndex++)
                {
                    if (dt.Columns[firstIndex].DataType.ToString() == "System.Int32" || dt.Columns[firstIndex].DataType.ToString() == "System.Single" || dt.Columns[firstIndex].DataType.ToString() == "System.Double" || dt.Columns[firstIndex].DataType.ToString() == "System.Decimal")
                    {
                        if (dt.Rows[secondIndex][firstIndex] == DBNull.Value)
                            dt.Rows[secondIndex][firstIndex] = 0;
                    }
                    else if (dt.Columns[firstIndex].DataType.ToString() == "System.String")
                    {
                        if (dt.Rows[secondIndex][firstIndex] == DBNull.Value || dt.Rows[secondIndex][firstIndex].ToString().Trim() == "")
                            dt.Rows[secondIndex][firstIndex] = "";
                    }
                }
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
                var validList = DomainApiClient<Dto.ADSXLProvisioningMatrix>.GetByAction<List<Dto.ADSXLProvisioningMatrix>>(DomainApiAction.Matrix.ADMatrix.GetADProvisioningMatrix, parameters);
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