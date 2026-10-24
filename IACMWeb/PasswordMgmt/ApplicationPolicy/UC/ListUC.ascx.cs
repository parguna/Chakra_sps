using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Web.UI;
using Telerik.Web.UI;

namespace DI.Chakra.Web.PasswordMgmt.ApplicationPolicy.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                grdViewAllRequest.DataBind();
            }
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                delete();
            }

            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                grdViewAllRequest.ExportSettings.FileName = "ApplicationPolicy";
                Export(grdViewAllRequest, e.ActionType);
            }
        }

        protected void grdViewAllRequest_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = grdViewAllRequest.SelectedItems[0] as GridDataItem;
            ContextObjectId = (long)g.GetDataKeyValue("Id");

        }

        protected void grdViewAllRequest_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            IList<PasswordPolicy> listPasswordPolicy = DomainApiClient<PasswordPolicy>.GetAll();
            grdViewAllRequest.DataSource = listPasswordPolicy;
        }
        #endregion

        #region methods
        private void delete()
        {
            try
            {
                if (!ContextObjectId.HasValue)
                    return;
                if (ContextObjectId.Value == 1)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "IACM Default Password Policy should not be deleted");
                    return;
                }

                try
                {
                    var param = new Dictionary<string, string>();
                    param.Add("id", ContextObjectId.Value.ToString());
                    var isDeleted = DomainApiClient<PasswordPolicy>.GetByAction<bool>("DeletePolicyById", param);
                    if (isDeleted)
                    {
                        grdViewAllRequest.Rebind();
                        this.ContextObjectId = null;
                        MasterPage.ShowAlert(AlertType.Success, "Policy Deleted Sucessfully");
                    }
                    else
                    {
                        MasterPage.ShowAlert(AlertType.Error, "In order to delete the passowrd policy, please remove the application(s)and then try again.");
                    }
                }
                catch (Exception ex)
                {
                    MasterPage.ShowAlert(AlertType.Error, "In order to delete the passowrd policy, please remove the application(s)and then try again.");
                }
            }
            catch (Exception)
            {
                MasterPage.ShowAlert(AlertType.Error, "Policy not Deleted Successfully");
            }
        }
        #endregion

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }
    }
}