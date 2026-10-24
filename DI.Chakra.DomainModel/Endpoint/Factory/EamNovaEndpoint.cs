using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class EamNovaEndpoint : IEndPointFactory
    {
        public IEndpointUserInfo GetUser()
        {
            return new SAPEamNovaUserInfo();
        }


        public IEndpointUserInfo GetUser(long id)
        { return SAPEamNovaUserInfo.GetById(id);
        }

        public IEndpointEntitlementInfo GetEntitlement()
        {
            return new SAPEamNovaUserRoleInfo();
        }
        public List<EntitlementType> GetEntitlementList()
        {
            return new List<EntitlementType> { EntitlementType.Role,EntitlementType.BusinessRole };
        }
        public List<PresentationUserEntitlement> GetAllEntitlementByType(EntitlementType type, long endpointsystemid)
        {
            return new PresentationEAMNovaUserEntitlement().GetAllEntitlementDetailByType(endpointsystemid, type);
        }
        public IEndpointEntitlementInfo GetEntitlement(long id)
        {
            return SAPEamNovaUserRoleInfo.GetById(id);
        }


        public IEndpointEntitlementInfo GetEntitlement(long id, EntitlementType type)
        {
            return SAPEamNovaUserRoleInfo.GetById(id);
        }

        public IEndpointEntitlementInfo GetEntitlement(long systemId, string entitlementName, EntitlementType entitlementType)
        {
            return SAPEamNovaUserRoleInfo.GetAll().Where(t => t.EndPointSystemId == systemId &&
           t.EntitlementName.ToUpper() == entitlementName.ToUpper()).FirstOrDefault();
        }
        public IEndpointEntitlementInfo GetEntitlement(long systemId, string entitlementName)
        {
            return SAPEamNovaUserRoleInfo.GetAll().Where(t => t.EndPointSystemId == systemId &&
           t.EntitlementName.ToUpper() == entitlementName.ToUpper()).FirstOrDefault();
        }
        public PresentationUserEntitlement GetEntitlement(IEndpointEntitlementInfo entitlement, string entitlementType)
        {
            return new PresentationEAMNovaUserEntitlement().GetEntitlement(entitlement, entitlementType);
        }
        public void UpdateEntitlement(PresentationUserEntitlement presentationUserEntitlement, IEndpointEntitlementInfo entitlement, string currentUser)
        {

        }

        public List<PresentationUserEntitlement> GetEntitlementDetail(long endpointId)
        {
            return new PresentationEAMNovaUserEntitlement().GetEntitlementDetail(endpointId);
        }
        public List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId)
        {
            throw new NotImplementedException();
        }

        public List<PresentationUserEntitlement> GetEntitlementDetailByApplicationType(long endpointsystemId)
        {
            throw new NotImplementedException();
        }
        public List<PresentationUserEntitlement> GetEntitlementDetailByApplicationTypeWithAllCanRequest(long endpointsystemId)
        {
            throw new NotImplementedException();
        }
        public List<Group> GetAllGroupsByEndpointID(long endPointID, string searchString)
        {
            throw new NotImplementedException();
        }
        public Dictionary<long, string> GetEntitlements(string searchString, long endpointSystemId, long? EndpointUserId, bool existingEntitlements)
        {
            throw new NotImplementedException();
        }
        public void UpdatePasswordresetcount(long endpointId, string userName, string currentUser)
        {
            
        }
        public List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId, string entitlementType, string userAccountName)
        {
            throw new NotImplementedException();
        }
        public List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(EndPoint endpoint, string userAccountName, long entitlementId, string entityCode, EntitlementType fromValue, EntitlementType toValue)
        {
            return new List<PresentationUserEntitlement>();
        }

        public List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(EndPoint endpoint, string userAccountName, long entitlementId, string entityCode, EntitlementType fromValue, EntitlementType toValue, string reserved, string reserved2, string description, ApplicationType applicationType)
        {
            var entitlements = new List<PresentationUserEntitlement>();
            try
            {
                if (fromValue == EntitlementType.BusinessRole && toValue == EntitlementType.TechnicalRole)
                {
                    using (var trans = new DataAccessTransaction(false))
                    {
                        var entitlemets = new EntitlementReconcilationRepository(trans).GetEAMTechRoleByBusinessRoles(endpoint.Id, userAccountName, entityCode);
                        entitlemets.ForEach(i =>
                        {
                            entitlements.Add(new PresentationSAPUserEntitlement()
                            {
                                Id = i.Id,
                                Name = i.Entitlement,
                                Description = i.EntitlementDescription,
                                EndpointSystemId = endpoint.Id,
                                EndpointSystemName = endpoint.DisplayName,
                                EntitlementType = EntitlementType.TechnicalRole,
                                EntitlementId = i.EntitlementId,
                                Remarks1 = UtilityMethods.ParseEnum<ApplicationType>(i.Functions).GetDescription(),
                                //Function hold Application type. This is applicable to EAM only.Hence we are using remarks 5 to find out tcode only for sap
                                Remarks5 = i.Functions,
                                LastAttestedOn = i.LastAttestedOn,
                                IsSensitiveAccess = i.IsSensitiveAccess,
                                IsSODConflict = i.IsSODConflict,
                                ValidTo = i.ValidToDate,
                                ValidFrom = i.ValidFromDate,
                                ParentEntitlementId = entitlementId,
                                UserName = i.AccountName
                            });
                        });
                    }
                }
                else if (fromValue == EntitlementType.TechnicalRole && toValue == EntitlementType.TCodes)
                {
                    if(applicationType != ApplicationType.ADSingaporePowerLocal)
                    {
                        var ep = EndPoint.GetByApplicationType(applicationType);
                        if(ep != null)
                        {
                            entitlements = new SAPEndPoint().GetTCodesForRole(endpoint, entityCode, ep);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new DI.Chakra.Logger.AttestationLogger().WriteLog(Logger.LogLevelType.Exception, $"{endpoint.DisplayName}: userAccountName:{userAccountName} entityCode:{entityCode}  fromValue:{fromValue} toValue:{toValue} GetEntitlementHierachyDetailByLevel failed.{ex.Message}", ex);
            }
            return entitlements.OrderBy(i=>i.Remarks1).ThenBy(i=> i.EntitlementType).ThenBy(i=>i.Name).ToList();
        }
    }
}
