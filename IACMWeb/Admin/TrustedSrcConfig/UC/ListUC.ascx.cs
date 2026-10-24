
using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Admin.TrustedSrcConfig.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        private static IList<Dto.ConfigGeneralTrustedSource> listConfig;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;

            if (!Page.IsPostBack)
                grdTrsConfig.DataBind();

        }

        protected void grdTrsConfig_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            listConfig = DomainApiClient<Dto.ConfigGeneralTrustedSource>.GetAll();
            grdTrsConfig.DataSource = listConfig;

        }

        private void delete()
        {
            if (!ContextObjectId.HasValue)
                return;

            DomainApiClient<Dto.ConfigGeneralTrustedSource>.DeleteById(this.ContextObjectId.Value);
            this.ContextObjectId = null;
            grdTrsConfig.Rebind();
            MasterPage.ShowAlert(AlertType.Success, "Deleted successfully");
        }
        
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Trusted Source Config - Delete");
                delete();
            }
            else if (e.ActionType == ACLAction.Add)
            {
                modalPopup.VisibleOnPageLoad = true;
            }
            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                grdTrsConfig.ExportSettings.FileName = "TrustedSource";
                grdTrsConfig.Export(grdTrsConfig,e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                grdTrsConfig.Export<IList<Dto.ConfigGeneralTrustedSource>>(listConfig, e.ActionType, "TrustedSource");
            }
        }

        protected void grdTrsConfig_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = grdTrsConfig.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/TrustedSrcConfig/List.aspx");
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Save();
            
        }



        private void Save()
        {
            long EPType = DetailUC.EpTypes;
            if (EPType == -1)
            {
                MasterPage.ShowAlert(AlertType.Warning, "Please select Trusted Source");
                return;
            }
            string strUser = DetailUC.User;
            string strEPCol = DetailUC.EPColumns;
           // long intEPType= DetailUC.EpTypes;
            bool blSelfUpdate = DetailUC.Selfupdate;
            if (DoesAlreadyExists())//DetailUC.User, DetailUC.EpTypes
            {
                MasterPage.ShowAlert(AlertType.Warning, "Trusted User Already Exists.");
                return;
            }

            Dto.ConfigGeneralTrustedSource trs = new Dto.ConfigGeneralTrustedSource();
            trs.AttributeName = strUser.Trim();
            trs.TrustedSourceAttribute = strEPCol.Trim();
            trs.TrustedSource = (EndpointType)EPType;
            trs.SelfUpdate = blSelfUpdate;
            DomainApiClient<Dto.ConfigGeneralTrustedSource>.Save(trs);
            MasterPage.ShowAlert(AlertType.Success, "Saved successfully");
            string redirectTarget = Request.Url.PathAndQuery;
            if (DI.Chakra.Infrastructure.UtilityMethods.IsLocalUrl(redirectTarget))
                Response.Redirect(redirectTarget);
            else
                Response.Redirect("~/Default.aspx");
        }
        private  bool DoesAlreadyExists()//string attributename, long endpointsystemid
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("attributename", DetailUC.User);
            parameter.Add("endpointType", DetailUC.EpTypes.ToString());

            return  DomainApiClient<Dto.ConfigGeneralTrustedSource>.GetByAction<bool>(DomainApiAction.Configuration.ConfigGeneralTrustedSource.DoesAlreadyExists, parameter);
            
        }
    }
}