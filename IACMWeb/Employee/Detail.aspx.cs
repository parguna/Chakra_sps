using DI.Chakra.Dto.Core;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Web;

namespace DI.Chakra.Web.Employee
{
    public partial class Detail : UIBasePage, IContextObjectRequired
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!IsPostBack)
            {
                if (Session != null && ContextObjectId != null)
                {
                    EmployeeAccountUC.EntityId = Convert.ToInt64(ContextObjectId);
                    //EmployeeInactiveAccountsUC.EntityId = Convert.ToInt64(ContextObjectId);
                }
            }
        }
        private void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (ContextObjectId.HasValue && ContextObjectId.Value > 0)
            {
                var usr = DomainApiClient<Dto.User>.GetById(ContextObjectId.Value);
                if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
                {
                    PresentationGridPagedDataParameter param = new PresentationGridPagedDataParameter();
                    param.Ids = new List<long>() { ContextObjectId.Value };
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
                        response.AddHeader("Content-Disposition", "attachment;filename=" + string.Format("{2}_{1}.{0}", fileType, DateTime.Now.ToString(Constants.Report_Date_Format), usr.DisplayName));
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
            
        }
    }
}