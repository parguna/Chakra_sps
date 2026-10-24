using System;
using System.Collections.Generic;
using System.Web;
using Application;
using DI.Chakra.Dto.Core;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;

namespace DI.Chakra.Web.Profile
{
    public partial class Profile : UIBasePage, IContextObjectEnabled
    {
        #region Members

        private SessionStatusImpl oSession = new SessionStatusImpl();

        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += MasterPage_ActionClicked;
            if (!IsPostBack)
            {
                if (oSession != null)
                {
                    DetailUC.EntityId = oSession.UserId;
                    DetailUC.ShowAccess = true;
                    //EmployeeInactiveAccountsUC.EntityId = oSession.UserId;
                }
            }
        }

        private void MasterPage_ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                PresentationGridPagedDataParameter param = new PresentationGridPagedDataParameter();
                param.Ids = new List<long>() { oSession.UserId };
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
                    response.AddHeader("Content-Disposition", "attachment;filename=" + string.Format("{2}_{1}.{0}", fileType, DateTime.Now.ToString(Constants.Report_Date_Format), oSession.User?.DisplayName));
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
        protected void btncancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }
        #endregion

    }
}