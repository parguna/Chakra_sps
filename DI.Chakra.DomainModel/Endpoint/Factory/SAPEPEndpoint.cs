using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Infrastructure;
using DI.Chakra.Transaction;
using DI.Chakra.DomainModel.Presentation;

namespace DI.Chakra.DomainModel
{
    public class SAPEPEndpoint : IEndPointFactory
    {
        public List<PresentationUserEntitlement> GetAllEntitlementByType(EntitlementType type, long endpointsystemid)
        {
            return new PresentationSAPEPUserEntitlement().GetAllEntitlementDetailByType(endpointsystemid, type);
        }

        public IEndpointUserInfo GetUser()
        {
            return new ADSUserInfo();
        }

        public IEndpointUserInfo GetUser(long id)
        {
            return ADSUserInfo.GetById(id, true);
        }

        public IEndpointEntitlementInfo GetEntitlement()
        {
            return new ADSGroupInfo();
        }
        public List<EntitlementType> GetEntitlementList()
        {
            List<EntitlementType> types = new List<EntitlementType>();
            types.Add(EntitlementType.Role);
            types.Add(EntitlementType.Group);
            return types;
        }
        public IEndpointEntitlementInfo GetEntitlement(long id)
        {
            return ADSGroupInfo.GetById(id);
        }

        public IEndpointEntitlementInfo GetEntitlement(long id, EntitlementType type)
        {
            switch (type)
            {
                case (EntitlementType.Group):
                    return ADSGroupInfo.GetById(id);
                default:
                    throw new ApplicationException("Entitlement Type : " + type.ToString() + " not supported for AD");
            }
        }

        public IEndpointEntitlementInfo GetEntitlement(long systemId, string entitlementName, EntitlementType entitlementType)
        {
            throw new ApplicationException();
        }
        public IEndpointEntitlementInfo GetEntitlement(long systemId, string entitlementName)
        {
            throw new ApplicationException();
        }
        //public void SaveUserInfo(long id, long endPointSystemId, string currentUser)
        //{
        //    ADSUserInfo.GetUserAccountByEndPointSystemId(id, endPointSystemId).Save(currentUser);
        //}
        //public Dictionary<long, string> GetEntitlementName(long endpointSystemId, string entitlementType)
        //{
        //    return new PresentationSAPEPUserEntitlement().GetEntitlementName(endpointSystemId, entitlementType);
        //}
        public PresentationUserEntitlement GetEntitlement(IEndpointEntitlementInfo entitlement, string entitlementType)
        {
            return new PresentationSAPEPUserEntitlement().GetEntitlement(entitlement, entitlementType);
        }
        public void UpdateEntitlement(PresentationUserEntitlement presentationUserEntitlement, IEndpointEntitlementInfo entitlement, string currentUser)
        {
            new PresentationSAPEPUserEntitlement().UpdateEntitlement(presentationUserEntitlement, entitlement, currentUser);
        }
        public List<PresentationUserEntitlement> GetEntitlementDetail(long endpointUserId)
        {
            return new PresentationSAPEPUserEntitlement().GetEntitlementDetail(endpointUserId);
        }
        public List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId)
        {
            return new PresentationSAPEPUserEntitlement().GetEntitlementHierachyDetail(entitlementId);
        }
        public List<PresentationUserEntitlement> GetEntitlementDetailByApplicationType(long endpointsystemId)
        {
            return new PresentationSAPEPUserEntitlement().GetEntitlementDetailByApplicationType(endpointsystemId);
        }
       
