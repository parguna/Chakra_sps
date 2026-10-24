using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector.BizCMS
{
    class Message
    {

        public const string BizCMS_101 = "CasinoManagementSystem Connector Connection succeeded.";
        public const string BizCMS_102 = "CasinoManagementSystem Connector Connection failed.";
        public const string BizCMS_103 = "File Connector Connection failed.";

        public const string BizCMS_105 = "Started Discovering CasinoManagementSystem Users";
        public const string BizCMS_106 = "Completed Discovering CasinoManagementSystem Users";
        public const string BizCMS_107 = "Started Discovering CasinoManagementSystem Functions";
        public const string BizCMS_108 = "Completed Discovering CasinoManagementSystem Functions";

        public const string BizCMS_109 = "Error occured while processing the DiscoverFunctions Method.";
        public const string BizCMS_110 = "Started Discovering CasinoManagementSystem Roles";
        public const string BizCMS_111 = "Completed Discovering CasinoManagementSystem Roles";

        public const string BizCMS_112 = "Error occured while processing the DiscoverRoles Method.";
        public const string BizCMS_113 = "Started Discovering CasinoManagementSystem PrivillegeAcctTrans";
        public const string BizCMS_114 = "Completed Discovering CasinoManagementSystem PrivillegeAcctTrans";

        public const string BizCMS_115 = "Error occured while processing the DiscoverPrivillegeAcctTrans Method.";
        public const string BIZCMS_ERROR_CMS_USERS = "Error occured while processing the Discover Users Method.";
        public const string BizCMS_104 = "AD ID";
        public const string BizCMS_116 = "RoleCode";
        public const string BizCMS_117 = "LastLoginDate";
        public const string BizCMS_118 = "Delete from BizCMSUserInfoBizCMSRoleInfo where BizCMSUserInfo_Id={0}";
        public const string BizCMS_119 = "INSERT INTO BizCMSUserInfoBizCMSRoleInfo(BizCMSUserInfo_Id,BizCMSRoleInfo_Id,EndpointSystemId,Version) VALUES({0},{1},{2},{3})";
        public const string BizCMS_120 = "GetUsers Failed:";
        public const string BizCMS_121 = "FunctionName";
        public const string BizCMS_122 = "System";
        public const string BizCMS_123 = "FunctionDesc";
        public const string BizCMS_124 = "GetFunctions Failed :";
        public const string BizCMS_125 = "FunctionGrp";
        public const string BizCMS_126 = "RoleDesc";
        public const string BizCMS_127 = "FunctionCode";
        public const string BizCMS_128 = "Delete from BizCMSRoleInfoBizCMSFunctionInfo where BizCMSRoleInfo_Id={0}  AND  BizCMSFunctionInfo_Id={1}";
        public const string BizCMS_129 = "INSERT INTO BizCMSRoleInfoBizCMSFunctionInfo(BizCMSRoleInfo_Id,BizCMSFunctionInfo_Id,EndpointSystemId,Version) VALUES({0},{1},{2},{3})";
        public const string BizCMS_130 = "GetRoles Failed : ";
        public const string BizCMS_131 = "IDASTnx_ID";
        public const string BizCMS_132 = "DateTime";
        public const string BizCMS_133 = "PerformedBy";
        public const string BizCMS_134 = "AppliedOn";
        public const string BizCMS_135 = "ActionTaken";
        public const string BizCMS_136 = "Activity";
        public const string BizCMS_137 = "GetPrivillegeAcctTrans Failed :";
        public const string BizCMS_138 = "NULL";
        public const string BizCMS_139 = "Not able get the Field From CSV File, Field Name {0}.";
        public const string BizCMS_140 = "<UserIntegrationDS><InterfaceRequestHeaderDT><SystemID> 102 </SystemID></InterfaceRequestHeaderDT><createUserVALParamDT><EmployeeID>{0}<EmployeeID> <AD - ID>{1}</AD - ID><FirstName>{2}</FirstName ><LastName>{3}</LastName><Email>{4}</Email ><IDAS_Tnx_ID>{5}</IDAS_Tnx_ID></createUserVALParamDT></UserIntegrationDS>";
        public const string BizCMS_141 = "<UserIntegrationDS><InterfaceRequestHeaderDT><SystemID>102</SystemID></InterfaceRequestHeaderDT><updateRoleVALParamDT><AD-ID>{0}</AD-ID><Roles actiontype=Add'><RoleCode>{1}</RoleCode></Roles><Roles actiontype=Remove><RoleCode>{2}</RoleCode></Roles></updateRoleVALParamDT><KioskCampaignIntegrationDS>";
        public const string BizCMS_142 = "<UserIntegrationDS><InterfaceRequestHeaderDT><SystemID>102</SystemID></InterfaceRequestHeaderDT><deleteUserVALParamDT><AD-ID>{0}</AD-ID><IDAS_Tnx_ID>{1}</IDAS_Tnx_ID><deleteUserVALParamDT></UserIntegrationDS>";

        public const string BizCMS_143 = "View";
        public const string BizCMS_144 = "Create";
        public const string BizCMS_145 = "Modify";
        public const string BizCMS_146 = "Delete";
        public const string BizCMS_147 = "Print";
        public const string BizCMS_148 = "Export";
        public const string BizCMS_149 = "Create";
        public const string BizCMS_150 = "RoleDesc";
        public const string BizCMS_151 = "FunctionalGroup";
        public const string BizCMS_152 = "RoleCode";
        public const string BizCMS_153 = "FNCode";
        public const string BizCMS_154 = "FNStatus";
        public const string BizCMS_155 = "FunctionDesc";
        public const string BizCMS_156 = "{0}-{1}";
        public const string BizCMS_ROLES_MANDATORY_COLS = "0,1,2";
        public const string BizCMS_USERS_MANDATORY_COLS = "0";
        public const string BizCMS_PRIV_MANDATORY_COLS = "0,1,2";
        public const string BizCMS_FUN_MANDATORY_COLS = "0,1,2,3,4,5,6,7,8,9";

        public const string BizCMS_DELETE_FUNCROLE_QUERY = "DELETE FROM BizCMSRoleInfoBizCMSFunctionInfo WHERE EndpointSystemId = {0}";

        public const string BizCMS_INSERT_FUNCROLE_QUERY = "INSERT INTO BizCMSRoleInfoBizCMSFunctionInfo (BizCMSRoleInfo_Id,BizCMSFunctionInfo_Id,EndpointSystemId,Version) VALUES({0},{1},{2},{3})";

    }
}
