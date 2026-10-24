using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector.WMS
{
    class Message
    {

        public const string WMS_101 = "CasinoManagementSystem Connector Connection succeeded.";
        public const string WMS_102 = "CasinoManagementSystem Connector Connection failed.";
        public const string WMS_103 = "File Connector Connection failed.";

        public const string WMS_105 = "Started Discovering CasinoManagementSystem Users";
        public const string WMS_106 = "Completed Discovering CasinoManagementSystem Users";
        public const string WMS_107 = "Started Discovering CasinoManagementSystem Functions";
        public const string WMS_108 = "Completed Discovering CasinoManagementSystem Functions";

        public const string WMS_109 = "Error occured while processing the DiscoverFunctions Method.";
        public const string WMS_110 = "Started Discovering CasinoManagementSystem Roles";
        public const string WMS_111 = "Completed Discovering CasinoManagementSystem Roles";

        public const string WMS_112 = "Error occured while processing the DiscoverRoles Method.";
        public const string WMS_113 = "Started Discovering CasinoManagementSystem PrivillegeAcctTrans";
        public const string WMS_114 = "Completed Discovering CasinoManagementSystem PrivillegeAcctTrans";

        public const string WMS_115 = "Error occured while processing the DiscoverPrivillegeAcctTrans Method.";
        public const string BIZCMS_ERROR_CMS_USERS = "Error occured while processing the Discover Users Method.";
        public const string WMS_104 = "AD ID";
        public const string WMS_116 = "RoleCode";
        public const string WMS_117 = "LastLoginDate";
        public const string WMS_118 = "Delete from WMSUserInfoWMSRoleInfo where WMSUserInfo_Id={0}";
        public const string WMS_119 = "INSERT INTO WMSUserInfoWMSRoleInfo(WMSUserInfo_Id,WMSRoleInfo_Id,EndpointSystemId,Version) VALUES({0},{1},{2},{3})";
        public const string WMS_120 = "GetUsers Failed:";
        public const string WMS_121 = "FunctionName";
        public const string WMS_122 = "System";
        public const string WMS_123 = "FunctionDesc";
        public const string WMS_124 = "GetFunctions Failed :";
        public const string WMS_125 = "FunctionGrp";
        public const string WMS_126 = "RoleDesc";
        public const string WMS_127 = "FunctionCode";
        public const string WMS_128 = "Delete from WMSRoleInfoWMSFunctionInfo where WMSRoleInfo_Id={0}  AND  WMSFunctionInfo_Id={1}";
        public const string WMS_129 = "INSERT INTO WMSRoleInfoWMSFunctionInfo(WMSRoleInfo_Id,WMSFunctionInfo_Id,EndpointSystemId,Version) VALUES({0},{1},{2},{3})";
        public const string WMS_130 = "GetRoles Failed : ";
        public const string WMS_131 = "IDASTnx_ID";
        public const string WMS_132 = "DateTime";
        public const string WMS_133 = "PerformedBy";
        public const string WMS_134 = "AppliedOn";
        public const string WMS_135 = "ActionTaken";
        public const string WMS_136 = "Activity";
        public const string WMS_137 = "GetPrivillegeAcctTrans Failed :";
        public const string WMS_138 = "NULL";
        public const string WMS_139 = "Not able get the Field From CSV File, Field Name {0}.";
        public const string WMS_140 = "<UserIntegrationDS><InterfaceRequestHeaderDT><SystemID> 102 </SystemID></InterfaceRequestHeaderDT><createUserVALParamDT><EmployeeID>{0}<EmployeeID> <AD - ID>{1}</AD - ID><FirstName>{2}</FirstName ><LastName>{3}</LastName><Email>{4}</Email ><IDAS_Tnx_ID>{5}</IDAS_Tnx_ID></createUserVALParamDT></UserIntegrationDS>";
        public const string WMS_141 = "<UserIntegrationDS><InterfaceRequestHeaderDT><SystemID>102</SystemID></InterfaceRequestHeaderDT><updateRoleVALParamDT><AD-ID>{0}</AD-ID><Roles actiontype=Add'><RoleCode>{1}</RoleCode></Roles><Roles actiontype=Remove><RoleCode>{2}</RoleCode></Roles></updateRoleVALParamDT><KioskCampaignIntegrationDS>";
        public const string WMS_142 = "<UserIntegrationDS><InterfaceRequestHeaderDT><SystemID>102</SystemID></InterfaceRequestHeaderDT><deleteUserVALParamDT><AD-ID>{0}</AD-ID><IDAS_Tnx_ID>{1}</IDAS_Tnx_ID><deleteUserVALParamDT></UserIntegrationDS>";

        public const string WMS_143 = "View";
        public const string WMS_144 = "Create";
        public const string WMS_145 = "Modify";
        public const string WMS_146 = "Delete";
        public const string WMS_147 = "Print";
        public const string WMS_148 = "Export";
        public const string WMS_149 = "Create";
        public const string WMS_150 = "RoleDesc";
        public const string WMS_151 = "FunctionalGroup";
        public const string WMS_152 = "RoleCode";
        public const string WMS_153 = "FNCode";
        public const string WMS_154 = "FNStatus";
        public const string WMS_155 = "FunctionDesc";
        public const string WMS_156 = "{0}-{1}";
        public const string WMS_ROLES_MANDATORY_COLS = "0,1,2";
        public const string WMS_USERS_MANDATORY_COLS = "0";
        public const string WMS_PRIV_MANDATORY_COLS = "0,1,2";
        public const string WMS_FUN_MANDATORY_COLS = "0,1,2,3,4,5,6,7,8,9";

        public const string WMS_DELETE_FUNCROLE_QUERY = "DELETE FROM WMSRoleInfoWMSFunctionInfo WHERE EndpointSystemId = {0}";

        public const string WMS_INSERT_FUNCROLE_QUERY = "INSERT INTO WMSRoleInfoWMSFunctionInfo (WMSRoleInfo_Id,WMSFunctionInfo_Id,EndpointSystemId,Version) VALUES({0},{1},{2},{3})";

    }
}
