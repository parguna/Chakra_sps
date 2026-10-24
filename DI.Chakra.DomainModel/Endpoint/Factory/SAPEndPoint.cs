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
    public class SAPEndPoint : IEndPointFactory
    {
        public List<PresentationUserEntitlement> GetAllEntitlementByType(EntitlementType type, long endpointsystemid)
        {
            return new PresentationSAPUserEntitlement().GetAllEntitlementDetailByType(endpointsystemid, type);
        }

        public IEndpointUserInfo GetUser()
        {
            return new SAPUserinfo();
        }
        public IEndpointUserInfo GetUser(long id)
        {
            return SAPUserinfo.GetById(id, true);
        }

        public IEndpointEntitlementInfo GetEntitlement()
        {
            return new SAPRoleInfo();
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
            return SAPRoleInfo.GetById(id);
        }
        public IEndpointEntitlementInfo GetEntitlement(long systemId, string entitlementName, EntitlementType entitlementType)
        {
            if (entitlementType == EntitlementType.Role)
            {
                return SAPRoleInfo.GetAll().Where(t => t.EndPointSystemId == systemId &&
                t.EntitlementType == entitlementType && t.EntitlementName.ToUpper() == entitlementName.ToUpper()).FirstOrDefault();
            }
            else if (entitlementType == EntitlementType.Group)
            {
                return SAPGroupInfo.GetAll().Where(t => t.EndPointSystemId == systemId &&
               t.EntitlementType == entitlementType && t.EntitlementName.ToUpper() == entitlementName.ToUpper()).FirstOrDefault();
            }
            else if (entitlementType == EntitlementType.Profile)
            {
                return SAPProfileInfo.GetAll().Where(t => t.EndPointSystemId == systemId &&
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
                    return SAPRoleInfo.GetById(id);
                default:
                    throw new ApplicationException("Entitlement Type : " + type.ToString() + " not supported for AD");
            }
        }

        //public void SaveUserInfo(long id, long endPointSystemId, string currentUser)
        //{
        //    SAPUserinfo.GetUserAccountByEndPointSystemId(id, endPointSystemId).Save(currentUser);
        //}
        //public Dictionary<long, string> GetEntitlementName(long endpointSystemId, string entitlementType)
        //{
        //    return new PresentationSAPUserEntitlement().GetEntitlementName(endpointSystemId, entitlementType);
        //}
        public PresentationUserEntitlement GetEntitlement(IEndpointEntitlementInfo entitlement, string entitlementType)
        {
            return new PresentationSAPUserEntitlement().GetEntitlement(entitlement, entitlementType);
        }
        public void UpdateEntitlement(PresentationUserEntitlement presentationUserEntitlement, IEndpointEntitlementInfo entitlement, string currentUser)
        {
            new PresentationSAPUserEntitlement().UpdateEntitlement(presentationUserEntitlement, entitlement, currentUser);
        }
        public List<PresentationUserEntitlement> GetEntitlementDetail(long entitlementId)
        {
            return new PresentationSAPUserEntitlement().GetEntitlementDetail(entitlementId);
        }

        public List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId)
        {
            return new PresentationSAPUserEntitlement().GetEntitlementHierachyDetail(entitlementId);
        }

        public List<PresentationUserEntitlement> GetEntitlementDetailByApplicationType(long endpointsystemId)
        {
            return new PresentationSAPUserEntitlement().GetEntitlementDetailByApplicationType(endpointsystemId);
        }

        public List<Group> GetAllGroupsByEndpointID(long endPointID, string searchString)
        {
            return SAPGroupInfo.GetAllGroupsByEndpointID(endPointID, searchString);
        }
        public Dictionary<long, string> GetEntitlements(string searchString, long endpointSystemId, long? EndpointUserId, bool existingEntitlements)
        {
            throw new ApplicationException();
        }
        public void UpdatePasswordresetcount(long endpointId, string userName, string currentUser)
        {
            SAPUserinfo.UpdatePasswordresetcount(endpointId, userName, currentUser);
        }
        //public List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(long endpointId, string userAccountName, long entitlementId, string entityCode, EntitlementType fromValue, EntitlementType toValue)
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
                else if ((fromValue == EntitlementType.TechnicalRole && toValue == EntitlementType.TCodes)||(fromValue == EntitlementType.IndirectRole && toValue == EntitlementType.TCodes))
                {
                    presentationUserEntitlementList = GetTCodesForRole(endpoint,entityCode, endpoint);
                }
            }
            catch (Exception ex)
            {
                new DI.Chakra.Logger.AttestationLogger().WriteLog(Logger.LogLevelType.Exception, $"{endpoint.DisplayName}: userAccountName:{userAccountName} entityCode:{entityCode}  fromValue:{fromValue} toValue:{toValue} GetIndirectRole failed.{ex.Message}", ex);
            }
            return presentationUserEntitlementList;
        }

        private List<PresentationUserEntitlement> GetIndirectRole(EndPoint endpoint, string userAccountName, long accountReconId,string positionCode=null)
        {
            var pEntitlement = new List<PresentationUserEntitlement>();
            
            try
            {
                var applnType = endpoint.ApplicationType;
                {
                    List<EntityModel.Presentation.PresentationBusinessRoleDetail> technicalRoles = null;
                    switch (applnType)
                    {
                        case ApplicationType.HRMS_FLINS:
                            #region FLINS
                            var roles = GetSAPFlinsIndirctRole(applnType, accountReconId);
                            if(roles != null)
                            {
                                foreach (var technicalRole in roles)
                                {
                                    var sapUserEntitlement = new PresentationSAPUserEntitlement();
                                    sapUserEntitlement.EntitlementType = EntitlementType.IndirectRole;
                                    sapUserEntitlement.ParentEntitlementId = accountReconId;
                                    sapUserEntitlement.EntitlementId = technicalRole.Id;
                                    sapUserEntitlement.Name = technicalRole.Entitlement;
                                    sapUserEntitlement.Description = technicalRole.Description;
                                    sapUserEntitlement.Category = EntitlementType.IndirectRole.ToString();
                                    sapUserEntitlement.EndPointSystemType = EndpointType.SAP;
                                    sapUserEntitlement.EndpointUserId = technicalRole.EndpointUserId;
                                    sapUserEntitlement.EndpointSystemName = endpoint.DisplayName;
                                    sapUserEntitlement.EndpointSystemId = endpoint.Id;
                                    sapUserEntitlement.ValidTo = technicalRole.ValidToDate;
                                    sapUserEntitlement.LastAttestedOn = technicalRole.LastAttestedOn;
                                    pEntitlement.Add(sapUserEntitlement);
                                }
                            }
                            return pEntitlement;
                            #endregion
                            break;
                        case ApplicationType.FLINS_BI:
                        case ApplicationType.SAP_GRC:
                            #region FLINSBI-GRC
                            technicalRoles = this.GetIndirectRole(applnType, userAccountName);
                            #endregion
                            break;
                        case ApplicationType.MSSL_ISU:
                        case ApplicationType.MSSL_CRM:
                        case ApplicationType.EBS_ISU:
                        case ApplicationType.EBS_CRM:
                            #region MSSL -EBS
                            if (!string.IsNullOrWhiteSpace(positionCode))
                            {
                                technicalRoles = this.GetMSSLEBSIndirectRoleByPosition(endpoint.Id, userAccountName, positionCode);
                            }
                            else
                            {
                                technicalRoles = GetMSSLEBSIndirectRole(endpoint.Id, accountReconId);
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
                            if(!string.IsNullOrWhiteSpace(technicalRole.BusinessRole))
                            if (!pEntitlement.Exists(i => i.Name.Equals(technicalRole.BusinessRole, StringComparison.InvariantCultureIgnoreCase)))
                            {
                                var sapUserEntitlement = new PresentationSAPUserEntitlement();
                                sapUserEntitlement.EntitlementType = EntitlementType.IndirectRole;
                                sapUserEntitlement.ParentEntitlementId = accountReconId;
                                sapUserEntitlement.EntitlementId = technicalRole.Id;
                                sapUserEntitlement.Name = technicalRole.BusinessRole;
                                sapUserEntitlement.Description = technicalRole.Description;
                                sapUserEntitlement.Category = EntitlementType.IndirectRole.ToString();
                                sapUserEntitlement.EndPointSystemType = EndpointType.SAP;
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

        public List<PresentationUserEntitlement> GetTechnicalRole(EndPoint ep, string userLogonName,string businessRole, long keyId)
        {
            var applnType = ep.ApplicationType;
            var pEntitlement = new List<PresentationUserEntitlement>();
            var account = AccountReconcilation.GetByAccountName(userLogonName, ep.Id);
            if (account == null || account.IsDeleted) return pEntitlement;
            if (applnType == ApplicationType.MSSL_CRM || applnType == ApplicationType.MSSL_ISU || applnType == ApplicationType.EBS_CRM || applnType == ApplicationType.EBS_ISU || applnType == ApplicationType.SAP_GRC || applnType == ApplicationType.FLINS_BI)
            {
                using (var trans = new DataAccessTransaction(false))
                {
                    var technicalRoles = new SAPRoleInfoRepository(trans).GetTechnicalRolesFromBusinessRole(ep.Id, account.EndpointUserId, businessRole, applnType);
                    foreach (var technicalRole in technicalRoles)
                    {
                        if (!pEntitlement.Exists(i => i.Name.Equals(technicalRole.ROLE_CODE, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            var sapUserEntitlement = new PresentationSAPUserEntitlement();
                            sapUserEntitlement.EntitlementType = EntitlementType.BusinessRole;
                            sapUserEntitlement.ParentEntitlementId = keyId;
                            sapUserEntitlement.EntitlementId = technicalRole.SAP_ROLEID;
                            sapUserEntitlement.Name = technicalRole.ROLE_CODE;
                            sapUserEntitlement.Description = technicalRole.ROLE_DESC;
                            sapUserEntitlement.Category = EntitlementType.TechnicalRole.ToString();
                            sapUserEntitlement.EndPointSystemType = EndpointType.SAP;
                            sapUserEntitlement.EndpointUserId = account.EndpointUserId;
                            sapUserEntitlement.EndpointSystemName = ep.DisplayName;
                            sapUserEntitlement.EndpointSystemId = ep.Id;
                            sapUserEntitlement.ValidTo = technicalRole.ValidToDate;
                            pEntitlement.Add(sapUserEntitlement);
                        }
                    }
                }

            }else if(applnType == ApplicationType.MSSL || applnType == ApplicationType.EBS)
            {
                using (var trans = new DataAccessTransaction(false))
                {
                    var technicalRoles = new SAPHANARoleInfoRepository(trans).GetTechnicalRolesFromBusinessRole(ep.Id, account.EndpointUserId, businessRole, applnType);
                    foreach (var technicalRole in technicalRoles)
                    {
                        if (!pEntitlement.Exists(i => i.Name.Equals(technicalRole.ROLE_CODE, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            var sapUserEntitlement = new PresentationSAPUserEntitlement();
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
        public List<PresentationUserEntitlement> GetBusinessRole(EndPoint ep,string userAccountName, long keyId, string entityCode)
        {
            var applnType = ep.ApplicationType;
            var account = AccountReconcilation.GetById(keyId);
            var pEntitlement = new List<PresentationUserEntitlement>();
            if (account != null)
            {
                if (applnType == ApplicationType.SAP_GRC || applnType == ApplicationType.FLINS_BI || applnType == ApplicationType.HRMS_FLINS || applnType == ApplicationType.MSSL_CRM || applnType == ApplicationType.MSSL_ISU || applnType == ApplicationType.EBS_CRM || applnType == ApplicationType.EBS_ISU)
                {
                    var businessRoles = SAPRoleInfo.GetBusinessRoles(ep.Id, account.EndpointUserId, applnType);
                    foreach (var businessRole in businessRoles)
                    {
                        if (!pEntitlement.Exists(i => i.Name.Equals(businessRole.BusinessRole, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            var sapUserEntitlement = new PresentationSAPUserEntitlement();
                            sapUserEntitlement.EntitlementType = EntitlementType.BusinessRole;
                            sapUserEntitlement.ParentEntitlementId = keyId;
                            sapUserEntitlement.EntitlementId = businessRole.SAP_ROLEID;
                            sapUserEntitlement.Name = businessRole.BusinessRole;
                            //sapUserEntitlement.Description = $"{(!string.IsNullOrWhiteSpace(businessRole.Module)? "Module: " + businessRole.Module:"")}{(!string.IsNullOrWhiteSpace(businessRole.SubModule)? " / SubModule: " + businessRole.SubModule : "")}";
                            sapUserEntitlement.Description = businessRole.Description;
                            sapUserEntitlement.Category = EntitlementType.BusinessRole.ToString();
                            sapUserEntitlement.EndPointSystemType = EndpointType.SAP;
                            sapUserEntitlement.EndpointUserId = account.EndpointUserId;
                            sapUserEntitlement.EndpointSystemName = ep.DisplayName;
                            sapUserEntitlement.EndpointSystemId = ep.Id;
                            sapUserEntitlement.ValidTo = businessRole.ValidToDate;

                            if (applnType == ApplicationType.HRMS_FLINS)
                                sapUserEntitlement.Remarks1 = businessRole.Company;

                            sapUserEntitlement.Remarks3 = businessRole.ValidToDate.HasValue ? businessRole.ValidToDate.Value.ToString("dd/MM/yyyy") : null;
                            pEntitlement.Add(sapUserEntitlement);
                        }
                    }
                }
                else if (applnType == ApplicationType.SRM)
                {
                    pEntitlement = SAPSRMBusinessRoleMapping.GetSRMBusinessRole(account.Id);
                }
            }
            return pEntitlement;
        }
        public List<PresentationUserEntitlement> GetTCodesForRole(EndPoint endpoint, string roleName, EndPoint dpEndpoint)
        {
            long endpointSystemId = dpEndpoint.Id;
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();

            SAPRoleInfo role = SAPRoleInfo.GetByRoleCode(roleName, endpointSystemId);

            if (role != null)
            {
                var tCodes = PresentationSAPTCodeReport.GetTcodeInfo(endpointSystemId, role.ROLE_CODE);
                if(tCodes == null)
                {
                    return presentationUserEntitlementList;
                }
                foreach (var tcode in tCodes)
                {
                    //var name = dpEndpoint != null ? $"{dpEndpoint.DisplayName}:{tcode.TCODE_Code}" : $"{tcode.TCODE_Code}";
                    var name = $"{tcode.TCODE_Code}";
                    if (presentationUserEntitlementList.Exists(i => i.Name == name && i.Description == tcode.TCODE_Desc)) continue;
                    // create tcde presenatation object
                    var entitlement = new PresentationSAPUserEntitlement();
                    entitlement.ParentEntitlementId = role.Id;
                    entitlement.EntitlementId = tcode.TCODE_Id;
                    entitlement.Name = name;
                    entitlement.Description = tcode.TCODE_Desc;
                    entitlement.Category = EntitlementType.TCodes.ToString();
                    entitlement.EndPointSystemType = EndpointType.SAP;
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
            return new PresentationSAPUserEntitlement().GetEntitlementDetailByApplicationType(endpointsystemId);
        }

        public List<PresentationUserEntitlement> GetSRMBusinessRoleFromTechnicalRole(EndPoint endpoint, string accountName, long keyId, string roleName, EntitlementType fromType, EntitlementType toType, string pOrg, string pGrp, string applnType = null)
        {
            var entitlements = new List<PresentationUserEntitlement>();
            if (endpoint.ApplicationType == ApplicationType.SRM && fromType == EntitlementType.BusinessRole && toType == EntitlementType.TechnicalRole)
            {
                entitlements = SAPSRMBusinessRoleMapping.GetSRMTechnicalRole(endpoint, accountName, applnType, pOrg, pGrp);
            }else if (endpoint.ApplicationType == ApplicationType.SRM && fromType == EntitlementType.TechnicalRole && toType == EntitlementType.TCodes)
            {
                entitlements = GetSRMTechnicalRoleTCode(endpoint, accountName, keyId, roleName, applnType);
            }
            return entitlements;
        }
        public  List<PresentationUserEntitlement> GetSRMTechnicalRoleTCode(EndPoint endpoint, string accountName, long keyId, string roleName, string applicationType)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            try
            {
                if (endpoint.ApplicationType == ApplicationType.SRM)
                {
                    if (!string.IsNullOrWhiteSpace(applicationType))
                    {
                        var applnType = EnumExt.GetValueFromDescription<ApplicationType>(applicationType);
                        var endpointSystem = EndPoint.GetByApplicationType(applnType);
                        if (endpointSystem != null)
                        {
                            presentationUserEntitlementList = this.GetTCodesForRole(endpoint, roleName, endpointSystem);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                new DI.Chakra.Logger.AttestationLogger().WriteLog(Logger.LogLevelType.Exception, $"{endpoint.DisplayName}: RoleName:{roleName} GetSRMTechnicalRoleTCode failed.{ex.Message}", ex);
            }
            return presentationUserEntitlementList.Distinct().ToList();
        }
        public List<EntityModel.Presentation.PresentationBusinessRoleDetail> GetIndirectRole(ApplicationType applicationType, string userLogonName)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new SAPRoleInfoRepository(dataAccessTransaction).GetSAPIndirectRole(applicationType, userLogonName);
            }
        }
        public List<EntityModel.Presentation.PresentationFlinsIndirectRole> GetSAPFlinsIndirctRole(ApplicationType applicationType, long accountReconId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new SAPRoleInfoRepository(dataAccessTransaction).GetSAPFlinsIndirctRole(applicationType, accountReconId);
            }
        }
        public List<EntityModel.Presentation.PresentationBusinessRoleDetail> GetMSSLEBSIndirectRoleByPosition(long endpointId, string userAccountName, string positionCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new SAPRoleInfoRepository(dataAccessTransaction).GetMSSLEBSIndirectRoleByPosition(endpointId, userAccountName, positionCode);
            }
        }
        public List<EntityModel.Presentation.PresentationBusinessRoleDetail> GetMSSLEBSIndirectRole(long endpointId, long accountReconId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new SAPRoleInfoRepository(dataAccessTransaction).GetMSSLEBSIndirectRole(endpointId, accountReconId);
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
                    if(applicationType == ApplicationType.SRM)
                    {
                        presentationUserEntitlementList = GetSRMBusinessRoleFromTechnicalRole(endpoint, userAccountName, entitlementId, entityCode, fromValue, toValue, reserved, reserved2, description);
                    }
                    else if(applicationType == ApplicationType.HRMS_FLINS)
                    {
                        using (var tran = new DataAccessTransaction(false))
                        {
                            var entitlements = new List<PresentationUserEntitlement>();
                            var roles = new SAPRoleInfoRepository(tran).GetSAPFlinsTechnicalRole(endpoint.Id, userAccountName, entityCode, description);
                            if(roles != null && roles.Count > 0)
                            {
                                foreach (var role in roles)
                                {
                                    PresentationSAPUserEntitlement usrEntitlement = new PresentationSAPUserEntitlement();
                                    usrEntitlement.Name = role.Entitlement;
                                    usrEntitlement.Description = role.EntitlementDescription;
                                    usrEntitlement.EndpointSystemName = endpoint.DisplayName;
                                    usrEntitlement.Remarks1 = role.TechnicalRule;
                                    usrEntitlement.ParentEntitlementId = entitlementId;
                                    usrEntitlement.LastAttestedOn = role.LastAttestedOn;
                                    usrEntitlement.Remarks3 = role.ValidToDate.HasValue ? role.ValidToDate.Value.ToString("dd/MM/yyyy"):null;
                                    entitlements.Add(usrEntitlement);
                                }
                            }
                            return entitlements;
                        }
                    }
                    else
                    {
                        presentationUserEntitlementList = GetTechnicalRole(endpoint, userAccountName, entityCode, entitlementId);
                    }
                }
                else if ((fromValue == EntitlementType.TechnicalRole && toValue == EntitlementType.TCodes) || (fromValue == EntitlementType.IndirectRole && toValue == EntitlementType.TCodes))
                {
                    if (applicationType == ApplicationType.SRM)
                    {
                        presentationUserEntitlementList = GetSRMBusinessRoleFromTechnicalRole(endpoint, userAccountName, entitlementId, entityCode, fromValue, toValue, reserved, reserved2, description);
                    }
                    else
                    {
                        presentationUserEntitlementList = GetTCodesForRole(endpoint, entityCode, endpoint);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return presentationUserEntitlementList;
        }

        
    }
}
