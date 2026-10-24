using DI.Chakra.Dto.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Web.PUB.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {

        
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            //HttpContext.Current.Session["ExportStatus"] = "true";
        }

        #region Events

        protected void Users_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            int PageIndex = grdPUBFiles.CurrentPageIndex;
            int PageSize = grdPUBFiles.MasterTableView.PageSize;
            PresentationGridPagedData<Dto.PUBFile> list = getData(PageIndex, PageSize);
            grdPUBFiles.VirtualItemCount = (int)list.TotalCount;
            grdPUBFiles.DataSource = list.Data;
        }

          PresentationGridPagedData<Dto.PUBFile> getData(int PageIndex, int PageSize)
        {
            string sortingFieldName = "";
            string sortOrder = "";
            string sortExpressions = string.Empty;
            if (grdPUBFiles.MasterTableView.SortExpressions.Count > 0)
            {
                sortingFieldName = grdPUBFiles.MasterTableView.SortExpressions[0].FieldName;
                sortOrder = grdPUBFiles.MasterTableView.SortExpressions[0].SortOrderAsString();
                foreach (GridSortExpression item in grdPUBFiles.MasterTableView.SortExpressions)
                {
                    sortExpressions += string.Format("{0} {1},", item.FieldName, item.SortOrderAsString());
                }
                sortExpressions = sortExpressions.Trim(',').Trim();
            }

            var filterExpression = grdPUBFiles.MasterTableView.FilterExpression;

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

            int startRowIndex = PageIndex * PageSize;

            PresentationGridPagedDataParameter param = new PresentationGridPagedDataParameter
            {
                FullTimeEmp = false,
                StartRowIndex = startRowIndex,
                MaximumRows = PageSize,
                FilterExpression = filterExpression,
                SortExpression = sortExpressions.Trim(',').Trim()
            };

            var list = DomainApiClient<Dto.PUBFile>
             .GetByAction<Dto.Core.PresentationGridPagedData<Dto.PUBFile>>(DomainApiAction.PUB.GetLazyLoadPUBFiles, null, param);
            return list;
        }

        protected void Users_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            GridDataItem g = grdPUBFiles.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");

        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                grdPUBFiles.ExportSettings.FileName = "PUBFiles";
                grdPUBFiles.Export(grdPUBFiles, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                PresentationGridPagedData<Dto.PUBFile> list = getData(0, -1);

                grdPUBFiles.Export<IList<Dto.PUBFile>>(list.Data, e.ActionType, "Endpoint");
            }


        }

        protected void Users_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if(e.CommandName == "FileDonwload")
            {
                 
                lnkEmployeeId_Command(e.CommandSource,new CommandEventArgs("FileDonwload", e.CommandArgument));
            }
            if (e.CommandName == "ShowInfo")
            {
                //GridDataItem item = e.Item as GridDataItem;
                //long UserId = (long)item.GetDataKeyValue("Id");
                //Dictionary<string, string> parameter = new Dictionary<string, string>();
                //parameter.Add("id", UserId.ToString());
                //User User = DomainApiClient<Dto.User>.GetByAction<User>
                //    (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, parameter);
                //if (User == null)
                //    return;
                //UserDetailUC.User = User;
                //UserDetail.VisibleOnPageLoad = true;
            }
            if (e.CommandName == RadGrid.FilterCommandName)
            {
                //SessionStatusImpl session = new SessionStatusImpl();
                //System.Web.UI.Pair filterPair = (System.Web.UI.Pair)e.CommandArgument;

                //string operand = filterPair.First.ToString();
                //string columnName = filterPair.Second.ToString();
                //string filterValue = string.Empty;
                //if (!operand.Equals("NoFilter"))
                //{
                //    filterValue = Users.MasterTableView.GetColumn(filterPair.Second.ToString()).CurrentFilterValue;
                //    UserGridFilterSettings filter = new UserGridFilterSettings();
                //    filter.UserId = session.User.Id;
                //    filter.GridID = Users.ID;
                //    filter.Operand = operand;
                //    filter.ColumnName = columnName;
                //    filter.FilterValue = filterValue;
                //    filter.PagePath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
                //    filter.CreatedBy = session.User.UserName;
                //    filter.CreatedDate = DateTime.Now;
                //    filter.LastModifiedBy = session.User.UserName;
                //    filter.LastModifiedDate = DateTime.Now;
                //    SaveGridFilter(filter);
            }
            else
            {
                //UserGridFilterSettings filter = new UserGridFilterSettings();
                //filter.UserId = session.User.Id;
                //filter.GridID = Users.ID;
                //filter.ColumnName = columnName;
                //filter.PagePath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
                //DeleteGridFilter(filter);
            }
        }

        protected void lnkEmployeeId_Command(object sender, CommandEventArgs e)
        {
            try
            {
                HttpContext.Current.Session["ExportStatus"] = "true";

                string fileName = string.Empty;//, contentType;
                Dto.Attachment attachment = null;
                var linkButn = sender as LinkButton;
                long id = 0;

                if (e != null)
                {
                    id = Convert.ToInt64(e.CommandArgument);

                }

                fileName = linkButn.Text; 
                 

                if (id > 0 && !string.IsNullOrWhiteSpace(fileName))
                {


                    var attachments = DomainApiClient<Dto.Attachment>.GetByAction<IList<Dto.Attachment>>(DomainApiAction.Attachment.GetByRequestId,
                        new Dictionary<string, string> { { "requestId", id.ToString() },
                                                    { "source", fileName }
                                                       });

                    if (attachments != null && attachments.Count() > 0)
                        attachment = attachments.FirstOrDefault();
                }



                if (attachment != null)
                {

                    HttpContext.Current.Session["ExportStatus"] = "true";
                    //ScriptManager.RegisterStartupScript(this, typeof(Page), "closeProgress", "StopProgressBarOnExport()",true);
                    HttpResponse response = Response;
                    response.Clear();
                    response.ClearContent();
                    response.ClearHeaders();
                    response.Buffer = true;
                    response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    response.AddHeader("Content-Length", attachment.AttachmentFile.Length.ToString());
                    response.AddHeader("Content-Disposition", $"attachment;filename={fileName}.csv");
                    response.BinaryWrite(attachment.AttachmentFile);
                    response.End();
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
            }
            finally
            {
            }
        }
            

        #endregion


        public void Refresh()
        {
            this.grdPUBFiles.Rebind();
        }

     
    }
}