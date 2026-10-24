using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.InHouse.UC
{
    public partial class List : UIBaseUC, IAccessControl
    {
        const string MatrixName = "InhouseUserInfo";
        #region Members
        static bool reloadDatasource = false;
        private const string V_ID = "ENTITY_ID";
        private const string V_TYPE = "TYPE";
        private const string V_TYPEID = "TYPE_ID";
        private static IList<InhouseUserInfo> inValidList;
        private SessionStatusImpl _sessionStatus = new SessionStatusImpl();
        static GridEditableItem selectedDataItem = null;
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
        public long EntityId
        {
            get
            {
                if (ViewState[V_ID] == null)
                    return -1;
                return (long)ViewState[V_ID];
            }
            set
            {
                ViewState[V_ID] = value;
            }
        }

        public string EndPointType
        {
            get
            {
                if (ViewState[V_TYPE] == null)
                    return EndpointType.InhouseSystems.ToString();
                return (string)ViewState[V_TYPE];
            }
            set
            {
                ViewState[V_TYPE] = value;
            }
        }

        public long EndPointTypeId
        {
            get
            {
                if (ViewState[V_TYPEID] == null)
                    return (long)EndpointType.InhouseSystems;
                return (long)ViewState[V_TYPEID];
            }
            set
            {
                ViewState[V_TYPEID] = value;
            }
        }


        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            Account.ExportSettings.Pdf.PageHeader.MiddleCell.Text = "Inhouse Accounts";
            Account.ExportSettings.Pdf.PageHeader.MiddleCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;
            Account.ExportSettings.Pdf.PageHeader.LeftCell.Text = PDFFileLeftCell;
            Account.ExportSettings.Pdf.PageHeader.LeftCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(Session["ADXLMessage"])))
            {
                MasterPage.ShowAlert(AlertType.Success, Convert.ToString(Session["ADXLMessage"]));
                Session["ADXLMessage"] = string.Empty;
            }
            Account.EnableLinqExpressions = true;
            XLUploadUC1.TemplateName = XLTemplates.Inhouse_UserInfo;
            this.MasterPage.DisableRedirect = true;
            this.MasterPage.ActionClicked += ActionClicked;

            if (!Page.IsPostBack)
            {
                LoadInhouseApplications();
                inValidList = new List<Dto.InhouseUserInfo>();
                rbInValid.Checked = false;
                rbValid.Checked = true;
                BindValidEntriesGrid();
                lblDetails.Text = XlHelper.GetWhoUpdatedData(XLUploadUC1.TemplateName);
            }
            else
                FillValidEntriesGrid();

            XlHelper.InvisibleBetween(Account.FilterMenu);
        }

        private void LoadInhouseApplications()
        {
            var dictEndpoints = new Dictionary<string, string>();
            dictEndpoints.Add("ALL", "ALL");
            var endpoints = DomainApiClient<Dto.EndPoint>.GetByAction<Dictionary<string, string>>
                   ("GetInhouseEndPointsWithCode", null);
            foreach (var ep in endpoints)
            {
                dictEndpoints.Add(ep.Key, ep.Value);
            }
            ddlInhouseSystems.DataSource = dictEndpoints;
            ddlInhouseSystems.DataTextField = "Value";
            ddlInhouseSystems.DataValueField = "Key";
            ddlInhouseSystems.DataBind();
        }

        protected void BindValidEntriesGrid()
        {
            reloadDatasource = true;
            FillValidEntriesGrid();
            Account.DataBind();
        }

        private void FillValidEntriesGrid()
        {
            bool isValidRecords = rbValid.Checked;

            if (rbInValid.Checked == true)
                FillInvalidEntriesGrid();
            else
            {
                List<Dto.Presentation.PresentationInhouseUserInfo> validList = GetDatasource();

                if (rbValid.Checked == true)
                    grdHeader.InnerText = "Valid Records ";
                Account.DataSource = validList;
                Account.MasterTableView.Columns.FindByUniqueName("ValidationMessage").Display = false;
                Account.MasterTableView.Columns.FindByUniqueName("Edit").Display = true;
                Account.MasterTableView.CommandItemSettings.ShowAddNewRecordButton = true;
                Account.MasterTableView.CommandItemSettings.ShowExportToCsvButton = true;
                Account.MasterTableView.CommandItemSettings.ShowExportToExcelButton = false;
            }
        }
        private List<Dto.Presentation.PresentationInhouseUserInfo> GetDatasource()
        {
            if (ViewState[XlHelper.xldatasource] == null || reloadDatasource)
            {
                string applicationCode = ddlInhouseSystems.SelectedValue == "ALL" ? "" : ddlInhouseSystems.SelectedValue;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("inhousesystemcode", applicationCode);
                var result = DomainApiClient<Dto.Presentation.PresentationInhouseUserInfo>.
                    GetByAction<List<Dto.Presentation.PresentationInhouseUserInfo>>
                    ("GetInhouseUserInfoBySystemCode", parameter);
                ViewState[XlHelper.xldatasource] = result;
                reloadDatasource = false;
            }
            return ViewState[XlHelper.xldatasource] as List<Dto.Presentation.PresentationInhouseUserInfo>;
        }

        private void FillInvalidEntriesGrid()
        {
            grdHeader.InnerText = "InValid Records ";
            Account.DataSource = inValidList;
            Account.MasterTableView.Columns.FindByUniqueName("ValidationMessage").Display = true;
            Account.MasterTableView.Columns.FindByUniqueName("Edit").Display = false;
            Account.MasterTableView.CommandItemSettings.ShowAddNewRecordButton = false;
            Account.MasterTableView.CommandItemSettings.ShowExportToCsvButton = false;
            Account.MasterTableView.CommandItemSettings.ShowExportToExcelButton = false;
        }

        private void UserControlButtonClicked(object sender, EventArgs e)
        {
            try
            {
                inValidList = null;
                IList<Dto.IXLData> invalid = XLUploadUC1.InvalidList;
                string errMsg = XLUploadUC1.ErrorMessage;
                if (invalid != null) inValidList = invalid.Select(item => (Dto.InhouseUserInfo)item).ToList();
                if (!(string.IsNullOrEmpty(errMsg)))
                {
                    rbInValid.Checked = true;
                    rbValid.Checked = false;
                    BindInValidGrid();
                    MasterPage.ShowAlert(AlertType.Error, errMsg);
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
            Account.DataBind();
        }

        protected void ValidRecordsSelectionChanged(object sender, EventArgs e)
        {
            BindValidEntriesGrid();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            modalPopup.VisibleOnPageLoad = false;
        }

        protected void EndpointUC_ApplicationChanged(EndPointSystemType EndPointSystemType)
        {
            //acre.Clear();
            if (EndPointSystemType != null)
            {
                EndPointType = EndPointSystemType.EndpointSystemTypeName.ToString();
                EndPointTypeId = EndPointSystemType.Id;
                Account.Rebind();
            }
            else
            {
                EndPointTypeId = 0;
                EntityId = -1;
                Account.Rebind();
            }
        }

        protected void EndpointUC_EndpointChanged(EndPoint EndPoint)
        {
            //acre.Clear();
            if (EndPoint != null)
            {
                EntityId = (long)EndPoint.Id;
                if (_sessionStatus.AccountNavigatorHelper != null)
                    _sessionStatus.AccountNavigatorHelper.ApplicationId = EndPoint.Id;
                else
                {
                    AccountNavigatorHelper accountNavigatorHelper = new AccountNavigatorHelper();
                    _sessionStatus.AccountNavigatorHelper = accountNavigatorHelper;
                    _sessionStatus.AccountNavigatorHelper.ApplicationTypeId = 0;
                    _sessionStatus.AccountNavigatorHelper.ApplicationId = EndPoint.Id;
                }
                Account.Rebind();
            }
            else
            {
                EntityId = -1;
                if (_sessionStatus.AccountNavigatorHelper != null)
                    _sessionStatus.AccountNavigatorHelper.ApplicationId = -1;
                Account.Rebind();
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
                if (Account.SelectedItems.Count > 0)
                {
                    selectedDataItem = Account.SelectedItems[0] as GridEditableItem;
                    selectedDataItem.Edit = true;
                    //selectedDataItem["Edit"].Controls[0].Visible = true;
                    //ChangeEditButton(Account.SelectedItems[0] as GridDataItem, true);
                    Account.Rebind();
                }
            }
            else if (e.ActionType == ACLAction.Add)
            {
                Account.MasterTableView.InsertItem();
                Account.Rebind();
                selectedDataItem = Account.MasterTableView.GetInsertItem();
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
                Account.MasterTableView.Columns.FindByUniqueName("CreatedBy").Visible = true;
                Account.MasterTableView.Columns.FindByUniqueName("CreatedDate").Visible = true;
                Account.MasterTableView.GetColumn("CreatedBy").Display = true;
                Account.MasterTableView.GetColumn("CreatedDate").Display = true;
                Account.Rebind();
                Account.DownloadTemplate(Account, e.ActionType);
            }
            //else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            //{
            //    Account.ExportSettings.FileName = "InhouseUsers";
            //    Account.Export(Account, e.ActionType);
            //}
            //else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            //{
            //    Account.ExportSettings.FileName = "InhouseUsers";
            //    Account.Export(Account, e.ActionType);
            //}
            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                var data = Account.DataSource as List<Dto.InhouseUserInfo>;
                if (data != null)
                {
                    if (data.Count > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(Account.MasterTableView.FilterExpression))
                            data = data.AsEnumerable().AsQueryable().Where(Account.MasterTableView.FilterExpression).ToList();
                        if (Account.MasterTableView.SortExpressions != null && Account.MasterTableView.SortExpressions.Count > 0)
                        {
                            data = data.AsEnumerable().AsQueryable().OrderBy(GetSortExpression(Account.MasterTableView.SortExpressions)).ToList();
                        }
                        GenericExport(data, e.ActionType, "InhouseUsers");
                    }
                }
                else
                {
                    var inHouseInfo = Account.DataSource as List<Dto.Presentation.PresentationInhouseUserInfo>;
                    if (inHouseInfo.Count > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(Account.MasterTableView.FilterExpression))
                            inHouseInfo = inHouseInfo.AsEnumerable().AsQueryable().Where(Account.MasterTableView.FilterExpression).ToList();
                        if (Account.MasterTableView.SortExpressions != null && Account.MasterTableView.SortExpressions.Count > 0)
                        {
                            inHouseInfo = inHouseInfo.AsEnumerable().AsQueryable().OrderBy(GetSortExpression(Account.MasterTableView.SortExpressions)).ToList();
                        }
                        GenericExport(inHouseInfo, e.ActionType, "InhouseUsers");
                    }
                }
            }

        }
        protected override IDMSReportParam GetReportHeaderColumnMapping()
        {
            var param = new Dto.IDMSReportParam();
            param.ReportDataColumns = new List<Dto.IDMSReportDataColumn>();
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("UserName", "UserName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Inhouse System Code", "InhouseSystemCode", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Employee Id", "EmployeeId", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Email", "Email", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Display Name", "DisplayName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("First Name", "FirstName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Last Name", "LastName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Account Type", "AccountType", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Account Created Date", "AccountCreatedDate", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Last Login Date", "LastLoginDate", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Is Locked", "IsLocked", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("IsDisabled", "IsDisabled", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("IsDeleted", "IsDeleted", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Remarks1", "Remarks1", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Remarks2", "Remarks2", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Remarks3", "Remarks3", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Remarks4", "Remarks4", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Remarks5", "Remarks5", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Permission", "Permission", null));
            if (rbInValid.Checked)
                param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Validation Message", "ValidationMessage", null));
            return param;
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
            if (id > 0)
            {
                editMode = true;
                GridEditableItem item = Account.EditItems[0] as GridEditableItem;
                item.ExtractValues(values);
                xldata = PopulateData(values, id);
                successMessage = "Record updated successfully.";
                oldData = GetSerializeString<Dto.InhouseUserInfo>(id);
            }
            else
            {
                editMode = false;
                editableItem = Account.MasterTableView.GetInsertItem();
                editableItem.ExtractValues(values);
                xldata = PopulateData(values, id);
                successMessage = "Record inserted successfully.";
            }

            Dictionary<string, string> parameterProcess = new Dictionary<string, string>();
            parameterProcess.Add("templateName", XLTemplates.Inhouse_UserInfo.ToString());
            parameterProcess.Add("isEditMode", editMode.ToString());
            var datas = DomainApiClient<Dto.IXLData>.GetByAction<IList<DI.Chakra.Dto.IXLData>>(DomainApiAction.XLProcessor.XLData.ProcessXLData, parameterProcess, xldata);

            if (datas != null && datas.Count > 0)
            {
                string hasvalue = datas.First().ValidationMessage;
                if (string.IsNullOrWhiteSpace(hasvalue))
                {
                    UpdatedUILog<Dto.InhouseUserInfo>(id, MatrixName,data:oldData);
                    MasterPage.ShowAlert(AlertType.Success, successMessage);
                    Account.MasterTableView.IsItemInserted = false;
                    Account.MasterTableView.ClearEditItems();
                    Account.EnableRowOnDemandVS = true;
                    if (id == 0)
                    {
                        reloadDatasource = true;
                        FillValidEntriesGrid();
                        Account.DataBind();
                    }
                    else
                    {
                        reloadDatasource = true;
                        FillValidEntriesGrid();
                        Account.DataBind();
                    }

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

        InhouseUserInfo AssignValues(InhouseUserInfo existingData, Hashtable values)
        {
            if (existingData == null)
                existingData = new InhouseUserInfo();
            existingData.UserName = (string)values["UserName"];
            existingData.InhouseSystemCode = (string)values["InhouseSystemCode"];
            existingData.EmployeeId = (string)values["EmployeeId"];
            existingData.Email = (string)values["Email"];
            existingData.DisplayName = (string)values["DisplayName"];
            existingData.FirstName = (string)values["FirstName"];
            existingData.LastName = (string)values["LastName"];
            existingData.AccountType = (AccountType)Enum.Parse(typeof(AccountType), (string)values["AccountTypeString"]);
            existingData.AccountCreatedDate = XlHelper.GetDateFromString(values["AccountCreatedDate"]);
            existingData.LastLoginDate = XlHelper.GetDateFromString(values["LastLoginDate"]);
            existingData.IsLocked = Convert.ToBoolean(values["IsLocked"]);
            existingData.IsDisabled = Convert.ToBoolean(values["IsDisabled"]);
            existingData.IsDeleted = Convert.ToBoolean(values["IsDeleted"]);
            existingData.Remarks1 = (string)values["Remarks1"];
            existingData.Remarks2 = (string)values["Remarks2"];
            existingData.Remarks3 = (string)values["Remarks3"];
            existingData.Remarks4 = (string)values["Remarks4"];
            existingData.Remarks5 = (string)values["Remarks5"];
            existingData.Permission = (string)values["Permission"];
            return existingData;
        }

        Dto.IXLData PopulateData(Hashtable values, long id)
        {
            var existingData = DomainApiClient<Dto.InhouseUserInfo>.GetById(id);
            return AssignValues(existingData, values);
        }

        protected void Account_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            Account.DataSource = inValidList;
        }

        protected void Account_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem Grid = Account.SelectedItems[0] as GridDataItem;
            if (Grid.GetDataKeyValue("ReconciliationId") != null)
            {
                this.ContextObjectId = (long)Grid.GetDataKeyValue("ReconciliationId");
            }
        }
        #endregion

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }

        protected void AccountType_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            //RadComboBox combo = sender as RadComboBox;
            //ViewState[combo.ID] = combo.SelectedValue;
            //if (combo.SelectedValue.ToString() != "All")
            //{
            //    string ColumnName = combo.ID;
            //    string filterExpression = "([" + ColumnName + "] = '" + e.Text + "')";
            //    Account.MasterTableView.FilterExpression = filterExpression;
            //}
            //else
            //{
            //    Account.MasterTableView.FilterExpression = string.Empty;
            //}
            reloadDatasource = true;
            GetDatasource();
            Account.DataBind();
        }

        protected void AccountType_PreRender(object sender, EventArgs e)
        {
            RadComboBox rcbFilter = sender as RadComboBox;
            if (ViewState[rcbFilter.ID] != null)
            {
                rcbFilter.SelectedValue = ViewState[rcbFilter.ID].ToString();
            }
        }

        protected void Account_ItemDataBound(object sender, GridItemEventArgs e)
        {
            try
            {
                if (e.Item is GridDataItem)
                {
                    GridDataItem item = (GridDataItem)e.Item;
                    ChangeEditButton(item, false);
                }
                if (e.Item is GridFilteringItem)
                {
                    GridFilteringItem Item = (GridFilteringItem)e.Item;
                    RadComboBox combo = (RadComboBox)Item.FindControl("AccountType");
                    if (combo != null)
                    {

                        combo.DataSource = Enum.GetValues(typeof(AccountType));
                        combo.DataBind();
                    }

                }
                if (e.Item is GridEditableItem)
                {
                    GridEditableItem edit = (GridEditableItem)e.Item;
                    var data = edit.GetDataKeyValue("Id")?.ToString();
                    if(data != null && !string.IsNullOrWhiteSpace(data))
                        edit["UserName"].Enabled = false;
                    else
                    {
                        edit["UserName"].Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                //MasterPage.ShowAlert(AlertType.Error, "Test");
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

        protected void XLUploadUC1_UserControlButtonClicked()
        {
            UserControlButtonClicked(this, EventArgs.Empty);
            modalPopup.VisibleOnPageLoad = false;
        }

        protected void XLUploadUC1_UserControlButtonClicked1(object sender, EventArgs e)
        {
            UserControlButtonClicked(this, EventArgs.Empty);
            modalPopup.VisibleOnPageLoad = false;
        }

        #region Methods
        //private void BindExtraCriteria()
        //{
        //    //MappingTypesField.Items.Clear();
        //    foreach (DI.Chakra.Infrastructure.Enums.MappingType item in Enum.GetValues(typeof(DI.Chakra.Infrastructure.Enums.MappingType)))
        //    {
        //        var description = UtilityMethods.GetEnumAttribute<Description>(item);
        //        ListItem listItem = new ListItem(description, ((byte)item).ToString());
        //        //MappingTypesField.Items.Add(listItem);
        //    }
        //}

        private void delete()
        {
            if (!ContextObjectId.HasValue)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item.");
                return;
            }
            try
            {
                LogDeletedRecords<Dto.InhouseUserInfo>(ContextObjectId.Value, MatrixName);
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("accReconId", ContextObjectId.Value.ToString());
                parameter.Add("currentUser", _sessionStatus.User.DisplayName);
                DomainApiClient<Dto.InhouseUserInfo>.
                    GetByAction<object>(DomainApiAction.Endpoint.InhouseSystems.DeleteByAccReconId,
                    parameter);
                
                MasterPage.ShowAlert(AlertType.Success, "InHouse User is Deleted Successfully");
                reloadDatasource = true;
                FillValidEntriesGrid();
                Account.DataBind();
            }
            catch (Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, "InHouse User cannot be Deleted");
            }
        }

        #endregion

        protected void ddlInhouseSystems_SelectedIndexChanged(object sender, EventArgs e)
        {
            reloadDatasource = true;
            FillValidEntriesGrid();
            Account.Rebind();
        }
    }
}
