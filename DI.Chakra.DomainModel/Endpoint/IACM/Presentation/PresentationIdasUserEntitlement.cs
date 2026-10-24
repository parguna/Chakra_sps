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
    public class PresentationIdasUserEntitlement : Presentation.PresentationUserEntitlement
    {
        public override PresentationUserEntitlement GetEntitlement(IEndpointEntitlementInfo entitlement, string entitlementType)
        {
            Role role = (Role)entitlement;
            PresentationIdasUserEntitlement UserEntitlement = new PresentationIdasUserEntitlement();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                UserEntitlement.EntitlementId = role.Id;
                UserEntitlement.Category = Convert.ToString(EntitlementType.Role);
                UserEntitlement.Name = role.RoleName;
                UserEntitlement.Description = role.RoleDescription;
                UserEntitlement.UserDefinedDesc = role.UserDefinedDesc;
                UserEntitlement.EndPointSystemType = EndpointType.IDAS;
                UserEntitlement.CanRequest = role.CanRequest;
                UserEntitlement.EntitlementType = EntitlementType.Role;

                UserEntitlement.UserDefinedDisplayName = role.UserDefinedDisplayName;
                UserEntitlement.Level1Approver = role.Level1Approver;
                UserEntitlement.Level2Approver = role.Level2Approver;
                UserEntitlement.IsLevel1Group = role.IsLevel1Group;
                UserEntitlement.IsLevel2Group = role.IsLevel2Group;
            }
            return UserEntitlement;
        }

        public override List<PresentationUserEntitlement> GetEntitlementDetail(long endpointUserId)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            User IDASUserInfo = new User();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                IDASUserInfo = User.GetByIdIncludeRoles(endpointUserId);
                if (IDASUserInfo == null)
                    return presentationUserEntitlementList;

                foreach (Role role in IDASUserInfo.Roles)
                {
                    PresentationADSUserEntitlement UserEntitlement = new PresentationADSUserEntitlement();
                    UserEntitlement.EntitlementId = IDASUserInfo.Id;
                    UserEntitlement.UserName = IDASUserInfo.UserName;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Role);
                    EndPoint idasEp = EndPoint.GetById(IDASUserInfo.EndPointSystemId, true);
                    UserEntitlement.EndPointSystemType = idasEp.EndPointSystemType.EndpointSystemTypeName;
                    UserEntitlement.EndpointSystemName = idasEp.DisplayName;
                    UserEntitlement.EntitlementType = EntitlementType.Group;
                    if (role != null)
                    {
                        UserEntitlement.Name = role.RoleName;
                        UserEntitlement.Description = role.RoleDescription;

                        UserEntitlement.UserDefinedDisplayName = role.UserDefinedDisplayName;
                        UserEntitlement.Level1Approver = role.Level1Approver;
                        UserEntitlement.Level2Approver = role.Level2Approver;
                        UserEntitlement.IsLevel1Group = role.IsLevel1Group;
                        UserEntitlement.IsLevel2Group = role.IsLevel2Group;

                        presentationUserEntitlementList.Add(UserEntitlement);
                    }
                }
            }
            return presentationUserEntitlementList;
        }

        public override List<PresentationUserEntitlement> GetEntitlementDetailByApplicationType(long endpointSystemId)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var Entitlement = Role.GetAll();
                Entitlement.ForEach(lq =>
                {
                    PresentationIdasUserEntitlement UserEntitlement = new PresentationIdasUserEntitlement();
                    //UserEntitlement.EntitlementId = lq.Id;
                    //UserEntitlement.Category = Convert.ToString(EntitlementType.Role);
                    //UserEntitlement.Name = lq.RoleName;
                    //UserEntitlement.Description = lq.RoleDescription;
                    //UserEntitlement.CanRequest = lq.CanRequest;
                    //UserEntitlement.UserDefinedDesc = lq.UserDefinedDesc;
                    //UserEntitlement.EndpointSystemId = endpointSystemId;

                    //UserEntitlement.UserDefinedDisplayName = lq.UserDefinedDisplayName;
                    //UserEntitlement.Level1Approver = lq.Level1Approver;
                    //UserEntitlement.Level2Approver = lq.Level2Approver;
                    //UserEntitlement.IsLevel1Group = lq.IsLevel1Group;
                    //UserEntitlement.IsLevel2Group = lq.IsLevel2Group;

                    //presentationUserEntitlementList.Add(UserEntitlement);

                    UserEntitlement.EntitlementId = lq.Id;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Role);
                    UserEntitlement.Name = lq.RoleName;
                    UserEntitlement.Description = lq.RoleDescription;
                    UserEntitlement.CanRequest = lq.CanRequest;
                    UserEntitlement.EndpointSystemId = endpointSystemId;
                    UserEntitlement.UserDefinedDisplayName = lq.UserDefinedDisplayName;
                    UserEntitlement.Level1Approver = lq.Level1Approver;
                    UserEntitlement.Level2Approver = lq.Level2Approver;
                    UserEntitlement.IsLevel1Group = lq.IsLevel1Group;
                    UserEntitlement.IsLevel2Group = lq.IsLevel2Group;
                    presentationUserEntitlementList.Add(UserEntitlement);
                });
            }
            return presentationUserEntitlementList;
        }



        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId)
        {
            return new List<PresentationUserEntitlement>();
        }

        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetail(string entitlementCode, long endpointId)
        {
            throw new NotImplementedException();
        }

        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId, string entitlementType, string userAccountName)
        {
            throw new NotImplementedException();
        }

        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(long endpointId, string entityCode, EntitlementType fromValue, EntitlementType toValue)
        {
            throw new NotImplementedException();
        }

        public override void UpdateEntitlement(PresentationUserEntitlement presentationUserEntitlement, IEndpointEntitlementInfo entitlement, string currentUser)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                Role role = (Role)entitlement;
                role.UserDefinedDesc = presentationUserEntitlement.UserDefinedDesc;
                role.CanRequest = presentationUserEntitlement.CanRequest;

                role.UserDefinedDisplayName = presentationUserEntitlement.UserDefinedDisplayName;
                role.Level1Approver = presentationUserEntitlement.Level1Approver;
                role.Level2Approver = presentationUserEntitlement.Level2Approver;
                role.IsLevel1Group = presentationUserEntitlement.IsLevel1Group;
                role.IsLevel2Group = presentationUserEntitlement.IsLevel2Group;

                role.Save(dataAccessTransaction);
                dataAccessTransaction.Commit();
            }
        }
    }
}
