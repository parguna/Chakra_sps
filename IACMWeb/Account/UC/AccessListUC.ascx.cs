using Telerik.Web.UI;
using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;

namespace DI.Chakra.Web.Account.UC
{
    public partial class AccessListUC : DI.Chakra.Web.UIBaseUC
    {
        #region Members
        private const string V_EntityID = "V_EntityID";
        private const string V_APPLICATIONTYPE = "APPLICATIONTYPE";       
        #endregion

        #region Property
        public long EntityId
        {
            get
            {
                if (ViewState[V_EntityID] == null)
                    return -1;
                return (long)ViewState[V_EntityID];
            }
            set
            {
                ViewState[V_EntityID] = value;
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
            AccessList.DataSource = DomainApiClient<Dto.Presentation.PresentationUserEntitlement>.
                GetByAction<List<Dto.Presentation.PresentationUserEntitlement>>
                (DomainApiAction.Endpoint.Presentation.PresentationUserEntitlement.GetDetailById, parameter);
        }
        #endregion
    }
}