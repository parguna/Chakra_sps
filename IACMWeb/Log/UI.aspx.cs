using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System.Text.RegularExpressions;
using DI.Chakra.Dto.Core;

namespace DI.Chakra.Web.Log
{
    public partial class UI : UIBasePage
    {
        private static IList<UILog> logs = new List<UILog>();
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
        }

        public void InitializeControl()
        {
            PresentationGridPagedData<UILog> data = GetUILogData();
            grdUILogs.MasterTableView.VirtualItemCount = data.TotalCount;
            grdUILogs.DataSource = data.Data;
        }

        private PresentationGridPagedData<UILog> GetUILogData(bool isExport = false)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            if (isExport == false)
            {
                parameter.Add("pagesize", grdUILogs.MasterTableView.PageSize.ToString());
                parameter.Add("pagenumber", (grdUILogs.CurrentPageIndex + 1).ToString());
            }
            else
            {
                parameter.Add("pagesize", null);
                parameter.Add("pagenumber", null);
            }
            string sortingFieldName = "";
            string sortOrder = "";
            string sortExpressions = string.Empty;
            if (grdUILogs.MasterTableView.SortExpressions.Count > 0)
            {
                sortingFieldName = grdUILogs.MasterTableView.SortExpressions[0].FieldName;
                sortOrder = grdUILogs.MasterTableView.SortExpressions[0].SortOrderAsString();
                foreach (GridSortExpression item in grdUILogs.MasterTableView.SortExpressions)
                {
                    sortExpressions += string.Format("{0} {1},", item.FieldName, item.SortOrderAsString());
                }
            }

            var filterExpression = grdUILogs.MasterTableView.FilterExpression;

            if (filterExpression.Contains("DateTime.Parse"))
            {
                List<string> finalOutput = new List<string>();
                foreach (string condition in filterExpression.Split(new string[] { " AND " }, StringSplitOptions.None))
                {
                    string rawExpression = condition;
                    var findMatch = Regex.Match(rawExpression, "DateTime.Parse(.*)", RegexOptions.IgnoreCase);
                    if (findMatch.Success)
                    {
                        var dtExtract = DateTime.Parse(findMatch.Value.Split('\"')[1]);
                        var conditionDate = string.Format("DateTime({0}, {1}, {2})", dtExtract.Year, dtExtract.Month, dtExtract.Day);
                        finalOutput.Add(Regex.Replace(rawExpression, "DateTime.Parse(.*)", string.Format("{0})", conditionDate), RegexOptions.IgnoreCase));
                    }
                    else
                        finalOutput.Add(rawExpression);
                }

                filterExpression = string.Join(" AND ", finalOutput);
            }

            parameter.Add("FilterExpression", filterExpression);
            parameter.Add("sortExpression", sortExpressions.Trim(',').Trim());

            var data = DomainApiClient<UILog>.GetByAction<PresentationGridPagedData<Dto.UILog>>(DomainApiAction.Log.UILog.GetAllOrderByLogDate, parameter);
            
            return data;
        }

        protected void grdUser_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            InitializeControl();
            grdUILogs.Visible = true;
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                // Commenting the below lines as a temp fix for pagesize over-riding issue (#507)
                grdUILogs.Export<IList<UILog>>(logs, e.ActionType, "UILog", false);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                var data = GetUILogData(true);
                grdUILogs.Export<IList<UILog>>(data.Data, e.ActionType, "UILog");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }


        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            grdUILogs.Rebind();
        }

        protected void grdUILogs_ItemDataBound(object sender, GridItemEventArgs e)
        {
            //if (e.Item is GridDataItem)
            //{
            //    GridDataItem dataItem = e.Item as GridDataItem;
            //    Label UserName = (Label)dataItem.FindControl("UserName");
            //    long UserId = dataItem.GetDataKeyValue("UserId") != null ? Convert.ToInt64(dataItem.GetDataKeyValue("UserId")) : 0;
            //    var user = DomainApiClient<Dto.User>.GetById(UserId);
            //    if (user != null && UserName != null)
            //    {
            //        UserName.Text = user.UserName;
            //    }
            //}
        }
    }
}