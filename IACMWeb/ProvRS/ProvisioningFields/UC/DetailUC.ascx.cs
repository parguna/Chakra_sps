using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.ProvRS.ProvisioningFields.UC
{
    public partial class DetailUC : UIBaseUC
    {
        #region Member

        private const string V_ID = "ENTITY_ID";
        private const string V_FIELDTYPE = "V_FIELDTYPE";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        private static Dto.NProvisioningField mProvisioningField = new Dto.NProvisioningField();

        #endregion

        #region Public Properties

        public Dto.NProvisioningField provisioningField
        {
            get
            {
                mProvisioningField = GetControl();
                return mProvisioningField;
            }
            set
            {
                mProvisioningField = value;
                SetControl();
            }
        }
        public UIMode UIMode
        {
            get
            {
                if (ViewState[V_DISPLAYMODE] == null)
                    return UIMode.View;
                return (UIMode)ViewState[V_DISPLAYMODE];
            }
            set
            {
                ViewState[V_DISPLAYMODE] = value;
            }
        }

        public long EntityId
        {
            get
            {
                if (ViewState[V_ID] == null)
                    return 0;
                return (long)ViewState[V_ID];
            }
            set
            {
                ViewState[V_ID] = value;
            }
        }

        #endregion

        #region Page Event
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtFieldName.Enabled = true;
                BindAccountType();
            }
        }
        public void ddlAccountType_SelectedIndexChanged(object sender, EventArgs e)
        {
            mProvisioningField.AccountType = (AccountType)Convert.ToInt32(ddlAccounType.SelectedValue);
        }

        #endregion

        #region Methods
        public void Clearvalue()
        {
            txtFieldName.Text = string.Empty;
            txtFieldName.Enabled = true;
            txtValue.Text = string.Empty;
            txtDescription.Text = string.Empty;
            BindAccountType();
        }

        private Dto.NProvisioningField GetControl()
        {
            if ((UIMode == UIMode.View) | (UIMode == UIMode.Edit))
            {
                EntityId = Convert.ToInt64(this.ContextObjectId);
                mProvisioningField.Id = EntityId;
            }
            else if (UIMode == UIMode.Add)
            {
                mProvisioningField.Id = 0;
            }
            mProvisioningField.FieldName = txtFieldName.Text.Trim();

            mProvisioningField.Value = txtValue.Text.Trim();
            mProvisioningField.AccountType = (AccountType)Convert.ToInt32(ddlAccounType.SelectedValue);
            mProvisioningField.Description = txtDescription.Text.Trim();
            mProvisioningField.IsActive = chkIsActive.Checked ? true : false;

            return mProvisioningField;
        }

        private void SetControl()
        {
            txtFieldName.Text = mProvisioningField.FieldName;
            txtFieldName.Enabled = false;
            txtFieldName.CssClass = "form-control";

            divValue.Visible = true;
            txtValue.Text = mProvisioningField.Value;

            
            if (mProvisioningField.AccountType != null)
                ddlAccounType.SelectedValue = Convert.ToString((byte)mProvisioningField.AccountType);
            else
                ddlAccounType.Items.FindByValue(Convert.ToInt16(AccountType.User).ToString()).Selected = true;

            txtDescription.Text = mProvisioningField.Description;
           
            chkIsActive.Checked = (mProvisioningField.IsActive == true) ? true : false;
            EntityId = mProvisioningField.Id;
        }

        private void BindAccountType()
        {
            ddlAccounType.Items.Clear();
            ddlAccounType.DataValueField = "Id";
            ddlAccounType.DataTextField = "AccountTypeDesc";
            ddlAccounType.DataSource = DomainApiClient<Dto.AccountCategory>.GetByAction<List<Dto.AccountCategory>>
                    (DomainApiAction.MetaData.AccountCategory.GetByIsActive, null);
            ddlAccounType.DataBind();
        }

        #endregion       
    }
}