using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Infrastructure;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;


namespace DI.Chakra.DomainModel
{
    public class SAPHANAEndPoint : IEndPointFactory
    {
        public List<PresentationUserEntitlement> GetAllEntitlementByType(EntitlementType type, long endpointsystemid)
        {
            return new PresentationSAPHANAUserEntitlement().GetAllEntitlementDetailByType(endpointsystemid, type);
        }
        public IEndpointUserInfo GetUser()
        {
            return new SAPHANAUserinfo();
        }
        public IEndpointUserInfo GetUser(long id)
        {
            return SAPHANAUserinfo.GetById(id, true);
        }

        public IEndpointEntitlementInfo GetEntitlement()
        {
            return new SAPHANARoleInfo();
        }
        public List<EntitlementType> GetEntitlementList()
        {
            List<EntitlementType> entitlementTypes = new List<EntitlementType>();
            entitlementTypes.Add(EntitlementType.Role);
            entitlementTypes.Add(EntitlementType.Group);
            entitlementTypes.Add(EntitlementType.Position);
            return entitlementTypes;
        }
        public IEndpointEntitlementInfo GetEntitlement(long id)
        {
            return SAPHANARoleInfo.GetById(id);
        }
        public IEndpointEntitlementInfo GetEntitlement(long systemId, string entitlementName, EntitlementType entitlementType)
        {
            if (entitlementType == EntitlementType.Role)
            {
                return SAPHANARoleInfo.GetAll().Where(t => t.EndPointSystemId == systemId &&
                t.EntitlementType == entitlementType && t.EntitlementName.ToUpper() == entitlementName.ToUpper()).FirstOrDefault();
            }
            else if (entitlementType == EntitlementType.Group)
            {
                return SAPHANAGroupInfo.GetAll().Where(t => t.EndPointSystemId == systemId &&
               t.EntitlementType == entitlementType && t.EntitlementName.ToUpper() == entitlementName.ToUpper()).FirstOrDefault();
            }
            else if (entitlementType == EntitlementType.Profile)
            {
                return SAPHANAProfileInfo.GetAll().Where(t => t.EndPointSystemId == systemId &&
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
                case (EntitlementType.Role):
                    return SAPHANARoleInfo.GetById(id);
                default:
                    throw new ApplicationException("Entitlement Type : " + type.ToString() + " not supported for SAPHANA");
            }
        }
        
        public PresentationUserEntitlement GetEntitlement(IEndpointEntitlementInfo entitlement, string entitlementType)
        {
            return new PresentationSAPHANAUserEntitlement().GetEntitlement(entitlement, entitlementType);
        }
        public void UpdateEntitlement(PresentationUserEntitlement presentationUserEntitlement, IEndpointEntitlementInfo entitlement, string currentUser)
        {
            new PresentationSAPHANAUserEntitlement().UpdateEntitlement(presentationUserEntitlement, entitlement, currentUser);
        }
        public List<PresentationUserEntitlement> GetEntitlementDetail(long entitlementId)
        {
            return new PresentationSAPHANAUserEntitlement().GetEntitlementDetail(entitlementId);
        }

        public List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId)
        {
            return new PresentationSAPHANAUserEntitlement().GetEntitlementHierachyDetail(entitlementId);
        }

        public List<PresentationUserEntitlement> GetEntitlementDetailByApplicationType(long endpointsystemId)
        {
            return new PresentationSAPHANAUserEntitlement().GetEntitlementDetailByApplicationType(endpointsystemId);
        }

