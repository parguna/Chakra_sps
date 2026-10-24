using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Infrastructure;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Transaction;

namespace DI.Chakra.DomainModel
{
    public class ADSEndPoint : IEndPointFactory
    {
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
        public IEndpointEntitlementInfo GetEntitlement(long id)
        {
            return ADSGroupInfo.GetById(id);
        }
        public IEndpointEntitlementInfo GetEntitlement(long systemId, string entitlementName, EntitlementType entitlementType)
        {
            return ADSGroupInfo.GetAll().Where(t => t.EndPointSystemId == systemId &&
            t.EntitlementType == entitlementType && t.EntitlementName.ToUpper() == entitlementName.ToUpper()).FirstOrDefault();
        }
        public IEndpointEntitlementInfo GetEntitlement(long systemId, string entitlementName)
        {
            return ADSGroupInfo.GetAll().Where(t => t.EndPointSystemId == systemId &&
            t.EntitlementName.ToUpper() == entitlementName.ToUpper()).FirstOrDefault();
        }
        public List<EntitlementType> GetEntitlementList()
        {
            return ADSGroupInfo.GetAll().Select(s => s.EntitlementType).Distinct().ToList();
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


        //public void SaveUserInfo(long id, long endPointSystemId, string currentUser)
        //{
        //    ADSUserInfo.GetUserAccountByEndPointSystemId(id, endPointSystemId).Save(currentUser);
        //}
        //public Dictionary<long, string> GetEntitlementName(long endpointSystemId, string entitlementType)
        //{
        //    return new PresentationADSUserEntitlement().GetEntitlementName(endpointSystemId, entitlementType);
        //}
        public PresentationUserEntitlement GetEntitlement(IEndpointEntitlementInfo entitlement, string entitlementType)
        {
            return new PresentationADSUserEntitlement().GetEntitlement(entitlement, entitlementType);
        }
        public void UpdateEntitlement(PresentationUserEntitlement presentationUserEntitlement, IEndpointEntitlementInfo entitlement, string currentUser)
        {
            new PresentationADSUserEntitlement().UpdateEntitlement(presentationUserEntitlement, entitlement, currentUser);
        }
        public List<PresentationUserEntitlement> GetEntitlementDetail(long endpointUserId)
        {
            return new PresentationADSUserEntitlement().GetEntitlementDetail(endpointUserId);
        }
        public List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId)
        {
            return new PresentationADSUserEntitlement().GetEntitlementHierachyDetail(entitlementId);
        }
        public List<PresentationUserEntitlement> GetEntitlementDetailByApplicationType(long endpointsystemId)
        {
            return new PresentationADSUserEntitlement().GetEntitlementDetailByApplicationType(endpointsystemId);
        }

        public List<Group> GetAllGroupsByEndpointID(long endPointID, string searchString)
        {
            return ADSGroupInfo.GetAllGroupsByEndpointID(endPointID, searchString);
        }

        public Dictionary<long, string> GetEntitlements(string searchString, long endpointSystemId, long? EndpointUserId, bool existingEntitlements)
        {
            return ADSGroupInfo.GetEntitlements(searchString, endpointSystemId, EndpointUserId, existingEntitlements);
        }
        public void UpdatePasswordresetcount(long endpointId, string userName, string currentUser)
        {
            ADSUserInfo.UpdatePasswordresetcount(endpointId, userName, currentUser);
        }
        public List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(long endpointId, string userAccountName, long entitlementId, string entityCode, EntitlementType fromValue, EntitlementType toValue)
        {
            return new List<PresentationUserEntitlement>();
        }

        public List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId, string entitlementType, string userAccountName)
        {
            return new List<PresentationUserEntitlement>();
        }

        public List<PresentationUserEntitlement> GetAllEntitlementByType(EntitlementType type, long endpointsystemid)
        {
            return new PresentationADSUserEntitlement().GetAllEntitlementDetailByType(endpointsystemid, type);
        }

        public List<PresentationUserEntitlement> GetEntitlementDetailByApplicationTypeWithAllCanRequest(long endpointsystemId)
        {
            return new PresentationADSUserEntitlement().GetEntitlementDetailByApplicationTypeWithAllCanRequest(endpointsystemId);
        }

        public List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(EndPoint endpoint, string userAccountName, long entitlementId, string entityCode, EntitlementType fromValue, EntitlementType toValue)
        {
            throw new NotImplementedException();
        }

        public List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(EndPoint endpoint, string userAccountName, long entitlementId, string entityCode, EntitlementType fromValue, EntitlementType toValue, string reserved, string reserved2, string description, ApplicationType applicationType)
        {
            throw new NotImplementedException();
        }
    }
}
