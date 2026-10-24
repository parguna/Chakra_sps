using DI.Chakra.Web.Report.MySchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
 

namespace DI.Chakra.Web.Report.CustomReport
{
    public partial class CustomReport : ReportBasePage
    {
        private static List<Dto.CustomReport> list = new List<Dto.CustomReport>();
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void UpdatePanelDownload_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
     .Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void RadReportJobs_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            list = DomainApiClient<Dto.CustomReport>.
                   GetByAction<List<Dto.CustomReport>>("GetAllRunReports", null).ToList();
            this.RadReportJobs.DataSource = list;
        }

        protected void RadReportJobs_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = RadReportJobs.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }

        private void ShowDocument(string fileName, byte[] fileContent)
        {
            HttpResponse Response = HttpContext.Current.Response;
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AppendHeader("Content-Disposition", "attachment;filename=" + fileName + ".xlsx");
            Response.Buffer = true;
            Response.AddHeader("Content-Length", fileContent.Length.ToString());
            Response.BinaryWrite(fileContent);
            Response.End();
        }

        protected void RadReportJobs_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "file")
            {
                GridDataItem ditem = (GridDataItem)e.Item;
                this.ContextObjectId = (long)ditem.GetDataKeyValue("Id");

                Dto.CustomReport custumReport = DomainApiClient<Dto.CustomReport>.GetById(ContextObjectId.Value);

                if (custumReport != null)
                    ShowDocument(custumReport.FileName, custumReport.Attachment);
            }
        }
    }
}