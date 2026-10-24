using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Transaction;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainModel
{
    public class PresentationSAPEPUserEntitlement : Presentation.PresentationUserEntitlement
    {
        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(long endpointId, long entityId, EntitlementType fromValue, EntitlementType toValue)
        {
            return new List<PresentationUserEntitlement>();
        }
        public override List<PresentationUserEntitlement> GetAllEntitlementDetailByType(long endpointSystemId, EntitlementType entitlementType)
        {
            switch (entitlementType)
            {
                case EntitlementType.Role:
                    List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
                    var adsGroups = SAPEPRoleInfo.GetByEndpointSystem(endpointSystemId).ToList();
                    adsGroups.ForEach(lq =>
                    {
                        PresentationADSUserEntitlement UserEntitlement = new PresentationADSUserEntitlement();
                        UserEntitlement.EntitlementId = lq.Id;
                        UserEntitlement.Category = Convert.ToString(EntitlementType.Role);
                        UserEntitlement.Name = lq.RoleName;
                        UserEntitlement.Description = lq.RoleDescription;
                        UserEntitlement.CanRequest = lq.CanRequest;
                        UserEntitlement.UserDefinedDesc = lq.UserDefinedDesc;
                        UserEntitlement.EndpointSystemId = endpointSystemId;
                        UserEntitlement.EntitlementType = EntitlementType.Role;

                        presentationUserEntitlementList.Add(UserEntitlement);
                    });
                    return presentationUserEntitlementList;
                default:
                    return new List<PresentationUserEntitlement>();
            }
        }

        public override List<PresentationUserEntitlement> GetEntitlementDetailByApplicationType(long endpointsystemId)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var Entitlement = SAPEPRoleInfo.GetByEndpointSystem(endpointsystemId);
                Entitlement.ForEach(lq =>
                {
                    PresentationSAPEPUserEntitlement UserEntitlement = new PresentationSAPEPUserEntitlement();
                    UserEntitlement.EntitlementId = lq.Id;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Role);
                    UserEntitlement.Name = lq.RoleName;
                    UserEntitlement.Description = lq.RoleDescription;
                    UserEntitlement.CanRequest = lq.CanRequest;
                    UserEntitlement.UserDefinedDesc = lq.UserDefinedDesc;
                    UserEntitlement.EndpointSystemId = endpointsystemId;
                    presentationUserEntitlementList.Add(UserEntitlement);
                });
            }
            return presentationUserEntitlementList;
        }
        public override void UpdateEntitlement(PresentationUserEntitlement presentationEntitlement, IEndpointEntitlementInfo entitlement, string currentUser)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                SAPEPRoleInfo sapepInfo = (SAPEPRoleInfo)entitlement;
                sapepInfo.UserDefinedDesc = presentationEntitlement.UserDefinedDesc;
                sapepInfo.CanRequest = presentationEntitlement.CanRequest;
                sapepInfo.Save(dataAccessTransaction);
                dataAccessTransaction.Commit();
            }
        }
        public override List<PresentationUserEntitlement> GetEntitlementDetail(long endpointUserId)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            SAPEPUserInfo SAPEPUserInfo = new SAPEPUserInfo();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                SAPEPUserInfo = SAPEPUserInfo.GetByIdWithNavigation(endpointUserId);

                if (SAPEPUserInfo == null)
                    return presentationUserEntitlementList;

                foreach (SAPEPGroupInfo grp in SAPEPUserInfo.SAPEPGroups)
                {
                    PresentationSAPEPUserEntitlement UserEntitlement = new PresentationSAPEPUserEntitlement();
                    UserEntitlement.EntitlementId = SAPEPUserInfo.Id;
                    UserEntitlement.UserName = SAPEPUserInfo.AccountName;
                    UserEntitlement.EndPointSystemType = SAPEPUserInfo.EndPointSystem.EndPointSystemType.EndpointSystemTypeName;
                    UserEntitlement.EndpointSystemName = SAPEPUserInfo.EndPointSystem.DisplayName;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Group);
                    if (grp != null)
                    {
                        UserEntitlement.Name = grp.GroupName;
                        UserEntitlement.Description = grp.GroupDescription;
                        presentationUserEntitlementList.Add(UserEntitlement);
                    }
                }
                foreach (SAPEPRoleInfo role in SAPEPUserInfo.SAPEPRoles)
                {
                    PresentationSAPEPUserEntitlement UserEntitlement = new PresentationSAPEPUserEntitlement();
                    UserEntitlement.EntitlementId = SAPEPUserInfo.Id;
                    UserEntitlement.UserName = SAPEPUserInfo.AccountName;
                    UserEntitlement.EndPointSystemType = SAPEPUserInfo.EndPointSystem.EndPointSystemType.EndpointSystemTypeName;
                    UserEntitlement.EndpointSystemName = SAPEPUserInfo.EndPointSystem.DisplayName;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Group);
                    if (role != null)
                    {
                        UserEntitlement.Name = role.RoleName;
                        UserEntitlement.Description = role.RoleDescription;
                        presentationUserEntitlementList.Add(UserEntitlement);
                    }
                }
            }
            return presentationUserEntitlementList;
        }
        public override PresentationUserEntitlement GetEntitlement(IEndpointEntitlementInfo entitlement, string entitlementType)
        {
            SAPEPRoleInfo sapepRoleInfo = (SAPEPRoleInfo)entitlement;
            PresentationSAPEPUserEntitlement UserEntitlement = new PresentationSAPEPUserEntitlement();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                UserEntitlement.EntitlementId = sapepRoleInfo.Id;
                UserEntitlement.Category = Convert.ToString(EntitlementType.Role);
                UserEntitlement.Name = sapepRoleInfo.RoleName;
                UserEntitlement.Description = sapepRoleInfo.RoleDescription;
                UserEntitlement.UserDefinedDesc = sapepRoleInfo.UserDefinedDesc;
                UserEntitlement.EndPointSystemType = EndpointType.SAPEnterprisePortal;
                UserEntitlement.CanRequest = sapepRoleInfo.CanRequest;
            }
            return UserEntitlement;
        }

        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId)
        {
            return new List<PresentationUserEntitlement>();
        }
        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetail(string entitlementCode, long endpointId)
        {
            return new List<PresentationUserEntitlement>();
        }
        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId, string entitlementType, string userAccountName)
        {
            return new List<PresentationUserEntitlement>();
        }
        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(long endpointId, string entityCode, EntitlementType fromValue, EntitlementType toValue)
        {
            return new List<PresentationUserEntitlement>();
        }

        public override List<PresentationUserEntitlement> GetEntitlementDetailByApplicationTypeWithAllCanRequest(long endpointSystemId)
        {
            throw new NotImplementedException();
        }
    }
}
