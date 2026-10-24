using Application;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Core;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Triggers.UC
{
    public partial class ListUC : UIBaseUC
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack)
            {
                BindEmployeeEvents();
            }
            this.MasterPage.ActionClicked += ActionClicked;
        }
        private EmployeeEvents EmployeeEvent
        {
            get
            {
                return (EmployeeEvents)Convert.ToInt16(ddlEventType.SelectedValue);
            }
        }
        private static IList<PresentationEmployeeEventTrigger> employeeEventTriggerList;

        private void BindEmployeeEvents()
        {
            ddlEventType.Items.Clear();
            ddlEventType.Items.Insert(0, new ListItem("--Select--", "-1"));

            foreach (EmployeeEvents item in Enum.GetValues(typeof(EmployeeEvents)))
            {
                if (item == EmployeeEvents.None)
                    continue;

                var description = UtilityMethods.GetEnumAttribute<Description>(item);
                ListItem listItem = new ListItem(description, ((int)item).ToString());
                ddlEventType.Items.Add(listItem);
            }
        }
        protected void TriggersGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            SetDataSource();
        }

        void SetDataSource()
        {
            try
            {
                var parameters = new Dictionary<string, string>();
                if (ddlEventType.SelectedIndex <= 0)
                {
                    parameters.Add("eventType", ((int)EmployeeEvents.None).ToString());
                }
                else
                {                    
                    parameters.Add("eventType", EmployeeEvent.ToString());                    
                }
                parameters.Add("includeProcessed", rdBtnIncludeProcessed.Checked ? bool.TrueString: bool.FalseString);
 
                int PageIndex = TriggersGrid.CurrentPageIndex;
                int PageSize = TriggersGrid.MasterTableView.PageSize;
                int startRowIndex = PageIndex * PageSize;
                PresentationGridPagedDataParameter param = new PresentationGridPagedDataParameter
                {
                    StartRowIndex = startRowIndex,
                    MaximumRows = PageSize,
                    FilterExpression = GetFilterExpression(TriggersGrid.MasterTableView.FilterExpression),
                    SortExpression = GetSortExpression(TriggersGrid.MasterTableView.SortExpressions)
                };
                var data = DomainApiClient<PresentationEmployeeEventTrigger>.
                        GetByAction<Dto.Core.PresentationGridPagedData<Dto.Presentation.PresentationEmployeeEventTrigger>>
                        (DomainApiAction.Provisioning.Presentation.PresentationEmployeeEventTrigger.GetLazyLoadEmployeeEventTrigger, parameters, param);
                TriggersGrid.VirtualItemCount = data.TotalCount;
                TriggersGrid.DataSource = data.Data;
            }
            catch (Exception ex)
            {

            }
        }

        protected void TriggersGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = TriggersGrid.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("TriggerId");
        }
        protected override IDMSReportParam GetReportHeaderColumnMapping()
        {
            var param = new Dto.IDMSReportParam();
            param.ReportDataColumns = new List<Dto.IDMSReportDataColumn>();
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Trigger ID", "TriggerId", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Employee Name", "UserName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Employee Id", "EmployeeId", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Event Type", "EventType", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Is Request Created", "IsRequestCreated", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Is Admin Check Required", "IsAdminCheckRequired", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Is Active", "IsActive", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Effective Date", "EffectiveDate", "dd-MM-yyyy hh:mm:ss tt"));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Comments", "Comments", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Created Date", "CreatedDate", "dd-MM-yyyy hh:mm:ss tt"));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Created By", "CreatedBy", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("LastModified Date", "LastModifiedDate", "dd-MM-yyyy hh:mm:ss tt"));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("LastModified By", "LastModifiedBy", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Actual Event Date", "ActualEventDate", "dd-MM-yyyy hh:mm:ss tt"));
            return param;
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Trigger - Delete");
                DeleteTrigger();
            }
            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                var parameters = new Dictionary<string, string>();
                if (ddlEventType.SelectedIndex <= 0)
                {
                    parameters.Add("eventType", ((int)EmployeeEvents.None).ToString());
                }
                else
                {

                    parameters.Add("eventType", EmployeeEvent.ToString());

                }
                parameters.Add("includeProcessed", rdBtnIncludeProcessed.Checked ? bool.TrueString : bool.FalseString);
                PresentationGridPagedDataParameter param = new PresentationGridPagedDataParameter
                {
                    StartRowIndex =0,
                    MaximumRows = -1,
                    FilterExpression = GetFilterExpression(TriggersGrid.MasterTableView.FilterExpression),
                    SortExpression = GetSortExpression(TriggersGrid.MasterTableView.SortExpressions)
                };
                var result = DomainApiClient<PresentationEmployeeEventTrigger>.
                        GetByAction<Dto.Core.PresentationGridPagedData<Dto.Presentation.PresentationEmployeeEventTrigger>>
                        (DomainApiAction.Provisioning.Presentation.PresentationEmployeeEventTrigger.GetLazyLoadEmployeeEventTrigger, parameters, param);
                var data = result.Data;

                if (data != null && data.Count > 0)
                {
                    GenericExport(data, e.ActionType, "Triggers");
                }
            }
        }

        private void DeleteTrigger()
        {
            if (!ContextObjectId.HasValue)
                return;
            DomainApiClient<EmployeeEventTrigger>.DeleteById(this.ContextObjectId.Value);
            MasterPage.ShowAlert(AlertType.Success, "Trigger deleted successfully");
            TriggersGrid.Rebind();
            this.ContextObjectId = null;
        }

        protected void RefreshButton_Click(object sender, EventArgs e)
        {
            this.TriggersGrid.Rebind();
        }

        protected void TriggersGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            //if (e.Item is GridDataItem)
            //{
            //    GridDataItem item = (GridDataItem)e.Item;
            //    ImageButton image = (ImageButton)item.FindControl("AttachmentImage");
            //    if (item["HasAttachment"].Text == Boolean.TrueString)
            //    {
            //        image.Visible = true;
            //    }
            //    else
            //        image.Visible = false;

            //ImageButton amendImage = (ImageButton)item.FindControl("AmendAttachmentImage");
            //if (item["HasAmendAttachment"].Text == Boolean.TrueString)
            //{
            //    amendImage.Visible = true;
            //}
            //else
            //    amendImage.Visible = false;
            //}
        }

        protected void TriggersGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {

        }

        private void ShowDocument(string fileName, byte[] fileContent)
        {
            HttpResponse response = HttpContext.Current.Response;
            response.Clear();
            response.ClearContent();
            response.ClearHeaders();
            response.Buffer = true;
            response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
            response.BinaryWrite(fileContent);
            response.End();
        }

        protected void UpdatePanelDownload_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                               .Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void AmendUpdatePanelDownload_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                          .Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }
        protected void ddlEventType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDataSource();
            TriggersGrid.DataBind();
        }

        protected void lnkTriggerId_Command(object sender, CommandEventArgs e)
        {
            GridDataItem item = TriggersGrid.Items[Convert.ToInt32(e.CommandArgument)];
            if (item == null)
                return;

            this.ContextObjectId = (long)item.GetDataKeyValue("Id");
            Response.Redirect($"Edit.aspx?contextId={this.ContextObjectId}");
        }

        protected void chkIncludeProcessed_CheckedChanged(object sender, EventArgs e)
        {
            SetDataSource();
            TriggersGrid.DataBind();

        }
    }
}