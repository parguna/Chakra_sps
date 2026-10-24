using Application;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DI.Chakra.Web;
using Telerik.Web.UI;
using DI.Chakra.DBConfigHelper;

namespace DI.Chakra.Web.NACR.Create.App
{
    public partial class RequestItemList : UIBaseUC
    {
        private const string V_REQUESTINFO = "V_REQUESTINFO";
        private const string V_REQUESTLIST = "V_REQUESTLIST";
        private const string V_CRITERIA = "V_CRITERIA";
        private const string V_SODAccessRequest = "V_SODAccessRequest";
        public event EventHandler ItemClicked;
        public static string sortOrder = "ASC";
        public bool RefreshEndpointTree
        {
            set
            {
                FilterUC.Initialize();
            }
        }
        public Dto.AdHocAccessRequest AdhocAccessRequest
        {
            get
            {
                if (ViewState[V_REQUESTINFO] == null)
                    return null;
                return (Dto.AdHocAccessRequest)ViewState[V_REQUESTINFO];
            }
            set
            {
                ViewState[V_REQUESTINFO] = value;
            }
        }
        public IList<PresentationAccessRequestItem> Items
        {
            get
            {
                if (ViewState[V_REQUESTLIST] == null)
                    return null;
                return (IList<PresentationAccessRequestItem>)ViewState[V_REQUESTLIST];
            }
            set
            {
                ViewState[V_REQUESTLIST] = value;
            }
        }
        public List<UserSODComplianceReport> SODAccessRequestList
        {
            get
            {
                if (ViewState[V_SODAccessRequest] == null)
                    return new List<UserSODComplianceReport>();
                return (List<UserSODComplianceReport>)ViewState[V_SODAccessRequest];
            }
            set
            {
                ViewState[V_SODAccessRequest] = value;
            }
        }
        public Dto.Presentation.PresentationAccessRequestFilterCriteria AccessRequestFilterCriteria
        {
            get
            {
                if (ViewState[V_CRITERIA] == null)
                    ViewState[V_CRITERIA] = new Dto.Presentation.PresentationAccessRequestFilterCriteria();

                return (Dto.Presentation.PresentationAccessRequestFilterCriteria)ViewState[V_CRITERIA];
            }
            set
            {
                ViewState[V_CRITERIA] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        public void Initialize()
        {
            requestList.DataSource = BindSource();
        }

        private List<AdHocAccessRequest> GetPendingRequest(long UserID)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("requesteeUserId", UserID.ToString());
            List<AdHocAccessRequest> listpendingrequest = DomainApiClient<Dto.AdHocAccessRequest>.GetByAction<List<AdHocAccessRequest>>(DomainApiAction.AdhocRequest.GetPendingByRequesteeUserId, parameter);
            return listpendingrequest;
        }

        private List<Dto.Presentation.PresentationUserAccessDetail> GetAccount(long UserID)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", UserID.ToString());
            return DomainApiClient<Dto.Presentation.PresentationUserAccessDetail>.
                GetByAction<List<Dto.Presentation.PresentationUserAccessDetail>>
                (DomainApiAction.Endpoint.Presentation.PresentationUserAccessDetail.GetUserAccessDetail, parameter);
        }

        public List<AdHocAccessRequest> GetFromInterim(long UserID, InterimStoreItemType interimType)
        {
            AdHocAccessRequest adhocaccessrequest = new AdHocAccessRequest();
            List<AdHocAccessRequest> listadhoc = new List<AdHocAccessRequest>();
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("itemType", interimType.ToString());
            IList<InterimStore> listinterim = DomainApiClient<Dto.InterimStore>.GetByAction<List<Dto.InterimStore>>
                (DomainApiAction.Tasks.InterimStore.GetByType, parameter);
            if (listinterim != null)
            {
                for (int i = 0; i < listinterim.Count; i++)
                {
                    adhocaccessrequest = UtilityMethods.Deserialize<AdHocAccessRequest>(listinterim[i].XmlData);
                    adhocaccessrequest.Id = listinterim[i].Id;
                    if (adhocaccessrequest.RequesteeUserId == UserID)
                        listadhoc.Add(adhocaccessrequest);
                }
            }
            return listadhoc;
        }

