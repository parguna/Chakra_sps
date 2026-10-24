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
    public class InhouseEndPoint : IEndPointFactory
    {
        public List<PresentationUserEntitlement> GetAllEntitlementByType(EntitlementType type, long endpointsystemid)
        {
            return new PresentationInhouseUserEntitlement().GetAllEntitlementDetailByType(endpointsystemid, type);
        }

        public IEndpointUserInfo GetUser()
        {
            return new InhouseUserInfo();
        }

        public IEndpointUserInfo GetUser(long id)
        {
            return InhouseUserInfo.GetById(id, true);
        }

        public IEndpointEntitlementInfo GetEntitlement()
        {
            return new InhousePermission();
        }
        public List<EntitlementType> GetEntitlementList()
        {
            List<EntitlementType> entitlementTypes = new List<EntitlementType>();
            entitlementTypes.Add(EntitlementType.Permission);
            entitlementTypes.Add(EntitlementType.Function);
            return entitlementTypes;
        }
        public IEndpointEntitlementInfo GetEntitlement(long id)
        {
            return InhousePermission.GetById(id);
        }

        public IEndpointEntitlementInfo GetEntitlement(long id, EntitlementType type)
        {
            switch (type)
            {
                case (EntitlementType.Permission):
                    return InhousePermission.GetById(id);
                default:
                    throw new ApplicationException("Entitlement Type : " + type.ToString() + " not supported for Inhouse");
            }
        }

        public IEndpointEntitlementInfo GetEntitlement(long systemId, string entitlementName, EntitlementType entitlementType)
        {
            return InhousePermission.GetAll().Where(t => t.EndPointSystemId == systemId &&
            t.EntitlementType == entitlementType && t.EntitlementName.ToUpper() == entitlementName.ToUpper()).FirstOrDefault();
        }
        public IEndpointEntitlementInfo GetEntitlement(long systemId, string entitlementName)
        {
            return InhousePermission.GetAll().Where(t => t.EndPointSystemId == systemId &&
            t.EntitlementName.ToUpper() == entitlementName.ToUpper()).FirstOrDefault();
        }
        //public void SaveUserInfo(long id, long endPointSystemId, string currentUser)
        //{
        //    ADSUserInfo.GetUserAccountByEndPointSystemId(id, endPointSystemId).Save(currentUser);
        //}
        //public Dictionary<long, string> GetEntitlementName(long endpointSystemId, string entitlementType)
        //{
        //    return new PresentationInhouseUserEntitlement().GetEntitlementName(endpointSystemId, entitlementType);
        //}
        public PresentationUserEntitlement GetEntitlement(IEndpointEntitlementInfo entitlement, string entitlementType)
        {
            return new PresentationInhouseUserEntitlement().GetEntitlement(entitlement, entitlementType);
        }
        public void UpdateEntitlement(PresentationUserEntitlement presentationUserEntitlement, IEndpointEntitlementInfo entitlement, string currentUser)
        {
            new PresentationInhouseUserEntitlement().UpdateEntitlement(presentationUserEntitlement, entitlement, currentUser);
        }
        public List<PresentationUserEntitlement> GetEntitlementDetail(long endpointUserId)
        {
            return new PresentationInhouseUserEntitlement().GetEntitlementDetail(endpointUserId);
        }
        public List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId)
        {
            return new PresentationInhouseUserEntitlement().GetEntitlementHierachyDetail(entitlementId);
        }
        public List<PresentationUserEntitlement> GetEntitlementDetailByApplicationType(long endpointsystemId)
        {
            return new PresentationInhouseUserEntitlement().GetEntitlementDetailByApplicationType(endpointsystemId);
        }

        public List<Group> GetAllGroupsByEndpointID(long endPointID, string searchString)
        {
            throw new ApplicationException();
        }
        public Dictionary<long, string> GetEntitlements(string searchString, long endpointSystemId, long? EndpointUserId, bool existingEntitlements)
        {
            throw new ApplicationException();
        }
        public void UpdatePasswordresetcount(long endpointId, string userName, string currentUser)
        {

        }
        public List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(EndPoint endpoint, string userAccountName, long entitlementId, string entityCode, EntitlementType fromValue, EntitlementType toValue)
        {

            var entitlements = new List<PresentationUserEntitlement>();
            entitlements = new PresentationInhouseUserEntitlement().GetEntitlementHierachyDetailByLevel(endpoint.Id, entityCode, fromValue, toValue);
            return entitlements;
        }
        public List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId, string entitlementType, string userAccountName)
        {
            return new List<PresentationUserEntitlement>();
        }
        public List<PresentationUserEntitlement> GetEntitlementDetailByApplicationTypeWithAllCanRequest(long endpointsystemId)
        {
            return new PresentationInhouseUserEntitlement().GetEntitlementDetailByApplicationTypeWithAllCanRequest(endpointsystemId);
        }

        public List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(EndPoint endpoint, string userAccountName, long entitlementId, string entityCode, EntitlementType fromValue, EntitlementType toValue, string reserved, string reserved2, string description, ApplicationType applicationType)
        {
            var entitlements = new List<PresentationUserEntitlement>();
            entitlements = new PresentationInhouseUserEntitlement().GetEntitlementHierachyDetailByLevel(endpoint.Id, entityCode, fromValue, toValue);
            return entitlements;
        }
    }
}
