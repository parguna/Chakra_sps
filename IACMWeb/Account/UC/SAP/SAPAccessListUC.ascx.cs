using System;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;

namespace DI.Chakra.Web.Account.UC.SAP
{
    public partial class SAPAccessListUC : DI.Chakra.Web.UIBaseUC
    {
        #region Members
        private const string V_ID = "ENTITY_ID";
        private const string V_APPLICATIONTYPE = "APPLICATIONTYPE";

        #endregion

        #region Property
        public long EntityId
        {
            get
            {
                if (ViewState[V_ID] == null)
                    return -1;
                return (long)ViewState[V_ID];
            }
            set
            {
                ViewState[V_ID] = value;
            }
        }
        public EndpointType ApplicationType
        {
            get
            {
                if (ViewState[V_APPLICATIONTYPE] == null)
                    return EndpointType.None;
                return (EndpointType)ViewState[V_APPLICATIONTYPE];
            }
            set
            {
                ViewState[V_APPLICATIONTYPE] = value;
            }
        }
        #endregion

        #region Events
        protected void AccessList_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userID", EntityId.ToString());
            AccessList.DataSource = DomainApiClient<Dto.Presentation.PresentationUserEntitlement>.GetByAction
                <List<Dto.Presentation.PresentationUserEntitlement>>
                (DomainApiAction.Endpoint.Presentation.PresentationUserEntitlement.GetDetailById, parameter);
        }

        protected void AccessList_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            switch (e.DetailTableView.Name)
            {
                case "TCodes":
                    {
                        parameter.Clear();

                        GridDataItem parentItem = e.DetailTableView.ParentItem;
                        long EndPointSystemId = Convert.ToInt64(parentItem.GetDataKeyValue("EndPointSystemId"));
                        string RoleCode = Convert.ToString(parentItem.GetDataKeyValue("Name"));
                        parameter.Add("roleCode", RoleCode);
                        parameter.Add("endPointSystemId", EndPointSystemId.ToString());
                        var TCodeDetail = DomainApiClient<Dto.SAPTCodeInfo>.GetByAction<List<Dto.SAPTCodeInfo>>
                            (DomainApiAction.Endpoint.SAP.SAPTCodeInfo.GetByRoleCode, parameter);

                        TCodeDetail.Add(new Dto.SAPTCodeInfo() { TCODE_Code = "Authorization", Id = EndPointSystemId, TCODE_Desc = RoleCode });
                        e.DetailTableView.DataSource = TCodeDetail;
                        break;
                    }

                case "Authorization":
                    {
                        GridDataItem parentItem = e.DetailTableView.ParentItem;
                        long EndPointSystemId = Convert.ToInt64(parentItem.GetDataKeyValue("Id"));
                        string RoleCode = Convert.ToString(parentItem.GetDataKeyValue("TCODE_Desc"));
                        parameter.Add("roleCode", RoleCode);
                        parameter.Add("endPointSystemId", EndPointSystemId.ToString());
                        e.DetailTableView.DataSource = DomainApiClient<Dto.Presentation.PresentationSAPRoleToAuthorizationReport>.
                            GetByAction<List<Dto.Presentation.PresentationSAPRoleToAuthorizationReport>>
                            (DomainApiAction.Endpoint.Presentation.SAP.GetSapRolesToAuthorizationData, parameter);
                        break;
                    }
            }
        }
        #endregion
    }
}