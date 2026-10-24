using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;

namespace DI.Chakra.DomainModel
{
    public class PresentationVPNUserEntitlement : Presentation.PresentationUserEntitlement
    {
        public override List<PresentationUserEntitlement> GetAllEntitlementDetailByType(long endpointSystemId, EntitlementType entitlementType)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            var vpnEntilements = new List<EntitlementReconcilation>();
            switch (entitlementType)
            {
                case EntitlementType.Application:
                   
                    vpnEntilements = EntitlementReconcilation.GetAllEntitlementByType(endpointSystemId,(int) entitlementType).ToList();
                    vpnEntilements.ForEach(lq =>
                    {
                        PresentationVPNUserEntitlement UserEntitlement = new PresentationVPNUserEntitlement();
                        UserEntitlement.EntitlementId = lq.Id;
                        UserEntitlement.Category = Convert.ToString(EntitlementType.Application);
                        UserEntitlement.Name = lq.Entitlement;
                        UserEntitlement.Description = lq.EntitlementDescription;
                        UserEntitlement.CanRequest = true;
                        UserEntitlement.UserDefinedDesc = lq.EntitlementDescription;
                        UserEntitlement.EndpointSystemId = endpointSystemId;
                        UserEntitlement.EntitlementType = EntitlementType.Application;

                        presentationUserEntitlementList.Add(UserEntitlement);
                    });
                    return presentationUserEntitlementList;
                case EntitlementType.Project:
                    vpnEntilements = EntitlementReconcilation.GetAllEntitlementByType(endpointSystemId, (int)entitlementType).ToList();
                    vpnEntilements.ForEach(lq =>
                    {
                        PresentationVPNUserEntitlement UserEntitlement = new PresentationVPNUserEntitlement();
                        UserEntitlement.EntitlementId = lq.Id;
                        UserEntitlement.Category = Convert.ToString(EntitlementType.Project);
                        UserEntitlement.Name = lq.Entitlement;
                        UserEntitlement.Description = lq.EntitlementDescription;
                        UserEntitlement.CanRequest = true;
                        UserEntitlement.UserDefinedDesc = lq.EntitlementDescription;
                        UserEntitlement.EndpointSystemId = endpointSystemId;
                        UserEntitlement.EntitlementType = EntitlementType.Project;

                        presentationUserEntitlementList.Add(UserEntitlement);
                    });
                    return presentationUserEntitlementList;
                default:
                    return new List<PresentationUserEntitlement>();
            }
        }

        public override PresentationUserEntitlement GetEntitlement(IEndpointEntitlementInfo entitlement, string entitlementType)
        {
            var vpnMatrix = (VPNMatrix)entitlement;
            PresentationVPNUserEntitlement UserEntitlement = new PresentationVPNUserEntitlement();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                UserEntitlement.EntitlementId = vpnMatrix.Id;
                UserEntitlement.Category = Convert.ToString(vpnMatrix.VPNMatrixType == VPNMatrixType.Application ? EntitlementType.Application : EntitlementType.Project);
                UserEntitlement.Name = vpnMatrix.Name;
                UserEntitlement.Description = vpnMatrix.Name;
                UserEntitlement.UserDefinedDesc = vpnMatrix.Name;
                UserEntitlement.EndPointSystemType = EndpointType.VPN;
                UserEntitlement.EntitlementType = vpnMatrix.VPNMatrixType == VPNMatrixType.Application? EntitlementType.Application: EntitlementType.Project;
                UserEntitlement.CanRequest = true;
            }
            return UserEntitlement;
        }

        public override List<PresentationUserEntitlement> GetEntitlementDetail(long endpointUserId)
        {
            var presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            var vpnUserInfo = VPNUserInfo.GetByEndpointUserId(endpointUserId);
            var entitlementRecons = EntitlementReconcilation.GetByEndpointUser(endpointUserId, vpnUserInfo.EndPointSystemId);
            foreach (var entitlementRecon in entitlementRecons)
            {
                var UserEntitlement = new PresentationVPNUserEntitlement();
                UserEntitlement.EntitlementId = entitlementRecon.EntitlementId;
                UserEntitlement.UserName = vpnUserInfo.AccountName;
                UserEntitlement.Category = Convert.ToString(entitlementRecon.EntitlementType == (int) EntitlementType.Application ? EntitlementType.Application : EntitlementType.Project);
                UserEntitlement.EndPointSystemType = vpnUserInfo.EndPointSystem.EndPointSystemType.EndpointSystemTypeName;
                UserEntitlement.EndpointSystemName = vpnUserInfo.EndPointSystem.DisplayName;
                UserEntitlement.EndpointSystemId = vpnUserInfo.EndPointSystemId;
                UserEntitlement.EntitlementType = entitlementRecon.EntitlementType == (int)EntitlementType.Application ? EntitlementType.Application : EntitlementType.Project;
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
    }
}
