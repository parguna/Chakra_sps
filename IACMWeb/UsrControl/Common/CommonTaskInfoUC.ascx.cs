
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class CommonTaskInfoUC : DI.Chakra.Web.UIBaseUC
    {
        #region Members
        private AdHocAccessRequest mAdhocAccessRequest = null;

        private const string V_ISFROMACCESSREQUEST = "V_ISFROMACCESSREQUEST";
        private const string V_CANLOADACCESSREQITEMS = "V_CANLOADACCESSREQITEMS";
        private const string V_COMPLIANCEREPORT = "V_COMPLIANCEREPORT";
        #endregion

        #region Properties

        public bool isFromAccessRequest
        {
            get
            {
                if (ViewState[V_ISFROMACCESSREQUEST] == null)
                    return false;
                return (bool)ViewState[V_ISFROMACCESSREQUEST];
            }
            set
            {
                ViewState[V_ISFROMACCESSREQUEST] = value;
            }
        }


        public bool canLoadAccessReqItems
        {
            get
            {
                if (ViewState[V_CANLOADACCESSREQITEMS] == null)
                    return false;
                return (bool)ViewState[V_CANLOADACCESSREQITEMS];
            }
            set
            {
                ViewState[V_CANLOADACCESSREQITEMS] = value;

            }
        }

        public List<UserSODComplianceReport> complianceReport
        {
            get
            {
                if (ViewState[V_COMPLIANCEREPORT] == null)
                    return null;
                return (List<UserSODComplianceReport>)ViewState[V_COMPLIANCEREPORT];
            }
            set
            {
                ViewState[V_COMPLIANCEREPORT] = value;
            }
        }

        public AdHocAccessRequest AdhocAccessRequest
        {
            set
            {
                mAdhocAccessRequest = value;
                SetForm(mAdhocAccessRequest);
            }
        }
        #endregion

        #region Methods
        private void SetEntitlements(AdHocAccessRequest AdhocAccessRequest)
        {

            RequestedEntitlementUC.AdhocAccessRequest = AdhocAccessRequest;
            RequestedEntitlementUC.Initialize();

            Dictionary<string, string> parameters1 = new Dictionary<string, string>();
            parameters1.Add("adhocAccessRequestId", AdhocAccessRequest.Id.ToString());

            var accessRequest = DomainApiClient<Dto.AccessRequest>.GetByAction<Dto.AccessRequest>(DomainApiAction.Provisioning.AccessRequest.GetByAdHocAccessRequestId, parameters1);
            entitlementInfo.Visible = false;
            if (accessRequest != null && AdhocAccessRequest != null && AdhocAccessRequest.EndpointSystemType != EndpointType.InhouseSystems && accessRequest.EmployeeEventTriggerId.HasValue)
            {
                entitlementInfo.Visible = true;
            }

                AccessRequestEntitlementDetailsUC.AdhocAccessRequest = AdhocAccessRequest;
            AccessRequestEntitlementDetailsUC.Initialize();
        }
        public void SetAdhocRequest(AdHocAccessRequest AdhocAccessRequest)
        {
            this.ContextObjectId = AdhocAccessRequest.RequesteeUserId;
        }
        public bool SaveNewAccountInfo(AdHocAccessRequest AdhocAccessRequest, out string errorMessage)
        {
            bool dontAllowToApprove = EMPForm.SaveUser();
            errorMessage = EMPForm.ErrorMessage;
            return dontAllowToApprove;
            // NewAccountInfoUC1.SaveNewUserOnApproval(AdhocAccessRequest);
        }
        private void SetForm(AdHocAccessRequest AdhocAccessRequest)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("requestId", AdhocAccessRequest.Id.ToString());
            parameter.Add("deepLoad", "true");
            PresentationMyRequestData adhocRequest = DomainApiClient<PresentationMyRequestData>.GetByAction<PresentationMyRequestData>(DomainApiAction.Attestation.Presentation.PresentationMyRequest.GetByAdhocId, parameter);
            LabelRequestID.Text = adhocRequest.AccessRequestID.ToString();

            if (!string.IsNullOrEmpty(adhocRequest.ServiceAccountComplianceString))
            {
                ServiceAccountCompliance.Visible = true;
                this.ServiceAccountComplianceUC.SecurityResponse = new List<SecurityResponseGridData>();
            }
            else
            {
                ServiceAccountCompliance.Visible = false;
            }
            this.LabelSystemName.Text = adhocRequest.EndPoint.DisplayName;
            this.LabelAccountType.Text = UtilityMethods.GetEnumAttribute<Description>(adhocRequest.AdHocAccessRequests.AccountType);
            this.lblRequestType.Text = UtilityMethods.GetEnumAttribute<Description>(adhocRequest.AdhocActionType);

            this.LabelValidFrom.Text = (adhocRequest.StartDate.HasValue) ? adhocRequest.StartDate.Value.ToShortDateString() + " " + adhocRequest.StartDate.Value.ToShortTimeString() : string.Empty;
            this.LabelValidTo.Text = (adhocRequest.EndDate.HasValue) ? adhocRequest.EndDate.Value.ToShortDateString() + " " + adhocRequest.EndDate.Value.ToShortTimeString() : string.Empty;


            //IEntitlementDetail accessDetail = DomainApiClient<Dto.AdHocAccessRequest>.GetByAction<IEntitlementDetail>(DomainApiAction.AdhocRequest.GetEntitlementDetail, null, AdhocAccessRequest);
            //if (null == accessDetail)
            //{
            //    return;
            //}


            SetEntitlements(AdhocAccessRequest);

            EndPoint endPoint = DomainApiClient<EndPoint>.GetById(adhocRequest.EndPoint.Id, true);
            this.RequestRiskUC.EntitlementConflict = AdhocAccessRequest.Entitlement;
            this.RequestRiskUC.RequestId = adhocRequest.AccessRequestID;
            this.RequestRiskUC.UserId = AdhocAccessRequest.RequesteeUserId;
            this.RequestRiskUC.canLoadAccessReqItems = canLoadAccessReqItems;
            this.RequestRiskUC.HasRisk = (adhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove) ? false : true;
            this.RequestRiskUC.Initialize();
            this.complianceReport = this.RequestRiskUC.complianceReport;

            if (AdhocAccessRequest.AdhocActionType == AdHocActionType.NewAccount && AdhocAccessRequest.EndpointSystemType == EndpointType.IDMS)
            {
                Dto.AccessRequest accessRequest = DomainApiClient<Dto.AccessRequest>.GetById(adhocRequest.AccessRequestID);
                if(accessRequest != null && accessRequest.RequestSource == AccessRequestSource.EmployeeCreation)
                {
                    EMPForm.Visible = true;
                    EMPForm.ContextObjectId = AdhocAccessRequest.RequesteeUserId;
                    EMPForm.IsEdit = true;
                }
                else
                {
                    EMPForm.Visible = false;
                }
                EmpDetailsHeader.Visible = true;
                Dto.User ouser = DomainApiClient<Dto.User>.GetById(AdhocAccessRequest.RequesteeUserId);

                EMPForm.IsFullTimeEmployee = ouser.FullTimeEmployee;
                if (Request.QueryString["ISAPPROVEPAGE"] != null)
                {
                    EMPForm.IsApprovePage = Convert.ToBoolean(Request.QueryString["ISAPPROVEPAGE"]);
                }
                else
                {
                    EMPForm.IsApprovePage = true;
                }
                EMPForm.Initialize();
                EMPForm.HideOrgChartLink();
            }
            else
            {
                EMPForm.Visible = false;
                EmpDetailsHeader.Visible = false;
            }

            #region Requestor Comments
            if (!string.IsNullOrEmpty(adhocRequest.Comments))
            {
                adhocRequest.Comments = adhocRequest.Comments.Trim().Replace("\n", "<br/>");
                LabelRequestReason.Text = adhocRequest.Comments;
            }
            #endregion
        }



        private List<string> GetUserExistingEntitlements(long UserId, long endPointId)
        {
            List<string> roles = new List<string>();
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("DIUserId", UserId.ToString());
            parameter.Add("endpointsystemId", endPointId.ToString());
            var userInfo = DomainApiClient<SAPUserinfo>.GetByAction<SAPUserinfo>("GetSAPUserByDIUserId", parameter);
            if (userInfo != null)
            {
                roles = userInfo.Roles;
            }
            return roles;
        }
        //private List<AdHocAccessRequest> GetFromInterim(long UserID)
        //{
        //    AdHocAccessRequest adhocaccessrequest = new AdHocAccessRequest();
        //    List<AdHocAccessRequest> listadhoc = new List<AdHocAccessRequest>();
        //    Dictionary<string, string> parameter = new Dictionary<string, string>();
        //    parameter.Add("itemType", InterimStoreItemType.AccessRequest.ToString());
        //    IList<InterimStore> listinterim = DomainApiClient<Dto.InterimStore>.GetByAction<List<Dto.InterimStore>>(DomainApiAction.Tasks.InterimStore.GetByType, parameter);
        //    if (listinterim != null)
        //    {
        //        for (int i = 0; i < listinterim.Count; i++)
        //        {
        //            adhocaccessrequest = UtilityMethods.Deserialize<AdHocAccessRequest>(listinterim[i].XmlData);
        //            adhocaccessrequest.Id = listinterim[i].Id;
        //            if (adhocaccessrequest.RequesteeUserId == UserID)
        //                listadhoc.Add(adhocaccessrequest);
        //        }
        //    }
        //    return listadhoc;
        //}
        //private List<AdHocAccessRequest> GetPendingRequest(long UserID)
        //{
        //    Dictionary<string, string> parameter = new Dictionary<string, string>();
        //    parameter.Add("requesteeUserId", UserID.ToString());
        //    List<AdHocAccessRequest> listpendingrequest = DomainApiClient<Dto.AdHocAccessRequest>.GetByAction<List<AdHocAccessRequest>>(DomainApiAction.AdhocRequest.GetPendingByRequesteeUserId, parameter);
        //    return listpendingrequest;
        //}
        //private List<PresentationAccessRequestItem> FilterSource(PresentationAccessRequestFilterCriteria Criteria)
        //{
        //    var interimlist = GetFromInterim(AdhocAccessRequest.RequesteeUserId);
        //    var pendinglist = GetPendingRequest(AdhocAccessRequest.RequesteeUserId);
        //    var returnlist = DomainApiClient<PresentationAccessRequestItem>.
        //       GetByAction<List<PresentationAccessRequestItem>>
        //       (DomainApiAction.Endpoint.Presentation.GetByCriteria, null, Criteria);

        //    // InterimFilter
        //    returnlist = (from items in returnlist
        //                  where
        //                  items.AccessRequestItemType == AccessRequestItemType.Application ?
        //                  !interimlist.Where(lq => (lq.AdhocActionType == (Criteria.RemoveAccess ? AdHocActionType.DeleteAccount : AdHocActionType.NewAccount))).Select(lq => lq.EndpointSystemId).Contains(items.EndpointSystemId) :
        //                  !interimlist.Where(lq => (lq.AdhocActionType == (Criteria.RemoveAccess ? AdHocActionType.AmendmentRemove : AdHocActionType.AmendmentAdd)) && lq.EndpointSystemId == items.EndpointSystemId).Select(lq => lq.Entitlement).ToList().Any(l => l != null && l.Contains(items.ItemName))
        //                  select items
        //                      ).ToList();
        //    // End
        //    // RequestFilter
        //    returnlist = (from items in returnlist
        //                  where
        //                  items.AccessRequestItemType == AccessRequestItemType.Application ?
        //                  !pendinglist.Where(lq => (lq.AdhocActionType == (Criteria.RemoveAccess ? AdHocActionType.DeleteAccount : AdHocActionType.NewAccount))).Select(lq => lq.EndpointSystemId).Contains(items.EndpointSystemId) :
        //                  !pendinglist.Where(lq => (Criteria.RemoveAccess ? lq.AdhocActionType == AdHocActionType.AmendmentRemove : (lq.AdhocActionType == AdHocActionType.AmendmentAdd || lq.AdhocActionType == AdHocActionType.NewAccount)) && lq.EndpointSystemId == items.EndpointSystemId).Select(lq => lq.Entitlement).ToList().Any(l => l != null && l.Contains(items.ItemName))
        //                  select items
        //                      ).ToList();
        //    // End

        //    // AccountFilter
        //    if (!Criteria.RemoveAccess)
        //    {
        //        var accountlist = GetAccount(AdhocAccessRequest.RequesteeUserId);
        //        returnlist = (from items in returnlist
        //                      where
        //                      items.AccessRequestItemType == AccessRequestItemType.Application ?
        //                      !accountlist.Where(lq => lq.Type == "User" && lq.AccountType == AccountType.User.ToString()).Select(lq => lq.EndpointHostName).Contains(items.ItemName) :
        //                      !accountlist.Where(lq => lq.Type != "User" && lq.AccountType == AccountType.User.ToString()).Select(lq => lq.Name).ToList().Any(l => l != null && l.Contains(items.ItemName))
        //                      select items
        //                     ).ToList();
        //    }
        //    // End
        //    return returnlist;

        //}
        //private List<Dto.Presentation.PresentationUserAccessDetail> GetAccount(long UserID)
        //{
        //    Dictionary<string, string> parameter = new Dictionary<string, string>();
        //    parameter.Add("userId", UserID.ToString());
        //    return DomainApiClient<Dto.Presentation.PresentationUserAccessDetail>.
        //        GetByAction<List<Dto.Presentation.PresentationUserAccessDetail>>
        //        (DomainApiAction.Endpoint.Presentation.PresentationUserAccessDetail.GetUserAccessDetail, parameter);
        //}
        #endregion


    }
}