        private List<PresentationAccessRequestItem> FilterSource(PresentationAccessRequestFilterCriteria Criteria)
        {
            //var interimlist = GetFromInterim(this.AdhocAccessRequest.RequesteeUserId, InterimStoreItemType.AccessRequest);
            //var pendinglist = GetPendingRequest(this.AdhocAccessRequest.RequesteeUserId);
            var returnlist = DomainApiClient<PresentationAccessRequestItem>.
               GetByAction<List<PresentationAccessRequestItem>>
               (DomainApiAction.Endpoint.Presentation.GetByCriteria, null, Criteria).Select(c => { c.RequestFor = AdhocAccessRequest.RequesteeUserId; c.RemoveAccess = Criteria.RemoveAccess; return c; }).ToList();


            ////disable the user account add option by default, only need to show the spl account features
            //if (returnlist.Any(a => a.AccessItemType == AccessItemType.Application) && !Criteria.RemoveAccess)
            //{
            //    returnlist.Where(a => a.AccessItemType == AccessItemType.Application).ToList().ForEach(a => { a.Enabled = false; });
            //}

            //// InterimFilter   
            //var interimAccountAccessRequestList = interimlist.Where(lq => (lq.AdhocActionType == (Criteria.RemoveAccess ? AdHocActionType.DeleteAccount : AdHocActionType.NewAccount))).ToList();
            //var interimEntitlementAccessRequestList = interimlist.Where(lq => (lq.AdhocActionType == (Criteria.RemoveAccess ? AdHocActionType.AmendmentRemove : AdHocActionType.AmendmentAdd))).ToList();

            ////update the enable flag 
            //foreach (var item in interimAccountAccessRequestList)
            //{
            //    if (item.EndPoint == null || item.EndPoint.IsMockEndPoint) continue;
            //    var temp = returnlist.Where(a => a.EndpointSystemId == item.EndpointSystemId && a.AccountType == (Criteria.RemoveAccess ? item.AccountType : AccountType.User)
            //                && a.AccessRequestItemType == AccessRequestItemType.Application && item.EndpointSystemId == a.EndpointSystemId);
            //    if (temp.Any())
            //        temp.First().Enabled = false;
            //}

            //foreach (var item in interimAccountAccessRequestList)
            //{
            //    if (item.EndPoint == null || item.EndPoint.IsMockEndPoint) continue;
            //    var entilements = item.GetAccessRequestDetail();
            //    if (entilements == null) continue;
            //    foreach (var entitlement in entilements)
            //    {
            //        var requestedItem = item.GetAccessRequestDetail().First();
            //        var temp = returnlist.Where(a => a.EndpointSystemId == item.EndpointSystemId && a.AccountType == (Criteria.RemoveAccess ? item.AccountType : AccountType.User)
            //                && requestedItem.AccessRequestItemType == a.AccessRequestItemType && entitlement.KeyId == a.KeyId
            //                && requestedItem.AccessItemType == a.AccessItemType
            //                && requestedItem.EntitlementType == a.EntitlementType);
            //        if (temp.Any())
            //            temp.First().Enabled = false;
            //    }
            //}

            //foreach (var item in interimEntitlementAccessRequestList)
            //{
            //    if (!Criteria.RemoveAccess && (item.EndPoint == null || item.EndPoint.IsMockEndPoint)) continue;
            //    var entilements = item.GetAccessRequestDetail();
            //    if (entilements == null) continue;
            //    foreach (var entitlement in entilements)
            //    {
            //        var requestedItem = item.GetAccessRequestDetail().First();
            //        var temp = returnlist.Where(a => a.EndpointSystemId == item.EndpointSystemId && a.AccountType == (Criteria.RemoveAccess ? item.AccountType : AccountType.User)
            //                && requestedItem.AccessRequestItemType == a.AccessRequestItemType && entitlement.KeyId == a.KeyId
            //                && requestedItem.AccessItemType == a.AccessItemType
            //                && requestedItem.EntitlementType == a.EntitlementType);
            //        if (temp.Any())
            //            temp.First().Enabled = false;
            //    }
            //}

            //// End
            //// RequestFilter
            //var pendingAccountAccessRequestList = pendinglist.Where(lq => (lq.AdhocActionType == (Criteria.RemoveAccess ? AdHocActionType.DeleteAccount : AdHocActionType.NewAccount))).ToList();
            //var pendingEntitlementAccessRequestList = pendinglist.Where(lq => (lq.AdhocActionType == (Criteria.RemoveAccess ? AdHocActionType.AmendmentRemove : AdHocActionType.AmendmentAdd))).ToList();

            //foreach (var item in pendingAccountAccessRequestList)
            //{
            //    if (item.EndPoint == null || item.EndPoint.IsMockEndPoint) continue;
            //    var temp = returnlist.Where(a => a.EndpointSystemId == item.EndpointSystemId && a.AccountType == (Criteria.RemoveAccess ? item.AccountType : AccountType.User)
            //    && a.AccessRequestItemType == AccessRequestItemType.Application && item.EndpointSystemId == a.EndpointSystemId);
            //    if (temp.Any())
            //        temp.First().Enabled = false;
            //}

            ////update the enable flag 
            //foreach (var item in pendingAccountAccessRequestList)
            //{
            //    if (item.EndPoint == null || item.EndPoint.IsMockEndPoint) continue;
            //    var entilements = item.GetAccessRequestDetail();
            //    if (entilements == null) continue;
            //    foreach (var entitlement in entilements)
            //    {
            //        var requestedItem = item.GetAccessRequestDetail().First();
            //        var temp = returnlist.Where(a => a.EndpointSystemId == item.EndpointSystemId && a.AccountType == (Criteria.RemoveAccess ? item.AccountType : AccountType.User)
            //            && a.AccessRequestItemType == requestedItem.AccessRequestItemType && entitlement.KeyId == a.KeyId
            //            && requestedItem.AccessItemType == a.AccessItemType
            //            && requestedItem.EntitlementType == a.EntitlementType);
            //        if (temp.Any())
            //            temp.First().Enabled = false;
            //    }
            //}

            //foreach (var item in pendingEntitlementAccessRequestList)
            //{
            //    if (!Criteria.RemoveAccess && (item.EndPoint == null || item.EndPoint.IsMockEndPoint)) continue;
            //    var entilements = item.GetAccessRequestDetail();
            //    if (entilements == null) continue;
            //    foreach (var entitlement in entilements)
            //    {
            //        var requestedItem = item.GetAccessRequestDetail().First();
            //        var temp = returnlist.Where(a => a.EndpointSystemId == item.EndpointSystemId && a.AccountType == (Criteria.RemoveAccess ? item.AccountType : AccountType.User)
            //        && a.AccessRequestItemType == requestedItem.AccessRequestItemType && entitlement.KeyId == a.KeyId
            //        && requestedItem.AccessItemType == a.AccessItemType
            //        && requestedItem.EntitlementType == a.EntitlementType);
            //        if (temp.Any())
            //            temp.First().Enabled = false;
            //    }
            //}

            //// AccountFilter
            //if (!Criteria.RemoveAccess)
            //{
            //    var accountlist = GetAccount(AdhocAccessRequest.RequesteeUserId);

            //    //update the enable flag 
            //    foreach (var item in accountlist)
            //    {
            //        var temp = returnlist.Where(a => a.EndpointSystemId == item.EndpointSystemID && a.AccountType == AccountType.User
            //        && a.AccessRequestItemType == AccessRequestItemType.Application && item.EndpointHostName == a.EndpointName);
            //        if (temp.Any())
            //            temp.First().Enabled = false;
            //    }
            //}

            return returnlist;

        }
        protected void RequestRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            RequestItemUC RequestItemUC = (RequestItemUC)e.Item.FindControl("RequestItemUC");
            RequestItemUC.PresentationAccessRequestItem = (PresentationAccessRequestItem)e.Item.DataItem;
            RequestItemUC.Initialize();
        }

