using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using TrackerEnabledDbContext.Common.Models;

namespace DI.Chakra.Web.Log
{
    public partial class History : UIBasePage
    {
        #region Members
        private const string entityPrefix = "DI.Chakra.EntityModel";
        private string checkNodeName = String.Empty;
        private const string V_TABLELIST = "V_TABLELIST";
        private const string V_USERNAME = "V_USERNAME";
        private const string V_DATE = "V_DATE";
        private static DataTable auditLogRecordDetails;
        #endregion

        #region Properties
        public IList<string> tableList
        {
            get
            {
                if (ViewState[V_TABLELIST] == null)
                    return new List<string>();
                return (IList<string>)ViewState[V_TABLELIST];
            }
            set
            {
                ViewState[V_TABLELIST] = value;
            }
        }
        public string userName
        {
            get
            {
                if (ViewState[V_USERNAME] == null)
                    return "";
                return (string)ViewState[V_USERNAME];
            }
            set
            {
                ViewState[V_USERNAME] = value;
            }
        }
        public DateTime? filterDate
        {
            get
            {
                if (ViewState[V_DATE] == null)
                    return null;
                return (DateTime)ViewState[V_DATE];
            }
            set
            {
                ViewState[V_DATE] = value;
            }
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                tableList = DomainApiClient<AuditConfiguration>.GetByAction<IList<string>>(DomainApiAction.Configuration.AuditConfiguration.GetAllTableList, null);
                RadTreeView.DataSource = tableList;
                RadTreeView.DataBind();
            }
            else
            {
                if (RadTreeView.SelectedNode != null)
                    checkNodeName = RadTreeView.SelectedNode.Text;
            }
        }
        protected void Filter_Click(object sender, EventArgs e)
        {
            HeaderItem.Text = "";
            string tableName = TableName.Text.Trim();
            IList<string> dataList = new List<string>();
            
            if(DatePicker.SelectedDate != null)
                filterDate = (DateTime)DatePicker.SelectedDate;
            if(UserSearchUC.SelectedUser != null)
                userName = UserSearchUC.SelectedUser.UserName;

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("tableName", tableName.ToString());
            parameter.Add("filterDate", filterDate.ToString());
            parameter.Add("userName", userName.ToString());
            tableList = DomainApiClient<AuditConfiguration>.GetByAction<IList<string>>(DomainApiAction.Configuration.AuditConfiguration.GetTableList, parameter);

            IList<string> data = tableList;
            RadTreeView.DataSource = data;
            RadTreeView.DataBind();

            checkNodeName = "";
            AuditHistoryGrid.Rebind();
        }
        protected void AuditHistoryGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            IList<AuditLog> auditHistoryList = new List<AuditLog>();
            if (!string.IsNullOrEmpty(checkNodeName))
            {
                string entityName = entityPrefix + "." + checkNodeName;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("entityName", entityName.ToString());
                parameter.Add("filterDate", filterDate.ToString());
                parameter.Add("userName", userName.ToString());
                auditHistoryList = DomainApiClient<AuditConfiguration>.GetByAction<IList<AuditLog>>(DomainApiAction.Configuration.AuditConfiguration.GetLogs, parameter);
            }
            AuditHistoryGrid.DataSource = auditHistoryList;
        }
        protected void AuditHistoryGrid_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if(e.CommandName == "AuditLog")
            {
                GridDataItem item = e.Item as GridDataItem;
                long auditLogId = (long)item.GetDataKeyValue("AuditLogId");
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("auditLogId", auditLogId.ToString());
                List<AuditLogDetail> auditLogDetails = DomainApiClient<AuditConfiguration>.GetByAction<List<AuditLogDetail>>(DomainApiAction.Configuration.AuditConfiguration.GetAuditLogDetail, parameter);
                AuditLogDetailGrid.DataSource = auditLogDetails;
                AuditLogDetailGrid.DataBind();
                DetailWindow.VisibleOnPageLoad = true;
            }
            if (e.CommandName == "Record")
            {
                GridDataItem item = e.Item as GridDataItem;
                long auditLogId = (long)item.GetDataKeyValue("AuditLogId");
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("auditLogId", auditLogId.ToString());
                auditLogRecordDetails = DomainApiClient<AuditConfiguration>.GetByAction<DataTable>(DomainApiAction.Configuration.AuditConfiguration.GetAuditLogRecordDetail, parameter);
                dynamicGrid.DataSource = auditLogRecordDetails;
                dynamicGrid.Rebind();
                RecordDetailWindow.VisibleOnPageLoad = true;
            }
        } 
        protected void RadTreeView_NodeClick(object sender, Telerik.Web.UI.RadTreeNodeEventArgs e)
        {
            HeaderItem.Text = checkNodeName;
            AuditHistoryGrid.Rebind();

        }
        protected void btnok_Click(object sender, EventArgs e)
        {
            DetailWindow.VisibleOnPageLoad = false;
        }
        protected void dynamicGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            dynamicGrid.DataSource = auditLogRecordDetails;
        }

        protected void RecordOk_Click(object sender, EventArgs e)
        {
            RecordDetailWindow.VisibleOnPageLoad = false;
        }
        #endregion


    }
}