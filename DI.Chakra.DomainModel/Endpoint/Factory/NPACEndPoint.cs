using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Infrastructure;
using DI.Chakra.DomainModel.Presentation;

namespace DI.Chakra.DomainModel
{
    public class NPACEndPoint : IEndPointFactory
    {
        public List<PresentationUserEntitlement> GetAllEntitlementByType(EntitlementType type, long endpointsystemid)
        {
            return new PresentationSAPUserEntitlement().GetAllEntitlementDetailByType(endpointsystemid, type);
        }

        public IEndpointUserInfo GetUser()
        {
            return new NPACUserInfo();
        }
        public IEndpointUserInfo GetUser(long id)
        {
            return NPACUserInfo.GetById(id, true);
        }

        public IEndpointEntitlementInfo GetEntitlement()
        {
            return new NPACLevelInfo();
        }
        public List<EntitlementType> GetEntitlementList()
        {
            List<EntitlementType> entitlementTypes = new List<EntitlementType>();
            entitlementTypes.Add(EntitlementType.Level);
            return entitlementTypes;
        }
        public IEndpointEntitlementInfo GetEntitlement(long id)
        {
            return NPACLevelInfo.GetById(id);
        }
        public IEndpointEntitlementInfo GetEntitlement(long systemId, string entitlementName, EntitlementType entitlementType)
        {
            if (entitlementType == EntitlementType.Role)
            {
                return NPACLevelInfo.GetAll().Where(t => t.EndPointSystemId == systemId &&
                t.EntitlementType == entitlementType && t.EntitlementName.ToUpper() == entitlementName.ToUpper()).FirstOrDefault();
            }
            else
                return null;
        }
        public IEndpointEntitlementInfo GetEntitlement(long systemId, string entitlementName)
        {
            return null;
        }
        public IEndpointEntitlementInfo GetEntitlement(long id, EntitlementType type)
        {
            switch (type)
            {
                case (EntitlementType.Level):
                    return NPACLevelInfo.GetById(id);
                default:
                    throw new ApplicationException("Entitlement Type : " + type.ToString() + " not supported for AD");
            }
        }

        //public void SaveUserInfo(long id, long endPointSystemId, string currentUser)
        //{
        //    SAPUserinfo.GetUserAccountByEndPointSystemId(id, endPointSystemId).Save(currentUser);
        //}
        //public Dictionary<long, string> GetEntitlementName(long endpointSystemId, string entitlementType)
        //{
        //    return new PresentationSAPUserEntitlement().GetEntitlementName(endpointSystemId, entitlementType);
        //}
        public PresentationUserEntitlement GetEntitlement(IEndpointEntitlementInfo entitlement, string entitlementType)
        {
            return new PresentationSAPUserEntitlement().GetEntitlement(entitlement, entitlementType);
        }
        public void UpdateEntitlement(PresentationUserEntitlement presentationUserEntitlement, IEndpointEntitlementInfo entitlement, string currentUser)
        {
            new PresentationSAPUserEntitlement().UpdateEntitlement(presentationUserEntitlement, entitlement, currentUser);
        }
        public List<PresentationUserEntitlement> GetEntitlementDetail(long entitlementId)
        {
            return new PresentationSAPUserEntitlement().GetEntitlementDetail(entitlementId);
        }

        public List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId)
        {
            return new PresentationSAPUserEntitlement().GetEntitlementHierachyDetail(entitlementId);
        }

        public List<PresentationUserEntitlement> GetEntitlementDetailByApplicationType(long endpointsystemId)
        {
            return new PresentationSAPUserEntitlement().GetEntitlementDetailByApplicationType(endpointsystemId);
        }

        public List<Group> GetAllGroupsByEndpointID(long endPointID, string searchString)
        {
            return SAPGroupInfo.GetAllGroupsByEndpointID(endPointID, searchString);
        }
        public Dictionary<long, string> GetEntitlements(string searchString, long endpointSystemId, long? EndpointUserId, bool existingEntitlements)
        {
            throw new ApplicationException();
        }
        public void UpdatePasswordresetcount(long endpointId, string userName, string currentUser)
        {
            throw new ApplicationException();
        }
        public List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(EndPoint endpoint, string userAccountName, long entitlementId, string entityCode, EntitlementType fromValue, EntitlementType toValue)
        {
            return new List<PresentationUserEntitlement>();
        }
        public List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId, string entitlementType, string userAccountName)
        {
            return new List<PresentationUserEntitlement>();
        }

        public List<PresentationUserEntitlement> GetEntitlementDetailByApplicationTypeWithAllCanRequest(long endpointsystemId)
        {
            return new PresentationSAPUserEntitlement().GetEntitlementDetailByApplicationType(endpointsystemId);
        }

        public List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(EndPoint endpoint, string userAccountName, long entitlementId, string entityCode, EntitlementType fromValue, EntitlementType toValue, string reserved, string reserved2, string description, ApplicationType applicationType)
        {
            throw new NotImplementedException();
        }
    }
}
