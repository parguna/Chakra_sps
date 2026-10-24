using Application;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Notification.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        private SessionStatusImpl SessionUser = new SessionStatusImpl();
        private static IList<Dto.Presentation.PresentationNotification> lst = new List<PresentationNotification>();
        #region Properties
        NotificationStatus NotificationStatus
        {
            get
            {
                return (NotificationStatus)Convert.ToByte(this.ddlStatus.SelectedValue);
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindStatus();
                radgridNotification.DataBind();
                // SetRadGridDefaultSelection();

            }
            this.MasterPage.ActionClicked += ActionClicked;
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                radgridNotification.ExportSettings.FileName = "MyNotification";
                radgridNotification.Export(radgridNotification, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                radgridNotification.Export<IList<Dto.Presentation.PresentationNotification>>(lst, e.ActionType, "MyNotification");
            }

        }

        private void BindStatus()
        {
            ddlStatus.Items.Clear();
            foreach (NotificationStatus status in Enum.GetValues(typeof(NotificationStatus)))
            {
                var description = UtilityMethods.GetEnumAttribute<Description>(status);
                ListItem item = new ListItem(description, Convert.ToInt16(status).ToString());
                ddlStatus.Items.Add(item);
            }

            ddlStatus.Items.FindByValue(Convert.ToInt16(NotificationStatus.UnRead).ToString()).Selected = true;
        }

        public void SetRadGridDefaultSelection()
        {
            radgridNotification.Items[0].Selected = true;
        }

        protected void radgridNotification_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            LoadGrid();

        }

        private void LoadGrid()
        {


            Dictionary<String, string> parameter = new Dictionary<String, string>();
            parameter.Add("userId", SessionUser.User.Id.ToString());
            parameter.Add("status", NotificationStatus.ToString());

            lst = DomainApiClient<Dto.Presentation.PresentationNotification>.GetByAction<List<Dto.Presentation.PresentationNotification>>
                (DomainApiAction.Notification.PresentationNotification.GetByUserIdStatus, parameter);

            radgridNotification.DataSource = lst;
        }


        protected void radgridNotification_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radgridNotification.SelectedItems.Count > 0)
            {
                GridDataItem g = radgridNotification.SelectedItems[0] as GridDataItem;
                this.ContextObjectId = (long)g.GetDataKeyValue("Id");
            }
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            radgridNotification.Rebind();
        }

        protected void radgridNotification_Init(object sender, EventArgs e)
        {
            GridFilterMenu menu = radgridNotification.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" || menu.Items[i].Text == "Contains" || menu.Items[i].Text == "EqualTo" ||
                    menu.Items[i].Text == "GreaterThan" || menu.Items[i].Text == "LessThan" || menu.Items[i].Text == "StartsWith" ||
                    menu.Items[i].Text == "EndsWith")
                {
                    i++;
                }
                else
                {
                    menu.Items.RemoveAt(i);
                }
            }
        }

        protected void radgridNotification_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "file")
            {
                GridDataItem ditem = (GridDataItem)e.Item;
                this.ContextObjectId = (long)ditem.GetDataKeyValue("Id");
                Dto.Notification parentNotification = DomainApiClient<Dto.Notification>.GetById(ContextObjectId.Value, true);
                if (parentNotification.NotificationAttachment != null && parentNotification.NotificationAttachment.Count > 0)
                    ShowDocument(parentNotification.NotificationAttachment[0].FileName, parentNotification.NotificationAttachment[0].Attachment, parentNotification.NotificationAttachment[0].ContentType);
            }
        }

        private void ShowDocument(string fileName, byte[] fileContent, string contenttype)
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

        protected void radgridNotification_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                ImageButton image = (ImageButton)item.FindControl("AttachmentImage");
                if (item["HasAttachments"].Text == Boolean.TrueString)
                {
                    image.Visible = true;
                }
                else
                    image.Visible = false;
            }
        }

        protected void UpdatePanelDownload_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
        .Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }


        //protected void Exploring_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        //{
        //    List<Dto.Notification> lst = new List<Dto.Notification>();
        //    Expression<Func<Dto.Notification, bool>> exp = null;

        //    ParameterExpression parameter = Expression.Parameter(typeof(Dto.Notification), "n");
        //    Expression body = null;

        //    foreach (GridColumn item in Exploring.MasterTableView.Columns)
        //    {
        //        if (item.CurrentFilterFunction != GridKnownFunction.NoFilter)
        //        {
        //            if (body == null)
        //                body = GetExpression(parameter, item.CurrentFilterFunction, item.UniqueName, item.CurrentFilterValue);
        //            else
        //                body = Expression.AndAlso(body, GetExpression(parameter, item.CurrentFilterFunction, item.UniqueName, item.CurrentFilterValue));

        //        }
        //    }

        //    GridSortExpressionCollection expressionCollections = Exploring.MasterTableView.SortExpressions;
        //    foreach (GridSortExpression expression in expressionCollections)
        //    {
        //       // GetOrderBy(expression.FieldName, expression.SortOrder.ToString());
        //    }

        //    if (body != null)
        //        exp = Expression.Lambda<Func<Dto.Notification, bool>>(body, parameter);

        //    //exp = p => p.Subject.Contains("not");
        //   // lst = DomainApiClient<Dto.Notification>.Get(exp, true, Exploring.MasterTableView.PageSize, Exploring.MasterTableView.CurrentPageIndex + 1);
        //    Exploring.DataSource = lst;
        //}

        //private Expression GetExpression(ParameterExpression parameter, GridKnownFunction filter, string uniqueName, string filterValue)
        //{
        //    MemberExpression member = Expression.Property(parameter, uniqueName);
        //    ConstantExpression constant = Expression.Constant(filterValue);
        //    Expression exp = GetBody(filter, member, constant);
        //    return exp;
        //}

        //private Expression GetBody(GridKnownFunction filter, MemberExpression member, ConstantExpression constant)
        //{
        //    MethodInfo containsMethod = typeof(string).GetMethod("Contains");
        //    MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        //    MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });

        //    switch (filter)
        //    {

        //        case GridKnownFunction.EqualTo:
        //            return Expression.Equal(member, constant);

        //        case GridKnownFunction.Contains:
        //            return Expression.Call(member, containsMethod, constant);

        //        case GridKnownFunction.GreaterThan:          
        //            return Expression.GreaterThan(Expression.Call(member, typeof(string).GetMethod("CompareTo", new[] { typeof(string) }), constant), Expression.Constant(0, typeof(long)));

        //        case GridKnownFunction.GreaterThanOrEqualTo:
        //            return Expression.GreaterThanOrEqual(Expression.Call(member, typeof(string).GetMethod("CompareTo", new[] { typeof(string) }), constant), Expression.Constant(0, typeof(long)));

        //        case GridKnownFunction.LessThan:
        //            return Expression.LessThan(Expression.Call(member, typeof(string).GetMethod("CompareTo", new[] { typeof(string) }), constant), Expression.Constant(0, typeof(long)));

        //        case GridKnownFunction.LessThanOrEqualTo:
        //            return Expression.LessThanOrEqual(Expression.Call(member, typeof(string).GetMethod("CompareTo", new[] { typeof(string) }), constant), Expression.Constant(0, typeof(long)));

        //        case GridKnownFunction.StartsWith:
        //            return Expression.Call(member, startsWithMethod, constant);

        //        case GridKnownFunction.EndsWith:
        //            return Expression.Call(member, endsWithMethod, constant);
        //    }
        //    return null;
        //}

        //public static Func<IQueryable<Dto.Notification>, IOrderedQueryable<Dto.Notification>> GetOrderBy(string orderColumn, string orderType)
        //{
        //    Type typeQueryable = typeof(IQueryable<Dto.Notification>);
        //    ParameterExpression argQueryable = Expression.Parameter(typeQueryable, "p");
        //    var outerExpression = Expression.Lambda(argQueryable, argQueryable);
        //    string[] props = orderColumn.Split('.');

        //    IQueryable<Dto.Notification> query = new List<Dto.Notification>().AsQueryable<Dto.Notification>();
        //    Type type = typeof(Dto.Notification);
        //    ParameterExpression arg = Expression.Parameter(type, "x");

        //    Expression expr = arg;
        //    foreach (string prop in props)
        //    {
        //        PropertyInfo pi = type.GetProperty(prop, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        //        expr = Expression.Property(expr, pi);
        //        type = pi.PropertyType;
        //    }
        //    LambdaExpression lambda = Expression.Lambda(expr, arg);
        //    string methodName = orderType.Contains("asc") ? "OrderBy" : "OrderByDescending";

        //    MethodCallExpression resultExp = Expression.Call(typeof(Queryable), methodName, new Type[] { typeof(Dto.Notification), type }, outerExpression.Body, Expression.Quote(lambda));
        //    var finalLambda = Expression.Lambda(resultExp, argQueryable);
        //    return (Func<IQueryable<Dto.Notification>, IOrderedQueryable<Dto.Notification>>)finalLambda.Compile();
        //}


       
    }
}