using Application;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Web
{
    public class AdhocAccessRequestValidation
    {
        public static IList<AdHocAccessRequest> GetFromInterim(AccountType accountType)
        {
            AdHocAccessRequest adhocaccessrequest = new AdHocAccessRequest();
            IList<AdHocAccessRequest> listadhoc = new List<AdHocAccessRequest>();

            IList<Dto.InterimStore> listinterim;
            if (accountType != AccountType.User)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("isSaved", Boolean.TrueString);
                parameter.Add("itemType", InterimStoreItemType.AccessRequest.ToString());
                listinterim = DomainApiClient<Dto.InterimStore>.GetByAction<List<Dto.InterimStore>>(DomainApiAction.Tasks.InterimStore.GetForSpecialAccounts, parameter);
            }
            else
            {
                SessionStatusImpl sessionStatus = new SessionStatusImpl();
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("userId", sessionStatus.User.Id.ToString());
                parameter.Add("isSaved", Boolean.TrueString);
                parameter.Add("itemType", InterimStoreItemType.AccessRequest.ToString());
                listinterim = DomainApiClient<Dto.InterimStore>.GetByAction<List<Dto.InterimStore>>(DomainApiAction.Tasks.InterimStore.GetByUserId, parameter);
            }

            if (listinterim != null)
            {
                for (int i = 0; i < listinterim.Count; i++)
                {
                    adhocaccessrequest = UtilityMethods.Deserialize<AdHocAccessRequest>(listinterim[i].XmlData);

                    adhocaccessrequest.Id = listinterim[i].Id;

                    Dictionary<string, string> eparameter = new Dictionary<string, string>();
                    eparameter.Add("id", adhocaccessrequest.EndpointSystemId.Value.ToString());

                    DomainApiClient<Dto.EndPoint>.GetByAction<Dto.EndPoint>(DomainApiAction.Endpoint.EndPoint.GetByIdIncludingOwner, eparameter);


                    listadhoc.Add(adhocaccessrequest);
                }
            }
            return listadhoc;
        }

        public static bool IsExists(AdHocAccessRequest adhocAccessRequest, out string ValidationMessage)
        {
            List<ApplicationType> applicationTypesForAmendAdd = new List<ApplicationType>() { ApplicationType.GEMS, ApplicationType.SAP_GRC, ApplicationType.EBS_CRM, ApplicationType.EBS_ISU, ApplicationType.MSSL_CRM, ApplicationType.MSSL_ISU, ApplicationType.SSIDS_GUI, ApplicationType.SSIDS_SMS, ApplicationType.FLINS_BI, ApplicationType.MSSL, ApplicationType.EBS };

            List<ApplicationType> specialApplicationTypesForAmendAdd = new List<ApplicationType>() { ApplicationType.EBS_CRM, ApplicationType.EBS_ISU, ApplicationType.MSSL_CRM, ApplicationType.MSSL_ISU, ApplicationType.MSSL, ApplicationType.EBS };

            ValidationMessage = string.Empty;

            AdHocAccessRequest isExistsRequestInInterimStore;
            long RequesterID = 0;
            long AccessRequestID = 0;
            long RequesteeID = adhocAccessRequest.RequesteeUserId;
            string accountName = null;
            string adhocEntitlement = null;
            long endPointUserId = 0;
            var adhocEntitlementDetail = adhocAccessRequest.GetEntitlementDetail();
            if (adhocEntitlementDetail != null)
            {
                if (adhocEntitlementDetail.SharedAccountDetail != null)
                {
                    accountName = adhocEntitlementDetail.SharedAccountDetail.AccountName;
                }
                adhocEntitlement = adhocEntitlementDetail.Entitlement;
                endPointUserId = adhocEntitlementDetail.EndpointUserId;
            }

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", adhocAccessRequest.EndpointSystemId.Value.ToString());
            var currentRequestEndpoint = DomainApiClient<Dto.EndPoint>.GetByAction<Dto.EndPoint>(DomainApiAction.Endpoint.EndPoint.GetByIdIncludingOwner, param);

            IList<AdHocAccessRequest> InterimrequestsList = GetFromInterim(adhocAccessRequest.AccountType);

            if (InterimrequestsList != null && InterimrequestsList.Count > 0)
            {
                isExistsRequestInInterimStore = InterimrequestsList.Where(j => j.AccountType == adhocAccessRequest.AccountType).
                       FirstOrDefault(item =>
                       {

                           string itemAccountName = null;
                           string entitlement = null;
                           IEntitlementDetail entitlementDetail = item.GetEntitlementDetail();

                           if (entitlementDetail != null && entitlementDetail.SharedAccountDetail != null)
                           {
                               itemAccountName = entitlementDetail.SharedAccountDetail.AccountName;
                               entitlement = entitlementDetail.Entitlement;
                           }
                           if (adhocAccessRequest.AdhocActionType == AdHocActionType.NewAccount && adhocAccessRequest.ApplicationType == ApplicationType.SRM)
                           {
                               itemAccountName = (entitlementDetail as AdHocSAPSRMRequest)?.AccountName;
                               accountName = (adhocAccessRequest.GetEntitlementDetail() as AdHocSAPSRMRequest)?.AccountName;
                           }
                           if (adhocAccessRequest.AdhocActionType == AdHocActionType.NewAccount && !string.IsNullOrWhiteSpace(accountName))
                           {
                               return
                                item.RequesteeUserId == adhocAccessRequest.RequesteeUserId &&
                                item.AdhocActionType == adhocAccessRequest.AdhocActionType &&
                                !string.IsNullOrWhiteSpace(itemAccountName) && itemAccountName.Equals(accountName, StringComparison.InvariantCultureIgnoreCase) && item.EndpointSystemType == adhocAccessRequest.EndpointSystemType &&
                                item.AccountType == adhocAccessRequest.AccountType &&
                                item.EndpointSystemId == adhocAccessRequest.EndpointSystemId;
                           }
                           else if (adhocAccessRequest.AccountType == AccountType.User && adhocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
                           {
                               return item.RequesteeUserId == adhocAccessRequest.RequesteeUserId &&
                                item.AdhocActionType == adhocAccessRequest.AdhocActionType &&
                                item.EndpointSystemType == adhocAccessRequest.EndpointSystemType &&
                                item.AccountType == adhocAccessRequest.AccountType &&
                                item.EndpointSystemId == adhocAccessRequest.EndpointSystemId;
                           }
                           else if (currentRequestEndpoint != null && entitlementDetail != null && applicationTypesForAmendAdd.Contains(currentRequestEndpoint.ApplicationType) && !specialApplicationTypesForAmendAdd.Contains(currentRequestEndpoint.ApplicationType) && (adhocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove))
                           {
                               return entitlementDetail.EndpointUserId == adhocEntitlementDetail.EndpointUserId
                                        && item.EndpointSystemId == adhocAccessRequest.EndpointSystemId
                                        && item.AdhocActionType == adhocAccessRequest.AdhocActionType
                                        && item.AccountType == adhocAccessRequest.AccountType
                                        && !string.IsNullOrWhiteSpace(entitlementDetail.Entitlement)
                                        && !string.IsNullOrWhiteSpace(adhocEntitlement)
                                        && entitlementDetail.Entitlement.Split(',').ToList().Intersect(adhocEntitlement.Split(',')).ToList().Count > 0;
                           }
                           else if (currentRequestEndpoint != null && entitlementDetail != null && applicationTypesForAmendAdd.Contains(currentRequestEndpoint.ApplicationType) && specialApplicationTypesForAmendAdd.Contains(currentRequestEndpoint.ApplicationType) && (adhocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove))
                           {
                               List<ApplicationType> applicationTypes = null;
                               if ((currentRequestEndpoint.ApplicationType == ApplicationType.EBS_CRM || currentRequestEndpoint.ApplicationType == ApplicationType.EBS_ISU) &&
                                   (item.ApplicationType == ApplicationType.EBS_CRM || item.ApplicationType == ApplicationType.EBS_ISU))
                               {
                                   applicationTypes = new List<ApplicationType>() { ApplicationType.EBS_ISU, ApplicationType.EBS_CRM };

                                   if (!string.IsNullOrWhiteSpace(((AdHocEBSRequest)adhocEntitlementDetail).Position))
                                   {
                                       return applicationTypes.Contains(item.ApplicationType)
                                            && item.AdhocActionType == adhocAccessRequest.AdhocActionType
                                            && item.AccountType == adhocAccessRequest.AccountType
                                            && !string.IsNullOrWhiteSpace(((AdHocEBSRequest)adhocEntitlementDetail).Position)
                                            && !string.IsNullOrWhiteSpace(((AdHocEBSRequest)entitlementDetail).Position)
                                            && (((AdHocEBSRequest)adhocEntitlementDetail).Position) == (((AdHocEBSRequest)entitlementDetail).Position)
                                            && (((AdHocEBSRequest)adhocEntitlementDetail).AccName) == (((AdHocEBSRequest)entitlementDetail).AccName);
                                   }
                                   if (!string.IsNullOrWhiteSpace(((AdHocEBSRequest)adhocEntitlementDetail).Roles))
                                   {
                                       return entitlementDetail.EndpointUserId == adhocEntitlementDetail.EndpointUserId
                                            && item.EndpointSystemId == adhocAccessRequest.EndpointSystemId
                                            && item.AdhocActionType == adhocAccessRequest.AdhocActionType
                                            && item.AccountType == adhocAccessRequest.AccountType
                                            && !string.IsNullOrWhiteSpace(((AdHocEBSRequest)adhocEntitlementDetail).Roles)
                                            && !string.IsNullOrWhiteSpace(((AdHocEBSRequest)entitlementDetail).Roles)
                                            && (((AdHocEBSRequest)adhocEntitlementDetail).Roles).Split(',').ToList().Intersect((((AdHocEBSRequest)entitlementDetail).Roles).Split(',').ToList()).ToList().Count > 0;
                                   }
                               }
                               if ((currentRequestEndpoint.ApplicationType == ApplicationType.MSSL_CRM || currentRequestEndpoint.ApplicationType == ApplicationType.MSSL_ISU) &&
                                   (item.ApplicationType == ApplicationType.MSSL_CRM || item.ApplicationType == ApplicationType.MSSL_ISU))
                               {
                                   applicationTypes = new List<ApplicationType>() { ApplicationType.MSSL_ISU, ApplicationType.MSSL_CRM };

                                   if (!string.IsNullOrWhiteSpace(((AdHocMSSLRequest)adhocEntitlementDetail).Position))
                                   {
                                       return applicationTypes.Contains(item.ApplicationType)
                                            && item.AdhocActionType == adhocAccessRequest.AdhocActionType
                                            && item.AccountType == adhocAccessRequest.AccountType
                                            && !string.IsNullOrWhiteSpace(((AdHocMSSLRequest)adhocEntitlementDetail).Position)
                                            && !string.IsNullOrWhiteSpace(((AdHocMSSLRequest)entitlementDetail).Position)
                                            && (((AdHocMSSLRequest)adhocEntitlementDetail).Position) == (((AdHocMSSLRequest)entitlementDetail).Position)
                                            && (((AdHocMSSLRequest)adhocEntitlementDetail).AccName) == (((AdHocMSSLRequest)entitlementDetail).AccName);
                                   }
                                   if (!string.IsNullOrWhiteSpace(((AdHocMSSLRequest)adhocEntitlementDetail).Roles))
                                   {
                                       return entitlementDetail.EndpointUserId == adhocEntitlementDetail.EndpointUserId
                                            && item.EndpointSystemId == adhocAccessRequest.EndpointSystemId
                                            && item.AdhocActionType == adhocAccessRequest.AdhocActionType
                                            && item.AccountType == adhocAccessRequest.AccountType
                                            && !string.IsNullOrWhiteSpace(((AdHocMSSLRequest)adhocEntitlementDetail).Roles)
                                            && !string.IsNullOrWhiteSpace(((AdHocMSSLRequest)entitlementDetail).Roles)
                                            && (((AdHocMSSLRequest)adhocEntitlementDetail).Roles).Split(',').ToList().Intersect((((AdHocMSSLRequest)entitlementDetail).Roles).Split(',').ToList()).ToList().Count > 0;
                                   }
                               }
                               if (currentRequestEndpoint.ApplicationType == ApplicationType.EBS)
                               {
                                   applicationTypes = new List<ApplicationType>() { ApplicationType.EBS };

                                   return applicationTypes.Contains(item.ApplicationType)
                                           && item.AdhocActionType == adhocAccessRequest.AdhocActionType
                                           && item.AccountType == adhocAccessRequest.AccountType
                                           && !string.IsNullOrWhiteSpace(((AdHocSAPHANARequest)adhocEntitlementDetail).Position)
                                           && !string.IsNullOrWhiteSpace(((AdHocSAPHANARequest)entitlementDetail).Position)
                                           && (((AdHocSAPHANARequest)adhocEntitlementDetail).Position) == (((AdHocSAPHANARequest)entitlementDetail).Position)
                                           && (((AdHocSAPHANARequest)adhocEntitlementDetail).AccName) == (((AdHocSAPHANARequest)entitlementDetail).AccName);
                               }
                               else if (currentRequestEndpoint.ApplicationType == ApplicationType.MSSL)
                               {
                                   applicationTypes = new List<ApplicationType>() { ApplicationType.MSSL };
                                   return applicationTypes.Contains(item.ApplicationType)
                                           && item.AdhocActionType == adhocAccessRequest.AdhocActionType
                                           && item.AccountType == adhocAccessRequest.AccountType
                                           && !string.IsNullOrWhiteSpace(((AdHocSAPHANARequest)adhocEntitlementDetail).Position)
                                           && !string.IsNullOrWhiteSpace(((AdHocSAPHANARequest)entitlementDetail).Position)
                                           && (((AdHocSAPHANARequest)adhocEntitlementDetail).Position) == (((AdHocSAPHANARequest)entitlementDetail).Position)
                                           && (((AdHocSAPHANARequest)adhocEntitlementDetail).AccName) == (((AdHocSAPHANARequest)entitlementDetail).AccName);
                               }
                           }
                           else if (entitlementDetail != null && adhocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                           {
                               return entitlementDetail.EndpointUserId != 0 && entitlementDetail.EndpointUserId == endPointUserId
                                        && item.EndpointSystemId == adhocAccessRequest.EndpointSystemId
                                        && item.AdhocActionType == adhocAccessRequest.AdhocActionType;
                           }

                           return false;

                       });

                if (isExistsRequestInInterimStore != null)
                {
                    RequesterID = isExistsRequestInInterimStore.RequesterUserId;
                    //if (RequesterID > 0)
                    //{
                    //    var user = DomainApiClient<Dto.User>.GetById(RequesterID);
                    //    ValidationMessage = $"Same access already requested in the current request list of {user.DisplayName}";
                    //}
                    //else
                    ValidationMessage = string.Format("Same access already requested in the current request list");
                    return true;
                }
            }

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("requesterId", RequesterID.ToString());
            parameter.Add("accessRequestId", AccessRequestID.ToString());
            string ids = DomainApiClient<Dto.AdHocAccessRequest>.GetByAction<string>(DomainApiAction.AdhocRequest.IsSameRequestPending, parameter, adhocAccessRequest);
            bool isExistsAccessRequest = false;
            if (!String.IsNullOrEmpty(ids))
            {
                isExistsAccessRequest = true;
                RequesterID = Convert.ToInt64(ids.Split(';')[0]);
                AccessRequestID = Convert.ToInt64(ids.Split(';')[1]);
            }


            if (isExistsAccessRequest)
            {

                Dictionary<string, string> eparameter = new Dictionary<string, string>();
                eparameter.Add("endpointUserID", RequesterID.ToString());
                eparameter.Add("endpointSystemName", "DIUser");

                string RequesterName = DomainApiClient<Dto.User>.GetByAction<string>(DomainApiAction.User.GetEndPointDetailByID, eparameter);

                if (AccessRequestID > 0)
                    ValidationMessage = string.Format("Same access already requested by {0} Access Request Id {1}", RequesterName, AccessRequestID);
                return true;
            }

            if (adhocAccessRequest.ApplicationType == ApplicationType.EBS_CRM || adhocAccessRequest.ApplicationType == ApplicationType.EBS_ISU || adhocAccessRequest.ApplicationType == ApplicationType.MSSL_CRM || adhocAccessRequest.ApplicationType == ApplicationType.MSSL_ISU)
            {
                var respone = ValidateCRMAndISUNewAccountRequests(adhocAccessRequest, out ValidationMessage);

                if (!respone && !string.IsNullOrEmpty(ValidationMessage))
                {
                    return true;
                }
            }

            //var ApplicationTypes = new[] { ApplicationType.ADSingaporePowerLocal , ApplicationType.SAPLocal,ApplicationType.CE, ApplicationType.SAP_EP  };

            var excludingApplicationTypes = new[] { ApplicationType.EAM };

            if (//!excludingApplicationTypes.Contains(adhocAccessRequest.ApplicationType) &&
                (adhocAccessRequest.AccountType == AccountType.Service || adhocAccessRequest.AccountType == AccountType.Shared || adhocAccessRequest.AccountType == AccountType.Secondary || (adhocAccessRequest.ApplicationType == ApplicationType.SAPLocal && adhocAccessRequest.AccountType == AccountType.User))
                && adhocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                return !ValidateAccountName(adhocAccessRequest, out ValidationMessage, RequesteeID);
            }

            return false;
        }


        public static bool ValidateAccountName(AdHocAccessRequest request, out string ValidationMsg, long RequesteeUserID)
        {
            string spc = "!@#$%^&*()+=[]\\\';,./{}|\":<>?";
            string dispname = null, accountName = null;
            ISharedAccountDetail sharedAccountDetail = null;
            IEntitlementDetail ADRequest = request.GetEntitlementDetail();
            if (ADRequest != null)
            {
                sharedAccountDetail = ADRequest.SharedAccountDetail;
            }
            if (sharedAccountDetail == null)
            {
                ValidationMsg = "There is no Add request for Shared/Service account.";
                return false;
            }

            accountName = sharedAccountDetail.AccountName;

            var Idispname = (ISharedAccountDisplay)sharedAccountDetail;
            if (Idispname != null)
            {
                dispname = Idispname.DisplayName;
            }

            if (accountName != null && spc.Any(ch => accountName.Contains(ch)))
            {
                ValidationMsg = "Special characters are not allowed in Account Name.";
                return false;
            }

            if (dispname != null && spc.Any(ch => dispname.Contains(ch)))
            {
                ValidationMsg = $"Special characters are not allowed in {(request.EndpointSystemType != EndpointType.SAP ? "Display Name" : "Description") }.";
                return false;
            }

            bool returnValue = ValidateAccountName(accountName,
                                                    request.EndpointSystemId.HasValue ? request.EndpointSystemId.Value : 0, out ValidationMsg, RequesteeUserID, request.AccountType);
            return returnValue;
        }

        public static bool ValidateAccountName(string AccountName, long EndpointSystemId, out string ValidationMsg, long DiUserId, AccountType accountType)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("accountName", AccountName);
            parameter.Add("EndpointSystemId", EndpointSystemId.ToString());

            var accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>
                        (DomainApiAction.Provisioning.AccountReconcilation.DoesAccountExistInAccountRecon, parameter);

            string spc = "!@#$%^&*()+=-[]\\\';,./{}|\":<>?";
            if (accountRecon != null && (accountRecon.IsLocked == false && accountRecon.IsDeleted == false))
            {
                ValidationMsg = "Account Name already exists, please try another name.";
                return false;
            }
            else if (accountRecon != null && (accountRecon.IsLocked == true && accountRecon.IsDeleted == true))
            {
                ValidationMsg = string.Empty;
                if (accountRecon.UserId == null)
                {
                    ValidationMsg = "Account Name already exists as inactive status and is not mapped. \nPlease contact IAM team for further assistance.";
                    return false;
                }

                List<long> UserIds = new List<long>();

                if (DiUserId > 0 && accountRecon.UserId.HasValue)
                {
                    if (accountType == AccountType.Service || accountType == AccountType.Shared)
                    {
                        UserIds = DomainApiClient<User>.GetByAction<List<long>>(DomainApiAction.User.GetSameOUUserIDs, new Dictionary<string, string> { { "DiUserId", DiUserId.ToString() } });

                        if (UserIds.Contains(DiUserId))
                        {
                            UserIds.Remove(DiUserId);
                        }
                    } 

                    if (accountRecon.UserId.Value != DiUserId && !UserIds.Contains(accountRecon.UserId.Value))
                    {
                        var usr = DomainApiClient<Dto.User>.GetById(accountRecon.UserId.Value);
                        if (usr != null)
                        {
                            ValidationMsg = $"Account Name already exists as inactive status and mapped to {usr.DisplayName}. \nPlease contact IAM team for further assistance.";
                            return false;
                        }
                    }
                }
            }

            for (int i = 0; i < spc.Length; i++)
            {
                if (AccountName.Contains(spc[i]))
                {
                    ValidationMsg = "Special characters not allowed in Account Name.";
                    return false;
                }
            }

            if (AccountName.Any(x => Char.IsWhiteSpace(x)))
            {
                ValidationMsg = "Space is not allowed in Account Name.";
                return false;
            }

            ValidationMsg = string.Empty;
            return true;
        }

        public static bool ValidateAccountNameInTheDependentSystemsINH(string accountName, long endpointsystemId, string permissions, AccountType accountType)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("accountName", accountName);
            parameter.Add("endpointSystemId", endpointsystemId.ToString());
            //parameter.Add("permissions", permissions);
            parameter.Add("accountType", Convert.ToInt32(accountType).ToString());
            var exists = DomainApiClient<Dto.AccountReconcilation>.GetByAction<bool>
                        ("DoesAccountNameExistInTheDependentSystemsWithPermission", parameter, new List<string> { permissions });
            return exists;
        }

        public static string ValidateAccountNameForMSSLAndEBS(AccountType accountType, string accountName)
        {
            var message = string.Empty;
            var validateAccountName = MasterConfigHelper.ValidateSAPSpecialAccountName;
            if (validateAccountName)
            {
                if (accountType == AccountType.Service && !accountName.StartsWith("RFC", StringComparison.InvariantCultureIgnoreCase))
                {
                    message = "Service Account name should be prefixed with 'RFC'. Please re-enter the account name with correct prefix.";
                }
                else if (accountType == AccountType.Shared && !accountName.StartsWith("SPECIAL", StringComparison.InvariantCultureIgnoreCase))
                {
                    message = "Shared Account name should be prefixed with 'SPECIAL'. Please re-enter the account name with correct prefix.";
                }
            }
            return message;
        }

        public static bool ValidateCRMAndISUNewAccountRequests(AdHocAccessRequest adhocAccessRequest, out string ValidationMessage)
        {
            ApplicationType appType = ApplicationType.None;
            ValidationMessage = string.Empty;
            AdHocAccessRequest isExistsRequestInInterimStore = null;
            var adhocEntitlementDetail = adhocAccessRequest.GetEntitlementDetail();
            string accountName = null;

            if (adhocEntitlementDetail != null)
            {
                if (adhocEntitlementDetail.SharedAccountDetail != null)
                {
                    accountName = adhocEntitlementDetail.SharedAccountDetail.AccountName;
                }
            }

            IList<AdHocAccessRequest> InterimrequestsList = GetFromInterim(adhocAccessRequest.AccountType);

            if (adhocAccessRequest.ApplicationType == ApplicationType.MSSL_CRM)
            {
                appType = ApplicationType.MSSL_ISU;
                ValidationMessage = "Account creation request already exists for MSSL_ISU, hence New account request for MSSL_CRM cannot be submitted";
            }
            else if (adhocAccessRequest.ApplicationType == ApplicationType.MSSL_ISU)
            {
                appType = ApplicationType.MSSL_CRM;
                ValidationMessage = "Account creation request already exists for MSSL_CRM, hence New account request for MSSL_ISU cannot be submitted";
            }
            else if (adhocAccessRequest.ApplicationType == ApplicationType.EBS_CRM)
            {
                appType = ApplicationType.EBS_ISU;
                ValidationMessage = "Account creation request already exists for EBS_ISU, hence New account request for EBS_CRM cannot be submitted";
            }
            else if (adhocAccessRequest.ApplicationType == ApplicationType.EBS_ISU)
            {
                appType = ApplicationType.EBS_CRM;
                ValidationMessage = "Account creation request already exists for EBS_CRM, hence New account request for EBS_ISU cannot be submitted";
            }

            InterimrequestsList = InterimrequestsList.Where(j => j.AccountType == adhocAccessRequest.AccountType && j.RequesteeUserId == adhocAccessRequest.RequesteeUserId && j.AdhocActionType == AdHocActionType.NewAccount).ToList();

            if (InterimrequestsList != null && InterimrequestsList.Count > 0)
            {
                isExistsRequestInInterimStore = InterimrequestsList.FirstOrDefault(item =>
                {
                    string itemAccountName = null;
                    string entitlement = null;
                    IEntitlementDetail entitlementDetail = item.GetEntitlementDetail();

                    if (entitlementDetail != null && entitlementDetail.SharedAccountDetail != null)
                    {
                        itemAccountName = entitlementDetail.SharedAccountDetail.AccountName;
                        entitlement = entitlementDetail.Entitlement;
                    }

                    if (adhocAccessRequest.AdhocActionType == AdHocActionType.NewAccount && !string.IsNullOrWhiteSpace(accountName))
                    {
                        return
                         item.RequesteeUserId == adhocAccessRequest.RequesteeUserId &&
                         item.AdhocActionType == adhocAccessRequest.AdhocActionType &&
                         !string.IsNullOrWhiteSpace(itemAccountName) && itemAccountName.Equals(accountName, StringComparison.InvariantCultureIgnoreCase) && item.EndpointSystemType == adhocAccessRequest.EndpointSystemType &&
                         item.AccountType == adhocAccessRequest.AccountType &&
                         item.ApplicationType == appType;
                    }
                    else if (adhocAccessRequest.AccountType == AccountType.User && adhocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
                    {
                        return item.RequesteeUserId == adhocAccessRequest.RequesteeUserId &&
                         item.AdhocActionType == adhocAccessRequest.AdhocActionType &&
                         item.EndpointSystemType == adhocAccessRequest.EndpointSystemType &&
                         item.AccountType == adhocAccessRequest.AccountType &&
                         item.ApplicationType == appType;
                    }
                    return false;
                });
            }

            if (isExistsRequestInInterimStore != null) return false;

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("appType", appType.ToString());
            bool result = DomainApiClient<Dto.AdHocAccessRequest>.GetByAction<bool>(DomainApiAction.AdhocRequest.IsDependentRequestPendingForEBSMSSL, parameter, adhocAccessRequest);

            if (result) return false;

            return true;
        }
    }
}