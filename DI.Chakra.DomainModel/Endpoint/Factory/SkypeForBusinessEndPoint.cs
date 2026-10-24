using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainModel
{
    public class SkypeForBusinessEndPoint : IEndPointFactory
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
            throw new NotImplementedException();
        }

        public IEndpointEntitlementInfo GetEntitlement(long id)
        {
            throw new NotImplementedException();
        }

        public IEndpointEntitlementInfo GetEntitlement(long id, EntitlementType type)
        {
            switch (type)
            {
                //TODO:implementation need to be checked
                case (EntitlementType.ResponseGroup):
                    return SkypeForBusinessResponseGroupInfo.GetById(id);
                case (EntitlementType.DCC):
                case (EntitlementType.DCCS):
                case (EntitlementType.IDD):
                case (EntitlementType.Default):
                    return SkypeForBusinessUserInfo.GetByIdAndType(id, type);
                default:
                    throw new ApplicationException("Entitlement Type : " + type.ToString() + " not supported for SkypeForBusiness");
            }
        }

        public IEndpointEntitlementInfo GetEntitlement(long systemId, string entitlementName, EntitlementType entitlementType)
        {
            throw new NotImplementedException();
        }

        public IEndpointEntitlementInfo GetEntitlement(long systemId, string entitlementName)
        {
            throw new NotImplementedException();
        }

        public PresentationUserEntitlement GetEntitlement(IEndpointEntitlementInfo entitlement, string entitlementType)
        {
            throw new NotImplementedException();
        }

        public List<PresentationUserEntitlement> GetEntitlementDetail(long endPointUserId)
        {
            return new PresentationSkypeForBusinessUserEntitlement().GetEntitlementDetail(endPointUserId);
        }

        public List<PresentationUserEntitlement> GetEntitlementDetailByApplicationType(long endpointsystemId)
        {
            throw new NotImplementedException();
        }

        public List<PresentationUserEntitlement> GetEntitlementDetailByApplicationTypeWithAllCanRequest(long endpointsystemId)
        {
            throw new NotImplementedException();
        }

        public List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId)
        {
            throw new NotImplementedException();
        }

        public List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId, string entitlementType, string userAccountName)
        {
            throw new NotImplementedException();
        }

        public List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(EndPoint endpoint, string userAccountName, long entitlementId, string entityCode, EntitlementType fromValue, EntitlementType toValue)
        {
            throw new NotImplementedException();
        }

        public List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(EndPoint endpoint, string userAccountName, long entitlementId, string entityCode, EntitlementType fromValue, EntitlementType toValue, string reserved, string reserved2, string description, ApplicationType applicationType)
        {
            throw new NotImplementedException();
        }

        public List<EntitlementType> GetEntitlementList()
        {
            List<EntitlementType> types = new List<EntitlementType>();
            types.Add(EntitlementType.Default);
            types.Add(EntitlementType.DCC);
            types.Add(EntitlementType.DCCS);
            types.Add(EntitlementType.IDD);
            types.Add(EntitlementType.ResponseGroup);
            return types;
        }

        public Dictionary<long, string> GetEntitlements(string searchString, long endpointSystemId, long? EndpointUserId, bool existingEntitlements)
        {
            throw new NotImplementedException();
        }

        public IEndpointUserInfo GetUser()
        {
            throw new NotImplementedException();
        }

        public IEndpointUserInfo GetUser(long id)
        {
            return SkypeForBusinessUserInfo.GetById(id, true);
        }

        public void UpdateEntitlement(PresentationUserEntitlement presentationUserEntitlement, IEndpointEntitlementInfo entitlement, string currentUser)
        {
            throw new NotImplementedException();
        }

        public void UpdatePasswordresetcount(long endpointId, string userName, string currentUser)
        {
            throw new NotImplementedException();
        }
    }
}
