using System;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Infrastructure;
using DI.Chakra.Dto.Core;
using System.Collections.Generic;
using System.Web;

namespace DI.Chakra.Web.Non_Employee
{
    public partial class Edit : DI.Chakra.Web.UIBasePage, IContextObjectRequired
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                EditForm.IsEdit = true;
                EditForm.RequestStatus = Convert.ToString(Request.QueryString["status"]);
                EditForm.Initialize();
                EmployeeAccountUC.EntityId = Convert.ToInt64(Request.QueryString["contextId"]);
                //EmployeeInactiveAccountsUC.EntityId = Convert.ToInt64(Request.QueryString["contextId"]);
            }
            EditForm.HideOrgChartLink();
            this.MasterPage.ActionClicked += ActionClicked;
        }

        private void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if(e.ActionType == ACLAction.Save)
            {
                bool isErrorOccured = EditForm.SaveUser();
                if (!isErrorOccured)
                {
                    MasterPage.ShowAlert(AlertType.Success, "Non Employee updated successfully");
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "PageScript", "Redirect();", true);
                }
            }else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                PresentationGridPagedDataParameter param = new PresentationGridPagedDataParameter();
                param.Ids = new List<long>() { EditForm.EmployeeUserId};
                param.StartRowIndex = 0;
                param.MaximumRows = -1;
                Session["ExportStatus"] = "false";
                var fileType = (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.ExcelAll) ? "xlsx" : "csv";
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("exportFileType", fileType);
                var data = DomainApiClient<Dto.User>.GetByAction<byte[]>("User", DomainApiAction.User.DownloadUserAccessDetail, parameter, param);
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
                    response.AddHeader("Content-Disposition", "attachment;filename=" + string.Format("{2}_{1}.{0}", fileType, DateTime.Now.ToString(Constants.Report_Date_Format), EditForm?.User?.DisplayName));
                    response.BinaryWrite(data);
                    response.End();
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Warning, "No records found for export.");
                }
            }
            
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/NonEmployee/List.aspx");
        }
    }
}