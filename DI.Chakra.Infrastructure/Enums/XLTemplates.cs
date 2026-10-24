using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure.Enums
{
    public class XLTemplateFriendlyName : Attribute, IAttribute
    {
        public XLTemplateFriendlyName(string text)
        {
            Value = text;
        }

        public string Value { get; private set; }
    }

    public class XLTemplateName : Attribute, IAttribute
    {
        public XLTemplateName(string text)
        {
            Value = text;
        }

        public string Value { get; private set; }
    }
    public enum XLTemplates
    {
        [XLTemplateName("XL_ADSPROVISIONINGMATRIX")]
        [XLTemplateFriendlyName("Active Directory Provisioning Matrix")]
        ADS_PROVISIONINGMATRIX = 0,
        [XLTemplateName("XL_LNSPROVISIONINGMATRIX")]
        [XLTemplateFriendlyName("Lotus Notes Provisioning Matrix")]
        LNS_PROVISIONINGMATRIX = 1,
        [XLTemplateName("XL_SAPHRFLINSRoleMatrix")]
        [XLTemplateFriendlyName("SAP HRMS FLINS Main Roles Matrix")]
        SAP_HRFLINSRoleMatrix = 2,
        [XLTemplateName("XL_SAPHRFLINSSuppRoleMatrix")]
        [XLTemplateFriendlyName("SAP HRMS FLINS Supplementary Roles Matrix")]
        SAP_HRFLINSSuppRoleMatrix = 3,

        [XLTemplateName("XL_SAPRoleMandatoryFields")]
        [XLTemplateFriendlyName("SAP Role Mandatory Fields Provisioning Matrix")]
        SAP_RoleMandatoryFields = 4,

        [XLTemplateName("XL_SAPRoleMatrix")]
        [XLTemplateFriendlyName("SAP Role Provisioning Matrix")]
        SAP_RoleMatrix = 5,

        [XLTemplateName("XL_SAPEBSRoleMatrix")]
        [XLTemplateFriendlyName("SAP EBS Role Provisioning Matrix")]
        SAP_EBSRoleMatrix = 6,

        [XLTemplateName("XL_SAPEBSPositionMatrix")]
        [XLTemplateFriendlyName("SAP EBS Position Provisioning Matrix")]
        SAP_EBSPositionMatrix = 7,

        [XLTemplateName("XL_SAPEBSCriticalTCodesMatrix")]
        [XLTemplateFriendlyName("SAP EBS Critical TCodes Provisioning Matrix")]
        SAP_EBSCriticalTCodesMatrix = 8,

        [XLTemplateName("XL_SAPMSSLRoleMatrix")]
        [XLTemplateFriendlyName("SAP MSSL Role Provisioning Matrix")]
        SAP_MSSLRoleMatrix = 9,

        [XLTemplateName("XL_SAPMSSLPositionMatrix")]
        [XLTemplateFriendlyName("SAP MSSL Position Provisioning Matrix")]
        SAP_MSSLPositionMatrix = 10,

        [XLTemplateName("XL_SAPMSSLCriticalTCodesMatrix")]
        [XLTemplateFriendlyName("SAP MSSL Critical TCodes Provisioning Matrix")]
        SAP_MSSLCriticalTCodesMatrix = 11,

        [XLTemplateName("XL_SAPPERERecruitmentRole")]
        [XLTemplateFriendlyName("SAP ERecruitment Role Provisioning Matrix")]
        SAP_PER_E_RecruitmentRole = 12,

        [XLTemplateName("XL_SAPPGRGRCRole")]
        [XLTemplateFriendlyName("SAP GRC Role Provisioning Matrix")]
        SAP_PGR_GRC_Role = 13,

        [XLTemplateName("XL_SAPPIPSAPPIRole")]
        [XLTemplateFriendlyName("SAP PIPSAPPIRole Provisioning Matrix")]
        SAP_PIP_SAP_PI_Role = 14,

        [XLTemplateName("XL_SAPPLRELearningRole")]
        [XLTemplateFriendlyName("SAP ELearning Role Provisioning Matrix")]
        SAP_PLR_E_Learning_Role = 15,

        [XLTemplateName("XL_SAPRoleBasedOnEEGroup")]
        [XLTemplateFriendlyName("SAP RoleBased On EEGroup Provisioning Matrix")]
        SAP_RoleBasedOnEEGroup = 16,

        [XLTemplateName("XL_SAPRoleBasedOnOrgMgt")]
        [XLTemplateFriendlyName("SAP RoleBased On Org Management Provisioning Matrix")]
        SAP_RoleBasedOnOrgMgt = 17,

        [XLTemplateName("XL_SAPRoleBasedOnPosition")]
        [XLTemplateFriendlyName("SAP RoleBased On Position Provisioning Matrix")]
        SAP_RoleBasedOnPosition = 18,


        [XLTemplateName("XL_Inhouse_UserInfo")]
        [XLTemplateFriendlyName("Inhouse UserInfo Provisioning Matrix")]
        Inhouse_UserInfo = 19,

        [XLTemplateName("XL_InhousePermissions")]
        [XLTemplateFriendlyName("Inhouse Permissions Provisioning Matrix")]
        Inhouse_Permissions = 20,

        [XLTemplateName("XL_GemsRoleMatrix")]
        [XLTemplateFriendlyName("GEMS Role Provisioning Matrix")]
        GEMSRoleMatrix = 21,

        [XLTemplateName("XL_LNSMailConfig")]
        [XLTemplateFriendlyName("LNS Mail Config Provisioning Matrix")]
        LNSMailConfig = 23,

        [XLTemplateName("XL_VPNConfigurations")]
        [XLTemplateFriendlyName("VPN Configurations")]
        VPNConfigurations = 24,

        [XLTemplateName("XL_NPACConfigurations")]
        [XLTemplateFriendlyName("NPAC Section Mapping")]
        NPACConfigurations = 25,

        [XLTemplateName("XL_SAP_HRFLINSConflictedBusinessRoles")]
        [XLTemplateFriendlyName("SAP FLINS Conflict Business Roles Matrix")]
        SAP_HRFLINSConflictedBusinessRoles = 26,

        [XLTemplateName("XL_SAPBIRoleCategories")]
        [XLTemplateFriendlyName("SAP BI Role Categories Provisioning Matrix")]
        SAP_BIRoleCategories = 27,

        [XLTemplateName("XL_SAPEPGroupInfo")]
        [XLTemplateFriendlyName("SAP EP Group Provisioning Matrix")]
        SAP_EPGroupInfo = 28,

        [XLTemplateName("XL_CEGroupInfo")]
        [XLTemplateFriendlyName("CE Group Provisioning Matrix")]
        CE_GroupInfo = 29,

        [XLTemplateName("XL_IPGroupInfo")]
        [XLTemplateFriendlyName("IP Group Provisioning Matrix")]
        IP_GroupInfo = 30,

        [XLTemplateName("XL_SAPEPRoleInfo")]
        [XLTemplateFriendlyName("SAP EP Role Provisioning Matrix")]
        SAP_EPRoleInfo = 31,

        [XLTemplateName("XL_CERoleInfo")]
        [XLTemplateFriendlyName("CE Role  Provisioning Matrix")]
        CE_RoleInfo = 32,

        [XLTemplateName("XL_IPRoleInfo")]
        [XLTemplateFriendlyName("IP Role Provisioning Matrix")]
        IP_RoleInfo = 33,

        [XLTemplateName("NPACDesignation")]
        [XLTemplateFriendlyName("NPAC Designation Matrix")]
        NPAC_Designation = 34,

        [XLTemplateName("XL_ApprovalException")]
        [XLTemplateFriendlyName("Approval Exception")]
        ApprovalException = 35,

        [XLTemplateName("XL_Offic365License")]
        [XLTemplateFriendlyName("Office365 License")]
        Office365License = 36,

        [XLTemplateName("XL_SMSProvisioningMatrix")]
        [XLTemplateFriendlyName("SMS Provisioning Matrix")]
        SMSProvisioningMatrix = 37,

        [XLTemplateName("XL_SMSRoleCatalogue")]
        [XLTemplateFriendlyName("SMS Role Catalogue")]
        SMSRoleCatalogue = 38,

        [XLTemplateName("XL_SAPSRMEnablerRoleMatrix")]
        [XLTemplateFriendlyName("SAP SRM Enabler Role Matrix")]
        SAPSRMEnablerRoleMatrix = 39,

        [XLTemplateName("XL_SAPSRMMainRoleMatrix")]
        [XLTemplateFriendlyName("SAP SRM Main Role Matrix")]
        SAPSRMMainRoleMatrix = 40,

        [XLTemplateName("XL_SAPSRMInternalPortalRole")]
        [XLTemplateFriendlyName("SAP SRM Internal Portal Role")]
        SAPSRMInternalPortalRole = 41,

        [XLTemplateName("XL_SAPSRMInternalPortalGroup")]
        [XLTemplateFriendlyName("SAP SRM Internal Portal Group")]
        SAPSRMInternalPortalGroup = 42,

        [XLTemplateName("XL_SAPSRMExternalPortalRole")]
        [XLTemplateFriendlyName("SAP SRM External Portal Role")]
        SAPSRMExternalPortalRole = 43,

        [XLTemplateName("XL_SAPSRMExternalPortalGroup")]
        [XLTemplateFriendlyName("SAP SRM External Portal Group")]
        SAPSRMExternalPortalGroup = 44,

        [XLTemplateName("XL_SAPSRMBusinessRoleMapping")]
        [XLTemplateFriendlyName("SAP SRM Business Role Mapping")]
        SAPSRMBusinessRoleMapping = 45,

        [XLTemplateName("XL_InhouseFunctions")]
        [XLTemplateFriendlyName("Inhouse Functions Provisioning Matrix")]
        Inhouse_Functions = 46,

        [XLTemplateName("XL_NOVAUserRoleMatrix")]
        [XLTemplateFriendlyName("NOVA User Role Matrix")]
        NOVAUserRoleMatrix = 47,

        [XLTemplateName("XL_NOVAManagementRoleMatrix")]
        [XLTemplateFriendlyName("NOVA Management Role Matrix")]
        NOVAManagementRoleMatrix = 48,

        [XLTemplateName("XL_NOVAExceptionalRoleMatrix")]
        [XLTemplateFriendlyName("NOVA Exceptional Role Matrix")]
        NOVAExceptionalRoleMatrix = 49,

        [XLTemplateName("XL_NOVASoDExceptionMatrix")]
        [XLTemplateFriendlyName("NOVA SoD Exception Matrix")]
        NOVASoDExceptionsMatrix = 50,

        [XLTemplateName("XL_InhouseUserPermissionFunctionMatrix")]
        [XLTemplateFriendlyName("Inhouse User Permission Function Matrix")]
        Inhouse_UserPermissionFunctionMatrix = 51,

        [XLTemplateName("XL_NOVANOVACompositeRoleMatrix")]
        [XLTemplateFriendlyName("NOVA Composite Role Matrix")]
        NOVACompositeRoleMatrix = 52,

        [XLTemplateName("XL_SkypeForBusinessConfigurations")]
        [XLTemplateFriendlyName("SkypeForBusiness Configurations")]
        SkypeForBusinessConfigurations = 53,

        [XLTemplateName("XL_SAP_SODConflictedBusinessRoles")]
        [XLTemplateFriendlyName("SAP SOD Conflict Business Roles Matrix")]
        SAP_SODConflictedBusinessRoles = 54,

        [XLTemplateName("XL_SAPHANARoleMatrix")]
        [XLTemplateFriendlyName("SAPHANA Role Provisioning Matrix")]
        SAPHANA_RoleMatrix = 60,
        [XLTemplateName("XL_SAPHANACriticalTCodesMatrix")]
        [XLTemplateFriendlyName("SAPHANA Critical TCodes Provisioning Matrix")]
        SAPHANA_CriticalTCodesMatrix = 61,
        [XLTemplateName("XL_SAPHANAPositionMatrix")]
        [XLTemplateFriendlyName("SAPHANA Position Provisioning Matrix")]
        SAPHANA_PositionMatrix = 62,
    }
}
