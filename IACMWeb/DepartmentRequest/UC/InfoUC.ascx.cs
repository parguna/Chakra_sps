using System;
using System.Linq;
using DI.Chakra.Infrastructure;
using System.Collections.Generic;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;

namespace DI.Chakra.Web.DepartmentRequest.UC
{
    public partial class InfoUC : UIBaseUC
    {
        #region Members
        private const string V_AccessRequestId = "V_AccessRequestId";
        #endregion

        #region Properties
        public long AccessRequestId
        {
            get
            {
                if (ViewState[V_AccessRequestId] != null)
                {
                    return Convert.ToInt64(ViewState[V_AccessRequestId]);
                }
                return 0;
            }
            set
            {
                ViewState[V_AccessRequestId] = value;
                SetPage();
            }
        }
        #endregion

        #region Methods

        private void SetEntitlements()
        {
           
        }
        private void SetPage()
        {
            // if there is an access request id
            if (AccessRequestId > 0)
            {
                // get access request
                var accessrequest = DomainApiClient<Dto.AccessRequest>.GetById(AccessRequestId, true);
                if (accessrequest == null)
                    return;

                // if ad hoc request
                if (accessrequest.AdHocAccessRequestId != null && accessrequest.AdHocAccessRequestId > 0)
                {
                    var adHocAccessRequest = DomainApiClient<Dto.AdHocAccessRequest>.GetById((long)accessrequest.AdHocAccessRequestId, true);
                    if (adHocAccessRequest == null)
                        return;

                    // account type label
                    AccountTypeLabel.Text = UtilityMethods.GetEnumAttribute<Description>((AccountType)adHocAccessRequest.AccountType);

                    // request for label
                    Dictionary<string, string> param = new Dictionary<string, string>();
                    param.Add("id", adHocAccessRequest.RequesteeUserId.ToString());

                    Dto.User user = null;

                    user = DomainApiClient<Dto.User>.GetByAction<User>
                        (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, param);

                    if (user != null)
                        RequestForLabel.Text = user.DisplayName;


                    // clear the user variable
                    user = null;

                    // requestBy label
                    param["id"] = adHocAccessRequest.RequesterUserId.ToString();
                    // get the requester user
                    user = DomainApiClient<Dto.User>.GetByAction<User>
                        (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, param);

                    // if user is found
                    if (user != null)
                    {
                        RequestByLabel.Text = user.DisplayName;
                    }
                    else
                    {
                        RequestByLabel.Text = "System";
                    }

                    // entitlement
                    RequestedEntitlementUC.AdhocAccessRequest = adHocAccessRequest;
                    RequestedEntitlementUC.Initialize();

                    // valid from label
                    ValidFromLabel.Text = adHocAccessRequest.ValidFrom != null ? adHocAccessRequest.ValidFrom.ToString() : String.Empty;

                    // valid to label
                    ValidToLabel.Text = adHocAccessRequest.ValidTo != null ? adHocAccessRequest.ValidTo.ToString() : string.Empty;

                    // execution date label
                    ExecutionDateLabel.Text = accessrequest.ExecutionTime.ToString();

                    // status label
                    StatusLabel.Text = UtilityMethods.GetEnumAttribute<Description>(accessrequest.ProgressStatus);
                }
                else
                {
                    // status label
                    StatusLabel.Text = UtilityMethods.GetEnumAttribute<Description>(accessrequest.ProgressStatus);

                    // account type label
                    AccountTypeLabel.Text = accessrequest.ApproverType;

                    User user = null;
                    Dictionary<string, string> param = new Dictionary<string, string>();

                    // request for label
                    param.Add("id", accessrequest.UserId.ToString());
                    user = DomainApiClient<Dto.User>.GetByAction<User>
                        (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, param);
                    if (user != null)
                    {
                        RequestForLabel.Text = user.DisplayName;
                    }

                    param.Clear();

                    // request by label
                    if (accessrequest.RequesterId > 0)
                    {
                        param.Add("id", accessrequest.RequesterId.ToString());

                        user = DomainApiClient<Dto.User>.GetByAction<User>
                            (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, param);

                        if (user != null)
                            RequestByLabel.Text = user.DisplayName;
                    }
                    else
                    {
                        RequestByLabel.Text = "System";
                    }

                    // execution date label
                    if (accessrequest.ExecutionTime != null && accessrequest.ExecutionTime > DateTime.MinValue)
                    {
                        ExecutionDateLabel.Text = accessrequest.ExecutionTime.ToString();
                    }

                }
            }

        }
        #endregion
    }
}