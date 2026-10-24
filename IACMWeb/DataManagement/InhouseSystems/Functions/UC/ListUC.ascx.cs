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

namespace DI.Chakra.Web.DataManagement.InHouseFunctions.UC
{
    public partial class List : DI.Chakra.Web.UIBaseUC, IAccessControl
    {
        const string MatrixName = "Inhouse Function";
        static GridEditableItem selectedDataItem = null;
        private static IList<Dto.InhouseFunction> inValidList;
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
        #endregion

        #region Events
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
            XLUploadUC1.TemplateName = XLTemplates.Inhouse_Functions;

            this.MasterPage.ActionClicked += ActionClicked;

            if (!Page.IsPostBack)
            {
                inValidList = new List<Dto.InhouseFunction>();
                ddlEndPointType.Items.Add(UtilityMethods.GetEnumAttribute<Description>(ApplicationType.InhouseSystems));
                ddlEndPointType.Enabled = false;
                this.EndpointSystemType = EndpointType.InhouseSystems;
                BindEndPointSystem();
                BindValidEntriesGrid();
                lblDetails.Text = XlHelper.GetWhoUpdatedData(XLUploadUC1.TemplateName);
            }
            else
                FillValidEntriesGrid();

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
                ddlEndpoint.DataSource = list.Where(p => p.IsFunctionLevel).ToList();
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
            //    grdInvalidEntries.ExportSettings.FileName = "InhouseFunction";
            //    ExportNew(grdInvalidEntries, e.ActionType);
            //}
            //else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            //{
            //    grdInvalidEntries.ExportSettings.FileName = "InhouseFunction";
            //    ExportNew(grdInvalidEntries, e.ActionType);
            //}
            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                var data = grdInvalidEntries.DataSource as List<Dto.InhouseFunction>;
                if (data != null && data.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(grdInvalidEntries.MasterTableView.FilterExpression))
                        data = data.AsEnumerable().AsQueryable().Where(grdInvalidEntries.MasterTableView.FilterExpression).ToList();
                    if (grdInvalidEntries.MasterTableView.SortExpressions != null && grdInvalidEntries.MasterTableView.SortExpressions.Count > 0)
                    {
                        data = data.AsEnumerable().AsQueryable().OrderBy(GetSortExpression(grdInvalidEntries.MasterTableView.SortExpressions)).ToList();
                    }
                    GenericExport(data, e.ActionType, "InhouseFunction");
                }
            }

        }

        protected override Dto.IDMSReportParam GetReportHeaderColumnMapping()
        {
            var param = new Dto.IDMSReportParam();
            param.ReportDataColumns = new List<Dto.IDMSReportDataColumn>();
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Function Name", "FunctionName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Permission Name", "PermissionName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Inhouse System Code", "InhouseSystemCode", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Description", "Description", null));
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
                if (invalid != null) inValidList = invalid.Select(item => (Dto.InhouseFunction)item).ToList();
                if (!string.IsNullOrWhiteSpace(errMsg))
                {
                    BindInValidGrid();
                    MasterPage.ShowAlert(AlertType.Error, errMsg);
                    modalPopup.VisibleOnPageLoad = false;
                }
                else
                {
                    BindInValidGrid();
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
                LogDeletedRecords<Dto.InhouseFunction>(ContextObjectId.Value, MatrixName);
                DomainApiClient<Dto.InhouseFunction>.DeleteById(ContextObjectId.Value);
                
                ShowAlertMessage(AlertType.Success, "Record deleted successfully.");
                FillValidEntriesGrid();
                grdInvalidEntries.DataBind();
            }
            catch (Exception)
            {
                MasterPage.ShowAlert(AlertType.Error, "InhouseFunction cannot be deleted.");
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
                oldData = GetSerializeString<Dto.InhouseFunction>(id);
            }
            else
            {
                editMode = false;
                editableItem = grdInvalidEntries.MasterTableView.GetInsertItem();
                xldata = PopulateData(editableItem, id);
                successMessage = "Record inserted successfully.";
            }


            Dictionary<string, string> parameterProcess = new Dictionary<string, string>();
            parameterProcess.Add("templateName", XLTemplates.Inhouse_Functions.ToString());
            parameterProcess.Add("isEditMode", editMode.ToString());
            var datas = DomainApiClient<Dto.IXLData>.GetByAction<IList<DI.Chakra.Dto.IXLData>>(DomainApiAction.XLProcessor.XLData.ProcessXLData, parameterProcess, xldata);
            if (datas != null && datas.Count > 0)
            {
                string hasvalue = datas.First().ValidationMessage;
                if (string.IsNullOrWhiteSpace(hasvalue))
                {
                    UpdatedUILog<Dto.InhouseFunction>(id,MatrixName,data: oldData);
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
        private Dto.InhouseFunction PopulateData(GridEditableItem editableItem, long id)
        {
            var existingData = DomainApiClient<Dto.InhouseFunction>.GetById(id);
            Hashtable values = new Hashtable();
            editableItem.ExtractValues(values);
            if (existingData != null)
            {
                existingData.InhouseSystemCode = (string)values["InhouseSystemCode"];
                existingData.PermissionName = (string)values["PermissionName"];
                existingData.FunctionName = (string)values["FunctionName"];
                existingData.Description = (string)values["Description"];
                existingData.IsActive = Convert.ToBoolean(values["IsActive"]);
                return existingData;
            }
            else
            {
                Dto.InhouseFunction xldata = new Dto.InhouseFunction();
                xldata.InhouseSystemCode = (string)values["InhouseSystemCode"];
                xldata.PermissionName = (string)values["PermissionName"];
                xldata.FunctionName = (string)values["FunctionName"];
                xldata.Description = (string)values["Description"];
                xldata.IsActive = Convert.ToBoolean(values["IsActive"]);
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
            FillValidEntriesGrid();
            grdInvalidEntries.DataBind();
        }
        private void FillInvalidEntriesGrid()
        {
            grdInvalidEntries.DataSource = inValidList;
            grdInvalidEntries.MasterTableView.Columns.FindByUniqueName("ValidationMessage").Display = true;
            grdInvalidEntries.MasterTableView.Columns.FindByUniqueName("Edit").Display = false;
            grdInvalidEntries.MasterTableView.CommandItemSettings.ShowAddNewRecordButton = false;
            grdInvalidEntries.MasterTableView.CommandItemSettings.ShowExportToCsvButton = false;
            grdInvalidEntries.MasterTableView.CommandItemSettings.ShowExportToExcelButton = false;
        }
        private void FillValidEntriesGrid()
        {
            List<Dto.InhouseFunction> validList = GetDatasource();
            grdInvalidEntries.DataSource = validList;
            grdInvalidEntries.MasterTableView.Columns.FindByUniqueName("ValidationMessage").Display = false;
            grdInvalidEntries.MasterTableView.Columns.FindByUniqueName("Edit").Display = true;
            grdInvalidEntries.MasterTableView.CommandItemSettings.ShowAddNewRecordButton = true;
            grdInvalidEntries.MasterTableView.CommandItemSettings.ShowExportToCsvButton = true;
            grdInvalidEntries.MasterTableView.CommandItemSettings.ShowExportToExcelButton = false;
        }
        private List<Dto.InhouseFunction> GetDatasource()
        {
            var param = new Dictionary<string, string>();
            param.Add("InhouseSystemCode", Convert.ToString(ddlEndpoint.SelectedValue));
            List<Dto.InhouseFunction> validList = DomainApiClient<Dto.InhouseFunction>.GetByAction<List<Dto.InhouseFunction>>(DomainApiAction.Endpoint.InhouseSystems.GetInhouseFunctionGridData, param);
            ViewState[XlHelper.xldatasource] = validList;
            return validList;
        }
    }
}