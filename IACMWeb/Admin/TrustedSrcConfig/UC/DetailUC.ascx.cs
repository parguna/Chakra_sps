
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

namespace DI.Chakra.Web.Admin.TrustedSrcConfig.UC
{
    public partial class DetailUC : UIBaseUC
    {
        private const string V_ENDPOINTTYPEID = "ENDPOINTTYPE_ID";
        
        public delegate void Add_SubmitButton(object sender, EventArgs e);
        public event Add_SubmitButton Add_SubmitButtonClicked;
        public delegate void Add_CancelButton(object sender, EventArgs e);
        public event Add_CancelButton Add_CancelButtonClicked;

        public int EndpointTypeId
        {
            get
            {
                if (ViewState[V_ENDPOINTTYPEID] == null)
                    return -1;
                return (int)ViewState[V_ENDPOINTTYPEID];
            }
            set
            {
                ViewState[V_ENDPOINTTYPEID] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDDLDi();
                BindApplicationType();
            }
        }

        public long EpTypes
        {
            get
            {
                if (ddlEPTypes.SelectedValue == "--Select--")
                    return -1;
                return int.Parse(ddlEPTypes.SelectedValue);
            }
            set { EpTypes = value; }
        }



        public string EPColumns
        {
            get
            { return ddlTg.Text; }
            set { EPColumns = value; }
        }



        public string User
        {
            get
            { return ddlDi.Text; }
            set { User = value; }
        }

        public bool Selfupdate
        {
            get
            {
                return chkSelfUpdate.Checked;
            }
            set
            {
                Selfupdate = value;
            }
        }
   

        private void BindApplicationType()
        {
            try
            {
                ddlEPTypes.Items.Clear();
                ddlEPTypes.DataSource = DomainApiClient<Dto.EndPointSystemType>.GetAll();
                ddlEPTypes.DataBind();
                ddlEPTypes.Items.Insert(0, "--Select--");
                //int index = ddlEPTypes.Items.Count;
                //ddlEPTypes.Items.Insert(index, new ListItem(UtilityMethods.GetEnumAttribute<Description>(EndpointType.TrustedSource), ((int)EndpointType.TrustedSource).ToString()));
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
        private void BindDDLDi()
        {
            try
            {
                string TableName = "User";
                ddlDi.Items.Clear();
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("tableName",TableName);
                IList<Dto.TableProperties> listDiColumns = DomainApiClient<Dto.TableProperties>.GetByAction<IList<TableProperties>>(DomainApiAction.Common.TableProperties.GetByTableName, parameter);
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
            catch (System.Exception)
            {
                throw;
            }
        }
        private void BindDDLTg()
        {

            try
            {
                string TableName = "";
           
                EndPointSystemType type = DomainApiClient<Dto.EndPointSystemType>.GetById(this.EndpointTypeId);
                if (type != null)
                    TableName = type.ObjectName;
                //TableName = UtilityMethods.GetEnumAttribute<TableName>((EndpointType)this.EndpointTypeId);

                ddlTg.Items.Clear();

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("tableName", TableName);

                IList<Dto.TableProperties> listTgColumns = DomainApiClient<Dto.TableProperties>.GetByAction<IList<Dto.TableProperties>>(DomainApiAction.Admin.TrustedSource.GetByTableName, parameter);
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
            catch (System.Exception)
            {
                throw;
            }
        }

        protected void ddlEPTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEPTypes.SelectedValue != "--Select--")
            {
                this.EndpointTypeId = Convert.ToInt32(ddlEPTypes.SelectedValue);
                BindDDLTg();
            }
            else
            {
                ddlTg.Items.Clear();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (Add_CancelButtonClicked != null)
            {
                Add_CancelButtonClicked(sender,e);
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Add_SubmitButtonClicked != null)
            {
                Add_SubmitButtonClicked(sender, e);
            }

        }
    }
}