        private void CreateAccountRequestIfNotAvailable(IAccessRequestItem requestedItem, EndPoint requestedEndpoint)
        {
            var endpointId = requestedEndpoint.Id; var parentEpId = requestedEndpoint.Id;
            AccessRequestBuilderBase arBase = new AccessRequestBuilderBase();
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", this.AdhocAccessRequest.RequesteeUserId.ToString());
            parameter.Add("accountType", AccountType.User.ToString());
            parameter.Add("endpointSystemId", endpointId.ToString());
            parameter.Add("parentEpId", parentEpId.ToString());
            var acntRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<long>
                (DomainApiAction.Provisioning.AccountReconcilation.GetByAccountType, parameter);
           // bool accExistsInInterim = CheckAccountRequestAvailableInInterim(requestedItem);
           // bool isOneWayTrust = false; //CheckIsOneWayTrust(endpointId);

            //check for accessrequest which is not complete yet with new account            
            //var accessRequestForNewAccount = DomainApiClient<Dto.AccessRequest>.GetByAction<List<Dto.AccessRequest>>
            //    (DomainApiAction.Provisioning.AccessRequest.GetByAccountType, parameter);

            if (acntRecon == 0)
            {
                AdHocAccessRequest newAccountRequest = this.AdhocAccessRequest;

                newAccountRequest.AdhocActionType = AdHocActionType.NewAccount;
                newAccountRequest.EntitlementDetail = string.Empty;

                if (arBase.IsExists(newAccountRequest) == false)
                    arBase.Save(newAccountRequest);
            }
            else if (acntRecon > 0)
            {
                requestedItem.ParentAccessRequestId = acntRecon;
            }
        }

