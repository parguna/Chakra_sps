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

namespace DI.Chakra.Web.DataManagement.SAPRoleEligibilityMatrix.UC
{
    public partial class List : DI.Chakra.Web.UIBaseUC, IAccessControl
    {
        const string MatrixName = "SAP FLINS Eligibility Role Matrix";
        static GridEditableItem selectedDataItem = null;
        private static IList<Dto.SAPRoleMatrix> inValidList;

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
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.MiddleCell.Text = "SAP Role Eligibility Matrix Configuration Management  ";
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.MiddleCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.LeftCell.Text = PDFFileLeftCell;
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.LeftCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(Session["ADXLMessage"])))
            {
                MasterPage.ShowAlert(AlertType.Success, Convert.ToString(Session["ADXLMessage"]));
                Session["ADXLMessage"] = string.Empty;
            }
            grdInvalidEntries.EnableLinqExpressions = true;
            XLUploadUC1.TemplateName = XLTemplates.SAP_RoleMatrix;

            //string strURL = Request.Url.GetLeftPart(UriPartial.Authority) + "/Files/CSV/DGM/WinAccessMatrix.csv";
            //this.btnExportToExcel.NavigateUrl = strURL;
            this.MasterPage.ActionClicked += ActionClicked;

            //lblDownloaded.Visible = false;
            if (!Page.IsPostBack)
            {
                inValidList = new List<Dto.SAPRoleMatrix>();
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
            //else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            //{
            //    grdInvalidEntries.ExportSettings.FileName = "SAPRoleMatrix";
            //    ExportNew(grdInvalidEntries, e.ActionType);
            //}
            //else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            //{
            //    grdInvalidEntries.ExportSettings.FileName = "SAPRoleMatrix";
            //    ExportNew(grdInvalidEntries, e.ActionType);
            //}
            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                var data = grdInvalidEntries.DataSource as List<Dto.SAPRoleMatrix>;
                if (data != null && data.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(grdInvalidEntries.MasterTableView.FilterExpression))
                        data = data.AsEnumerable().AsQueryable().Where(grdInvalidEntries.MasterTableView.FilterExpression).ToList();
                    if (grdInvalidEntries.MasterTableView.SortExpressions != null && grdInvalidEntries.MasterTableView.SortExpressions.Count > 0)
                    {
                        data = data.AsEnumerable().AsQueryable().OrderBy(GetSortExpression(grdInvalidEntries.MasterTableView.SortExpressions)).ToList();
                    }
                    GenericExport(data, e.ActionType, "SAPFLINSEligibilityRoleMatrix");
                }
            }
        }

        protected override Dto.IDMSReportParam GetReportHeaderColumnMapping()
        {
            var param = new Dto.IDMSReportParam();
            param.ReportDataColumns = new List<Dto.IDMSReportDataColumn>();
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Application", "Application", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Eligiblity Rule", "EligibilityRule", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Company", "Company", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Department Code", "PersonnelArea", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Branch Code", "PersonnelSubArea", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Position", "Position", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("OrgUnit", "OrgUnit", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Cost center", "CostCenter", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Remarks", "Remarks", null));
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
                if (invalid != null) inValidList = invalid.Select(item => (Dto.SAPRoleMatrix)item).ToList();
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
                LogDeletedRecords<Dto.SAPRoleMatrix>(ContextObjectId.Value,MatrixName);
                DomainApiClient<Dto.SAPRoleMatrix>.DeleteById(ContextObjectId.Value);
                
                ShowAlertMessage(AlertType.Success, "Record deleted successfully.");
                FillValidEntriesGrid();
                grdInvalidEntries.DataBind();
            }
            catch (Exception)
            {
                MasterPage.ShowAlert(AlertType.Error, "SAP FLINS Role Matrix cannot be deleted.");
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
                oldData = GetSerializeString<Dto.SAPRoleMatrix>(id);
            }
            else
            {
                editMode = false;
                editableItem = grdInvalidEntries.MasterTableView.GetInsertItem();
                xldata = PopulateData(editableItem, id);
                successMessage = "Record inserted successfully.";
            }

            Dictionary<string, string> parameterProcess = new Dictionary<string, string>();
            parameterProcess.Add("templateName", XLTemplates.SAP_RoleMatrix.ToString());
            parameterProcess.Add("isEditMode", editMode.ToString());
            var datas = DomainApiClient<Dto.IXLData>.GetByAction<IList<DI.Chakra.Dto.IXLData>>(DomainApiAction.XLProcessor.XLData.ProcessXLData, parameterProcess, xldata);

            if (datas != null && datas.Count > 0)
            {
                string hasvalue = datas.First().ValidationMessage;
                if (string.IsNullOrWhiteSpace(hasvalue))
                {
                    UpdatedUILog<Dto.SAPRoleMatrix>(id, MatrixName, data: oldData);
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
        private Dto.SAPRoleMatrix PopulateData(GridEditableItem editableItem, long id)
        {
            var existingData = DomainApiClient<Dto.SAPRoleMatrix>.GetById(id);
            Hashtable values = new Hashtable();
            editableItem.ExtractValues(values);
            if (existingData != null)
            {

                existingData.Application = (string)values["Application"];
                existingData.Company = ((string)values["Company"]) == null ? string.Empty : (string)values["Company"];
                existingData.CostCenter = ((string)values["CostCenter"]) == null ? string.Empty : (string)values["CostCenter"];
                existingData.EligibilityRule = (string)values["EligibilityRule"];
                existingData.OrgUnit = ((string)values["OrgUnit"]) == null ? string.Empty : (string)values["OrgUnit"];
                existingData.PersonnelArea = ((string)values["PersonnelArea"]) == null ? string.Empty : (string)values["PersonnelArea"];
                existingData.PersonnelSubArea = ((string)values["PersonnelSubArea"]) == null ? string.Empty : (string)values["PersonnelSubArea"];
                existingData.Position = ((string)values["Position"]) == null ? string.Empty : (string)values["Position"];
                existingData.Remarks = (string)values["Remarks"];
                return existingData;


            }
            else
            {
                Dto.SAPRoleMatrix xldata = new Dto.SAPRoleMatrix();
                xldata.Application = (string)values["Application"];
                xldata.Company = ((string)values["Company"]) == null ? string.Empty : (string)values["Company"];
                xldata.CostCenter = ((string)values["CostCenter"]) == null ? string.Empty : (string)values["CostCenter"];
                xldata.EligibilityRule = (string)values["EligibilityRule"];
                xldata.OrgUnit = ((string)values["OrgUnit"]) == null ? string.Empty : (string)values["OrgUnit"];
                xldata.PersonnelArea = ((string)values["PersonnelArea"]) == null ? string.Empty : (string)values["PersonnelArea"];
                xldata.PersonnelSubArea = ((string)values["PersonnelSubArea"]) == null ? string.Empty : (string)values["PersonnelSubArea"];
                xldata.Position = ((string)values["Position"]) == null ? string.Empty : (string)values["Position"];
                xldata.Remarks = (string)values["Remarks"];
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
                List<Dto.SAPRoleMatrix> validList= DomainApiClient<Dto.SAPRoleMatrix>.GetByAction<List<Dto.SAPRoleMatrix>>(DomainApiAction.Matrix.SAP_FLINS.SAPHRFLINSRoleMatrix.GetAllSAPRoleEligibilityMatrices, parameters);

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