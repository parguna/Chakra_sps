using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class PresentationSAPHANAUserEntitlement : Presentation.PresentationUserEntitlement
    {

        public override List<PresentationUserEntitlement> GetAllEntitlementDetailByType(long endpointSystemId, EntitlementType entitlementType)
        {
            switch (entitlementType)
            {
                case EntitlementType.Role:
                    List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
                    var sapGroups = SAPHANARoleInfo.GetByEndpointSystem(endpointSystemId).Where(a => a.EntitlementType == entitlementType).ToList();
                    sapGroups.ForEach(lq =>
                    {
                        PresentationADSUserEntitlement UserEntitlement = new PresentationADSUserEntitlement();
                        UserEntitlement.EntitlementId = lq.Id;
                        UserEntitlement.Category = Convert.ToString(EntitlementType.Role);
                        UserEntitlement.Name = lq.ROLE_CODE;
                        UserEntitlement.Description = lq.ROLE_DESC;
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

        public override PresentationUserEntitlement GetEntitlement(IEndpointEntitlementInfo entitlement, string entitlementType)
        {
            SAPHANARoleInfo sapRoleInfo = (SAPHANARoleInfo)entitlement;
            PresentationSAPHANAUserEntitlement UserEntitlement = new PresentationSAPHANAUserEntitlement();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                UserEntitlement.EntitlementId = sapRoleInfo.Id;
                UserEntitlement.Category = Convert.ToString(EntitlementType.Role);
                UserEntitlement.Name = sapRoleInfo.ROLE_CODE;
                UserEntitlement.Description = sapRoleInfo.ROLE_DESC;
                UserEntitlement.UserDefinedDesc = sapRoleInfo.UserDefinedDesc;
                UserEntitlement.EndPointSystemType = EndpointType.SAPHANA;
                UserEntitlement.EntitlementType = EntitlementType.Role;
                UserEntitlement.CanRequest = sapRoleInfo.CanRequest;
            }
            return UserEntitlement;
        }
        public override void UpdateEntitlement(PresentationUserEntitlement presentationEntitlement, IEndpointEntitlementInfo entitlement, string currentUser)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                SAPHANARoleInfo sapGroupInfo = (SAPHANARoleInfo)entitlement;
                sapGroupInfo.UserDefinedDesc = presentationEntitlement.UserDefinedDesc;
                sapGroupInfo.CanRequest = presentationEntitlement.CanRequest;
                sapGroupInfo.Save(dataAccessTransaction);
                dataAccessTransaction.Commit();
            }
        }
        public override List<PresentationUserEntitlement> GetEntitlementDetail(long entitlementId)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            SAPHANAUserinfo SAPUserinfo = new SAPHANAUserinfo();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                SAPUserinfo = SAPHANAUserinfo.GetByIdWithNavigation(entitlementId);
                if (SAPUserinfo == null)
                    return presentationUserEntitlementList;
                
                foreach (var role in SAPUserinfo.SAPRoleInfo)
                {
                    PresentationSAPHANAUserEntitlement UserEntitlement = new PresentationSAPHANAUserEntitlement();
                    UserEntitlement.EntitlementId = SAPUserinfo.Id;
                    UserEntitlement.UserName = SAPUserinfo.AccountName;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Role);
                    UserEntitlement.EndPointSystemType = SAPUserinfo.EndPointSystem.EndPointSystemType.EndpointSystemTypeName;
                    UserEntitlement.EndpointSystemName = SAPUserinfo.EndPointSystem.DisplayName;
                    UserEntitlement.EndpointSystemId = SAPUserinfo.EndPointSystemId;
                    UserEntitlement.EntitlementType = EntitlementType.Role;
                    if (role != null)
                    {
                        UserEntitlement.Name = role.ROLE_CODE;
                        UserEntitlement.Description = role.ROLE_DESC;
                        presentationUserEntitlementList.Add(UserEntitlement);
                    }
                }
            }
            return presentationUserEntitlementList;
        }
        public override List<PresentationUserEntitlement> GetEntitlementDetailByApplicationType(long endpointsystemId)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var Entitlement = SAPHANARoleInfo.GetByEndpointSystem(endpointsystemId);
                Entitlement.ForEach(lq =>
                {
                    PresentationSAPHANAUserEntitlement UserEntitlement = new PresentationSAPHANAUserEntitlement();
                    UserEntitlement.EntitlementId = lq.Id;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Role);
                    UserEntitlement.Name = lq.ROLE_CODE;
                    UserEntitlement.Description = lq.ROLE_DESC == "" ? string.Empty : lq.ROLE_DESC;
                    UserEntitlement.CanRequest = lq.CanRequest;
                    UserEntitlement.UserDefinedDesc = lq.UserDefinedDesc == "" ? string.Empty : lq.UserDefinedDesc;
                    UserEntitlement.EndpointSystemId = endpointsystemId;
                    UserEntitlement.EntitlementType = EntitlementType.Role;
                    presentationUserEntitlementList.Add(UserEntitlement);
                });
            }
            return presentationUserEntitlementList;
        }

        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long endpointUserId)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            SAPHANAUserinfo userInfo = SAPHANAUserinfo.GetByIdIncludeSAPRoleInfo(endpointUserId);

            if (userInfo != null)
            {
                EndPoint ep = EndPoint.GetByIdIncludingEndPointSystemType(userInfo.EndPointSystemId);
                if (userInfo.SAPRoleInfo != null)
                {
                    List<SAPHANATCodeInfo> tCodes = null;
                    List<PresentationSAPHANARoleToAuthorizationReport> auths = null;
                    PresentationSAPHANAUserEntitlement entitlement = null;
                    // loop all roles
                    foreach (var role in userInfo.SAPRoleInfo)
                    {
                        // create role presentation object
                        entitlement = new PresentationSAPHANAUserEntitlement();
                        entitlement.ParentEntitlementId = endpointUserId;
                        entitlement.EntitlementId = role.Id;
                        entitlement.UserName = role.ROLE_CODE;
                        entitlement.Description = role.ROLE_DESC;
                        entitlement.Category = role.EntitlementType.ToString();
                        entitlement.EndPointSystemType = EndpointType.SAPHANA;
                        entitlement.EndpointSystemName = ep.DisplayName;
                        entitlement.EndpointSystemId = role.EndPointSystemId;
                        entitlement.EntitlementType = role.EntitlementType;
                        presentationUserEntitlementList.Add(entitlement);

                        // get tcode for role
                        tCodes = SAPHANATCodeInfo.GetByRoleCode(role.ROLE_CODE, userInfo.EndPointSystemId);
                        foreach (var tcode in tCodes)
                        {
                            // create tcde presenatation object
                            entitlement = new PresentationSAPHANAUserEntitlement();
                            entitlement.ParentEntitlementId = role.Id;
                            entitlement.EntitlementId = tcode.Id;
                            entitlement.UserName = tcode.TCODE_Code;
                            entitlement.Description = tcode.TCODE_Desc;
                            entitlement.Category = EntitlementType.TCodes.ToString();
                            entitlement.EndPointSystemType = EndpointType.SAPHANA;
                            entitlement.EndpointSystemName = ep.DisplayName;
                            entitlement.EndpointSystemId = tcode.EndPointSystemId;
                            entitlement.EntitlementType = EntitlementType.TCodes;
                            presentationUserEntitlementList.Add(entitlement);
                        }

                        // get auth for role
                        auths = PresentationSAPHANARoleToAuthorizationReport.GetSapRolesToAuthorizationData(role.ROLE_CODE, userInfo.EndPointSystemId);
                        foreach (var auth in auths)
                        {
                            // create auth presenatation object
                            entitlement = new PresentationSAPHANAUserEntitlement();
                            entitlement.ParentEntitlementId = role.Id;
                            entitlement.EntitlementId = auth.SAPRoleToAuthMapId;
                            entitlement.UserName = auth.AuthCode;
                            entitlement.Description = auth.ObjectCode;
                            entitlement.Category = "Authorization";
                            entitlement.EndPointSystemType = EndpointType.SAPHANA;
                            entitlement.EndpointSystemName = ep.DisplayName;
                            entitlement.EndpointSystemId = userInfo.EndPointSystemId;
                            entitlement.EntitlementType = EntitlementType.TCodes;
                            presentationUserEntitlementList.Add(entitlement);
                        }
                    }

                }
            }

            return presentationUserEntitlementList;
        }
        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId, string entitlementType, string userAccountName)
        {
            return new List<PresentationUserEntitlement>();
        }

        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetail(string entitlementCode, long endpointId)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            List<SAPHANARoleToAuthMap> sapRoleToAuthMap = new List<SAPHANARoleToAuthMap>();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var sapRoleInfo = SAPHANARoleInfo.GetByRoleCode(entitlementCode, endpointId);
                sapRoleToAuthMap = SAPHANARoleToAuthMap.GetRolesByRoleName(entitlementCode, endpointId).ToList();

                if (sapRoleToAuthMap == null)
                    return presentationUserEntitlementList;

                foreach (SAPHANARoleToAuthMap grp in sapRoleToAuthMap)
                {
                    PresentationSAPHANAUserEntitlement UserEntitlement = new PresentationSAPHANAUserEntitlement();
                    UserEntitlement.ParentEntitlementId = sapRoleInfo.Id;
                    UserEntitlement.EntitlementId = grp.Id;
                    UserEntitlement.UserName = grp.FromValue;
                    UserEntitlement.Description = grp.ObjectCode;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.TCodes);
                    UserEntitlement.EndPointSystemType = EndpointType.SAPHANA;
                    UserEntitlement.EndpointSystemName = "";
                    UserEntitlement.EndpointSystemId = endpointId;
                    presentationUserEntitlementList.Add(UserEntitlement);
                }
            }
            return presentationUserEntitlementList;
        }

        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(long endpointId, string entityCode, EntitlementType fromValue, EntitlementType toValue)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();

            if (fromValue == EntitlementType.Role && toValue == EntitlementType.TCodes)
            {
                presentationUserEntitlementList = GetTCodesForRole(Convert.ToInt64(entityCode), endpointId);
            }
            else if (fromValue == EntitlementType.Role && toValue == EntitlementType.Authorization)
            {
                presentationUserEntitlementList = GetAuthForRole(Convert.ToInt64(entityCode), endpointId);
            }

            return presentationUserEntitlementList;
        }

        public List<PresentationUserEntitlement> GetTCodesForRole(long roleId, long endpointSystemId)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();

            SAPHANARoleInfo role = SAPHANARoleInfo.GetById(roleId);

            if (role != null)
            {
                var tCodes = SAPHANARoleTCodeMapping.GetByRoleCodeAndEndpointId(role.ROLE_CODE, endpointSystemId);
                EndPoint ep = EndPoint.GetByIdIncludingEndPointSystemType(endpointSystemId);
                //TODO:Siva
                //var tCodes = SAPTCodeInfo.GetByRoleCode(role.ROLE_CODE, endpointSystemId);
                foreach (var tcode in tCodes)
                {
                    // create tcde presenatation object
                    var entitlement = new PresentationSAPHANAUserEntitlement();
                    entitlement.ParentEntitlementId = role.Id;
                    entitlement.EntitlementId = tcode.Id;
                    entitlement.Name = tcode.TCode;
                    entitlement.Description = tcode.TCode;
                    entitlement.Category = EntitlementType.TCodes.ToString();
                    entitlement.EndPointSystemType = EndpointType.SAPHANA;
                    entitlement.EndpointSystemName = ep.DisplayName;
                    entitlement.EndpointSystemId = tcode.EndPointSystemId;
                    entitlement.EntitlementType = EntitlementType.TCodes;
                    presentationUserEntitlementList.Add(entitlement);
                }

            }

            return presentationUserEntitlementList;
        }

        public List<PresentationUserEntitlement> GetAuthForRole(long roleId, long endpointSystemId)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();

            var role = SAPHANARoleInfo.GetById(roleId);

            if (role != null)
            {
                EndPoint ep = EndPoint.GetByIdIncludingEndPointSystemType(endpointSystemId);

                var auths = PresentationSAPHANARoleToAuthorizationReport.GetSapRolesToAuthorizationData(role.ROLE_CODE, endpointSystemId);
                foreach (var auth in auths)
                {
                    // create auth presenatation object
                    var entitlement = new PresentationSAPHANAUserEntitlement();
                    entitlement.ParentEntitlementId = role.Id;
                    entitlement.EntitlementId = auth.SAPRoleToAuthMapId;
                    entitlement.UserName = auth.AuthCode;
                    entitlement.Name = auth.AuthCode;
                    entitlement.Description = auth.ObjectCode;
                    entitlement.Category = EntitlementType.Authorization.ToString();
                    entitlement.EndPointSystemType = EndpointType.SAPHANA;
                    entitlement.EndpointSystemName = ep.DisplayName;
                    entitlement.EndpointSystemId = endpointSystemId;
                    entitlement.EntitlementType = EntitlementType.Authorization;
                    entitlement.UserDefinedDesc = auth.FieldCode;
                    presentationUserEntitlementList.Add(entitlement);
                }

            }

            return presentationUserEntitlementList;
        }

        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(long endpointId, long entityId, EntitlementType fromValue, EntitlementType toValue)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            try
            {
                if (fromValue == EntitlementType.Role && toValue == EntitlementType.TCodes)
                {
                    presentationUserEntitlementList = GetTCodesForRole(entityId, endpointId);
                }
                if (fromValue == EntitlementType.Account && toValue == EntitlementType.BusinessRole)
                {
                    //TODO:Business Role
                    //presentationUserEntitlementList = GetTCodesForRole(entityId, endpointId);
                }
                if (fromValue == EntitlementType.BusinessRole && toValue == EntitlementType.Role)
                {
                    //presentationUserEntitlementList = GetTCodesForRole(entityId, endpointId);
                }
            }
            catch (Exception ex)
            {
            }
            return presentationUserEntitlementList;
        }

        public override List<PresentationUserEntitlement> GetEntitlementDetailByApplicationTypeWithAllCanRequest(long endpointSystemId)
        {
            throw new NotImplementedException();
        }

    }
}
