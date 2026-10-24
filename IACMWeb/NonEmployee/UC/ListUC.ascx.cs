using Application;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Core;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Non_Employee.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        private static List<Dto.Presentation.PresentationUser> list = new List<Dto.Presentation.PresentationUser>();
        private static int totalCount = 0;
        private SessionStatusImpl SessionUser = new SessionStatusImpl();
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
                FullTimeEmp = false,
                StartRowIndex = startRowIndex,
                MaximumRows = PageSize,
                FilterExpression = filterExpression,
                SortExpression = sortExpressions.Trim(',').Trim()
            };

            string departmentCode = new SessionStatusImpl().User.DepartmentCode;

            List<Dto.PagePermission> pagePermissions = (List<Dto.PagePermission>)Session["PagePermissions"];
            bool showAllNonEmployees = !string.IsNullOrWhiteSpace(departmentCode) ? pagePermissions.Any(x => x.PageId == MasterPage.PageId && x.Permission.Name.ToLower() == "showallnonemployees") : true;

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("departmentCode", departmentCode);
            parameters.Add("showAllNonEmployees", showAllNonEmployees.ToString());

            var usrList = DomainApiClient<Dto.Presentation.PresentationUser>
             .GetByAction<Dto.Core.PresentationGridPagedData<Dto.Presentation.PresentationUser>>(DomainApiAction.Endpoint.Presentation.PresentationUser.GetLazyLoadFullTimeEmployees, parameters, param);
            Users.VirtualItemCount = (int)usrList.TotalCount;
            totalCount = Users.VirtualItemCount;
            Users.DataSource = usrList.Data;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
        }

        protected void Users_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            GridDataItem g = Users.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");

            this.MasterPage.DisableRedirect = g["Status"].Text == AccessRequestStatus.Requested.ToString();
        }
        protected void Button1_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                var fileType = (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.ExcelAll) ? "xlsx" : "csv";
                string departmentCode = new SessionStatusImpl().User.DepartmentCode;
                List<Dto.PagePermission> pagePermissions = (List<Dto.PagePermission>)Session["PagePermissions"];
                bool showAllNonEmployees = !string.IsNullOrWhiteSpace(departmentCode) ? pagePermissions.Any(x => x.PageId == MasterPage.PageId && x.Permission.Name.ToLower() == "showallnonemployees") : true;

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("fullTimeEmp", Boolean.FalseString);
                parameter.Add("startRowIndex", 0.ToString());
                parameter.Add("maximumRows", "-1");
                parameter.Add("FilterExpression", GetFilterExpression(Users.MasterTableView.FilterExpression));
                parameter.Add("sortExpression", GetSortExpression(Users.MasterTableView.SortExpressions));
                parameter.Add("exportFileType", fileType);
                parameter.Add("departmentCode", departmentCode);
                parameter.Add("showAllNonEmployees", showAllNonEmployees.ToString());

                var data = DomainApiClient<Dto.Presentation.PresentationUser>
                 .GetByAction<byte[]>(DomainApiAction.Endpoint.Presentation.PresentationUser.DownloadNonEmployees, parameter);

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
                    response.AddHeader("Content-Disposition", "attachment;filename=" + string.Format("NonEmployee.{0}", fileType));
                    response.BinaryWrite(data);
                    response.End();
                    HttpContext.Current.ApplicationInstance.CompleteRequest();

                }
            }
            else if (e.ActionType == ACLAction.ExtendNonEmployee)
            {
                if (Session != null && ContextObjectId != null)
                {
                    User user = DomainApiClient<Dto.User>.GetById(Convert.ToInt64(ContextObjectId));
                    joinDate.Text = user.JoinDate.Value.ToString("dd-MM-yyyy");
                    terminationDate.Text = user.TerminationDate.HasValue ? user.TerminationDate.Value.ToString("dd-MM-yyyy") : "";
                    DatePickerValidTo.MinDate = user.TerminationDate.HasValue ? user.TerminationDate.Value : DateTime.Now;
                    DatePickerValidTo.SelectedDate = user.TerminationDate.HasValue ? user.TerminationDate.Value : DateTime.Now;

                    ExtendEmployeeWin.VisibleOnPageLoad = true;
                    ExtendUC.EntityId = Convert.ToInt64(ContextObjectId);
                    ExtendUC.ShowAccess = false;
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Error, "Please select User first.");
                }
            }
            else if (e.ActionType == ACLAction.RehireNonEmployee)
            {
                if (Session != null && ContextObjectId != null)
                {
                    User user = DomainApiClient<Dto.User>.GetById(Convert.ToInt64(ContextObjectId));
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("userId", user.Id.ToString());
                    List<Dto.AccessRequest> accessRequestList = DomainApiClient<Dto.AccessRequest>.GetByAction<List<Dto.AccessRequest>>(DomainApiAction.Provisioning.AccessRequest.GetIDASRequestByUserId, parameter);
                    if (accessRequestList != null && accessRequestList.Count > 0)
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Already valid access request in place.");
                    }
                    else if (user.IsActive)
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Please select inactive User to rehire.");
                    }
                    else
                    {
                        Response.Redirect("~/NonEmployee/Rehire.aspx?contextId=" + ContextObjectId.ToString());
                    }

                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Error, "Please select User first.");
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
                UserDetailUC.User = User;
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
        #endregion

        protected void ExtendSave_Click(object sender, EventArgs e)
        {
            if (AsyncUploadAttach.UploadedFiles.Count > 0)
            {
                try
                {
                    User user = DomainApiClient<Dto.User>.GetById(Convert.ToInt64(ContextObjectId));
                    user.TerminationDate = DatePickerValidTo.SelectedDate;
                    DomainApiClient<Dto.User>.Save(user);
                    GetUserAttachment(user);

                    ExtendEmployeeWin.VisibleOnPageLoad = false;

                    var evnt = EmployeeEvents.Resignation;
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("userId", Convert.ToString(user.Id));
                    parameter.Add("evnt", evnt.ToString());

                    var trigger = DomainApiClient<Dto.EmployeeEventTrigger>.GetByAction<List<Dto.EmployeeEventTrigger>>
                        (DomainApiAction.Provisioning.EmployeeEventTrigger.GetTriggerToExecute, parameter);

                    if (trigger != null && trigger.Count > 0)
                    {
                        Dictionary<string, string> parameters1 = new Dictionary<string, string>();
                        parameters1.Add("currentUser", SessionUser.User.DisplayName);
                        trigger[0].EffectiveDate = user.TerminationDate.Value;
                        DomainApiClient<Dto.EmployeeEventTrigger>.GetByAction<bool>("UdpateEventTrigger", parameters1, trigger[0]);
                    }
                    else if (user.IsActive)
                    {
                        Dictionary<string, string> parameterEmpTri = new Dictionary<string, string>();
                        parameterEmpTri.Add("userId", Convert.ToString(user.Id));
                        parameterEmpTri.Add("evnt", evnt.ToString());
                        parameterEmpTri.Add("effectiveDate", user.TerminationDate.Value.ToString("dd-MM-yyyy"));
                        parameterEmpTri.Add("oldOrgCode", string.Empty);
                        parameterEmpTri.Add("newOrgCode", string.Empty);
                        parameterEmpTri.Add("newDesignationCode", string.Empty);
                        parameterEmpTri.Add("newGrade", string.Empty);

                        bool triggerCreated = DomainApiClient<EmployeeEventTrigger>.GetByAction<bool>
                            (DomainApiAction.Provisioning.EmployeeEventTrigger.CreateEventTrigger, parameterEmpTri);
                    }

                    MasterPage.ShowAlert(AlertType.Success, "User updated successfully.");
                }
                catch (Exception ex)
                {
                    MasterPage.ShowAlert(AlertType.Error, "Error while updating user, please contact Administrator.");
                }
            }
            else
            {
                MasterPage.ShowAlert(AlertType.Warning, "Please select Attachment");
            }
        }

        protected void ExtedCancel_Click(object sender, EventArgs e)
        {
            ExtendEmployeeWin.VisibleOnPageLoad = false;
        }
        private UserAttachment GetUserAttachment(User userobj)
        {
            if (AsyncUploadAttach.UploadedFiles.Count > 0)
            {
                Telerik.Web.UI.UploadedFile file = AsyncUploadAttach.UploadedFiles[0];
                UserAttachment attach = new UserAttachment();
                byte[] fileData = new byte[file.ContentLength];
                file.InputStream.Read(fileData, 0, Convert.ToInt32(file.ContentLength));
                attach.FileName = file.FileName;
                attach.AttachmentFile = fileData;
                attach.UserId = userobj.Id;
                attach.IsAmendAttachment = true;
                DomainApiClient<Dto.UserAttachment>.Save(attach);
                return attach;
            }
            else
            {
                return null;
            }
        }

        protected void lnkEmployeeId_Command(object sender, CommandEventArgs e)
        {
            GridDataItem item = Users.Items[Convert.ToInt32(e.CommandArgument)];
            if (item == null)
                return;

            this.ContextObjectId = (long)item.GetDataKeyValue("Id");
            User user = DomainApiClient<Dto.User>.GetById(ContextObjectId.Value);
            var status = user.RequestStatus.ToString();
            if (this.ContextObjectId > 0 && status.Equals("Requested", StringComparison.InvariantCultureIgnoreCase))
            {
                ShowAlertMessage(AlertType.Warning, "Non-Employee details cannot be updated as it is still pending for approval.");
            }
            else
            {
                Response.Redirect($"Edit.aspx?contextId={this.ContextObjectId}&status={status}");
            }

        }
    }
}