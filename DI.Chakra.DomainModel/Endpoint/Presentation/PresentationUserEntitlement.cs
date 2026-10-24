using AutoMapper;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    [Serializable]

    public abstract class PresentationUserEntitlement : IXLData
    {
        public long EndpointUserId { get; set; }

        public long EntitlementId { get; set; }

        public EntitlementType EntitlementType { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public string Remarks1 { get; set; }
        public string Functions { get; set; }

        public EndpointType EndPointSystemType { get; set; }

        public string EndpointSystemName { get; set; }
        public string EndpointSystemCode { get; set; }

        public long EndpointSystemId { get; set; }

        public bool? CanRequest { get; set; }

        public DateTime? ReviewDate { get; set; }

        public string UserDefinedDesc { get; set; }

        public Nullable<long> WogUserId { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public bool IsValid { get; set; }

        public string ValidationMessage { get; set; }

        public long Id { get; set; }

        public string CanRequestString
        {
            get { return CanRequest != null ? Convert.ToBoolean(CanRequest) ? "Yes" : "No" : "Not defined"; }
        }

        public long? ParentEntitlementId { get; set; }

        public bool IsSODConflict { get; set; }

        public bool IsSensitiveAccess { get; set; }

        public string UserDefinedDisplayName { get; set; }

        public Nullable<long> Level1Approver { get; set; }

        public Nullable<long> Level2Approver { get; set; }

        public Nullable<bool> IsLevel1Group { get; set; }

        public Nullable<bool> IsLevel2Group { get; set; }

        public string Approver1Name { get; set; }
        public string Approver2Name { get; set; }
        public string Remarks2 { get; set; }
        public string Remarks3 { get; set; }
        public string Remarks4 { get; set; }
        public string Remarks5 { get; set; }
        public string Remarks6 { get; set; }
        public string Remarks7 { get; set; }
        public string Remarks8 { get; set; }
        public string Remarks9 { get; set; }
        public string Remarks10 { get; set; }
        public AccountType AccountType { get; set; }
        public string AccountTypeString { get; set; }
        public Nullable<DateTime> LastAttestedOn { get; set; }
        public static PresentationUserEntitlement ORMap(EntityModel.Presentation.PresentationUserEntitlement presentationUsrEntitlementEntityModel)
        {
            if (presentationUsrEntitlementEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationUserEntitlement, PresentationUserEntitlement>(presentationUsrEntitlementEntityModel);
        }

        public static List<PresentationUserEntitlement> ORMapList(IList<EntityModel.Presentation.PresentationUserEntitlement> presentationUsrEntitlementEntityModels)
        {
            return presentationUsrEntitlementEntityModels.Select(ORMap).ToList();
        }

        public static List<EntitlementType> GetEntitlementList(EndpointType applicationType)
        {
            return EndPointFactory.GetEndPoint(applicationType).GetEntitlementList();
        }
        public static Dictionary<long, string> GetEntitlementName(EndpointType applicationType, long endpointSystemId, string entitlementType, string searchString)
        {
            searchString = CheckNull.AsteriskToString(searchString);
            var getByEndpointSystem = EndPointFactory.GetEndPoint(applicationType).GetEntitlementDetailByApplicationType(endpointSystemId);
            Dictionary<long, string> val = getByEndpointSystem.Where(x => x.Category == entitlementType && x.Name.ToLower().Contains(searchString.ToLower()))
                               .Select(item => new { Key = item.EntitlementId, Value = item.Name })
                               .Distinct()
                               .ToDictionary(item => item.Key, item => item.Value);
            return val;
        }

        public static List<PresentationUserEntitlement> GetUserAccountDetail(long userId)
        {
            List<PresentationUserEntitlement> presentationUserAccountDetaills = new List<PresentationUserEntitlement>();
            List<AccountReconcilation> accountReconcilation = new List<AccountReconcilation>();

            EndPoint splocalEp = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
            long spLocalId = splocalEp != null ? splocalEp.Id : 0;

            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                accountReconcilation = AccountReconcilation.GetByUser(userId, true, false).Where(X => X.ExceptionType != ExceptionTypes.Attribute && !X.IsDeleted).ToList();
                if (accountReconcilation == null || accountReconcilation.Count == 0)
                    return presentationUserAccountDetaills;

                foreach (AccountReconcilation acct in accountReconcilation)
                {
                    PresentationUserEntitlement userEntitlement = new PresentationADSUserEntitlement();
                    userEntitlement.EntitlementId = acct.Id;
                    userEntitlement.EndpointUserId = acct.EndpointUserId;
                    userEntitlement.Description = acct.FullName;
                    userEntitlement.UserName = acct.AccountName;
                    userEntitlement.Category = "Account";
                    if(acct.EndPoint.EndPointSystemType != null)
                    {
                        userEntitlement.EndPointSystemType = acct.EndPoint.EndPointSystemType.EndpointSystemTypeName;
                    }
                    userEntitlement.EndpointSystemName = acct.EndPoint.DisplayName;
                    userEntitlement.EndpointSystemId = acct.EndpointSystemId;
                    userEntitlement.ReviewDate = acct.LastAttestedOn;
                    userEntitlement.ValidTo = acct.ValidToDate.HasValue ? acct.ValidToDate.Value.Date : acct.ValidToDate;
                    userEntitlement.AccountTypeString = spLocalId != 0 && acct.EndpointSystemId == spLocalId && acct.AccountType == AccountType.User ? "Primary Account" : acct.AccountType.ToString();
                    //ADSUserInfo aDSUserInfo = null;
                    //if ((acct.EndPoint.EndpointSystemTypeId == Convert.ToInt64(EndpointType.ActiveDirectory)))
                    //    aDSUserInfo = ADSUserInfo.GetById(acct.EndpointUserId);
                    //if (aDSUserInfo != null)
                    //{
                    //    userEntitlement.WogUserId = aDSUserInfo.WogUserId;
                    //}

                    userEntitlement.AccountType = acct.AccountType;
                    //EndPointFactory.GetEndPoint(acct.EndPoint.EndPointSystemType.EndpointSystemTypeName).GetUser(acct.EndpointUserId).WogUserId;

                    presentationUserAccountDetaills.Add(userEntitlement);
                }
            }
            return presentationUserAccountDetaills;
        }
        public static List<PresentationUserEntitlement> GetUserAccountDetailInactiveAccounts(long userId)
        {
            List<PresentationUserEntitlement> presentationUserAccountDetaills = new List<PresentationUserEntitlement>();
            List<AccountReconcilation> accountReconcilation = new List<AccountReconcilation>();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                accountReconcilation = AccountReconcilation.GetInActiveByUser(userId, true).ToList();
                if (accountReconcilation == null || accountReconcilation.Count == 0)
                    return presentationUserAccountDetaills;

                foreach (AccountReconcilation acct in accountReconcilation)
                {
                    PresentationUserEntitlement userEntitlement = new PresentationADSUserEntitlement();
                    userEntitlement.EntitlementId = acct.Id;
                    userEntitlement.EndpointUserId = acct.EndpointUserId;
                    userEntitlement.Description = acct.FullName;
                    userEntitlement.UserName = acct.AccountName;
                    userEntitlement.Category = "Account";
                    userEntitlement.EndPointSystemType = acct.EndPoint.EndPointSystemType.EndpointSystemTypeName;
                    userEntitlement.EndpointSystemName = acct.EndPoint.DisplayName;
                    userEntitlement.EndpointSystemId = acct.EndpointSystemId;
                    userEntitlement.ReviewDate = acct.LastAttestedOn;
                    userEntitlement.ValidTo = acct.ValidToDate;
                    ADSUserInfo aDSUserInfo = null;
                    if ((acct.EndPoint.EndpointSystemTypeId == Convert.ToInt64(EndpointType.ActiveDirectory)))
                        aDSUserInfo = ADSUserInfo.GetById(acct.EndpointUserId);
                    if (aDSUserInfo != null)
                    {
                        userEntitlement.WogUserId = aDSUserInfo.WogUserId;
                    }

                    userEntitlement.AccountType = acct.AccountType;
                    //EndPointFactory.GetEndPoint(acct.EndPoint.EndPointSystemType.EndpointSystemTypeName).GetUser(acct.EndpointUserId).WogUserId;

                    presentationUserAccountDetaills.Add(userEntitlement);
                }
            }
            return presentationUserAccountDetaills;
        }

        public static List<PresentationUserEntitlement> GetUserAccountEntitlment(long userId, long endpointUserId, long endpointSystemId)
        {
            List<PresentationUserEntitlement> presentationUserAccountEntitlement = new List<PresentationUserEntitlement>();
            List<EntitlementReconcilation> entitlementReconcilation = new List<EntitlementReconcilation>();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                entitlementReconcilation = EntitlementReconcilation.GetByUserAndReconcilationId(userId, endpointSystemId, endpointUserId).ToList();
                if (entitlementReconcilation == null || entitlementReconcilation.Count == 0)
                    return presentationUserAccountEntitlement;

                foreach (EntitlementReconcilation acct in entitlementReconcilation)
                {
                    PresentationUserEntitlement userEntitlement = new PresentationADSUserEntitlement();
                    userEntitlement.ParentEntitlementId = endpointUserId;
                    userEntitlement.EntitlementId = acct.EntitlementId;
                    userEntitlement.EndpointUserId = acct.EndpointUserId;
                    userEntitlement.Description = acct.EntitlementDescription;
                    userEntitlement.UserName = acct.Entitlement;
                    userEntitlement.Category = ((EntitlementType)acct.EntitlementType).ToString();
                    userEntitlement.EndPointSystemType = acct.EndPoint.EndPointSystemType.EndpointSystemTypeName;
                    userEntitlement.EndpointSystemName = acct.EndPoint.DisplayName;
                    userEntitlement.EndpointSystemId = acct.EndpointSystemId;
                    userEntitlement.ReviewDate = acct.LastAttestedOn;
                    userEntitlement.Functions = acct.Functions;
                    userEntitlement.WogUserId = (acct.EndPoint.EndpointSystemTypeId == Convert.ToInt64(EndpointType.ActiveDirectory)) ? ADSUserInfo.GetById(acct.EndpointUserId).WogUserId : null;
                    //userEntitlement.ValidTo = acct.ValidToDate;
                    //EndPointFactory.GetEndPoint(acct.EndPoint.EndPointSystemType.EndpointSystemTypeName).GetUser(acct.EndpointUserId).WogUserId;
                    presentationUserAccountEntitlement.Add(userEntitlement);
                }
            }
            return presentationUserAccountEntitlement;
        }

        public static PagedResults<PresentationUserEntitlement> GetUserAccountEntitlmentPaging(long userId, long endpointUserId, long endpointSystemId, int? pageSize, int? pageNumber)
        {
            List<PresentationUserEntitlement> presentationUserAccountEntitlement = new List<PresentationUserEntitlement>();
            List<EntitlementReconcilation> entitlementReconcilation = new List<EntitlementReconcilation>();

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                entitlementReconcilation = EntitlementReconcilation.GetByUserAndReconcilationId(userId, endpointSystemId, endpointUserId).ToList();
            }
            if (entitlementReconcilation == null || entitlementReconcilation.Count == 0)
                return new PagedResults<PresentationUserEntitlement>();

                foreach (EntitlementReconcilation acct in entitlementReconcilation)
                {
                    PresentationUserEntitlement userEntitlement = new PresentationADSUserEntitlement();
                    userEntitlement.ParentEntitlementId = endpointUserId;
                    userEntitlement.EntitlementId = acct.EntitlementId;
                    userEntitlement.EndpointUserId = acct.EndpointUserId;
                    userEntitlement.Description = acct.EntitlementDescription;
                    userEntitlement.UserName = acct.Entitlement;
                    userEntitlement.Category = ((EntitlementType)acct.EntitlementType).ToString();
                    userEntitlement.EndPointSystemType = acct.EndPoint.EndPointSystemType.EndpointSystemTypeName;
                    userEntitlement.EndpointSystemName = acct.EndPoint.DisplayName;
                    userEntitlement.EndpointSystemId = acct.EndpointSystemId;
                    userEntitlement.ReviewDate = acct.LastAttestedOn;
                    userEntitlement.WogUserId = (acct.EndPoint.EndpointSystemTypeId == Convert.ToInt64(EndpointType.ActiveDirectory)) ? ADSUserInfo.GetById(acct.EndpointUserId).WogUserId : null;
                    presentationUserAccountEntitlement.Add(userEntitlement);
                }

            var aa = presentationUserAccountEntitlement.Skip(pageNumber.Value * pageSize.Value).Take(pageSize.Value).ToList();

            PagedResults<PresentationUserEntitlement> paged = new PagedResults<PresentationUserEntitlement>()
            {
                Results = aa,
                TotalNumberOfRecords = presentationUserAccountEntitlement.Count,
                PageNumber = pageNumber.Value,
                PageSize = pageSize.Value
            };

            return paged;
        }

        public static List<PresentationUserEntitlement> GetEntitlementDetail(long endpointUserId, EndpointType applicationType)
        {
            return EndPointFactory.GetEndPoint(applicationType).GetEntitlementDetail(endpointUserId);
        }

        public static List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long endpointUserId, EndpointType applicationType)
        {
            return EndPointFactory.GetEndPoint(applicationType).GetEntitlementHierachyDetail(endpointUserId);
        }
        public static List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long endpointUserId, EndpointType applicationType, string entitlementType)
        {
            return EndPointFactory.GetEndPoint(applicationType).GetEntitlementHierachyDetail(endpointUserId, entitlementType, "");
        }
        //public static List<PresentationUserEntitlement> GetEntitlementDetails(long entitlementId, EndpointType applicationType)
        //{
        //    return EndPointFactory.GetEndPoint(applicationType).GetEntitlementDetails(entitlementId);
        //}
        public static PresentationUserEntitlement GetEntitlement(long id, EndpointType applicationType, string entitlementType)
        {
            return EndPointFactory.GetEndPoint(applicationType).GetEntitlement(EndPointFactory.GetEndPoint(applicationType).GetEntitlement(id, (EntitlementType)Enum.Parse(typeof(EntitlementType), entitlementType, true)), entitlementType);
        }
        public static List<PresentationUserEntitlement> GetDetailById(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                AccountReconcilation recon = AccountReconcilation.GetByIdIncludingEndPoint(id);
                //if (recon.EndPoint.ApplicationType == ApplicationType.Office365 || recon.EndPoint.ApplicationType == ApplicationType.GEMS || recon.EndPoint.ApplicationType == ApplicationType.SSIDS_GUI || recon.EndPoint.ApplicationType == ApplicationType.SSIDS_SMS || recon.EndPoint.ApplicationType == ApplicationType.OneCard)
                {
                    List<PresentationUserEntitlement> entitlements = new List<PresentationUserEntitlement>();
                    var entRecons = EntitlementReconcilation.GetAllActiveByAccountReconcilationId(recon.Id);
                    foreach (var entRecon in entRecons)
                    {
                        entitlements.Add(new PresentationOffice365UserEntitlement()
                        {
                            EntitlementId = entRecon.EntitlementId,
                            Category = Convert.ToString((EntitlementType)entRecon.EntitlementType),
                            Name = entRecon.Entitlement,
                            Description = entRecon.EntitlementDescription,
                            EndpointSystemId = entRecon.EndpointSystemId,
                    });
                    }
                    return entitlements;
                }
                //else
                //{
                //    return GetEntitlementDetail(recon.EndpointUserId, recon.EndPoint.EndPointSystemType.EndpointSystemTypeName);
                //}
            }
        }

        public static IList<PresentationUserEntitlement> GetEntitlementDetailByApplicationType(long endpointsystemId, EndpointType applicationType)
        {
            return EndPointFactory.GetEndPoint(applicationType).GetEntitlementDetailByApplicationType(endpointsystemId);
        }

        public static IList<PresentationUserEntitlement> GetEntitlementDetailByApplicationTypeWithAllCanRequest(long endpointsystemId, EndpointType applicationType)
        {
            return EndPointFactory.GetEndPoint(applicationType).GetEntitlementDetailByApplicationTypeWithAllCanRequest(endpointsystemId);
        }

        public static void UpdateEntitlement(PresentationUserEntitlement presentationUserEntitlement, string currentUser)
        {
            IEndPointFactory getEndpoint = EndPointFactory.GetEndPoint(presentationUserEntitlement.EndPointSystemType);
            IEndpointEntitlementInfo entitlement = getEndpoint.GetEntitlement(presentationUserEntitlement.EntitlementId, presentationUserEntitlement.EntitlementType);
            getEndpoint.UpdateEntitlement(presentationUserEntitlement, entitlement, currentUser);
        }

        public static string GetEntitlement(long id, long endpointSystemId, string entitlementType)
        {
            IEndpointEntitlementInfo entitlement;
            EntitlementType entType = EntitlementType.None;
            if (!string.IsNullOrEmpty(entitlementType))
            {
                entType = (EntitlementType)Enum.Parse(typeof(EntitlementType), entitlementType, true);
            }
            EndpointType applicationType = DomainModel.EndPoint.GetByIdIncludingEndPointSystemType(endpointSystemId).EndPointSystemType.EndpointSystemTypeName;
            if (string.IsNullOrEmpty(entitlementType))
            {
                entitlement = EndPointFactory.GetEndPoint(applicationType).GetEntitlement(id);
            }
            else
            {
                entitlement = EndPointFactory.GetEndPoint(applicationType).GetEntitlement(id, entType);
            }
            string getString = EndPointFactory.GetEndPoint(applicationType).GetEntitlement(entitlement, entitlementType).Name;
            return getString;
        }
        public abstract PresentationUserEntitlement GetEntitlement(IEndpointEntitlementInfo entitlement, string entitlementType);

        public abstract void UpdateEntitlement(PresentationUserEntitlement presentationUserEntitlement, IEndpointEntitlementInfo entitlement, string currentUser);

        public abstract List<PresentationUserEntitlement> GetEntitlementDetail(long endpointUserId);

        public abstract List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId);
        public abstract List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId, string entitlementType, string userAccountName);

        public abstract List<PresentationUserEntitlement> GetEntitlementHierachyDetail(string entitlementCode, long endpointId);

        public abstract List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(long endpointId, string entityCode, EntitlementType fromValue, EntitlementType toValue);

        public abstract List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(long endpointId, long entityId, EntitlementType fromValue, EntitlementType toValue);

        public abstract List<PresentationUserEntitlement> GetEntitlementDetailByApplicationType(long endpointSystemId);

        public abstract List<PresentationUserEntitlement> GetEntitlementDetailByApplicationTypeWithAllCanRequest(long endpointSystemId);

        public abstract List<PresentationUserEntitlement> GetAllEntitlementDetailByType(long endpointSystemId, EntitlementType entitlementType);

        public static List<PresentationUserEntitlement> GetEntitlementDetailByEndpointId(long endpointSystemId)
        {
            var endpoint = DomainModel.EndPoint.GetByIdIncludingEndPointSystemType(endpointSystemId);
            EndpointType applicationType = endpoint.EndPointSystemType.EndpointSystemTypeName;
            List<PresentationUserEntitlement> entitlement = EndPointFactory.GetEndPoint(applicationType).GetEntitlementDetailByApplicationType(endpointSystemId);
            entitlement.ForEach(a => { a.EndPointSystemType = applicationType; a.EndpointSystemName = endpoint.DisplayName; a.EndpointSystemCode = endpoint.ApplicationCode; });
            return entitlement;
        }
        public static List<PresentationUserEntitlement> GetEntitlementDetailByEndpointIdWithAllCanRequest(long endpointSystemId)
        {
            var endpoint = DomainModel.EndPoint.GetByIdIncludingEndPointSystemType(endpointSystemId);
            EndpointType applicationType = endpoint.EndPointSystemType.EndpointSystemTypeName;
            List<PresentationUserEntitlement> entitlement = EndPointFactory.GetEndPoint(applicationType).GetEntitlementDetailByApplicationTypeWithAllCanRequest(endpointSystemId);
            entitlement.ForEach(a => { a.EndPointSystemType = applicationType; a.EndpointSystemName = endpoint.DisplayName; a.EndpointSystemCode = endpoint.ApplicationCode; });
            return entitlement;
        }

        public static List<PresentationUserEntitlement> GetFunctionsForEntitlements(long endpointId, string entityCode, EntitlementType fromValue)
        {
            var endpoint = EndPoint.GetById(endpointId, true);
            var endpointHierarchy = EndpointHierarchy.GetEndpointSystemIdHierarchyByLevel(endpoint.EndpointSystemTypeId.Value, 3);

            if (endpointHierarchy != null)
            {
                var toValue = endpointHierarchy.EntitlementType;
                return EndPointFactory.GetEndPoint(endpoint.EndPointSystemType.EndpointSystemTypeName)
                                                .GetEntitlementHierachyDetailByLevel(
                                                    endpoint,
                                                    "",
                                                    0,
                                                    entityCode,
                                                    fromValue,
                                                    toValue);
            }
            else
            {
                return new List<PresentationUserEntitlement>();
            }
        }
    }
    public class PresentationBusinessRoleDetail
    {
        public long SAP_ROLEID { get; set; }
        public long EndpointUserId { get; set; }
        public string BusinessRole { get; set; }
        public string Description { get; set; }
        public string Company { get; set; }
        public string Module { get; set; }
        public string SubModule { get; set; }
        public string Remarks { get; set; }
        public DateTime? ValidToDate { get; set; }
    }
}