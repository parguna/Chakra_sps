using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Data;

namespace DI.Chakra.Web.EndPoints.EndpointSystem.Common
{
    public partial class MappingDetailUC : UIBaseUC
    {
        #region Members

        private const string V_ENDPOINTTYPEID = "ENDPOINTTYPE_ID";
        private const string V_ENDPOINTSYSTEMID = "ENDPOINTSYSTEM_ID";
        private const string V_USEROBJECT = "USEROBJECT_NAME"; 
        private const string V_LISTATTRIBUTEMAPPINGCOLUMNS = "LIST_ATTRIBUTEMAPPINGCOLUMNS";
        private IList<AttributeMappingColumns> mAttributeMappingColumns = new List<AttributeMappingColumns>();
        private SessionStatusImpl sessionstatus = new SessionStatusImpl();
        #endregion

        #region Properties

        public List<Dto.AttributeMappingColumns> ListAttributeMappingColumns
        {
            get
            {
                if (ViewState[V_LISTATTRIBUTEMAPPINGCOLUMNS] == null)
                    return null;
                return (List<Dto.AttributeMappingColumns>)ViewState[V_LISTATTRIBUTEMAPPINGCOLUMNS];
            }
            set
            {
                ViewState[V_LISTATTRIBUTEMAPPINGCOLUMNS] = value;
                SetMappingDetail();
            }
        }

