using System;
using Application;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;
using DI.Chakra.Logger;
using DI.Chakra.Dto.Core;
using System.Web;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.AdvanceDelegation.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        #region Members
        private SessionStatusImpl SessionUser = new SessionStatusImpl();
        private static List<Dto.Presentation.PresentationTaskdelegation> list;
        public bool? IsSelectedRecordInActive
        {
            get
            {
                if (ViewState["IsRecordInActive"] == null)
                    return null;

                return (bool)ViewState["IsRecordInActive"];
            }
            set
            {
                ViewState["IsRecordInActive"] = value;
            }
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            DisableMasterRedirectToHandleAddEditAction();
            this.MasterPage.ActionClicked += ActionClicked;
        }

        protected void AdvaceDelegateGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            SetDataSource();
        }

        private void SetDataSource()
        {
            try
            {
                int PageIndex = AdvaceDelegateGrid.CurrentPageIndex;
                int PageSize = AdvaceDelegateGrid.MasterTableView.PageSize;
                int startRowIndex = PageIndex * PageSize;
                Dictionary<string, string> parameter = new Dictionary<string, string>();

                parameter.Add("actionUser", SessionUser.UserId.ToString());
                parameter.Add("isAdmin", (SessionUser.isAdminGroup || SessionUser.isIACMAdminGroup).ToString());
                parameter.Add("includeInActive", chkIncludeInactive.Checked ? bool.TrueString : bool.FalseString);

                PresentationGridPagedDataParameter param = new PresentationGridPagedDataParameter
                {
                    FullTimeEmp = true,
                    StartRowIndex = startRowIndex,
                    MaximumRows = PageSize,
                    FilterExpression = GetFilterExpression(AdvaceDelegateGrid.MasterTableView.FilterExpression),
                    SortExpression = GetSortExpression(AdvaceDelegateGrid.MasterTableView.SortExpressions),
                };
                var response = DomainApiClient<Dto.Presentation.PresentationTaskdelegation>.GetByAction<PresentationGridPagedData<Dto.Presentation.PresentationTaskdelegation>>
                (DomainApiAction.Tasks.PresentationTaskdelegation.GetPagedAllDelegationByActionUser, parameter, param);
                AdvaceDelegateGrid.VirtualItemCount = response.TotalCount;
                AdvaceDelegateGrid.DataSource = response.Data;
            }
            catch (Exception ex)
            {
                ShowAlertMessage(AlertType.Error, "Error while getting data.");
            }
        }

        protected void AdvaceDelegateGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = AdvaceDelegateGrid.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("DelegationId");
            this.IsSelectedRecordInActive= (g.FindControl("Label3") as Label).Text.ToLower() == "no";
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                if (!ContextObjectId.HasValue)
                    return;
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Delegation - Delete");
                DomainApiClient<Dto.TaskDelegation>.DeleteById(this.ContextObjectId.Value);
                this.ContextObjectId = null;
                AdvaceDelegateGrid.Rebind();
                MasterPage.ShowAlert(AlertType.Success, "Delegation deleted successfully");
            }

            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                ExportAll(e.ActionType);
                //AdvaceDelegateGrid.ExportSettings.FileName = "TaskDelegation";
                //ExportNew(AdvaceDelegateGrid, e.ActionType, "TaskDelegation");
            }
            else if (e.ActionType == ACLAction.Add)
            {
                this.MasterPage.RedirectTo(((ImageButton)sender).CommandArgument, e.ActionType);
            }
            else if (e.ActionType == ACLAction.Edit)
            {
                if (this.ContextObjectId != null && this.chkIncludeInactive.Checked && (IsSelectedRecordInActive??false))
                {
                    MasterPage.ShowAlert(AlertType.Error, "Inactive record can not be modified");
                    return;
                }
                this.MasterPage.RedirectTo(((ImageButton)sender).CommandArgument, e.ActionType);
            }

            //else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            //{
            //    AdvaceDelegateGrid.ExportSettings.FileName = "TaskDelegation";
            //    ExportNew(AdvaceDelegateGrid, e.ActionType, "TaskDelegation");
            //}
        }
        public void ExportAll(ACLAction actionType)
        {
            PresentationGridPagedDataParameter param = new PresentationGridPagedDataParameter
            {
                FullTimeEmp = true,
                StartRowIndex = 0,
                MaximumRows = -1,
                FilterExpression = GetFilterExpression(AdvaceDelegateGrid.MasterTableView.FilterExpression),
                SortExpression = GetSortExpression(AdvaceDelegateGrid.MasterTableView.SortExpressions),
                FromDate = null,
                ToDate = null
            };
            var fileType = (actionType == ACLAction.Excel || actionType == ACLAction.ExcelAll) ? "xlsx" : "csv";
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("actionUser", SessionUser.UserId.ToString());
            parameter.Add("isAdmin", (SessionUser.isAdminGroup || SessionUser.isIACMAdminGroup).ToString());
            parameter.Add("exportFileType", fileType);
            parameter.Add("includeInActive", chkIncludeInactive.Checked ? bool.TrueString : bool.FalseString);
            Session["ExportStatus"] = "false";
            var data = DomainApiClient<Dto.Presentation.PresentationTaskdelegation>.GetByAction<byte[]>(DomainApiAction.Tasks.PresentationTaskdelegation.DownloadDelegation, parameter, param);
            if (data != null)
            {
                Session["ExportStatus"] = "true";
                HttpResponse response = Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                response.AddHeader("Content-Length", data.Length.ToString());
                response.AddHeader("Content-Disposition", "attachment;filename=" + $"AdvanceDelegation_{DateTime.Now.ToString(Constants.Report_Date_Format)}.{fileType}");
                response.BinaryWrite(data);
                response.End();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }
        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }
        protected void AdvaceDelegateGrid_GridExporting(object source, GridExportingArgs e)
        {
            if (e.ExportType == ExportType.Excel)
            {
                string css = "<style type='text/css'> br { mso-data-placement: same-cell; } </style>";
                e.ExportOutput = e.ExportOutput.Replace("</head>", css + "</head>");
            }
        }
        #endregion

        protected void AdvaceDelegateGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = e.Item as GridDataItem;
                item["DelegationComments"].Text = item["DelegationComments"].Text.Replace(Environment.NewLine, "<br />");
            }
        }

        protected void chkIncludeInactive_CheckedChanged(object sender, EventArgs e)
        {
            DisableMasterRedirectToHandleAddEditAction();
            SetDataSource();
            AdvaceDelegateGrid.DataBind();
        }

        private void DisableMasterRedirectToHandleAddEditAction()
        {
            this.MasterPage.DisableRedirect = this.chkIncludeInactive.Checked;
        }
       
    }
}