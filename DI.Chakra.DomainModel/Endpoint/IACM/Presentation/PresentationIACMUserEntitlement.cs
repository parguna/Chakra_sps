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
    public class PresentationIACMUserEntitlement : Presentation.PresentationUserEntitlement
    {
        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(long endpointId, long entityId, EntitlementType fromValue, EntitlementType toValue)
        {
            List<PresentationUserEntitlement> entitlementList = new List<Presentation.PresentationUserEntitlement>();

            if (fromValue == EntitlementType.Role && toValue == EntitlementType.AccessLevel)
            {
                var role = IACMRoleInfo.GetById(entityId);
                // get all functions for permission
                if (role != null)
                {
                    var functions = DomainModel.Presentation.PresentationRoleAccess.GetAccessToRoles(role.Name);
                    var endpoint = EndPoint.GetByIdIncludingEndPointSystemType(endpointId);

                    foreach (var function in functions)
                    {
                        var fun = new PresentationIACMUserEntitlement
                        {
                            EndpointSystemId = endpointId,
                            EndpointSystemName = endpoint.DisplayName,
                            EndPointSystemType = endpoint.EndPointSystemType.EndpointSystemTypeName,
                            EntitlementId = function.AccessId,
                            EntitlementType = toValue,
                            Name = function.PageName,
                            UserName = function.PageName,
                            Description = function.AccessRights,
                            ParentEntitlementId = entityId,
                            Category = toValue.ToString(),
                        };

                        if (!entitlementList.Any(x => x.EntitlementId == function.AccessId))
                            entitlementList.Add(fun);
                    }
                }
            }

            return entitlementList;
        }
        public override List<PresentationUserEntitlement> GetAllEntitlementDetailByType(long endpointSystemId, EntitlementType entitlementType)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            switch (entitlementType)
            {                
                case EntitlementType.Group:                    
                    var adsGroups = IACMGroupInfo.GetAllByEndpointSystemId(endpointSystemId).Where(a => a.EntitlementType == entitlementType).ToList();
                    adsGroups.ForEach(lq =>
                    {
                        PresentationADSUserEntitlement UserEntitlement = new PresentationADSUserEntitlement();
                        UserEntitlement.EntitlementId = lq.Id;
                        UserEntitlement.Category = Convert.ToString(EntitlementType.Group);
                        UserEntitlement.Name = lq.Name;
                        UserEntitlement.Description = lq.Description;
                        UserEntitlement.CanRequest = lq.CanRequest;
                        UserEntitlement.UserDefinedDesc = lq.UserDefinedDesc;
                        UserEntitlement.EndpointSystemId = endpointSystemId;
                        UserEntitlement.EntitlementType = EntitlementType.Group;
                        UserEntitlement.UserDefinedDisplayName = lq.UserDefinedDisplayName;
                        UserEntitlement.Level1Approver = lq.Level1Approver;
                        UserEntitlement.Level2Approver = lq.Level2Approver;
                        UserEntitlement.IsLevel1Group = lq.IsLevel1Group;
                        UserEntitlement.IsLevel2Group = lq.IsLevel2Group;

                        presentationUserEntitlementList.Add(UserEntitlement);
                    });
                    return presentationUserEntitlementList;
                case EntitlementType.Role:
                    var roleInfo = IACMRoleInfo.GetAll().Where(a => a.EntitlementType == entitlementType).ToList();
                    roleInfo.ForEach(lq =>
                    {
                        PresentationADSUserEntitlement UserEntitlement = new PresentationADSUserEntitlement();
                        UserEntitlement.EntitlementId = lq.Id;
                        UserEntitlement.Category = Convert.ToString(EntitlementType.Role);
                        UserEntitlement.Name = lq.Name;
                        UserEntitlement.Description = lq.Description;
                        UserEntitlement.CanRequest = lq.CanRequest;
                        UserEntitlement.UserDefinedDesc = lq.UserDefinedDesc;
                        UserEntitlement.EndpointSystemId = endpointSystemId;
                        UserEntitlement.EntitlementType = EntitlementType.Role;
                        UserEntitlement.UserDefinedDisplayName = lq.UserDefinedDisplayName;
                        UserEntitlement.Level1Approver = lq.Level1Approver;
                        UserEntitlement.Level2Approver = lq.Level2Approver;
                        UserEntitlement.IsLevel1Group = lq.IsLevel1Group;
                        UserEntitlement.IsLevel2Group = lq.IsLevel2Group;

                        presentationUserEntitlementList.Add(UserEntitlement);
                    });
                    return presentationUserEntitlementList;
                default:
                    return new List<PresentationUserEntitlement>();
            }
        }

        public override PresentationUserEntitlement GetEntitlement(IEndpointEntitlementInfo entitlement, string entitlementType)
        {
            PresentationIACMUserEntitlement UserEntitlement = new PresentationIACMUserEntitlement();
            if (entitlementType == Infrastructure.Enums.EntitlementType.Role.ToString())
            {
                IACMRoleInfo role = (IACMRoleInfo)entitlement;                
                using (var dataAccessTransaction = new DataAccessTransaction())
                {
                    UserEntitlement.EntitlementId = role.Id;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Role);
                    UserEntitlement.Name = role.Name;
                    UserEntitlement.Description = role.Description;
                    UserEntitlement.UserDefinedDesc = role.UserDefinedDesc;
                    UserEntitlement.EndPointSystemType = EndpointType.IDMS;
                    UserEntitlement.CanRequest = role.CanRequest;
                    UserEntitlement.EntitlementType = EntitlementType.Role;

                    UserEntitlement.UserDefinedDisplayName = role.UserDefinedDisplayName;
                    UserEntitlement.Level1Approver = role.Level1Approver;
                    UserEntitlement.Level2Approver = role.Level2Approver;
                    UserEntitlement.IsLevel1Group = role.IsLevel1Group;
                    UserEntitlement.IsLevel2Group = role.IsLevel2Group;
                }
            }
            else if (entitlementType == Infrastructure.Enums.EntitlementType.Group.ToString())
            {
                IACMGroupInfo grp = (IACMGroupInfo)entitlement;
                using (var dataAccessTransaction = new DataAccessTransaction())
                {
                    UserEntitlement.EntitlementId = grp.Id;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Group);
                    UserEntitlement.Name = grp.Name;
                    UserEntitlement.Description = grp.Description;
                    UserEntitlement.UserDefinedDesc = grp.UserDefinedDesc;
                    UserEntitlement.EndPointSystemType = EndpointType.IDMS;
                    UserEntitlement.CanRequest = grp.CanRequest;
                    UserEntitlement.EntitlementType = EntitlementType.Group;

                    UserEntitlement.UserDefinedDisplayName = grp.UserDefinedDisplayName;
                    UserEntitlement.Level1Approver = grp.Level1Approver;
                    UserEntitlement.Level2Approver = grp.Level2Approver;
                    UserEntitlement.IsLevel1Group = grp.IsLevel1Group;
                    UserEntitlement.IsLevel2Group = grp.IsLevel2Group;
                }
            }

            return UserEntitlement;
        }

        public override List<PresentationUserEntitlement> GetEntitlementDetail(long endpointUserId)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            IACMUserInfo IDASUserInfo = new IACMUserInfo();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                IDASUserInfo = IACMUserInfo.GetByIdIncludeRoles(endpointUserId);
                if (IDASUserInfo == null)
                    return presentationUserEntitlementList;

                EndPoint idasEp = EndPoint.GetById(IDASUserInfo.EndPointSystemId, true);

                foreach (IACMRoleInfo role in IDASUserInfo.Roles)
                {
                    PresentationADSUserEntitlement UserEntitlement = new PresentationADSUserEntitlement();
                    UserEntitlement.EntitlementId = IDASUserInfo.Id;
                    UserEntitlement.UserName = IDASUserInfo.UserName;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Role); 
                    UserEntitlement.EndPointSystemType = idasEp.EndPointSystemType.EndpointSystemTypeName;
                    UserEntitlement.EndpointSystemName = idasEp.DisplayName;
                    UserEntitlement.EntitlementType = EntitlementType.Role;
                    if (role != null)
                    {
                        UserEntitlement.Name = role.Name;
                        UserEntitlement.Description = role.Description;

                        UserEntitlement.UserDefinedDisplayName = role.UserDefinedDisplayName;
                        UserEntitlement.Level1Approver = role.Level1Approver;
                        UserEntitlement.Level2Approver = role.Level2Approver;
                        UserEntitlement.IsLevel1Group = role.IsLevel1Group;
                        UserEntitlement.IsLevel2Group = role.IsLevel2Group;

                        presentationUserEntitlementList.Add(UserEntitlement);
                    }
                }

                foreach (IACMGroupInfo grp in IDASUserInfo.Groups)
                {
                    PresentationADSUserEntitlement UserEntitlement = new PresentationADSUserEntitlement();
                    UserEntitlement.EntitlementId = IDASUserInfo.Id;
                    UserEntitlement.UserName = IDASUserInfo.UserName;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Group); 
                    UserEntitlement.EndPointSystemType = idasEp.EndPointSystemType.EndpointSystemTypeName;
                    UserEntitlement.EndpointSystemName = idasEp.DisplayName;
                    UserEntitlement.EntitlementType = EntitlementType.Group;
                    if (grp != null)
                    {
                        UserEntitlement.Name = grp.Name;
                        UserEntitlement.Description = grp.Description;

                        UserEntitlement.UserDefinedDisplayName = grp.UserDefinedDisplayName;
                        UserEntitlement.Level1Approver = grp.Level1Approver;
                        UserEntitlement.Level2Approver = grp.Level2Approver;
                        UserEntitlement.IsLevel1Group = grp.IsLevel1Group;
                        UserEntitlement.IsLevel2Group = grp.IsLevel2Group;

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
                var Entitlement = IACMRoleInfo.GetAll().Where(a => a.CanRequest == true).ToList();
                Entitlement.ForEach(lq =>
                {
                    PresentationIACMUserEntitlement UserEntitlement = new PresentationIACMUserEntitlement();
                    UserEntitlement.EntitlementId = lq.Id;
                    UserEntitlement.EntitlementType = EntitlementType.Role;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Role);
                    UserEntitlement.Name = lq.Name;
                    UserEntitlement.Description = lq.Description;
                    UserEntitlement.CanRequest = lq.CanRequest;
                    UserEntitlement.EndpointSystemId = endpointSystemId;
                    UserEntitlement.UserDefinedDesc = lq.UserDefinedDesc;
                    UserEntitlement.UserDefinedDisplayName = lq.UserDefinedDisplayName;
                    UserEntitlement.Level1Approver = lq.Level1Approver;
                    UserEntitlement.Level2Approver = lq.Level2Approver;
                    UserEntitlement.IsLevel1Group = lq.IsLevel1Group;
                    UserEntitlement.IsLevel2Group = lq.IsLevel2Group;
                    presentationUserEntitlementList.Add(UserEntitlement);
                });

                var group = IACMGroupInfo.GetAll();
                group.ForEach(lg =>
                {
                    PresentationIACMUserEntitlement UserEntitlement = new PresentationIACMUserEntitlement();
                    UserEntitlement.EntitlementId = lg.Id;
                    UserEntitlement.EntitlementType = EntitlementType.Group;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Group);
                    UserEntitlement.Name = lg.Name;
                    UserEntitlement.Description = lg.Description;
                    UserEntitlement.CanRequest = lg.CanRequest;
                    UserEntitlement.EndpointSystemId = endpointSystemId;
                    UserEntitlement.UserDefinedDesc = lg.UserDefinedDesc;
                    UserEntitlement.UserDefinedDisplayName = lg.UserDefinedDisplayName;
                    UserEntitlement.Level1Approver = lg.Level1Approver;
                    UserEntitlement.Level2Approver = lg.Level2Approver;
                    UserEntitlement.IsLevel1Group = lg.IsLevel1Group;
                    UserEntitlement.IsLevel2Group = lg.IsLevel2Group;
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
            if (fromValue == EntitlementType.Role)
            {
                var role = IACMRoleInfo.GetByRoleName(entityCode);
                if (role != null)
                {
                    return GetEntitlementHierachyDetailByLevel(endpointId, role.Id, fromValue, toValue);
                }
            }

            return new List<PresentationUserEntitlement>();
        }

        public override void UpdateEntitlement(PresentationUserEntitlement presentationUserEntitlement, IEndpointEntitlementInfo entitlement, string currentUser)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                if (presentationUserEntitlement.EntitlementType == EntitlementType.Role)
                {
                    IACMRoleInfo role = (IACMRoleInfo)entitlement;
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
                else if(presentationUserEntitlement.EntitlementType == EntitlementType.Group)
                {
                    IACMGroupInfo grp = (IACMGroupInfo)entitlement;
                    grp.UserDefinedDesc = presentationUserEntitlement.UserDefinedDesc;
                    grp.CanRequest = presentationUserEntitlement.CanRequest;

                    grp.UserDefinedDisplayName = presentationUserEntitlement.UserDefinedDisplayName;
                    grp.Level1Approver = presentationUserEntitlement.Level1Approver;
                    grp.Level2Approver = presentationUserEntitlement.Level2Approver;
                    grp.IsLevel1Group = presentationUserEntitlement.IsLevel1Group;
                    grp.IsLevel2Group = presentationUserEntitlement.IsLevel2Group;

                    grp.Save(dataAccessTransaction);
                    dataAccessTransaction.Commit();
                }
            }
        }

        public override List<PresentationUserEntitlement> GetEntitlementDetailByApplicationTypeWithAllCanRequest(long endpointSystemId)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var Entitlement = IACMRoleInfo.GetAll().ToList();
                Entitlement.ForEach(lq =>
                {
                    PresentationIACMUserEntitlement UserEntitlement = new PresentationIACMUserEntitlement();
                    UserEntitlement.EntitlementId = lq.Id;
                    UserEntitlement.EntitlementType = EntitlementType.Role;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Role);
                    UserEntitlement.Name = lq.Name;
                    UserEntitlement.Description = lq.Description;
                    UserEntitlement.CanRequest = lq.CanRequest;
                    UserEntitlement.EndpointSystemId = endpointSystemId;
                    UserEntitlement.UserDefinedDesc = lq.UserDefinedDesc;
                    UserEntitlement.UserDefinedDisplayName = lq.UserDefinedDisplayName;
                    UserEntitlement.Level1Approver = lq.Level1Approver;
                    UserEntitlement.Level2Approver = lq.Level2Approver;
                    UserEntitlement.IsLevel1Group = lq.IsLevel1Group;
                    UserEntitlement.IsLevel2Group = lq.IsLevel2Group;
                    presentationUserEntitlementList.Add(UserEntitlement);
                });

                var group = IACMGroupInfo.GetAll();
                group.ForEach(lg =>
                {
                    PresentationIACMUserEntitlement UserEntitlement = new PresentationIACMUserEntitlement();
                    UserEntitlement.EntitlementId = lg.Id;
                    UserEntitlement.EntitlementType = EntitlementType.Group;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Group);
                    UserEntitlement.Name = lg.Name;
                    UserEntitlement.Description = lg.Description;
                    UserEntitlement.CanRequest = lg.CanRequest;
                    UserEntitlement.EndpointSystemId = endpointSystemId;
                    UserEntitlement.UserDefinedDesc = lg.UserDefinedDesc;
                    UserEntitlement.UserDefinedDisplayName = lg.UserDefinedDisplayName;
                    UserEntitlement.Level1Approver = lg.Level1Approver;
                    UserEntitlement.Level2Approver = lg.Level2Approver;
                    UserEntitlement.IsLevel1Group = lg.IsLevel1Group;
                    UserEntitlement.IsLevel2Group = lg.IsLevel2Group;
                    presentationUserEntitlementList.Add(UserEntitlement);
                });
            }
            return presentationUserEntitlementList;
        }
    }
}
