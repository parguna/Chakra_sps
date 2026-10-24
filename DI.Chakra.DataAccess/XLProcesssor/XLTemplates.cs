using System;

namespace DI.Chakra.DataAccess
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

        [XLTemplateName("XL_SAPROLECATALOG")] [XLTemplateFriendlyName("SAP Role Catalog")] SAP_ROLE_CATALOG = 3, 

        [XLTemplateName("XL_SAPPositionMatrix")] [XLTemplateFriendlyName("SAP Position Provisioning Matrix")] SAP_PositionMatrix = 4,

        [XLTemplateName("XL_SAPCriticalTCodesMatrix")] [XLTemplateFriendlyName("SAP Critical TCodes Provisioning Matrix")] SAP_CriticalTCodesMatrix = 5,

        [XLTemplateName("XL_INHOUSE_USERINFO")] [XLTemplateFriendlyName("Inhouse UserInfo Provisioning Matrix")] INHOUSE_USERINFO = 6,  

        [XLTemplateName("XL_INHOUSEPERMISSIONS")] [XLTemplateFriendlyName("Inhouse Permissions Provisioning Matrix")] INHOUSE_PERMISSIONS = 7,  

        [XLTemplateName("XL_LNSMailQuotaConfig")] [XLTemplateFriendlyName("LNS Mail Qouta Config Provisioning Matrix")] LNSMailConfig = 8,

        [XLTemplateName("XL_VPNConfigurations")] [XLTemplateFriendlyName("VPN Configurations")] VPNConfigurations = 9,

        [XLTemplateName("XL_ADSCATALOG")] [XLTemplateFriendlyName("Active Directory Catalog")] ADS_CATALOG = 10,

        [XLTemplateName("XL_LNSCATALOG")] [XLTemplateFriendlyName("Lotus Notes Catalog")] LNS_CATALOG = 11
    }
}