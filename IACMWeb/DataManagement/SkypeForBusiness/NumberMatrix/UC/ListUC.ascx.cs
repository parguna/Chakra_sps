using Application;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.DataManagement.SkypeForBusiness.NumberMatrix.UC
{
    public partial class ListUC : DI.Chakra.Web.UIBaseUC, IAccessControl
    {
        const string MatrixName = "SkypeForBusiness UnAssigned Number Matrix";
        private List<Dto.SkypeForBusinessUnAssignedNumberMatrix> listNumberMatrixData ;
        static GridEditableItem selectedDataItem = null;
        public bool IsEdited
        {
            get
            {
                return (bool)ViewState["IsEdited"];
            }
            set
            {
                ViewState["IsEdited"] = value;
            }

        }
        public long EndpointSystemId
        {
            get
            {
                if (ViewState["EndpointSystemId"] != null)
                    return Convert.ToInt64(ViewState["EndpointSystemId"]);
                return 0;
            }
            set
            {
                ViewState["EndpointSystemId"] = value;
            }

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;

            if (!IsPostBack)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("applicationType", ((int)DI.Chakra.Infrastructure.Enums.ApplicationType.MicrosoftTeams).ToString());
                var SelectedApplication = DomainApiClient<Dto.EndPoint>.GetByAction<Dto.EndPoint>(DomainApiAction.Endpoint.EndPoint.GetEndPointByApplicationType, dic);
                EndpointSystemId = SelectedApplication.Id;
                ViewState["IsEdited"] = false;
                NumberMatrixRadGrid.DataBind();
            }
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == Infrastructure.Enums.ACLAction.Edit)
            {

                if (NumberMatrixRadGrid.SelectedItems.Count > 0)
                {
                    selectedDataItem = NumberMatrixRadGrid.SelectedItems[0] as GridEditableItem;
                    selectedDataItem.Edit = true;
                    NumberMatrixRadGrid.Rebind();
                }
                IsEdited = true;
            }
            else if (e.ActionType == Infrastructure.Enums.ACLAction.Save)
            {
                //if (selectedDataItem != null)
                //{
                //    AddOrUpdate();
                //}
                bool isUpdated = false;
                foreach (GridDataItem item in NumberMatrixRadGrid.MasterTableView.Items)
                {
                    if(item != null)
                    {
                        var chkReserved = item.FindControl("chkReserved") as CheckBox;
                        if (chkReserved != null && chkReserved.Visible)
                        {
                            Dto.SkypeForBusinessUnAssignedNumberMatrix data = null;
                            var id = Convert.ToInt64(item.GetDataKeyValue("Id"));
                            if (id > 0)
                            {
                                data = PopulateData(item, id);
                                if (data != null)
                                {
                                    Dictionary<string, string> parameterProcess = new Dictionary<string, string>();
                                    oldData = GetSerializeString<Dto.SkypeForBusinessUnAssignedNumberMatrix>(id);
                                    UpdatedUILog<Dto.SkypeForBusinessUnAssignedNumberMatrix>(id, MatrixName, data: oldData);
                                    DomainApiClient<Dto.SkypeForBusinessUnAssignedNumberMatrix>.Save(data);
                                    isUpdated = true;
                                }
                            }
                        }
                    }
                }
                if (isUpdated)
                {
                    MasterPage.ShowAlert(Infrastructure.AlertType.Success, "Record updated successfully");
                    NumberMatrixRadGrid.Rebind();
                }
                else
                {
                    MasterPage.ShowAlert(Infrastructure.AlertType.Warning, "Please select record(s) to update");
                }
            }
            //else if (e.ActionType == Infrastructure.Enums.ACLAction.Excel || e.ActionType == Infrastructure.Enums.ACLAction.ExcelAll)
            //{
            //    NumberMatrixRadGrid.ExportSettings.FileName = "LineURIExtensionManagement";
            //    NumberMatrixRadGrid.Export(NumberMatrixRadGrid, e.ActionType);
            //}
            //else if (e.ActionType == Infrastructure.Enums.ACLAction.CSV || e.ActionType == Infrastructure.Enums.ACLAction.CSVAll)
            //{
            //    NumberMatrixRadGrid.ExportSettings.FileName = "LineURIExtensionManagement";
            //    NumberMatrixRadGrid.Export(NumberMatrixRadGrid, e.ActionType);
            //}
            else if (e.ActionType == Infrastructure.Enums.ACLAction.Excel || e.ActionType == Infrastructure.Enums.ACLAction.CSV || e.ActionType == Infrastructure.Enums.ACLAction.ExcelAll || e.ActionType == Infrastructure.Enums.ACLAction.CSVAll)
            {
                var data = GetData();
                if (data != null && data.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(NumberMatrixRadGrid.MasterTableView.FilterExpression))
                        data = data.AsEnumerable().AsQueryable().Where(NumberMatrixRadGrid.MasterTableView.FilterExpression).ToList();
                    if (NumberMatrixRadGrid.MasterTableView.SortExpressions != null && NumberMatrixRadGrid.MasterTableView.SortExpressions.Count > 0)
                    {
                        data = data.AsEnumerable().AsQueryable().OrderBy(GetSortExpression(NumberMatrixRadGrid.MasterTableView.SortExpressions)).ToList();
                    }
                    GenericExport(data, e.ActionType, "LineURIExtensionManagement");
                }
            }
        }
        protected override Dto.IDMSReportParam GetReportHeaderColumnMapping()
        {
            var param = new Dto.IDMSReportParam();
            param.ReportDataColumns = new List<Dto.IDMSReportDataColumn>();
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Number", "Number", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Assigned", "Assigned", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Assigned To", "CreatedBy", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Reserved", "Reserved", null));
            return param;
        }

        //private void AddOrUpdate()
        //{
        //    bool isEditMode = false;
        //    var editableItem = selectedDataItem;
        //    Dto.SkypeForBusinessUnAssignedNumberMatrix data = null;
        //    var id = Convert.ToInt64(editableItem.GetDataKeyValue("Id"));
        //    if (id > 0)
        //    {
        //        isEditMode = true;
        //        GridEditableItem item = NumberMatrixRadGrid.EditItems[0] as GridEditableItem;
        //        data = PopulateData(item, id);
        //        if(data != null)
        //        {
        //            Dictionary<string, string> parameterProcess = new Dictionary<string, string>();
        //            DomainApiClient<Dto.SkypeForBusinessUnAssignedNumberMatrix>.Save(data);
        //            MasterPage.ShowAlert(Infrastructure.AlertType.Success, "Record updated successfully");
        //            NumberMatrixRadGrid.MasterTableView.ClearEditItems();
        //            this.ContextObjectId = null;
        //            NumberMatrixRadGrid.Rebind();
        //        }
        //    }
        //}
        private Dto.SkypeForBusinessUnAssignedNumberMatrix PopulateData(GridDataItem editableItem, long id)
        {
            var existingData = DomainApiClient<Dto.SkypeForBusinessUnAssignedNumberMatrix>.GetById(id);
            if(existingData != null)
            {
                Hashtable values = new Hashtable();
                editableItem.ExtractValues(values);
                var userSession = new SessionStatusImpl()?.User?.UserName;
                var chkReserved = editableItem.FindControl("chkReserved") as CheckBox;
                var Reserved = chkReserved.Checked;
                existingData.Reserved = Reserved;
                existingData.LastModifiedBy = userSession;
                existingData.LastModifiedDate = DateTime.Now;
                
            }
            return existingData;

        }
        protected void NumberMatrixRadGrid_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (NumberMatrixRadGrid.MasterTableView.IsItemInserted)
            {
                e.Canceled = false;
                NumberMatrixRadGrid.Rebind();
            }

        }

        protected void NumberMatrixRadGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            BindSkypeForBusinessNumberMatrixData();

        }

        protected void NumberMatrixRadGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NumberMatrixRadGrid.SelectedItems.Count == 0) return;
            
            GridDataItem Grid = NumberMatrixRadGrid.SelectedItems[0] as GridDataItem;
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
        void ChangeEditButton(GridDataItem item, bool visible)
        {
            if (item != null && item["Edit"].Controls.Count > 0)
            {
                LinkButton gridLinkButton = (LinkButton)item["Edit"].Controls[0];
                gridLinkButton.Visible = visible;
            }
        }
        protected void NumberMatrixRadGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                ChangeEditButton(item, false);
                var isAssigned = Convert.ToBoolean(item.GetDataKeyValue("Assigned").ToString());
                var checkBox = item.FindControl("chkReserved") as CheckBox;
                if (checkBox != null)
                {
                    if (isAssigned)
                    {
                        checkBox.Enabled = false;
                        checkBox.Visible = false;
                    }
                    else
                    {
                        checkBox.Enabled = true;
                        checkBox.Visible = true;
                    }
                }
            }
        }
        private void BindSkypeForBusinessNumberMatrixData()
        {
            listNumberMatrixData = GetData();
            NumberMatrixRadGrid.DataSource = listNumberMatrixData;
        }

        private List<Dto.SkypeForBusinessUnAssignedNumberMatrix> GetData()
        {
            Dictionary<string, string> paramerters = new Dictionary<string, string>();
            paramerters.Add("endpointId", this.EndpointSystemId.ToString());
            var sess = new SessionStatusImpl();
            paramerters.Add("currentUser", sess.User.UserName);
            listNumberMatrixData = DomainApiClient<Dto.SkypeForBusinessUnAssignedNumberMatrix>.GetByAction<List<Dto.SkypeForBusinessUnAssignedNumberMatrix>>(DomainApiAction.Endpoint.SkypeForBusiness.GetSFBExtensionMatrixData, paramerters);
            return listNumberMatrixData;
        }

        protected void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            var x = sender as CheckBox;
            if (x != null)
            {
                bool checkStatus = x.Checked;
                foreach (GridDataItem item in NumberMatrixRadGrid.MasterTableView.Items)
                {
                    CheckBox chkbx = item.FindControl("chkReserved") as CheckBox;
                    if (chkbx != null && chkbx.Visible && chkbx.Enabled)
                    {
                        chkbx.Checked = checkStatus;
                        item.Selected = chkbx.Checked;
                    }
                }

            }
        }
    }
}