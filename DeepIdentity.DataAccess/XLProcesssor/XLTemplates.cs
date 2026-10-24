using System;

namespace DeepIdentity.DataAccess
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
        [XLTemplateName("XL_ADSPROVISIONINGMATRIX")] [XLTemplateFriendlyName("Active Directory Provisioning Matrix")] ADS_PROVISIONINGMATRIX = 0,

        [XLTemplateName("XL_LNSPROVISIONINGMATRIX")] [XLTemplateFriendlyName("Lotus Notes Provisioning Matrix")] LNS_PROVISIONINGMATRIX = 1,

        [XLTemplateName("XL_SAPPROVISIONINGMATRIX")] [XLTemplateFriendlyName("SAP Provisioning Matrix")] SAP_PROVISIONINGMATRIX = 2,

        [XLTemplateName("XL_SAPRoleCatalog")] [XLTemplateFriendlyName("SAP Role Catalog")] SAP_Role_Catalog = 3,

        [XLTemplateName("XL_SAPPositionMatrix")] [XLTemplateFriendlyName("SAP Position Provisioning Matrix")] SAP_PositionMatrix = 4,

        [XLTemplateName("XL_SAPCriticalTCodesMatrix")] [XLTemplateFriendlyName("SAP Critical TCodes Provisioning Matrix")] SAP_CriticalTCodesMatrix = 5,


        [XLTemplateName("XL_Inhouse_UserInfo")] [XLTemplateFriendlyName("Inhouse UserInfo Provisioning Matrix")] Inhouse_UserInfo = 6,

        [XLTemplateName("XL_InhousePermissions")] [XLTemplateFriendlyName("Inhouse Permissions Provisioning Matrix")] Inhouse_Permissions = 7,

        [XLTemplateName("XL_LNSMailQuotaConfig")] [XLTemplateFriendlyName("LNS Mail Qouta Config Provisioning Matrix")] LNSMailConfig = 8,

        [XLTemplateName("XL_VPNConfigurations")] [XLTemplateFriendlyName("VPN Configurations")] VPNConfigurations = 9
    }
}