        public List<Group> GetAllGroupsByEndpointID(long endPointID, string searchString)
        {
            return SAPHANAGroupInfo.GetAllGroupsByEndpointID(endPointID, searchString);
        }
        public Dictionary<long, string> GetEntitlements(string searchString, long endpointSystemId, long? EndpointUserId, bool existingEntitlements)
        {
            throw new ApplicationException();
        }
        public void UpdatePasswordresetcount(long endpointId, string userName, string currentUser)
        {
            SAPHANAUserinfo.UpdatePasswordresetcount(endpointId, userName, currentUser);
        }
        public List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(EndPoint endpoint, string userAccountName, long entitlementId, string entityCode, EntitlementType fromValue, EntitlementType toValue)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            try
            {
                if (fromValue == EntitlementType.Account && toValue == EntitlementType.BusinessRole)
                {
                    return GetBusinessRole(endpoint, userAccountName, entitlementId, entityCode);
                }
                if (fromValue == EntitlementType.Account && toValue == EntitlementType.IndirectRole)
                {
                    return GetIndirectRole(endpoint, userAccountName, entitlementId);
                }
                if (fromValue == EntitlementType.Position && toValue == EntitlementType.IndirectRole)
                {
                    return GetIndirectRole(endpoint, userAccountName, entitlementId, entityCode);
                }
                else if (fromValue == EntitlementType.BusinessRole && toValue == EntitlementType.TechnicalRole)
                {
                    return GetTechnicalRole(endpoint, userAccountName, entityCode, entitlementId);
                }
                else if ((fromValue == EntitlementType.TechnicalRole && toValue == EntitlementType.TCodes) || (fromValue == EntitlementType.IndirectRole && toValue == EntitlementType.TCodes))
                {
                    presentationUserEntitlementList = GetTCodesForRole(endpoint, entityCode, endpoint);
                }
            }
            catch (Exception ex)
            {
                new DI.Chakra.Logger.AttestationLogger().WriteLog(Logger.LogLevelType.Exception, $"{endpoint.DisplayName}: userAccountName:{userAccountName} entityCode:{entityCode}  fromValue:{fromValue} toValue:{toValue} GetIndirectRole failed.{ex.Message}", ex);
            }
            return presentationUserEntitlementList;
        }

