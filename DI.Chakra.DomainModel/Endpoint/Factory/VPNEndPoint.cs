using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainModel
{
    public class VPNEndPoint : IEndPointFactory
    {
        public List<PresentationUserEntitlement> GetAllEntitlementByType(EntitlementType type, long endpointsystemid)
        {
            throw new NotImplementedException();
        }

        public List<Group> GetAllGroupsByEndpointID(long endPointID, string searchString)
        {
            throw new NotImplementedException();
        }

        public IEndpointEntitlementInfo GetEntitlement()
        {
            return new VPNMatrix();
        }

        public IEndpointEntitlementInfo GetEntitlement(long id)
        {
            return VPNMatrix.GetById(id);
        }

        public IEndpointEntitlementInfo GetEntitlement(long id, EntitlementType type)
        {
            switch (type)
            {
                //TODO:implementation need to be checked
                case (EntitlementType.Application):
                    return VPNMatrix.GetByIdAndType(id, VPNMatrixType.Application);
                case (EntitlementType.Project):
                    return VPNMatrix.GetByIdAndType(id, VPNMatrixType.Project);
                default:
                    throw new ApplicationException("Entitlement Type : " + type.ToString() + " not supported for VPN");
            }
        }

        public IEndpointEntitlementInfo GetEntitlement(long systemId, string entitlementName, EntitlementType entitlementType)
        {
            if (entitlementType == EntitlementType.Application)
            {
                return VPNMatrix.GetAll().Where(t => t.EndPointSystemId == systemId &&
                t.EntitlementType == entitlementType && t.EntitlementName.ToUpper() == entitlementName.ToUpper()).FirstOrDefault();
            }
            else if (entitlementType == EntitlementType.Project)
            {
                return VPNMatrix.GetAll().Where(t => t.EndPointSystemId == systemId &&
                t.EntitlementType == entitlementType && t.EntitlementName.ToUpper() == entitlementName.ToUpper()).FirstOrDefault();
            }
            else
                return null;
        }

        public IEndpointEntitlementInfo GetEntitlement(long systemId, string entitlementName)
        {
            return null;
        }

        public PresentationUserEntitlement GetEntitlement(IEndpointEntitlementInfo entitlement, string entitlementType)
        {
            throw new NotImplementedException();
        }

        public List<PresentationUserEntitlement> GetEntitlementDetail(long endPointUserId)
        {
            return new PresentationVPNUserEntitlement().GetEntitlementDetail(endPointUserId);
        }

        public List<PresentationUserEntitlement> GetEntitlementDetailByApplicationType(long endpointsystemId)
        {
            return new PresentationSAPUserEntitlement().GetEntitlementDetailByApplicationType(endpointsystemId);
        }

        public List<PresentationUserEntitlement> GetEntitlementDetailByApplicationTypeWithAllCanRequest(long endpointsystemId)
        {
            throw new NotImplementedException();
        }

        public List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId)
        {
            return new PresentationVPNUserEntitlement().GetEntitlementHierachyDetail(entitlementId);
        }

        public List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId, string entitlementType, string userAccountName)
        {
            return new List<PresentationUserEntitlement>();
        }

        public List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(EndPoint endpoint, string userAccountName, long entitlementId, string entityCode, EntitlementType fromValue, EntitlementType toValue)
        {
            return new List<PresentationUserEntitlement>();
        }

        public List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(EndPoint endpoint, string userAccountName, long entitlementId, string entityCode, EntitlementType fromValue, EntitlementType toValue, string reserved, string reserved2, string description, ApplicationType applicationType)
        {
            throw new NotImplementedException();
        }

        public List<EntitlementType> GetEntitlementList()
        {
            List<EntitlementType> entitlementTypes = new List<EntitlementType>();
            entitlementTypes.Add(EntitlementType.Application);
            entitlementTypes.Add(EntitlementType.Project);
            return entitlementTypes;
        }

        public Dictionary<long, string> GetEntitlements(string searchString, long endpointSystemId, long? EndpointUserId, bool existingEntitlements)
        {
            throw new NotImplementedException();
        }

        public IEndpointUserInfo GetUser()
        {
            return new VPNUserInfo();
        }

        public IEndpointUserInfo GetUser(long id)
        {
            return VPNUserInfo.GetById(id, true);
        }

        public void UpdateEntitlement(PresentationUserEntitlement presentationUserEntitlement, IEndpointEntitlementInfo entitlement, string currentUser)
        {
            throw new ApplicationException();
        }

        public void UpdatePasswordresetcount(long endpointId, string userName, string currentUser)
        {
            throw new ApplicationException();
        }
    }
}