        public bool CheckIsOneWayTrust(long endpointSystemId)
        {
            bool result = false;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("key", "IsOneWayTrust");
            parameter.Add("value", Boolean.TrueString);
            List<Dto.EndPoint> isOneWayTrustEps = DomainApiClient<Dto.EndPoint>.GetByAction<List<Dto.EndPoint>>
                (DomainApiAction.Endpoint.EndPoint.GetByEndpointConfiguration, parameter);
            if (isOneWayTrustEps != null && isOneWayTrustEps.Count > 0)
                if (isOneWayTrustEps.Any(x => x.Id == endpointSystemId))
                    result = true;
            return result;
        }

        private bool CheckAccountRequestAvailableInInterim(IAccessRequestItem requestedItem)
        {
            var interimlist = this.GetFromInterim(this.AdhocAccessRequest.RequesteeUserId, InterimStoreItemType.AccessRequest);
            var interimAccountAccessRequestList = interimlist.Where(lq => lq.AdhocActionType == AdHocActionType.NewAccount
                                    && lq.EndpointSystemId == requestedItem.EndpointSystemId && lq.AccountType == requestedItem.AccountType).ToList();
            if (interimAccountAccessRequestList.Any())
                return true;
            else
                return false;
        }

        private void ProcessRequestedItem(IAccessRequestItem requestedItem)
        {
            Dictionary<string, string> endpointParameter = new Dictionary<string, string>();
            endpointParameter.Add("id", requestedItem.EndpointSystemId.ToString());

            var requestedEndpoint = DomainApiClient<Dto.EndPoint>.GetByAction<Dto.EndPoint>
                       (DomainApiAction.Endpoint.EndPoint.GetByIdIncludingEndPointSystemType, endpointParameter);

            this.AdhocAccessRequest.EndpointSystemId = requestedItem.EndpointSystemId;
            this.AdhocAccessRequest.EndpointSystemType = requestedItem.EndpointType;
            this.AdhocAccessRequest.EndPoint = requestedEndpoint;
            this.AdhocAccessRequest.AccountType = requestedItem.AccountType;

            var accountNotNeededForEndpoints = "".Split(',');
            AccessRequestBuilderBase arBase = new AccessRequestBuilderBase();
            if (requestedItem.AccessRequestItemType == AccessRequestItemType.Entitlement)
            {
                if (this.AdhocAccessRequest.IsAddAccess)
                    this.AdhocAccessRequest.AdhocActionType = AdHocActionType.AmendmentAdd;
                else
                    this.AdhocAccessRequest.AdhocActionType = AdHocActionType.AmendmentRemove;
            }
            else
            {
                if (this.AdhocAccessRequest.IsAddAccess)
                    this.AdhocAccessRequest.AdhocActionType = AdHocActionType.NewAccount;
                else
                    this.AdhocAccessRequest.AdhocActionType = AdHocActionType.DeleteAccount;
            }

            this.AdhocAccessRequest.SetRequestedItems = new List<PresentationAccessRequestItem> { (PresentationAccessRequestItem)requestedItem };
            arBase.Save(this.AdhocAccessRequest);
        }