        public long EndpointTypeId
        {
            get
            {
                if (ViewState[V_ENDPOINTTYPEID] == null)
                    return -1;
                return (long)ViewState[V_ENDPOINTTYPEID];
            }
            set
            {
                ViewState[V_ENDPOINTTYPEID] = value;

            }
        }
        public long EndpointSystemId
        {
            get
            {
                if (ViewState[V_ENDPOINTSYSTEMID] == null)
                    return 0;
                return (long)ViewState[V_ENDPOINTSYSTEMID];
            }
            set
            {
                ViewState[V_ENDPOINTSYSTEMID] = value;
            }
        }
        public string UserObjectName  
        {
            get
            {
                if (ViewState[V_USEROBJECT] == null)
                {
                    return UserObjectAutoCompleteBox.Entries.Count > 0 ? UserObjectAutoCompleteBox.Entries[0].Text : "";
                }
                return (string)ViewState[V_USEROBJECT];
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    UserObjectAutoCompleteBox.Entries.Clear();
                    UserObjectAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry() { Text = value });
                }
            }
        }

        public bool MapAll
        {
            get
            {
                return (rbtMapAll.Checked);
            }
            set
            {
                rbtMapAll.Checked = value;
            }
        }

        public bool MapAnyOne   
        {
            get
            {
                return (rbtMapAnyOne.Checked);
            }
            set
            {
                rbtMapAnyOne.Checked = value;
            }
        }

        #endregion 

        #region Methods

        private void SetMappingDetail()
        {
            try
            {
                MappingGrid.Rebind();
            }
            catch (Exception)
            {

            }
        }
        public void InitializePage()
        {
            BindAttributeUserObjectByEndpointsystemType();
            BindDDLDi();
            BindDDLTg();
            
        }
        public void BindAttributeUserObjectByEndpointsystemType()
        {
            
            if (this.EndpointSystemId > 0) return;
            UserObjectAutoCompleteBox.Enabled = true;
            UserObjectAutoCompleteBox.Entries.Clear();
            EndPointSystemType endPointType = DomainApiClient<Dto.EndPointSystemType>.GetById(this.EndpointTypeId);
            string tableName = endPointType?.ObjectName;
            if (!string.IsNullOrWhiteSpace(tableName))
            {
                var UserObjectTypeName = GetUserObjectTypeName(tableName);
                if (!string.IsNullOrWhiteSpace(UserObjectTypeName))
                    UserObjectAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(UserObjectTypeName, tableName));
            }
            
        }
        private string GetUserObjectTypeName(string tableName)
        {
            var UserObjectTypeName = string.Empty;
            switch (tableName)
            {
                case "DI_ADS_UserInfo": UserObjectTypeName= "ADSUserInfo";break;
                case "DI_SAP_UserInfo": UserObjectTypeName = "SAPUserinfo"; break;
                case "DI_SAPEP_UserInfo": UserObjectTypeName = "SAPEPUserInfo"; break;
                case "DI_NPAC_UserInfo": UserObjectTypeName = "NPACUserInfo"; break;
                case "DI_GEMS_UserInfo": UserObjectTypeName = "GEMSUserInfo"; break;
                case "DI_VPN_UserInfo": UserObjectTypeName = "VPNUserInfo"; break;
                case "DI_INH_UserInfo": UserObjectTypeName = "InhouseUserInfo"; break;
                case "DI_Office365_UserInfo": UserObjectTypeName = "Office365UserInfo"; break;
                case "DI_SMS_UserInfo": UserObjectTypeName = "SMSUserInfo"; break;
                case "DI_IACM_UserInfo": UserObjectTypeName = "IACMUserInfo"; break;
                case "DI_SSIDS_UserInfo": UserObjectTypeName = "SSIDSUserInfo"; break;
                case "DI_SFB_UserInfo": UserObjectTypeName = "SkypeForBusinessUserInfo"; break;
                default:
                    break;
            }
            return UserObjectTypeName;
        }
        public void DisplayViewMode()
        {
            ControlStatus(contentwrapper, true); 
        }

        private void ControlStatus(Control control, bool isDisable)
        {
            foreach (Control c in control.Controls)
                try
                {
                    if (c.HasControls())
                        ControlStatus(c, isDisable);
                    else
                    {
                        if (isDisable)
                        {
                            if (c is WebControl)
                                ((WebControl)c).Enabled = false;
                        }
                        else
                        {
                            if (c is WebControl)
                                ((WebControl)c).Enabled = true;
                        }
                    }
                }
                catch (Exception)
                {
                }
        }
        private void BindDDLDi()
        {
            try
            {
                string TableName = "User";
                ddlDi.Items.Clear();
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("tableName", TableName);
                IList<Dto.TableProperties> listDiColumns = DomainApiClient<Dto.TableProperties>.GetByAction<IList<Dto.TableProperties>>(DomainApiAction.Common.TableProperties.GetByTableName, parameter);
                  
                if (listDiColumns != null && listDiColumns.Count > 0)
                {
                    ddlDi.Items.Clear();
                    ddlDi.DataSource = listDiColumns;
                    ddlDi.DataTextField = Constants.COLUMNNAME;
                    ddlDi.DataBind();
                }
                else
                {
                    ddlDi.Items.Clear();
                    ddlDi.Items.Add(new ListItem("", "0"));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void BindDDLTg()
        {
            try
            {
                string TableName = string.Empty;
                TableName = UserObjectAutoCompleteBox.Entries.Count > 0 ? UserObjectAutoCompleteBox.Entries[0].Text : "ADSUserInfo";
                ddlTg.Items.Clear();
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("tableName", TableName);
                IList<Dto.TableProperties> listTgColumns = DomainApiClient<Dto.TableProperties>.GetByAction<IList<Dto.TableProperties>>(DomainApiAction.Common.TableProperties.GetByTableName, parameter);
                if (listTgColumns != null && listTgColumns.Count > 0)
                {
                    ddlTg.Items.Clear();
                    ddlTg.DataSource = listTgColumns;
                    ddlTg.DataTextField = Constants.COLUMNNAME;
                    ddlTg.DataBind();
                }
                else
                {
                    ddlTg.Items.Clear();
                    ddlTg.Items.Add(new ListItem("", "0"));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Events        
        protected void btnAddAttribute_Click(object sender, EventArgs e)
        {
            AddAttributeradWin.VisibleOnPageLoad = true;
            BindDDLTg();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            AddAttributeradWin.VisibleOnPageLoad = false;
        }
        
        protected void btnAdd_Click1(object sender, EventArgs e)
        {
            AttributeMappingColumns attmap = new AttributeMappingColumns()
            {
                DiColumn = ddlDi.SelectedItem.Text,
                Tgcolumn = ddlTg.SelectedItem.Text,
                EndpointSystemTypeId = EndpointTypeId,
                EndpointSystemId = EndpointSystemId,
               
            };

            if (this.ListAttributeMappingColumns == null)
                this.ListAttributeMappingColumns = new List<AttributeMappingColumns>() { attmap };
            else
            {
                var Existing = this.ListAttributeMappingColumns.Where(l => l.DiColumn == attmap.DiColumn && l.Tgcolumn == attmap.Tgcolumn).ToList();
                if (Existing.Count == 0)
                    this.ListAttributeMappingColumns.Add(attmap);
                else
                    ShowAlertMessage(AlertType.Error, "This mapping is already mapped");
            }
            MappingGrid.Rebind();
            AddAttributeradWin.VisibleOnPageLoad = false;
        }

        protected void MappingGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                GridDataItem item = e.Item as GridDataItem;
                string strDi = Convert.ToString(item["DiColumn"].Text);
                string strTg = Convert.ToString(item["TgColumn"].Text);
                var obj = (from lst in ListAttributeMappingColumns
                           where lst.DiColumn == strDi && lst.Tgcolumn == strTg
                           select lst).FirstOrDefault();
                if (obj == null)
                    return;
                ListAttributeMappingColumns.Remove(obj);

            }
            MappingGrid.Rebind();
        }

        protected void MappingGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            MappingGrid.DataSource = ListAttributeMappingColumns;
        }

        #endregion
    }
}