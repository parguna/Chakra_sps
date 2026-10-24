namespace DI.Chakra.Connector.PeopleSoftFINOrPROC
{
    public static class Message
    {
        public const string PeopleSoftFINOrPROC_001 = "Discovery Process";
        public const string PeopleSoftFINOrPROC_002 = "System";
        public const string PeopleSoftFINOrPROC_100 = "Started PSFinance Connector Configuration";
        public const string PeopleSoftFINOrPROC_101 = "PSFinance Connector Configuration Established SuccessFully.";
        public const string PeopleSoftFINOrPROC_102 = "Error occured while Establishing the Configuration (PSFinance Connector Method).";
        public const string PeopleSoftFINOrPROC_103 = "Started PSFinance Connection Establish";
        public const string PeopleSoftFINOrPROC_104 = "{0} - Connection Established SuccessFully ";
        public const string PeopleSoftFINOrPROC_105 = "Error occured while Establishing the Connection {0}(connect Method).";
        public const string PeopleSoftFINOrPROC_106 = "{0} - PSFinance users discovery started";
        public const string PeopleSoftFINOrPROC_107 = "{0} - PSFinance users discovery completed";
        public const string PeopleSoftFINOrPROC_108 = "Error occured while processing the DiscoverHRMSUsers Method.";
        public const string PeopleSoftFINOrPROC_109 = "{0} - PSFinance role discovery started";
        public const string PeopleSoftFINOrPROC_110 = "{0} - PSFinance role discovery completed";
        public const string PeopleSoftFINOrPROC_111 = "Error occured while processing the DiscoverRole Method.";
        public const string PeopleSoftFINOrPROC_112 = "{0} - PSFinance userrole discovery started";
        public const string PeopleSoftFINOrPROC_113 = "{0} - PSFinance userrole discovery completed";
        public const string PeopleSoftFINOrPROC_114 = "Error occured while processing the DiscoverUserRole Method.";
        public const string PeopleSoftFINOrPROC_115 = "Started user role connect configuration ";
        public const string PeopleSoftFINOrPROC_116 = "UserRole Configuration Established SuccessFully";
        public const string PeopleSoftFINOrPROC_117 = "Error occured while connecting userrole csv file.";
        public const string PeopleSoftFINOrPROC_118 = "{0} - PSFinance budescription discovery started";
        public const string PeopleSoftFINOrPROC_119 = "{0} - PSFinance budescription completed";
        public const string PeopleSoftFINOrPROC_120 = "Error occured while processing the DiscoverBUDescription Method.";
        public const string PeopleSoftFINOrPROC_121 = "{0} - PSFinance bankdescription discovery started";
        public const string PeopleSoftFINOrPROC_122 = "{0} - PSFinance bankdescription completed";
        public const string PeopleSoftFINOrPROC_123 = "Error occured while processing the DiscoverBankDescription Method.";
        public const string PeopleSoftFINOrPROC_124 = "{0} - PSFinance CCtoOU discovery started";
        public const string PeopleSoftFINOrPROC_125 = "{0} - PSFinance CCtoOU completed";
        public const string PeopleSoftFINOrPROC_126 = "Error occured while processing the DiscoverCCtoOU Method.";
        public const string PeopleSoftFINOrPROC_127 = "{0} - PSFinance OUtoTagging discovery started";
        public const string PeopleSoftFINOrPROC_128 = "{0} - PSFinance OUtoTagging completed";
        public const string PeopleSoftFINOrPROC_129 = "Error occured while processing the DiscoverOUtoTagging Method.";
        public const string PeopleSoftFINOrPROC_130 = "<UserIntegrationDS><InterfaceRequestHeaderDT><SystemID> 102 </SystemID></InterfaceRequestHeaderDT><createUserVALParamDT><EmployeeID>{0}<EmployeeID> <AD - ID>{1}</AD - ID><FirstName>{2}</FirstName ><LastName>{3}</LastName><Email>{4}</Email ><IDAS_Tnx_ID>{5}</IDAS_Tnx_ID></createUserVALParamDT></UserIntegrationDS>";
        public const string PeopleSoftFINOrPROC_131 = "<UserIntegrationDS><InterfaceRequestHeaderDT><SystemID>102</SystemID></InterfaceRequestHeaderDT><updateRoleVALParamDT><AD-ID>{0}</AD-ID><Roles actiontype=Add'><RoleCode>{1}</RoleCode></Roles><Roles actiontype=Remove><RoleCode>{2}</RoleCode></Roles></updateRoleVALParamDT><KioskCampaignIntegrationDS>";
        public const string PeopleSoftFINOrPROC_132 = "<UserIntegrationDS><InterfaceRequestHeaderDT><SystemID>102</SystemID></InterfaceRequestHeaderDT><deleteUserVALParamDT><AD-ID>{0}</AD-ID><IDAS_Tnx_ID>{1}</IDAS_Tnx_ID><deleteUserVALParamDT></UserIntegrationDS>";

        public const string PeopleSoftFINOrPROC_133 = "{0} - Finance Role Defination discovery started";
        public const string PeopleSoftFINOrPROC_134 = "{0} - Finance Role Defination discovery completed";
        public const string PeopleSoftFINOrPROC_135 = "{0} - Procurement Role Defination discovery started";
        public const string PeopleSoftFINOrPROC_136 = "{0} - Procurement Role Defination discovery completed";
        public const string PeopleSoftFINOrPROC_137 = "{0} - Role2Function discovery started";
        public const string PeopleSoftFINOrPROC_138 = "{0} - Role2Function discovery completed";
        public const string FIN_U2R_MANDATORY = "ROLEUSER";
        public const string DELETE_USER2BU_QUERY = "DELETE FROM PeopleSoftFINOrPROCBUDescriptionInfoPeopleSoftFINOrPROCUserInfo";
        public const string INSERT_USER2BU_QUERY = "INSERT INTO PeopleSoftFINOrPROCBUDescriptionInfoPeopleSoftFINOrPROCUserInfo(PeopleSoftFINOrPROCBUDescriptionInfo_Id,PeopleSoftFINOrPROCUserInfo_Id) VALUES({0}, {1})";
        public const string DELETE_USER2BANK_QUERY = "DELETE FROM PeopleSoftFINOrPROCUserInfoPeopleSoftFINOrPROCBankDescriptionInfo";
        public const string INSERT_USER2BANK_QUERY = "INSERT INTO PeopleSoftFINOrPROCUserInfoPeopleSoftFINOrPROCBankDescriptionInfo(PeopleSoftFINOrPROCUserInfo_Id,PeopleSoftFINOrPROCBankDescriptionInfo_Id) VALUES({0}, {1})";
    }
}