        bool IsUserHasSODConflictForCurrentRequest(List<UserSODComplianceReport> userSODComplianceReport, string requestedRole, out int riskScore, out string conflictPair, out string sensitiveRole)
        {
            riskScore = 0; conflictPair = ""; sensitiveRole = "";
            if (userSODComplianceReport != null && userSODComplianceReport.Count > 0)
            {
                var newConflicts = userSODComplianceReport.Where(a => a.Type == "New" && (a.ConflictEntitlement1 == requestedRole || a.ConflictEntitlement2 == requestedRole));
                if (newConflicts != null && newConflicts.Count() > 0)
                {
                    foreach (UserSODComplianceReport conflict in newConflicts)
                    {
                        riskScore = Math.Max(conflict.RiskScore, riskScore);
                        if (conflict.IsSensitiveRole)
                            sensitiveRole += string.Format("{0}, ", conflict.ConflictEntitlement1);
                        else
                            conflictPair += string.Format("{0}-{1}, ", conflict.ConflictEntitlement1, conflict.ConflictEntitlement2);
                    }
                    conflictPair = conflictPair.Trim().TrimEnd(',');
                    sensitiveRole = sensitiveRole.Trim().TrimEnd(',');
                    return true;
                }
            }
            return false;
        }

        private bool HasSODConflict(PresentationAccessRequestItem requestedItem, out string message)
        {
            message = string.Empty;

            if (requestedItem.RemoveAccess)
            {
                return false;
            }
            //run the User SOD Conflict now and get the list conflicts and its riskscore
            //based on the risk code just dont add the request to cart.
            bool isCurrentRequestHasConflict = false;
            UserSODComplianceRequest request = new UserSODComplianceRequest();
            request.EndpointSystemId = requestedItem.EndpointSystemId;
            request.EndpointTypeValue = EndpointType.None;

            request.EndpointTypeValue = requestedItem.EndpointType;
            request.EntitlementDetails = requestedItem.ItemName;
            request.PolicyId = 0;
            request.IsFromAccessRequest = true;
            request.UserId = new SessionStatusImpl().UserId;
            request.RequesteeUserId = this.AdhocAccessRequest.RequesteeUserId;

            var roleConflictDetails = DomainApiClient<UserSODComplianceReport>.GetByAction<List<UserSODComplianceReport>>(DomainApiAction.Complaince.CompliancePolicy.Policy.GetUserSODComplianceReport, null, request);
            SODAccessRequestList = roleConflictDetails;

            string conflictPair = ""; int riskScore = 0; string sensitiveRole = "";
            isCurrentRequestHasConflict = IsUserHasSODConflictForCurrentRequest(roleConflictDetails, request.EntitlementDetails, out riskScore, out conflictPair, out sensitiveRole);

            if (isCurrentRequestHasConflict)
            {
                string messageToShow = "";
                if (!string.IsNullOrEmpty(conflictPair)) messageToShow = string.Format("Role Conflict - {0}", conflictPair);
                if (!string.IsNullOrEmpty(sensitiveRole)) messageToShow += string.Format("Sensitive Role - {0}", sensitiveRole);
                message = messageToShow;
                #region To validate against SOD Configuration
                List<SODPolicyConfiguration> policyConfiguration = DomainApiClient<SODPolicyConfiguration>.GetAll().ToList();
                bool canAllowRequest = true;
                foreach (UserSODComplianceReport report in roleConflictDetails)
                {
                    foreach (SODPolicyConfiguration config in policyConfiguration)
                    {
                        bool isScoresatisfied = true;
                        Infrastructure.Enums.Operator operate = (Operator)Enum.Parse(typeof(Operator), config.Operator);
                        Infrastructure.Enums.RiskScoreOperator riskoper = (RiskScoreOperator)Enum.Parse(typeof(RiskScoreOperator), config.RiskScoreOperator);
                        SODRiskLevel riskLevel = DomainApiClient<SODRiskLevel>.GetById(config.RiskLevelId);
                        if (Infrastructure.Enums.RiskScoreOperator.Greaterthan.Equals(riskoper))
                        {
                            isScoresatisfied = report.RiskScore > config.RiskScore;
                        }
                        else if (Infrastructure.Enums.RiskScoreOperator.Lessthan.Equals(riskoper))
                        {
                            isScoresatisfied = report.RiskScore < config.RiskScore;
                        }
                        else if (Infrastructure.Enums.RiskScoreOperator.Equalto.Equals(riskoper))
                        {
                            isScoresatisfied = report.RiskScore == config.RiskScore;
                        }

                        if (Infrastructure.Enums.Operator.AND == operate)
                        {
                            if (riskLevel.RiskLevelCode.ToString().Equals(report.RiskLevel) && isScoresatisfied)
                            {
                                canAllowRequest = false;
                                break;
                            }
                        }
                        if (Infrastructure.Enums.Operator.OR == operate)
                        {
                            if (config.RiskLevelId.ToString().Equals(report.RiskLevel) || isScoresatisfied)
                            {
                                canAllowRequest = false;
                                break;
                            }
                        }
                    }
                    if (!canAllowRequest)
                        break;
                }
                if (!canAllowRequest)
                {
                    MasterPage.ShowAlert(AlertType.Error, messageToShow);
                    return true;
                }
                #endregion
                else
                {
                    MasterPage.ShowAlert(AlertType.Warning, messageToShow);
                    return false;
                }
            }
            return isCurrentRequestHasConflict;
        }