        public List<Group> GetAllGroupsByEndpointID(long endPointID, string searchString)
        {
            return SAPEPGroupInfo.GetAllGroupsByEndpointID(endPointID, searchString);
        }
        public Dictionary<long, string> GetEntitlements(string searchString, long endpointSystemId, long? EndpointUserId, bool existingEntitlements)
        {
            throw new ApplicationException();
        }
        public void UpdatePasswordresetcount(long endpointId, string userName, string currentUser)
        {
            SAPEPUserInfo.UpdatePasswordresetcount(endpointId, userName, currentUser);
        }
        public List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(EndPoint endpoint,  string userAccountName, long entitlementId, string entityCode, EntitlementType fromValue, EntitlementType toValue)
        {
            var endpointId = endpoint.Id;
            var presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            if (fromValue == EntitlementType.Account && toValue == EntitlementType.BusinessRole)
            {
                var businessRoles = EntitlementReconcilation.GetSAPEPBusinessRoleByAccount(endpointId, entitlementId, userAccountName);
                {
                    foreach (var presentationObj in businessRoles)
                    {
                        var entitlement = new PresentationSAPEPUserEntitlement();
                        entitlement.ParentEntitlementId = entitlementId;
                        entitlement.EntitlementId = presentationObj.Id;
                        entitlement.Name = presentationObj.Entitlement;
                        entitlement.Description = presentationObj.EntitlementDescription;
                        entitlement.Category = EntitlementType.BusinessRole.ToString();
                        entitlement.EndPointSystemType = EndpointType.SAPEnterprisePortal;
                        //Comments Used for EndpointDisplayname
                        entitlement.EndpointSystemName = endpoint.DisplayName;
                        entitlement.EndpointSystemId = endpointId;
                        entitlement.EntitlementType = toValue;
                        //Function field is used to hold the Uniquename for attestation purpose. Used only for fetching the unique name
                        entitlement.Remarks10 = presentationObj.Functions;
                        presentationUserEntitlementList.Add(entitlement);
                    }
                }
            }
            else if (fromValue == EntitlementType.BusinessRole && toValue == EntitlementType.TechnicalRole)
            {
                var businessRoles = EntitlementReconcilation.GetSAPEPTechnicalRoleFromBusinessRole(endpointId, userAccountName, entityCode);
                {
                    foreach (var presentationObj in businessRoles)
                    {
                        var entitlement = new PresentationSAPEPUserEntitlement();
                        entitlement.ParentEntitlementId = entitlementId;
                        entitlement.EntitlementId = presentationObj.EntitlementId;
                        entitlement.Name = presentationObj.Entitlement;
                        entitlement.Description = presentationObj.EntitlementDescription;
                        entitlement.Category = EntitlementType.TechnicalRole.ToString();
                        entitlement.EndPointSystemType = EndpointType.SAPEnterprisePortal;
                        //Comments Used for EndpointDisplayname
                        entitlement.EndpointSystemName = endpoint.DisplayName;
                        entitlement.EndpointSystemId = endpointId;
                        entitlement.EntitlementType = toValue;
                        //Function field is used to hold the Uniquename for attestation purpose. Used only for fetching the unique name
                        entitlement.Remarks10 = presentationObj.Functions;
                        presentationUserEntitlementList.Add(entitlement);
                    }
                }
            }
            return presentationUserEntitlementList;
        }
        public List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId, string entitlementType, string userAccountName)
        {
            return new List<PresentationUserEntitlement>();
        }

        public List<PresentationUserEntitlement> GetEntitlementDetailByApplicationTypeWithAllCanRequest(long endpointsystemId)
        {
            return new PresentationSAPEPUserEntitlement().GetEntitlementDetailByApplicationTypeWithAllCanRequest(endpointsystemId);
        }

        public List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(EndPoint endpoint, string userAccountName, long entitlementId, string entityCode, EntitlementType fromValue, EntitlementType toValue, string reserved, string reserved2, string description, ApplicationType applicationType)
        {
            var endpointId = endpoint.Id;
            var presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            if (fromValue == EntitlementType.Account && toValue == EntitlementType.BusinessRole)
            {
                var businessRoles = EntitlementReconcilation.GetSAPEPBusinessRoleByAccount(endpointId, entitlementId, userAccountName);
                {
                    foreach (var presentationObj in businessRoles)
                    {
                        var entitlement = new PresentationSAPEPUserEntitlement();
                        entitlement.ParentEntitlementId = entitlementId;
                        entitlement.EntitlementId = presentationObj.Id;
                        entitlement.Name = presentationObj.Entitlement;
                        entitlement.Description = presentationObj.EntitlementDescription;
                        entitlement.Category = EntitlementType.BusinessRole.ToString();
                        entitlement.EndPointSystemType = EndpointType.SAPEnterprisePortal;
                        //Comments Used for EndpointDisplayname
                        entitlement.EndpointSystemName = endpoint.DisplayName;
                        entitlement.EndpointSystemId = endpointId;
                        entitlement.EntitlementType = toValue;
                        //Function field is used to hold the Uniquename for attestation purpose. Used only for fetching the unique name
                        entitlement.Remarks10 = presentationObj.Functions;
                        presentationUserEntitlementList.Add(entitlement);
                    }
                }
            }
            else if (fromValue == EntitlementType.BusinessRole && toValue == EntitlementType.TechnicalRole)
            {
                var businessRoles = EntitlementReconcilation.GetSAPEPTechnicalRoleFromBusinessRole(endpointId, userAccountName, entityCode);
                {
                    foreach (var presentationObj in businessRoles)
                    {
                        var entitlement = new PresentationSAPEPUserEntitlement();
                        entitlement.ParentEntitlementId = entitlementId;
                        entitlement.EntitlementId = presentationObj.EntitlementId;
                        entitlement.Name = presentationObj.Entitlement;
                        entitlement.Description = presentationObj.EntitlementDescription;
                        entitlement.Category = EntitlementType.TechnicalRole.ToString();
                        entitlement.EndPointSystemType = EndpointType.SAPEnterprisePortal;
                        //Comments Used for EndpointDisplayname
                        entitlement.EndpointSystemName = endpoint.DisplayName;
                        entitlement.EndpointSystemId = endpointId;
                        entitlement.EntitlementType = toValue;
                        //Function field is used to hold the Uniquename for attestation purpose. Used only for fetching the unique name
                        entitlement.Remarks10 = presentationObj.Functions;
                        presentationUserEntitlementList.Add(entitlement);
                    }
                }
            }
            return presentationUserEntitlementList;
        }
    }
}