        private List<PresentationUserEntitlement> GetIndirectRole(EndPoint endpoint, string userAccountName, long accountReconId, string positionCode = null)
        {
            var pEntitlement = new List<PresentationUserEntitlement>();

            try
            {
                var applnType = endpoint.ApplicationType;
                {
                    List<EntityModel.Presentation.PresentationBusinessRoleDetail> technicalRoles = null;
                    switch (applnType)
                    {
                        case ApplicationType.EBS:
                        case ApplicationType.MSSL:
                        #region MSSL -EBS
                        if (!string.IsNullOrWhiteSpace(positionCode))
                        {
                            technicalRoles = this.GetSAPHANAIndirectRoleByPosition(endpoint.Id, userAccountName, positionCode);
                        }
                        else
                        {
                            technicalRoles = GetSAPHANAIndirectRole(endpoint.Id, accountReconId);
                        }
                        #endregion
                        break;
                        default:
                            break;
                    }
                    if (technicalRoles != null && technicalRoles.Count > 0)
                    {
                        foreach (var technicalRole in technicalRoles)
                        {
                            if (!string.IsNullOrWhiteSpace(technicalRole.BusinessRole))
                                if (!pEntitlement.Exists(i => i.Name.Equals(technicalRole.BusinessRole, StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    var sapUserEntitlement = new PresentationSAPHANAUserEntitlement();
                                    sapUserEntitlement.EntitlementType = EntitlementType.IndirectRole;
                                    sapUserEntitlement.ParentEntitlementId = accountReconId;
                                    sapUserEntitlement.EntitlementId = technicalRole.Id;
                                    sapUserEntitlement.Name = technicalRole.BusinessRole;
                                    sapUserEntitlement.Description = technicalRole.Description;
                                    sapUserEntitlement.Category = EntitlementType.IndirectRole.ToString();
                                    sapUserEntitlement.EndPointSystemType = EndpointType.SAPHANA;
                                    sapUserEntitlement.EndpointUserId = technicalRole.EndpointUserId;
                                    sapUserEntitlement.EndpointSystemName = endpoint.DisplayName;
                                    sapUserEntitlement.EndpointSystemId = endpoint.Id;
                                    sapUserEntitlement.ValidTo = technicalRole.ValidToDate;
                                    pEntitlement.Add(sapUserEntitlement);
                                }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new DI.Chakra.Logger.AttestationLogger().WriteLog(Logger.LogLevelType.Exception, $"{endpoint.DisplayName}: userAccountName:{userAccountName} GetIndirectRole failed.{ex.Message}", ex);
            }
            return pEntitlement;
        }

        public List<PresentationUserEntitlement> GetTechnicalRole(EndPoint ep, string userLogonName, string businessRole, long keyId)
        {
            var applnType = ep.ApplicationType;
            var pEntitlement = new List<PresentationUserEntitlement>();
            var account = AccountReconcilation.GetByAccountName(userLogonName, ep.Id);
            if (account == null || account.IsDeleted) return pEntitlement;
            if (applnType == ApplicationType.MSSL || applnType == ApplicationType.EBS)
            {
                using (var trans = new DataAccessTransaction(false))
                {
                    var technicalRoles = new SAPHANARoleInfoRepository(trans).GetTechnicalRolesFromBusinessRole(ep.Id, account.EndpointUserId, businessRole, applnType);
                    foreach (var technicalRole in technicalRoles)
                    {
                        if (!pEntitlement.Exists(i => i.Name.Equals(technicalRole.ROLE_CODE, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            var sapUserEntitlement = new PresentationSAPHANAUserEntitlement();
                            sapUserEntitlement.EntitlementType = EntitlementType.BusinessRole;
                            sapUserEntitlement.ParentEntitlementId = keyId;
                            sapUserEntitlement.EntitlementId = technicalRole.SAP_ROLEID;
                            sapUserEntitlement.Name = technicalRole.ROLE_CODE;
                            sapUserEntitlement.Description = technicalRole.ROLE_DESC;
                            sapUserEntitlement.Category = EntitlementType.TechnicalRole.ToString();
                            sapUserEntitlement.EndPointSystemType = EndpointType.SAPHANA;
                            sapUserEntitlement.EndpointUserId = account.EndpointUserId;
                            sapUserEntitlement.EndpointSystemName = ep.DisplayName;
                            sapUserEntitlement.EndpointSystemId = ep.Id;
                            sapUserEntitlement.ValidTo = technicalRole.ValidToDate;
                            pEntitlement.Add(sapUserEntitlement);
                        }
                    }
                }

            }
            return pEntitlement;
        }
        public List<PresentationUserEntitlement> GetBusinessRole(EndPoint ep, string userAccountName, long keyId, string entityCode)
        {
            var applnType = ep.ApplicationType;
            var account = AccountReconcilation.GetById(keyId);
            var pEntitlement = new List<PresentationUserEntitlement>();
            if (account != null)
            {
                if (applnType == ApplicationType.MSSL || applnType == ApplicationType.EBS)                
                {
                    var businessRoles = SAPHANARoleInfo.GetBusinessRoles(ep.Id, account.EndpointUserId, applnType);
                    foreach (var businessRole in businessRoles)
                    {
                        if (!pEntitlement.Exists(i => i.Name.Equals(businessRole.BusinessRole, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            var sapUserEntitlement = new PresentationSAPHANAUserEntitlement();
                            sapUserEntitlement.EntitlementType = EntitlementType.BusinessRole;
                            sapUserEntitlement.ParentEntitlementId = keyId;
                            sapUserEntitlement.EntitlementId = businessRole.SAP_ROLEID;
                            sapUserEntitlement.Name = businessRole.BusinessRole;
                            sapUserEntitlement.Description = businessRole.Description;
                            sapUserEntitlement.Category = EntitlementType.BusinessRole.ToString();
                            sapUserEntitlement.EndPointSystemType = EndpointType.SAPHANA;
                            sapUserEntitlement.EndpointUserId = account.EndpointUserId;
                            sapUserEntitlement.EndpointSystemName = ep.DisplayName;
                            sapUserEntitlement.EndpointSystemId = ep.Id;
                            sapUserEntitlement.ValidTo = businessRole.ValidToDate;
                            sapUserEntitlement.Remarks3 = businessRole.ValidToDate.HasValue ? businessRole.ValidToDate.Value.ToString("dd/MM/yyyy") : null;
                            pEntitlement.Add(sapUserEntitlement);
                        }
                    }
                }
            }
            return pEntitlement;
        }
        public List<PresentationUserEntitlement> GetTCodesForRole(EndPoint endpoint, string roleName, EndPoint dpEndpoint)
        {
            long endpointSystemId = dpEndpoint.Id;
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();

            var role = SAPHANARoleInfo.GetByRoleCode(roleName, endpointSystemId);

            if (role != null)
            {
                var tCodes = PresentationSAPHANATCodeReport.GetTcodeInfo(endpointSystemId, role.ROLE_CODE);
                if (tCodes == null)
                {
                    return presentationUserEntitlementList;
                }
                foreach (var tcode in tCodes)
                {
                    var name = $"{tcode.TCODE_Code}";
                    if (presentationUserEntitlementList.Exists(i => i.Name == name && i.Description == tcode.TCODE_Desc)) continue;
                    var entitlement = new PresentationSAPHANAUserEntitlement();
                    entitlement.ParentEntitlementId = role.Id;
                    entitlement.EntitlementId = tcode.TCODE_Id;
                    entitlement.Name = name;
                    entitlement.Description = tcode.TCODE_Desc;
                    entitlement.Category = EntitlementType.TCodes.ToString();
                    entitlement.EndPointSystemType = EndpointType.SAPHANA;
                    entitlement.EndpointSystemName = endpoint.DisplayName;
                    entitlement.EndpointSystemId = endpointSystemId;
                    entitlement.EntitlementType = EntitlementType.TCodes;
                    presentationUserEntitlementList.Add(entitlement);
                }
            }

            return presentationUserEntitlementList;
        }
        public List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId, string entitlementType, string userAccountName)
        {
            return new List<PresentationUserEntitlement>();
        }

        public List<PresentationUserEntitlement> GetEntitlementDetailByApplicationTypeWithAllCanRequest(long endpointsystemId)
        {
            return new PresentationSAPHANAUserEntitlement().GetEntitlementDetailByApplicationType(endpointsystemId);
        }

        public List<EntityModel.Presentation.PresentationBusinessRoleDetail> GetIndirectRole(ApplicationType applicationType, string userLogonName)
        {
            throw new NotImplementedException();
        }

        public List<EntityModel.Presentation.PresentationBusinessRoleDetail> GetSAPHANAIndirectRoleByPosition(long endpointId, string userAccountName, string positionCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new SAPHANARoleInfoRepository(dataAccessTransaction).GetSAPHANAIndirectRoleByPosition(endpointId, userAccountName, positionCode);
            }
        }
        public List<EntityModel.Presentation.PresentationBusinessRoleDetail> GetSAPHANAIndirectRole(long endpointId, long accountReconId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new SAPHANARoleInfoRepository(dataAccessTransaction).GetSAPHANAIndirectRole(endpointId, accountReconId);
            }
        }

        public List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(EndPoint endpoint, string userAccountName, long entitlementId, string entityCode, EntitlementType fromValue, EntitlementType toValue, string reserved, string reserved2, string description, ApplicationType applicationType)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            try
            {
                if (fromValue == EntitlementType.Account && toValue == EntitlementType.BusinessRole)
                {
                    presentationUserEntitlementList = GetBusinessRole(endpoint, userAccountName, entitlementId, entityCode);
                }
                if (fromValue == EntitlementType.Account && toValue == EntitlementType.IndirectRole)
                {
                    presentationUserEntitlementList = GetIndirectRole(endpoint, userAccountName, entitlementId);
                }
                if (fromValue == EntitlementType.Position && toValue == EntitlementType.IndirectRole)
                {
                    presentationUserEntitlementList = GetIndirectRole(endpoint, userAccountName, entitlementId, entityCode);
                }
                else if (fromValue == EntitlementType.BusinessRole && toValue == EntitlementType.TechnicalRole)
                {
                    presentationUserEntitlementList = GetTechnicalRole(endpoint, userAccountName, entityCode, entitlementId);
                }
                else if ((fromValue == EntitlementType.TechnicalRole && toValue == EntitlementType.TCodes) || (fromValue == EntitlementType.IndirectRole && toValue == EntitlementType.TCodes))
                {
                    presentationUserEntitlementList = GetTCodesForRole(endpoint, entityCode, endpoint);
                }
            }
            catch (Exception ex)
            {
            }
            return presentationUserEntitlementList;
        }
    }
}
