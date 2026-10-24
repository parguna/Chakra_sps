using Application;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Admin.Recon
{
    public partial class ExceptionAttribute : UIBasePage
    {
        private const string V_ENDPOINTTYPEID = "V_ENDPOINTTYPEID";
        private const string V_ATTRIBUTELIST = "V_ATTRIBUTELIST";
        private SessionStatusImpl sessionstatus = new SessionStatusImpl();

        public long EndPointTypeId
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
                SetExceptionAttribute();
            }
        }

        public List<Dto.ExceptionAttribute> AttributeList
        {
            get
            {
                if (ViewState[V_ATTRIBUTELIST] == null)
                    return null;
                return (List<Dto.ExceptionAttribute>)ViewState[V_ATTRIBUTELIST];
            }
            set
            {
                ViewState[V_ATTRIBUTELIST] = value;
                //SetValue();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!IsPostBack)
            {
                EndpointUC.Initialize();
            }
            DropDownList ddlEndpoint = (DropDownList)EndpointUC.FindControl("ApplicationTypeField");
            int getVal = (int)EndpointType.InhouseSystems;
            ddlEndpoint.Items.Remove(ddlEndpoint.Items.FindByValue(getVal.ToString()));
        }
        protected void EndpointUC_ApplicationChanged(EndPointSystemType EndPointSystemType)
        {
            this.EndPointTypeId = EndPointSystemType.Id;
            EndPointSystemType endPointType = DomainApiClient<Dto.EndPointSystemType>.GetById(this.EndPointTypeId);
            string tableName = endPointType.ObjectName;

            ddlAttributes.Items.Clear();
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("tableName", tableName);
            IList<Dto.TableProperties> listTgColumns = DomainApiClient<Dto.TableProperties>.GetByAction<IList<Dto.TableProperties>>(DomainApiAction.Common.TableProperties.GetByTableName, parameter);

            if (listTgColumns != null && listTgColumns.Count > 0)
            {
                listTgColumns = listTgColumns.OrderBy(x => x.ColumnName).ToList();
                ddlAttributes.Items.Clear();
                ddlAttributes.DataSource = listTgColumns;
                ddlAttributes.Items.Remove(ddlAttributes.Items.FindByValue("TextToFind"));
                ddlAttributes.DataTextField = Constants.COLUMNNAME;
                ddlAttributes.DataBind();
            }
        }
        protected void AttributesGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            AttributesGrid.DataSource = AttributeList;
        }
        protected void AttributesGrid_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                GridDataItem item = e.Item as GridDataItem;
                string strAtt = Convert.ToString(item["Attributes"].Text);
                var obj = (from lst in AttributeList
                           where lst.Attributes == strAtt
                           select lst).FirstOrDefault();
                if (obj == null)
                    return;
                AttributeList.Remove(obj);
            }
            AttributesGrid.Rebind();
        }
        protected void AddUser_Click(object sender, EventArgs e)
        {
            Dto.ExceptionAttribute exceptionAttribute = new Dto.ExceptionAttribute()
            {
                EndpointSystemTypeId = this.EndPointTypeId,
                Attributes = ddlAttributes.SelectedItem.Text,
                
            };
            if (this.AttributeList == null)
                this.AttributeList = new List<Dto.ExceptionAttribute>() { exceptionAttribute };
            else
            {
                var Existing = this.AttributeList.Where(l => l.EndpointSystemTypeId == exceptionAttribute.EndpointSystemTypeId && l.Attributes == exceptionAttribute.Attributes).ToList();
                if (Existing.Count == 0)
                    this.AttributeList.Add(exceptionAttribute);
                else
                    MasterPage.ShowAlert(AlertType.Warning, "Attribute already exist");
            }
            ddlAttributes.ClearSelection();
            AttributesGrid.Rebind();
        }
        private void SetExceptionAttribute()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("endpointTypeId", this.EndPointTypeId.ToString());
            this.AttributeList = DomainApiClient<Dto.ExceptionAttribute>.GetByAction< List<Dto.ExceptionAttribute>>(DomainApiAction.Recon.ExceptionAttribute.GetByEndpointTypeId, parameter);
            AttributesGrid.Rebind();
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                if (this.AttributeList != null && this.AttributeList.Count > 0)
                {
                    DomainApiClient<Dto.ExceptionAttribute>.GetByAction<System.Object>("SaveExceptionAttribute", null, this.AttributeList);
                    ClearSelection();
                    MasterPage.ShowAlert(AlertType.Success,"Exception Attributes saved successfully");
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Please select the required fields");
                }
            }
        }

        private void ClearSelection()
        {
            ddlAttributes.ClearSelection();
            EndpointUC.ClearApplicationType();
            this.AttributeList.Clear();
            AttributesGrid.Rebind();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/Recon/ReconException.aspx");
        }
    }
}