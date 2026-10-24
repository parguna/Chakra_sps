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

namespace DI.Chakra.Web.DataManagement.SAPSRMMainRoleMatrix.UC
{
    public partial class List : DI.Chakra.Web.UIBaseUC, IAccessControl
    {
        const string MatrixName = "SAP SRM Main Role Matrix";
        static GridEditableItem selectedDataItem = null;
        private static IList<Dto.SAPSRMMainRoleMatrix> inValidList;
        static bool reloadDatasource = false;
        #region Members
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
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.MiddleCell.Text = "Provisioning SAP SRM Main Roles Matrix";
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.MiddleCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.LeftCell.Text = PDFFileLeftCell;
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.LeftCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(Session["ADXLMessage"])))
            {
                MasterPage.ShowAlert(AlertType.Success, Convert.ToString(Session["ADXLMessage"]));
                Session["ADXLMessage"] = string.Empty;
            }
            grdInvalidEntries.EnableLinqExpressions = true;
            XLUploadUC1.TemplateName = XLTemplates.SAPSRMMainRoleMatrix;

            this.MasterPage.ActionClicked += ActionClicked;

            if (!Page.IsPostBack)
            {
                inValidList = new List<Dto.SAPSRMMainRoleMatrix>();
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
            //    grdInvalidEntries.ExportSettings.FileName = "SAPSRMMainRoleMatrix";
            //    ExportNew(grdInvalidEntries, e.ActionType);
            //}
            //else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            //{
            //    grdInvalidEntries.ExportSettings.FileName = "SAPSRMMainRoleMatrix";
            //    ExportNew(grdInvalidEntries, e.ActionType);
            //}
            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                var data = grdInvalidEntries.DataSource as List<Dto.SAPSRMMainRoleMatrix>;
                if (data != null && data.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(grdInvalidEntries.MasterTableView.FilterExpression))
                        data = data.AsEnumerable().AsQueryable().Where(grdInvalidEntries.MasterTableView.FilterExpression).ToList();
                    if (grdInvalidEntries.MasterTableView.SortExpressions != null && grdInvalidEntries.MasterTableView.SortExpressions.Count > 0)
                    {
                        data = data.AsEnumerable().AsQueryable().OrderBy(GetSortExpression(grdInvalidEntries.MasterTableView.SortExpressions)).ToList();
                    }
                    GenericExport(data, e.ActionType, "SAPSRMMainRoleMatrix");
                }
            }
        }
        protected override Dto.IDMSReportParam GetReportHeaderColumnMapping()
        {
            var param = new Dto.IDMSReportParam();
            param.ReportDataColumns = new List<Dto.IDMSReportDataColumn>();
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("RoleType", "RoleType", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("BusinessRole", "BusinessRole", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("InternalPortalRoles", "InternalPortalRoles", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("InternalPortalGroups", "InternalPortalGroups", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("ExternalPortalRoles", "ExternalPortalRoles", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("ExternalPortalGroups", "ExternalPortalGroups", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("SRMRoles", "SRMRoles", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("SUSRoles", "SUSRoles", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("SRMEnablerRule", "SRMEnablerRule", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("BIRoles", "BIRoles", null));

            if(grdInvalidEntries.Columns.FindByDataField("BIEnablerRule").Display)
                param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("BIEnablerRule", "BIEnablerRule", null));

            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("ADGroups", "ADGroups", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Level1 Approver", "Level1Approver", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Level1 Approver Type", "Level1ApproverType", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Level2 Approver", "Level2Approver", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Level2 Approver Type", "Level2ApproverType", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Level3 Approver", "Level3Approver", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Level3 Approver Type", "Level3ApproverType", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("DepartmentCode", "DepartmentCode", null));
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
                if (invalid != null) inValidList = invalid.Select(item => (Dto.SAPSRMMainRoleMatrix)item).ToList();
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
                LogDeletedRecords<Dto.SAPSRMMainRoleMatrix>(ContextObjectId.Value,MatrixName);
                DomainApiClient<Dto.SAPSRMMainRoleMatrix>.DeleteById(ContextObjectId.Value);
                var sess = new SessionStatusImpl();
                DILog.UILogger.WriteLog(sess.User.Id, LogLevelType.Info, "Deleted by " + sess.User.UserName);
                MasterPage.ShowAlert(AlertType.Success, "Record deleted successfully.");
                reloadDatasource = true;
                FillValidEntriesGrid();
                grdInvalidEntries.DataBind();
            }
            catch (Exception)
            {
                MasterPage.ShowAlert(AlertType.Error, "SAPSRMMainRoleMatrix cannot be deleted.");
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
                oldData = GetSerializeString<Dto.SAPSRMMainRoleMatrix>(id);
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
            parameterProcess.Add("templateName", XLTemplates.SAPSRMMainRoleMatrix.ToString());
            parameterProcess.Add("isEditMode", editMode.ToString());
            var datas = DomainApiClient<Dto.IXLData>.GetByAction<IList<DI.Chakra.Dto.IXLData>>(DomainApiAction.XLProcessor.XLData.ProcessXLData, parameterProcess, xldata);
            reloadDatasource = true;
            if (datas != null && datas.Count > 0)
            {
                string hasvalue = datas.First().ValidationMessage;
                if (string.IsNullOrWhiteSpace(hasvalue))
                {
                    UpdatedUILog<Dto.SAPSRMMainRoleMatrix>(id, MatrixName, data: oldData);
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
        private Dto.SAPSRMMainRoleMatrix PopulateData(GridEditableItem editableItem, long id, out string validationMsg)
        {
            var existingData = DomainApiClient<Dto.SAPSRMMainRoleMatrix>.GetById(id);
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
                    return new Dto.SAPSRMMainRoleMatrix();
                }
                if (IsValidApproverType(Approvertype2.Text))
                {
                    existingData.Level2ApproverType = GetApproverType(Approvertype2.Text);
                    validationMsg = "";
                }
                else
                {
                    validationMsg = InvalidApprovalTypeMatrixMessage;
                    return new Dto.SAPSRMMainRoleMatrix();
                }
                if (IsValidApproverType(Approvertype3.Text))
                {
                    existingData.Level3ApproverType = GetApproverType(Approvertype3.Text);
                    validationMsg = "";
                }
                else
                {
                    validationMsg = InvalidApprovalTypeMatrixMessage;
                    return new Dto.SAPSRMMainRoleMatrix();
                }

                existingData.Level1Approver = XlHelper.FormatLeadingZeros(((string)values["Level1Approver"]), 8);
                existingData.Level2Approver = XlHelper.FormatLeadingZeros(((string)values["Level2Approver"]), 8);
                existingData.Level3Approver = XlHelper.FormatLeadingZeros(((string)values["Level3Approver"]), 8);


                existingData.RoleType = (string)values["RoleType"] == null ? string.Empty : (string)values["RoleType"];
                existingData.BusinessRole = (string)values["BusinessRole"] == null ? string.Empty : (string)values["BusinessRole"];
                existingData.SRMRoles = (string)values["SRMRoles"] == null ? string.Empty : (string)values["SRMRoles"];
                existingData.SUSRoles = (string)values["SUSRoles"] == null ? string.Empty : (string)values["SUSRoles"];
                existingData.SRMEnablerRule = (string)values["SRMEnablerRule"] == null ? string.Empty : (string)values["SRMEnablerRule"];

                existingData.InternalPortalRoles = (string)values["InternalPortalRoles"] == null ? string.Empty : (string)values["InternalPortalRoles"];
                existingData.InternalPortalGroups = (string)values["InternalPortalGroups"] == null ? string.Empty : (string)values["InternalPortalGroups"];
                existingData.ExternalPortalRoles = (string)values["ExternalPortalRoles"] == null ? string.Empty : (string)values["ExternalPortalRoles"];
                existingData.ExternalPortalGroups = (string)values["ExternalPortalGroups"] == null ? string.Empty : (string)values["ExternalPortalGroups"];
                existingData.ADGroups = (string)values["ADGroups"] == null ? string.Empty : (string)values["ADGroups"];
                existingData.BIRoles = (string)values["BIRoles"] == null ? string.Empty : (string)values["BIRoles"];
                existingData.DepartmentCode = (string)values["DepartmentCode"] == null ? string.Empty : (string)values["DepartmentCode"];
                return existingData;

            }
            else
            {
                Dto.SAPSRMMainRoleMatrix xldata = new Dto.SAPSRMMainRoleMatrix();
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
                    return new Dto.SAPSRMMainRoleMatrix();
                }
                if (IsValidApproverType(Approvertype2.Text))
                {
                    xldata.Level2ApproverType = GetApproverType(Approvertype2.Text);
                    validationMsg = "";
                }
                else
                {
                    validationMsg = InvalidApprovalTypeMatrixMessage;
                    return new Dto.SAPSRMMainRoleMatrix();
                }
                if (IsValidApproverType(Approvertype3.Text))
                {
                    xldata.Level3ApproverType = GetApproverType(Approvertype3.Text);
                    validationMsg = "";
                }
                else
                {
                    validationMsg = InvalidApprovalTypeMatrixMessage;
                    return new Dto.SAPSRMMainRoleMatrix();
                }
                xldata.Level1Approver = XlHelper.FormatLeadingZeros(((string)values["Level1Approver"]), 8);
                xldata.Level2Approver = XlHelper.FormatLeadingZeros(((string)values["Level2Approver"]), 8);
                xldata.Level3Approver = XlHelper.FormatLeadingZeros(((string)values["Level3Approver"]), 8);

                xldata.RoleType = (string)values["RoleType"] == null ? string.Empty : (string)values["RoleType"];
                xldata.BusinessRole = (string)values["BusinessRole"] == null ? string.Empty : (string)values["BusinessRole"];
                xldata.SRMRoles = (string)values["SRMRoles"] == null ? string.Empty : (string)values["SRMRoles"];
                xldata.SUSRoles = (string)values["SUSRoles"] == null ? string.Empty : (string)values["SUSRoles"];
                xldata.SRMEnablerRule = (string)values["SRMEnablerRule"] == null ? string.Empty : (string)values["SRMEnablerRule"];

                xldata.InternalPortalRoles = (string)values["InternalPortalRoles"] == null ? string.Empty : (string)values["InternalPortalRoles"];
                xldata.InternalPortalGroups = (string)values["InternalPortalGroups"] == null ? string.Empty : (string)values["InternalPortalGroups"];
                xldata.ExternalPortalRoles = (string)values["ExternalPortalRoles"] == null ? string.Empty : (string)values["ExternalPortalRoles"];
                xldata.ExternalPortalGroups = (string)values["ExternalPortalGroups"] == null ? string.Empty : (string)values["ExternalPortalGroups"];
                xldata.ADGroups = (string)values["ADGroups"] == null ? string.Empty : (string)values["ADGroups"];
                xldata.BIRoles = (string)values["BIRoles"] == null ? string.Empty : (string)values["BIRoles"];
                xldata.DepartmentCode = (string)values["DepartmentCode"] == null ? string.Empty : (string)values["DepartmentCode"];

                return xldata;
            }
        }
        #endregion
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
                List<Dto.SAPSRMMainRoleMatrix> validList = GetDatasource(parameters);

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
        private List<Dto.SAPSRMMainRoleMatrix> GetDatasource(Dictionary<string, string> parameters)
        {
            if (ViewState[XlHelper.xldatasource] == null || reloadDatasource)
            {
                List<Dto.SAPSRMMainRoleMatrix> validList = DomainApiClient<Dto.SAPSRMMainRoleMatrix>.GetByAction<List<Dto.SAPSRMMainRoleMatrix>>(DomainApiAction.Matrix.SRM.GetAllSAPSRMEMainRoleMatrix, parameters);
                ViewState[XlHelper.xldatasource] = validList;
                reloadDatasource = false;
            }
            return ViewState[XlHelper.xldatasource] as List<Dto.SAPSRMMainRoleMatrix>;
        }
    }
}