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

namespace DI.Chakra.Web.DataManagement.SAPHANARolesMatrix.UC
{
    public partial class List : DI.Chakra.Web.UIBaseUC, IAccessControl
    {
        const string MatrixName = "SAP HANA Roles Matrix";
        static GridEditableItem selectedDataItem = null;
        private static IList<Dto.SAPHANAXLRolesMatrix> inValidList;

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
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.MiddleCell.Text = "Provisioning SAP HANA Roles Matrix  ";
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.MiddleCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.LeftCell.Text = PDFFileLeftCell;
            grdInvalidEntries.ExportSettings.Pdf.PageHeader.LeftCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(Session["ADXLMessage"])))
            {
                MasterPage.ShowAlert(AlertType.Success, Convert.ToString(Session["ADXLMessage"]));
                Session["ADXLMessage"] = string.Empty;
            }
            grdInvalidEntries.EnableLinqExpressions = true;
            XLUploadUC1.TemplateName = XLTemplates.SAPHANA_RoleMatrix;
            
            this.MasterPage.ActionClicked += ActionClicked;

            if (!Page.IsPostBack)
            {
                inValidList = new List<Dto.SAPHANAXLRolesMatrix>();
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
                var data = grdInvalidEntries.DataSource as List<Dto.SAPHANAXLRolesMatrix>;
                if (data != null && data.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(grdInvalidEntries.MasterTableView.FilterExpression))
                        data = data.AsEnumerable().AsQueryable().Where(grdInvalidEntries.MasterTableView.FilterExpression).ToList();
                    if (grdInvalidEntries.MasterTableView.SortExpressions != null && grdInvalidEntries.MasterTableView.SortExpressions.Count > 0)
                    {
                        data = data.AsEnumerable().AsQueryable().OrderBy(GetSortExpression(grdInvalidEntries.MasterTableView.SortExpressions)).ToList();
                    }
                    GenericExport(data, e.ActionType, "SAPHANAXLRoleMatrix");
                }
            }

        }
        protected override Dto.IDMSReportParam GetReportHeaderColumnMapping()
        {
            var param = new Dto.IDMSReportParam();
            param.ReportDataColumns = new List<Dto.IDMSReportDataColumn>();
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Remarks", "Remarks", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Application", "Application", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Module", "Module", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Sub Module", "SubModule", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Role name", "RoleName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Technical Role", "TechnicalRole", null));
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
                if (invalid != null) inValidList = invalid.Select(item => (Dto.SAPHANAXLRolesMatrix)item).ToList();
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
                LogDeletedRecords<Dto.SAPHANAXLRolesMatrix>(ContextObjectId.Value,MatrixName);
                DomainApiClient<Dto.SAPHANAXLRolesMatrix>.DeleteById(ContextObjectId.Value);
               
                ShowAlertMessage(AlertType.Success, "Record deleted successfully.");
                FillValidEntriesGrid();
                grdInvalidEntries.DataBind();

            }
            catch (Exception)
            {
                MasterPage.ShowAlert(AlertType.Error, "SAPHANAXLRoleMatrix cannot be deleted.");
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
            string validationmsg = string.Empty;

            if (id > 0)
            {
                editMode = true;
                GridEditableItem item = grdInvalidEntries.EditItems[0] as GridEditableItem;
                xldata = PopulateData(item, id, out validationmsg);
                successMessage = "Record updated successfully.";
                oldData = GetSerializeString<Dto.SAPHANAXLRolesMatrix>(id);
            }
            else
            {
                editMode = false;
                editableItem = grdInvalidEntries.MasterTableView.GetInsertItem();
                xldata = PopulateData(editableItem, id, out validationmsg);
                successMessage = "Record inserted successfully.";
            }
            
            if (!string.IsNullOrWhiteSpace(validationmsg))
            {
                MasterPage.ShowAlert(AlertType.Error, validationmsg);
                return;
            }

            Dictionary<string, string> parameterProcess = new Dictionary<string, string>();
            parameterProcess.Add("templateName", XLTemplates.SAPHANA_RoleMatrix.ToString());
            parameterProcess.Add("isEditMode", editMode.ToString());
            var datas = DomainApiClient<Dto.IXLData>.GetByAction<IList<DI.Chakra.Dto.IXLData>>(DomainApiAction.XLProcessor.XLData.ProcessXLData, parameterProcess, xldata);

            if (datas != null && datas.Count > 0)
            {
                string hasvalue = datas.First().ValidationMessage;
                if (string.IsNullOrWhiteSpace(hasvalue))
                {
                    UpdatedUILog<Dto.SAPHANAXLRolesMatrix>(id, MatrixName, data: oldData);
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
        private Dto.SAPHANAXLRolesMatrix PopulateData(GridEditableItem editableItem, long id, out string validationMsg)
        {
            Dto.SAPHANAXLRolesMatrix xldata = DomainApiClient<Dto.SAPHANAXLRolesMatrix>.GetById(id);
            Hashtable values = new Hashtable();
            editableItem.ExtractValues(values);
            if (xldata != null)
            {
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
                    return new Dto.SAPHANAXLRolesMatrix();
                }
                if (IsValidApproverType(Approvertype2.Text))
                {
                    xldata.Level2ApproverType = GetApproverType(Approvertype2.Text);
                    validationMsg = "";
                }
                else
                {
                    validationMsg = InvalidApprovalTypeMatrixMessage;
                    return new Dto.SAPHANAXLRolesMatrix();
                }
                if (IsValidApproverType(Approvertype3.Text))
                {
                    xldata.Level3ApproverType = GetApproverType(Approvertype3.Text);
                    validationMsg = "";
                }
                else
                {
                    validationMsg = InvalidApprovalTypeMatrixMessage;
                    return new Dto.SAPHANAXLRolesMatrix();
                }

                xldata.Level1Approver = XlHelper.FormatLeadingZeros(((string)values["Level1Approver"]), 8);
                xldata.Level2Approver = XlHelper.FormatLeadingZeros(((string)values["Level2Approver"]), 8);
                xldata.Level3Approver = XlHelper.FormatLeadingZeros(((string)values["Level3Approver"]), 8);
                xldata.Application = (string)values["Application"];
                xldata.Module = (string)values["Module"];
                xldata.Remarks = (string)values["Remarks"];
                xldata.RoleName = (string)values["RoleName"];
                xldata.SubModule = (string)values["SubModule"];
                xldata.TechnicalRole = (string)values["TechnicalRole"];
            }
            else
            {
                xldata = new Dto.SAPHANAXLRolesMatrix();
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
                    return new Dto.SAPHANAXLRolesMatrix();
                }
                if (IsValidApproverType(Approvertype2.Text))
                {
                    xldata.Level2ApproverType = GetApproverType(Approvertype2.Text);
                    validationMsg = "";
                }
                else
                {
                    validationMsg = InvalidApprovalTypeMatrixMessage;
                    return new Dto.SAPHANAXLRolesMatrix();
                }
                if (IsValidApproverType(Approvertype3.Text))
                {
                    xldata.Level3ApproverType = GetApproverType(Approvertype3.Text);
                    validationMsg = "";
                }
                else
                {
                    validationMsg = InvalidApprovalTypeMatrixMessage;
                    return new Dto.SAPHANAXLRolesMatrix();
                }
                xldata.Level1Approver = XlHelper.FormatLeadingZeros(((string)values["Level1Approver"]), 8);
                xldata.Level2Approver = XlHelper.FormatLeadingZeros(((string)values["Level2Approver"]), 8);
                xldata.Level3Approver = XlHelper.FormatLeadingZeros(((string)values["Level3Approver"]), 8);
                xldata.Application = (string)values["Application"];
                xldata.Module = (string)values["Module"];
                xldata.Remarks = (string)values["Remarks"];
                xldata.RoleName = (string)values["RoleName"];
                xldata.SubModule = (string)values["SubModule"];
                xldata.TechnicalRole = (string)values["TechnicalRole"];
            }
            if (!string.IsNullOrWhiteSpace(validationMsg))
            {
                return new Dto.SAPHANAXLRolesMatrix();
            }
            if (xldata != null)
            {
                if (string.IsNullOrWhiteSpace(xldata.Application))
                {
                    validationMsg = "Please enter Application";
                }
                var param = new Dictionary<string, string>();
                param.Add("applicationCode", xldata.Application);
                var ep = DomainApiClient<Dto.EndPoint>.GetByAction<Dto.EndPoint>("GetByApplicationCode", param);
                if (ep == null || (ep.ApplicationType != ApplicationType.EBS && ep.ApplicationType != ApplicationType.MSSL))
                {
                    MasterPage.ShowAlert(AlertType.Error, "");
                    validationMsg = "Please enter valid Application";
                    return new Dto.SAPHANAXLRolesMatrix();
                }
                if (id == 0)
                {
                    xldata.EndpointSystemId = ep.Id;
                }
            }
            return xldata;
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
                List<Dto.SAPHANAXLRolesMatrix> validList= DomainApiClient<Dto.SAPHANAXLRolesMatrix>.GetByAction<List<Dto.SAPHANAXLRolesMatrix>>(DomainApiAction.Matrix.SAPHANA.GetAllSAPHANAXLRoleMatrix, parameters);

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