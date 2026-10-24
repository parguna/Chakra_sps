using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.DataManagement.SkypeForBusiness.NumberRange.UC
{
    public partial class ListUC : DI.Chakra.Web.UIBaseUC, IAccessControl
    {
        const string MatrixName = "NumberRange Matrix";
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
                if(ViewState["EndpointSystemId"] != null)
                    return Convert.ToInt64(ViewState["EndpointSystemId"]);
                return 0;
            }
            set
            {
                ViewState["EndpointSystemId"] = value;
            }

        }
        static GridEditableItem selectedDataItem = null;
        private static IList<Dto.SkypeForBusinessUnAssignedNumber> listSkypeForBusinessUnAssignedNumber;
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
                NumberRangesRadGrid.DataBind();
            }
        }

        protected void NumberRangesRadGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            BindSkypeForBusinessUnAssignedNumber();
        }
        private void BindSkypeForBusinessUnAssignedNumber()
        {
            listSkypeForBusinessUnAssignedNumber = DomainApiClient<Dto.SkypeForBusinessUnAssignedNumber>.GetAll();
            NumberRangesRadGrid.DataSource = listSkypeForBusinessUnAssignedNumber;
        }
        protected void NumberRangesRadGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem Grid = NumberRangesRadGrid.SelectedItems[0] as GridDataItem;
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
            if (e.ActionType == Infrastructure.Enums.ACLAction.Add)
            {
                NumberRangesRadGrid.MasterTableView.InsertItem();
                NumberRangesRadGrid.Rebind();
                selectedDataItem = NumberRangesRadGrid.MasterTableView.GetInsertItem();
                IsEdited = false;

            }
            else if (e.ActionType == Infrastructure.Enums.ACLAction.Edit)
            {

                if (NumberRangesRadGrid.SelectedItems.Count > 0)
                {
                    selectedDataItem = NumberRangesRadGrid.SelectedItems[0] as GridEditableItem;
                    selectedDataItem.Edit = true;
                    NumberRangesRadGrid.Rebind();
                }
                IsEdited = true;
            }
            else if (e.ActionType == Infrastructure.Enums.ACLAction.Save)
            {
                if (selectedDataItem != null)
                {
                    AddOrUpdate();
                }
            }
            else if (e.ActionType == Infrastructure.Enums.ACLAction.Excel || e.ActionType == Infrastructure.Enums.ACLAction.ExcelAll)
            {
                NumberRangesRadGrid.ExportSettings.FileName = $"SkypeForBusinessExtensionRanges{DateTime.Now.ToString(Constants.Report_Date_Format)}";
                NumberRangesRadGrid.Export(NumberRangesRadGrid, e.ActionType);
            }
            else if (e.ActionType == Infrastructure.Enums.ACLAction.CSV || e.ActionType == Infrastructure.Enums.ACLAction.CSVAll)
            {
                NumberRangesRadGrid.ExportSettings.FileName = $"SkypeForBusinessExtensionRanges{DateTime.Now.ToString(Constants.Report_Date_Format)}";
                NumberRangesRadGrid.Export(NumberRangesRadGrid, e.ActionType);
            }
            if (e.ActionType == ACLAction.Delete)
            {
                Delete();
            }
        }

        private void Delete()
        {
            if (!ContextObjectId.HasValue || ContextObjectId.Value == 0)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item.");
                return;
            }
            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("endpointId", EndpointSystemId.ToString());
                param.Add("Id", ContextObjectId.Value.ToString());
                var canDelete = DomainApiClient<Dto.SkypeForBusinessUnAssignedNumber>.GetByAction<bool>(DomainApiAction.Endpoint.SkypeForBusiness.CheckNumberGeneratedAndAssigned, param);
                if (canDelete)
                {
                    UpdatedUILog<Dto.SkypeForBusinessUnAssignedNumber>(ContextObjectId.Value, MatrixName);
                    var status = DomainApiClient<Dto.SkypeForBusinessUnAssignedNumber>.GetByAction<bool>(DomainApiAction.Endpoint.SkypeForBusiness.DeleteNumberRangeAndMatrix, param);
                    if (status)
                    {
                        MasterPage.ShowAlert(AlertType.Success, "Number Range deleted successfully.");
                        NumberRangesRadGrid.Rebind();
                    }
                    else
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Error occured while deleting SkypeForBusiness NumberRange.");
                    }
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Error, "SkypeForBusiness NumberRange cannot be deleted.");
                }
            }
            catch (Exception)
            {
                MasterPage.ShowAlert(AlertType.Error, "SkypeForBusiness NumberRange cannot be deleted.");
            }
        }

        private void AddOrUpdate()
        {
            bool isEditMode = false;
            var editableItem = selectedDataItem;
            Dto.SkypeForBusinessUnAssignedNumber data = null;
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
            string validationMessage = string.Empty;
            if (id > 0)
            {
                isEditMode = true;
                GridEditableItem item = NumberRangesRadGrid.EditItems[0] as GridEditableItem;
                data = PopulateData(item, id, out validationMessage);
                successMessage = "Record updated successfully.";
            }
            else
            {
                
                editableItem = NumberRangesRadGrid.MasterTableView.GetInsertItem();
                data = PopulateData(editableItem, id, out validationMessage);
                successMessage = "Record inserted successfully.";
            }

            if (!string.IsNullOrWhiteSpace(validationMessage))
            {
                MasterPage.ShowAlert(Infrastructure.AlertType.Error, validationMessage);
                return;
            }
            if(data != null)
            {
                oldData = GetSerializeString<Dto.SkypeForBusinessUnAssignedNumber>(id);
                Dictionary<string, string> parameterProcess = new Dictionary<string, string>();
                DomainApiClient<Dto.SkypeForBusinessUnAssignedNumber>.Save(data);
                if (!isEditMode)
                {
                    MasterPage.ShowAlert(Infrastructure.AlertType.Success, "Record inserted successfully");
                    NumberRangesRadGrid.MasterTableView.IsItemInserted = false;
                }
                else
                {
                    UpdatedUILog<Dto.SkypeForBusinessUnAssignedNumber>(id,MatrixName,data: oldData);
                    MasterPage.ShowAlert(Infrastructure.AlertType.Success, "Record updated successfully");
                    NumberRangesRadGrid.MasterTableView.ClearEditItems();
                    this.ContextObjectId = null;
                }
                NumberRangesRadGrid.Rebind();
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
        protected void NumberRangesRadGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                ChangeEditButton(item, false);
            }
            //if (e.Item is GridEditableItem )
            //{
            //    GridEditableItem editItem = e.Item as GridEditableItem;
            //    int pkId = 0;
            //    int.TryParse(editItem.GetDataKeyValue("Id").ToString(), out pkId);
            //    LinkButton updateButton = (LinkButton)editItem.FindControl("UpdateButton");
            //    if(updateButton != null)
            //    {
            //        updateButton.Visible = false;
            //    }
            //}
            //else if(e.Item is GridDataInsertItem)
            //{
            //    GridDataInsertItem insertItem = e.Item as GridDataInsertItem;
            //    LinkButton insertButton = (LinkButton)insertItem.FindControl("InsertButton");
            //    if (insertButton != null)
            //    {
            //        insertButton.Visible = false;
            //    }
            //}
        }
        private Dto.SkypeForBusinessUnAssignedNumber PopulateData(GridEditableItem editableItem, long id, out string validationMsg)
        {
            validationMsg = string.Empty;
            var existingData = DomainApiClient<Dto.SkypeForBusinessUnAssignedNumber>.GetById(id);
            Hashtable values = new Hashtable();
            editableItem.ExtractValues(values);
            int sRange, eRange, priority;
            var startRange = (string)values["StartRange"];
            var endRange = (string)values["EndRange"];
            var sPriority = (string)values["Priority"];
            if (int.TryParse(startRange, out sRange) && sRange > 0)
            {
                if(sRange.ToString().Length != 4)
                {
                    validationMsg += "StartRange should be 4 Digit" + Environment.NewLine;
                }
            }
            else
            {
                validationMsg += "StartRange is mandatory and must be integer and greater than Zero"+Environment.NewLine;
            }
            if (int.TryParse(endRange, out eRange) && eRange > 0)
            {
                if (eRange.ToString().Length != 4)
                {
                    validationMsg += "EndRange should be 4 Digit" + Environment.NewLine;
                }
                
            }
            else
            {
                validationMsg += "EndRange is mandatory and must be integer and greater than Zero" + Environment.NewLine;
            }
            
            if (string.IsNullOrWhiteSpace(validationMsg))
            {
                if (sRange >=eRange || eRange <=sRange)
                {
                    validationMsg += "Start Range should be less than EndRange" + Environment.NewLine;
                    return existingData;
                }
                var numbers = DomainApiClient<Dto.SkypeForBusinessUnAssignedNumber>.GetAll();
                if(existingData != null)
                {
                    if (numbers.Any(i => id != existingData.Id &&  (i.StartRange >= sRange && i.EndRange <= sRange) ||(i.StartRange >= eRange && i.EndRange <= eRange) ))
                    {
                        validationMsg += "Number Range should not be overlap with existing records" + Environment.NewLine;
                        return existingData;
                    }
                }
                else
                {
                    if (numbers.Any(i => (i.StartRange <= eRange && i.EndRange >= eRange)))
                    {
                        validationMsg += "Number Range should not be overlap with existing records" + Environment.NewLine;
                        return existingData;
                    }
                }
               
                if (string.IsNullOrWhiteSpace(validationMsg))
                {
                    var userSession = new SessionStatusImpl()?.User?.UserName;
                    
                    if (existingData != null)
                    {
                        existingData.StartRange = sRange;
                        existingData.EndRange = eRange;
                        existingData.LastModifiedBy= userSession;
                        existingData.LastModifiedDate = DateTime.Now;
                    }
                    else
                    {
                       
                        existingData = new Dto.SkypeForBusinessUnAssignedNumber();
                        existingData.EndPointSystemId = this.EndpointSystemId;
                        existingData.StartRange = sRange;
                        existingData.EndRange = eRange;
                        existingData.Priority = numbers.Max(i=>i.Priority)+1;
                        existingData.CreatedDate = DateTime.Now;
                        existingData.CreatedBy = userSession;
                    }
                }
                    
            }
            return existingData;
        }

        protected void NumberRangesRadGrid_ItemCommand(object source,GridCommandEventArgs e)
        {
            if (NumberRangesRadGrid.MasterTableView.IsItemInserted)
            {
                e.Canceled= false;
                NumberRangesRadGrid.Rebind();
            }
           
        }
    }
}