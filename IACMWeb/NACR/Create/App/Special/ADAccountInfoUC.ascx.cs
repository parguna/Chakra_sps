using Application;
using DI.Chakra.Infrastructure.Enums;
using System.Dynamic;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure;
using Newtonsoft.Json;
using System;

namespace DI.Chakra.Web.NACR.Create.App.Special
{
    public partial class ADAccountInfoUC : UIBaseUC
    {
        private const string V_ENDPOINTID = "V_ENDPOINTID";
        private const string V_ACCOUNTTYPE = "V_ACCOUNTTYPE";

        #region Members
        #endregion

        #region Property
        public long EndPointID
        {
            get
            {
                if (ViewState[V_ENDPOINTID] == null)
                    return 0;
                return (long)ViewState[V_ENDPOINTID];
            }
            set
            {
                ViewState[V_ENDPOINTID] = value;
            }
        }

        public AccountType RequestedAccountType
        {
            get
            {
                if (ViewState[V_ACCOUNTTYPE] == null)
                    return AccountType.None;
                return (AccountType)ViewState[V_ACCOUNTTYPE];
            }
            set
            {
                ViewState[V_ACCOUNTTYPE] = value;
            }
        }



        #endregion

        #region Methods

        public void Initialize()
        {
            OUAutoCompleteBox.Clear();
            DefaultAccountInfoUC.EndPointID = EndPointID;
            DefaultAccountInfoUC.RequestedAccountType = RequestedAccountType;
            DefaultAccountInfoUC.Initilize();
            Session["EndpointSystemID"] = this.EndPointID;

            if(RequestedAccountType != AccountType.Secondary)
            {
                divPasswordNever.Visible = true;
            }
            else
            {
                divPasswordNever.Visible = false;
            }
        }

        public bool CheckValidation(out string errorMessage, long endpointTypeId, AccountType accountType)
        {
            var canSaveDefault = DefaultAccountInfoUC.CheckValidation(out errorMessage, endpointTypeId, accountType);

            if (OUAutoCompleteBox.GetValue() == null)
            {
                errorMessage += "Please enter OU value";
                return false;
            }
            return true;
        }

        public string GetDetail()
        {
            dynamic detail = DefaultAccountInfoUC.GetDetailObject();
            AutoCompleteBoxEntry selectedOU = OUAutoCompleteBox.GetValue();
            var OUText = "";
            if (selectedOU != null)
            {
                OUText = selectedOU.Text;
            }

            ExpandoHelper.AddProperty(detail, "OU", OUText);


            if (ValidToPicker.SelectedDate.HasValue)
            {
                DateTime fromValue = DateTime.Now.Date;
                DateTime toValue = ValidToPicker.SelectedDate.Value.Date;
                ExpandoHelper.AddProperty(detail, "AccountExpiryDays", toValue.Subtract(fromValue).Days);
            }
            else
            {
                ExpandoHelper.AddProperty(detail, "AccountExpiryDays", 0);
            }

            if(RequestedAccountType != AccountType.Secondary)
            {
                ExpandoHelper.AddProperty(detail, "PasswordNeverExpire", chkPasswordExpires.Checked);
            }
            else
            {
                ExpandoHelper.AddProperty(detail, "PasswordNeverExpire", false);
            }

            return JsonConvert.SerializeObject(detail);
        }



        #endregion

        protected void chkPasswordExpires_CheckedChanged(object sender, System.EventArgs e)
        {

        }
    }
}