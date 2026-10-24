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
    public class IDASEndpoint : IEndPointFactory
    {
        List<Group> IEndPointFactory.GetAllGroupsByEndpointID(long endPointID, string searchString)
        {
            return IACMGroupInfo.GetAllByEndpointSystemId(endPointID, searchString);
        }

        IEndpointEntitlementInfo IEndPointFactory.GetEntitlement()
        {
            return new IACMRoleInfo();
        }

        IEndpointEntitlementInfo IEndPointFactory.GetEntitlement(long id)
        {
            return IACMRoleInfo.GetById(id);
        }

        IEndpointEntitlementInfo IEndPointFactory.GetEntitlement(long systemId, string entitlementName)
        {
            return IACMRoleInfo.GetAll().Where(t => t.EndPointSystemId == systemId &&
            t.EntitlementName.ToUpper() == entitlementName.ToUpper()).FirstOrDefault();
        }

        public IEndpointEntitlementInfo GetEntitlement(long id, EntitlementType type)
        {
            switch (type)
            {
                case (EntitlementType.Group):
                    return IACMGroupInfo.GetById(id);
                case (EntitlementType.Role):
                    return IACMRoleInfo.GetById(id);
                default:
                    throw new ApplicationException("Entitlement Type : " + type.ToString() + " not supported for IACM");
            }
        }

        public IEndpointEntitlementInfo GetEntitlement(long systemId, string entitlementName, EntitlementType entitlementType)
        {
            if (entitlementType == EntitlementType.Role)
            {
                return IACMRoleInfo.GetAll().Where(t => t.EndPointSystemId == systemId &&
                  t.EntitlementType == entitlementType && t.EntitlementName.ToUpper() == entitlementName.ToUpper()).FirstOrDefault();
            }
            else
            {
                return IACMGroupInfo.GetAll().Where(t => t.EndPointSystemId == systemId &&
                   t.EntitlementType == entitlementType && t.EntitlementName.ToUpper() == entitlementName.ToUpper()).FirstOrDefault();
            }
        }
        public IEndpointEntitlementInfo GetEntitlement(long systemId, string entitlementName)
        {
            return IACMRoleInfo.GetAll().Where(t => t.EndPointSystemId == systemId && t.EntitlementName.ToUpper() == entitlementName.ToUpper()).FirstOrDefault();
        }

        PresentationUserEntitlement IEndPointFactory.GetEntitlement(IEndpointEntitlementInfo entitlement, string entitlementType)
        {
            return new PresentationIACMUserEntitlement().GetEntitlement(entitlement, entitlementType);
        }

        IEndpointEntitlementInfo IEndPointFactory.GetEntitlement(long id, EntitlementType type)
        {
            switch (type)
            {
                case (EntitlementType.Role):
                    return IACMRoleInfo.GetById(id);
                case (EntitlementType.Group):
                    return IACMGroupInfo.GetById(id);
                default:
                    throw new ApplicationException("Entitlement Type : " + type.ToString() + " not supported for IDAS");
            }
        }

        IEndpointEntitlementInfo IEndPointFactory.GetEntitlement(long systemId, string entitlementName, EntitlementType entitlementType)
        {
            if(entitlementType == EntitlementType.Role)
                return IACMRoleInfo.GetAll().Where(t => t.Name.ToUpper() == entitlementName.ToUpper()).FirstOrDefault();
            else
                return IACMGroupInfo.GetAll().Where(t => t.Name.ToUpper() == entitlementName.ToUpper()).FirstOrDefault();
            //throw new NotImplementedException();
        }

        List<PresentationUserEntitlement> IEndPointFactory.GetEntitlementDetail(long endpointUserId)
        {
            return new PresentationIACMUserEntitlement().GetEntitlementDetail(endpointUserId);
        }

        List<PresentationUserEntitlement> IEndPointFactory.GetEntitlementDetailByApplicationType(long endpointsystemId)
        {
            return new PresentationIACMUserEntitlement().GetEntitlementDetailByApplicationType(endpointsystemId);
        }

        List<EntitlementType> IEndPointFactory.GetEntitlementList()
        {            
            List<EntitlementType> entitlementTypes = new List<EntitlementType>();
            entitlementTypes.Add(EntitlementType.Role);
            entitlementTypes.Add(EntitlementType.Group);
            return entitlementTypes;
        }

        Dictionary<long, string> IEndPointFactory.GetEntitlements(string searchString, long endpointSystemId, long? EndpointUserId, bool existingEntitlements)
        {
            return IACMGroupInfo.GetEntitlements(searchString, endpointSystemId, EndpointUserId, existingEntitlements);
        }

        IEndpointUserInfo IEndPointFactory.GetUser()
        {
            return new IACMUserInfo();
        }

        public List<PresentationUserEntitlement> GetAllEntitlementByType(EntitlementType type, long endpointsystemid)
        {
            return new PresentationIACMUserEntitlement().GetAllEntitlementDetailByType(endpointsystemid, type);
        }

        IEndpointUserInfo IEndPointFactory.GetUser(long id)
        {
            return IACMUserInfo.GetById(id, true);
        }

        void IEndPointFactory.UpdateEntitlement(PresentationUserEntitlement presentationUserEntitlement, IEndpointEntitlementInfo entitlement, string currentUser)
        {
            new PresentationIACMUserEntitlement().UpdateEntitlement(presentationUserEntitlement, entitlement, currentUser);
        }

        void IEndPointFactory.UpdatePasswordresetcount(long endpointId, string userName, string currentUser)
        {
            
        }

        public List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId)
        {
            return new PresentationIACMUserEntitlement().GetEntitlementHierachyDetail(entitlementId);
        }

        public List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId, string entitlementType, string userAccountName)
        {
            return new List<PresentationUserEntitlement>();
        }

        public List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(EndPoint endpoint, string userAccountName, long entitlementId, string entityCode, EntitlementType fromValue, EntitlementType toValue)
        {
            var entitlements = new List<PresentationUserEntitlement>();
            entitlements = new PresentationIACMUserEntitlement().GetEntitlementHierachyDetailByLevel(endpoint.Id, entityCode, fromValue, toValue);
            return entitlements;
        }

        public List<PresentationUserEntitlement> GetEntitlementDetailByApplicationTypeWithAllCanRequest(long endpointsystemId)
        {
            return new PresentationIACMUserEntitlement().GetEntitlementDetailByApplicationTypeWithAllCanRequest(endpointsystemId);
        }

        public List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(EndPoint endpoint, string userAccountName, long entitlementId, string entityCode, EntitlementType fromValue, EntitlementType toValue, string reserved, string reserved2, string description, ApplicationType applicationType)
        {
            var entitlements = new List<PresentationUserEntitlement>();
            entitlements = new PresentationIACMUserEntitlement().GetEntitlementHierachyDetailByLevel(endpoint.Id, entityCode, fromValue, toValue);
            return entitlements;
        }
    }
}
