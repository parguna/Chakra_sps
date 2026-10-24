using Application;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Core;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Web.Master;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Employee.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        private static List<Dto.Presentation.PresentationUser> list = new List<Dto.Presentation.PresentationUser>();
        private static int totalCount = 0;
        #region Events
        protected void Users_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            int PageIndex = Users.CurrentPageIndex;
            int PageSize = Users.MasterTableView.PageSize;
            string sortingFieldName = "";
            string sortOrder = "";
            string sortExpressions = string.Empty;
            if (Users.MasterTableView.SortExpressions.Count > 0)
            {
                sortingFieldName = Users.MasterTableView.SortExpressions[0].FieldName;
                sortOrder = Users.MasterTableView.SortExpressions[0].SortOrderAsString();
                foreach (GridSortExpression item in Users.MasterTableView.SortExpressions)
                {
                    sortExpressions += string.Format("{0} {1},", item.FieldName, item.SortOrderAsString());
                }
                sortExpressions = sortExpressions.Trim(',').Trim();
            }

            var filterExpression = Users.MasterTableView.FilterExpression;

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

            SessionStatusImpl session = new SessionStatusImpl();
            int startRowIndex = PageIndex * PageSize;

            PresentationGridPagedDataParameter param = new PresentationGridPagedDataParameter
            {
                FullTimeEmp = true,
                StartRowIndex = startRowIndex,
                MaximumRows = PageSize,
                FilterExpression = filterExpression,
                SortExpression = sortExpressions.Trim(',').Trim()
            };
            var usrList = DomainApiClient<Dto.Presentation.PresentationUser>
             .GetByAction<Dto.Core.PresentationGridPagedData<Dto.Presentation.PresentationUser>>(DomainApiAction.Endpoint.Presentation.PresentationUser.GetLazyLoadFullTimeEmployees,null, param);
            Users.VirtualItemCount = (int)usrList.TotalCount;
            totalCount = Users.VirtualItemCount;
            Users.DataSource = usrList.Data;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            //Users.Rebind();
        }


        protected void Users_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            GridDataItem g = Users.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }

        protected void Button1_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                Session["ExportStatus"] = "false";
                var fileType = (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.ExcelAll) ? "xlsx" : "csv";
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("exportFileType", fileType);
                PresentationGridPagedDataParameter param = new PresentationGridPagedDataParameter
                {
                    FullTimeEmp = true,
                    StartRowIndex = 0,
                    MaximumRows = -1,
                    FilterExpression = GetFilterExpression(Users.MasterTableView.FilterExpression),
                    SortExpression = GetSortExpression(Users.MasterTableView.SortExpressions)
                };
                var data = DomainApiClient<Dto.Presentation.PresentationUser>
                    .GetByAction<byte[]>(DomainApiAction.Endpoint.Presentation.PresentationUser.DownloadFullTimeEmployees, parameter, param);
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
                    response.AddHeader("Content-Disposition", "attachment;filename=" + string.Format("Employee.{0}", fileType));
                    response.BinaryWrite(data);
                    response.End();
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                    
                }
            }
        }

        protected void Users_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "ShowInfo")
            {
                GridDataItem item = e.Item as GridDataItem;
                long UserId = (long)item.GetDataKeyValue("Id");
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", UserId.ToString());
                User User = DomainApiClient<Dto.User>.GetByAction<User>
                    (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, parameter);
                if (User == null)
                    return;
                NUserDetailUC.User = User;
                UserDetail.VisibleOnPageLoad = true;
            }
            if (e.CommandName == RadGrid.FilterCommandName)
            {
                SessionStatusImpl session = new SessionStatusImpl();
                System.Web.UI.Pair filterPair = (System.Web.UI.Pair)e.CommandArgument;

                string operand = filterPair.First.ToString();
                string columnName = filterPair.Second.ToString();
                string filterValue = string.Empty;
                if (!operand.Equals("NoFilter"))
                {
                    filterValue = Users.MasterTableView.GetColumn(filterPair.Second.ToString()).CurrentFilterValue;
                    UserGridFilterSettings filter = new UserGridFilterSettings();
                    filter.UserId = session.User.Id;
                    filter.GridID = Users.ID;
                    filter.Operand = operand;
                    filter.ColumnName = columnName;
                    filter.FilterValue = filterValue;
                    filter.PagePath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
                    filter.CreatedBy = session.User.UserName;
                    filter.CreatedDate = DateTime.Now;
                    filter.LastModifiedBy = session.User.UserName;
                    filter.LastModifiedDate = DateTime.Now;
                    SaveGridFilter(filter);
                }
                else
                {
                    UserGridFilterSettings filter = new UserGridFilterSettings();
                    filter.UserId = session.User.Id;
                    filter.GridID = Users.ID;
                    filter.ColumnName = columnName;
                    filter.PagePath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
                    DeleteGridFilter(filter);
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            UserDetail.VisibleOnPageLoad = false;
        }

        protected void lnkUsername_Command(object sender, CommandEventArgs e)
        {
            GridDataItem item = Users.Items[Convert.ToInt32(e.CommandArgument)];
            if (item == null)
                return;

            this.ContextObjectId = (long)item.GetDataKeyValue("Id");
            Response.Redirect($"Detail.aspx?contextId={this.ContextObjectId}");
        }
        #endregion
    }
}