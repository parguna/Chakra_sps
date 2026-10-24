using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DI.Chakra.DomainApi
{
    public class XLDataConverter : TypeConverter<DomainModel.IXLData, Dto.IXLData>
    {
        protected override Dto.IXLData ConvertCore(DomainModel.IXLData source)
        {
            if (source is DomainModel.InhousePermission) return Mapper.Map<Dto.InhousePermission>(source);
            else if (source is DomainModel.InhouseUserInfo) return Mapper.Map<Dto.InhouseUserInfo>(source);
            else if (source is DomainModel.InhouseFunction) return Mapper.Map<Dto.InhouseFunction>(source);
            else if (source is DomainModel.InhUserPermissionFunction) return Mapper.Map<Dto.InhUserPermissionFunction>(source);
            else if (source is DomainModel.Presentation.PresentationUserEntitlement) return Mapper.Map<Dto.Presentation.PresentationUserEntitlement>(source);
            else if (source is DomainModel.ADSXLProvisioningMatrix) return Mapper.Map<Dto.ADSXLProvisioningMatrix>(source);
            else if (source is DomainModel.ProvisioningConfigMatrixSAPOrg) return Mapper.Map<Dto.ProvisioningConfigMatrixSAPOrg>(source);
            else if (source is DomainModel.SAPBIRoleCategory) return Mapper.Map<Dto.SAPBIRoleCategory>(source);
            else if (source is DomainModel.SAPPIRole) return Mapper.Map<Dto.SAPPIRole>(source);
            else if (source is DomainModel.ProvisioningConfigMatrixSapEEGroup) return Mapper.Map<Dto.ProvisioningConfigMatrixSapEEGroup>(source);
            else if (source is DomainModel.ProvisioningConfigMatrixSAPPosition) return Mapper.Map<Dto.ProvisioningConfigMatrixSAPPosition>(source);
            else if (source is DomainModel.SAPHRFLINSRoleMatrix) return Mapper.Map<Dto.SAPHRFLINSRoleMatrix>(source);
            else if (source is DomainModel.SAPHRFLINSSuppRoleMatrix) return Mapper.Map<Dto.SAPHRFLINSSuppRoleMatrix>(source);
            else if (source is DomainModel.SAPRoleMandatoryField) return Mapper.Map<Dto.SAPRoleMandatoryField>(source);
            else if (source is DomainModel.SAPRoleMatrix) return Mapper.Map<Dto.SAPRoleMatrix>(source);
            else if (source is DomainModel.SAPHRFLINSConflictingRole) return Mapper.Map<Dto.SAPHRFLINSConflictingRole>(source);
            else if (source is DomainModel.SAPGRCRole) return Mapper.Map<Dto.SAPGRCRole>(source);
            else if (source is DomainModel.SAPXLMSSLCriticalTCodeMatrix) return Mapper.Map<Dto.SAPXLMSSLCriticalTCodeMatrix>(source);
            else if (source is DomainModel.SAPXLMSSLPositionsMatrix) return Mapper.Map<Dto.SAPXLMSSLPositionsMatrix>(source);
            else if (source is DomainModel.SAPXLMSSLRolesMatrix) return Mapper.Map<Dto.SAPXLMSSLRolesMatrix>(source);
            else if (source is DomainModel.SAPEBSXLCriticalTCodesMatrix) return Mapper.Map<Dto.SAPEBSXLCriticalTCodesMatrix>(source);
            else if (source is DomainModel.SAPEBSXLPositionsMatrix) return Mapper.Map<Dto.SAPEBSXLPositionsMatrix>(source);
            else if (source is DomainModel.SAPEBSXLRoleMatrix) return Mapper.Map<Dto.SAPEBSXLRoleMatrix>(source);
            else if (source is DomainModel.SAPEPRoleInfo) return Mapper.Map<Dto.SAPEPRoleInfo>(source);
            else if (source is DomainModel.SAPEPGroupInfo) return Mapper.Map<Dto.SAPEPGroupInfo>(source);
            else if (source is DomainModel.NPACSectionInfo) return Mapper.Map<Dto.NPACSectionInfo>(source);
            else if (source is DomainModel.NPACDesignationInfo) return Mapper.Map<Dto.NPACDesignationInfo>(source);
            else if (source is DomainModel.SAPSRMMainRoleMatrix) return Mapper.Map<Dto.SAPSRMMainRoleMatrix>(source);
            else if (source is DomainModel.SAPSRMEnablerRoleMatrix) return Mapper.Map<Dto.SAPSRMEnablerRoleMatrix>(source);
            else if (source is DomainModel.SAPSRMBusinessRoleMapping) return Mapper.Map<Dto.SAPSRMBusinessRoleMapping>(source);
            else if (source is DomainModel.GEMSRoleMatrix) return Mapper.Map<Dto.GEMSRoleMatrix>(source);
            else if (source is DomainModel.Office365LicenseConfig) return Mapper.Map<Dto.Office365LicenseConfig>(source);
            else if (source is DomainModel.SMSXLProvisioningMatrix) return Mapper.Map<Dto.SMSXLProvisioningMatrix>(source);
            else if (source is DomainModel.SMSXLRoleCatalogue) return Mapper.Map<Dto.SMSXLRoleCatalogue>(source);
            else if (source is DomainModel.NOVAUserRoleMatrix) return Mapper.Map<Dto.NOVAUserRoleMatrix>(source);
            else if (source is DomainModel.NOVAManagementRoleMatrix) return Mapper.Map<Dto.NOVAManagementRoleMatrix>(source);
            else if (source is DomainModel.NOVAExceptionalRoleMatrix) return Mapper.Map<Dto.NOVAExceptionalRoleMatrix>(source);
            else if (source is DomainModel.NOVASoDExceptionsMatrix) return Mapper.Map<Dto.NOVASoDExceptionsMatrix>(source);
            else if (source is DomainModel.ApproverExceptionInfo) return Mapper.Map<Dto.ApproverExceptionInfo>(source);
            else if (source is DomainModel.VPNMatrix) return Mapper.Map<Dto.VPNMatrix>(source);
            else if (source is DomainModel.SkypeForBusinessProvisioningMatrix) return Mapper.Map<Dto.SkypeForBusinessProvisioningMatrix>(source);
            else if (source is DomainModel.NOVACompositeRoleMatrix) return Mapper.Map<Dto.NOVACompositeRoleMatrix>(source);
            else if (source is DomainModel.SAPSODConflictingRole) return Mapper.Map<Dto.SAPSODConflictingRole>(source);
            else if (source is DomainModel.SAPHANAXLRolesMatrix) return Mapper.Map<Dto.SAPHANAXLRolesMatrix>(source);
            else if (source is DomainModel.SAPHANAXLCriticalTCodeMatrix) return Mapper.Map<Dto.SAPHANAXLCriticalTCodeMatrix>(source);
            else if (source is DomainModel.SAPHANAXLPositionsMatrix) return Mapper.Map<Dto.SAPHANAXLPositionsMatrix>(source);
            return null;
        }
    }

    public class XLDataConverterDomainConverter : TypeConverter<Dto.IXLData, DomainModel.IXLData>
    {
        protected override DomainModel.IXLData ConvertCore(Dto.IXLData source)
        {
            if (source is Dto.InhousePermission) return Mapper.Map<DomainModel.InhousePermission>(source);
            else if (source is Dto.InhouseUserInfo) return Mapper.Map<DomainModel.InhouseUserInfo>(source);
            else if (source is Dto.InhouseFunction) return Mapper.Map<DomainModel.InhouseFunction>(source);
            else if (source is Dto.InhUserPermissionFunction) return Mapper.Map<DomainModel.InhUserPermissionFunction>(source);
            else if (source is Dto.Presentation.PresentationUserEntitlement) return Mapper.Map<DomainModel.Presentation.PresentationUserEntitlement>(source);
            else if (source is Dto.ADSXLProvisioningMatrix) return Mapper.Map<DomainModel.ADSXLProvisioningMatrix>(source);
            else if (source is Dto.ProvisioningConfigMatrixSAPOrg) return Mapper.Map<DomainModel.ProvisioningConfigMatrixSAPOrg>(source);
            else if (source is Dto.SAPBIRoleCategory) return Mapper.Map<DomainModel.SAPBIRoleCategory>(source);
            else if (source is Dto.SAPPIRole) return Mapper.Map<DomainModel.SAPPIRole>(source);
            else if (source is Dto.ProvisioningConfigMatrixSapEEGroup) return Mapper.Map<DomainModel.ProvisioningConfigMatrixSapEEGroup>(source);
            else if (source is Dto.ProvisioningConfigMatrixSAPPosition) return Mapper.Map<DomainModel.ProvisioningConfigMatrixSAPPosition>(source);
            else if (source is Dto.SAPHRFLINSRoleMatrix) return Mapper.Map<DomainModel.SAPHRFLINSRoleMatrix>(source);
            else if (source is Dto.SAPHRFLINSSuppRoleMatrix) return Mapper.Map<DomainModel.SAPHRFLINSSuppRoleMatrix>(source);
            else if (source is Dto.SAPRoleMandatoryField) return Mapper.Map<DomainModel.SAPRoleMandatoryField>(source);
            else if (source is Dto.SAPRoleMatrix) return Mapper.Map<DomainModel.SAPRoleMatrix>(source);
            else if (source is Dto.SAPHRFLINSConflictingRole) return Mapper.Map<DomainModel.SAPHRFLINSConflictingRole>(source);
            else if (source is Dto.SAPGRCRole) return Mapper.Map<DomainModel.SAPGRCRole>(source);
            else if (source is Dto.SAPXLMSSLCriticalTCodeMatrix) return Mapper.Map<DomainModel.SAPXLMSSLCriticalTCodeMatrix>(source);
            else if (source is Dto.SAPXLMSSLPositionsMatrix) return Mapper.Map<DomainModel.SAPXLMSSLPositionsMatrix>(source);
            else if (source is Dto.SAPXLMSSLRolesMatrix) return Mapper.Map<DomainModel.SAPXLMSSLRolesMatrix>(source);
            else if (source is Dto.SAPEBSXLCriticalTCodesMatrix) return Mapper.Map<DomainModel.SAPEBSXLCriticalTCodesMatrix>(source);
            else if (source is Dto.SAPEBSXLPositionsMatrix) return Mapper.Map<DomainModel.SAPEBSXLPositionsMatrix>(source);
            else if (source is Dto.SAPEBSXLRoleMatrix) return Mapper.Map<DomainModel.SAPEBSXLRoleMatrix>(source);
            else if (source is Dto.SAPEPRoleInfo) return Mapper.Map<DomainModel.SAPEPRoleInfo>(source);
            else if (source is Dto.SAPEPGroupInfo) return Mapper.Map<DomainModel.SAPEPGroupInfo>(source);
            else if (source is Dto.NPACSectionInfo) return Mapper.Map<DomainModel.NPACSectionInfo>(source);
            else if (source is Dto.NPACDesignationInfo) return Mapper.Map<DomainModel.NPACDesignationInfo>(source);
            else if (source is Dto.SAPSRMMainRoleMatrix) return Mapper.Map<DomainModel.SAPSRMMainRoleMatrix>(source);
            else if (source is Dto.SAPSRMEnablerRoleMatrix) return Mapper.Map<DomainModel.SAPSRMEnablerRoleMatrix>(source);
            else if (source is Dto.SAPSRMBusinessRoleMapping) return Mapper.Map<DomainModel.SAPSRMBusinessRoleMapping>(source);
            else if (source is Dto.GEMSRoleMatrix) return Mapper.Map<DomainModel.GEMSRoleMatrix>(source);
            else if (source is Dto.Office365LicenseConfig) return Mapper.Map<DomainModel.Office365LicenseConfig>(source);
            else if (source is Dto.SMSXLProvisioningMatrix) return Mapper.Map<DomainModel.SMSXLProvisioningMatrix>(source);
            else if (source is Dto.SMSXLRoleCatalogue) return Mapper.Map<DomainModel.SMSXLRoleCatalogue>(source);
            else if (source is Dto.NOVAUserRoleMatrix) return Mapper.Map<DomainModel.NOVAUserRoleMatrix>(source);
            else if (source is Dto.NOVAManagementRoleMatrix) return Mapper.Map<DomainModel.NOVAManagementRoleMatrix>(source);
            else if (source is Dto.NOVAExceptionalRoleMatrix) return Mapper.Map<DomainModel.NOVAExceptionalRoleMatrix>(source);
            else if (source is Dto.NOVASoDExceptionsMatrix) return Mapper.Map<DomainModel.NOVASoDExceptionsMatrix>(source);
            else if (source is Dto.ApproverExceptionInfo) return Mapper.Map<DomainModel.ApproverExceptionInfo>(source);
            else if (source is Dto.VPNMatrix) return Mapper.Map<DomainModel.VPNMatrix>(source);
            else if (source is Dto.SkypeForBusinessProvisioningMatrix) return Mapper.Map<DomainModel.SkypeForBusinessProvisioningMatrix>(source);
            else if (source is Dto.NOVACompositeRoleMatrix) return Mapper.Map<DomainModel.NOVACompositeRoleMatrix>(source);
            else if (source is Dto.SAPSODConflictingRole) return Mapper.Map<DomainModel.SAPSODConflictingRole>(source);
            else if (source is Dto.SAPHANAXLRolesMatrix) return Mapper.Map<DomainModel.SAPHANAXLRolesMatrix>(source);
            else if (source is Dto.SAPHANAXLCriticalTCodeMatrix) return Mapper.Map<DomainModel.SAPHANAXLCriticalTCodeMatrix>(source);
            else if (source is Dto.SAPHANAXLPositionsMatrix) return Mapper.Map<DomainModel.SAPHANAXLPositionsMatrix>(source);
            return null;
        }
    }
}