using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Web.Log
{
    public partial class Configuration : UIBasePage
    {
        #region Members
        private string checkNodeName = String.Empty;
        private const string V_TABLEPROPERTYLIST = "V_TABLEPROPERTYLIST";
        private const string V_TABLELIST = "V_TABLELIST";
        private const string V_PROPERTYLIST = "V_PROPERTYLIST";
        private List<AuditConfigurationDetail> auditConfigurationDetail = new List<AuditConfigurationDetail>();
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
        public IList<string> propertyList
        {
            get
            {
                if (ViewState[V_PROPERTYLIST] == null)
                    return new List<string>();
                return (IList<string>)ViewState[V_PROPERTYLIST];
            }
            set
            {
                ViewState[V_PROPERTYLIST] = value;
            }
        }
        public IList<TableProperties> tablePropertiesList
        {
            get
            {
                if (ViewState[V_TABLEPROPERTYLIST] == null)
                    return new List<TableProperties>();
                return (IList<TableProperties>)ViewState[V_TABLEPROPERTYLIST];
            }
            set
            {
                ViewState[V_TABLEPROPERTYLIST] = value;
            }
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            AuditConfigurationGrid.AllowMultiRowSelection = true;
            if (Page.IsPostBack)
            {
                if(RadTreeView.SelectedNode!=null)
                    checkNodeName = RadTreeView.SelectedNode.Text;
            }
            else
            {
                tableList = DomainApiClient<Dto.TableProperties>.GetByAction<IList<string>>(DomainApiAction.Common.TableProperties.GetTableList, null);
                RadTreeView.DataSource = tableList;
                RadTreeView.DataBind();
            }
        }
        protected void RadTreeView_NodeClick(object sender, Telerik.Web.UI.RadTreeNodeEventArgs e)
        {
            TableNameHeader.Text = checkNodeName;
            BindTableProperties(checkNodeName);
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("tableName", checkNodeName);
            propertyList = DomainApiClient<AuditConfigurationDetail>.GetByAction<IList<string>>(DomainApiAction.Configuration.AuditConfigurationDetail.GetPropertiesByTableName, parameter);
        }
        protected void AuditConfigurationGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            AuditConfigurationGrid.DataSource = this.tablePropertiesList;
        }

        protected void Filter_Click(object sender, EventArgs e)
        {
            TableNameHeader.Text = "";
            string tableName = TableName.Text;
            IList<string> dataList = new List<string>();
            if (!string.IsNullOrEmpty(tableName))
                dataList = tableList.Where(x => x.ToLower().Contains(tableName.ToLower())).ToList();
            else
                dataList = tableList;

            RadTreeView.DataSource = dataList;
            RadTreeView.DataBind();
            
            BindGrid();
        }
        protected void AuditConfigurationGrid_PreRender(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(checkNodeName))
            {
                int i = 0;
                int cnt = AuditConfigurationGrid.MasterTableView.Items.Count;
                foreach (GridDataItem item in AuditConfigurationGrid.MasterTableView.Items)
                {
                    CheckBox checkBox = item.FindControl("ClientSelectColumn") as CheckBox;
                    string propertyName = item.GetDataKeyValue("ColumnName").ToString();

                    if (propertyList.Contains(propertyName))
                    {
                        checkBox.Checked = true;
                        item.Selected = true;
                        i++;
                    }
                    else
                    {
                        checkBox.Checked = false;
                        item.Selected = false;
                    }
                }
                foreach (GridItem item in AuditConfigurationGrid.MasterTableView.GetItems(GridItemType.Header))
                {
                    CheckBox chkbx = (CheckBox)item.FindControl("CheckBox");
                    if ((cnt > 0) && (i == cnt))
                        chkbx.Checked = true;
                    else
                        chkbx.Checked = false;
                }
            }
        }
        protected void CheckIsAuditEnabled_CheckedChanged(object sender, EventArgs e)
        {
            TableNameHeader.Text = "";
            IList<string> data = new List<string>();
            if (CheckIsAuditEnabled.Checked)
                data = DomainApiClient<AuditConfiguration>.GetAll().Select(x => x.TableName).ToList();
            else
                data = tableList;
            RadTreeView.DataSource = data;
            RadTreeView.DataBind();
            BindGrid();
        }
        protected void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            var x = sender as CheckBox;
            if (x != null)
            {
                bool checkStatus = x.Checked;
                foreach (GridDataItem item in AuditConfigurationGrid.MasterTableView.Items)
                {
                    CheckBox chkbx = (CheckBox)item["CheckTemp"].FindControl("ClientSelectColumn");
                    chkbx.Checked = checkStatus;
                    item.Selected = checkStatus;

                    string propertyName = item.GetDataKeyValue("ColumnName").ToString();
                    if (checkStatus)
                    {
                        if (!propertyList.Contains(propertyName))
                            propertyList.Add(propertyName);
                    }
                    else
                        propertyList.Remove(propertyName);
                }
            }
        }
        protected void ClientSelectColumn_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            GridDataItem selectedItem = (GridDataItem)chk.Parent.Parent;
            selectedItem.Selected = chk.Checked;
            string propertyName = selectedItem.GetDataKeyValue("ColumnName").ToString();
            if (chk.Checked && !propertyList.Contains(propertyName))
                propertyList.Add(propertyName);
            else
                propertyList.Remove(propertyName);
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                Save();
            }
        }
        #endregion

        #region Methods
        private void Save()
        {
            AuditConfiguration auditConfiguration;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("tableName", checkNodeName);
            auditConfiguration = DomainApiClient<AuditConfiguration>.GetByAction<AuditConfiguration>(DomainApiAction.Configuration.AuditConfiguration.GetByTableName, parameter);

            if (auditConfiguration == null)
                auditConfiguration = new AuditConfiguration();

            auditConfiguration.TableName = checkNodeName;
            foreach (string propertyName in propertyList)
            {
                AuditConfigurationDetail auditConfigDetail = new AuditConfigurationDetail();
                if (auditConfiguration.Id > 0)
                {
                    Dictionary<string, string> parameter1 = new Dictionary<string, string>();
                    parameter1.Add("tableName", checkNodeName);
                    parameter1.Add("property", propertyName);
                    long configurationDetailId = DomainApiClient<AuditConfigurationDetail>.GetByAction<long>(DomainApiAction.Configuration.AuditConfigurationDetail.GetByTableProperties, parameter1);
                    auditConfigDetail.Id = configurationDetailId;
                }
                auditConfigDetail.PropertyName = propertyName;
                auditConfigDetail.TableId = auditConfiguration.Id;
                auditConfigurationDetail.Add(auditConfigDetail);
            }
            auditConfiguration.AuditConfigurationDetails = auditConfigurationDetail;
            DomainApiClient<AuditConfiguration>.Save(auditConfiguration);
            MasterPage.ShowAlert(AlertType.Success, "Audit Configuration saved successfully");
        }
        private void BindTableProperties(string checkNodeName)
        {
            if (!string.IsNullOrEmpty(checkNodeName))
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("tableName", checkNodeName);
                this.tablePropertiesList = DomainApiClient<Dto.TableProperties>.GetByAction<IList<Dto.TableProperties>>(DomainApiAction.Common.TableProperties.GetByTableName, parameter);
            }
            
            AuditConfigurationGrid.Rebind();
        }
        private void BindGrid()
        {
            this.tablePropertiesList = null;
            AuditConfigurationGrid.Rebind();
        }
        
        #endregion

        
    }
}