        protected void RequestItemUC_ItemClicked(object sender, EventArgs e)
        {
            PresentationAccessRequestItem item = (PresentationAccessRequestItem)sender;
            string message = string.Empty;
            bool IsSODConflict = HasSODConflict(item, out message);

            if (IsSODConflict == false)
            {
                ProcessRequestedItem(item);
                if (item.AccessRequestItemType == AccessRequestItemType.Application && !this.AdhocAccessRequest.IsAddAccess)
                {
                    //this.AdhocAccessRequest.
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("accountReconcilationId", item.KeyId.ToString());
                    parameter.Add("userId", item.RequestFor.ToString());
                    parameter.Add("epId", item.EndpointSystemId.ToString());

                    var returnlist = DomainApiClient<PresentationAccessRequestItem>.
             GetByAction<List<PresentationAccessRequestItem>>(DomainApiAction.Endpoint.Presentation.PresentationAccessRequestItem.GetByAccountReconcilationId, parameter);
                    if (returnlist != null && returnlist.Count > 0)
                    {
                        foreach (var entitlementItem in returnlist)
                        {
                            entitlementItem.RemoveAccess = true;
                            entitlementItem.RequestFor = item.RequestFor;
                            ProcessRequestedItem(entitlementItem);
                        }
                    }
                }
                //if(this.AdhocAccessRequest.)
                if (!string.IsNullOrEmpty(message))
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Item Added Successfully-" + message);
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Success, "Item Added Successfully");
                }

                requestList.Rebind();

