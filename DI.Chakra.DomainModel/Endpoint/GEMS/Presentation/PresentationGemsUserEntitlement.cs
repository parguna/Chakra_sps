using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainModel
{
    public class PresentationGemsUserEntitlement : Presentation.PresentationUserEntitlement
    {
        public override List<PresentationUserEntitlement> GetAllEntitlementDetailByType(long endpointSystemId, EntitlementType entitlementType)
        {
            throw new NotImplementedException();
        }

        public override PresentationUserEntitlement GetEntitlement(IEndpointEntitlementInfo entitlement, string entitlementType)
        {
            throw new NotImplementedException();
        }

        public override List<PresentationUserEntitlement> GetEntitlementDetail(long endpointUserId)
        {
            throw new NotImplementedException();
        }

        public override List<PresentationUserEntitlement> GetEntitlementDetailByApplicationType(long endpointSystemId)
        {
            throw new NotImplementedException();
        }

        public override List<PresentationUserEntitlement> GetEntitlementDetailByApplicationTypeWithAllCanRequest(long endpointSystemId)
        {
            throw new NotImplementedException();
        }

        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId)
        {
            throw new NotImplementedException();
        }

        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId, string entitlementType, string userAccountName)
        {
            throw new NotImplementedException();
        }

        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetail(string entitlementCode, long endpointId)
        {
            throw new NotImplementedException();
        }

        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(long endpointId, string entityCode, EntitlementType fromValue, EntitlementType toValue)
        {
            throw new NotImplementedException();
        }

        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(long endpointId, long entityId, EntitlementType fromValue, EntitlementType toValue)
        {
            throw new NotImplementedException();
        }

        public override void UpdateEntitlement(PresentationUserEntitlement presentationUserEntitlement, IEndpointEntitlementInfo entitlement, string currentUser)
        {
            throw new NotImplementedException();
        }
    }
}
