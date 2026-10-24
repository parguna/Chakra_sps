using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel 
{
    public class PresentationEAMNovaUserEntitlement :  PresentationUserEntitlement
    {
       
        public override List<PresentationUserEntitlement> GetAllEntitlementDetailByType(long endpointSystemId, EntitlementType entitlementType)
        {
            long Id = 0;
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            var _novaMatrixResponse = loadMatrixData(new NovaMatrixParmeter() { novaMatrixType = NovaMatrixType.All });
            foreach (var matrix in _novaMatrixResponse.NovaMatrixResponseDetails) {
                presentationUserEntitlementList.Add(new PresentationADSUserEntitlement {   
                EntitlementId = Id++,
                Category =  Convert.ToString(EntitlementType.Role),
                Name = matrix.Position,
                Description = matrix.ToString(),
                CanRequest = true,
                UserDefinedDesc = matrix.ToString(),
                EndpointSystemId = endpointSystemId,
                EntitlementType = EntitlementType.Role,
                UserDefinedDisplayName = matrix.JobDescription 
                } );
            }
            return presentationUserEntitlementList;
        }

        public override PresentationUserEntitlement GetEntitlement(IEndpointEntitlementInfo entitlement, string entitlementType)
        {
            SAPEamNovaUserRoleInfo userRoleInfo = entitlement as SAPEamNovaUserRoleInfo;

            if (userRoleInfo == null) return new PresentationADSUserEntitlement();

            return new PresentationADSUserEntitlement
            {
                EntitlementId = Id++,
                Category = Convert.ToString(EntitlementType.Role),
                Name = userRoleInfo.RoleName,
                Description = userRoleInfo.EntitlementName,
                CanRequest = true,
                UserDefinedDesc = userRoleInfo.EntitlementName,
                EntitlementType = EntitlementType.Role
            };
        }

        public override List<PresentationUserEntitlement> GetEntitlementDetail(long endpointUserId)
        {
            var ep = DI.Chakra.DomainModel.EndPoint.GetByApplicationType(ApplicationType.EAM);
            var presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            var userInfo = SAPEamNovaUserInfo.GetById(endpointUserId);
            var entitlementRecons = EntitlementReconcilation.GetByEndpointUser(endpointUserId, userInfo.EndPointSystemId);
            foreach (var entitlementRecon in entitlementRecons)
            {
                var entType = (EntitlementType)entitlementRecon.EntitlementType;
                var UserEntitlement = new PresentationSkypeForBusinessUserEntitlement();
                UserEntitlement.EntitlementId = entitlementRecon.EntitlementId;
                UserEntitlement.UserName = userInfo.AccountName;
                UserEntitlement.Category = Convert.ToString(entType);
                UserEntitlement.EndPointSystemType = EndpointType.EAM_NOVA;
                UserEntitlement.EndpointSystemName = ep.DisplayName;
                UserEntitlement.EndpointSystemId = userInfo.EndPointSystemId;
                UserEntitlement.EntitlementType = entType;
                UserEntitlement.Name = entitlementRecon.Entitlement;
                UserEntitlement.Description = entitlementRecon.EntitlementDescription;
                presentationUserEntitlementList.Add(UserEntitlement);
            }
            return presentationUserEntitlementList;
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

        NovaMatrixResponse loadMatrixData(NovaMatrixParmeter parameter)
        {
            return DomainModel.NOVAUserRoleMatrix.GetNovaMatrix(parameter);
        }

    }
}