                if (ItemClicked != null)
                    ItemClicked(this, e);
            }
        }

        protected void requestList_ItemDataBound(object sender, Telerik.Web.UI.RadListViewItemEventArgs e)
        {

            Telerik.Web.UI.RadListViewDataItem dataItem = (Telerik.Web.UI.RadListViewDataItem)e.Item;

            if (e.Item.ItemType == RadListViewItemType.DataItem || e.Item.ItemType == RadListViewItemType.AlternatingItem)
            {
                PresentationAccessRequestItem yourDataSource = (PresentationAccessRequestItem)dataItem.DataItem;

                RequestItemUC RequestItemUC = (RequestItemUC)e.Item.FindControl("RequestItemUC");
                RequestItemUC.PresentationAccessRequestItem = yourDataSource;
                RequestItemUC.Initialize();
            }
        }

        protected void requestList_NeedDataSource(object sender, RadListViewNeedDataSourceEventArgs e)
        {
            if (this.AdhocAccessRequest == null)
                return;

            this.Initialize();
        }

        protected void requestList_Sorting(object sender, RadListViewSortEventArgs e)
        {
            requestList.SortExpressions.Clear();
            List<PresentationAccessRequestItem> data = BindSource();
            if (data != null && data.Count > 0)
                if (sortOrder == "ASC")
                {
                    requestList.DataSource = data.OrderBy(x => x.EntitlementType);
                    sortOrder = "DESC";
                }
                else
                {
                    requestList.DataSource = data.OrderByDescending(x => x.EntitlementType);
                    sortOrder = "ASC";
                }
            requestList.Rebind();
        }

        private List<PresentationAccessRequestItem> BindSource()
        {
            if (FilterUC.EntitlementTypes.Count <= 0)
            {
                MasterPage.ShowAlert(AlertType.Warning, "Please select subsystem and do search");
                return new List<PresentationAccessRequestItem>();
            }

            var criteria = AccessRequestFilterCriteria;
            criteria.RemoveAccess = false;
            criteria.IncludeApplications = false; //(FilterUC.ApplicationTypes.Count > 0 || FilterUC.EntitlementTypes.Count == 0);
            criteria.IncludeEntitlementTypes = false;
            if (AdhocAccessRequest.IsAddAccess == false)
            {
                criteria.RemoveAccess = true;
                criteria.IncludeApplications = true;
                criteria.ApplicationEndpointSystems.AddRange(FilterUC.EntitlementTypes);
            }
            criteria.DIUserId = AdhocAccessRequest.RequesteeUserId;
            criteria.DIRequesterUserId = AdhocAccessRequest.RequesterUserId;
            criteria.ApplicationEndpointSystems.Clear();
            criteria.EntitlementEndpointSystems.Clear();
            criteria.EntitlementTypes.Clear();
            //criteria.ApplicationEndpointSystems.AddRange(FilterUC.ApplicationTypes);
            criteria.EntitlementEndpointSystems.AddRange(FilterUC.EntitlementTypes);
            criteria.IncludeEntitlements = (FilterUC.EntitlementTypes.Count > 0);
            criteria.IncludeEntitlementTypes = (FilterUC.EntitlementSubTypes.Count > 0);
            criteria.EntitlementTypes.AddRange(FilterUC.EntitlementSubTypes);
            AccessRequestFilterCriteria = criteria;
            List<PresentationAccessRequestItem> items = FilterSource(criteria);
            return items.OrderByDescending(x => x.AccessItemType).ThenBy(x=>x.EntitlementType).ThenBy(x => x.ItemDisplayName).ToList();
        }

        protected void SortAccessName_Command(object sender, CommandEventArgs e)
        {
            String commandArgument = e.CommandArgument.ToString();
            var propertyInfo = typeof(PresentationAccessRequestItem).GetProperty(commandArgument);
            requestList.SortExpressions.Clear();
            LinkButton ctl;
            ctl = (LinkButton)sender;
            ImageButton ib = (ImageButton)ctl.Parent.FindControl(commandArgument + "Asc");

            List<PresentationAccessRequestItem> data = BindSource();
            if (data != null && data.Count > 0)
            {
                if (sortOrder == "ASC")
                {
                    requestList.DataSource = data.OrderBy(x => propertyInfo.GetValue(x, null));
                    sortOrder = "DESC";
                    ib.CssClass = "rgSortDesc";
                }
                else
                {
                    requestList.DataSource = data.OrderByDescending(x => propertyInfo.GetValue(x, null));
                    sortOrder = "ASC";
                    ib.CssClass = "rgSortAsc";
                }
            }
            requestList.DataBind();
        }

        //protected void RequestRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        //{
        //    if (e.CommandName == "AddCart")
        //    {
        //        RequestItemUC RequestItemUC = (RequestItemUC)e.Item.FindControl("RequestItemUC");

        //        PresentationAccessRequestItem item = RequestItemUC.PresentationAccessRequestItem;

        //        bool hasSODConflict = IsSODCompliant(item);

        //        if (hasSODConflict == false)
        //        {
        //            ProcessRequestedItem(item);

        //            if (ItemClicked != null)
        //                ItemClicked(this, e);

        //        }

        //    }
        //}
    }
}