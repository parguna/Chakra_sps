using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public interface IEndPointFactory
    {

        IEndpointUserInfo GetUser();

        IEndpointUserInfo GetUser(long id);

        IEndpointEntitlementInfo GetEntitlement();
        List<EntitlementType> GetEntitlementList();
        List<PresentationUserEntitlement> GetAllEntitlementByType(EntitlementType type, long endpointsystemid);
        IEndpointEntitlementInfo GetEntitlement(long id);

        IEndpointEntitlementInfo GetEntitlement(long id, EntitlementType type);

        IEndpointEntitlementInfo GetEntitlement(long systemId, string entitlementName, EntitlementType entitlementType);
        IEndpointEntitlementInfo GetEntitlement(long systemId, string entitlementName);
        //void SaveUserInfo(long id, long endPointSystemId, string currentUser);

        PresentationUserEntitlement GetEntitlement(IEndpointEntitlementInfo entitlement, string entitlementType);

        void UpdateEntitlement(PresentationUserEntitlement presentationUserEntitlement, IEndpointEntitlementInfo entitlement, string currentUser);

        List<PresentationUserEntitlement> GetEntitlementDetail(long endpointId);

        List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId);

        List<PresentationUserEntitlement> GetEntitlementDetailByApplicationType(long endpointsystemId);
        List<PresentationUserEntitlement> GetEntitlementDetailByApplicationTypeWithAllCanRequest(long endpointsystemId);

        List<Group> GetAllGroupsByEndpointID(long endPointID, string searchString);

        Dictionary<long, string> GetEntitlements(string searchString, long endpointSystemId, long? EndpointUserId, bool existingEntitlements);

        void UpdatePasswordresetcount(long endpointId, string userName, string currentUser);

        List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId, string entitlementType, string userAccountName);
        //List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(long endpointId, string userAccountName, long entitlementId, string entityCode, EntitlementType fromValue, EntitlementType toValue);
        List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(EndPoint endpoint, string userAccountName, long entitlementId, string entityCode, EntitlementType fromValue, EntitlementType toValue);
        List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(EndPoint endpoint, string userAccountName, long entitlementId, string entityCode, EntitlementType fromValue, EntitlementType toValue, string reserved, string reserved2, string description, ApplicationType applicationType);
    }
    public class PresentationEndpointDefaultRole
    {
        public PresentationEndpointDefaultRole(EntitlementType entitlementType, string defaultRole)
        {
            this.EntitlementType = entitlementType;
            this.DefaultRole = defaultRole;
        }
        public EntitlementType EntitlementType { get; set; }
        public string DefaultRole { get; set; }
    }
}
